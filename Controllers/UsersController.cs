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
using Microsoft.AspNetCore.SignalR;
using LandedMVC.Hubs;

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
		/// The user API service
		/// </summary>
		private readonly ApiService<UserDto> _apiService;

		/// <summary>
		/// The user event API service
		/// </summary>
		private readonly ApiService<UserEventDto> _eventApiService;

		private readonly IHubContext<ConsoleHub, IConsoleHub> _consoleHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="apiService">The model API service.</param>
        public UsersController(ApiService<UserDto> apiService, ApiService<UserEventDto> eventApiService, IHubContext<ConsoleHub, IConsoleHub> consoleHub)
        {
            _apiService = apiService;
			_eventApiService = eventApiService;
			_consoleHub = consoleHub;
        }

		/// <summary>
		/// Default index view.
		/// </summary>
		/// <returns>IActionResult.</returns>
		public IActionResult Index()
        {
			return View();
		}

		/// <summary>Gets user events as an asynchronous operation.</summary>
		/// <param name="uId">The user identifier.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
		public async Task<IActionResult> EventsAsync(int uId, CancellationToken token)
		{
			var user = await _apiService.GetOneAsync(() => new UserDto { Id = uId }, token);
			if (user == null)
			{
				var response = BadRequest($"No user was found with id:{uId}");
				await _consoleHub.Clients.All.SendWarnAsync("UsersController\\EventsAsync - bad request", response);
				return response;
			}
			var result = View(user?.ToModel() ?? default);
			await _consoleHub.Clients.All.SendInfoAsync("UsersController\\EventsAsync", result);
			return result;
		}

		/// <summary>
		/// Get all users as an asynchronous operation.
		/// </summary>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A collection of model dtos representing the results of the asynchronous operation.</returns>
		[HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken token)
        {
			
            var results = await _apiService.GetAllAsync(token);
            var jsonResult = Json(results);
            await _consoleHub.Clients.All.SendInfoAsync("UsersController\\GetAllAsync", jsonResult);
            return jsonResult;
        }

		/// <summary>Get all user events as an asynchronous operation.</summary>
		/// <param name="uId">The user ID.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A collection of event dtos representing the results of the asynchronous operation.</returns>
		[HttpGet]
		public async Task<IActionResult> GetAllEventsAsync(int uId, CancellationToken token)
		{
			var results = await _eventApiService.GetAllAsync(() => new UserEventDto { UserId = uId }, token);
            var jsonResult = Json(results);
            await _consoleHub.Clients.All.SendInfoAsync("UsersController\\GetAllEventsAsync", jsonResult);
            return Json(results);
		}

		/// <summary>
		/// Delete user as an asynchronous operation.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		[HttpDelete]
        public async Task DeleteAsync(int id, CancellationToken token)
        {
            await _apiService.DeleteAsync(() => new UserDto { Id = id }, token);
            await _consoleHub.Clients.All.SendInfoAsync("UsersController\\DeleteAsync", id);

		}

		/// <summary>
		/// Delete event as an asynchronous operation.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="userId">The user identifier.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		[HttpDelete]
		public async Task DeleteEventAsync(int id, int userId, CancellationToken token)
		{
			await _eventApiService.DeleteAsync(() => new UserEventDto { Id = id, UserId = userId }, token);
			await _consoleHub.Clients.All.SendInfoAsync("UsersController\\DeleteEventAsync", id, userId);
		}

		/// <summary>
		/// Add or update a user as an asynchronous operation.
		/// </summary>
		/// <param name="model">The user model.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A model dto representing the results of the asynchronous operation.</returns>
		[HttpPost]
        public async Task<IActionResult> UpsertAsync(UserModel model, CancellationToken token)
        {
			if (!ModelState.IsValid)
			{
				var response = BadRequest(ModelState);
				await _consoleHub.Clients.All.SendWarnAsync("UsersController\\UpsertAsync - bad request", response);
				return response;
			}

            if (model.Id == 0)
            {
				var dto = await _apiService.AddAsync(model.ToDto(), token);
				if(dto != null)
				{
					model = dto.ToModel();
                    await _consoleHub.Clients.All.SendInfoAsync("UsersController\\UpsertAsync - add", model);
                }
            }
            else { 
                await _apiService.EditAsync(model.ToDto(), token);
                await _consoleHub.Clients.All.SendInfoAsync("UsersController\\UpsertAsync - update", model);
            }
			return Json(model);
		}

		/// <summary>
		/// Add or update a user event as an asynchronous operation.
		/// </summary>
		/// <param name="model">The user model.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A model dto representing the results of the asynchronous operation.</returns>
		[HttpPost]
		public async Task<IActionResult> UpsertEventAsync(EventModel model, CancellationToken token)
		{
			if (!ModelState.IsValid)
			{
				var response = BadRequest(ModelState);
				await _consoleHub.Clients.All.SendWarnAsync("UsersController\\UpsertEventAsync - bad request", response);
				return response;
			}

			if (model.Id == 0)
			{
				var dto = await _eventApiService.AddAsync(model.ToUserEventDto(), token);
				if (dto != null)
				{
					model = dto.ToModel();
					await _consoleHub.Clients.All.SendInfoAsync("UsersController\\UpsertEventAsync - add", model);
				}
			}
			else
			{
				await _eventApiService.EditAsync(model.ToUserEventDto(), token);
				await _consoleHub.Clients.All.SendInfoAsync("UsersController\\UpsertEventAsync - update", model);
			}
			return Json(model);
		}

	}
}
