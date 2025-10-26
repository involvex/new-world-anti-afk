# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2025-10-22

### Changed
- The core anti-AFK logic has been completely refactored to use an external PowerShell script (`anti-afk.ps1`) instead of the internal `keybd_event`-based simulation. This improves reliability and separates the logic from the main application.
- The application now executes the `anti-afk.ps1` script with a `-TriggerOnce` parameter, which runs the anti-AFK sequence once and then exits.

### Added
- `CHANGELOG.md` to track project changes.
- The `anti-afk.ps1` script is now included in the project and copied to the output directory on build.

### Fixed
- Potential issues with input simulation being blocked by the game or other applications are now mitigated by using a PowerShell script.
