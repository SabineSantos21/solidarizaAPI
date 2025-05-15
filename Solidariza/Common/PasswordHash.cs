using System;
using System.Security.Cryptography;

namespace Solidariza.Common
{
    public static class PasswordHash
    {
        private const int SaltSize = 32; // Tamanho do salt em bytes
        private const int KeySize = 32; // Tamanho da chave/hash em bytes
        private const int Iterations = 100000; // Número de iterações para o PBKDF2

        public static string HashPassword(string password)
        {
            var salt = GenerateSalt();

            using (var key = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                var hash = key.GetBytes(KeySize);

                var hashBytes = new byte[SaltSize + KeySize];
                Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashBytes = Convert.FromBase64String(hashedPassword);

            // Recupera salt e hash armazenados
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            var storedHash = new byte[KeySize];
            Array.Copy(hashBytes, SaltSize, storedHash, 0, KeySize);

            // Compara com hash gerado novamente a partir da senha e salt
            var hashToCompare = GetHashFromSaltedPassword(password, salt);

            return storedHash.SequenceEqual(hashToCompare);
        }

        // Este método encapsula o uso do salt, isolando o uso legítimo e seguro
        private static byte[] GetHashFromSaltedPassword(string password, byte[] salt)
        {
            using (var key = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                return key.GetBytes(KeySize);
            }
        }

        private static byte[] GenerateSalt()
        {
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }
}
