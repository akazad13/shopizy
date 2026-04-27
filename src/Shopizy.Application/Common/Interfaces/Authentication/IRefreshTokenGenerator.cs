namespace Shopizy.Application.Common.Interfaces.Authentication;

public interface IRefreshTokenGenerator
{
    string Generate();
    TimeSpan Lifetime { get; }
}
