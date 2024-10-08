namespace _Client.Scripts.Infrastructure.Services.SceneManagement
{
    public interface IScenePresetsService : IService
    {
        void LoadPresets();
        ScenePreset GetPreset(string id);
    }
}