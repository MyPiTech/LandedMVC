// ***********************************************************************
// Assembly         : LandedMVC
// Author           : Shawn Wheeler
// Created          : 02-27-2024
//
// Last Modified By : Shawn Wheeler
// Last Modified On : 02-27-2024
// ***********************************************************************
// <copyright file="EventsController.cs" company="LandedMVC">
//     Copyright (c) MyPiTech. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using LandedMVC.Dtos;
using LandedMVC.Extensions;
using LandedMVC.Hubs;
using LandedMVC.Models;
using LandedMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LandedMVC.Controllers
{
	/// <summary>
	/// Class EventsController.
	/// Implements the <see cref="Controller" />
	/// </summary>
	/// <seealso cref="Controller" />
	public class EventsController: ControllerBase<EventsController>
	{
		/// <summary>
		/// The event API service
		/// </summary>
		private readonly ApiService<EventDto> _apiService;

		/// <summary>
		/// Initializes a new instance of the <see cref="EventsController"/> class.
		/// </summary>
		/// <param name="apiService">The API service.</param>
		public EventsController(ApiService<EventDto> apiService,
			IHubContext<ConsoleHub, IConsoleHub> consoleHub,
			ILogger<EventsController> logger,
			IConfiguration configuration,
			IHttpContextAccessor accessor
		) : base(configuration, logger, consoleHub, accessor)
		{
			_apiService = apiService;
		}

		/// <summary>
		/// Default index view.
		/// </summary>
		/// <returns>IActionResult.</returns>
		public async Task<IActionResult> IndexAsync()
		{
			try
			{
				return View(new ApiModel { ApiBase = _apiBase });
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "EventsController\\IndexAsync");
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Get all events as an asynchronous operation.
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
				await _logger.LogDebugAsync("EventsController\\GetAllAsync", result);
				return result;
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "EventsController\\GetAllAsync");
				return BadRequest(ex.Message);
			}
		}

		// <summary>
		/// Delete event as an asynchronous operation.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		[HttpDelete]
		public async Task DeleteAsync(int id, CancellationToken token)
		{
			try
			{
				await _apiService.DeleteAsync(() => new EventDto { Id = id }, token);
				// TODO: merge Id into string.
				await _logger.LogInformationAsync("EventsController\\DeleteAsync - id:", id);
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "EventsController\\DeleteAsync");
			}
		}

		/// <summary>
		/// Add or update an event as an asynchronous operation.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A dto representing the results of the asynchronous operation.</returns>
		[HttpPost]
		public async Task<IActionResult> UpsertAsync(EventModel model, CancellationToken token)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					var response = BadRequest(ModelState);
					await _logger.LogWarningAsync("EventsController\\UpsertAsync", response);
					return response;
				}

				if (model.Id == 0)
				{
					var dto = await _apiService.AddAsync(model.ToDto(), token);
					if (dto != null)
					{
						model = dto.ToModel();
						await _logger.LogInformationAsync("EventsController\\UpsertAsync - add", model);
					}
				}
				else
				{
					await _apiService.EditAsync(model.ToDto(), token);
					await _logger.LogInformationAsync("EventsController\\UpsertAsync - update", model);
				}
				return Ok(model);
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "EventsController\\UpsertAsync");
				return BadRequest(ex.Message);
			}
		}
	}
}
