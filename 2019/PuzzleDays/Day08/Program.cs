
using Helpers;
using System.Text;

var inputLayers = (await new LayerReader(6, 25).ReadInputFromFile()).First();

// Part 1

var fewest0Layer = inputLayers.OrderBy(x => x.Values.SelectMany(i => i).Where(i => i == 0).Count()).First();

var layerValues = fewest0Layer.Values.SelectMany(i => i).ToList();

var countOnes = layerValues.Where(i => i == 1).Count();
var countTwos = layerValues.Where(i => i == 2).Count();

Console.WriteLine($"Image check: {countOnes * countTwos}.");

// Part 2

var finalImage = new int[6][];

// Process
for(var row = 0; row < 6; row++)
{
    finalImage[row] = new int[25];

    for (var col = 0; col < 25; col++)
    {
        foreach(var layer in inputLayers)
        {
            // Search until we find a non transparent pixel
            // 0 = black, 1 = white, 2 = transparent
            var pixelInLayer = layer.Values[row][col];

            if (pixelInLayer != 2)
            {
                finalImage[row][col] = pixelInLayer;
                break;
            }
        }
    }
}

// Print

Console.WriteLine("Image: ");
Console.WriteLine(string.Join("", Enumerable.Range(0, 26).Select(x => "_")));

for(var i = 0; i < finalImage.Length; i++)
{
    var row = new StringBuilder();

    row.Append("|");

    for(var j = 0; j < finalImage[i].Length; j++)
    {
        row.Append(finalImage[i][j] == 0 ? "X" : " ");
    }

    row.Append("|");

    Console.WriteLine(row.ToString());
}

Console.WriteLine(string.Join("", Enumerable.Range(0, 26).Select(x => "¯")));

class Layer
{
    public int Height { get; set; }

    public int Width { get; set; }

    public int[][] Values { get; set; }
}

class LayerReader : FileReader<List<Layer>>
{
    private readonly int height;
    private readonly int width;

    public LayerReader(int height, int width)
    {
        this.height = height;
        this.width = width;
    }

    protected override List<Layer> ProcessLineOfFile(string line)
    {
        var rows = Enumerable.Range(0, line.Length / width)
        .Select(i => line.Substring(i * width, width))
        .ToList();

        var layers = rows.Select((v, i) => new { Value = v, Index = i })
        .GroupBy(x => x.Index / height)
        .Select(x => x.Select(y => y.Value).ToList())
        .ToList();

        var result = new List<Layer>();

        foreach(var layer in layers)
        {
            var values = layer.Select(x => x.ToCharArray().Select(str => int.Parse(str.ToString())).ToArray()).ToArray();

            result.Add(new Layer
            {
                Height = height,
                Width = width,
                Values = values
            });
        }

        return result;
    }
}