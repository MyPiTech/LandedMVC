// ***********************************************************************
// Assembly         : LandedMVC
// Author           : Shawn Wheeler
// Created          : 04-30-2024
//
// Last Modified By : Shawn Wheeler
// Last Modified On : 05-01-2024
// ***********************************************************************
// <copyright file="DataTableHub.cs" company="LandedMVC">
//     Copyright (c) MyPiTech. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.AspNetCore.SignalR;

namespace LandedMVC.Hubs
{
	/// <summary>
	/// Class DataTableHub.
	/// Implements the <see cref="Hub" />
	/// </summary>
	/// <seealso cref="Hub" />
	public class DataTableHub : Hub
	{
		/// <summary>
		/// Adds the item.
		/// </summary>
		/// <param name="item">The item.</param>
		public async Task AddItem(object item)
		{
			await Clients.Others.SendAsync("ItemAdded", item);
		}
		/// <summary>
		/// Deletes the item.
		/// </summary>
		/// <param name="item">The item.</param>
		public async Task DeleteItem(object item)
		{
			await Clients.Others.SendAsync("ItemDeleted", item);
		}
		/// <summary>
		/// Updates the item.
		/// </summary>
		/// <param name="item">The item.</param>
		public async Task UpdateItem(object item)
		{
			await Clients.Others.SendAsync("ItemUpdated", item);
		}
	}
}
