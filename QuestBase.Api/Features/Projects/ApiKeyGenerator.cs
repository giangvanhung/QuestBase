using System.Security.Cryptography;
using System.Text;

public static class ApiKeyGenerator
{
    public static string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return $"qb_{Convert.ToHexString(bytes).ToLowerInvariant()}";
    }
    public static string Hash(string key)
    {
        var bytes = Encoding.UTF8.GetBytes(key);
        return Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
    }
}