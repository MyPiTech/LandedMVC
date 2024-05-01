// ***********************************************************************
// Assembly         : LandedMVC
// Author           : Shawn Wheeler
// Created          : 04-02-2024
//
// Last Modified By : Shawn Wheeler
// Last Modified On : 04-15-2024
// ***********************************************************************
// <copyright file="ConsoleHub.cs" company="LandedMVC">
//     Copyright (c) MyPiTech. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.AspNetCore.SignalR;

namespace LandedMVC.Hubs
{
	/// <summary>
	/// Class ConsoleHub.
	/// Implements the <see cref="Microsoft.AspNetCore.SignalR.Hub{LandedMVC.Hubs.IConsoleHub}" />
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.SignalR.Hub{LandedMVC.Hubs.IConsoleHub}" />
	public class ConsoleHub : Hub<IConsoleHub>
    {
		/// <summary>
		/// Send log as an asynchronous operation.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public async Task SendLogAsync(object? data)
        {
            await Clients.All.SendLogAsync(data ?? string.Empty);
        }
		/// <summary>
		/// Send information as an asynchronous operation.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public async Task SendInfoAsync(object? data)
        {
            await Clients.All.SendInfoAsync(data ?? string.Empty);
        }
		/// <summary>
		/// Send warn as an asynchronous operation.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public async Task SendWarnAsync(object? data)
        {
            await Clients.All.SendWarnAsync(data ?? string.Empty);
        }
		/// <summary>
		/// Send error as an asynchronous operation.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		public async Task SendErrorAsync(object? data)
        {
            await Clients.All.SendErrorAsync(data ?? string.Empty);
        }
    }
}
