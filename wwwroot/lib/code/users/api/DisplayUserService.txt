// ***********************************************************************
// Assembly         : TestApi
// Author           : Shawn Wheeler
// Created          : 01-02-2024
//
// Last Modified By : Shawn Wheeler
// Last Modified On : 03-05-2024
// ***********************************************************************
// <copyright file="UserService.cs" company="TestApi">
//     Copyright (c) MyPiTech. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.EntityFrameworkCore;
using Test.Data;
using System.Linq.Expressions;
using TestApi.Dtos;

namespace TestApi.Services
{
	/// <summary>
	/// Class UserService.
	/// Implements the <see cref="TestApi.Services.Service{TestApi.Services.UserService, Test.Data.User, TestApi.Dtos.UserDto}" />
	/// Implements the <see cref="TestApi.Services.IService{Test.Data.User, TestApi.Dtos.CreateUserDto, TestApi.Dtos.UserDto}" />
	/// </summary>
	/// <seealso cref="TestApi.Services.Service{TestApi.Services.UserService, Test.Data.User, TestApi.Dtos.UserDto}" />
	/// <seealso cref="TestApi.Services.IService{Test.Data.User, TestApi.Dtos.CreateUserDto, TestApi.Dtos.UserDto}" />
	public class UserService : Service<UserService, User, UserDto>, IService<User, CreateUserDto, UserDto>
    {

		/// <summary>
		/// Initializes a new instance of the <see cref="UserService"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="dataContext">The data context.</param>
		public UserService(ILogger<UserService> logger, MSTestDataContext dataContext) : base(logger, dataContext)
        {
            _toDto = u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Notes = u.Notes
            };
        }

		/// <summary>
		/// Get all as an asynchronous operation.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;List`1&gt; representing the asynchronous operation.</returns>
		public async Task<List<UserDto>> GetAllAsync(Expression<Func<User, bool>>? predicate, CancellationToken token)
        {
            return await Dtos().ToListAsync(token);
        }

		/// <summary>
		/// Get as an asynchronous operation.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;UserDto&gt; representing the asynchronous operation.</returns>
		public async Task<UserDto?> GetAsync(Expression<Func<User, bool>> predicate, CancellationToken token)
        {
            return await Dtos(predicate).FirstOrDefaultAsync(token);
        }

		/// <summary>
		/// Create as an asynchronous operation.
		/// </summary>
		/// <param name="dto">The dto.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <param name="parentId">The parent identifier.</param>
		/// <returns>A Task&lt;UserDto&gt; representing the asynchronous operation.</returns>
		public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken token, int? parentId = null)
        {
            var entity = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Notes = dto.Notes
            };
            await _dataContext.AddAsync(entity, token);
            await _dataContext.SaveChangesAsync(token);

            _logger.LogInformation($"User: {string.Join(", ", dto.LastName, dto.FirstName)} Id:{entity.Id} created.");
            return AsDto(entity);
        }

		/// <summary>
		/// Delete as an asynchronous operation.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task representing the asynchronous operation.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Invalid identifier.</exception>
		public async Task DeleteAsync(Expression<Func<User, bool>> predicate, CancellationToken token)
        {
            var entity = await _dataContext.Users.FirstOrDefaultAsync(predicate, token);

            if (entity == null) throw new ArgumentOutOfRangeException("Invalid identifier.");

            _dataContext.Remove(entity);
            await _dataContext.SaveChangesAsync(token);
        }

		/// <summary>
		/// Replace as an asynchronous operation.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <param name="dto">The dto.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;UserDto&gt; representing the asynchronous operation.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Invalid identifier.</exception>
		public async Task<UserDto> ReplaceAsync(Expression<Func<User, bool>> predicate, CreateUserDto dto, CancellationToken token)
        {
            var entity = await _dataContext.Users.FirstOrDefaultAsync(predicate, token);

            if(entity == null) throw new ArgumentOutOfRangeException("Invalid identifier.");

            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Notes = dto.Notes;

            await _dataContext.SaveChangesAsync(token);

            return AsDto(entity);
        }

		/// <summary>
		/// Update as an asynchronous operation.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <param name="dto">The dto.</param>
		/// <param name="token">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
		/// <returns>A Task&lt;UserDto&gt; representing the asynchronous operation.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Invalid identifier.</exception>
		public async Task<UserDto> UpdateAsync(Expression<Func<User, bool>> predicate, CreateUserDto dto, CancellationToken token)
        {
            var entity = await _dataContext.Users.FirstOrDefaultAsync(predicate, token);

            if (entity == null) throw new ArgumentOutOfRangeException("Invalid identifier.");

            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Notes = dto.Notes ?? entity.Notes;

            await _dataContext.SaveChangesAsync(token);
            return AsDto(entity);
        }
    }
}
