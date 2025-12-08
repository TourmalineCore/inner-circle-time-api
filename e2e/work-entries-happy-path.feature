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

    # Step 1: Create a new work entry
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

    # Step 2: Verify that work entry is in the database with the id, title, taskId, startTime and endTime
    Given url apiRootUrl
    Given path 'tracking/work-entries'
    And params { startTime: "2025-11-05T00:00:00", endTime: "2025-11-05T23:59:59" }
    When method GET
    And match response.workEntries contains
    """
    {
        "id": "#(newWorkEntryId)",
        "title": "#(randomTitle)",
        "startTime": "#(startTime)",
        "endTime": "#(endTime)",
        "taskId": "#(taskId)",
    }
    """

    # Cleanup: Delete the work entry (hard delete)
    Given url apiRootUrl
    Given path 'tracking/work-entries', newWorkEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that work entry was deleted
    Given url apiRootUrl
    Given path 'tracking/work-entries'
    And params { startTime: "2025-11-05T00:00:00", endTime: "2025-11-05T23:59:59" }
    When method GET
    And assert response.workEntries.filter(x => x.id == newWorkEntryId).length == 0
