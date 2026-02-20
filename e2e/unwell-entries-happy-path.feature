Feature: Unwell Entries
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

    # Create a new unwell entry
    * def startTime = '2029-11-05T14:00:00'
    * def endTime = '2029-11-05T16:00:00'
    
    Given url apiRootUrl
    Given path 'tracking/unwell-entries'
    And request
    """
    {
        "startTime": "#(startTime)",
        "endTime": "#(endTime)",
    }
    """
    When method POST
    Then status 200

    * def newUnwellEntryId = response.newUnwellEntryId

    # Update unwell entry
    * def newStartTime = '2029-11-06T11:00:00'
    * def newEndTime = '2029-11-06T12:00:00'
    
    Given path 'tracking/unwell-entries', newUnwellEntryId
    And request
    """
    {
        "startTime": "#(newStartTime)",
        "endTime": "#(newEndTime)",
    }
    """
    When method POST
    Then status 200

    # Verify updated unwell entry data
    Given path 'tracking/work-entries'
    And params { startDate: "2029-11-06", endDate: "2029-11-06" }
    When method GET
    And match response.unwellEntries contains
    """
    {
        "id": "#(newUnwellEntryId)",
        "type": 2,
        "startTime": "#(newStartTime)",
        "endTime": "#(newEndTime)",
    }
    """

    # Cleanup: Delete the unwell entry (hard delete)
    Given path 'tracking/work-entries', newUnwellEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that unwell entry was deleted
    Given path 'tracking/work-entries'
    And params { startDate: "2029-11-06", endDate: "2029-11-06" }
    When method GET
    Then status 200
    And assert response.unwellEntries.filter(x => x.id == newUnwellEntryId).length == 0