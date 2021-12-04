namespace IntCodeInterpreter.Models.Instructions.IO
{
    public class InputInstruction : Instruction
    {
        #region Constructors

        public InputInstruction(Parameter destination)
        {
            Destination = destination;
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

        public Parameter Destination { get; }

        #endregion
    }
}