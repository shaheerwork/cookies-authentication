using CookieAuth.API.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register services
builder.Services.AddSingleton<IUserService, UserService>();

// Configure cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "CookieAuth.API";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax; // Set to Strict in production
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

// Configure CORS for web client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebClient", builder =>
        builder.WithOrigins("https://localhost:5002") // Web app URL
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowWebClient");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
