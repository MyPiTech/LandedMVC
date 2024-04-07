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
using LandedMVC.Extensions;

namespace LandedMVC.Controllers
{
	/// <summary>
	/// Class UsersController.
	/// Implements the <see cref="Controller" />
	/// </summary>
	/// <seealso cref="Controller" />
	public class UsersController : ControllerBase<UsersController>
    {
		/// <summary>
		/// The user API service
		/// </summary>
		private readonly ApiService<UserDto> _apiService;

		/// <summary>
		/// The user event API service
		/// </summary>
		private readonly ApiService<UserEventDto> _eventApiService;

		/// <summary>Initializes a new instance of the <see cref="T:LandedMVC.Controllers.UsersController" /> class.</summary>
		/// <param name="apiService">The model API service.</param>
		/// <param name="eventApiService"></param>
		/// <param name="consoleHub"></param>
		/// <param name="logger"></param>
		/// <param name="configuration"></param>
		public UsersController(
			ApiService<UserDto> apiService, 
			ApiService<UserEventDto> eventApiService, 
			IHubContext<ConsoleHub, IConsoleHub> consoleHub, 
			ILogger<UsersController> logger,
			IConfiguration configuration
		) : base(configuration, logger, consoleHub)
		{
			_apiService = apiService;
			_eventApiService = eventApiService;
		}

		/// <summary>
		/// Default index view.
		/// </summary>
		/// <returns>IActionResult.</returns>
		public IActionResult Index()
        {
			return View(new ApiModel { ApiBase = _apiBase });
		}

		/// <summary>Gets user events as an asynchronous operation.</summary>
		/// <param name="uId">The user identifier.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
		public async Task<IActionResult> EventsAsync(int uId, CancellationToken token)
		{
			try
			{
				var user = await _apiService.GetOneAsync(() => new UserDto { Id = uId }, token);
				if (user == null)
				{
					var response = BadRequest($"No user was found with id:{uId}");
					await _logger.LogWarningAsync("UsersController\\EventsAsync", _consoleHub, response);
					return response;
				}
				var model = user.ToModel();
				model.ApiBase = _apiBase;

				var result = View(model);
				await _logger.LogDebugAsync("UsersController\\EventsAsync", _consoleHub, result);
				return result;
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "UsersController\\EventsAsync", _consoleHub);
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Get all users as an asynchronous operation.
		/// </summary>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A collection of model dtos representing the results of the asynchronous operation.</returns>
		[HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken token)
        {
			try
			{
				var results = await _apiService.GetAllAsync(token);
				var result = Ok(results);
				await _logger.LogDebugAsync("UsersController\\GetAllAsync", _consoleHub, result);
				return result;
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "UsersController\\GetAllAsync", _consoleHub);
				return BadRequest(ex.Message);
			}
        }

		/// <summary>Get all user events as an asynchronous operation.</summary>
		/// <param name="uId">The user ID.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A collection of event dtos representing the results of the asynchronous operation.</returns>
		[HttpGet]
		public async Task<IActionResult> GetAllEventsAsync(int uId, CancellationToken token)
		{
			try
			{
				var results = await _eventApiService.GetAllAsync(() => new UserEventDto { UserId = uId }, token);
				var result = Json(results);
				await _logger.LogDebugAsync("UsersController\\GetAllEventsAsync", _consoleHub, result);
				return result;
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "UsersController\\GetAllEventsAsync", _consoleHub);
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Delete user as an asynchronous operation.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		[HttpDelete]
        public async Task DeleteAsync(int id, CancellationToken token)
        {
			try
			{
				await _apiService.DeleteAsync(() => new UserDto { Id = id }, token);
				await _logger.LogInformationAsync("UsersController\\DeleteAsync - id:", _consoleHub, id);
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "UsersController\\DeleteAsync", _consoleHub);
			}

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
			try
			{
				await _eventApiService.DeleteAsync(() => new UserEventDto { Id = id, UserId = userId }, token);
				await _logger.LogInformationAsync("UsersController\\DeleteEventAsync - ids:", _consoleHub, id, userId);
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "UsersController\\DeleteEventAsync", _consoleHub);
			}
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
			try
			{
				if (!ModelState.IsValid)
				{
					var response = BadRequest(ModelState);
					await _logger.LogWarningAsync("UsersController\\UpsertAsync", _consoleHub, response);
					return response;
				}

				if (model.Id == 0)
				{
					var dto = await _apiService.AddAsync(model.ToDto(), token);
					if (dto != null)
					{
						model = dto.ToModel();
						await _logger.LogInformationAsync("UsersController\\UpsertAsync - add", _consoleHub, model);
					}
				}
				else
				{
					await _apiService.EditAsync(model.ToDto(), token);
					await _logger.LogInformationAsync("UsersController\\UpsertAsync - update", _consoleHub, model);
				}
				return Json(model);
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "UsersController\\UpsertAsync", _consoleHub);
				return BadRequest(ex.Message);
			}
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
			try
			{
				if (!ModelState.IsValid)
				{
					var response = BadRequest(ModelState);
					await _logger.LogWarningAsync("UsersController\\UpsertEventAsync", _consoleHub, response);
					return response;
				}

				if (model.Id == 0)
				{
					var dto = await _eventApiService.AddAsync(model.ToUserEventDto(), token);
					if (dto != null)
					{
						model = dto.ToModel();
						await _logger.LogInformationAsync("UsersController\\UpsertEventAsync - add", _consoleHub, model);
					}
				}
				else
				{
					await _eventApiService.EditAsync(model.ToUserEventDto(), token);
					await _logger.LogInformationAsync("UsersController\\UpsertEventAsync - update", _consoleHub, model);
				}
				return Json(model);
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "UsersController\\UpsertEventAsync", _consoleHub);
				return BadRequest(ex.Message);
			}
		}

	}
}
