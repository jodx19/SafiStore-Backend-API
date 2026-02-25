using Microsoft.EntityFrameworkCore;

namespace SafiStore.Api.Common.Extensions
{
    public static class DbUpdateExceptionExtensions
    {
        // Detect common SQL Server unique constraint violation codes (2627, 2601)
        // and provider-agnostic messages. Keep this encapsulated so controllers
        // and higher layers never inspect DbUpdateException directly.
        public static bool IsUniqueConstraintViolation(this DbUpdateException ex)
        {
            // Inspect inner exception messages for common SQL Server codes.
            // Different providers will have different messages; this method centralizes detection.
            var inner = ex.InnerException?.Message ?? string.Empty;
            if (inner.Contains("2627") || inner.Contains("2601"))
                return true;

            // SQLite uses "UNIQUE constraint failed"
            if (inner.Contains("UNIQUE constraint failed"))
                return true;

            // Postgres unique violation contains "unique constraint"
            if (inner.Contains("unique constraint") || inner.Contains("duplicate key"))
                return true;

            return false;
        }
    }
}
