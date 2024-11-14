namespace PuzzleRunnerBlazor.Components
{
    public partial class BlazorCanvas
    {
        public class ImageData
        {
            public int PixelSize { get; set; }
            public Pixel[] Pixels { get; set; }

            public int Height => Pixels.Max(x => x.Y) + 1;
            public int Width => Pixels.Max(x => x.X) + 1;
        }

        public class Pixel
        {
            public int X { get; set; }
            public int Y { get; set; }
            public string Text { get; set; }
            public string Color { get; set; }
        }
    }
}
