namespace IntCodeInterpreter.Models.Instructions.Comparison
{
    public class EqualsInstruction : ComparisonInstruction
    {
        #region Constructors

        public EqualsInstruction(Parameter valueOne,
                                 Parameter valueTwo,
                                 Parameter destination)
            : base(valueOne,
                   valueTwo,
                   destination)
        {
        }

        #endregion

        #region Instance Properties

        public override OpCode OpCode
        {
            get
            {
                return OpCode.Equals;
            }
        }

        #endregion
    }
}