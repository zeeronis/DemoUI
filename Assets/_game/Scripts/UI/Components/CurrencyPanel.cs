using TMPro;
using UnityEngine;

namespace PuzzleDemo.UI
{
    // If we want to reduce the amount of boilerplate code, we can inject a config asset with icons here.
    public class CurrencyPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _text;
        // icon

        public void Initialize(CurrencyPack cPack)
        {
            _text.text = cPack.amount.ToString();
        }
    }
}
