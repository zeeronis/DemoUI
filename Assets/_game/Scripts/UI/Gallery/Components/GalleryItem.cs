using UnityEngine;
using UnityEngine.UI;

namespace PuzzleDemo.UI
{
    public class GalleryItem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;

        private System.Action<GalleryItem> _onClick;

        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }

        public void Initialize(Sprite sprite, System.Action<GalleryItem> onClick)
        {
            _image.sprite = sprite;
            _onClick = onClick;
        }

        private void OnClick()
        {
            _onClick?.Invoke(this);
        }
    }
}
