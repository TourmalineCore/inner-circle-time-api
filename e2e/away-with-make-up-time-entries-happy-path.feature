Feature: Away with Make Up Time Entries
    # https://github.com/karatelabs/karate/issues/1191
    # https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

    # Use case:  
    # I was away for two hours
    # I want a 30-minute make-up tomorrow and 1 hour 30 minutes the day after tomorrow
    # Then I realized I can't do the make-up tomorrow and want to move the 30-minute make-up to another day.
  Scenario: I was away for two hours

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

    # Create a new away with two make up time entry
    * def startTime = '2034-11-05T14:00:00'
    * def endTime = '2034-11-05T16:00:00'
    * def description = '[API-E2E]-Away description-'+ Math.random()
    * def makeUpTime1StartTime = '2034-11-06T17:00:00'
    * def makeUpTime1EndTime = '2034-11-06T17:30:00'
    * def makeUpTime2StartTime = '2034-11-07T17:00:00'
    * def makeUpTime2EndTime = '2034-11-07T18:30:00'
    
    Given url apiRootUrl
    Given path 'tracking/away-with-make-up-time-entries'
    And request
    """
    {
        "startTime": "#(startTime)",
        "endTime": "#(endTime)",
        "description": "#(description)",
        "makeUpTimeList": [
            {
                "startTime": "#(makeUpTime1StartTime)",
                "endTime": "#(makeUpTime1EndTime)"
            },
            {
                "startTime": "#(makeUpTime2StartTime)",
                "endTime": "#(makeUpTime2EndTime)"
            }
        ]
    }
    """
    When method POST
    Then status 200

    * def newAwayWithMakeUpTimeEntryId = response.newAwayWithMakeUpTimeEntryId

    # Update a first make up time entry
    * def rescheduledMakeUpTime1StartTime = '2034-11-08T18:00:00'
    * def rescheduledMakeUpTime1EndTime = '2034-11-08T18:30:00'
    
    Given path 'tracking/away-with-make-up-time-entries', newAwayWithMakeUpTimeEntryId
    And request
    """
    {
        "startTime": "#(startTime)",
        "endTime": "#(endTime)",
        "description": "#(description)",
        "makeUpTimeList": [
            {
                "startTime": "#(rescheduledMakeUpTime1StartTime)",
                "endTime": "#(rescheduledMakeUpTime1EndTime)"
            },
            {
                "startTime": "#(makeUpTime2StartTime)",
                "endTime": "#(makeUpTime2EndTime)"
            }
        ]
    }
    """
    When method POST
    Then status 200

    # Verify updated away with make up time entry data
    Given path 'tracking/entries'
    And params { startDate: "2034-11-05", endDate: "2034-11-08" }
    When method GET
    Then status 200
    And match response.awayWithMakeUpTimeEntries contains
    """
    {
        "id": "#(newAwayWithMakeUpTimeEntryId)",
        "type": 3,
        "startTime": "#(startTime)",
        "endTime": "#(endTime)",
        "description": "#(description)",
        "makeUpTimeList": [
            {
                "id": "#number",
                "startTime": "#(makeUpTime2StartTime)",
                "endTime": "#(makeUpTime2EndTime)"
            },
            {
                "id": "#number",
                "startTime": "#(rescheduledMakeUpTime1StartTime)",
                "endTime": "#(rescheduledMakeUpTime1EndTime)"
            },
        ]
    }
    """
    And match response.makeUpTimeEntries contains 
    """
    {
        "relatedEntryId": "#(newAwayWithMakeUpTimeEntryId)",
        "type": 4,
        "startTime": "#(rescheduledMakeUpTime1StartTime)",
        "endTime": "#(rescheduledMakeUpTime1EndTime)",
    }
    """
    And match response.makeUpTimeEntries contains
    """
    {
        "relatedEntryId": "#(newAwayWithMakeUpTimeEntryId)",
        "type": 4,
        "startTime": "#(makeUpTime2StartTime)",
        "endTime": "#(makeUpTime2EndTime)"
    }
    """

    # Cleanup: Delete the away with make up time entry (hard delete)
    Given path 'tracking/entries', newAwayWithMakeUpTimeEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that away with make up time was deleted
    Given path 'tracking/entries'
    And params { startDate: "2034-11-05", endDate: "2034-11-08" }
    When method GET
    Then status 200
    And assert response.awayWithMakeUpTimeEntries.filter(x => x.id == newAwayWithMakeUpTimeEntryId).length == 0
    And assert response.makeUpTimeEntries.filter(x => x.relatedEntryId == newAwayWithMakeUpTimeEntryId).length == 0


    # Use case:
    # I plan to be away tomorrow from 4 PM to 5 PM
    # I will make up in advance today
    # I made up as planned
    # But my away reason was cancelled, and thus I will not be away (I delete it)
    # And my task tracked yesterday remains untouched
  Scenario: I make up in advance, but in the end I didn't have to be away
    
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

    # Create a new away with make up time entry
    * def awayStartTime = '2034-12-05T16:00:00'
    * def awayEndTime = '2034-12-05T17:00:00'
    * def awayDescription = '[API-E2E]-Away description-' + Math.random()
    * def makeUpTimeStartTime = '2034-12-06T17:00:00'
    * def makeUpTimeEndTime = '2034-12-06T18:00:00'
    
    Given url apiRootUrl
    Given path 'tracking/away-with-make-up-time-entries'
    And request
    """
    {
        "startTime": "#(awayStartTime)",
        "endTime": "#(awayEndTime)",
        "description": "#(awayDescription)",
        "makeUpTimeList": [
            {
                "startTime": "#(makeUpTimeStartTime)",
                "endTime": "#(makeUpTimeEndTime)"
            },
        ]
    }
    """
    When method POST
    Then status 200

    * def newAwayWithMakeUpTimeEntryId = response.newAwayWithMakeUpTimeEntryId

    # Get employee's projects
    Given url apiRootUrl
    Given path 'tracking/task-entries/projects'
    And params { startDate: "2034-12-06", endDate: "2034-12-06" }
    When method GET
    Then status 200

    * def firstProjectId = response.projects[0].id

    # Create a new task entry for the same time as make up
    * def randomTitle = '[API-E2E]-Test-task-entry-' + Math.random()
    * def taskEntryStartTime = '2034-12-06T17:00:00'
    * def taskEntryEndTime = '2034-12-06T18:00:00'
    * def taskId = '#2233'
    * def taskEntryDescription = 'Task description'
    
    Given url apiRootUrl
    Given path 'tracking/task-entries'
    And request
    """
    {
        "title": "#(randomTitle)",
        "startTime": "#(taskEntryStartTime)",
        "endTime": "#(taskEntryEndTime)",
        "projectId": #(firstProjectId), 
        "taskId": "#(taskId)",
        "description": "#(taskEntryDescription)",
    }
    """
    When method POST
    Then status 200

    * def newTaskEntryId = response.newTaskEntryId
    
    # Soft delete, as if the user is deleting it
    Given path 'tracking/entries', newAwayWithMakeUpTimeEntryId, 'soft-delete'
    And request
    """
    {
        "deletionReason": "Deletion reason",
    }
    """
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that away with make up time was deleted but task entry was not deleted
    Given path 'tracking/entries'
    And params { startDate: "2034-12-05", endDate: "2034-12-06" }
    When method GET
    Then status 200
    And assert response.awayWithMakeUpTimeEntries.filter(x => x.id == newAwayWithMakeUpTimeEntryId).length == 0
    And assert response.makeUpTimeEntries.filter(x => x.relatedEntryId == newAwayWithMakeUpTimeEntryId).length == 0
    And match response.taskEntries contains deep
    """
    {
        "id": "#(newTaskEntryId)",
    }
    """

    # Cleanup: Delete the task entry (hard delete)
    Given path 'tracking/entries', newTaskEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup: Delete the away with make up time entry (hard delete)
    Given path 'tracking/entries', newAwayWithMakeUpTimeEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that task entry and away with make up time was deleted
    Given path 'tracking/entries'
    And params { startDate: "2034-12-06", endDate: "2034-12-06" }
    When method GET
    Then status 200
    And assert response.awayWithMakeUpTimeEntries.filter(x => x.id == newAwayWithMakeUpTimeEntryId).length == 0
    And assert response.makeUpTimeEntries.filter(x => x.relatedEntryId == newAwayWithMakeUpTimeEntryId).length == 0
    And assert response.taskEntries.filter(x => x.id == newTaskEntryId).length == 0