using System.Collections.Generic;
using System.Threading.Tasks;
using SafiStore.Api.Application.DTOs;

namespace SafiStore.Api.Application.DTOs
{
    /// <summary>
    /// DTO for updating the status of an order (Admin only).
    /// Moved from inline declaration in AdminController.
    /// </summary>
    public class UpdateOrderStatusDto
    {
        /// <summary>New status: e.g., "Pending", "Shipped", "Delivered", "Cancelled"</summary>
        public string Status { get; set; } = string.Empty;
    }
}
