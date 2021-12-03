namespace Helpers
{
    public class CharArrayFileReader : FileReader<char[]>
    {
        protected override char[] ProcessLineOfFile(string line)
        {
            return line.ToCharArray();
        }
    }
}
