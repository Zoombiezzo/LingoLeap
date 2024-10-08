using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace _Client.Scripts.Infrastructure.Services.AssetManagement.AddressablesService
{
    public class AddressablesService : IAddressablesService
    {
        private readonly Dictionary<object, AsyncOperationHandle> _loadedAssetsHandlers = new(16);
        
        public IEnumerator Initialize(Action<float> onProgress = null)
        {
            var asyncOp = UnityEngine.AddressableAssets.Addressables.InitializeAsync();

            while ( asyncOp.IsDone == false )
            {
                var progress = asyncOp.GetDownloadStatus().Percent;
                onProgress?.Invoke(progress);
                
                yield return null;
            }
            
#if UNITY_EDITOR
            Debug.Log( "[AssetBundle]: Initialized..." );
#endif
        }

        public IEnumerator CheckAndUpdateBundles(Action<float> onProgress = null)
        {
#if UNITY_EDITOR
            Debug.Log( "[AssetBundle]: Check and Download Updates..." );
#endif

            var marker = 0f;
            var progress = 0f;
            var catalogsToUpdate = new List<string>( 1024 );
            var updateKeys = new List<object>( 1024 );
            var locations = new List<IResourceLocation>( 1024 );
            var needDownloadSize = 0L;
            var updateCompleted = false;

            var checkForCatalogUpdatesHandle = UnityEngine.AddressableAssets
                .Addressables.CheckForCatalogUpdates( false );
            while ( checkForCatalogUpdatesHandle.IsDone == false )
            {
                var downloadStatus = checkForCatalogUpdatesHandle.GetDownloadStatus();

                progress = marker + downloadStatus.Percent * .1f;

                onProgress?.Invoke(progress);
                
                yield return null;
            }

            marker = progress;

            if ( checkForCatalogUpdatesHandle.Status == AsyncOperationStatus.Succeeded )
            {
                catalogsToUpdate.AddRange( checkForCatalogUpdatesHandle.Result );
#if UNITY_EDITOR
                Debug.Log( $"[AssetBundle]: Total {catalogsToUpdate.Count} updates found" );
                foreach ( var catalog in catalogsToUpdate )
                    Debug.Log( $"[AssetBundle]: Update for \"{catalog}\" exists" );
#endif
            }

            UnityEngine.AddressableAssets
                .Addressables.Release( checkForCatalogUpdatesHandle );

            if ( catalogsToUpdate.Count > 0 )
            {
                foreach ( var catalog in catalogsToUpdate )
                {
#if UNITY_EDITOR
                    Debug.Log( $"[AssetBundle]: Cleaning for \"{catalog}\"..." );
#endif
                    var clearDependencyCacheHandle = UnityEngine.AddressableAssets
                        .Addressables.ClearDependencyCacheAsync( catalog, false );
                    yield return clearDependencyCacheHandle;
#if UNITY_EDITOR
                    Debug.Log( $"[AssetBundle]: Clean is {clearDependencyCacheHandle.Result}" );
#endif
                    UnityEngine.AddressableAssets
                        .Addressables.Release( clearDependencyCacheHandle );
                }

                var updateCatalogsHandle = UnityEngine.AddressableAssets
                    .Addressables.UpdateCatalogs( catalogsToUpdate, false );
                while ( updateCatalogsHandle.IsDone == false )
                {
                    var downloadStatus = updateCatalogsHandle.GetDownloadStatus();

                    progress = marker + downloadStatus.Percent * .2f;
                    
                    onProgress?.Invoke(progress);

                    yield return null;
                }

                marker = progress;

                if ( updateCatalogsHandle.Status == AsyncOperationStatus.Succeeded )
                {
                    foreach ( var resourceLocator in updateCatalogsHandle.Result )
                    {
                        updateKeys.AddRange( resourceLocator.Keys );
#if UNITY_EDITOR
                        Debug.Log( "[AssetBundle]: Update for locator ID " +
                                  $"\"{resourceLocator.LocatorId}\"" );
#endif
                    }
                }

                UnityEngine.AddressableAssets.Addressables.Release( updateCatalogsHandle );
#if UNITY_EDITOR
                Debug.Log( $"[AssetBundle]: {updateKeys.Count} keys need updating" );
#endif
                if ( updateKeys.Count > 0 )
                {
                    var loadResourceLocationsHandle = UnityEngine.AddressableAssets
                        .Addressables.LoadResourceLocationsAsync( ( IEnumerable ) updateKeys,
                        UnityEngine.AddressableAssets.Addressables.MergeMode.Union );
                    while ( loadResourceLocationsHandle.IsDone == false )
                    {
                        var downloadStatus = loadResourceLocationsHandle.GetDownloadStatus();

                        progress = marker + downloadStatus.Percent * .2f;
                        
                        onProgress?.Invoke(progress);

                        yield return null;
                    }

                    marker = progress;

                    if ( loadResourceLocationsHandle.Status == AsyncOperationStatus.Succeeded )
                        locations.AddRange( loadResourceLocationsHandle.Result );

                    UnityEngine.AddressableAssets
                        .Addressables.Release( loadResourceLocationsHandle );

                    if ( locations.Count > 0 )
                    {
                        var getDownloadSizeHandle = UnityEngine.AddressableAssets
                            .Addressables.GetDownloadSizeAsync( locations );
                        yield return getDownloadSizeHandle;

                        if ( getDownloadSizeHandle.Status == AsyncOperationStatus.Succeeded )
                            needDownloadSize = getDownloadSizeHandle.Result;
#if UNITY_EDITOR
                        Debug.Log( $"[AssetBundle]: Download required {needDownloadSize} bytes" );
#endif
                        UnityEngine.AddressableAssets
                            .Addressables.Release( getDownloadSizeHandle );

                        if ( needDownloadSize > 0L )
                        {
                            var downloadDependenciesHandle = UnityEngine.AddressableAssets
                                .Addressables.DownloadDependenciesAsync( locations, false );
                            while ( downloadDependenciesHandle.IsDone == false )
                            {
                                var downloadStatus = downloadDependenciesHandle.GetDownloadStatus();

                                progress = marker + downloadStatus.Percent * .5f;
                                
                                onProgress?.Invoke(progress);

                                yield return null;
                            }

                            marker = progress;

                            UnityEngine.AddressableAssets
                                .Addressables.Release( downloadDependenciesHandle );
                        }

                        updateCompleted = true;
                    }
                }
            }

            if ( updateCompleted == false )
            {
                progress = 1f;
                marker = 1f;
                
                onProgress?.Invoke(progress);

                yield return null;
            }

#if UNITY_EDITOR
            Debug.Log( "[AssetBundle]: Check and update completed!" );
#endif
        }

        public async Task<List<T>> LoadAll<T>(string label)
        {
            List<T> data = new List<T>();
            
            var handle = Addressables.LoadAssetsAsync<T>(label, null);
            
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                data.AddRange(handle.Result);
            }

            //Addressables.Release(handle);
            
            return data;
        }

        public async Task<T> Load<T>(string path)
        {
            var handle =  Addressables.LoadAssetAsync<T>(path);
            
            await handle.Task;
            
            return handle.Result;
        }

        public async Task<T> Load<T>(object reference)
        {
            Release(reference);
            
            var handle =  Addressables.LoadAssetAsync<T>(reference);
            
            _loadedAssetsHandlers.Add(reference, handle);
            
            await handle.Task;
            
            return handle.Result;
        }

        public void Release(object reference)
        {
            if (_loadedAssetsHandlers.TryGetValue(reference, out var handler))
            {
                Addressables.Release(handler);
                _loadedAssetsHandlers.Remove(reference);
            }
        }
    }
}