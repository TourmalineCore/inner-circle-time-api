function fn() {
  return {
    getAuthHeaders: function (tokenValue) {
      return {
        [this.getAuthHeaderKey()]: this.getAuthHeaderValue(tokenValue)
      }
    },

    getAuthHeaderKey: function () {
      return this.shouldUseFakeExternalDependencies()
        ? 'X-DEBUG-TOKEN'
        : 'Authorization';
    },

    getAuthHeaderValue: function (tokenValue) {
      return this.shouldUseFakeExternalDependencies()
        ? tokenValue
        : 'Bearer ' + tokenValue;
    },

    shouldUseFakeExternalDependencies: function () {
      return this.getEnvVariable('SHOULD_USE_FAKE_EXTERNAL_DEPENDENCIES') === 'true';
    },

    getEnvVariable: function (variable) {
      var System = Java.type('java.lang.System');

      return System.getenv(variable);
    },

    getEmployeeIdFromToken: function (tokenValue) {
      var decodedString;

      if (tokenValue.includes('.')) {
        var payload = tokenValue.split('.')[1];

        decodedString = decodeString(payload);
      } else {
        decodedString = decodeString(tokenValue);
      }

      var tokenData = JSON.parse(decodedString);

      return Number(tokenData.employeeId);
    },
  }

  function decodeString(string) {
    var Bytes = Java.type('java.util.Base64');
    var decodedBytes = Bytes.getDecoder().decode(string);

    return new java.lang.String(decodedBytes);
  }
}