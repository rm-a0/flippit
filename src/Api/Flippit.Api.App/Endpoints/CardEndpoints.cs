using Flippit.Api.App.Helpers;
using Flippit.Api.BL.Facades;
using Flippit.Api.BL.Services;
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

        cardEndPoints.MapPost("", [Authorize] async Task<Results<Ok<Guid>, ValidationProblem>> (ICardFacade CardFacade, CardDetailModel model, IValidator<CardDetailModel> validator) =>
        {
            var validationErrors = await ValidationHelper.ValidateModelAsync(model, validator);
            if (validationErrors != null)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }
            var id = CardFacade.Create(model);
            return TypedResults.Ok(id);
        });

        cardEndPoints.MapPut("", [Authorize] async Task<Results<Ok<Guid?>, ValidationProblem, ForbidHttpResult, NotFound<string>>> (
            ICardFacade CardFacade, 
            CardDetailModel model, 
            IValidator<CardDetailModel> validator,
            ICurrentUserService currentUserService) =>
        {
            var validationErrors = await ValidationHelper.ValidateModelAsync(model, validator);
            if (validationErrors != null)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }

            // Check if card exists and user has permission
            var existingCard = CardFacade.GetById(model.Id);
            if (existingCard == null)
            {
                return TypedResults.NotFound($"Card with ID {model.Id} was not found.");
            }

            // Check authorization: must be owner or admin
            var currentUserId = currentUserService.CurrentUserId;
            var isAdmin = currentUserService.IsInRole("Admin");
            if (currentUserId != existingCard.CreatorId && !isAdmin)
            {
                return TypedResults.Forbid();
            }

            var id = CardFacade.Update(model);
            return TypedResults.Ok(id);
        });

        cardEndPoints.MapPost("upsert", [Authorize] async Task<Results<Ok<Guid>, ValidationProblem>> (ICardFacade CardFacade, CardDetailModel model, IValidator<CardDetailModel> validator) =>
        {
            var validationErrors = await ValidationHelper.ValidateModelAsync(model, validator);
            if (validationErrors != null)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }
            var id = CardFacade.CreateOrUpdate(model);
            return TypedResults.Ok(id);
        });

        cardEndPoints.MapDelete("/{id:guid}", [Authorize] Results<NoContent, ForbidHttpResult, NotFound<string>> (
            ICardFacade cardFacade, 
            Guid id,
            ICurrentUserService currentUserService) =>
        {
            // Check if card exists and user has permission
            var existingCard = cardFacade.GetById(id);
            if (existingCard == null)
            {
                return TypedResults.NotFound($"Card with ID {id} was not found.");
            }

            // Check authorization: must be owner or admin
            var currentUserId = currentUserService.CurrentUserId;
            var isAdmin = currentUserService.IsInRole("Admin");
            if (currentUserId != existingCard.CreatorId && !isAdmin)
            {
                return TypedResults.Forbid();
            }

            cardFacade.Delete(id);
            return TypedResults.NoContent();
        });

        return routeGroupBuilder;
    }
}
