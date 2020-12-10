namespace IntCodeInterpreter.Models.Instructions.Arithmetic
{
    public abstract class ArithmeticInstruction : Instruction
    {
        #region Constructors

        protected ArithmeticInstruction(Parameter operandOne,
                                        Parameter operandTwo,
                                        Parameter destination)
        {
            OperandOne = operandOne;
            OperandTwo = operandTwo;
            Destination = destination;
        }

        #endregion

        #region Instance Properties

        public override int InstructionPointerIncrement
        {
            get
            {
                return 4;
            }
        }

        public Parameter Destination { get; }

        public Parameter OperandOne { get; }

        public Parameter OperandTwo { get; }

        #endregion
    }
}