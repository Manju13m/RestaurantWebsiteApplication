using System.Security.Cryptography;

namespace RestaurantWebsiteApplication.Password
{
    public class PasswordService
    {
        // Constants for hashing
        private const int SaltSize = 16; // Salt size in bytes
        private const int Iterations = 10000; // Iteration count for PBKDF2

        // Method to generate a salt
        public byte[] GenerateSalt()
        {
            byte[] salt;
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt = new byte[SaltSize]);
            }
            return salt;
        }

        // Method to hash a password with a given salt
        public byte[] HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                return pbkdf2.GetBytes(32); // 256-bit hash
            }
        }

        // Method to verify a password against a stored hash and salt
        public bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            byte[] computedHash = HashPassword(password, storedSalt);
            return computedHash.SequenceEqual(storedHash);
        }
    }
}








/*GenerateSalt() Method:

Uses RNGCryptoServiceProvider to generate a random salt of SaltSize bytes.
HashPassword() Method:

Uses Rfc2898DeriveBytes(PBKDF2) to hash the password using the provided salt and number of iterations (Iterations).
Returns a 256-bit hash.
VerifyPassword() Method:

Computes the hash of the provided password using the stored salt.
Compares the computed hash with the stored hash to verify the password.*/