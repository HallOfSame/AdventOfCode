namespace IntCodeInterpreter.Models.Instructions.IO
{
    public class OutputInstruction : Instruction
    {
        #region Constructors

        public OutputInstruction(Parameter source)
        {
            Source = source;
        }

        #endregion

        #region Instance Properties

        public override int InstructionPointerIncrement
        {
            get
            {
                return 2;
            }
        }

        public override OpCode OpCode
        {
            get
            {
                return OpCode.Input;
            }
        }

        public Parameter Source { get; }

        #endregion
    }
}