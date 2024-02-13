using System;

namespace AoC2023_Exec.Exceptions
{
    public class InvalidWorkerException : Exception
    {
        public InvalidWorkerException(Type workerType)
            : base($"Instance of type {workerType.FullName} is not an IWorker!")
        { }
    }
}