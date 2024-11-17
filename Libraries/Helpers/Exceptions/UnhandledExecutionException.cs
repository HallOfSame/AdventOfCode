using System;

namespace Helpers.Exceptions;

public class UnhandledExecutionException : Exception
{
    public UnhandledExecutionException(Exception innerException) : base("Unhandled exception during processing",
                                                                        innerException)
    {
    }
}