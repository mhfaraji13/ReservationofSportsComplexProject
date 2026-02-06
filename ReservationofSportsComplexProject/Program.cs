using Microsoft.EntityFrameworkCore;
using ReservationSportsComplex.Application.Interfaces;
using ReservationSportsComplex.Application.Mappings;
using ReservationSportsComplex.Application.Services;
using ReservationSportsComplex.Domain.Interfaces;
using ReservationSportsComplex.Infrastructure.Data;
using ReservationSportsComplex.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(connectionString));

builder.Services.AddScoped<IApplicationDbContext>(provider =>
provider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ISportHall, SportHallRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
