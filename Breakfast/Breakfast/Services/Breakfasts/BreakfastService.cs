using Breakfast.ServiceErrors;

namespace Breakfast.Services.Breakfasts;

using Models;
using ErrorOr;

public interface IBreakfastService
{
   ErrorOr<Created> CreateBreakfast(Breakfast breakfast);
   ErrorOr<Breakfast> GetBreakfast(Guid id);
   ErrorOr<UpsertedBreakfast> UpsertBreakfast(Guid id, Breakfast breakfast);
   ErrorOr<Deleted> DeleteBreakfast(Guid id);
}

public class BreakfastService : IBreakfastService
{
   // in memory persistence 
   private static readonly Dictionary<Guid, Breakfast> InMemoryBreakfasts = new();

   public ErrorOr<Created> CreateBreakfast(Breakfast breakfast)
   {
      InMemoryBreakfasts.Add(breakfast.Id, breakfast);
      return Result.Created;
   }

   public ErrorOr<Breakfast> GetBreakfast(Guid id)
   {
      if (InMemoryBreakfasts.TryGetValue(id, out var breakfast))
      {
         return breakfast;
      }

      return Errors.Breakfast.NotFound;
   }

   public ErrorOr<UpsertedBreakfast> UpsertBreakfast(Guid id, Breakfast breakfast)
   {
      var isNewlyCreated = !InMemoryBreakfasts.ContainsKey(breakfast.Id);
      
      if (!InMemoryBreakfasts.ContainsKey(id))
      {
         InMemoryBreakfasts.Add(id, breakfast);
      }
      else
      {
         InMemoryBreakfasts[id] = breakfast;
      }

      return new UpsertedBreakfast(isNewlyCreated);
   }

   public ErrorOr<Deleted> DeleteBreakfast(Guid id)
   {
      InMemoryBreakfasts.Remove(id);
      return Result.Deleted;
   }
}