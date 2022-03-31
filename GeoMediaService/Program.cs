using GeoMediaService.Options;
using GeoMediaService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<YoutubeService>();
builder.Services.AddScoped<InstaService>();

builder.Services.Configure<YoutubeOptions>(builder.Configuration.GetSection("Youtube"));
builder.Services.Configure<InstaOptions>(builder.Configuration.GetSection("Insta"));

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
