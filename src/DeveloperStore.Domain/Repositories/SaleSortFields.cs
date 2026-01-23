namespace DeveloperStore.Domain.Repositories;

/// <summary>
/// Defines allowed sort fields for sale queries to prevent property enumeration attacks.
/// </summary>
public static class SaleSortFields
{
    /// <summary>
    /// All allowed sort field names for Sale queries.
    /// </summary>
    public static readonly HashSet<string> AllowedFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "SaleNumber",
        "SaleDate",
        "TotalAmount",
        "Status",
        "CreatedAt",
        "UpdatedAt"
    };

    /// <summary>
    /// Validates if a field name is allowed for sorting.
    /// </summary>
    /// <param name="fieldName">The field name to validate.</param>
    /// <returns>True if allowed, false otherwise.</returns>
    public static bool IsAllowed(string fieldName)
    {
        return AllowedFields.Contains(fieldName);
    }

    /// <summary>
    /// Validates an order specification string.
    /// Throws exception if invalid fields are found.
    /// </summary>
    /// <param name="orderBy">The order specification (e.g., "SaleDate desc, TotalAmount").</param>
    /// <exception cref="ArgumentException">Thrown when invalid field is detected.</exception>
    public static void ValidateOrderBy(string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return;

        var fields = orderBy
            .Split(',')
            .Select(x => x.Trim().Trim('"').Split(' ', StringSplitOptions.RemoveEmptyEntries)[0])
            .Where(x => !string.IsNullOrEmpty(x));

        foreach (var field in fields)
        {
            if (!IsAllowed(field))
            {
                throw new ArgumentException(
                    $"Invalid sort field '{field}'. Allowed fields: {string.Join(", ", AllowedFields)}",
                    nameof(orderBy));
            }
        }
    }
}
