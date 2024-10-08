using _Client.Scripts.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace _Client.Scripts.Infrastructure.Helpers
{
    [RequireComponent(typeof(CanvasScaler))]
    [ExecuteAlways]
    public class CanvasScaleFactorAdapter : MonoBehaviour
    {
        [SerializeField] private Vector2 _targetSize;
        [SerializeField] private float _verticalScaleFactor;
        [SerializeField] private AnimationCurve _verticalSquareScaleFactorCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve _verticalSquareScaleMultiplierCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        [SerializeField] private float _horizontalScaleFactor;
        [SerializeField] private AnimationCurve _horizontalSquareScaleFactorCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve _horizontalSquareScaleMultiplierCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
        [SerializeField] private float _squareScaleFactor;
        
        private CanvasScaler _canvasScaler;

        private void Awake()
        {
            TryGetComponent(out _canvasScaler);
            UpdateCanvasScale();
        }

        private void OnRectTransformDimensionsChange()
        {
            UpdateCanvasScale();
        }

        private void UpdateCanvasScale()
        {
            if (_canvasScaler == null) return;

            // Определяем текущее соотношение сторон
            var currentAspectRatio = Screen.width * 1f / Screen.height;
            
            var targetSize = _targetSize.x < _targetSize.y
                ? new Vector2(_targetSize.y, _targetSize.x)
                : new Vector2(_targetSize.x, _targetSize.y);

            // Соотношение сторон для горизонтального (ширина больше высоты), вертикального и квадратного экранов
            var targetAspectRatioHorizontal = targetSize.x / targetSize.y; // Горизонтальный
            var targetAspectRatioVertical = targetSize.y / targetSize.x; // Вертикальный
            var targetAspectRatioSquare = 1f; // Квадратное соотношение сторон (1:1)

            // Используем интерполяцию для плавного перехода между масштабами:
            float valueAspect;
            float aspect;

            if (currentAspectRatio <= targetAspectRatioSquare) // Между вертикальным и квадратным
            {
                valueAspect = currentAspectRatio.Remap(targetAspectRatioVertical, targetAspectRatioSquare, 0f, 1f);
                valueAspect = _verticalSquareScaleFactorCurve.Evaluate(valueAspect);
                aspect = Mathf.Lerp(_verticalScaleFactor, _squareScaleFactor, valueAspect);
                aspect *= _verticalSquareScaleMultiplierCurve.Evaluate(valueAspect);
            }
            else // Между квадратным и горизонтальным
            {
                valueAspect = currentAspectRatio.Remap(targetAspectRatioSquare, targetAspectRatioHorizontal, 0f, 1f);
                valueAspect = _horizontalSquareScaleFactorCurve.Evaluate(valueAspect);
                aspect = Mathf.Lerp(_squareScaleFactor, _horizontalScaleFactor, valueAspect);
                aspect *= _horizontalSquareScaleMultiplierCurve.Evaluate(valueAspect);
            }

            // Определяем текущие размеры экрана
            var currentSize = new Vector2(Screen.width, Screen.height);

            // Рассчитываем масштабный коэффициент
            var scaleFactorX = currentSize.x / _targetSize.x;
            var scaleFactorY = currentSize.y / _targetSize.y;

            // Выбираем минимальный масштабный коэффициент, чтобы избежать деформации
            var scaleFactor = Mathf.Min(scaleFactorX, scaleFactorY);

            // Применяем финальный масштаб к canvasScaler
            _canvasScaler.scaleFactor = aspect * scaleFactor;
        }

#if UNITY_EDITOR
        [SerializeField]
        private bool _updateInEditMode = true;
        
        protected void Update()
        {
            UpdateCanvasScale();
        }
        private void OnValidate()
        {
            TryGetComponent(out _canvasScaler);
        }

        private void Reset()
        {
            TryGetComponent(out _canvasScaler);
        }
#endif
    }
}