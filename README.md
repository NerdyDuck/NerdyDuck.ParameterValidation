# NerdyDuck.ParameterValidation

This project provides a library with a set of constraint classes that can be used to validate arbitrary values, like application settings. The constraints can be expressed in strings, so they can be stored along with the validated values. It also contains classes to consistently serialize and deserialize various data types into strings that can then be stored in formats like XML or databases.

#### Platforms
- .NET Framework 4.6 or newer for desktop applications
- Universal Windows Platform (UWP) 10.0 (Windows 10) or newer for Windows Store Apps and [Windows 10 IoT](https://dev.windows.com/en-us/iot).

#### Dependencies
The project uses the following NuGet packages that are either found on NuGet.org or my own feed (see below):
- NerdyDuck.CodedExceptions

#### Languages
The neutral resource language for all texts is English (en-US). Currently, the only localization available is German (de-DE). If you like to add other languages, feel free to send a pull request with the translated resources!

#### How to get
- Use the NuGet package from my [MyGet](https://www.myget.org) feed: [https://www.myget.org/F/nerdyduck-release/api/v3/index.json](https://www.myget.org/F/nerdyduck-release/api/v3/index.json). If you need to debug the library, get the debug symbols from the same feed: [https://www.myget.org/F/nerdyduck-release/symbols/](https://www.myget.org/F/nerdyduck-release/symbols/).
- Download the binaries from the [Releases](../../releases/) page.
- You can clone the repository and compile the libraries yourself (see the [Wiki](../../wiki/) for requirements).

#### More information
For examples and a complete class reference, please see the [Wiki](../../wiki/). :exclamation: **Work in progress**.

#### Licence
The project is licensed under the [Apache License, Version 2.0](LICENSE).

#### History
#####2016-04-06 / v1.1.1 / DAK
- Updated reference for [NerdyDuck.CodedExceptions](../NerdyDuck.CodedExceptions) to v1.2.0.
- Added deployment project to compile all projects and create/push the NuGet package in one go. Removed separate NuGet project. Removes also dependency on NuGet Packager Template.
- Extracted file signing into its own reusable MSBuild target file.
- Extracted resource generation for desktop project into its own reusable MSBuild target file.
- Created a MSBuild target for automatic T4 transformations on build. Removes dependency on Visual Studio Modeling SDK.

#####2016-01-27 / v1.1.0 / DAK
- Changed argument type of [`ParameterConvert.ToDataType`](../../wiki/4b242047-5017-498c-2161-28f426df88dd), [`ParameterConvert.TryGetTypeConstraint`](../../wiki/8ab10564-458a-340d-bd21-07b27b4009b3), [`ParameterConvert.HasEncryptedConstraint`](../../wiki/cbc2ac67-5f06-636c-aab9-0c9ed58599d8) and [`ConstraintParser.ConcatConstraints`](../../wiki/58929f4c-736f-a231-e4eb-edf924756d8b) from `IList<Constraint> constraints` to `IReadOnlyList<Constraint> constraints`.
- Added [`ReadOnlyConstraint`](../../wiki/4b242047-5017-498c-2161-28f426df88dd) and [`DisplayHintConstraint`](../../wiki/4b242047-5017-498c-2161-28f426df88dd)
- Added [`EnumValuesConstraint.FromType`](../../wiki/4b242047-5017-498c-2161-28f426df88dd) and [`EnumValuesConstraint.GetEnumValuesDictionary`](../../wiki/4b242047-5017-498c-2161-28f426df88dd) method.
- Fixed a bug preventing serialization of [`ParameterDataType`](../../wiki/6575758d-3df2-4465-adfd-8fed94d84b74).
- Completed XML comments in code.
- Added example projects.

#####2016-01-11 / v1.0.1 / DAK
- Changed target version for UWP library to Windows 10 build 10586; minimum version remains build 10240.
- Changed automatic signing of assemblies from post-compiler batch script to msbuild task.
- Compiled against [NerdyDuck.CodedExceptions](../NerdyDuck.CodedExceptions) v1.1.2 .

#####2015-12-09 / v1.0.0 / DAK
- Initial release.
