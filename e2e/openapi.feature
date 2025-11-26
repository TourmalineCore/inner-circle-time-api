Feature: OpenApi

  Background:
    * header Content-Type = 'application/json'

  Scenario: Fetch Json API Definitions

    * def jsUtils = read('./js-utils.js')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    
    # Check that response contains non-empty JSON with an expected OpenApi version
    Given url apiRootUrl
    And path 'swagger/openapi/v1.json'
    And method GET
    Then status 200
    And assert response.openapi == '3.0.1'
