using EKO.PingPingApi.Filters;
using EKO.PingPingApi.Infrastructure.Services;
using EKO.PingPingApi.Infrastructure.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(provider => new ArgumentNullExceptionFilter());

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ArgumentNullExceptionFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IRequestService, PingPingRequestService>();
builder.Services.AddSingleton<IPingPingService, PingPingService>();

var app = builder.Build();

// Enable the CORS middleware
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
