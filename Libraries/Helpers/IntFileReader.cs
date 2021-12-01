namespace Helpers
{
    public class IntFileReader : FileReader<int>
    {
        #region Instance Methods

        protected override int ProcessLineOfFile(string line)
        {
            return int.Parse(line);
        }

        #endregion
    }
}