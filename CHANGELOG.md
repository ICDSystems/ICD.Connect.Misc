# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

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
