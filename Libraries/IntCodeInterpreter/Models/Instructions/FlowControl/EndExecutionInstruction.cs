namespace IntCodeInterpreter.Models.Instructions.FlowControl
{
    public class EndExecutionInstruction : Instruction
    {
        #region Instance Properties

        public override int InstructionPointerIncrement
        {
            get
            {
                return 1;
            }
        }

        public override OpCode OpCode
        {
            get
            {
                return OpCode.EndExecution;
            }
        }

        #endregion
    }
}