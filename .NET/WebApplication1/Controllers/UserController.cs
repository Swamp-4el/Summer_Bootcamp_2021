using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Contexts;
using WebApplication1.Models.DBModels;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        private readonly MessengerDBContext _dbContext;

        public UserController(MessengerDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<IActionResult> Index(string searchUser)
        {
            var users = _dbContext.Users.Select(u => u);

            if (!string.IsNullOrWhiteSpace(searchUser))
            {
                int identifier = default;
                if (int.TryParse(searchUser, out identifier))
                {
                    users = users.Where(u => u.Identifier == identifier);
                }
                else
                {
                    users = users.Where(u => u.UserName.Contains(searchUser) && u.Visible);
                }
            }
            else
            {
                users = users.Where(u => u.Visible);
            }

            return View(await users.ToListAsync());
        }

        [Authorize]
        public IActionResult DeleteFriend(int? id)
        {
            if(id is null)
            {
                return NotFound();
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var friend = _dbContext.Users.FirstOrDefault(f => f.Id == id);

            if(friend is null)
            {
                return NotFound();
            }

            var userFriend = _dbContext.UserFriends
                .FirstOrDefault(x => (x.FriendId == user.Id && x.UserId == friend.Id)
                                   || (x.UserId == user.Id && x.FriendId == friend.Id));

            if(userFriend is null)
            {
                return NotFound();
            }

            userFriend.IsFriends = false;

            _dbContext.SaveChanges();

            return RedirectToAction("Friends", "User");
        }

        [Authorize]
        public IActionResult Friends(string searchUser)
        {
            var userFriends = _dbContext.UserFriends.ToList();

            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var friendsFirst = userFriends.Where(x => x.UserId == user.Id && x.IsFriends);

            var friendsSecond = userFriends.Where(x => x.FriendId == user.Id 
                                                    && x.FriendId != x.UserId 
                                                    && x.IsFriends);

            var model = new List<FriendsViewModel>();

            GetFriends(model, friendsFirst, false);
            GetFriends(model, friendsSecond, true);

            if (!string.IsNullOrWhiteSpace(searchUser))
            {
                model = model.Where(f => f.UserName.Contains(searchUser)).ToList();
            }

            return View(model.OrderByDescending(x => x.UserName));
        }

        private void GetFriends(List<FriendsViewModel> model, IEnumerable<UserFriend> friends, bool flag)
        {
            foreach (var item in friends)
            {
                var idfriend = flag ? item.UserId : item.FriendId;
                var friend = _dbContext.Users.FirstOrDefault(f => f.Id == idfriend);

                model.Add(new FriendsViewModel
                {
                    FriendId = friend.Id,
                    UserName = friend.UserName,
                });
            }
        }
    }
}
