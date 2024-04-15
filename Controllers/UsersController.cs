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

		/// <summary>Initializes a new instance of the <see cref="T:LandedMVC.Controllers.UsersController" /> class.</summary>
		/// <param name="apiService">The model API service.</param>
		/// <param name="consoleHub"></param>
		/// <param name="logger"></param>
		/// <param name="configuration"></param>
		public UsersController(
			ApiService<UserDto> apiService,  
			IHubContext<ConsoleHub, IConsoleHub> consoleHub, 
			ILogger<UsersController> logger,
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
				await _logger.LogErrorAsync(ex, "UsersController\\IndexAsync");
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
				await _logger.LogDebugAsync("UsersController\\GetAllAsync", result);
				return result;
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "UsersController\\GetAllAsync");
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
				await _logger.LogInformationAsync("UsersController\\DeleteAsync - id:", id);
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "UsersController\\DeleteAsync");
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
					await _logger.LogWarningAsync("UsersController\\UpsertAsync", response);
					return response;
				}

				if (model.Id == 0)
				{
					var dto = await _apiService.AddAsync(model.ToDto(), token);
					if (dto != null)
					{
						model = dto.ToModel();
						await _logger.LogInformationAsync("UsersController\\UpsertAsync - add", model);
					}
				}
				else
				{
					await _apiService.EditAsync(model.ToDto(), token);
					await _logger.LogInformationAsync("UsersController\\UpsertAsync - update", model);
				}
				return Ok(model);
			}
			catch (Exception ex)
			{
				await _logger.LogErrorAsync(ex, "UsersController\\UpsertAsync");
				return BadRequest(ex.Message);
			}
		}
	}
}
