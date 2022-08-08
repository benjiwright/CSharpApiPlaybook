using Microsoft.AspNetCore.Mvc;

namespace Breakfast.Controllers;

public class ErrorsController : ControllerBase
{
   public IActionResult Error()
   {
      return Problem();
   }
}