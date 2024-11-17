using System;

namespace Helpers.Exceptions;

public class InputLoadException : Exception
{
    public InputLoadException(Exception innerException) : base("Failed to load puzzle input", innerException)
    {
    }
}