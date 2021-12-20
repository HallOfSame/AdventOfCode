using Helpers.FileReaders;
using Helpers.Maps;
using Helpers.Structure;

using Spectre.Console;

var solver = new Solver(new Day20Problem());

await solver.Solve();

class Day20Problem : ProblemBase
{
    protected override Task<string> SolvePartOneInternal()
    {
        //ImageRenderer.DrawImage(originalImage, "Original");

        var enhancer = new ImageEnhancer(enhancementTemplate);

        var enhancedOnce = enhancer.Enhance(originalImage);

        //ImageRenderer.DrawImage(enhancedOnce, "Once");

        var enhancedTwice = enhancer.Enhance(enhancedOnce);

        //ImageRenderer.DrawImage(enhancedTwice, "Twice");

        return Task.FromResult(enhancedTwice.Pixels.Values.Count(x => x.IsLit).ToString());
    }

    protected override async Task<string> SolvePartTwoInternal()
    {
        throw new NotImplementedException();
    }

    public override async Task ReadInput()
    {
        var strings = await new StringFileReader().ReadInputFromFile();

        enhancementTemplate = strings[0];

        originalImage = new Image();

        var y = 0;

        foreach (var s in strings.Skip(1))
        {
            if (string.IsNullOrEmpty(s))
            {
                continue;
            }

            var charArray = s.ToCharArray();

            for (var x = 0; x < charArray.Length; x++)
            {
                originalImage.Pixels[new Coordinate(x,
                                                    y)] = new Pixel
                                                          {
                                                              IsLit = charArray[x] == '#'
                                                          };
            }

            y++;
        }
    }

    private Image originalImage;

    private string enhancementTemplate;
}

class ImageEnhancer
{
    private readonly char[] enhancementTemplate;

    public ImageEnhancer(string enhancementTemplate)
    {
        this.enhancementTemplate = enhancementTemplate.ToCharArray();
    }

    public Image Enhance(Image inputImage)
    {
        var enhancedImage = new Image();

        // Add one to current max, going further will just be all 0s
        var minX = inputImage.Pixels.Min(x => x.Key.X) - 1;
        var maxX = inputImage.Pixels.Max(x => x.Key.X) + 1;
        var minY = inputImage.Pixels.Min(x => x.Key.Y) - 1;
        var maxY = inputImage.Pixels.Max(x => x.Key.Y) + 1;

        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
            {
                var coordinate = new Coordinate(x, y);

                var centeredPixels = inputImage.GetPixelsCenteredAt(coordinate);

                var binaryValue = Convert.ToInt32(string.Join(string.Empty,
                                                              centeredPixels.Select(x => x.Value)),
                                                  2);

                var newPixelValue = enhancementTemplate[binaryValue];

                enhancedImage.Pixels[coordinate] = new Pixel
                                                   {
                                                       IsLit = newPixelValue == '#'
                                                   };
            }
        }

        // If all 0s become lit, we need to know that in the image for further processing
        if (enhancementTemplate[0] == '#')
        {
            enhancedImage.UntrackedPixelsAreLit = true;
        }

        return enhancedImage;
    }
}

class Image
{
    public bool UntrackedPixelsAreLit { get; set; }

    public Dictionary<Coordinate, Pixel> Pixels { get; } = new();

    public Pixel[] GetPixelsCenteredAt(Coordinate coordinate)
    {
        var pixelArray = new Pixel[9];

        var idx = 0;

        for (var y = coordinate.Y - 1; y <= coordinate.Y + 1; y++)
        {
            for (var x = coordinate.X - 1; x <= coordinate.X + 1; x++)
            {
                pixelArray[idx++] = GetPixelAt(x,
                                               y);
            }
        }

        return pixelArray;
    }

    public Pixel GetPixelAt(int x,
                            int y)
    {
        var destination = new Coordinate(x, y);

        if (!Pixels.TryGetValue(destination,
                                out var pixel))
        {
            Pixels[destination] = new Pixel
                                  {
                                      IsLit = UntrackedPixelsAreLit
                                  };
        }

        return Pixels[destination];
    }
}

static class ImageRenderer
{
    public static void DrawImage(Image image,
                                 string title)
    {
        var coordinates = image.Pixels.Keys.ToArray();

        var minY = coordinates.Min(x => x.Y);
        var maxY = coordinates.Max(x => x.Y);
        var minX = coordinates.Min(x => x.X);
        var maxX = coordinates.Max(x => x.X);

        var xAdjust = minX * -1;
        var yAdjust = minY * -1;

        var canvas = new Canvas(maxX - minX + 1,
                                maxY - minY + 1);

        foreach (var (coordinate2D, pixel) in image.Pixels)
        {
            var color = pixel.IsLit switch
            {
                true => Color.White,
                false => Color.Grey
            };

            canvas.SetPixel(coordinate2D.X + xAdjust,
                            coordinate2D.Y + yAdjust,
                            color);
        }

        var panel = new Panel(canvas)
                    {
                        Header = new PanelHeader(title)
                    };

        AnsiConsole.Write(panel);
    }
}

class Pixel
{
    public bool IsLit { get; init; }

    public int Value
    {
        get
        {
            return IsLit
                       ? 1
                       : 0;
        }
    }
}