﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using FurCoNZ.Web.DAL;
using FurCoNZ.Web.Models;
using System.Data;
using Microsoft.AspNetCore.Http;

namespace FurCoNZ.Web.Services
{
    public class EntityFrameworkUserService : IUserService
    {
        private readonly FurCoNZDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EntityFrameworkUserService(FurCoNZDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task CreateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (await _dbContext.Users.AnyAsync(u => u.Id == user.Id, cancellationToken))
                throw new DuplicateNameException(nameof(user));

            cancellationToken.ThrowIfCancellationRequested();

            await _dbContext.Users.AddAsync(user, cancellationToken); // Seems silly to be async, but used in some SQL Server scenarios

            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst("user")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return null;

            return await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId , cancellationToken);
        }

        public async Task<User> GetUserFromIssuerAsync(string issuer, string subject, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(issuer))
                throw new ArgumentNullException(nameof(issuer));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException(nameof(subject));

            var linkedAccount = await _dbContext.LinkedAccounts
                .Include(la => la.User)
                .FirstOrDefaultAsync(l => l.Issuer == issuer && l.Subject == subject ,cancellationToken);

            if (linkedAccount == null)
                return null;

            return linkedAccount.User;
        }

        public async Task<User> GetUserAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == id , cancellationToken);
        }

        public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            _dbContext.Users.Update(user);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> ListUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users.ToListAsync(cancellationToken);
        }
    }
}
