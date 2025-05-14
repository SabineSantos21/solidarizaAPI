using System.Security.Cryptography;

namespace Solidariza.Common
{
    public class PasswordHash
    {
        // Parâmetros para PBKDF2
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32;  // 256 bits
        private const int Iterations = 100000;

        public static string HashPassword(string password)
        {
            // Gera um salt aleatório
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Gera o hash usando PBKDF2
            using (var key = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                var hash = key.GetBytes(KeySize);

                // Junta salt + hash para armazenar
                var hashBytes = new byte[SaltSize + KeySize];
                Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);

                // Retorna em Base64
                return Convert.ToBase64String(hashBytes);
            }
        }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashBytes = Convert.FromBase64String(hashedPassword);

            // Recupera o salt e o hash do valor armazenado
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            var hash = new byte[KeySize];
            Array.Copy(hashBytes, SaltSize, hash, 0, KeySize);

            // Gera o hash da senha recebida usando o mesmo salt
            var key = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var hashToCompare = key.GetBytes(KeySize);

            // Compara byte a byte
            return hash.SequenceEqual(hashToCompare);
        }
    }
}
