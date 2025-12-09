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

        cardEndPoints.MapPut("", [Authorize] async Task<Results<Ok<Guid?>, ValidationProblem>> (ICardFacade CardFacade, CardDetailModel model, IValidator<CardDetailModel> validator) =>
        {
            var validationErrors = await ValidationHelper.ValidateModelAsync(model, validator);
            if (validationErrors != null)
            {
                return TypedResults.ValidationProblem(validationErrors);
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

        cardEndPoints.MapDelete("/{id:guid}", [Authorize(Roles = "Admin")] (ICardFacade cardFacade, Guid id) =>
        {
            cardFacade.Delete(id);
        });

        return routeGroupBuilder;
    }
}
