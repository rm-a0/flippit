using Flippit.Api.App.Helpers;
using Flippit.Api.BL.Facades;
using Flippit.Common.Models.Card;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Flippit.Api.App.Endpoints;

public static class CardEndpoints
{
    public static RouteGroupBuilder MapCardEndpoints(this RouteGroupBuilder routeGroupBuilder)
    {
        var cardEndPoints = routeGroupBuilder.MapGroup("cards")
            .WithTags("cards");

        cardEndPoints.MapGet("", (ICardFacade cardFacade, [FromQuery] string? filter = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null) =>
        {
            var cards = cardFacade.GetAll(filter, sortBy, page, pageSize);
            return TypedResults.Ok(cards);
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

        var cardModifyingEndpoints = cardEndPoints.MapGroup("")
            .RequireAuthorization();

        cardModifyingEndpoints.MapPost("", async Task<Results<Ok<Guid>, ValidationProblem>> (ICardFacade CardFacade, CardDetailModel model, IValidator<CardDetailModel> validator, IHttpContextAccessor httpContextAccessor) =>
        {
            var validationErrors = await ValidationHelper.ValidateModelAsync(model, validator);
            if (validationErrors != null)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);
            var id = CardFacade.Create(model, userRoles, userId);
            return TypedResults.Ok(id);
        });

        cardModifyingEndpoints.MapPut("", async Task<Results<Ok<Guid?>, ValidationProblem, ForbidHttpResult>> (ICardFacade CardFacade, CardDetailModel model, IValidator<CardDetailModel> validator, IHttpContextAccessor httpContextAccessor) =>
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
                var id = CardFacade.Update(model, userRoles, userId);
                return TypedResults.Ok(id);
            }
            catch (UnauthorizedAccessException)
            {
                return TypedResults.Forbid();
            }
        });

        cardModifyingEndpoints.MapPost("upsert", async Task<Results<Ok<Guid>, ValidationProblem>> (ICardFacade CardFacade, CardDetailModel model, IValidator<CardDetailModel> validator, IHttpContextAccessor httpContextAccessor) =>
        {
            var validationErrors = await ValidationHelper.ValidateModelAsync(model, validator);
            if (validationErrors != null)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);
            var id = CardFacade.CreateOrUpdate(model, userRoles, userId);
            return TypedResults.Ok(id);
        });

        cardModifyingEndpoints.MapDelete("/{id:guid}", Results<Ok, ForbidHttpResult> (ICardFacade cardFacade, Guid id, IHttpContextAccessor httpContextAccessor) =>
        {
            var userId = GetUserId(httpContextAccessor);
            var userRoles = GetUserRoles(httpContextAccessor);
            try
            {
                cardFacade.Delete(id, userRoles, userId);
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
