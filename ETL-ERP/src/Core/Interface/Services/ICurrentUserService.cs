namespace Interface.Services;

public interface ICurrentUserService
{
    long UserId { get; }
    bool IsAuthenticated { get; }
}
