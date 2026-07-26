Feature: To Dos
  Background:
    * header Content-Type = 'application/json'

    * def jsUtils = read('./js-utils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    * def authSlytherineTenantDracoLoginWithAllPermissions = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_DRACO_MALFOY_LOGIN_WITH_ALL_PERMISSIONS')
    * def authSlytherineTenantDracoPasswordWithAllPermissions = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_DRACO_MALFOY_PASSWORD_WITH_ALL_PERMISSIONS')
    
  Scenario: Happy Path
    # Step 0: Authentication
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

    # Step 1: Create a new To Do
    * def randomToDoName = '[API-E2E]-Test-to-do-' + Math.random()
    
    Given url apiRootUrl
    Given path 'to-dos'
    And request
    """
    {
      "name": "#(randomToDoName)"
    }
    """
    When method POST
    Then status 200

    * def newToDoId = response.newToDoId

    # Step 2: Verify that the new ToDo is in the list with the received id and generated name
    Given url apiRootUrl
    Given path 'to-dos'
    When method GET
    And match response.toDos contains
    """
    {
      "id": "#(newToDoId)",
      "name": "#(randomToDoName)",
    }
    """

    # Cleanup: Delete the To Do (hard delete)
    Given path 'to-dos'
    And params { toDoId: "#(newToDoId)" }
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }
