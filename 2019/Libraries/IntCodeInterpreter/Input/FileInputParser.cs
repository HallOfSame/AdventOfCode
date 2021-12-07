using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IntCodeInterpreter.Input
{
    public class FileInputParser
    {
        #region Instance Methods

        public async Task<List<long>> ReadOperationsFromFile(string fileName)
        {
            var fileText = await File.ReadAllTextAsync(fileName);

            var ints = fileText.Split(',')
                               .Select(long.Parse)
                               .ToList();

            return ints;
        }

        #endregion
    }
}