using Flippit.Api.BL.Facades;
using Flippit.Common.Models.CompletedLesson;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flippit.Api.App.Endpoints;

public static class CompletedLessonEndpoints
{
    public static RouteGroupBuilder MapCompletedLessonEndpoints(this RouteGroupBuilder routeGroupBuilder)
    {
        var completedLessonsEndPoints = routeGroupBuilder.MapGroup("completedLessons")
            .WithTags("completedLessons");

        completedLessonsEndPoints.MapGet("", (ICompletedLessonFacade facade, [FromQuery] int pageSize = 10, [FromQuery] int page = 1, [FromQuery] string? filter = null, [FromQuery] string? sortBy = null) =>
        {
            var lessons = facade.GetAll(filter, sortBy, page, pageSize);
            return TypedResults.Ok(lessons);
        });

        completedLessonsEndPoints.MapGet("/{id:guid}", (ICompletedLessonFacade facade, Guid id) =>
        {
            var lesson = facade.GetById(id);
            return TypedResults.Ok(lesson);
        });

        completedLessonsEndPoints.MapPost("", [Authorize] (ICompletedLessonFacade facade, CompletedLessonDetailModel model) =>
        {
            var id = facade.Create(model);
            return TypedResults.Ok(id);
        });

        completedLessonsEndPoints.MapPut("", [Authorize] (ICompletedLessonFacade facade, CompletedLessonDetailModel model) =>
        {
            var id = facade.Update(model);
            return TypedResults.Ok(id);
        });

        completedLessonsEndPoints.MapPost("upsert", [Authorize] (ICompletedLessonFacade facade, CompletedLessonDetailModel model) =>
        {
            var id = facade.CreateOrUpdate(model);
            return TypedResults.Ok(id);
        });

        completedLessonsEndPoints.MapDelete("/{id:guid}", [Authorize(Roles = "Admin")] (ICompletedLessonFacade facade, Guid id) =>
        {
            facade.Delete(id);
        });

        return routeGroupBuilder;
    }
}
