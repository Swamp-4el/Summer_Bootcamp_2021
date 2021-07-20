using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApplication1.Contexts;
using WebApplication1.Models.DBModels;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class InvitationController : Controller
    {
        private readonly MessengerDBContext _dbContext;

        public InvitationController(MessengerDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public IActionResult Invite(int? id)
        {
            if(id is null)
            {
                return NotFound();
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var friend = _dbContext.Users.FirstOrDefault(f => f.Id == id);

            if(user is null || friend is null)
            {
                return NotFound();
            }

            if(_dbContext.UserFriends
                .Any(x => (x.UserId == user.Id && x.FriendId == friend.Id)
                       || (x.FriendId == user.Id && x.UserId == friend.Id)))
            {
                return BadRequest();
            }

            _dbContext.UserFriends.Add(new UserFriend
            {
                FriendId = friend.Id,
                UserId = user.Id,
                IsFriends = false,
            });

            _dbContext.SaveChanges();

            return View();
        }

        [HttpGet]
        public IActionResult Confirm(int? id)
        {
            if(id is null)
            {
                return NotFound();
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var request = _dbContext.UserFriends.FirstOrDefault(x => x.UserId == id && x.FriendId == user.Id);

            if (request is null)
            {
                return NotFound();
            }

            request.IsFriends = true;
            _dbContext.Chats.Add(new Chat
            {
                UserId = user.Id,
                FriendId = id.Value,
            });

            _dbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}
