using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.DBModels;

namespace WebApplication1.Contexts
{
    public class MessengerDBContext : DbContext
    {
        public DbSet<User> Users { get; set; } 
        
        public DbSet<Message> Messages { get; set; }

        public DbSet<UserFriend> UserFriends { get; set; }

        public DbSet<Chat> Chats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFriend>().HasKey(k => new { k.UserId, k.FriendId });
        }

        public MessengerDBContext(DbContextOptions<MessengerDBContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
