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

        #endregion
    }
}