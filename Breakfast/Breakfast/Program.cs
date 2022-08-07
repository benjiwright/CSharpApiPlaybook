using Breakfast.Services.Breakfasts;

#region dependancy injection

var builder = WebApplication.CreateBuilder(args);
{
   builder.Services.AddControllers();
   builder.Services.AddSingleton<IBreakfastService, BreakfastService>();
}

#endregion

#region middleware pipeline stuff

var app = builder.Build();
{
   const string exceptionRoute = "/error";
   app.UseExceptionHandler(exceptionRoute);
   app.UseHttpsRedirection();
   app.MapControllers();
   app.Run();
}

#endregion