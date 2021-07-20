namespace WebApplication1.Models.DBModels
{
    public class UserFriend
    {
        public int UserId { get; set; }

        public int FriendId { get; set; }

        public bool IsFriends { get; set; }
    }
}
