using System.Linq;

namespace Helpers.FileReaders
{
    public class _2DArrayReader : FileReader<int[]>
    {
        #region Instance Methods

        protected override int[] ProcessLineOfFile(string line)
        {
            return line.ToCharArray()
                       .Select(x => (int)char.GetNumericValue(x))
                       .ToArray();
        }

        #endregion
    }
}