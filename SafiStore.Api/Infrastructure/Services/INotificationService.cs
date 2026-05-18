using System.Collections.Generic;
using System.Threading.Tasks;
using SafiStore.Api.Models.Domain;

namespace SafiStore.Api.Infrastructure.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(int userId, string type, string message, int? orderId = null);
        Task<List<Notification>> GetNotificationsForUserAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(int userId);
    }
}
