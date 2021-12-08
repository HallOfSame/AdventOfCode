namespace Day10
{
    internal class Map
    {
        public Map(int height, int width)
        {
            Height = height;
            Width = width;

            MapSpaces = new MapSpace[Height, Width];
        }

        public MapSpace[,] MapSpaces { get; }

        public int Height { get; }

        public int Width { get; }

        public List<MapSpace> Asteroids => MapSpaces.Cast<MapSpace>().Where(x => x.HasAsteroid).ToList();

        public bool IsInBounds(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return false;
            }

            return x < Height && y < Width;
        }
    }

    internal class MapSpace
    {
        public bool HasAsteroid { get; init; }

        public bool IsEmpty => !HasAsteroid;

        public int X { get; init; }

        public int Y { get; init; }
    }
}
