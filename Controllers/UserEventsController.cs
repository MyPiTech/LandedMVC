// ***********************************************************************
// Assembly         : LandedMVC
// Author           : Shawn Wheeler
// Created          : 02-20-2024
//
// Last Modified By : Shawn Wheeler
// Last Modified On : 04-15-2024
// ***********************************************************************
// <copyright file="UserEventsController.cs" company="LandedMVC">
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
	/// Class UserEventsController.
	/// Implements the <see cref="LandedMVC.Controllers.ControllerBase{LandedMVC.Controllers.UserEventsController}" />
	/// </summary>
	/// <seealso cref="LandedMVC.Controllers.ControllerBase{LandedMVC.Controllers.UserEventsController}" />
	[Route("users/{uId:int}")]
	public class UserEventsController : ControllerBase<UserEventsController>
	{
		/// <summary>
		/// The API service
		/// </summary>
		private readonly ApiService<UserEventDto> _apiService;

		/// <summary>
		/// The user API service
		/// </summary>
		private readonly ApiService<UserDto> _usersApiService;

		/// <summary>
		/// Initializes a new instance of the <see cref="UserEventsController"/> class.
		/// </summary>
		/// <param name="apiService">The API service.</param>
		/// <param name="usersApiService">The users API service.</param>
		/// <param name="consoleHub">The console hub.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="configuration">The configuration.</param>
		/// <param name="accessor">The accessor.</param>
		public UserEventsController(
			ApiService<UserEventDto> apiService,
			ApiService<UserDto> usersApiService,
			IHubContext<ConsoleHub, IConsoleHub> consoleHub,
			ILogger<UserEventsController> logger,
			IConfiguration configuration,
			IHttpContextAccessor accessor
		) : base(configuration, logger, consoleHub, accessor)
		{
			_apiService = apiService;
			_usersApiService = usersApiService;
		}

		/// <summary>
		/// Gets user events as an asynchronous operation.
		/// </summary>
		/// <param name="uId">The user identifier.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
		[Route("events")]
		public async Task<IActionResult> IndexAsync([FromRoute] int uId, CancellationToken token)
		{
			try
			{
				var user = await _usersApiService.GetOneAsync(() => new UserDto { Id = uId }, token);
				if (user == null)
				{
					var response = BadRequest($"No user was found with id:{uId}");
					await _logger.LogWarningAsync("UserEventsController\\IndexAsync", response);
					return response;
				}
				var model = user.ToModel();
				model.ApiBase = _apiBase;

				var result = View("/Views/users/events.cshtml", model);
				await _logger.LogDebugAsync("UserEventsController\\IndexAsync", result);
				return result;
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "UserEventsController\\IndexAsync");
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Default index view.
		/// </summary>
		/// <param name="uId">The user ID.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A collection of event dtos representing the results of the asynchronous operation.</returns>
		[HttpGet]
		[Route("getAll")]
		public async Task<IActionResult> GetAllAsync([FromRoute] int uId, CancellationToken token)
		{
			try
			{
				var results = await _apiService.GetAllAsync(() => new UserEventDto { UserId = uId }, token);
				var result = Ok(results);
				await _logger.LogDebugAsync("UserEventsController\\GetAllAsync", result);
				return result;
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "UserEventsController\\GetAllAsync");
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Delete event as an asynchronous operation.
		/// </summary>
		/// <param name="uId">The user identifier.</param>
		/// <param name="eId">The event identifier.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		[HttpDelete]
		[Route("delete")]
		public async Task DeleteAsync([FromRoute] int uId, int eId, CancellationToken token)
		{
			try
			{
				await _apiService.DeleteAsync(() => new UserEventDto { Id = eId, UserId = uId }, token);
				await _logger.LogInformationAsync("UserEventsController\\DeleteAsync - ids:", uId, eId);
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "UserEventsController\\DeleteAsync");
			}
		}

		/// <summary>
		/// Add or update a user event as an asynchronous operation.
		/// </summary>
		/// <param name="model">The user model.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A model dto representing the results of the asynchronous operation.</returns>
		[HttpPost]
		[Route("upsert")]
		public async Task<IActionResult> UpsertAsync(EventModel model, CancellationToken token)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					var response = BadRequest(ModelState);
					await _logger.LogWarningAsync("UserEventsController\\UpsertAsync", response);
					return response;
				}

				if (model.Id == 0)
				{
					var dto = await _apiService.AddAsync(model.ToUserEventDto(), token);
					if (dto != null)
					{
						model = dto.ToModel();
						await _logger.LogInformationAsync("UserEventsController\\UpsertAsync - add", model);
					}
				}
				else
				{
					await _apiService.EditAsync(model.ToUserEventDto(), token);
					await _logger.LogInformationAsync("UserEventsController\\UpsertAsync - update", model);
				}
				return Ok(model);
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "UserEventsController\\UpsertAsync");
				return BadRequest(ex.Message);
			}
		}

	}
}
