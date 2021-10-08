# ![Logo](media/NerdyDuck.ParameterValidation.svg) NerdyDuck.ParameterValidation

This project provides a library with a set of constraint classes that can be used to validate arbitrary values, like application settings. The constraints can be expressed in strings, so they can be stored along with the validated values. It also contains classes to consistently serialize and deserialize various data types into strings that can then be stored in formats like XML or databases.

#### Platforms
- .NET Standard 2.0 (netstandard2.0), to support .NET Framework (4.6.1 and up), .NET Core (2.0 and up), Mono (5.4 and up), and the Xamarin and UWP platforms.
- .NET 5 (net5.0)
- .NET 6 (net6.0)

#### Dependencies
The project uses the following NuGet packages not issued by Microsoft as part of the BCL:
- [NerdyDuck.CodedExceptions](https://www.nuget.org/packages/NerdyDuck.CodedExceptions)

#### Languages
The neutral resource language for all texts is English (en-US). Currently, the only localization available is German (de-DE). If you like to add other languages, feel free to send a pull request with the translated resources!

#### How to get
- Use the NuGet package (include debug symbol files and supports [SourceLink](https://github.com/dotnet/sourcelink): https://www.nuget.org/packages/NerdyDuck.ParameterValidation
- Download the binaries from the [Releases](../../releases/) page.

#### More information
For examples and a complete class reference, please see the [Wiki](../../wiki/). :exclamation: **Work in progress**.

#### License
The project is licensed under the [MIT License](LICENSE).

#### History
##### TBD / 2.0.0-rc.1 / DAK
- Upgraded platform to .NET Standard 2.0, .NET 5 and .NET 6
- Removed separate binaries for UWP (use .NET Standard 2.0 instead)
- Changed German resources from de-DE to just de.
- Restructured repository, using Directory.Build.props/.targets for common configuration
- Switched license from Apache 2.0 to MIT
- Complete unit tests added
- Some bug fixes

#####2016-04-14 / v1.1.2 / DAK
- Updated reference for [NerdyDuck.CodedExceptions](../NerdyDuck.CodedExceptions) to v1.2.1.
- Switched exception error codes from literals to `ErrorCodes` enumeration, including comment text from ErrorCodes.csv.
- Universal project compiled against Microsoft.NETCore.UniversalWindowsPlatform 5.1.0.

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
