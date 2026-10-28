using System;

namespace REPOShock.src.PiShockLibrary.Exceptions;

internal class PiShockDeviceException : Exception
{
    public PiShockDeviceException(string message, Exception? innerException = null)  : base(message, innerException)
    {
    }
}