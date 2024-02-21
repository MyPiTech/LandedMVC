using Microsoft.AspNetCore.Mvc;
using LandedMVC.Dtos;
using LandedMVC.Models;
using LandedMVC.Services;

namespace LandedMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly ApiService<UserDto> _userApiService;

        public UsersController(ILogger<UsersController> logger, ApiService<UserDto> userApiService)
        {
            _logger = logger;
            _userApiService = userApiService;
        }
        public async Task<IActionResult> IndexAsync(CancellationToken token)
        {
			/*var users = await _userApiService.GetAllAsync(token);
            return View(new DataModel<UserDto> { Data = users });*/

			return View();
		}

        [HttpGet]
        public async Task<IActionResult> UserAsync(int id, CancellationToken token)
        {
            var user = await _userApiService.GetOneAsync(()=> new UserDto { Id = id },token);
            return View(user?.ToModel() ?? default);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAsync(int id, CancellationToken token)
        {
            await _userApiService.DeleteAsync(() => new UserDto { Id = id }, token);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UserAsync(UserModel user, CancellationToken token)
        {
            if (user.Id == 0)
            {
                await _userApiService.AddAsync(user.ToDto(), token);
            }
            else { 
                await _userApiService.EditAsync(user.ToDto(), token); 
            }
            return RedirectToAction("Index");
        }
    }
}
