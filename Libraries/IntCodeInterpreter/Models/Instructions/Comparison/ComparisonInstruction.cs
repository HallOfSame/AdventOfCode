namespace IntCodeInterpreter.Models.Instructions.Comparison
{
    public abstract class ComparisonInstruction : Instruction
    {
        #region Constructors

        protected ComparisonInstruction(Parameter valueOne,
                                        Parameter valueTwo,
                                        Parameter destination)
        {
            ValueOne = valueOne;
            ValueTwo = valueTwo;
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

        public Parameter ValueOne { get; }

        public Parameter ValueTwo { get; }

        #endregion
    }
}