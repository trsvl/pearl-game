using UnityEngine;

namespace Gameplay.Animations
{
    public class UpdateUIPosition
    {
        private readonly Camera _camera;
        private readonly RectTransform _targetRectTransform;
        private readonly Canvas _targetCanvas;
        private readonly RectTransform _canvasRectTransform;


        public UpdateUIPosition(Camera camera, RectTransform targetRectTransform)
        {
            _camera = camera;
            _targetRectTransform = targetRectTransform;
            _targetCanvas = _targetRectTransform.GetComponentInParent<Canvas>();
            _canvasRectTransform = _targetCanvas.GetComponent<RectTransform>();
        }

        public void Update(Vector3 gameObjectViewportPoint)
        {
            Vector3 viewPosition = _camera.WorldToScreenPoint(gameObjectViewportPoint);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRectTransform,
                viewPosition,
                _targetCanvas.worldCamera,
                out Vector2 localPoint
            );

            _targetRectTransform.anchoredPosition = localPoint;
        }
    }
}