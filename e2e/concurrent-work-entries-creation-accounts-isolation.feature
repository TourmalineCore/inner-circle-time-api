Feature: Work Entries
    # https://github.com/karatelabs/karate/issues/1191
    # https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

  Background:
    * header Content-Type = 'application/json'

  Scenario: Concurrent Work Entries Creation Accounts Isolation

    * def jsUtils = read('./js-utils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    * def authFirstAccountLogin = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_DRACO_MALFOY_LOGIN_WITH_ALL_PERMISSIONS')
    * def authFirstAccountPassword = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_DRACO_MALFOY_PASSWORD_WITH_ALL_PERMISSIONS')
    * def authSecondAccountLogin = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_SEVERUS_SNAPE_LOGIN_WITH_ALL_PERMISSIONS') 
    * def authSecondAccountPassword = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_SEVERUS_SNAPE_PASSWORD_WITH_ALL_PERMISSIONS')
    
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

    # Get employee's projects in first account
    # Here we specified 2028 year to avoid conflicts with other tests
    Given url apiRootUrl
    Given path 'tracking/work-entries/projects'
    And params { startDate: "2028-11-06", endDate: "2028-11-06" }
    When method GET
    Then status 200

    * def firstAccountProjectId = response.projects[0].id

    # Create a new work entry in first account
    # Here we specified 2028 year to avoid conflicts with other tests
    * def randomTitle = '[API-E2E]-Test-work-entry-' + Math.random()
    * def startTime = '2028-12-05T14:00:00'
    * def endTime = '2028-12-05T16:00:00'
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
        "projectId": #(firstAccountProjectId), 
        "taskId": "#(taskId)",
        "description": "#(description)",
    }
    """
    When method POST
    Then status 200

    * def firstAccountNewWorkEntryId = response.newWorkEntryId

    # Authentication with second account
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

    # Get employee's projects in second account
    # Here we specified 2028 year to avoid conflicts with other tests
    Given url apiRootUrl
    Given path 'tracking/work-entries/projects'
    And params { startDate: "2028-11-06", endDate: "2028-11-06" }
    When method GET
    Then status 200

    * def secondAccountProjectId = response.projects[0].id

    # Create a new work entry in second account with same time
    Given url apiRootUrl
    Given path 'tracking/work-entries'
    And request
    """
    {
        "title": "#(randomTitle)",
        "startTime": "#(startTime)",
        "endTime": "#(endTime)",
        "projectId": #(secondAccountProjectId), 
        "taskId": "#(taskId)",
        "description": "#(description)",
    }
    """
    When method POST
    Then status 200

    * def secondAccountNewWorkEntryId = response.newWorkEntryId

    # Cleanup: Delete work entry in second account
    Given path 'tracking/work-entries', secondAccountNewWorkEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that work entry in second account was deleted
    Given path 'tracking/work-entries'
    And params { startDate: "2028-11-06", endDate: "2028-11-06" }
    When method GET
    Then status 200
    And assert response.workEntries.filter(x => x.id == secondAccountNewWorkEntryId).length == 0

    * configure headers = jsUtils().getAuthHeaders(firstAccountAccessToken)

    # Cleanup: Delete work entry in first account
    Given path 'tracking/work-entries', firstAccountNewWorkEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that work entry in first account was deleted
    Given path 'tracking/work-entries'
    And params { startDate: "2028-11-06", endDate: "2028-11-06" }
    When method GET
    Then status 200
    And assert response.workEntries.filter(x => x.id == firstAccountNewWorkEntryId).length == 0


