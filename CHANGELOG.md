# PiRhoUtilities Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
### Removed
### Changed

## [1.2.0] - 2019-12-?
### Added
- Added the ability to specify a method, property, or field as the values and options for PopupAttribute
- Added Test Assemblies
- Added TabsAttribute
### Changed
- Fixed situations for multiple attributes where callbacks on private properties would throw a null reference
- Fixed Placeholders with delayed TextFields
- Fixed ComboBox styling
- Fixed Foldout icon on high DPI monitors
- Fixed Samples.meta warning when installing package via git

## [1.1.1] - 2019-10-24
### Added
- Added an generic extension method for SetLabel on BaseField
### Changed
- Make it so SerializedDictionary lists stay in memory during runtime so that they can be accessed by serialized properties
- Changed so the ConfigureProperty call automatically gets the tooltip instead of having to pass it in

## [1.1.0] - 2019-10-23
### Added
- Added a NoLabel attribute which removes the label from a BaseField<> or PropertyField
- Added SerializedProperty constructors for each of the controls so that they can be created and automatically be bound to properties
- Added DragAndDrop functionality for Pickers and the History window
- Added an extension method for SerializedProperty to retreive the tooltip attribute defined it's field
- Added the ability to declary maximums and minimums for various attributes as defined in fields, methods, or properties
### Changed
- Custom label now affects all class names defined in the drawer. This includes Frame's label so that Lists and Dictionaries are now affected

## [1.0.5] - 2019-10-22
### Added
- Added a public getter for the Control on all BaseField<> classes

## [1.0.4] - 2019-10-21
### Changed
- Made Rollout button only clickable from the Icon due to UIElements triggering it when clicking on the Add Key text

## [1.0.3] - 2019-10-14
### Added
- Added support for SerializedDataList

## [1.0.2] - 2019-10-08
### Changed
- Made setting the text of a MessageBox not trigger a change event (Label does this internally for bindings it seems)

## [1.0.1] - 2019-10-08
### Changed
- Fixed Configuration path

## [1.0.0] - 2019-10-07
### Added
- First official release
