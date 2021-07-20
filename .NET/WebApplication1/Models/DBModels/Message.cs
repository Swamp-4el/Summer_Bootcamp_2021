using System;

namespace WebApplication1.Models.DBModels
{
    public class Message
    {
        public int Id { get; set; }

        public int SenderId { get; set; }

        public int RecipientId { get; set; }

        public string Data { get; set; }

        public DateTime UpdateDate { get; set; }

        public int ChatId { get; set; }
    }
}
