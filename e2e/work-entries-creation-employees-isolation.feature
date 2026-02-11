Feature: Work Entries
    # https://github.com/karatelabs/karate/issues/1191
    # https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

  Background:
    * header Content-Type = 'application/json'

  Scenario: Work Entries Creation Employees Isolation

    * def jsUtils = read('./js-utils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    * def authSlytherineTenantDracoLoginWithAllPermissions = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_DRACO_MALFOY_LOGIN_WITH_ALL_PERMISSIONS')
    * def authSlytherineTenantDracoPasswordWithAllPermissions = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_DRACO_MALFOY_PASSWORD_WITH_ALL_PERMISSIONS')
    * def authSlytherineTenantSeverusLoginWithAllPermissions = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_SEVERUS_SNAPE_LOGIN_WITH_ALL_PERMISSIONS') 
    * def authSlytherineTenantSeverusPasswordWithAllPermissions = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_SEVERUS_SNAPE_PASSWORD_WITH_ALL_PERMISSIONS')
    
    # Authentication with Draco's credentials
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

    * def dracoAccountAccessToken = karate.toMap(response.accessToken.value)

    * configure headers = jsUtils().getAuthHeaders(dracoAccountAccessToken)

    # Get employee's projects for Draco
    # Here we specified 2027 year to avoid conflicts with other tests
    Given url apiRootUrl
    Given path 'tracking/work-entries/projects'
    And params { startDate: "2027-11-06", endDate: "2027-11-06" }
    When method GET
    Then status 200

    * def dracoAccountProjectId = response.projects[0].id

    # Create a new work entry for Draco
    # Here we specified 2027 year to avoid conflicts with other tests
    * def randomTitle = '[API-E2E]-Test-work-entry-' + Math.random()
    * def startTime = '2027-12-05T14:00:00'
    * def endTime = '2027-12-05T16:00:00'
    * def taskId = '#2233'
    * def description = 'Task description'
    
    Given url apiRootUrl
    Given path 'tracking/work-entries'
    And request
    """
    {
        "title": "#(randomTitle)",
        "startTime": "#(startTime)",
        "endTime": "#(endTime)",
        "projectId": #(dracoAccountProjectId), 
        "taskId": "#(taskId)",
        "description": "#(description)",
    }
    """
    When method POST
    Then status 200

    * def dracoAccountNewWorkEntryId = response.newWorkEntryId

    # Authentication with Severus' credentials
    Given url authApiRootUrl
    And path '/login'
    And request
    """
    {
        "login": "#(authSlytherineTenantSeverusLoginWithAllPermissions)",
        "password": "#(authSlytherineTenantSeverusPasswordWithAllPermissions)"
    }
    """
    And method POST
    Then status 200

    * def severusAccountAccessToken = karate.toMap(response.accessToken.value)

    * configure headers = jsUtils().getAuthHeaders(severusAccountAccessToken)

    # Get employee's projects for Severus
    # Here we specified 2027 year to avoid conflicts with other tests
    Given url apiRootUrl
    Given path 'tracking/work-entries/projects'
    And params { startDate: "2027-11-06", endDate: "2027-11-06" }
    When method GET
    Then status 200

    * def severusAccountProjectId = response.projects[0].id

    # Create a new work entry for Severus with the same time as Draco
    Given url apiRootUrl
    Given path 'tracking/work-entries'
    And request
    """
    {
        "title": "#(randomTitle)",
        "startTime": "#(startTime)",
        "endTime": "#(endTime)",
        "projectId": #(severusAccountProjectId), 
        "taskId": "#(taskId)",
        "description": "#(description)",
    }
    """
    When method POST
    Then status 200

    * def severusAccountNewWorkEntryId = response.newWorkEntryId

    # Cleanup: Delete Severus' work entry
    Given path 'tracking/work-entries', severusAccountNewWorkEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that Severus' work entry was deleted
    Given path 'tracking/work-entries'
    And params { startDate: "2027-11-06", endDate: "2027-11-06" }
    When method GET
    Then status 200
    And assert response.workEntries.filter(x => x.id == severusAccountNewWorkEntryId).length == 0

    * configure headers = jsUtils().getAuthHeaders(dracoAccountAccessToken)

    # Cleanup: Delete Draco's work entry
    Given path 'tracking/work-entries', dracoAccountNewWorkEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that  Draco's work entry was deleted
    Given path 'tracking/work-entries'
    And params { startDate: "2027-11-06", endDate: "2027-11-06" }
    When method GET
    Then status 200
    And assert response.workEntries.filter(x => x.id == dracoAccountNewWorkEntryId).length == 0


