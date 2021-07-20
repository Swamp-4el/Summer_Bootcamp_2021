using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApplication1.Contexts;
using WebApplication1.Models.ViewModels;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly MessengerDBContext _dbContext;
        private readonly IEncryptionService _encryptionService;
        public MessageController(MessengerDBContext dBContext, IEncryptionService encryptionService)
        {
            _dbContext = dBContext;
            _encryptionService = encryptionService;
        }


        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if(id is null)
            {
                return NotFound();
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var message = _dbContext.Messages.FirstOrDefault(m => m.Id == id);

            if(user is null || message is null || message.SenderId != user.Id)
            {
                return NotFound();
            }

            _dbContext.Messages.Remove(message);
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Chat", new { id = message.RecipientId });
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var message = _dbContext.Messages.FirstOrDefault(m => m.Id == id);
            if (message is null || user is null || message.SenderId != user.Id)
            {
                return NotFound();
            }

            return View(new EditMessageViewModel
            {
                Id = message.Id,
                Message = _encryptionService.Decrypt(message, user.Key).Data,
            });
        }

        [HttpPost]
        public IActionResult Edit(EditMessageViewModel model)
        {
            var message = _dbContext.Messages.FirstOrDefault(m => m.Id == model.Id);
            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            message.Data = model.Message;

            _encryptionService.Encrypt(message, user.Key);
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Chat", new { id = message.RecipientId });
        }

    }
}
