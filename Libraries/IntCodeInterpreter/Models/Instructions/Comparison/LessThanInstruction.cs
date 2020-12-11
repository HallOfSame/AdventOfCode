namespace IntCodeInterpreter.Models.Instructions.Comparison
{
    public class LessThanInstruction : ComparisonInstruction
    {
        #region Constructors

        public LessThanInstruction(Parameter valueOne,
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
                return OpCode.LessThan;
            }
        }

        #endregion
    }
}