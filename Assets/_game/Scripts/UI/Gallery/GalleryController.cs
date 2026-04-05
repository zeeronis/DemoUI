using PuzzleDemo.UI.GalleryMessages;
using PuzzleDemo.UI.Types;
using UISystem;

namespace PuzzleDemo.UI
{
    public class GalleryController : Controller
    {
        // gallery service

        protected override void OnCreated()
        {
            Channel.SubscribeToIntent<SelectPuzzleIntent>(OnSelectPuzzle);
        }

        protected override void OnOpenWindowRequest()
        {
            OpenView(new GalleryState
            {
                
            });
        }

        private void OnSelectPuzzle(SelectPuzzleIntent data)
        {
            // Can be implemented as an async call with data return.
            // var exitData = await OpenMOpen<PuzzlePreview, PuzzlePreviewContext, PuzzlePreviewExitData>(...);
            UIFlow.Open<PuzzlePreview, PuzzlePreviewContext>(new PuzzlePreviewContext
            {
                puzzleId = data.puzzleId,
                playCondition = PuzzlePreviewContext.PlayCondition.Free,
                playContidionValue = 0,
            });
        }
    }
}
