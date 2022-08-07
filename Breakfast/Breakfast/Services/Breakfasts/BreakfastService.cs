using Breakfast.ServiceErrors;

namespace Breakfast.Services.Breakfasts;

using Models;
using ErrorOr;

public interface IBreakfastService
{
   void CreateBreakfast(Models.Breakfast breakfast);
   ErrorOr<Breakfast> GetBreakfast(Guid id);
   Breakfast UpsertBreakfast(Guid id, Models.Breakfast breakfast);
   void DeleteBreakfast(Guid id);
}

public class BreakfastService : IBreakfastService
{
   // in memory persistence 
   private static readonly Dictionary<Guid, Breakfast> InMemoryBreakfasts = new();

   public void CreateBreakfast(Breakfast breakfast)
   {
      InMemoryBreakfasts.Add(breakfast.Id, breakfast);
   }

   public ErrorOr<Breakfast> GetBreakfast(Guid id)
   {
      if (InMemoryBreakfasts.TryGetValue(id, out var breakfast))
      {
         return breakfast;
      }

      return Errors.Breakfast.NotFound;
   }

   public Breakfast UpsertBreakfast(Guid id, Breakfast breakfast)
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