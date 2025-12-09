using Flippit.Api.App.Helpers;
using Flippit.Api.BL.Facades;
using Flippit.Common.Models.User;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Flippit.Api.App.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder routeGroupBuilder)
    {
        var userEndPoints = routeGroupBuilder.MapGroup("users")
            .WithTags("users");

        userEndPoints.MapGet("", ([FromServices] IUserFacade userFacade, [FromQuery] string? filter = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null) => {
            var users = userFacade.GetAll(filter, sortBy, page, pageSize);
            return TypedResults.Ok(users);
        });

        userEndPoints.MapGet("/{id:guid}",
            Results<Ok<UserDetailModel>, NotFound<string>> (
                Guid id,
                [FromServices] IUserFacade userFacade
            )
            =>
            userFacade.GetById(id) is { } user
                ? TypedResults.Ok(user)
                : TypedResults.NotFound($"User with ID {id} was not found.")
        );

        userEndPoints.MapGet("/{id:guid}/collections", (ICollectionFacade collectionFacade, Guid id, [FromQuery] string ? filter = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string ? sortBy = null) =>
        {
            var collections = collectionFacade.SearchByCreatorId(id, filter, sortBy, page, pageSize);
            return TypedResults.Ok(collections);
        });

        userEndPoints.MapGet("/{userId:guid}/CompletedLessons", (ICompletedLessonFacade facade, Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null) =>
        {
            var AllCompletedLessons = facade.SearchByCreatorId(userId, sortBy, page, pageSize);
            return TypedResults.Ok(AllCompletedLessons);
        });

        userEndPoints.MapPost("", [Authorize] async Task<Results<Ok<Guid>, ValidationProblem>> ([FromBody] UserDetailModel model, IUserFacade userFacade, IValidator<UserDetailModel> validator) => {
            var validationErrors = await ValidationHelper.ValidateModelAsync(model, validator);
            if (validationErrors != null)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }
            var id = userFacade.Create(model);
            return TypedResults.Ok(id);
        });

        userEndPoints.MapPut("", [Authorize] async Task<Results<Ok<Guid?>, ValidationProblem>> ([FromBody] UserDetailModel model, IUserFacade userFacade, IValidator<UserDetailModel> validator) => {
            var validationErrors = await ValidationHelper.ValidateModelAsync(model, validator);
            if (validationErrors != null)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }
            var id = userFacade.Update(model);
            return TypedResults.Ok(id);
        });

        userEndPoints.MapPost("upsert", [Authorize] async Task<Results<Ok<Guid>, ValidationProblem>> ([FromBody] UserDetailModel model, IUserFacade userFacade, IValidator<UserDetailModel> validator) => {
            var validationErrors = await ValidationHelper.ValidateModelAsync(model, validator);
            if (validationErrors != null)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }
            var id = userFacade.CreateOrUpdate(model);
            return TypedResults.Ok(id);
        });

        userEndPoints.MapDelete("{id:guid}", [Authorize(Roles = "Admin")] (Guid id, IUserFacade userFacade) =>
        {
            userFacade.Delete(id);
        });

        return routeGroupBuilder;
    }
}
