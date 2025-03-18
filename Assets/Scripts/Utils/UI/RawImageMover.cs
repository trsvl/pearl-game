using UnityEngine;
using UnityEngine.UI;

namespace Utils.UI
{
    public class RawImageMover : MonoBehaviour
    {
        [SerializeField] private float _axisX;
        [SerializeField] private float _axisY;

        private RawImage _rawImage;
        private float _width;
        private float _height;


        private void Start()
        {
            _rawImage = GetComponent<RawImage>();
            _width = _rawImage.uvRect.width;
            _height = 1920f / 1080f * _width;
        }

        private void Update()
        {
            _rawImage.uvRect = new Rect(_rawImage.uvRect.position + new Vector2(_axisX, _axisY) * Time.deltaTime,
                new Vector2(_width, _height));
        }
    }
}