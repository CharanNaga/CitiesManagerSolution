using CitiesManager.WebAPI.DatabaseContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

//Adding DbContext as a service to the IoC container
builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

//add swagger as service
builder.Services.AddEndpointsApiExplorer(); //enables swagger to read metadata(http methods, url attributes) of endpoints (web api action methods) i.e., generates description of all endpoints
builder.Services.AddSwaggerGen(
    options =>
    {
        options.IncludeXmlComments(
            Path.Combine(AppContext.BaseDirectory, "api.xml")
            );
    }); //configure swagger to generate documentation for API's endpoints (openapi specification).

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHsts();

app.UseHttpsRedirection();

app.UseSwagger(); //creates endpoint for swagger.json file (openapi specification abt web api controller endpoints)
app.UseSwaggerUI(); //creates swagger UI for testing all web api controllers / action methods

app.UseAuthorization();

app.MapControllers();

app.Run();
