using Helpers;

namespace Day10
{
    public class InputReader : FileReader<int>
    {
        #region Instance Methods

        protected override int ProcessLineOfFile(string line)
        {
            return int.Parse(line);
        }

        #endregion
    }
}