using Helpers.Maps;

namespace Helpers.Drawing;

public class DrawableCoordinate : Coordinate
{
    public string? Text { get; set; }

    public string? Color { get; set; } = "black";
}