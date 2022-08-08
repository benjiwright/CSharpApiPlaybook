using Breakfast.RequestResponse;
using Breakfast.Services.Breakfasts;
using Microsoft.AspNetCore.Mvc;

namespace Breakfast.Controllers;

public class BreakfastsController : ApiController
{
   private readonly IBreakfastService _breakfastService;

   public BreakfastsController(IBreakfastService breakfastService)
   {
      _breakfastService = breakfastService;
   }

   [HttpPost]
   public IActionResult CreateBreakfast(CreateBreakfastRequest request)
   {
      // map request to our model
      var breakfast = new Models.Breakfast(
         Guid.NewGuid(),
         request.Name,
         request.Description,
         request.StartDateTime,
         request.EndDateTime,
         DateTime.Now,
         request.Sweets,
         request.Savories
      );

      var createBreakfastResult = _breakfastService.CreateBreakfast(breakfast);

      return createBreakfastResult.Match(
         _ => CreatedAtBreakfast(breakfast),
         errors => Problem(errors));
   }

   [HttpGet("{id:guid}")]
   public IActionResult GetBreakfast(Guid id)
   {
      var getBreakfastResult = _breakfastService.GetBreakfast(id);

      return getBreakfastResult.Match(
         breakfast => Ok(MapCreateBreakfastResponse(breakfast)),
         errors => Problem(errors));
   }


   [HttpPut("{id:guid}")]
   public IActionResult UpsertBreakfast(Guid id, UpsertBreakRequest request)
   {
      var breakfast = new Models.Breakfast(
         id,
         request.Name,
         request.Description,
         request.StartDateTime,
         request.EndDateTime,
         DateTime.Now,
         request.Sweets,
         request.Savories);

      var upsertBreakfastResult = _breakfastService.UpsertBreakfast(id, breakfast);

      return upsertBreakfastResult.Match(
         upsert => upsert.IsNewlyCreated
            ? CreatedAtBreakfast(breakfast)
            : NoContent(),
         errors => Problem(errors));

   }

   [HttpDelete("{id:guid}")]
   public IActionResult DeleteBreakfast(Guid id)
   {
      var deleteBreakfastResult = _breakfastService.DeleteBreakfast(id);

      return deleteBreakfastResult.Match(
         _ => NoContent(),
         errors => Problem(errors));
   }

   // TODO: automapper
   private CreateBreakfastResponse MapCreateBreakfastResponse(Models.Breakfast breakfast)
   {
      var response = new CreateBreakfastResponse(
         breakfast.Id,
         breakfast.Name,
         breakfast.Description,
         breakfast.StartDateTime,
         breakfast.EndDateTime,
         breakfast.LastModified,
         breakfast.Sweets,
         breakfast.Savories);
      return response;
   }

   private CreatedAtActionResult CreatedAtBreakfast(Models.Breakfast breakfast)
   {
      return CreatedAtAction( // 204
         actionName: nameof(GetBreakfast), // endpoint
         routeValues: new {id = breakfast.Id}, // route requires the new Id
         value: MapCreateBreakfastResponse(breakfast)); // payload
   }
}