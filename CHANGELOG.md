# NewPlatform.Flexberry.Caching Changelog
All notable changes to this project will be documented in this file.
This project adheres to [Semantic Versioning](http://semver.org/).

## [Unreleased]

### Added

### Fixed

### Changed

## [2.0.1] - 2023-10-11

### Changed

* The `TryGetFromCache` method no longer throws an internal exception if the key is not found.

## [2.0.0] - 2021-04-06

### Added

* `.NET Standard 2.0` implementation.
* `ICacheService.Trim(int)` to remove a specified percentage of cache entries (also to force flushing expired items).

### Fixed

### Changed

* csproj format to `Microsoft.NET.Sdk`.

