namespace IntCodeInterpreter.Models.Instructions.Arithmetic
{
    public class AddInstruction : ArithmeticInstruction
    {
        #region Constructors

        public AddInstruction(Parameter operandOne,
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
                return OpCode.Add;
            }
        }

        #endregion
    }
}