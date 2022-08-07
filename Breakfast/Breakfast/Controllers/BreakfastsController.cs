using Breakfast.RequestResponse;
using Breakfast.Services.Breakfasts;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace Breakfast.Controllers;

[ApiController]
[Route("[controller]")]
public class BreakfastsController : ControllerBase
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

      _breakfastService.CreateBreakfast(breakfast);

      // covert our model back to an api response 
      var response = new CreateBreakfastResponse(
         breakfast.Id,
         breakfast.Name,
         breakfast.Description,
         breakfast.StartDateTime,
         breakfast.EndDateTime,
         breakfast.LastModified,
         breakfast.Savories,
         breakfast.Sweets);

      return CreatedAtAction( // 204
         actionName: nameof(GetBreakfast), // endpoint
         routeValues: new {id = breakfast.Id}, // route requires the new Id
         value: response); // payload
   }

   [HttpGet("{id:guid}")]
   public IActionResult GetBreakfast(Guid id)
   {
      var getBreakfastResult = _breakfastService.GetBreakfast(id);

      if (getBreakfastResult.IsError) return NotFound();
      
      // TODO: automapper
      var breakfast = getBreakfastResult.Value;
      var response = MapCreateBreakfastResponse(breakfast);

      return Ok(response);
   }

   private static CreateBreakfastResponse MapCreateBreakfastResponse(Models.Breakfast breakfast)
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

      var model = _breakfastService.UpsertBreakfast(id, breakfast);

      var response = new CreateBreakfastResponse(
         model.Id,
         model.Name,
         model.Description,
         model.StartDateTime,
         model.EndDateTime,
         model.LastModified,
         model.Sweets,
         model.Savories);

      return Ok(response);
   }

   [HttpDelete("{id:guid}")]
   public IActionResult DeleteBreakfast(Guid id)
   {
      _breakfastService.DeleteBreakfast(id);
      return NoContent();
   }
}