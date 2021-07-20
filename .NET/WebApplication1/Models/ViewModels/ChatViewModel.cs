using System.Collections.Generic;

namespace WebApplication1.Models.ViewModels
{
    public class ChatViewModel
    {
        public int Id { get; set; }

        public int FriendId { get; set; }

        public IEnumerable<MessageViewModel> Messages { get; set; }

        public MessageViewModel NewMessage { get; set; }
    }
}
