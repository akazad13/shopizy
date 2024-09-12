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
                    SaltSize = 128 / 8,
                }
            },
        };

    private HashVersion DefaultVersion => _versions[1];

    public bool Verify(string clearText, string data)
    {
        try
        {
            byte[] dataBytes = Convert.FromBase64String(data);
            return Verify(clearText, dataBytes);
        }
        catch
        {
            return false;
        }
    }

    public string CreateHashString(string clearText, int iterations = default)
    {
        byte[] data = Hash(clearText, iterations);
        return Convert.ToBase64String(data);
    }

    public bool IsLatestHastversion(byte[] data)
    {
        short version = BitConverter.ToInt16(data, 0);
        return version == DefaultVersion.Version;
    }

    public bool IsLatestHastversion(string data)
    {
        byte[] dataBytes = Convert.FromBase64String(data);
        return IsLatestHastversion(dataBytes);
    }

    private byte[] GetRandomBytes(int lengh)
    {
        byte[] data = new byte[lengh];
        using (var randomNumberGen = RandomNumberGenerator.Create())
        {
            randomNumberGen.GetBytes(data);
        }
        return data;
    }

    private byte[] Hash(string clearText, int iterations)
    {
        HashVersion currentVersion = DefaultVersion;

        byte[] saltBytes = GetRandomBytes(currentVersion.SaltSize);
        byte[] versionBytes = BitConverter.GetBytes(currentVersion.Version);
        byte[] iterationBytes = BitConverter.GetBytes(iterations);
        byte[] hashBytes = KeyDerivation.Pbkdf2(
            clearText,
            saltBytes,
            currentVersion.KeyDerivation,
            iterations,
            currentVersion.HashSize
        );

        int indexVersion = 0;
        int indexIteration = indexVersion + versionBytes.Length;
        int indexSalt = indexIteration + iterationBytes.Length;
        int indexHash = indexSalt + currentVersion.SaltSize;

        byte[] resultBytes = new byte[
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
        HashVersion currentVersion = _versions[BitConverter.ToInt16(data, 0)];
        int iteration = BitConverter.ToInt32(data, 2);

        //Create the byte arrays for the salt and hash
        byte[] saltBytes = new byte[currentVersion.SaltSize];
        byte[] hashBytes = new byte[currentVersion.HashSize];

        //Calculate the indexes of the salt and the hash
        int indexSalt = 2 + 4; // Int16 (Version) and Int32 (Iteration)
        int indexHash = indexSalt + currentVersion.SaltSize;

        //Fill the byte arrays with salt and hash
        Array.Copy(data, indexSalt, saltBytes, 0, currentVersion.SaltSize);
        Array.Copy(data, indexHash, hashBytes, 0, currentVersion.HashSize);

        //Hash the current clearText with the parameters given via the data
        byte[] verificationHashBytes = KeyDerivation.Pbkdf2(
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
