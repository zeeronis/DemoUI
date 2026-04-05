namespace PuzzleDemo
{
    public class CurrencyService
    {
        public CurrencyPack GetBalance(CurrencyType type)
        {
            return new CurrencyPack(type, 100);
        }

        public bool HasBalance(CurrencyType type, float amount)
        {
            return GetBalance(type).amount >= amount;
        }
    }
}
