using System;
using Xunit;
using Solidariza.Common;

namespace Solidariza.Tests
{
    public class PasswordHashTests
    {
        [Fact]
        public void HashPassword_Returns_NonNullAndDifferentFromOriginal()
        {
            // Arrange
            string testep = "MySecureTeste123";

            // Act
            var hashed = PasswordHash.HashPassword(testep);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(hashed));
            Assert.NotEqual(testep, hashed);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            string password = "CorrectPassword";
            string hashedPassword = PasswordHash.HashPassword(password);

            // Act
            var result = PasswordHash.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            // Arrange
            string originalPassword = "OriginalPassword";
            string wrongPassword = "WrongPassword";
            string hashedPassword = PasswordHash.HashPassword(originalPassword);

            // Act
            var result = PasswordHash.VerifyPassword(wrongPassword, hashedPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HashPassword_SamePasswordDifferentHashes()
        {
            // Arrange
            string password = "RepeatPassword";

            // Act
            var hash1 = PasswordHash.HashPassword(password);
            var hash2 = PasswordHash.HashPassword(password);

            // Assert
            Assert.NotEqual(hash1, hash2); // Salt aleatório deve gerar hashes diferentes
        }

        [Fact]
        public void VerifyPassword_ThrowsFormatException_WhenHashIsInvalid()
        {
            // Arrange
            string password = "AnyPassword";
            string invalidHash = "NotAValidBase64";

            // Act & Assert
            Assert.Throws<FormatException>(() =>
                PasswordHash.VerifyPassword(password, invalidHash));
        }
    }
}
