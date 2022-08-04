namespace Breakfast.Models;

public class Breakfast
{
   public Guid Id { get; }
   public string Name { get; }
   public string Description { get; }
   public DateTime StartDateTime { get; }
   public DateTime EndDateTime { get; }
   public DateTime LastModified { get; }
   public List<string> Sweets { get; }
   public List<string> Savories { get; }

   public Breakfast(
      Guid id,
      string name,
      string description,
      DateTime startDateTime,
      DateTime endDateTime,
      DateTime lastModified,
      List<string> sweets,
      List<string> savories)
   {
      // TODO: create model rules
      Id = id;
      Name = name;
      Description = description;
      StartDateTime = startDateTime;
      EndDateTime = endDateTime;
      LastModified = lastModified;
      Sweets = sweets;
      Savories = savories;
   }
}