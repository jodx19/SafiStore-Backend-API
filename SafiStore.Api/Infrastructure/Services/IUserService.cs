using System.Collections.Generic;
using System.Threading.Tasks;
using SafiStore.Api.Application.DTOs;

namespace SafiStore.Api.Infrastructure.Services
{
    public interface IUserService
    {
        Task<(List<UserDto> Users, int Total)> GetAllUsersAsync(int page = 1, int pageSize = 20);
        Task DeleteUserAsync(int userId);
    }
}
