namespace Tasker.Seedwork.AesEncryption
{
    public interface IAesSecurity
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
