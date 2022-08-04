namespace Breakfast.RequestResponse;

public record CreateBreakfastResponse(
   Guid Id,
   string Name,
   string Description,
   DateTime StartDateTime,
   DateTime EndDateTime,
   DateTime LastModified,
   List<string> Sweets,
   List<string> Savories
);