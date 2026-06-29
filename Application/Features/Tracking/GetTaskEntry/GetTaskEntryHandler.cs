using Application.SharedQueries;
using Core.Entities;

namespace Application.Features.Tracking.GetTaskEntry;

public class GetTaskEntryHandler
{
    private readonly IGetEntryByIdQuery _getEntryByIdQuery;

    public GetTaskEntryHandler(
        IGetEntryByIdQuery getEntryByIdQuery
    )
    {
        _getEntryByIdQuery = getEntryByIdQuery;
    }

    public async Task<GetTaskEntryResponse> HandleAsync(long taskEntryId)
    {
        var taskEntry = await _getEntryByIdQuery.GetAsync<TaskEntry>(taskEntryId);

        if (taskEntry == null)
        {
            throw new ArgumentException($"Task Entry with id {taskEntryId} does not exist");
        }

        return new GetTaskEntryResponse
        {
            TaskEntry = new GetTaskEntryDto
            {
                Id = taskEntry.Id,
                StartTime = taskEntry.StartTime,
                EndTime = taskEntry.EndTime,
                Type = taskEntry.Type,
                Title = taskEntry.Title,
                ProjectId = taskEntry.ProjectId,
                TaskId = taskEntry.TaskId,
                Description = taskEntry.Description
            },
        };
    }
}
