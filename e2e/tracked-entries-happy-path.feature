Feature: Tracked entries
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
    Given path 'tracking/work-entries/projects'
    And params { startDate: "2028-11-05", endDate: "2028-11-05" }
    When method GET
    Then status 200

    * def firstProjectId = response.projects[0].id

    # Create a new work entry
    * def workEntryRandomTitle = '[API-E2E]-Test-work-entry-' + Math.random()
    * def workEntryStartTime = '2028-11-05T14:00:00'
    * def workEntryEndTime = '2028-11-05T16:00:00'
    * def workEntryTaskId = '#2233'
    * def workEntryDescription = 'Task description'
    
    Given path 'tracking/work-entries'
    And request
    """
    {
        "title": "#(workEntryRandomTitle)",
        "startTime": "#(workEntryStartTime)",
        "endTime": "#(workEntryEndTime)",
        "projectId": #(firstProjectId), 
        "taskId": "#(workEntryTaskId)",
        "description": "#(workEntryDescription)",
    }
    """
    When method POST
    Then status 200

    * def newWorkEntryId = response.newWorkEntryId

    * def unwellStartTime = '2028-11-05T17:00:00'
    * def unwellEndTime = '2028-11-05T18:00:00'

    # Create a new unwell entry
    Given path 'tracking/unwell-entries'
    And request
    """
    {
        "startTime": "#(unwellStartTime)",
        "endTime": "#(unwellEndTime)",
    }
    """
    When method POST
    Then status 200

    * def newUnwellEntryId = response.newUnwellEntryId

    # Verify work entries and unwell entries
    Given path 'tracking/work-entries'
    And params { startDate: "2028-11-05", endDate: "2028-11-05" }
    When method GET
    And match response.workEntries contains
    """
    {
        "id": "#(newWorkEntryId)",
        "type": 1,
        "title": "#(workEntryRandomTitle)",
        "startTime": "#(workEntryStartTime)",
        "endTime": "#(workEntryEndTime)",
        "projectId": #(firstProjectId),
        "taskId": "#(workEntryTaskId)",
        "description": "#(workEntryDescription)",
    }
    """
    And match response.unwellEntries contains
    """
    {
        "id": "#(newUnwellEntryId)",
        "type": 2,
        "startTime": "#(unwellStartTime)",
        "endTime": "#(unwellEndTime)",
    }
    """

    # Cleanup: Delete the work entry (hard delete)
    Given path 'tracking/work-entries', newWorkEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup: Delete the unwell entry (hard delete)
    Given path 'tracking/work-entries', newUnwellEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that work entry and unwell entry were deleted
    Given path 'tracking/work-entries'
    And params { startDate: "2028-11-05", endDate: "2028-11-05" }
    When method GET
    Then status 200
    And assert response.workEntries.filter(x => x.id == newWorkEntryId).length == 0
    And assert response.unwellEntries.filter(x => x.id == newUnwellEntryId).length == 0
