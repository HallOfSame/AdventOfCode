using System.Collections.Generic;
using Helpers.Drawing;

namespace Helpers.Interfaces;

public interface IVisualize2d
{
    IEnumerable<DrawableCoordinate> GetCoordinates();
}