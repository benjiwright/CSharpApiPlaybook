
namespace Breakfast.Services.Breakfasts;

public interface IBreakfastService
{
   void CreateBreakfast(Models.Breakfast breakfast);
   Models.Breakfast GetBreakfast(Guid id);
   Models.Breakfast UpsertBreakfast(Guid id, Models.Breakfast breakfast);
   void DeleteBreakfast(Guid id);
}

public class BreakfastService : IBreakfastService
{
   // in memory persistence (sub EF or DB)
   private static readonly Dictionary<Guid, Models.Breakfast> InMemoryBreakfasts = new();

   public void CreateBreakfast(Models.Breakfast breakfast)
   {
      InMemoryBreakfasts.Add(breakfast.Id, breakfast);
   }

   public Models.Breakfast GetBreakfast(Guid id)
   {
      return InMemoryBreakfasts[id];
   }

   public Models.Breakfast UpsertBreakfast(Guid id, Models.Breakfast breakfast)
   {

      if (!InMemoryBreakfasts.ContainsKey(id))
      {
         InMemoryBreakfasts.Add(id, breakfast);
      }
      else
      {
         InMemoryBreakfasts[id] = breakfast;
      }

      return InMemoryBreakfasts[id];
   }

   public void DeleteBreakfast(Guid id)
   {
      InMemoryBreakfasts.Remove(id);
   }
}