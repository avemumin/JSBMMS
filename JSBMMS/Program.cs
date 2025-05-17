using JSBMMS.Services;
using JSBMMS.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ICardService, CardService>();
builder.Services.AddSingleton<ICardActionService, CardActionService>();
builder.Services.AddControllers();

var app = builder.Build();


app.MapControllers();

app.Run();
