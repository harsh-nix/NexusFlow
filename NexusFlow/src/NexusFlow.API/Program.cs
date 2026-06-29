using NexusFlow.API.Extensions;
using NexusFlow.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();

// AutoMapper - will register when profiles are created
// builder.Services.AddAutoMapper(typeof(Program));

// ── Pipeline ──────────────────────────────────────
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("NexusFlowCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();