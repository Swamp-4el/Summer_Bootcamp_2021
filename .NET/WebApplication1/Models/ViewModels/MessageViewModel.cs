using WebApplication1.Models.DBModels;

namespace WebApplication1.Models.ViewModels
{
    public class MessageViewModel
    {
        public ParticipantViewModel Sender { get; set; }

        public ParticipantViewModel Recipient { get; set; }

        public Message Message { get; set; }
    }
}
