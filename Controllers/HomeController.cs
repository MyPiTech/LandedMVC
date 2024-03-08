// ***********************************************************************
// Assembly         : LandedMVC
// Author           : Shawn Wheeler
// Created          : 02-18-2024
//
// Last Modified By : Shawn Wheeler
// Last Modified On : 02-26-2024
// ***********************************************************************
// <copyright file="HomeController.cs" company="LandedMVC">
//     Copyright (c) MyPiTech. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using LandedMVC.Models;
using LandedMVC.Services;
using Microsoft.AspNetCore.Mvc;
using TestApi.Dtos;

namespace LandedMVC.Controllers
{
	/// <summary>
	/// Class HomeController.
	/// Implements the <see cref="Controller" />
	/// </summary>
	/// <seealso cref="Controller" />
	public class HomeController : Controller
    {
		/// <summary>
		/// The API service
		/// </summary>
		private readonly ApiService<EmailDto> _apiService;

		/// <summary>
		/// Initializes a new instance of the <see cref="HomeController"/> class.
		/// </summary>
		/// <param name="apiService">The API service.</param>
		public HomeController(ApiService<EmailDto> apiService)
        {
            _apiService = apiService;
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
		/// Send an email as an asynchronous operation.
		/// </summary>
		/// <param name="model">The email model.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
		[HttpPost]
		public async Task<IActionResult> EmailAsync(EmailModel model, CancellationToken token) 
        {
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			await _apiService.PostAsync(model.ToDto(), token);
			return Json(model);
		}

    }
}
