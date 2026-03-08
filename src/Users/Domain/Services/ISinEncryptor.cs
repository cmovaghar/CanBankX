namespace Users.Domain.Services
{
    public interface ISinEncryptor
    {
        string Encrypt(string sin);
        string Decrypt(string encryptedSin);
    }
}
