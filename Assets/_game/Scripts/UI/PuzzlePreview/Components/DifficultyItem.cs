using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleDemo.UI
{
    public class DifficultyItem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _piecesText;
        [SerializeField] private Button _button;
        [Space]
        [SerializeField] private GameObject[] _activeOnItemSelectedObjects;

        private System.Action<DifficultyItem> _onClick;
        public int Difficulty { get; private set; }

        void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }

        public void Initialize(int difficulty, int piecesAmount, System.Action<DifficultyItem> onClick)
        {
            if (_piecesText)
                _piecesText.text = piecesAmount.ToString();

            Difficulty = difficulty;
            _onClick = onClick;
        }

        private void OnClick()
        {
            _onClick?.Invoke(this);
        }

        public void SetActive(bool isActive)
        {
            // do any visual feedbacks here
            if (_activeOnItemSelectedObjects != null)
            {
                foreach (var item in _activeOnItemSelectedObjects)
                {
                    if (item == null)
                        continue;

                    item.SetActive(isActive);
                }
            }
        }
    }
}