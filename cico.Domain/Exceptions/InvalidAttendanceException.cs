namespace cico.Domain.Exceptions;

public class InvalidAttendanceException : DomainException
{
    public InvalidAttendanceException(string message)
        : base(message)
    {
    }
}
