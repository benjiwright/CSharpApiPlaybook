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