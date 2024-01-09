using CitiesManager.Core.Identities;
using CitiesManager.Core.ServiceContracts;
using CitiesManager.Core.Services;
using CitiesManager.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(
    options =>
    {
        options.Filters.Add(new ProducesAttribute("application/json")); //making default content type for all action methods to 'application/json' so that in swagger.json we can see only application/json content RESPONSE
        options.Filters.Add(new ConsumesAttribute("application/json")); //making default REQUEST type to application/json

        //Authorization policy
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        options.Filters.Add(new AuthorizeFilter(policy));
    })
    .AddXmlSerializerFormatters(); //for enabling xml formatter which isn't supported by .net core by default

//adding jwtservice to the Ioc Container
builder.Services.AddTransient<IJwtService,JwtService>();

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

        ////Applying custom cors policy which will override the default policy
        //options.AddPolicy("4100Client",policyBuilder =>
        //{
        //    policyBuilder.WithOrigins(builder.Configuration.GetSection("AllowedOriginsDummy").Get<string[]>()); //giving permission to url of angular application to read response from our core application

        //    policyBuilder.WithHeaders(
        //        "Authorization", "origin", "accept"
        //        );//List of Request headers allowed in the client side

        //    policyBuilder.WithMethods("GET"); //Request methods to be allowed
        //});
    });

//adding Identity as a service to IoC container
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>  //for creating users, roles tables
{
    options.Password.RequiredLength = 5; //min length of password is 5 chars
    options.Password.RequireNonAlphanumeric = false; //may or mayn't contain atleast one non alphanumeric value
    options.Password.RequireUppercase = false; //may or mayn't contain atleast one uppercase letter
    options.Password.RequireLowercase = true; //must and should contain atleast one lowercase letter
    options.Password.RequireDigit = true; //Contain atleast one digit
})
    .AddEntityFrameworkStores<ApplicationDbContext>()  //creating tables using IdentityDbContext overall in entire application
    .AddDefaultTokenProviders() //Generating tokens at runtime randomly while Email & phone number verifications, forgot or resetting passwords
    .AddUserStore<
        UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>
        >()  //configuring repository layer for users table i.e., users store
    .AddRoleStore<
        RoleStore<ApplicationRole, ApplicationDbContext, Guid>
        >(); //configuring repository layer for roles table i.e., roles store


//Adding Authentication for JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
                )
        };
    });

//Adding Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles(); //optional

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

app.UseAuthentication(); //when we make request to app pipeline, if a user is already logged in (identity cookie already present in browser).. that cookie automatically submitted to server as part of request cookies then this authentication will read that particular cookie & extract user details like UserID & UserName
app.UseAuthorization(); // Validates access permissions of the user.

app.MapControllers();

app.Run();
