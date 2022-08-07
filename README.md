# CSharpApiPlaybook

Recipe for making boilerplate C# API projects. Taken from the good folks at [freeCodeCamp.org](https://youtu.be/PmDJIooZjBE).

## Steps

### create solution and projects (mvc and contracts separate for nuget reasons)

``` bash
# set ENVS
export PROJECT_NAME=Breakfast
# solution
dotnet new sln -o $PROJECT_NAME
cd $PROJECT_NAME
# contracts project
dotnet new classlib -o $PROJECT_NAME.Contracts
# api project
dotnet new webapi -o $PROJECT_NAME
# add both projects to the solution
dotnet sln add `ls -r **/*.csproj`
# main project will need a ref to the contracts
dotnet add ./$PROJECT_NAME reference ./$PROJECT_NAME.Contracts/
# verify build/run
dotnet build; dotnet run --project ./$PROJECT_NAME
```

optional: clean up Program.cs file

```csharp
var builder = WebApplication.CreateBuilder(args);
{
   builder.Services.AddControllers();
}

var app = builder.Build();
{
   app.UseHttpsRedirection();
   app.MapControllers();
   app.Run();
}
```

### define the API model for request and response as records for the CRUDs in a new `RequestResponse` folder

```bash
CreateBreakfastRequest.cs
CreateBreakfastResponse.cs
UpsertBreakRequest.cs
```

### create controller

```csharp
[ApiController]
[Route("[controller]")]
public class BreakfastController : ControllerBase
{
   [HttpPost]
   public IActionResult CreateBreakfast(CreateBreakfastRequest request)
   {
      return Ok(request);
   }
   
   [HttpGet("{id:guid}")]
   public IActionResult GetBreakfast(Guid id)
   {
      return Ok(id);
   }
   
   [HttpPut("{id:guid}")]
   public IActionResult UpsertBreakfast(Guid id, UpsertBreakRequest request)
   {
      return Ok(request);
   }
   
   [HttpDelete("{id:guid}")]
   public IActionResult DeleteBreakfast(Guid id)
   {
      return Ok(id);
   }
}
```

### create abstraction from API to our internal models (new models folder)

```bash
mkdir ./$PROJECT_NAME/Models
touch ./$PROJECT_NAME/Models/Breakfast.cs
```

```csharp
namespace Breakfast.Models;

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
```

take incoming api request and map to our internal model

```csharp
   // BreakfastController
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

      // TODO: save to persistence model (DB)

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

      return CreatedAtAction(                   // 204
         actionName: nameof(GetBreakfast),      // endpoint
         routeValues: new {id = breakfast.Id},  // route requires the new Id
         value: response);                      // payload
   }

```

create DB/persistence abstraction

```bash
mkdir ./$PROJECT_NAME/Services
mkdir ./$PROJECT_NAME/Services/Breakfasts
touch ./$PROJECT_NAME/Services/Breakfasts/BreakfastService.cs
```

```csharp
namespace Breakfast.Services.Breakfasts;

public interface IBreakfastService
{
   void CreateBreakfast(Models.Breakfast breakfast);
}

public class BreakfastService : IBreakfastService
{
   // in memory persistence only (sub EF or DB)
   private readonly Dictionary<Guid, Models.Breakfast> _breakfasts = new();

   public void CreateBreakfast(Models.Breakfast breakfast)
   {
      _breakfasts.Add(breakfast.Id, breakfast);
   }
}
```

inject the new service into the `controller`. Use the service.

```csharp
private readonly IBreakfastService _breakfastService;

public BreakfastController(IBreakfastService breakfastService)
{
   _breakfastService = breakfastService;
}

# Update our TODO in the controller to use the service

var model = _breakfastService.CreateBreakfast(breakfast);

```

register DI in Program.cs

```csharp
builder.Services.AddSingleton<IBreakfastService, BreakfastService>();
```

### better error handling

add to the middleware pipeline at the beginning

```csharp
var app = builder.Build();
{
   const string exceptionRoute = "/error";
   app.UseExceptionHandler(exceptionRoute);
   // ...
}
```

create new error route / controller

```csharp
public class ErrorsController : ControllerBase
{
   [Route("/error")]
   public IActionResult Error()
   {
      return Problem();
   }
}

```

optional: use error lib/package

```bash
 dotnet add ./$PROJECT_NAME package ErrorOr
 mkdir ./${PROJECT_NAME}/ServiceErrors
 touch ./${PROJECT_NAME}/ServiceErrors/Errors.Breakfast.cs
```

create specific errors to things that can go wrong in our service. E.g.: Look for breakfast by Id, but not found.

```csharp
// ... Custom error for when Breakfast is not found

namespace Breakfast.ServiceErrors;

using ErrorOr;

public static class Errors
{
   public static class Breakfast
   {
      public static Error NotFound => Error.NotFound(
         code: "Breakfast.NotFound",
         description: "Breakfast not found"
      );
   }
}

// ... Update the service to use our custom error

public interface IBreakfastService
{
   ErrorOr<Breakfast> GetBreakfast(Guid id);
}

public ErrorOr<Breakfast> GetBreakfast(Guid id)
{
   if (InMemoryBreakfasts.TryGetValue(id, out var breakfast))
   {
      return breakfast;
   }

   return Errors.Breakfast.NotFound;
}

/// ... Update Api controller as well

   [HttpGet("{id:guid}")]
   public IActionResult GetBreakfast(Guid id)
   {
      var getBreakfastResult = _breakfastService.GetBreakfast(id);

      if (getBreakfastResult.IsError) return NotFound();
      
      // TODO: auto-mapper
      var breakfast = getBreakfastResult.Value;
      var response = MapCreateBreakfastResponse(breakfast);

      return Ok(response);
   }


```
