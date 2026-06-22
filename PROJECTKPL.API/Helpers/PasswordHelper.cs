using System.Security.Cryptography;
using System.Text;

namespace PROJECTKPL.API.Helpers
{
    public static class PasswordHelper
    {
        public static string Hash(string password)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }

        public static bool Verify(string inputPassword, string storedHash)
        {
            string inputHash = Hash(inputPassword);
            return inputHash == storedHash;
        }
    }

}
