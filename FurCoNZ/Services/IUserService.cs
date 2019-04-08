﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FurCoNZ.Models;

namespace FurCoNZ.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> ListUsersAsync(CancellationToken cancellationToken = default);
        Task<User> GetUserAsync(string id, CancellationToken cancellationToken = default);
        Task CreateUserAsync(User user, CancellationToken cancellationToken = default);

        Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    }
}
