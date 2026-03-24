namespace BankMore.Infrastructure.Auth
{
    public interface IPasswordService
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }

    public class PasswordService : IPasswordService
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
