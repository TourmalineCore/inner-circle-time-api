Feature: Metrics
    # https://github.com/karatelabs/karate/issues/1191
    # https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

  Background:
    * header Content-Type = 'application/json'

    # Use case:
    # I didn't feel well from 8 a.m. to 8:20 p.m. on Monday.
    # I keep track of this time in the time tracker.
    # Then I want the total tracked time to be 0.3(3) hours.
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
    * def unwellStartTime = '2028-09-04T08:00:00'
    * def unwellEndTime = '2028-09-04T08:20:00'
    
    Given url apiRootUrl
    Given path 'tracking/unwell-entries'
    And request
    """
    {
        "startTime": "#(unwellStartTime)",
        "endTime": "#(unwellEndTime)"
    }
    """
    When method POST
    Then status 200

    * def newUnwellEntryId = response.newUnwellEntryId

    # Get metrics
    Given path 'reporting/metrics'
    And params { startDate: "2028-09-04", endDate: "2028-09-10" }
    When method GET
    Then status 200
    # We expect 20 minutes = 0.3(3) hours.
    # The backend may round it differently because the fraction is infinite.
    # Karate does not support decimal and uses double, which has only ~15-17 significant digits
    # So we check that the value is between 0.333333333333333 and 0.333333333333334.
    * print response.trackedHours
    And match response.trackedHours == '#? _ > 0.333333333333333 && _ < 0.333333333333334'
    
    # Cleanup: Delete the unwell entry on Monday (hard delete)
    Given path 'tracking/entries', newUnwellEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }
    
    # Cleanup Verification: Verify that all entries was deleted
    Given path 'tracking/entries'
    And params { startDate: "2028-09-04", endDate: "2028-09-04" }
    When method GET
    Then status 200
    And assert response.unwellEntries.filter(x => x.id == newUnwellEntryId).length == 0