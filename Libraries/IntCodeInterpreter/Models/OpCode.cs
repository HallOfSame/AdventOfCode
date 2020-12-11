namespace IntCodeInterpreter.Models
{
    public enum OpCode
    {
        Unknown = -1,

        Add = 1,

        Multiply = 2,

        Input = 3,

        Output = 4,

        EndExecution = 99
    }
}