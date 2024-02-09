using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Shopizy.Application.Common.Interfaces.Authentication;

namespace Shopizy.Infrastructure.Security.Hashing;

public class PasswordManager : IPasswordManager
{
    private sealed class HashVersion
    {
        public short Version { get; set; }
        public int SaltSize { get; set; }
        public int HashSize { get; set; }
        public KeyDerivationPrf KeyDerivation { get; set; }
    }

    private readonly Dictionary<short, HashVersion> _versions =
        new()
        {
            {
                1,
                new HashVersion
                {
                    Version = 1,
                    KeyDerivation = KeyDerivationPrf.HMACSHA512,
                    HashSize = 256 / 8,
                    SaltSize = 128 / 8
                }
            }
        };

    private HashVersion DefaultVersion => _versions[1];

    public bool Verify(string clearText, string data)
    {
        try
        {
            var dataBytes = Convert.FromBase64String(data);
            return Verify(clearText, dataBytes);
        }
        catch
        {
            return false;
        }
    }

    public string CreateHashString(string clearText, int iterations = default)
    {
        var data = Hash(clearText, iterations);
        return Convert.ToBase64String(data);
    }

    public bool IsLatestHastversion(byte[] data)
    {
        var version = BitConverter.ToInt16(data, 0);
        return version == DefaultVersion.Version;
    }

    public bool IsLatestHastversion(string data)
    {
        var dataBytes = Convert.FromBase64String(data);
        return IsLatestHastversion(dataBytes);
    }

    private byte[] GetRandomBytes(int lengh)
    {
        var data = new byte[lengh];
        using (var randomNumberGen = RandomNumberGenerator.Create())
        {
            randomNumberGen.GetBytes(data);
        }
        return data;
    }

    private byte[] Hash(string clearText, int iterations)
    {
        var currentVersion = DefaultVersion;

        var saltBytes = GetRandomBytes(currentVersion.SaltSize);
        var versionBytes = BitConverter.GetBytes(currentVersion.Version);
        var iterationBytes = BitConverter.GetBytes(iterations);
        var hashBytes = KeyDerivation.Pbkdf2(
            clearText,
            saltBytes,
            currentVersion.KeyDerivation,
            iterations,
            currentVersion.HashSize
        );

        var indexVersion = 0;
        var indexIteration = indexVersion + versionBytes.Length;
        var indexSalt = indexIteration + iterationBytes.Length;
        var indexHash = indexSalt + currentVersion.SaltSize;

        var resultBytes = new byte[
            versionBytes.Length
                + iterationBytes.Length
                + currentVersion.SaltSize
                + currentVersion.HashSize
        ];
        Array.Copy(versionBytes, 0, resultBytes, indexVersion, versionBytes.Length);
        Array.Copy(iterationBytes, 0, resultBytes, indexIteration, iterationBytes.Length);
        Array.Copy(saltBytes, 0, resultBytes, indexSalt, currentVersion.SaltSize);
        Array.Copy(hashBytes, 0, resultBytes, indexHash, currentVersion.HashSize);
        return resultBytes;
    }

    private bool Verify(string clearText, byte[] data)
    {
        //Get the current version and number of iterations
        var currentVersion = _versions[BitConverter.ToInt16(data, 0)];
        var iteration = BitConverter.ToInt32(data, 2);

        //Create the byte arrays for the salt and hash
        var saltBytes = new byte[currentVersion.SaltSize];
        var hashBytes = new byte[currentVersion.HashSize];

        //Calculate the indexes of the salt and the hash
        var indexSalt = 2 + 4; // Int16 (Version) and Int32 (Iteration)
        var indexHash = indexSalt + currentVersion.SaltSize;

        //Fill the byte arrays with salt and hash
        Array.Copy(data, indexSalt, saltBytes, 0, currentVersion.SaltSize);
        Array.Copy(data, indexHash, hashBytes, 0, currentVersion.HashSize);

        //Hash the current clearText with the parameters given via the data
        var verificationHashBytes = KeyDerivation.Pbkdf2(
            clearText,
            saltBytes,
            currentVersion.KeyDerivation,
            iteration,
            currentVersion.HashSize
        );

        //Check if generated hashes are equal
        return hashBytes.SequenceEqual(verificationHashBytes);
    }
}
