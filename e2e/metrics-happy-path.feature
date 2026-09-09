Feature: Metrics
    # https://github.com/karatelabs/karate/issues/1191
    # https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

  Background:
    * header Content-Type = 'application/json'

    # Use case:
    # I didn't feel well from 8 a.m. to 12 p.m. on Monday.
    # And after lunch, I worked from 1 p.m. to 5 p.m. on Monday.
    # On Tuesday, I only tracked a stand-up from 8 a.m. to 8:20 a.m.
    # I keep track of this time in the time tracker.
    # Then I want the total tracked time to be 8.3(3) hours.
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

    # Create a new unwell entry on Monday
    * def unwellStartTime = '2028-09-04T08:00:00'
    * def unwellEndTime = '2028-09-04T12:00:00'
    
    Given url apiRootUrl
    Given path 'tracking/unwell-entries'
    And request
    """
    {
        "startTime": "#(unwellStartTime)",
        "endTime": "#(unwellEndTime)"
    }
    """
    When method POST
    Then status 200

    * def newUnwellEntryId = response.newUnwellEntryId

    # Get employee's projects
    Given path 'tracking/task-entries/projects'
    And params { startDate: "2028-09-04", endDate: "2028-09-05" }
    When method GET
    Then status 200

    * def firstProjectId = response.projects[0].id

    # Create a new task entry on Monday
    * def mondayTaskTitle = '[API-E2E]-Test-task-entry-' + Math.random()
    * def mondayTaskStartTime = '2028-09-04T13:00:00'
    * def mondayTaskEndTime = '2028-09-04T17:00:00'
    * def mondayTaskId = '#2233'
    * def mondayTaskDescription = 'Task description'
    
    Given path 'tracking/task-entries'
    And request
    """
    {
        "title": "#(mondayTaskTitle)",
        "startTime": "#(mondayTaskStartTime)",
        "endTime": "#(mondayTaskEndTime)",
        "projectId": #(firstProjectId), 
        "taskId": "#(mondayTaskId)",
        "description": "#(mondayTaskDescription)"
    }
    """
    When method POST
    Then status 200

    * def mondayNewTaskEntryId = response.newTaskEntryId

    # Create a new task entry on Tuesday
    * def tuesdayTaskTitle = 'Stand-up'
    * def tuesdayTaskStartTime = '2028-09-05T08:00:00'
    * def tuesdayTaskEndTime = '2028-09-05T08:20:00'
    * def tuesdayTaskId = '#1'
    * def tuesdayTaskDescription = 'Stand-up'
    
    Given path 'tracking/task-entries'
    And request
    """
    {
        "title": "#(tuesdayTaskTitle)",
        "startTime": "#(tuesdayTaskStartTime)",
        "endTime": "#(tuesdayTaskEndTime)",
        "projectId": #(firstProjectId), 
        "taskId": "#(tuesdayTaskId)",
        "description": "#(tuesdayTaskDescription)"
    }
    """
    When method POST
    Then status 200

    * def tuesdayNewTaskEntryId = response.newTaskEntryId

    # Get metrics
    Given path 'reporting/metrics'
    And params { startDate: "2028-09-04", endDate: "2028-09-10" }
    When method GET
    Then status 200
    # We expect 8 hours 20 minutes = 8.3(3) hours.
    # The backend may round it differently because the fraction is infinite.
    # So we check that the value is between 8.333333 and 8.333334.
    And match response.unwellHours == '#? _ > 8.333333 && _ < 8.333334'
    
    # Cleanup: Delete the unwell entry on Monday (hard delete)
    Given path 'tracking/entries', newUnwellEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }
    
    # Cleanup: Delete the task entry on Monday (hard delete)
    Given path 'tracking/entries', mondayNewTaskEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup: Delete the task entry on Tuesday (hard delete)
    Given path 'tracking/entries', tuesdayNewTaskEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that all entries was deleted
    Given path 'tracking/entries'
    And params { startDate: "2028-09-04", endDate: "2028-09-05" }
    When method GET
    Then status 200
    And assert response.taskEntries.filter(x => x.id == mondayNewTaskEntryId).length == 0
    And assert response.taskEntries.filter(x => x.id == tuesdayNewTaskEntryId).length == 0
    And assert response.unwellEntries.filter(x => x.id == newUnwellEntryId).length == 0