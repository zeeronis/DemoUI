using System.Collections.Generic;
using PuzzleDemo.UI.GalleryMessages;
using UISystem;
using UnityEngine;

namespace PuzzleDemo.UI
{
    // TODO: anchors ring buffer for items
    // TODO: use pool for items
    // TODO: improve update state logic
    public class GalleryView : View
    {
        [Header("References")]
        [SerializeField] private Transform _itemsContainer;
        [SerializeField] private GalleryItem _itemPrefab;

        private readonly List<GalleryItem> _items = new();
        protected System.Action<GalleryItem> _onItemClickAction;

        protected override void OnCreated()
        {
            _onItemClickAction = OnItemClick;
            Channel.SubscribeToState<GalleryState>(UpdateState);
        }

        private void UpdateState(GalleryState data)
        {
            ReleaseItems();
            CreateItems(data);
        }

        private void CreateItems(GalleryState data)
        {
            if (_itemsContainer == null || _itemPrefab == null)
                return;

            // get puzzles for data
            for (int i = 0; i < 10; i++)
            {
                var item = Instantiate(_itemPrefab, _itemsContainer);
                item.Initialize(sprite: null, _onItemClickAction);
                _items.Add(item);
            }
        }

        private void ReleaseItems()
        {
            foreach (var item in _items)
            {
                Destroy(item.gameObject);
            }

            _items.Clear();
        }

        private void OnItemClick(GalleryItem item)
        {
            Channel.SendIntent(new SelectPuzzleIntent
            {
                // get puzzle id from item
                puzzleId = "test_puzzle_id",
            });
        }
    }
}
