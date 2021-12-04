namespace IntCodeInterpreter.Models.Instructions.FlowControl.Jump
{
    public class JumpIfTrueInstruction : JumpInstruction
    {
        #region Constructors

        public JumpIfTrueInstruction(Parameter value,
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
                return OpCode.JumpIfTrue;
            }
        }

        #endregion
    }
}