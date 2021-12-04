namespace IntCodeInterpreter.Models.Instructions.FlowControl.Jump
{
    public abstract class JumpInstruction : Instruction
    {
        #region Constructors

        protected JumpInstruction(Parameter value,
                                  Parameter instructionPointer)
        {
            Value = value;
            InstructionPointer = instructionPointer;
        }

        #endregion

        #region Instance Properties

        public override int InstructionPointerIncrement
        {
            get
            {
                // Since these instructions update the instruction pointer manually, don't increment by anything
                return 0;
            }
        }

        public Parameter InstructionPointer { get; }

        public Parameter Value { get; }

        #endregion
    }
}