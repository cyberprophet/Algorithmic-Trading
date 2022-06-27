using System.Security.Cryptography;
using System.Text;

namespace ShareInvest.Security
{
    public static class Crypto
    {
        public static string Encrypt(byte[] key, string param) => Convert.ToBase64String(new HMACSHA512(key).ComputeHash(Encoding.ASCII.GetBytes(param)));
    }
}