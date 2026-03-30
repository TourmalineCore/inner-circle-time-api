using Application.ExternalDeps.AssignmentsApi;

namespace Application.Features.Internal.GetAllProjects;

public class GetAllProjectsHandler
{
    private readonly IAssignmentsApi _assignmentsApi;

    public GetAllProjectsHandler(
        IAssignmentsApi assignmentsApi
    )
    {
        _assignmentsApi = assignmentsApi;
    }

    public async Task<GetAllProjectsResponse> HandleAsync()
    {
        return new GetAllProjectsResponse
        {
            Projects = await _assignmentsApi.GetAllProjectsAsync()
        };
    }
}
