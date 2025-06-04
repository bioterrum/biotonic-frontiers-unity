using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BiotonicFrontiers.UI.Map
{
    /// <summary>Single 16×16 clickable tile on the land‑map grid.</summary>
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public class LandTile : MonoBehaviour, IPointerClickHandler
    {
        private int   _x;
        private int   _y;
        private Image _image;

        private void Awake() => _image = GetComponent<Image>();

        public void Init(int x, int y, Color color)
        {
            _x = x;
            _y = y;
            _image.color = color;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var mapUI = GetComponentInParent<LandMapUI>();
            mapUI?.HandleTileClick(_x, _y);
        }
    }
}