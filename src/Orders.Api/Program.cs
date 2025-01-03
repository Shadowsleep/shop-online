using MongoDB.Bson;
using MongoDB.Driver;
using Orders.Application;
using Orders.Infrastructure;
using Orders.Infrastructure.Repositories;
using MongoDB.Bson.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();


        //Dependencies
        builder.Services.ConfigurationServicesOrderApplication();
        builder.Services.ConfigurationServicesOrderInfrastructure();

        ConfigMongo(builder);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void ConfigMongo(WebApplicationBuilder builder)
    {
        var mongoSettings = builder.Configuration.GetSection("mongo").Get<MongoDbOptions>();
        BsonSerializer.RegisterSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(GuidRepresentation.Standard));

        builder.Services.AddSingleton(new MongoClient(mongoSettings.ConnectionString));
        builder.Services.AddSingleton(provider =>
        {
            var mongoClient = provider.GetRequiredService<MongoClient>();
            return mongoClient.GetDatabase(mongoSettings.Database);
        });
    }
}