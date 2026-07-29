Feature: Vacation Entries
    # https://github.com/karatelabs/karate/issues/1191
    # https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

    # Use case:  
    # I have planned a 14 days paid vacation.
    # I track my vacation in the time tracker.
    # Then, I realized that I need shift it for a month later.
  Scenario: Track and update vacation in the time tracker

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

    # Create a new vacation entry
    * def vacationLeaveStartDate = '2036-06-02'
    * def vacationLeaveEndDate = '2036-06-15'
    * def isUnpaid = false
    
    Given url apiRootUrl
    Given path 'tracking/vacation-entries'
    And request
    """
    {
        "period": {
            "startDate": "#(vacationLeaveStartDate)",
            "endDate": "#(vacationLeaveEndDate)"
        },
        "isUnpaid": "#(isUnpaid)"
    }
    """
    When method POST
    Then status 200

    * def newVacationEntryId = response.newVacationEntryId 

    # Update a vacation entry
    * def rescheduledVacationStartDate = '2036-07-07'
    * def rescheduledVacationEndDate = '2036-07-20'
    
    Given path 'tracking/vacation-entries', newVacationEntryId
    And request
    """
    {
        "period": {
            "startDate": "#(rescheduledVacationStartDate)",
            "endDate": "#(rescheduledVacationEndDate)"
        },
        "isUnpaid": "#(isUnpaid)"
    }
    """
    When method POST
    Then status 200

    # Verify updated a vacation entry using endpoint with id
    Given path 'tracking/vacation-entries', newVacationEntryId
    When method GET
    Then status 200
    And match response contains
    """
    {
        "id": "#(newVacationEntryId)",
        "entryType": 6,
        "period": {
            "startDate": "#(rescheduledVacationStartDate)",
            "endDate": "#(rescheduledVacationEndDate)"
        },
        "isUnpaid": "#(isUnpaid)"
    }
    """

    # Verify updated a vacation entry data using endpoint with period
    Given path 'tracking/entries'
    And params { startDate: "2036-07-07", endDate: "2036-07-13" }
    When method GET
    Then status 200
    And match response.vacationEntries contains
    """
    {
        "id": "#(newVacationEntryId)",
        "entryType": 6,
        "period": {
            "startDate": "#(rescheduledVacationStartDate)",
            "endDate": "#(rescheduledVacationEndDate)"
        },
        "isUnpaid": "#(isUnpaid)"
    }
    """

    # Cleanup: Delete the vacation entry (hard delete)
    Given path 'tracking/entries', newVacationEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that vacation entry was deleted
    Given path 'tracking/entries'
    And params { startDate: "2036-07-07", endDate: "2036-07-20" }
    When method GET
    Then status 200
    And assert response.vacationEntries.filter(x => x.id == newVacationEntryId).length == 0