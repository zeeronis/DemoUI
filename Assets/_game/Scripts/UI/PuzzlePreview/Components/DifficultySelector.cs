using System.Collections.Generic;
using UnityEngine;

namespace PuzzleDemo.UI
{
    // TODO: use pool
    public class DifficultySelector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _itemsContainer;
        [SerializeField] private DifficultyItem _itemPrefab;

        private readonly List<DifficultyItem> _items = new();
        private System.Action<DifficultyItem> _onItemClickAction;
        public int DifficultyLevel { get; private set; }

        void Awake()
        {
            _onItemClickAction = SelectItem;
        }

        public void Initialize(IReadOnlyCollection<int> piecesAmount)
        {
            Clear();

            if (piecesAmount == null || piecesAmount.Count == 0)
            {
                Debug.LogError("Pieces amount is null or empty");
                return;
            }

            if (_itemsContainer == null || _itemPrefab == null)
            {
                Debug.LogError("Items container or item prefab is null");
                return;
            }

            int difficulty = 0;
            foreach (var amount in piecesAmount)
            {
                var item = Instantiate(_itemPrefab, _itemsContainer);
                item.Initialize(difficulty++, amount, _onItemClickAction);
                _items.Add(item);
            }

            if (_items.Count > 0)
                SelectItem(_items[0]);
        }

        public void Clear()
        {
            foreach (var item in _items)
            {
                Destroy(item.gameObject);
            }

            DifficultyLevel = -1;
            _items.Clear();
        }

        private void SelectItem(DifficultyItem selectedItem)
        {
            DifficultyLevel = selectedItem.Difficulty;

            foreach (var item in _items)
            {
                item.SetActive(item == selectedItem);
            }
        }
    }
}
