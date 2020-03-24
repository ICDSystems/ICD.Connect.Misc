# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [8.2.1] - 2020-03-24
### Changed
 - Fixed an issue where Cresnet Occupancy Sensors weren't using an Occupancy Sensor Control

## [8.2.0] - 2020-03-20
### Added
 - Created Unsplash Service Device.
 - Added Vibe Board device
 - Added YKUP USB switcher device

### Changed
 - Fixed web requests to use new web port response.
 - Crestron ComPorts are instantiated with more appropriate default comspec values

## [8.1.0] - 2020-02-27
### Added
 - Added console command to GlsPartCn for setting sensitivity

### Changed
 - GlsPartCn sensitivity is now optional

## [8.0.1] - 2019-11-19
### Changed
 - Improvements to IO port debugging

## [8.0.0] - 2019-10-07
### Changed
 - Fixed issue with CEN-IO ports not being avaliable due to inheritance issues

### Removed
 - Moved TV Presets features to Sources project

## [7.4.0] - 2019-09-16
### Added
 - Added GenericBaseUtils class for setup and teardown of Crestron devices

### Changed
 - Crestron device description contains originator ID and combine name
 - Using CresnetSettingsUtils to standardize cresnet settings serialization

## [7.3.0] - 2019-08-26
### Added
 - GlsPartCnAdapter has SupportsFeedback property 

### Changed
 - HTTP/S port no longer accepts a string body

## [7.2.1] - 2019-06-27
### Changed
 - Fixed bug that was preventing IR command between-time from working correctly

## [7.2.0] - 2019-03-26
### Added
 - Added Crestron Ethernet IO devices CEN-IO-COM-102, CEN-IO-DI-104, CEN-IO-IR-104, CEN-IO-RY-104

## [7.1.0] - 2019-01-29
### Added
 - Cec Port support for Crestron devices
 
### Changed
 - Fixed KeyNotFound exceptions when converting Crestron ComSpec default values

## [7.0.0] - 2019-01-10
### Added
 - Added port configuration features to devices and ports

## [6.5.2] - 2020-03-05
### Changed
 - Fixed a bug where CENCN adapters don't upated online/offline correctly

## [6.5.1] - 2019-09-19
### Changed
 - Fixed a bug where Crestron ControlSystem IR Ports were not being registered
 - Crestron port online status no longer depends on registration state

## [6.5.0] - 2019-08-26
### Changed
 - Fixed a mistake that was preventing Occupancy Sensors from being instantiated
 - Crestron port online state is driven by the online state of the parent device

## [6.4.0] - 2019-07-25
### Changed
 - Split CsaPws10sHubEnetAdapter into two master - slave adapters and changed interface implementations to match

## [6.3.0] - 2019-07-11
### Added
 - IC2nCbdPBaseWithVersiportAdapter is an IPortParent
 - AbstractC2nCbdPBaseWithVersiportAdapter exposes VersiPorts

## [6.2.4] - 2019-07-11
### Changed
 - Fixed a bug where C2N-CBD-P settings were not properly constrained to a Cresnet Bridge parent device

## [6.2.3] - 2019-06-27
### Changed
 - No longer logging an error when CEN card frames report NoAttempt for registration

## [6.2.2] - 2019-05-24
### Changed
 - Treating crestron sigs that are actually NULL as null sigs

## [6.2.1] - 2019-05-22
### Changed
 - Better handling of Crestron C3 CardIds when using single card cage vs three card cage

## [6.2.0] - 2019-05-16
### Added
 - Added occupancy telemetry

### Changed
 - Combined extensions directories
 - Checking for nullsigs before getting sig values

## [6.1.1] - 2019-05-24
### Added
 - Adding Card Parent and Card Address attributes to C3 card settings.

## [6.1.0] - 2019-04-19
### Added
 - Cresnet Occupancy Sensor Adapters - GLS-ODT-C-CN, GLS-OIR-C-CN, and GLS-OIRLCL-C-CN

## [6.0.2] - 2019-01-16
### Changed
 - Less obnoxious RelayPortAdapter error when failing to get the internal port

## [6.0.1] - 2019-01-10
### Changed
 - Small utils compatibility changes

## [6.0.0] - 2018-10-30
### Added
 - Added ControlPortParentSettingsProperty to port settings parent properties
 - Added Global Cache IP2SL device
 - Added RaspberryPi project for controlling GPIO pins as Krang IO ports

### Changed
 - Fixed registration issue with Crestron ports on some secondary devices
 - Fixed issue where versiports setup as digital outputs don't handle rapid changes of state properly
 - iTachFlex wraps a configurable TCP port.
 - Fail gracefully when a referenced port/device is not present in the configuration

## [5.6.0] - 2018-10-18
### Added
 - Debug logging for IOPort
 
### Changed
 - More appropriate exception types
 - Small logging improvements

## [5.5.0] - 2018-09-25
### Added
 - Additional constructors for sig collections

## [5.4.0] - 2018-09-14
### Added
 - Shim for getting crestron button states
 - Added SigExtensions

### Changed
 - Small performance improvements

## [5.3.0] - 2018-07-19
### Added
 - Added catch for NotSupportedException for all DmOutputExtensions methods
 - Added basic support for DMPS3 4K Series audio switching

## [5.2.0] - 2018-07-02
### Added
 - Added missing GetIoPort method to C3Io16Adapter

### Changed
 - Control cards now use uint CardId instead of byte IPID
 - Fixed bug where IR Port would stop sending commands after an unavailable command
 - Fixed bugs in control card/frame instantiation

## [5.1.3] - 2018-05-24
### Changed
 - Default tv presets path to processor address

## [5.1.2] - 2018-05-09
### Changed
 - Fixing issues with loading configs in UTF8-BOM encoding

## [5.1.1] - 2018-05-03
### Added
 - Adding IP attribute to settings

## [5.1.0] - 2018-04-27
### Added
 - Adding originator id attribute to cresnet parent settings

## [5.0.0] - 2018-04-23
### Changed
 - IOccupancySensorControl now uses an enum for occupancy state, supporting unknown, occupied, unoccupied
 - Updated AbstractOccupancySensor to implement interface changes
 - Updated MockOccupancySensor to implement interface changes
 - Removed suffix from assembly name
 - Using API event args
