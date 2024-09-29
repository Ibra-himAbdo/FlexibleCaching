var builder = WebApplication.CreateBuilder(args);

var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

builder.Services.AddFlexibleCaching(redisConnectionString);

builder.Services.AddControllers();
builder.Services.AddLogging();
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

await app.RunAsync();
