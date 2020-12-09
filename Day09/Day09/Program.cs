using System;
using System.Threading.Tasks;

namespace Day09
{
    internal class Program
    {
        #region Class Methods

        public static async Task Main(string[] args)
        {
            var input = await new InputReader().ReadInputFromFile();

            var processor = new XmasCipherProcessor(25);

            // PT 1
            var invalidValue = processor.FindInvalidValue(input);

            Console.WriteLine($"Invalid value in sequence is {invalidValue}.");

            var weakness = processor.GetEncryptionWeakness(input);

            // PT 2
            Console.WriteLine($"Weakness is {weakness}.");
        }

        #endregion
    }
}