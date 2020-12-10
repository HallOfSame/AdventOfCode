namespace IntCodeInterpreter.Models.Instructions.Arithmetic
{
    public class MultiplyInstruction : ArithmeticInstruction
    {
        #region Constructors

        public MultiplyInstruction(Parameter operandOne,
                                   Parameter operandTwo,
                                   Parameter destination)
            : base(operandOne,
                   operandTwo,
                   destination)
        {
        }

        #endregion

        #region Instance Properties

        public override OpCode OpCode
        {
            get
            {
                return OpCode.Multiply;
            }
        }

        #endregion
    }
}