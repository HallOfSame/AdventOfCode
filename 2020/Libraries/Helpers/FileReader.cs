using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Helpers
{
    public abstract class FileReader<TOutput>
    {
        #region Instance Methods

        public async Task<List<TOutput>> ReadInputFromFile()
        {
            await using var file = File.OpenRead("PuzzleInput.txt");
            using var reader = new StreamReader(file);

            var outputs = new List<TOutput>();

            while (!reader.EndOfStream)
            {
                var nextLine = await reader.ReadLineAsync();

                var output = ProcessLineOfFile(nextLine);

                outputs.Add(output);
            }

            return outputs;
        }

        protected abstract TOutput ProcessLineOfFile(string line);

        #endregion
    }
}