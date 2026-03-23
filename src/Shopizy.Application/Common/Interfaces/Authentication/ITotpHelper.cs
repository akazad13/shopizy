namespace Shopizy.Application.Common.Interfaces.Authentication;

public interface ITotpHelper
{
    bool VerifyCode(string secret, string code);
}
