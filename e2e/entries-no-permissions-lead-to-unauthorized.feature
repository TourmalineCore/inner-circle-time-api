Feature: Entries
    # https://github.com/karatelabs/karate/issues/1191
    # https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

  Background:
    * header Content-Type = 'application/json'

  Scenario: No Permissions Lead to Unauthorized for Entries Endpoints

    * def jsUtils = read('./js-utils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    * def authSlytherineTenantGregoryLoginWithoutPermissions = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_GREGORY_GOYLE_LOGIN_WITHOUT_PERMISSIONS')
    * def authSlytherineTenantGregoryPasswordWithoutPermissions = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_GREGORY_GOYLE_PASSWORD_WITHOUT_PERMISSIONS')
    
    # Authentication
    Given url authApiRootUrl
    And path '/login'
    And request
    """
    {
        "login": "#(authSlytherineTenantGregoryLoginWithoutPermissions)",
        "password": "#(authSlytherineTenantGregoryPasswordWithoutPermissions)"
    }
    """
    And method POST
    Then status 200

    * def accessToken = karate.toMap(response.accessToken.value)

    * configure headers = jsUtils().getAuthHeaders(accessToken)


    Given url apiRootUrl
    Given path 'tracking/entries'
    When method GET
    Then status 403

    Given path 'tracking/entries', 100500, 'hard-delete'
    When method DELETE
    Then status 403

    Given path 'tracking/entries', 100500, 'soft-delete'
    When method DELETE
    Then status 403