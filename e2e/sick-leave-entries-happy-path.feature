Feature: Sick Leave Entries
    # https://github.com/karatelabs/karate/issues/1191
    # https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

    # Use case:  
    # I went to the doctor and was given sick leave from Monday to Friday.
    # I want to track my sick leave in the time tracker.
    # Then, after visiting the doctor, they extended my sick leave to next Monday
    # And I want to update my sick leave in the time tracker
  Scenario: Track and update sick leave in the time tracker

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

    # Create a new sick leave entry
    * def sickLeaveStartDate = '2035-06-04'
    * def sickLeaveEndDate = '2035-06-08'
    
    Given url apiRootUrl
    Given path 'tracking/sick-leave-entries'
    And request
    """
    {
        "period": {
            "startDate": "#(sickLeaveStartDate)",
            "endDate": "#(sickLeaveEndDate)"
        }
    }
    """
    When method POST
    Then status 200

    * def newSickLeaveEntryId = response.newSickLeaveEntryId 

    # Update a sick leave entry
    * def newSickLeaveEndDate = '2035-06-11'
    
    Given path 'tracking/sick-leave-entries', newSickLeaveEntryId
    And request
    """
    {
        "period": {
            "startDate": "#(sickLeaveStartDate)",
            "endDate": "#(newSickLeaveEndDate)"
        }
    }
    """
    When method POST
    Then status 200

    # Verify updated sick leave entry using endpoint with id
    Given path 'tracking/sick-leave-entries', newSickLeaveEntryId
    When method GET
    Then status 200
    And match response contains
    """
    {
        "id": "#(newSickLeaveEntryId)",
        "entryType": 5,
        "period": {
            "startDate": "#(sickLeaveStartDate)",
            "endDate": "#(newSickLeaveEndDate)"
        }
    }
    """

    # Verify updated sick leave entry data using endpoint with period
    Given path 'tracking/entries'
    And params { startDate: "2035-06-04", endDate: "2035-06-08" }
    When method GET
    Then status 200
    And match response.sickLeaveEntries contains
    """
    {
        "id": "#(newSickLeaveEntryId)",
        "entryType": 5,
        "startDate": "#(sickLeaveStartDate)",
        "endDate": "#(newSickLeaveEndDate)",
    }
    """

    # Cleanup: Delete the sick leave entry (hard delete)
    Given path 'tracking/entries', newSickLeaveEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that sick leave entry was deleted
    Given path 'tracking/entries'
    And params { startDate: "2035-06-04", endDate: "2035-06-11" }
    When method GET
    Then status 200
    And assert response.sickLeaveEntries.filter(x => x.id == newSickLeaveEntryId).length == 0