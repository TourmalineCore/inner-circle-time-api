Feature: Task Entries
    # https://github.com/karatelabs/karate/issues/1191
    # https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

  Background:
    * header Content-Type = 'application/json'

  Scenario: Task Entries Creation Tenants Isolation

    * def jsUtils = read('./js-utils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    * def authSlytherineTenantDracoLoginWithAllPermissions = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_DRACO_MALFOY_LOGIN_WITH_ALL_PERMISSIONS')
    * def authSlytherineTenantDracoPasswordWithAllPermissions = jsUtils().getEnvVariable('AUTH_SLYTHERINE_TENANT_DRACO_MALFOY_PASSWORD_WITH_ALL_PERMISSIONS')
    * def authRavenclawTenantChoLoginWithAllPermissions = jsUtils().getEnvVariable('AUTH_RAVENCLAW_TENANT_CHO_CHANG_LOGIN_WITH_ALL_PERMISSIONS') 
    * def authRavenclawTenantChoPasswordWithAllPermissions = jsUtils().getEnvVariable('AUTH_RAVENCLAW_TENANT_CHO_CHANG_PASSWORD_WITH_ALL_PERMISSIONS')
    
    # Authentication with slytherine tenant
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

    * def slytherineTenantAccessToken = karate.toMap(response.accessToken.value)

    * configure headers = jsUtils().getAuthHeaders(slytherineTenantAccessToken)

    # Get employee's projects in slytherin tenant
    # Here we specified 2031 year to avoid conflicts with other tests
    Given url apiRootUrl
    Given path 'tracking/task-entries/projects'
    And params { startDate: "2031-11-06", endDate: "2031-11-06" }
    When method GET
    Then status 200

    * def slytherineTenantProjectId = response.projects[0].id

    # Create a new task entry in slytherin tenant
    # Here we specified 2031 year to avoid conflicts with other tests
    * def randomTitle = '[API-E2E]-Test-task-entry-' + Math.random()
    * def startTime = '2031-11-06T14:00:00'
    * def endTime = '2031-11-06T16:00:00'
    * def taskId = '#2233'
    * def description = 'Task description'
    
    Given url apiRootUrl
    Given path 'tracking/task-entries'
    And request
    """
    {
        "title": "#(randomTitle)",
        "startTime": "#(startTime)",
        "endTime": "#(endTime)",
        "projectId": #(slytherineTenantProjectId), 
        "taskId": "#(taskId)",
        "description": "#(description)",
    }
    """
    When method POST
    Then status 200

    * def slytherineTenantNewTaskEntryId = response.newTaskEntryId

    # Authentication with ravenclaw tenant
    Given url authApiRootUrl
    And path '/login'
    And request
    """
    {
        "login": "#(authRavenclawTenantChoLoginWithAllPermissions)",
        "password": "#(authRavenclawTenantChoPasswordWithAllPermissions)"
    }
    """
    And method POST
    Then status 200

    * def ravenclawTenantAccessToken = karate.toMap(response.accessToken.value)

    * configure headers = jsUtils().getAuthHeaders(ravenclawTenantAccessToken)

    # Get employee's projects in ravenclaw tenant
    # Here we specified 2031 year to avoid conflicts with other tests
    Given url apiRootUrl
    Given path 'tracking/task-entries/projects'
    And params { startDate: "2031-11-06", endDate: "2031-11-06" }
    When method GET
    Then status 200

    * def ravenclawTenantProjectId = response.projects[0].id

    # Create a new task entry in ravenclaw tenant the same time as slytherin tenant
    Given url apiRootUrl
    Given path 'tracking/task-entries'
    And request
    """
    {
        "title": "#(randomTitle)",
        "startTime": "#(startTime)",
        "endTime": "#(endTime)",
        "projectId": #(ravenclawTenantProjectId), 
        "taskId": "#(taskId)",
        "description": "#(description)",
    }
    """
    When method POST
    Then status 200

    * def ravenclawTenantNewTaskEntryId = response.newTaskEntryId

    # Cleanup: Delete task entry in ravenclaw tenant
    Given path 'tracking/entries', ravenclawTenantNewTaskEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that task entry in ravenclaw tenant was deleted
    Given path 'tracking/entries'
    And params { startDate: "2031-11-06", endDate: "2031-11-06" }
    When method GET
    Then status 200
    And assert response.taskEntries.filter(x => x.id == ravenclawTenantNewTaskEntryId).length == 0

    * configure headers = jsUtils().getAuthHeaders(slytherineTenantAccessToken)

    # Cleanup: Delete task entry in slytherine tenant
    Given path 'tracking/entries', slytherineTenantNewTaskEntryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup Verification: Verify that task entry in slytherin tenant was deleted
    Given path 'tracking/entries'
    And params { startDate: "2031-11-06", endDate: "2031-11-06" }
    When method GET
    Then status 200
    And assert response.taskEntries.filter(x => x.id == slytherineTenantNewTaskEntryId).length == 0


