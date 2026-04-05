namespace PuzzleDemo.UI.PuzzlePreviewMessages
{
    public struct ExitIntent
    {
        public int difficultyLevel;
        public Reason reason;

        public enum Reason
        {
            Close = 1,
            Play = 2,
        }
    }
}
