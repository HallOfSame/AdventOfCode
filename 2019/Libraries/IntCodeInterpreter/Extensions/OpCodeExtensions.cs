using IntCodeInterpreter.Models;

namespace IntCodeInterpreter.Extensions
{
    public static class OpCodeExtensions
    {
        #region Class Methods

        public static bool IsArithmetic(this OpCode opCode)
        {
            return opCode == OpCode.Add || opCode == OpCode.Multiply;
        }

        public static bool IsComparison(this OpCode opCode)
        {
            return opCode == OpCode.LessThan || opCode == OpCode.Equals;
        }

        public static bool IsJump(this OpCode opCode)
        {
            return opCode == OpCode.JumpIfFalse || opCode == OpCode.JumpIfTrue;
        }

        #endregion
    }
}