namespace DocumentManagementStore.Common.Core.Exceptions;

public class ViewDoesNotExistException : Exception
{
    public ViewDoesNotExistException(string message)
        : base(message) { }
}
