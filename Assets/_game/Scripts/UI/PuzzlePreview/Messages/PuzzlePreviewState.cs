using System.Collections.Generic;
using UnityEngine;

namespace PuzzleDemo.UI.PuzzlePreviewMessages
{
    public struct PuzzlePreviewState
    {
        public IReadOnlyCollection<int> piecesAmount;
        public CurrencyPack rewardCurrency;
        public CurrencyPack softCurrency;
        public Sprite puzzleSprite;
    }
}
