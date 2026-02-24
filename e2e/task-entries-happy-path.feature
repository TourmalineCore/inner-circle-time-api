Feature: Task Entries
    # https://github.com/karatelabs/karate/issues/1191
    # https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

  Background:
    * header Content-Type = 'application/json'

  Scenario: Happy Path

    * def jsUtils = read('./js-utils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
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

    # Get employee's projects
    Given url apiRootUrl
    Given path 'tracking/task-entries/projects'
    And params { startDate: "2030-11-05", endDate: "2030-11-05" }
    When method GET
    Then status 200

    * def firstProjectId = response.projects[0].id
    * def secondProjectId = response.projects[1].id

    # Create a new task entry
    * def randomTitle = '[API-E2E]-Test-task-entry-' + Math.random()
    * def startTime = '2030-11-05T14:00:00'
    * def endTime = '2030-11-05T16:00:00'
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

    # Update task entry
    * def newRandomTitle = '[API-E2E]-Test-task-entry-' + Math.random()
    * def newStartTime = '2030-11-06T11:00:00'
    * def newEndTime = '2030-11-06T12:00:00'
    * def newTaskId = '#2235'
    * def newDescription = 'New task description'
    
    Given path 'tracking/task-entries', newTaskEntryId
    And request
    """
    {
        "title": "#(newRandomTitle)",
        "startTime": "#(newStartTime)",
        "endTime": "#(newEndTime)",
        "projectId": #(secondProjectId), 
        "taskId": "#(newTaskId)",
        "description": "#(newDescription)",
    }
    """
    When method POST
    Then status 200

    # Verify updated task entry data
    Given path 'tracking/entries'
    And params { startDate: "2030-11-06", endDate: "2030-11-06" }
    When method GET
    And match response.taskEntries contains
    """
    {
        "id": "#(newTaskEntryId)",
        "type": 1,
        "title": "#(newRandomTitle)",
        "startTime": "#(newStartTime)",
        "endTime": "#(newEndTime)",
        "projectId": #(secondProjectId),
        "taskId": "#(newTaskId)",
        "description": "#(newDescription)",
    }
    """

    # Cleanup: Delete the task entry (hard delete)
    Given path 'tracking/entries', newTaskEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that task entry was deleted
    Given path 'tracking/entries'
    And params { startDate: "2030-11-06", endDate: "2030-11-06" }
    When method GET
    Then status 200
    And assert response.taskEntries.filter(x => x.id == newTaskEntryId).length == 0