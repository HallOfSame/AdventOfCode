using System.Collections.Generic;

namespace Day08
{
    public class OperationFixer
    {
        #region Fields

        private readonly List<Operation> originalOperations;

        #endregion

        #region Constructors

        public OperationFixer(List<Operation> originalOperations)
        {
            this.originalOperations = originalOperations;
        }

        #endregion

        #region Instance Methods

        public int GetAccumulatorForFixedProgram()
        {
            var emulator = new Emulator();

            var emulatorResult = emulator.RunOperations(originalOperations,
                                                        out var originalHistory);

            if (emulatorResult != Emulator.ResultCode.Deadlock)
            {
                return emulator.AccumulatorValue;
            }


            do
            {
                Operation poppedInstruction;

                // Pop from the original history until we find a jmp or nop
                do
                {
                    poppedInstruction = originalHistory.Pop();
                }
                while (poppedInstruction.OperationType == OperationType.ACC);

                // Flip the popped instruction
                var popIndex = originalOperations.IndexOf(poppedInstruction);

                var newInstruction = new Operation(poppedInstruction.OperationType == OperationType.JMP
                                                       ? OperationType.NOP
                                                       : OperationType.JMP,
                                                   poppedInstruction.Argument);

                // Build an altered instruction list where the only difference from the original is our popped instruction
                var alteredOperations = new List<Operation>(originalOperations)
                                        {
                                            [popIndex] = newInstruction
                                        };

                // Test it out
                // Since we preserve the stack from the original instructions our next loop will try a jmp / nop further back
                emulatorResult = emulator.RunOperations(alteredOperations,
                                                        out _);
            }
            while (emulatorResult != Emulator.ResultCode.ExecutionComplete);

            return emulator.AccumulatorValue;
        }

        #endregion
    }
}