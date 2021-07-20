using WebApplication1.Models.DBModels;

namespace WebApplication1.Services
{
    public interface IEncryptionService
    {
        void Encrypt(Message message, int key);

        Message Decrypt(Message message, int key);
    }
}
