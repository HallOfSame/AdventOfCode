namespace Helpers
{
    public class MultilineStringReader : FileReader<string>
    {
        protected override string ProcessLineOfFile(string line)
        {
            return line;
        }
    }
}
