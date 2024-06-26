﻿using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureMongoDb(builder.Configuration);
builder.Services.ConfigureLocatorSettings(builder.Configuration);
builder.Services.ConfigureBackgroundSettings(builder.Configuration);
builder.Services.ConfigureBroker(builder.Configuration);
builder.Services.ConfigureMakeTrendsBackgroundServiceSettings(builder.Configuration);
builder.Services.ConfigureMakeTrendsSettings(builder.Configuration);

builder.Services.AddScoped<InboxHashtagRepository>();
builder.Services.AddScoped<HashtagRepository>();
builder.Services.AddScoped<TrendsByContinentRepository>();
builder.Services.AddScoped<TrendsByCountryRepository>();
builder.Services.AddScoped<TrendsGlobalRepository>();

builder.Services.AddHttpClient<InboxBackgroundService>();

builder.Services.AddHostedService<MakeTrendsBackgroundService>();
builder.Services.AddHostedService<InboxBackgroundService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
var app = builder.Build();

app.MapGroup("/trends").MapGet("/global", async (TrendsGlobalRepository trendsGlobalRepository) =>
{
    return Results.Ok(await trendsGlobalRepository.GetTopTenAsync());
});

app.MapGroup("/trends").MapGet("/country/{countryName}", async ([FromRoute] string countryName, TrendsByCountryRepository trendsByCountryRepository) =>
{
    return Results.Ok(await trendsByCountryRepository.GetTrendsByCountryAsync(countryName));
});

app.MapGroup("/trends").MapGet("/continent/{continentName}", async ([FromRoute] string continentName, TrendsByContinentRepository trendsByContinentRepository) =>
{
    return Results.Ok(await trendsByContinentRepository.GetTrendsByContinentAsync(continentName));
});

app.UseHttpsRedirection();

app.Run();