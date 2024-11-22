using Helpers.Drawing;

namespace PuzzleRunner.Components.Shared
{
    public partial class BlazorCanvas
    {
        public class ImageData
        {
            public int PixelSize { get; set; }
            public required DrawableCoordinate[] Pixels { get; set; }

            public int Height => Pixels.Max(x => (int)x.Y) + 1;
            public int Width => Pixels.Max(x => (int)x.X) + 1;
        }
    }
}
