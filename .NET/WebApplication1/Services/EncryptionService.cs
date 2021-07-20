using System.Text;
using WebApplication1.Models.DBModels;

namespace WebApplication1.Services
{
    public class EncryptionService : IEncryptionService
    {
        private string Encrypt(string message, int key)
        {
            var result = new StringBuilder();

            foreach (var symbol in message)
                result.Append(GetEncryptSymbol(symbol, key));

            return result.ToString();
        }

        private static char GetEncryptSymbol(char symbol, int key) => (char)(symbol ^ key);

        public void Encrypt(Message message, int key)
        {
            message.Data = Encrypt(message.Data, key);
        }

        public Message Decrypt(Message message, int key)
        {
            message.Data = Encrypt(message.Data, key);
            return message;
        }
    }
}
