Feature: Internal
    # https://github.com/karatelabs/karate/issues/1191
    # https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

  Background:
    * header Content-Type = 'application/json'

  Scenario: Internal Happy Path

    * def jsUtils = read('./js-utils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    * def rootUrl  = apiRootUrl.replace('/api', '')
    * def authSlytherineTenantDracoLoginWithAllPermissions = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_DRACO_MALFOY_LOGIN_WITH_ALL_PERMISSIONS')
    * def authSlytherineTenantDracoPasswordWithAllPermissions = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_DRACO_MALFOY_PASSWORD_WITH_ALL_PERMISSIONS')
    
    # Authentication
    Given url authApiRootUrl
    And path '/login'
    And request
    """
    {
        "login": "#(authSlytherineTenantDracoLoginWithAllPermissions)",
        "password": "#(authSlytherineTenantDracoPasswordWithAllPermissions)"
    }
    """
    And method POST
    Then status 200

    * def accessToken = karate.toMap(response.accessToken.value)

    * configure headers = jsUtils().getAuthHeaders(accessToken)

    # Get projects list
    Given url rootUrl 
    Given path 'internal/projects'
    When method GET
    Then status 200

    * def firstProjectId = response.projects[0].id

    # Create a new task entry
    * def randomTitle = '[API-E2E]-Test-task-entry-' + Math.random()
    * def startTime = '2033-11-05T14:00:00'
    * def endTime = '2033-11-05T16:00:00'
    * def taskId = '#2233'
    * def description = 'Task description'
    
    Given url apiRootUrl
    Given path 'tracking/task-entries'
    And request
    """
    {
        "title": "#(randomTitle)",
        "startTime": "#(startTime)",
        "endTime": "#(endTime)",
        "projectId": #(firstProjectId), 
        "taskId": "#(taskId)",
        "description": "#(description)",
    }
    """
    When method POST
    Then status 200

    * def newTaskEntryId = response.newTaskEntryId

    # Verify that entries by project are returned correctly
    Given url rootUrl 
    Given path 'internal/projects/tracked-task-hours'
    And params { projectId: "#(firstProjectId)", startDate: "2033-11-01", endDate: "2033-11-30" }
    When method GET
    And match response.employeesTrackedTaskHours contains
    """
    {
        "employeeId": "#number",
        "trackedHours": 2,
    }
    """

    # Cleanup: Delete the task entry (hard delete)
    Given url apiRootUrl
    Given path 'tracking/entries', newTaskEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that task entry was deleted
    Given path 'tracking/entries'
    And params { startDate: "2033-11-05", endDate: "2033-11-05" }
    When method GET
    Then status 200
    And assert response.taskEntries.filter(x => x.id == newTaskEntryId).length == 0