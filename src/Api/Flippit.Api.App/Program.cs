using System.Text.Json;
using System.Text.Json.Serialization;
using Flippit.Api.BL.Facades;
using Flippit.Api.BL.Installers;
using Flippit.Api.BL.Mappers;
using Flippit.Api.DAL.Memory.Installers;
using Flippit.Common.Models.Card;
using Flippit.Common.Models.Collection;
using Flippit.Common.Models.CompletedLesson;
using Flippit.Common.Models.User;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


var DALinstaller = new ApiDALMemoryInstaller();
DALinstaller.Install(builder.Services);

builder.Services.AddSingleton<UserMapper>();
builder.Services.AddSingleton<CardMapper>();
builder.Services.AddSingleton<CollectionMapper>();
builder.Services.AddSingleton<CompletedLessonMapper>();

var BLInstaller = new ApiBLInstaller();
BLInstaller.Install(builder.Services);

builder.Services.AddScoped<IValidator<UserDetailModel>, UserDetailModelValidator>();
builder.Services.AddScoped<IValidator<CardDetailModel>, CardDetailModelValidator>();
builder.Services.AddScoped<IValidator<CollectionDetailModel>, CollectionDetailModelValidator>();


builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false));
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    //app.UseOpenApi();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

UseEndpoints(app);

app.Run();

void UseEndpoints(WebApplication application)
{
    var endpointsBase = application.MapGroup("api")
        .WithOpenApi();

    UseUserEndpoints(endpointsBase);
}

void UseUserEndpoints(RouteGroupBuilder routeGroupBuilder)
{
    var userEndpoints = routeGroupBuilder.MapGroup("users")
        .WithTags("users");

    userEndpoints.MapGet("", ([FromServices] IUserFacade userFacade, [FromQuery] string? filter = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null) => {
        var users = userFacade.GetAll(filter, sortBy, page, pageSize);
        return TypedResults.Ok(users);
    });

    userEndpoints.MapGet("/{id:guid}",
        Results<Ok<UserDetailModel>, NotFound<string>> (
            Guid id,
            [FromServices] IUserFacade userFacade
        )
        =>
        userFacade.GetById(id) is { } user
            ? TypedResults.Ok(user)
            : TypedResults.NotFound($"User with ID {id} was not found.")
    );

    userEndpoints.MapPost("", async Task<Results<Ok<Guid>, ValidationProblem>> ([FromBody] UserDetailModel model, IUserFacade userFacade, IValidator<UserDetailModel> validator) => {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = userFacade.Create(model);
        return TypedResults.Ok(id);
    });

    userEndpoints.MapPut("", async Task<Results<Ok<Guid?>, ValidationProblem>> ([FromBody] UserDetailModel model, IUserFacade userFacade, IValidator<UserDetailModel> validator) => {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = userFacade.Update(model);
        return TypedResults.Ok(id);
    });

    userEndpoints.MapPut("upsert", async Task<Results<Ok<Guid>, ValidationProblem>> ([FromBody] UserDetailModel model, IUserFacade userFacade, IValidator<UserDetailModel> validator) => {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = userFacade.CreateOrUpdate(model);
        return TypedResults.Ok(id);
    });

    userEndpoints.MapDelete("{id:guid}", (Guid id, IUserFacade userFacade) =>
    {
        userFacade.Delete(id);
    });

}
void UseCardEndpoints(RouteGroupBuilder routeGroupBuilder)
{

}
void UseCollectionEndpoints(RouteGroupBuilder routeGroupBuilder)
{

}
void UseCompletedLessonEndpoints(RouteGroupBuilder routeGroupBuilder)
{

}

async Task<Dictionary<string, string[]>?> ValidateModelAsync<T>(T model, IValidator<T> validator)
{
    var validationResult = await validator.ValidateAsync(model);

    if (validationResult.IsValid)
    {
        return null;
    }

    return validationResult.Errors
        .GroupBy(e => e.PropertyName)
        .ToDictionary(
            g => g.Key,
            g => g.Select(e => e.ErrorMessage).ToArray()
        );
}









