namespace Breakfast.RequestResponse;

public record UpsertBreakRequest(
   Guid Id,
   string Name,
   string Description,
   DateTime StartDateTime,
   DateTime EndDateTime,
   DateTime LastModified,
   List<string> Sweets,
   List<string> Savories
);