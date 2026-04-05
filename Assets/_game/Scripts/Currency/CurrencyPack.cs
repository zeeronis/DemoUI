namespace PuzzleDemo
{
    public struct CurrencyPack
    {
        public CurrencyType type;
        public float amount;

        public CurrencyPack(CurrencyType type, float amount)
        {
            this.type = type;
            this.amount = amount;
        }
    }
}
