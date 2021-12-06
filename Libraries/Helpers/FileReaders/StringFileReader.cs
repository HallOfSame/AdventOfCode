namespace Helpers.FileReaders
{
    public class StringFileReader : FileReader<string>
    {
        protected override string ProcessLineOfFile(string line)
        {
            return line;
        }
    }
}
