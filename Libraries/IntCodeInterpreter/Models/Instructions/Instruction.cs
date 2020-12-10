namespace IntCodeInterpreter.Models.Instructions
{
    public abstract class Instruction
    {
        #region Instance Properties

        public abstract int InstructionPointerIncrement { get; }

        public abstract OpCode OpCode { get; }

        #endregion
    }
}