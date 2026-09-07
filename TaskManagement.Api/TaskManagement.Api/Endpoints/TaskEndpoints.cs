using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOS;
using TaskManagement.Api.Interfaces;

namespace TaskManagement.Api.Endpoints
{
    public static class TaskEndpoints
    {
        public static void MapTaskEndpoints(this WebApplication app)
        {
            app.MapGet("/minimal/tasks", ([AsParameters] TaskQuery query, ITaskService taskService) =>
            {
                var result = taskService.GetAll(query);
                return Results.Ok(result);
            }).WithGroupName("v1");

            app.MapPost("/minimal/tasks", async([FromBody] CreateTaskDto createTaskDto, ITaskService taskService, CancellationToken cancellationToken) =>
            {
                var createdTask = await taskService.CreateAsync(createTaskDto, cancellationToken);
                return Results.Created($"/minimal/tasks/{createdTask.Id}", createdTask);
            }).WithGroupName("v1");
        }
    }
}
