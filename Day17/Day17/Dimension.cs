using System.Collections.Generic;
using System.Linq;

namespace Day17
{
    public class Dimension
    {
        #region Constructors

        public Dimension(IEnumerable<Coordinate> initialActive)
        {
            ActiveCoordinates = initialActive.ToHashSet();
        }

        #endregion

        #region Instance Properties

        public HashSet<Coordinate> ActiveCoordinates { get; set; }

        public int ActiveCubeCount
        {
            get
            {
                return ActiveCoordinates.Count;
            }
        }

        #endregion
    }
}