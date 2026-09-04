using System.Text;
using BookApp_api.Extension;
using BookApp_api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalNg", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "http://127.0.0.1:4200",
                "https://agent-6a9aaf9e9a992d8b--vermillion-khapse-68d62e.netlify.app",
                "https://vermillion-khapse-68d62e.netlify.app")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<BookAppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(
        typeof(Program).Assembly));

builder.Services.AddSingleton<ITokenService, TokenService>();

var jwt = builder.Configuration.GetSection("Jwt");
var jwtKey = jwt["Key"]
    ?? throw new InvalidOperationException("JWT key is not configured.");
var jwtIssuer = jwt["Issuer"]
    ?? throw new InvalidOperationException("JWT issuer is not configured.");
var jwtAudience = jwt["Audience"]
    ?? throw new InvalidOperationException("JWT audience is not configured.");
var jwtExpiryMinutes = jwt.GetValue<int?>("ExpiryMinutes")
    ?? throw new InvalidOperationException("JWT expiry is not configured.");

if (jwtExpiryMinutes <= 0)
{
    throw new InvalidOperationException("JWT expiry must be greater than zero.");
}

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BookAppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("LocalNg");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();