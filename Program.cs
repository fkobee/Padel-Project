
using RankingPadelAPI.Data;
using Microsoft.EntityFrameworkCore;
using RankingPadelAPI.Services;
using RankingPadelAPI.Repositories;
using RankingPadelAPI.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IJugadorService, JugadorService>();
builder.Services.AddScoped<ISessionService, SessionService>();

builder.Services.AddScoped<IJugadorRepository, JugadorRepository>();
builder.Services.AddScoped<IJugadorService, JugadorService>();

builder.Services.AddScoped<ITorneoRepository, TorneoRepository>();
builder.Services.AddScoped<ITorneosService, TorneoService>();

builder.Services.AddScoped<IPartidoRepository, PartidoRepository>();
builder.Services.AddScoped<IPartidoService, PartidoService>();

builder.Services.AddScoped<UserRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
