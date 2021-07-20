using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Contexts;
using WebApplication1.Models.DBModels;
using WebApplication1.Models.ViewModels;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly MessengerDBContext _dbContext;
        private readonly IEncryptionService _encryptionService; 

        public ChatController(MessengerDBContext dBContext, IEncryptionService encryptionService)
        {
            _dbContext = dBContext;
            _encryptionService = encryptionService;
        }

        [HttpGet]
        public IActionResult Index(int? id)
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

            var userFriend = _dbContext.UserFriends
                .FirstOrDefault(x => (x.UserId == user.Id && x.FriendId == friend.Id) 
                                  || (x.FriendId == user.Id && x.UserId == friend.Id));

            if (userFriend is null)
            {
                return RedirectToAction("Invite", "Invitation", new { id = id });
            }

            if (!userFriend.IsFriends)
            {
                return RedirectToAction("ErrorChating", "Chat");
            }

            var chat = _dbContext.Chats
                .FirstOrDefault(x => (x.UserId == user.Id && x.FriendId == friend.Id)
                                  || (x.FriendId == user.Id && x.UserId == friend.Id));

            return View(new ChatViewModel
            {
                Id = chat.Id,
                FriendId = friend.Id,
                Messages = GetMessages(chat.Id, user, friend),
                NewMessage = new MessageViewModel(),
            });
        }

        [HttpPost]
        public IActionResult SendMessage(ChatViewModel model)
        {
            var users = _dbContext.Users.ToList();

            var user = users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var friend = users.FirstOrDefault(u => u.Id == model.FriendId);

            var chat = _dbContext.Chats
                .FirstOrDefault(x => (x.UserId == user.Id && x.FriendId == friend.Id)
                                  || (x.FriendId == user.Id && x.UserId == friend.Id));

            var message = new Message
            {
                ChatId = chat.Id,
                SenderId = user.Id,
                RecipientId = model.FriendId,
                Data = model.NewMessage.Message.Data,
                UpdateDate = DateTime.Now,
            };

            _encryptionService.Encrypt(message, user.Key);
            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();

            return RedirectToAction("Index", new { id = friend.Id });
        }

        [HttpGet]
        public IActionResult ErrorChating()
        {
            return View();
        }

        private List<MessageViewModel> GetMessages(int idChat, User user, User friend)
        {
            return _dbContext.Messages.Where(m => m.ChatId == idChat).Select(message => new MessageViewModel
            {
                Sender = new ParticipantViewModel
                {
                    Id = message.Id,
                    UserName = message.SenderId == user.Id ? user.UserName : friend.UserName,
                },
                Recipient = new ParticipantViewModel
                {
                    Id = message.RecipientId,
                    UserName = message.RecipientId == user.Id ? user.UserName : friend.UserName,
                },
                Message = _encryptionService.Decrypt(message, GetKey(message.SenderId, user, friend)), 
            }).ToList();
        }

        private static int GetKey(int senderId, User user, User friend)
        {
            return senderId == user.Id ? user.Key : friend.Key;
        }
    }
}
