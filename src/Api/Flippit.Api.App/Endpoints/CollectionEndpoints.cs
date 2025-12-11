using Flippit.Api.App.Helpers;
using Flippit.Api.BL.Facades;
using Flippit.Api.BL.Services;
using Flippit.Common.Models.Collection;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Flippit.Api.App.Endpoints;

public static class CollectionEndpoints
{
    public static RouteGroupBuilder MapCollectionEndpoints(this RouteGroupBuilder routeGroupBuilder)
    {
        var collectionsEndPoints = routeGroupBuilder.MapGroup("collections")
            .WithTags("collections");

        collectionsEndPoints.MapGet("", (ICollectionFacade collectionFacade, [FromQuery] string? filter = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null) =>
        {
            var collections = collectionFacade.GetAll(filter, sortBy, page, pageSize);
            return TypedResults.Ok(collections);
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

        collectionsEndPoints.MapGet("/{id:guid}/cards", (ICardFacade collectionFacade, Guid id, [FromQuery] string? filter = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null) =>
        {
            var cards = collectionFacade.SearchByCollectionId(id, filter, sortBy, page, pageSize);
            return TypedResults.Ok(cards);
        });

        collectionsEndPoints.MapGet("/{id:guid}/CompletedLessons", (ICompletedLessonFacade facade, Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null) =>
        {
            var AllCompletedLessons = facade.SearchByCollectionId(id, sortBy, page, pageSize);
            return TypedResults.Ok(AllCompletedLessons);
        });

        collectionsEndPoints.MapPost("", [Authorize] async Task<Results<Ok<Guid>, ValidationProblem>> (ICollectionFacade collectionFacade, CollectionDetailModel model, IValidator<CollectionDetailModel> validator) =>
        {
            var validationErrors = await ValidationHelper.ValidateModelAsync(model, validator);
            if (validationErrors != null)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }
            var id = collectionFacade.Create(model);
            return TypedResults.Ok(id);
        });

        collectionsEndPoints.MapPut("", [Authorize] async Task<Results<Ok<Guid?>, ValidationProblem, ForbidHttpResult, NotFound<string>>> (
            ICollectionFacade collectionFacade, 
            CollectionDetailModel model, 
            IValidator<CollectionDetailModel> validator,
            ICurrentUserService currentUserService) =>
        {
            var validationErrors = await ValidationHelper.ValidateModelAsync(model, validator);
            if (validationErrors != null)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }

            // Check if collection exists and user has permission
            var existingCollection = collectionFacade.GetById(model.Id);
            if (existingCollection == null)
            {
                return TypedResults.NotFound($"Collection with ID {model.Id} was not found.");
            }

            // Check authorization: must be owner or admin
            var currentUserId = currentUserService.CurrentUserId;
            var isAdmin = currentUserService.IsInRole("Admin");
            if (currentUserId != existingCollection.CreatorId && !isAdmin)
            {
                return TypedResults.Forbid();
            }

            var id = collectionFacade.Update(model);
            return TypedResults.Ok(id);
        });

        collectionsEndPoints.MapPost("upsert", [Authorize] async Task<Results<Ok<Guid>, ValidationProblem>> (ICollectionFacade collectionFacade, CollectionDetailModel model, IValidator<CollectionDetailModel> validator) =>
        {
            var validationErrors = await ValidationHelper.ValidateModelAsync(model, validator);
            if (validationErrors != null)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }
            var id = collectionFacade.CreateOrUpdate(model);
            return TypedResults.Ok(id);
        });

        collectionsEndPoints.MapDelete("/{id:guid}", [Authorize] Results<NoContent, ForbidHttpResult, NotFound<string>> (
            ICollectionFacade collectionFacade, 
            Guid id,
            ICurrentUserService currentUserService) =>
        {
            // Check if collection exists and user has permission
            var existingCollection = collectionFacade.GetById(id);
            if (existingCollection == null)
            {
                return TypedResults.NotFound($"Collection with ID {id} was not found.");
            }

            // Check authorization: must be owner or admin
            var currentUserId = currentUserService.CurrentUserId;
            var isAdmin = currentUserService.IsInRole("Admin");
            if (currentUserId != existingCollection.CreatorId && !isAdmin)
            {
                return TypedResults.Forbid();
            }

            collectionFacade.Delete(id);
            return TypedResults.NoContent();
        });

        return routeGroupBuilder;
    }
}
