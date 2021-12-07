namespace IntCodeInterpreter.Models.Instructions.FlowControl
{
    internal class AdjustRelativeBaseInstruction : Instruction
    {
        public AdjustRelativeBaseInstruction(Parameter amount)
        {
            Amount = amount;
        }

        public override int InstructionPointerIncrement => 2;

        public override OpCode OpCode => OpCode.AdjustRelativeBase;

        public Parameter Amount { get; }
    }
}
