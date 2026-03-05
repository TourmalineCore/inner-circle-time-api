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
    And params { startDate: "2033-11-05", endDate: "2033-11-05" }
    When method GET
    Then status 200

    * def firstProjectId = response.projects[0].id
    * def secondProjectId = response.projects[1].id

    * def startTime = '2033-11-05T14:00:00'
    * def endTime = '2033-11-05T16:00:00'

    # Create a new task entry with Asia/Yekaterinburg time zone
    * def randomTitle = '[API-E2E]-Test-task-entry-' + Math.random()
    * def taskId = '#2233'
    * def description = 'Task description'
    * def timeZoneIdAsiaYekaterinburg = 'Asia/Yekaterinburg'
    
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
        "timeZoneId": "#(timeZoneIdAsiaYekaterinburg)"
    }
    """
    When method POST
    Then status 200

    * def newTaskEntryId1 = response.newTaskEntryId

    * def timeZoneIdEuropeMoscow = 'Europe/Moscow'
    
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
        "timeZoneId": "#(timeZoneIdEuropeMoscow)"
    }
    """
    When method POST
    Then status 200

    * def newTaskEntryId2 = response.newTaskEntryId

    # Verify task entry data
    Given path 'tracking/entries'
    And params { startDate: "2033-11-05", endDate: "2033-11-05" }
    When method GET
    And match response.taskEntries contains
    """
    {
        "id": "#(newTaskEntryId1)",
        "type": 1,
        "title": "#(randomTitle)",
        "startTime": "#(startTime)",
        "endTime": "#(endTime)",
        "projectId": #(firstProjectId),
        "taskId": "#(taskId)",
        "description": "#(description)",
    },
      {
        "id": "#(newTaskEntryId2)",
        "type": 1,
        "title": "#(randomTitle)",
        "startTime": "#(startTime)",
        "endTime": "#(endTime)",
        "projectId": #(firstProjectId),
        "taskId": "#(taskId)",
        "description": "#(description)",
    }
    """

    # Cleanup: Delete the task entry (hard delete)
    Given path 'tracking/entries', newTaskEntryId1, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup: Delete the task entry (hard delete)
    Given path 'tracking/entries', newTaskEntryId2, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that task entry was deleted
    Given path 'tracking/entries'
    And params { startDate: "2033-11-05", endDate: "2033-11-05" }
    When method GET
    Then status 200
    And assert response.taskEntries.filter(x => x.id == newTaskEntryId1).length == 0
    And assert response.taskEntries.filter(x => x.id == newTaskEntryId2).length == 0