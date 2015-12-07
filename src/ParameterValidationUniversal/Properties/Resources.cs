#region Copyright
/*******************************************************************************
 * <copyright file="Resources.cs" owner="Daniel Kopp">
 * Copyright 2015 Daniel Kopp
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * </copyright>
 * <author name="Daniel Kopp" email="dak@nerdyduck.de" />
 * <assembly name="NerdyDuck.ParameterValidation">
 * Validation and serialization of parameter values for .NET
 * </assembly>
 * <file name="Resources.cs" date="2015-12-04">
 * Helper class to access localized string resources.
 * </file>
 ******************************************************************************/
#endregion

using System;

namespace NerdyDuck.ParameterValidation.Properties
{
	/// <summary>
	/// Helper class to access localized string resources.
	/// </summary>
	internal static class Resources
	{
		#region String resource properties
		/// <summary>
		/// Gets a localized string similar to "At least one of the specified scheme strings is null or empty or contains only white-space.".
		/// </summary>
		internal static string AllowedSchemeConstraint_CheckSchemes_InvalidScheme
		{
			get { return GetResource("AllowedSchemeConstraint_CheckSchemes_InvalidScheme"); }
		}

		/// <summary>
		/// Gets a localized string similar to "The list of allowed schemes is empty.".
		/// </summary>
		internal static string AllowedSchemeConstraint_CheckSchemes_NoScheme
		{
			get { return GetResource("AllowedSchemeConstraint_CheckSchemes_NoScheme"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0}: URI scheme '{1}' is not supported. Allowed schemes: {2}.".
		/// </summary>
		internal static string AllowedSchemeConstraint_Validate_Failed
		{
			get { return GetResource("AllowedSchemeConstraint_Validate_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "No allowed schemes set.".
		/// </summary>
		internal static string AllowedSchemeConstraint_Validate_NotConfigured
		{
			get { return GetResource("AllowedSchemeConstraint_Validate_NotConfigured"); }
		}

		/// <summary>
		/// Gets a localized string similar to "ASCII".
		/// </summary>
		internal static string CharacterSetConstraint_CharSet_Ascii
		{
			get { return GetResource("CharacterSetConstraint_CharSet_Ascii"); }
		}

		/// <summary>
		/// Gets a localized string similar to "ISO-646-IRV Odette Subset".
		/// </summary>
		internal static string CharacterSetConstraint_CharSet_Iso646Odette
		{
			get { return GetResource("CharacterSetConstraint_CharSet_Iso646Odette"); }
		}

		/// <summary>
		/// Gets a localized string similar to "ISO 8859-1".
		/// </summary>
		internal static string CharacterSetConstraint_CharSet_Iso8859
		{
			get { return GetResource("CharacterSetConstraint_CharSet_Iso8859"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Windows-1252".
		/// </summary>
		internal static string CharacterSetConstraint_CharSet_Windows1252
		{
			get { return GetResource("CharacterSetConstraint_CharSet_Windows1252"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Parameter value '{0}' is not a valid supported character set.".
		/// </summary>
		internal static string CharacterSetConstraint_SetParameters_InvalidValue
		{
			get { return GetResource("CharacterSetConstraint_SetParameters_InvalidValue"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} contains at least one character not defined in the {1} character set.".
		/// </summary>
		internal static string CharacterSetConstraint_Validate_Failed
		{
			get { return GetResource("CharacterSetConstraint_Validate_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Cannot configure constraint.".
		/// </summary>
		internal static string ConstraintConfigurationException_Message
		{
			get { return GetResource("ConstraintConfigurationException_Message"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} has one or more flags that are not defined or not valid for this parameter.".
		/// </summary>
		internal static string EnumConstraint_Validate_InvalidFlag
		{
			get { return GetResource("EnumConstraint_Validate_InvalidFlag"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0}: value '{1}' is not defined in enumeration, or not valid for this parameter.".
		/// </summary>
		internal static string EnumConstraint_Validate_NotDefined
		{
			get { return GetResource("EnumConstraint_Validate_NotDefined"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0}: value type '{1}' is not supported by enumeration.".
		/// </summary>
		internal static string EnumConstraint_Validate_NotSupported
		{
			get { return GetResource("EnumConstraint_Validate_NotSupported"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Enumeration type of {0} ('{1}') does not match expected enumeration type ('{2}').".
		/// </summary>
		internal static string EnumTypeConstraint_Validate_WrongEnum
		{
			get { return GetResource("EnumTypeConstraint_Validate_WrongEnum"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Type '{0}' does not match underlying data type {1} of the enumeration and cannot be converted into this type.".
		/// </summary>
		internal static string EnumValuesConstraint_InvalidValueType
		{
			get { return GetResource("EnumValuesConstraint_InvalidValueType"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Data type is not one of the integer types.".
		/// </summary>
		internal static string EnumValuesConstraint_NotInteger
		{
			get { return GetResource("EnumValuesConstraint_NotInteger"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Value '{0}' is not a valid hexadecimal value.".
		/// </summary>
		internal static string EnumValuesConstraint_SetParameters_InvalidHex
		{
			get { return GetResource("EnumValuesConstraint_SetParameters_InvalidHex"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Parameter '{0}' is not a valid key/value pair of format 'Name=Value'.".
		/// </summary>
		internal static string EnumValuesConstraint_SetParameters_InvalidValue
		{
			get { return GetResource("EnumValuesConstraint_SetParameters_InvalidValue"); }
		}

		/// <summary>
		/// Gets a localized string similar to "First parameter is not a valid value of the ParameterDataType enumeration.".
		/// </summary>
		internal static string EnumValuesConstraint_SetParameters_NotDataType
		{
			get { return GetResource("EnumValuesConstraint_SetParameters_NotDataType"); }
		}

		/// <summary>
		/// Gets a localized string similar to "No enumeration values specified for the {0} constraint.".
		/// </summary>
		internal static string EnumValuesConstraint_SetParameters_NoValues
		{
			get { return GetResource("EnumValuesConstraint_SetParameters_NoValues"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Constraint is not sufficiently configured.".
		/// </summary>
		internal static string EnumValuesConstraint_Validate_NotConfigured
		{
			get { return GetResource("EnumValuesConstraint_Validate_NotConfigured"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} contains at least one character not allowed in a file name. Actual value: '{1}'".
		/// </summary>
		internal static string FileNameConstraint_Validate_Failed
		{
			get { return GetResource("FileNameConstraint_Validate_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Data type is not supported by the {0} constraint.".
		/// </summary>
		internal static string Global_CheckDataType_NotSupported
		{
			get { return GetResource("Global_CheckDataType_NotSupported"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Parameter data type '{0}' cannot be converted into alternative comparison data type '{1}'.".
		/// </summary>
		internal static string Global_CheckValueType_AltNotConvertible
		{
			get { return GetResource("Global_CheckValueType_AltNotConvertible"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Parameter data type '{0}' cannot be converted into expected data type '{1}'.".
		/// </summary>
		internal static string Global_CheckValueType_NotConvertible
		{
			get { return GetResource("Global_CheckValueType_NotConvertible"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Data type may not be 'None'.".
		/// </summary>
		internal static string Global_ParameterDataType_None
		{
			get { return GetResource("Global_ParameterDataType_None"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Parameter for constraint {0} is invalid.".
		/// </summary>
		internal static string Global_SetParameters_Invalid
		{
			get { return GetResource("Global_SetParameters_Invalid"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Invalid number of constraint parameters. Constraint {0} requires {1} parameter(s).".
		/// </summary>
		internal static string Global_SetParameters_InvalidCount
		{
			get { return GetResource("Global_SetParameters_InvalidCount"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Invalid number of constraint parameters. Constraint {0} requires {1} to {2} parameter(s).".
		/// </summary>
		internal static string Global_SetParameters_InvalidCountVariable
		{
			get { return GetResource("Global_SetParameters_InvalidCountVariable"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Invalid number of constraint parameters. Constraint {0} requires at least {1} parameter(s).".
		/// </summary>
		internal static string Global_SetParameters_InvalidMinCount
		{
			get { return GetResource("Global_SetParameters_InvalidMinCount"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} is empty or contains only white-space.".
		/// </summary>
		internal static string Global_Validate_StringEmpty
		{
			get { return GetResource("Global_Validate_StringEmpty"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Type '{0}' cannot be validated by the {1} constraint.".
		/// </summary>
		internal static string Global_Validate_TypeMismatch
		{
			get { return GetResource("Global_Validate_TypeMismatch"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Cannot  compare data type '{0}' to data type '{1}', and no conversion to a comparable data type exists.".
		/// </summary>
		internal static string Global_Validate_TypeNotConvertible
		{
			get { return GetResource("Global_Validate_TypeNotConvertible"); }
		}

		/// <summary>
		/// Gets a localized string similar to "The data type is not supported.".
		/// </summary>
		internal static string InvalidDataTypeException_MessageDefault
		{
			get { return GetResource("InvalidDataTypeException_MessageDefault"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Length may not be negative.".
		/// </summary>
		internal static string LengthConstraint_LengthNegative
		{
			get { return GetResource("LengthConstraint_LengthNegative"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} must be {1} byte(s) in length. Current length: {2} byte(s).".
		/// </summary>
		internal static string LengthConstraint_Validate_FailedBytes
		{
			get { return GetResource("LengthConstraint_Validate_FailedBytes"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} must be {1} character(s) in length. Current length: {2} character(s).".
		/// </summary>
		internal static string LengthConstraint_Validate_FailedString
		{
			get { return GetResource("LengthConstraint_Validate_FailedString"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} contains at least one upper-case character.".
		/// </summary>
		internal static string LowercaseConstraint_Validate_Failed
		{
			get { return GetResource("LowercaseConstraint_Validate_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Maximum length may not be negative.".
		/// </summary>
		internal static string MaximumLengthConstraint_LengthNegative
		{
			get { return GetResource("MaximumLengthConstraint_LengthNegative"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} may not be longer than {1} byte(s). Current length: {2} byte(s).".
		/// </summary>
		internal static string MaximumLengthConstraint_Validate_FailedBytes
		{
			get { return GetResource("MaximumLengthConstraint_Validate_FailedBytes"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} may not be longer than {1} character(s). Current length: {2} character(s).".
		/// </summary>
		internal static string MaximumLengthConstraint_Validate_FailedString
		{
			get { return GetResource("MaximumLengthConstraint_Validate_FailedString"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} may not be greater than '{1}'. Actual value: '{2}'.".
		/// </summary>
		internal static string MaximumValueConstraint_Validate_Failed
		{
			get { return GetResource("MaximumValueConstraint_Validate_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Minimum length may not be negative.".
		/// </summary>
		internal static string MinimumLengthConstraint_LengthNegative
		{
			get { return GetResource("MinimumLengthConstraint_LengthNegative"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} must be at least {1} byte(s) long. Current length: {2} byte(s).".
		/// </summary>
		internal static string MinimumLengthConstraint_Validate_FailedBytes
		{
			get { return GetResource("MinimumLengthConstraint_Validate_FailedBytes"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} must be at least {1} character(s) long. Current length: {2} character(s).".
		/// </summary>
		internal static string MinimumLengthConstraint_Validate_FailedString
		{
			get { return GetResource("MinimumLengthConstraint_Validate_FailedString"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} may not be less than '{1}'. Actual value: '{2}'.".
		/// </summary>
		internal static string MinimumValueConstraint_Validate_Failed
		{
			get { return GetResource("MinimumValueConstraint_Validate_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Parameter conversion failed.".
		/// </summary>
		internal static string ParameterConversionException_Message
		{
			get { return GetResource("ParameterConversionException_Message"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Cannot decrypt parameter data.".
		/// </summary>
		internal static string ParameterConvert_Decrypt_Failed
		{
			get { return GetResource("ParameterConvert_Decrypt_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Cannot encrypt parameter data.".
		/// </summary>
		internal static string ParameterConvert_Encrypt_Failed
		{
			get { return GetResource("ParameterConvert_Encrypt_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Data type {0} is not supported by parameter validation.".
		/// </summary>
		internal static string ParameterConvert_NetToParameterDataType_NoMatch
		{
			get { return GetResource("ParameterConvert_NetToParameterDataType_NoMatch"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Parameter data type {0} cannot be matched to a .NET data type.".
		/// </summary>
		internal static string ParameterConvert_ParameterToNetDataType_NoMatch
		{
			get { return GetResource("ParameterConvert_ParameterToNetDataType_NoMatch"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Parameter validation failed.".
		/// </summary>
		internal static string ParameterValidationException_Message
		{
			get { return GetResource("ParameterValidationException_Message"); }
		}

		/// <summary>
		/// Gets a localized string similar to "No validation error.".
		/// </summary>
		internal static string ParameterValidationResult_Success
		{
			get { return GetResource("ParameterValidationResult_Success"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} contains at least one character not allowed in a file or directory path. Actual value: '{1}'".
		/// </summary>
		internal static string PathConstraint_Validate_FailedChars
		{
			get { return GetResource("PathConstraint_Validate_FailedChars"); }
		}

		/// <summary>
		/// Gets a localized string similar to "The options are not valid.".
		/// </summary>
		internal static string RegexConstraint_OptionsInvalid
		{
			get { return GetResource("RegexConstraint_OptionsInvalid"); }
		}

		/// <summary>
		/// Gets a localized string similar to "The regular expression pattern is not valid.".
		/// </summary>
		internal static string RegexConstraint_PatternInvalid
		{
			get { return GetResource("RegexConstraint_PatternInvalid"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} does not match regular expression '{1}'.".
		/// </summary>
		internal static string RegexConstraint_Validate_Failed
		{
			get { return GetResource("RegexConstraint_Validate_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} contains at least one lower-case character.".
		/// </summary>
		internal static string UppercaseConstraint_Validate_Failed
		{
			get { return GetResource("UppercaseConstraint_Validate_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Cannot deserialize XML data into an object of type '{0}'.".
		/// </summary>
		internal static string ParameterConvert_FromXml_Failed
		{
			get { return GetResource("ParameterConvert_FromXml_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Cannot find a 'Type' constraint to determine the target data type.".
		/// </summary>
		internal static string ParameterConvert_ToDataType_NoTypeConstraint
		{
			get { return GetResource("ParameterConvert_ToDataType_NoTypeConstraint"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Cannot resolve type '{0}' from 'Type' constraint.".
		/// </summary>
		internal static string ParameterConvert_ToDataType_ResolveFailed
		{
			get { return GetResource("ParameterConvert_ToDataType_ResolveFailed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Cannot convert string '{0}' to enumeration of type '{1}'.".
		/// </summary>
		internal static string ParameterConvert_ToEnumeration_Failed
		{
			get { return GetResource("ParameterConvert_ToEnumeration_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Type '{0}' is not an enumeration.".
		/// </summary>
		internal static string ParameterConvert_ToEnumeration_NotEnum
		{
			get { return GetResource("ParameterConvert_ToEnumeration_NotEnum"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Object data type '{0}' does not match parameter data type '{1}'.".
		/// </summary>
		internal static string ParameterConvert_ToString_TypeMismatch
		{
			get { return GetResource("ParameterConvert_ToString_TypeMismatch"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Cannot serialize parameter of type '{0}'.".
		/// </summary>
		internal static string ParameterConvert_ToXml_Failed
		{
			get { return GetResource("ParameterConvert_ToXml_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Data type '{0}' is not supported.".
		/// </summary>
		internal static string ParameterConvert_To_DataTypeNotSupported
		{
			get { return GetResource("ParameterConvert_To_DataTypeNotSupported"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Cannot convert string '{0}' to data type '{1}'.".
		/// </summary>
		internal static string ParameterConvert_To_Failed
		{
			get { return GetResource("ParameterConvert_To_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Cannot parse constraint string.".
		/// </summary>
		internal static string ConstraintParserException_Message
		{
			get { return GetResource("ConstraintParserException_Message"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Constraint string contains an empty constraint.".
		/// </summary>
		internal static string ConstraintParser_Parse_EmptyConstraint
		{
			get { return GetResource("ConstraintParser_Parse_EmptyConstraint"); }
		}

		/// <summary>
		/// Gets a localized string similar to "At least one parameter of constraint [{0}] is invalid.".
		/// </summary>
		internal static string ConstraintParser_Parse_ParametersInvalid
		{
			get { return GetResource("ConstraintParser_Parse_ParametersInvalid"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Invalid data outside of constraints at position {0}.".
		/// </summary>
		internal static string ConstraintParser_Parse_InvalidContent
		{
			get { return GetResource("ConstraintParser_Parse_InvalidContent"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Invalid character '{0}' in constraint name.".
		/// </summary>
		internal static string ConstraintParser_Parse_InvalidCharInName
		{
			get { return GetResource("ConstraintParser_Parse_InvalidCharInName"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Invalid character '{0}' in parameters.".
		/// </summary>
		internal static string ConstraintParser_Parse_InvalidCharInParameters
		{
			get { return GetResource("ConstraintParser_Parse_InvalidCharInParameters"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Invalid character '{0}' after parameter value.".
		/// </summary>
		internal static string ConstraintParser_Parse_InvalidCharAfterParam
		{
			get { return GetResource("ConstraintParser_Parse_InvalidCharAfterParam"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0}: '{1}' is not a valid IP endpoint, host name is invalid.".
		/// </summary>
		internal static string EndpointConstraint_Validate_Failed
		{
			get { return GetResource("EndpointConstraint_Validate_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0}: '{1}' is not a valid IP endpoint, port number is invalid.".
		/// </summary>
		internal static string EndpointConstraint_Validate_FailedPort
		{
			get { return GetResource("EndpointConstraint_Validate_FailedPort"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} is not a valid host name or IP address. Actual value: '{1}'".
		/// </summary>
		internal static string HostNameConstraint_Validate_Failed
		{
			get { return GetResource("HostNameConstraint_Validate_Failed"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Constraint '{0}' is not defined for data type '{1}'.".
		/// </summary>
		internal static string ConstraintParser_HandleUnknownConstraint_NotDefined
		{
			get { return GetResource("ConstraintParser_HandleUnknownConstraint_NotDefined"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Unknown constraint name '{0}'.".
		/// </summary>
		internal static string ConstraintParser_HandleUnknownConstraint_UnknownName
		{
			get { return GetResource("ConstraintParser_HandleUnknownConstraint_UnknownName"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Unmasked character '{0}' in parameter value.".
		/// </summary>
		internal static string ConstraintParser_Parse_UnmaskedChar
		{
			get { return GetResource("ConstraintParser_Parse_UnmaskedChar"); }
		}

		/// <summary>
		/// Gets a localized string similar to "{0} may not be null.".
		/// </summary>
		internal static string ParameterValidator_Validate_ValueNull
		{
			get { return GetResource("ParameterValidator_Validate_ValueNull"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Data type '{1}' is not supported by constraint '{0}'.".
		/// </summary>
		internal static string InvalidDataTypeException_Message
		{
			get { return GetResource("InvalidDataTypeException_Message"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Last constraint is incomplete.".
		/// </summary>
		internal static string ConstraintParser_Parse_ConstraintIncomplete
		{
			get { return GetResource("ConstraintParser_Parse_ConstraintIncomplete"); }
		}

		/// <summary>
		/// Gets a localized string similar to "Invalid character '{0}' after parameters.".
		/// </summary>
		internal static string ConstraintParser_Parse_InvalidCharAfterParams
		{
			get { return GetResource("ConstraintParser_Parse_InvalidCharAfterParams"); }
		}
		#endregion

#if WINDOWS_UWP
		#region Private fields
		private static Windows.ApplicationModel.Resources.Core.ResourceMap mResourceMap;
		private static Windows.ApplicationModel.Resources.Core.ResourceContext mContext;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the main resource map of the assembly.
		/// </summary>
		internal static Windows.ApplicationModel.Resources.Core.ResourceMap ResourceMap
		{
			get
			{
				if (object.ReferenceEquals(mResourceMap, null))
				{
					mResourceMap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap;
				}

				return mResourceMap;
			}
		}

		/// <summary>
		/// Gets or sets the resource context to use when retrieving resources.
		/// </summary>
		internal static Windows.ApplicationModel.Resources.Core.ResourceContext Context
		{
			get { return mContext; }
			set { mContext = value; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Retrieves a string resource using the resource map.
		/// </summary>
		/// <param name="name">The name of the string resource.</param>
		/// <returns>A localized string.</returns>
		internal static string GetResource(string name)
		{
			Windows.ApplicationModel.Resources.Core.ResourceContext context = Context;
			if (context == null)
			{
				context = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForViewIndependentUse();
			}

			Windows.ApplicationModel.Resources.Core.ResourceCandidate resourceCandidate = ResourceMap.GetValue("NerdyDuck.ParameterValidation/Resources/" + name, context);

			if (resourceCandidate == null)
			{
				throw new ArgumentOutOfRangeException(nameof(name));
			}

			return resourceCandidate.ValueAsString;
		}
		#endregion
#endif

#if WINDOWS_DESKTOP
		#region Private fields
		private static System.Resources.ResourceManager mResourceManager;
		private static System.Globalization.CultureInfo mResourceCulture;
		#endregion

		#region Properties
		/// <summary>
		/// Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static System.Resources.ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(mResourceManager, null))
				{
					System.Resources.ResourceManager temp = new System.Resources.ResourceManager("NerdyDuck.ParameterValidation.Properties.Resources", typeof(Resources).Assembly);
					mResourceManager = temp;
				}
				return mResourceManager;
			}
		}

		/// <summary>
		/// Overrides the current thread's CurrentUICulture property for all resource lookups using this strongly typed resource class.
		/// </summary>
		[System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static System.Globalization.CultureInfo Culture
		{
			get { return mResourceCulture; }
			set { mResourceCulture = value; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Retrieves a string resource using the resource manager.
		/// </summary>
		/// <param name="name">The name of the string resource.</param>
		/// <returns>A localized string.</returns>
		internal static string GetResource(string name)
		{
			return ResourceManager.GetString(name, mResourceCulture);
		}
		#endregion
#endif
	}
}
