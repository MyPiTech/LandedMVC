// ***********************************************************************
// Assembly         : LandedMVC
// Author           : Shawn Wheeler
// Created          : 04-03-2024
//
// Last Modified By : Shawn Wheeler
// Last Modified On : 04-15-2024
// ***********************************************************************
// <copyright file="IConsoleHub.cs" company="LandedMVC">
//     Copyright (c) MyPiTech. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace LandedMVC.Hubs
{
	/// <summary>
	/// Interface IConsoleHub
	/// </summary>
	public interface IConsoleHub {
		/// <summary>
		/// Sends the log asynchronous.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>Task.</returns>
		Task SendLogAsync(params object[]? data);
		/// <summary>
		/// Sends the information asynchronous.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>Task.</returns>
		Task SendInfoAsync(params object[]? data);
		/// <summary>
		/// Sends the warn asynchronous.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>Task.</returns>
		Task SendWarnAsync(params object[]? data);
		/// <summary>
		/// Sends the error asynchronous.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>Task.</returns>
		Task SendErrorAsync(params object[]? data);
    }
}
