Feature: Work Entries
    # https://github.com/karatelabs/karate/issues/1191
    # https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

  Background:
    * header Content-Type = 'application/json'

  Scenario: Happy Path

    * def jsUtils = read('./js-utils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    * def authLogin = jsUtils().getEnvVariable('AUTH_FIRST_TENANT_LOGIN_WITH_ALL_PERMISSIONS')
    * def authPassword = jsUtils().getEnvVariable('AUTH_FIRST_TENANT_PASSWORD_WITH_ALL_PERMISSIONS')
    
    # Authentication
    Given url authApiRootUrl
    And path '/login'
    And request
    """
    {
        "login": "#(authLogin)",
        "password": "#(authPassword)"
    }
    """
    And method POST
    Then status 200

    * def accessToken = karate.toMap(response.accessToken.value)

    * configure headers = jsUtils().getAuthHeaders(accessToken)

    # Create a new work entry
    * def randomTitle = '[API-E2E]-Test-work-entry-' + Math.random()
    * def startTime = '2025-11-05T14:00:00'
    * def endTime = '2025-11-05T16:00:00'
    * def taskId = '#2233'
    
    Given url apiRootUrl
    Given path 'tracking/work-entries'
    And request
    """
    {
        "title": "#(randomTitle)",
        "startTime": "#(startTime)",
        "endTime": "#(endTime)", 
        "taskId": "#(taskId)",
    }
    """
    When method POST
    Then status 200

    * def newWorkEntryId = response.newWorkEntryId

    # Update work entry
    * def newRandomTitle = '[API-E2E]-Test-work-entry-' + Math.random()
    * def newStartTime = '2025-11-06T11:00:00'
    * def newEndTime = '2025-11-06T12:00:00'
    * def newTaskId = '#2235'
    
    Given path 'tracking/work-entries', newWorkEntryId
    And request
    """
    {
        "title": "#(newRandomTitle)",
        "startTime": "#(newStartTime)",
        "endTime": "#(newEndTime)", 
        "taskId": "#(newTaskId)",
    }
    """
    When method POST
    Then status 200

    # Verify updated work entry data
    Given path 'tracking/work-entries'
    And params { startTime: "2025-11-06T00:00:00", endTime: "2025-11-06T23:59:59" }
    When method GET
    And match response.workEntries contains
    """
    {
        "id": "#(newWorkEntryId)",
        "title": "#(newRandomTitle)",
        "startTime": "#(newStartTime)",
        "endTime": "#(newEndTime)",
        "taskId": "#(newTaskId)",
    }
    """

    # Cleanup: Delete the work entry (hard delete)
    Given path 'tracking/work-entries', newWorkEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that work entry was deleted
    Given path 'tracking/work-entries'
    And params { startTime: "2025-11-06T00:00:00", endTime: "2025-11-06T23:59:59" }
    When method GET
    Then status 200
    And assert response.workEntries.filter(x => x.id == newWorkEntryId).length == 0
