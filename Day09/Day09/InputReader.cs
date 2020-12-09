using System;

using Helpers;

namespace Day09
{
    public class InputReader : FileReader<long>
    {
        #region Instance Methods

        protected override long ProcessLineOfFile(string line)
        {
            return long.Parse(line);
        }

        #endregion
    }
}