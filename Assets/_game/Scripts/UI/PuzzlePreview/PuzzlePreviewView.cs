using PuzzleDemo.UI.PuzzlePreviewMessages;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleDemo.UI
{
    // TODO: use pool for items
    public class PuzzlePreviewView : View
    {
        [Header("References")]
        [SerializeField] private CurrencyPanel _softCurrencyPanel;
        [SerializeField] private TextMeshProUGUI _rewardText;
        [SerializeField] private Image _puzzleImage;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _playButton;
        [Space]
        [SerializeField] private DifficultySelector _difficultySelector;

        [Header("Settings")]
        [SerializeField] private string _rewardTextFormat;

        protected override void OnCreated()
        {
            if (_closeButton)
                _closeButton.onClick.AddListener(OnCloseButton_Click);

            if (_playButton)
                _playButton.onClick.AddListener(OnPlayButton_Click);

            Channel.SubscribeToState<PuzzlePreviewState>(SetState);
        }

        private void SetState(PuzzlePreviewState state)
        {
            if (_puzzleImage)
                _puzzleImage.sprite = state.puzzleSprite;

            if (_rewardText)
                _rewardText.text = !string.IsNullOrEmpty(_rewardTextFormat)
                    ? string.Format(_rewardTextFormat, state.rewardCurrency.amount)
                    : state.rewardCurrency.amount.ToString();

            if (_softCurrencyPanel)
                _softCurrencyPanel.Initialize(state.softCurrency);

            if (_difficultySelector)
                _difficultySelector.Initialize(state.piecesAmount);
        }

        private void OnPlayButton_Click()
        {
            Channel.SendIntent(new ExitIntent
            {
                difficultyLevel = GetDifficultyLevel(),
                reason = ExitIntent.Reason.Play,
            });
        }

        private void OnCloseButton_Click()
        {
            Channel.SendIntent(new ExitIntent
            {
                reason = ExitIntent.Reason.Close,
            });
        }

        private int GetDifficultyLevel()
        {   
            if (_difficultySelector == null)
            {
                Debug.LogError("Difficulty selector is null");
                return 0;
            }

            return _difficultySelector.DifficultyLevel;
        }
    }
}
