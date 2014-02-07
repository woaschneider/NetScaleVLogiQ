using System;
using System.Text;
using System.Security.Cryptography;

namespace HWB.NETSCALE.FRONTEND.WPF
{
    public static class Md5Stuff
    {
        public static string GetMd5Hash(string data)
        {
            var enc = new ASCIIEncoding();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(enc.GetBytes(data));
            var sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }
            return sb.ToString();
        }

        // Verify a hash against a string.
        public static bool VerifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            return false;
        }
    }
}