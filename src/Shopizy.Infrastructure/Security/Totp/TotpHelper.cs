using System.Security.Cryptography;
using Shopizy.Application.Common.Interfaces.Authentication;

namespace Shopizy.Infrastructure.Security.Totp;

public class TotpHelper : ITotpHelper
{
    public bool VerifyCode(string base32Secret, string code, int windowSize = 1)
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30;
        for (int i = -windowSize; i <= windowSize; i++)
        {
            if (GenerateCode(base32Secret, timestamp + i) == code) return true;
        }
        return false;
    }

    bool ITotpHelper.VerifyCode(string secret, string code) => VerifyCode(secret, code);

    private static string GenerateCode(string base32Secret, long timestamp)
    {
        var key = Base32Decode(base32Secret);
        var timestampBytes = BitConverter.GetBytes(timestamp);
        if (BitConverter.IsLittleEndian) Array.Reverse(timestampBytes);
        using var hmac = new HMACSHA1(key);
        var hash = hmac.ComputeHash(timestampBytes);
        int offset = hash[^1] & 0x0F;
        int code = ((hash[offset] & 0x7F) << 24) | (hash[offset + 1] << 16) | (hash[offset + 2] << 8) | hash[offset + 3];
        return (code % 1_000_000).ToString("D6");
    }

    private static byte[] Base32Decode(string input)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        input = input.ToUpperInvariant().TrimEnd('=');
        var result = new List<byte>();
        int buffer = 0, bitsLeft = 0;
        foreach (char c in input)
        {
            int val = alphabet.IndexOf(c);
            if (val < 0) continue;
            buffer = (buffer << 5) | val;
            bitsLeft += 5;
            if (bitsLeft >= 8) { bitsLeft -= 8; result.Add((byte)(buffer >> bitsLeft)); buffer &= (1 << bitsLeft) - 1; }
        }
        return result.ToArray();
    }
}
