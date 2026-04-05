using System.Collections.Generic;
using PuzzleDemo.UI.PuzzlePreviewMessages;
using UISystem;
using UnityEngine;

namespace PuzzleDemo.UI
{
    public class PuzzlePreviewController : Controller
    {
        // TODO: add gallery service
        private readonly CurrencyService _currencyService;
        private readonly List<int> _piecesAmount = new();
        private PuzzlePreviewContext _context;

        public PuzzlePreviewController(CurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        protected override void OnCreated()
        {
            Channel.SubscribeToIntent<PuzzlePreviewContext>(data => _context = data);
            Channel.SubscribeToIntent<ExitIntent>(OnExit);
        }

        protected override void OnOpenWindowRequest()
        {
            if (string.IsNullOrEmpty(_context.puzzleId))
            {
                Debug.LogError("Puzzle id is null");
                return;
            }

            // TODO: Try get puzzle info from gallery service
            Sprite puzzleSprite = null;
            CurrencyPack rewardCurrency = new(CurrencyType.Soft, 120);

            _piecesAmount.Clear();
            _piecesAmount.Add(36);
            _piecesAmount.Add(64);
            _piecesAmount.Add(100);
            _piecesAmount.Add(144);
            _piecesAmount.Add(256);
            _piecesAmount.Add(400);

            OpenView(new PuzzlePreviewState
            {
                softCurrency = _currencyService.GetBalance(CurrencyType.Soft),
                rewardCurrency = rewardCurrency,
                piecesAmount = _piecesAmount,
                puzzleSprite = puzzleSprite,

                // TODO: pass any additional data to the view
            });
        }

        private void OnExit(ExitIntent exit)
        {
            switch (exit.reason)
            {
                default: // log warn
                case ExitIntent.Reason.Close:
                    CloseView();
                    break;

                case ExitIntent.Reason.Play:
                    PlayPuzzle(exit.difficultyLevel);
                    break;
            }
        }

        // TODO: replace if() to abstract condition class with IsMet(...) method.
        private void PlayPuzzle(int difficultyLevel)
        {
            if (_context.playCondition == PuzzlePreviewContext.PlayCondition.Free)
            {
                Debug.LogError("Free play");
                CloseView();
                return;
            }

            if (_context.playCondition == PuzzlePreviewContext.PlayCondition.Soft)
            {
                if (!_currencyService.HasBalance(CurrencyType.Soft, _context.playContidionValue))
                {
                    // TODO: any feedbacks
                    Debug.LogError("Not enough soft currency");
                    return;
                }

                Debug.LogError("Play for soft currency");
                CloseView();
                return;
            }

            if (_context.playCondition == PuzzlePreviewContext.PlayCondition.Rewarded)
            {
                // can show reward? show reward -> play on success callback
                Debug.LogError("Play for rewarded");
                CloseView();
                return;
            }
        }
    }

    public struct PuzzlePreviewContext
    {
        public string puzzleId;

        // TODO: replace with abstract condition class
        public PlayCondition playCondition;
        public float playContidionValue;

        public enum PlayCondition
        {
            Free,
            Soft,
            Rewarded,
        }
    }
}
