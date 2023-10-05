using Slider.Repository;
using Slider.Interface;
using Article.Interface;
using Article.Repository;
using Mongo.client;
using Cms.Context;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

var IConfiguration = Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISliderRepository, SliderRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddSingleton(Configuration);
builder.Services.AddSingleton<MongoConnection>();
builder.Services.AddSingleton<DapperContext>();

builder.Services.AddCors(options =>
{
     options.AddPolicy("XPolicy", builder =>
        {
            builder
                   .WithOrigins("http://localhost:3000")
                //    .AllowAnyOrigin()
                   .WithMethods("GET", "POST")
                   .WithHeaders("Content-Type","Access-Control-Allow-Origin", "Auth");
        });
});

IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()  // Optionally, add environment variables.
            .Build();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles(new StaticFileOptions()
{
    ServeUnknownFileTypes = true,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers["Access-Control-Allow-Origin"] = "*";
    },
});
app.UseCors("XPolicy");

// app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
