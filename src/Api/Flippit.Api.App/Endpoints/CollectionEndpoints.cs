using Flippit.Api.App.Helpers;
using Flippit.Api.BL.Facades;
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

        var collectionModifyingEndpoints = collectionsEndPoints.MapGroup("")
            .RequireAuthorization();

        collectionModifyingEndpoints.MapPost("", async Task<Results<Ok<Guid>, ValidationProblem>> (ICollectionFacade collectionFacade, CollectionDetailModel model, IValidator<CollectionDetailModel> validator, IHttpContextAccessor httpContextAccessor) =>
        {
            var validationErrors = await ValidationHelper.ValidateModelAsync(model, validator);
            if (validationErrors != null)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);
            var id = collectionFacade.Create(model, userRoles, userId);
            return TypedResults.Ok(id);
        });

        collectionModifyingEndpoints.MapPut("", async Task<Results<Ok<Guid?>, ValidationProblem, ForbidHttpResult>> (ICollectionFacade collectionFacade, CollectionDetailModel model, IValidator<CollectionDetailModel> validator, IHttpContextAccessor httpContextAccessor) =>
        {
            var validationErrors = await ValidationHelper.ValidateModelAsync(model, validator);
            if (validationErrors != null)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);
            try
            {
                var id = collectionFacade.Update(model, userRoles, userId);
                return TypedResults.Ok(id);
            }
            catch (UnauthorizedAccessException)
            {
                return TypedResults.Forbid();
            }
        });

        collectionModifyingEndpoints.MapPost("upsert", async Task<Results<Ok<Guid>, ValidationProblem>> (ICollectionFacade collectionFacade, CollectionDetailModel model, IValidator<CollectionDetailModel> validator, IHttpContextAccessor httpContextAccessor) =>
        {
            var validationErrors = await ValidationHelper.ValidateModelAsync(model, validator);
            if (validationErrors != null)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);
            var id = collectionFacade.CreateOrUpdate(model, userRoles, userId);
            return TypedResults.Ok(id);
        });

        collectionModifyingEndpoints.MapDelete("/{id:guid}", Results<Ok, ForbidHttpResult> (ICollectionFacade collectionFacade, Guid id, IHttpContextAccessor httpContextAccessor) =>
        {
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);
            try
            {
                collectionFacade.Delete(id, userRoles, userId);
                return TypedResults.Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return TypedResults.Forbid();
            }
        });

        return routeGroupBuilder;
    }

    private static string? GetUserId(IHttpContextAccessor httpContextAccessor)
        => httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    private static IList<string> GetUserRoles(IHttpContextAccessor httpContextAccessor)
        => httpContextAccessor.HttpContext?.User.Claims
            .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList()
        ?? [];
}
