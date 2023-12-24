using System.Collections.Generic;
using System.Linq;

namespace Helpers.Maps;

public class InfiniteMap
{
    private readonly Dictionary<Coordinate, char> originalMap;

    private readonly decimal xSize;
    private readonly decimal ySize;

    public InfiniteMap(Dictionary<Coordinate, char> originalMap)
    {
        this.originalMap = originalMap;
        var maxX = originalMap.Max(x => x.Key.X);
        var maxY = originalMap.Max(y => y.Key.Y);
        var minX = originalMap.Min(x => x.Key.X);
        var minY = originalMap.Min(y => y.Key.Y);
        xSize = maxX - minX + 1;
        ySize = maxY - minY + 1;
    }

    public char GetMapChar(Coordinate c)
    {
        // Adjust coordinates to get an infinite map
        var x = c.X % xSize;

        var y = c.Y % ySize;

        if (x < 0)
        {
            x = xSize + x;
        }

        if (y < 0)
        {
            y = ySize + y;
        }

        return originalMap[new Coordinate(x, y)];
    }
}