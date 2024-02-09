namespace Shopizy.Application.Common.Interfaces.Authentication;

public interface IPasswordManager
{
    bool Verify(string clearText, string data);
    string CreateHashString(string clearText, int iterations = 10000);
    bool IsLatestHastversion(byte[] data);
    public bool IsLatestHastversion(string data);
}
