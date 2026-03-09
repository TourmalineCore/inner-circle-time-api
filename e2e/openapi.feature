Feature: OpenApi

  Background:
    * header Content-Type = 'application/json'

  Scenario: Fetch Json API Definitions

    * def jsUtils = read('./js-utils.js')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    # we need to trim because __version files contains one extra empty line
    * def apiVersion = karate.readAsString('../__version').trim()
    
    # Check that response contains correct api name and version from __version file
    Given url apiRootUrl
    And path 'swagger/openapi.json'
    And method GET
    Then status 200
    And match response.info == 
    """
    {
        "title": "inner-circle-items-api",
        "version": "#(apiVersion)"
    }
    """
