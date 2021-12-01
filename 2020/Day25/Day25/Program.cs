using System;
using System.IO;
using System.Linq;

namespace Day25
{
    public class Program
    {
        public static void Main(string[] args)
        {
            const long subjectNumber = 7;

            var cardPublicKey = 14012298L;

            var doorPublicKey = 74241L;

            var cardLoopSize = 0;
            var doorLoopSize = 0;

            var currentValue = 1L;

            var loops = 0;

            while (cardLoopSize == 0 || doorLoopSize == 0)
            {
                currentValue = CalculateLoops(currentValue,
                                              subjectNumber,
                                              1);

                loops++;

                if (currentValue == cardPublicKey)
                {
                    cardLoopSize = loops;
                }

                if (currentValue == doorPublicKey)
                {
                    doorLoopSize = loops;
                }
            }


            var encryptionKey = CalculateLoops(1,
                                               doorPublicKey,
                                               cardLoopSize);

            Console.WriteLine($"Encryption key: {encryptionKey}.");
        }

        private static long CalculateLoops(long startingValue,
                                    long subjectNumber,
                                    int numberOfLoops)
        {
            var currentValue = startingValue;

            for (var i = 0; i < numberOfLoops; i++)
            {
                currentValue = currentValue * subjectNumber;
                Math.DivRem(currentValue,
                            20201227L,
                            out currentValue);
            }

            return currentValue;
        }
    }
}
