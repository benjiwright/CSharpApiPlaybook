namespace Breakfast.RequestResponse;

public record CreateBreakfastRequest(
   string Name,
   string Description,
   DateTime StartDateTime,
   DateTime EndDateTime,
   List<string> Sweets,
   List<string> Savories
);