namespace IntCodeInterpreter.Models.Instructions.FlowControl.Jump
{
    public class JumpIfFalseInstruction : JumpInstruction
    {
        #region Constructors

        public JumpIfFalseInstruction(Parameter value,
                                      Parameter instructionPointer)
            : base(value,
                   instructionPointer)
        {
        }

        #endregion

        #region Instance Properties

        public override OpCode OpCode
        {
            get
            {
                return OpCode.JumpIfFalse;
            }
        }

        #endregion
    }
}