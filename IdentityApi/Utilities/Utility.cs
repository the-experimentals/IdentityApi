using System;
using System.Security.Cryptography;
using System.Text;
using IdentityApi.Account;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace IdentityApi.Utilities;

public class Utility
{
    public const int OTP_LENGTH = 6;

    public static UserSecret GetUserSecret(byte[] salt, string password)
    {
        if (salt == null) // create new salt
        {
            // generate a 128-bit salt using a secure PRNG
            salt = new byte[128 / 8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
        }

        // derive a 256-bit subkey (use HMACSHA256 with 10,000 iterations)
        var secretHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA256,
            10000,
            256 / 8));

        UserSecret userSecret = new() { SALT = salt, SECRET_HASH = secretHash };

        return userSecret;
    }

    public static String ComputeSHA(string plainText)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainText));

        StringBuilder sb = new();
        for (var i = 0; i < bytes.Length; i++)
        {
            sb.Append(bytes[i].ToString("x2"));
        }

        return sb.ToString();
    }

    public static string GetUniqueString(int maxSize)
    {
        var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        var data = new byte[1];
        using var crypto = RandomNumberGenerator.Create();


        crypto.GetNonZeroBytes(data); //produces wild sequences of numbers that are NOT reproducible.
        data = new byte[maxSize];
        crypto.GetNonZeroBytes(data);

        StringBuilder result = new(maxSize);
        foreach (var b in data)
        {
            result.Append(chars[b % chars.Length]);
        }

        return result.ToString();
    }
}
