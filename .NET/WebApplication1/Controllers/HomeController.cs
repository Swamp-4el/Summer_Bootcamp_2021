using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Contexts;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly MessengerDBContext _dbContext;

        public HomeController(MessengerDBContext dbContext)
        {
            _dbContext = dbContext;
        }
 
        public IActionResult Index()
        {
            var model = new List<ConfirmInviteViewModel>();

            if (User.Identity.IsAuthenticated)
            {
                var users = _dbContext.Users.ToList();
                var user = users.FirstOrDefault(u => u.UserName == User.Identity.Name);

                if(user is null)
                {
                    return NotFound();
                }

                var invitations = _dbContext.UserFriends.Where(x => x.FriendId == user.Id && !x.IsFriends);

                if (invitations != null)
                {
                    foreach (var invite in invitations)
                    {
                        var invitingUser = users.FirstOrDefault(u => u.Id == invite.UserId);

                        model.Add(new ConfirmInviteViewModel
                        {
                            Id = invitingUser.Id,
                            UserName = invitingUser.UserName,
                        });
                    }
                }
            }
            return View(model);
        }
    }
}
