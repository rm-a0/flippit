using System.Globalization;
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
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    UseCardEndpoints(endpointsBase);
    UseCollectionEndpoints(endpointsBase);
    UseCompletedLessonEndpoints(endpointsBase);
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

    userEndpoints.MapGet("/{id:guid}/collections", (ICollectionFacade collectionFacade, Guid id) =>
    {
        var collections = collectionFacade.SearchByCreatorId(id);
        return TypedResults.Ok(collections);
    });

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

    userEndpoints.MapPost("upsert", async Task<Results<Ok<Guid>, ValidationProblem>> ([FromBody] UserDetailModel model, IUserFacade userFacade, IValidator<UserDetailModel> validator) => {
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
    var cardEndPoints = routeGroupBuilder.MapGroup("cards")
        .WithTags("cards");

    cardEndPoints.MapGet("", (ICardFacade cardFacade, [FromQuery] string? filter = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null) =>
    {
        var cards = cardFacade.GetAll(filter, sortBy, page, pageSize);
        return TypedResults.Ok(cards);
    });

    cardEndPoints.MapPost("", async Task<Results<Ok<Guid>, ValidationProblem>> (ICardFacade CardFacade, CardDetailModel model, IValidator<CardDetailModel> validator) =>
    {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = CardFacade.Create(model);
        return TypedResults.Ok(id);
    });

    cardEndPoints.MapPut("", async Task<Results<Ok<Guid?>, ValidationProblem>> (ICardFacade CardFacade, CardDetailModel model, IValidator<CardDetailModel> validator) =>
    {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = CardFacade.Update(model);
        return TypedResults.Ok(id);
    });

    cardEndPoints.MapPost("upsert", async Task<Results<Ok<Guid>, ValidationProblem>> (ICardFacade CardFacade, CardDetailModel model, IValidator<CardDetailModel> validator) =>
    {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = CardFacade.CreateOrUpdate(model);
        return TypedResults.Ok(id);
    });

    cardEndPoints.MapGet("/{id:guid}",
       Results<Ok<CardDetailModel>, NotFound<string>> (
           Guid id,
           [FromServices] ICardFacade cardFacade
       )
       =>
       cardFacade.GetById(id) is { } card
           ? TypedResults.Ok(card)
           : TypedResults.NotFound($"Card with ID {id} was not found.")
    );

    cardEndPoints.MapDelete("/{id:guid}", (ICardFacade cardFacade, Guid id) =>
    {
        cardFacade.Delete(id);
    });

}
void UseCollectionEndpoints(RouteGroupBuilder routeGroupBuilder)
{
    var collectionsEndPoints = routeGroupBuilder.MapGroup("collections")
        .WithTags("collections");

    collectionsEndPoints.MapGet("", (ICollectionFacade collectionFacade, [FromQuery] string? filter = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null) =>
    {
        var collections = collectionFacade.GetAll(filter, sortBy, page, pageSize);
        return TypedResults.Ok(collections);
    });

    collectionsEndPoints.MapGet("/{id:guid}/cards", (ICardFacade collectionFacade, Guid id) =>
    {
        var cards = collectionFacade.SearchByCollectionId(id);
        return TypedResults.Ok(cards);
    });

    collectionsEndPoints.MapPost("", async Task<Results<Ok<Guid>, ValidationProblem>> (ICollectionFacade collectionFacade, CollectionDetailModel model, IValidator<CollectionDetailModel> validator) =>
    {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = collectionFacade.Create(model);
        return TypedResults.Ok(id);
    });

    collectionsEndPoints.MapPut("", async Task<Results<Ok<Guid?>, ValidationProblem>> (ICollectionFacade collectionFacade, CollectionDetailModel model, IValidator<CollectionDetailModel> validator) =>
    {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = collectionFacade.Update(model);
        return TypedResults.Ok(id);
    });

    collectionsEndPoints.MapPost("upsert", async Task<Results<Ok<Guid>, ValidationProblem>> (ICollectionFacade collectionFacade, CollectionDetailModel model, IValidator<CollectionDetailModel> validator) =>
    {
        var validationErrors = await ValidateModelAsync(model, validator);
        if (validationErrors != null)
        {
            return TypedResults.ValidationProblem(validationErrors);
        }
        var id = collectionFacade.CreateOrUpdate(model);
        return TypedResults.Ok(id);
    });

    collectionsEndPoints.MapGet("/{id:guid}",
       Results<Ok<CollectionDetailModel>, NotFound<string>> (
           Guid id,
           [FromServices] ICollectionFacade collectionFacade
       )
       =>
       collectionFacade.GetById(id) is { } card
           ? TypedResults.Ok(card)
           : TypedResults.NotFound($"Collection with ID {id} was not found.")
    );

    collectionsEndPoints.MapDelete("/{id:guid}", (ICollectionFacade collectionFacade, Guid id) =>
    {
        collectionFacade.Delete(id);
    });
}
void UseCompletedLessonEndpoints(RouteGroupBuilder routeGroupBuilder)
{
    var completedLessonsEndpoints = routeGroupBuilder.MapGroup("completedLessons")
        .WithTags("completedLessons");

    completedLessonsEndpoints.MapGet("", (ICompletedLessonFacade facade, [FromQuery] int pageSize = 10, [FromQuery] int page = 1, [FromQuery] string? filter = null, [FromQuery] string? sortBy = null) =>
    {
        var lessons = facade.GetAll(filter, sortBy, page, pageSize);
        return TypedResults.Ok(lessons);
    });

    completedLessonsEndpoints.MapDelete("{id:guid}", (ICompletedLessonFacade facade, Guid id) =>
    {
        facade.Delete(id);
    });

    completedLessonsEndpoints.MapPost("", (ICompletedLessonFacade facade, CompletedLessonDetailModel model) =>
    {
        var id = facade.Create(model);
        return TypedResults.Ok(id);
    });

    completedLessonsEndpoints.MapPut("", (ICompletedLessonFacade facade, CompletedLessonDetailModel model) =>
    {
        var id = facade.Update(model);
        return TypedResults.Ok(id);
    });

    completedLessonsEndpoints.MapPost("upsert", (ICompletedLessonFacade facade, CompletedLessonDetailModel model) =>
    {
        var id = facade.Create(model);
        return TypedResults.Ok(id);

    });

    completedLessonsEndpoints.MapGet("{id:guid}", (ICompletedLessonFacade facade, Guid id) =>
    {
        var lesson = facade.GetById(id);
        return TypedResults.Ok(lesson);
    });
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









