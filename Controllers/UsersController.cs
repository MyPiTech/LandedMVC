// ***********************************************************************
// Assembly         : LandedMVC
// Author           : Shawn Wheeler
// Created          : 02-20-2024
//
// Last Modified By : Shawn Wheeler
// Last Modified On : 02-21-2024
// ***********************************************************************
// <copyright file="UsersController.cs" company="LandedMVC">
//     Copyright (c) MyPiTech. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.AspNetCore.Mvc;
using LandedMVC.Dtos;
using LandedMVC.Models;
using LandedMVC.Services;

namespace LandedMVC.Controllers
{
	/// <summary>
	/// Class UsersController.
	/// Implements the <see cref="Controller" />
	/// </summary>
	/// <seealso cref="Controller" />
	public class UsersController : Controller
    {
		/// <summary>
		/// The logger
		/// </summary>
		private readonly ILogger<UsersController> _logger;
		/// <summary>
		/// The user API service
		/// </summary>
		private readonly ApiService<UserDto> _userApiService;

		/// <summary>
		/// Initializes a new instance of the <see cref="UsersController"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="userApiService">The user API service.</param>
		public UsersController(ILogger<UsersController> logger, ApiService<UserDto> userApiService)
        {
            _logger = logger;
            _userApiService = userApiService;
        }

		/// <summary>
		/// Default index view.
		/// </summary>
		/// <returns>IActionResult.</returns>
		public IActionResult Index()
        {
			return View();
		}

		/// <summary>
		/// Get users as an asynchronous operation.
		/// </summary>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
		[HttpGet]
        public async Task<IActionResult> UsersAsync(CancellationToken token)
        {
            var users = await _userApiService.GetAllAsync(token);
            return Json(users);
        }

		/// <summary>
		/// Get user as an asynchronous operation.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
		[HttpGet]
        public async Task<IActionResult> UserAsync(int id, CancellationToken token)
        {
            var user = await _userApiService.GetOneAsync(()=> new UserDto { Id = id },token);
            return View(user?.ToModel() ?? default);
        }

		/// <summary>
		/// Delete user as an asynchronous operation.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
		[HttpGet]
        public async Task<IActionResult> DeleteAsync(int id, CancellationToken token)
        {
            await _userApiService.DeleteAsync(() => new UserDto { Id = id }, token);
            return RedirectToAction("Index");
        }

		/// <summary>
		/// Add User as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
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
