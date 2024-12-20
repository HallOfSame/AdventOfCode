using System.Collections.Generic;
using Helpers.Maps;

namespace Helpers.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool IsCharAtCoordinate(this Dictionary<Coordinate, char> map, Coordinate coordinate, char targetChar)
        {
            return map.TryGetValue(coordinate, out var atCoordinate) && atCoordinate == targetChar;
        }
    }
}
