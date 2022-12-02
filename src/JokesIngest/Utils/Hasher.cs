using System.Security.Cryptography;
using System.Text;
using System;

namespace JokesIngest.Utils
{
    internal static class Hasher
    {
        public static string HashToBase64(string text) =>
            Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(text)));
    }
}