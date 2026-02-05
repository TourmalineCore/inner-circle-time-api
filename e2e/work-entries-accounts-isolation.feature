Feature: Work Entries
    # https://github.com/karatelabs/karate/issues/1191
    # https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

  Background:
    * header Content-Type = 'application/json'

  Scenario: Accounts Isolation

    * def jsUtils = read('./js-utils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    * def authFirstAccountLogin = jsUtils().getEnvVariable('AUTH_FIRST_TENANT_LOGIN_WITH_ALL_PERMISSIONS')
    * def authFirstAccountPassword = jsUtils().getEnvVariable('AUTH_FIRST_TENANT_PASSWORD_WITH_ALL_PERMISSIONS')
    * def authSecondAccountLogin = jsUtils().getEnvVariable('AUTH_FIRST_TENANT_SECOND_ACCOUNT_LOGIN_WITH_ALL_PERMISSIONS') 
    * def authSecondAccountPassword = jsUtils().getEnvVariable('AUTH_FIRST_TENANT_SECOND_ACCOUNT_PASSWORD_WITH_ALL_PERMISSIONS')
    
    # Authentication
    Given url authApiRootUrl
    And path '/login'
    And request
    """
    {
        "login": "#(authFirstAccountLogin)",
        "password": "#(authFirstAccountPassword)"
    }
    """
    And method POST
    Then status 200

    * def firstAccountAccessToken = karate.toMap(response.accessToken.value)

    * configure headers = jsUtils().getAuthHeaders(firstAccountAccessToken)

    # Get employee's projects
    # Here we specified 2026 year to avoid conflicts with other tests
    Given url apiRootUrl
    Given path 'tracking/work-entries/projects'
    And params { startDate: "2026-11-06", endDate: "2026-11-06" }
    When method GET
    Then status 200

    * def firtAccountProjectId = response.projects[0].id

    # Create a new work entry
    # Here we specified 2026 year to avoid conflicts with other tests
    * def randomTitle = '[API-E2E]-Test-work-entry-' + Math.random()
    * def startTime = '2026-12-05T14:00:00'
    * def endTime = '2026-12-05T16:00:00'
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
        "projectId": #(firtAccountProjectId),
        "taskId": "#(taskId)",
        "description": "#(description)",
    }
    """
    When method POST
    Then status 200

    * def newWorkEntryId = response.newWorkEntryId

    # Authentication with other account
    Given url authApiRootUrl
    And path '/login'
    And request
    """
    {
        "login": "#(authSecondAccountLogin)",
        "password": "#(authSecondAccountPassword)"
    }
    """
    And method POST
    Then status 200

    * def secondAccountAccessToken = karate.toMap(response.accessToken.value)

    * configure headers = jsUtils().getAuthHeaders(secondAccountAccessToken)

    # Cannot get work entries created within another Account
    Given url apiRootUrl
    Given path 'tracking/work-entries'
    And params { startDate: "2026-11-06", endDate: "2026-11-06" }
    When method GET
    Then status 200
    And assert response.workEntries.filter(x => x.id == newWorkEntryId).length == 0

    # Cannot delete work entry of another Account
    Given path 'tracking/work-entries', newWorkEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: false }

    * configure headers = jsUtils().getAuthHeaders(firstAccountAccessToken)

    # Cleanup: Delete the work entry (hard delete)
    Given path 'tracking/work-entries', newWorkEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that work entry was deleted
    Given path 'tracking/work-entries'
    And params { startDate: "2026-11-06", endDate: "2026-11-06" }
    When method GET
    Then status 200
    And assert response.workEntries.filter(x => x.id == newWorkEntryId).length == 0
