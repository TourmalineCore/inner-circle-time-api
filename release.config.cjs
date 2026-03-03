module.exports = {
  // You can find out more about the configuration of this file here https://semantic-release.gitbook.io/semantic-release/usage/configuration
  branches: [
    'master'
  ],
  // Plugins https://semantic-release.gitbook.io/semantic-release/extending/plugins-list
  plugins: [
    [
      '@semantic-release/exec',
      {
        // Need to rewrite version in __version
        prepareCmd: 'echo ${nextRelease.version} > __version'
      }
    ],
    [
      // Analyzes commits and determines which release version should be released.
      '@semantic-release/commit-analyzer',
      {
        // decided to switch to angular to conventionalcommits preset for major bumps with short hand syntax like fix!
        // they also consider to make that switch but not sure regarding the timeline https://github.com/semantic-release/semantic-release/issues/3406
        // here you can find more about the issue with ! for angular preset
        // https://github.com/semantic-release/commit-analyzer/issues/231#issuecomment-2127394159
        preset: "conventionalcommits",
        releaseRules: "./release.rules.cjs"
      }
    ],
    // Add release notes
    [
      '@semantic-release/release-notes-generator',
      {
        writerOpts: {
          commitsSort: ['scope', 'subject']
        }
      }
    ],
    // Create GitHub Release
    '@semantic-release/github',
    [
      // Plugin for commits changes
      '@semantic-release/git',
      {
        // Tracks the __version file in the project root
        // And updates this file with a new version with each release
        // Also add this file to commit
        assets: ['__version'],
        // Release commit message
        message: 'chore(release): ${nextRelease.version} [deploy to prod]'
      }
    ]
  ],
  tagFormat: '${version}'
}