using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CAASD.Core.Helpers
{
    public static class SecurityHelper
    {
        public static string HashPassword(string password)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new();

            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }

        public static bool VerifyPassword(string password, string hash)
        {
            string hashOfInput = HashPassword(password);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hash) == 0;
        }

        public static string GenerarToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static string GenerarCodigoVerificacion()
        {
            Random random = new();
            return random.Next(100000, 999999).ToString();
        }
    }
}