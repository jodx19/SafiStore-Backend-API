using System.Collections.Generic;
using System.Threading.Tasks;
using SafiStore.Api.Application.DTOs;

namespace SafiStore.Api.Infrastructure.Services
{
    public interface IOrderService
    {
        Task<SafiStore.Api.Application.DTOs.ServiceResult<int>> CreateOrderAsync(CreateOrderDto dto);
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<List<OrderDto>> GetOrdersForUserAsync(int userId);

        Task<(List<OrderDto> Orders, int Total)> GetAllOrdersAsync(int page = 1, int pageSize = 20);

        Task<OrderDto> UpdateOrderStatusAsync(int orderId, string status);


    }
}
