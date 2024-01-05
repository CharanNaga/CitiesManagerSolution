using CitiesManager.WebAPI.DatabaseContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(
    options =>
    {
        options.Filters.Add(new ProducesAttribute("application/json")); //making default content type for all action methods to 'application/json' so that in swagger.json we can see only application/json content RESPONSE
        options.Filters.Add(new ConsumesAttribute("application/json")); //making default REQUEST type to application/json
    })
    .AddXmlSerializerFormatters(); //for enabling xml formatter which isn't supported by .net core by default

//adding api versioning to identify same end points from different versions
builder.Services.AddApiVersioning(
    config =>
    {
        config.ApiVersionReader = new UrlSegmentApiVersionReader(); //to identify current working version of api from request url (mentioned in CustomController Route as route parameter)
        //config.ApiVersionReader = new QueryStringApiVersionReader(); //to identify current working version of api from query string 'api-version' (mentioned in CustomController)
        //config.ApiVersionReader = new HeaderApiVersionReader("api-version"); //to identify current working version of api from request header 'api-version' (Make get request with key 'api-version' value 1 in postman)


        config.DefaultApiVersion = new ApiVersion(1, 0); //setting default api version to 1.0 if no version is specified in querystring
        config.AssumeDefaultVersionWhenUnspecified = true;
    });

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

        //writing swagger documentations for the two versions
        //2. supplying versions explicitly using the below SwaggerDoc's
        options.SwaggerDoc("v1", new OpenApiInfo()
        {
            Title = "Cities Web API",
            Version = "1.0"
        });

        options.SwaggerDoc("v2", new OpenApiInfo()
        {
            Title = "Cities Web API",
            Version = "2.0"
        });

    }); //configure swagger to generate documentation for API's endpoints (openapi specification).

builder.Services.AddVersionedApiExplorer(options =>
{
    //3. letting swagger substitute to corresponding swagger version path.
    options.GroupNameFormat = "'v'VVV"; //eg:v1
    options.SubstituteApiVersionInUrl = true; //when set true then the version number above can be substituted in SwaggerEndPoint options provided
});

//CORS: localhost:7283
builder.Services.AddCors(
    options =>
    {
        options.AddDefaultPolicy(policyBuilder =>
        {
            //policyBuilder.WithOrigins("http://localhost:4200"); //giving permission to url of angular application to read response from our core application

            policyBuilder.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()); //giving permission to url of angular application to read response from our core application

            policyBuilder.WithHeaders(
                "Authorization", "origin", "accept", "content-type"
                );//List of Request headers allowed in the client side

            policyBuilder.WithMethods("GET", "POST", "PUT", "DELETE"); //Request methods to be allowed
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHsts();
app.UseHttpsRedirection();

app.UseSwagger(); //creates endpoint for swagger.json file (openapi specification abt web api controller endpoints)
app.UseSwaggerUI(options =>
{
    //providing two seperate endpoints for locating swagger.json based on the versions requested by the user.
    //1.enabling end points
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "2.0");
}); //creates swagger UI for testing all web api controllers / action methods

//to generate response header for each request, use middleware cors
app.UseRouting();
app.UseCors(); //allow to send response header 'Access-Control-Allow-Origin' with value of http://localhost:4200

app.UseAuthorization();

app.MapControllers();

app.Run();
