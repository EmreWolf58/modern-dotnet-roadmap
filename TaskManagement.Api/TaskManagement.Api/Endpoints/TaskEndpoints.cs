using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOS;
using TaskManagement.Api.Interfaces;

namespace TaskManagement.Api.Endpoints
{
    public static class TaskEndpoints
    {
        public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/minimal/tasks")
                .WithGroupName("v1")
                .WithTags("Minimal Tasks");

            group.MapGet("/", ([AsParameters] TaskQuery query, ITaskService taskService) =>
            {
                var result = taskService.GetAll(query);
                return Results.Ok(result);
            });

            group.MapPost("/", async([FromBody] CreateTaskDto createTaskDto, ITaskService taskService, CancellationToken cancellationToken) =>
            {
                var createdTask = await taskService.CreateAsync(createTaskDto, cancellationToken);
                return Results.Created($"/minimal/tasks/{createdTask.Id}", createdTask);
            });

            group.MapGet("/{id:int}", (int id, ITaskService taskService) =>
            {
                var task = taskService.GetById(id);

                if (task is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(task);
            });

            group.MapPut("/{id:int}", async (int id, UpdateTaskDto updateTaskDto, ITaskService taskService, CancellationToken cancellationToken) =>
            {
                var updateTask = await taskService.UpdateAsync(id, updateTaskDto, cancellationToken);

                if (updateTask is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(updateTask);
            });

            group.MapDelete("/{id:int}" , async (int id, ITaskService taskService, CancellationToken cancellationToken) =>
            {
                var deleted = await taskService.DeleteAsync(id, cancellationToken);

                if (!deleted)
                {
                    return Results.NotFound();
                }

                return Results.NoContent();
            });
        }
    }
}
