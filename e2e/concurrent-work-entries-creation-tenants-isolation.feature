Feature: Work Entries
    # https://github.com/karatelabs/karate/issues/1191
    # https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

  Background:
    * header Content-Type = 'application/json'

  Scenario: Concurrent Work Entries Creation Tenants Isolation

    * def jsUtils = read('./js-utils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    * def authFirstTenantLogin = jsUtils().getEnvVariable('AUTH_FIRST_TENANT_LOGIN_WITH_ALL_PERMISSIONS')
    * def authFirstTenantPassword = jsUtils().getEnvVariable('AUTH_FIRST_TENANT_PASSWORD_WITH_ALL_PERMISSIONS')
    * def authSecondTenantLogin = jsUtils().getEnvVariable('AUTH_SECOND_TENANT_LOGIN_WITH_ALL_PERMISSIONS') 
    * def authSecondTenantPassword = jsUtils().getEnvVariable('AUTH_SECOND_TENANT_PASSWORD_WITH_ALL_PERMISSIONS')
    
    # Authentication
    Given url authApiRootUrl
    And path '/login'
    And request
    """
    {
        "login": "#(authFirstTenantLogin)",
        "password": "#(authFirstTenantPassword)"
    }
    """
    And method POST
    Then status 200

    * def firstTenantAccessToken = karate.toMap(response.accessToken.value)

    * configure headers = jsUtils().getAuthHeaders(firstTenantAccessToken)

    # Create a new work entry in first tenant
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
        "taskId": "#(taskId)",
        "description": "#(description)",
    }
    """
    When method POST
    Then status 200

    * def firstTenantNewWorkEntryId = response.newWorkEntryId

    # Authentication with second tenant
    Given url authApiRootUrl
    And path '/login'
    And request
    """
    {
        "login": "#(authSecondTenantLogin)",
        "password": "#(authSecondTenantPassword)"
    }
    """
    And method POST
    Then status 200

    * def secondTenantAccessToken = karate.toMap(response.accessToken.value)

    * configure headers = jsUtils().getAuthHeaders(secondTenantAccessToken)

    # Create a new work entry in second tenant with same time
    Given url apiRootUrl
    Given path 'tracking/work-entries'
    And request
    """
    {
        "title": "#(randomTitle)",
        "startTime": "#(startTime)",
        "endTime": "#(endTime)", 
        "taskId": "#(taskId)",
        "description": "#(description)",
    }
    """
    When method POST
    Then status 200

    * def secondTenantNewWorkEntryId = response.newWorkEntryId

    # Cleanup: Delete work entry in second tenant
    Given path 'tracking/work-entries', secondTenantNewWorkEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that work entry in second tenant was deleted
    Given path 'tracking/work-entries'
    And params { startDate: "2027-11-06", endDate: "2027-11-06" }
    When method GET
    Then status 200
    And assert response.workEntries.filter(x => x.id == secondTenantNewWorkEntryId).length == 0

    * configure headers = jsUtils().getAuthHeaders(firstTenantAccessToken)

    # Cleanup: Delete work entry in first tenant
    Given path 'tracking/work-entries', firstTenantNewWorkEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that work entry in first tenant was deleted
    Given path 'tracking/work-entries'
    And params { startDate: "2027-11-06", endDate: "2027-11-06" }
    When method GET
    Then status 200
    And assert response.workEntries.filter(x => x.id == firstTenantNewWorkEntryId).length == 0


