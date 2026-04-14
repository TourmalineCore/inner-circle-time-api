Feature: Personal Reporting
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

    * def holderEmployeeId = jsUtils().getEmployeeIdFromToken(response.accessToken.value)

    # Verify: Get employee's
    Given url apiRootUrl
    Given path 'reporting/employees'
    When method GET
    Then status 200
    And assert response.employees.length > 0

    # Get employee's projects
    Given path 'tracking/task-entries/projects'
    And params { startDate: "2033-11-05", endDate: "2033-11-05" }
    When method GET
    Then status 200

    * def firstProjectId = response.projects[0].id

    # Create a new task entry
    * def randomTitle = '[API-E2E]-Test-task-entry-' + Math.random()
    * def startTime = '2033-11-05T14:00:00'
    * def endTime = '2033-11-05T16:00:00'
    * def taskId = '#2233'
    * def description = 'Task description'
    
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

    # Verify: Get personal report
    Given path 'reporting/personal-report'
    And params { employeeId: "#(holderEmployeeId)", year: "2033", month: "11" }
    When method GET
    And match response.trackedEntries contains
    """
    {
        "id": "#number",
        "trackedHoursPerDay": 2,
        "startTime": "#(startTime)",
        "endTime": "#(endTime)",
        "hours": 2,
        "entryType": 1,
        "project": {
            "id": #(firstProjectId),
            "name": "#string"
        },
        "task": {
            "id": "#(taskId)",
            "title": "#(randomTitle)"
        },
        "description": "#(description)"
    }
    """
    And assert response.taskHours == 2
    And assert response.unwellHours == 0
    

    # Cleanup: Delete the task entry (hard delete)
    Given path 'tracking/entries', newTaskEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that task entry was deleted
    Given path 'tracking/entries'
    And params { startDate: "2033-11-05", endDate: "2031-11-05" }
    When method GET
    Then status 200
    And assert response.taskEntries.filter(x => x.id == newTaskEntryId).length == 0