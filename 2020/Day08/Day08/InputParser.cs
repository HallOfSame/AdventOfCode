using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Day08
{
    public class InputParser
    {
        #region Instance Methods

        public async Task<List<Operation>> ReadInput(StreamReader streamReader)
        {
            var operations = new List<Operation>();

            while (!streamReader.EndOfStream)
            {
                var nextLine = await streamReader.ReadLineAsync();

                var splitLine = nextLine.Split(' ',
                                               2);

                var operationType = splitLine[0] switch
                {
                    "nop" => OperationType.NOP,
                    "jmp" => OperationType.JMP,
                    "acc" => OperationType.ACC,
                    _ => throw new InvalidDataException($"{splitLine[0]} is not a valid operation type.")
                };

                var argument = int.Parse(splitLine[1]
                                             .Trim());

                operations.Add(new Operation(operationType,
                                             argument));
            }

            return operations;
        }

        #endregion
    }
}