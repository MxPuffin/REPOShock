using System;

namespace REPOShock.src.PiShockLibrary.Exceptions;

internal class PiShockAPIException : Exception
{
    public PiShockAPIException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}
