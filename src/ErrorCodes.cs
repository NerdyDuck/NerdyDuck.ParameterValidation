#region Copyright
/*******************************************************************************
 * NerdyDuck.Collections - Validation and serialization of parameter values
 * 
 * The MIT License (MIT)
 *
 * Copyright (c) Daniel Kopp, dak@nerdyduck.de
 *
 * All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 ******************************************************************************/
#endregion

namespace NerdyDuck.ParameterValidation;

/// <summary>
/// Error codes for the NerdyDuck.ParameterValidation assembly.
/// </summary>
internal enum ErrorCodes
{
	/// <summary>
	/// 0x00; ParameterValidationResult.Success; No error, successful validation.
	/// </summary>
	ParameterValidationResult_Success = 0x00,

	/// <summary>
	/// 0x0001; TypeExtensions.ToStringAssemblyNameOnly; type is null.
	/// </summary>
	TypeExtensions_ToStringAssemblyNameOnly_ArgNull,

	/// <summary>
	/// 0x0002; Constraint.ctor; name is null or empty.
	/// </summary>
	Constraint_ctor_ArgNull,

	/// <summary>
	/// 0x0003; Constraint.Validate; dataType is None.
	/// </summary>
	Constraint_Validate_TypeNone,

	/// <summary>
	/// 0x0004; Constraint.SetParameters; dataType is None.
	/// </summary>
	Constraint_SetParameters_TypeNone,

	/// <summary>
	/// 0x0005; Constraint.SetParameters; parameters is null.
	/// </summary>
	Constraint_SetParameters_ParametersNull,

	/// <summary>
	/// 0x0006; Constraint.AssertDataType; Data Type is not supported by constraint.
	/// </summary>
	Constraint_AssertDataType_TypeNotSupported,

	/// <summary>
	/// 0x0007; FileNameConstraint.Validate; value is empty or whitespace.
	/// </summary>
	FileNameConstraint_Validate_ValueNullEmpty,

	/// <summary>
	/// 0x0008; FileNameConstraint.Validate; value contains invalid characters.
	/// </summary>
	FileNameConstraint_Validate_ValueInvalid,

	/// <summary>
	/// 0x0009; PathConstraint.Validate; value is empty or whitespace.
	/// </summary>
	PathConstraint_Validate_ValueEmpty,

	/// <summary>
	/// 0x000a; PathConstraint.Validate; value contains invalid characters.
	/// </summary>
	PathConstraint_Validate_ValueInvalid,

	/// <summary>
	/// 0x000b; LowerCaseConstraint.Validate; value contains at least one upper-case character.
	/// </summary>
	LowerCaseConstraint_Validate_ValueInvalid,

	/// <summary>
	/// 0x000c; UpperCaseConstraint.Validate; value contains at least one lower-case character.
	/// </summary>
	UpperCaseConstraint_Validate_ValueInvalid,

	/// <summary>
	/// 0x000d; AllowedSchemeConstraint.ctor(string); scheme is null or empty.
	/// </summary>
	AllowedSchemeConstraint_ctor_StringNullEmpty,

	/// <summary>
	/// 0x000e; AllowedSchemeConstraint.ctor(IEnumerable(Of string)); schemes is null.
	/// </summary>
	AllowedSchemeConstraint_ctor_EnumNull,

	/// <summary>
	/// 0x000f; AllowedSchemeConstraint.CheckSchemes; No schemes provided.
	/// </summary>
	AllowedSchemeConstraint_CheckSchemes_NoSchemes,

	/// <summary>
	/// 0x0010; AllowedSchemeConstraint.CheckSchemes; At least one scheme is null, empty or whitespace.
	/// </summary>
	AllowedSchemeConstraint_CheckSchemes_SchemeNullEmpty,

	/// <summary>
	/// 0x0011; AllowedSchemeConstraint.Validate; Scheme is not allowed.
	/// </summary>
	AllowedSchemeConstraint_Validate_SchemeNotAllowed,

	/// <summary>
	/// 0x0012; CharacterSetConstraint.SetParameters; Exactly one parameter required.
	/// </summary>
	CharacterSetConstraint_SetParameters_OnlyOneParam,

	/// <summary>
	/// 0x0013; CharacterSetConstraint.SetParameters; Not a valid character set.
	/// </summary>
	CharacterSetConstraint_SetParameters_CharSetInvalid,

	/// <summary>
	/// 0x0014; CharacterSetConstraint.Validate; At least one character is not defined in character set.
	/// </summary>
	CharacterSetConstraint_Validate_CharNotDefined,

	/// <summary>
	/// 0x0015; MaximumValueConstraint.ctor(ParameterDataType,object); maximumValue is null.
	/// </summary>
	MaximumValueConstraint_ctor_MaxValueNull,

	/// <summary>
	/// 0x0016; ParameterConvert.NetToParameterDataType; type is null.
	/// </summary>
	ParameterConvert_NetToParameterDataType_ArgNull,

	/// <summary>
	/// 0x0017; ParameterConvert.NetToParameterDataType; Type is not supported.
	/// </summary>
	ParameterConvert_NetToParameterDataType_TypeNotSupported,

	/// <summary>
	/// 0x0018; ParameterConvert.ParameterToNetDataType; Data type cannot be matched to a .NET type.
	/// </summary>
	ParameterConvert_ParameterToNetDataType_NoMatch,

	/// <summary>
	/// 0x0019; MaximumValueConstraint.Validate; value is larger than maximum value.
	/// </summary>
	MaximumValueConstraint_Validate_TooLarge,

	/// <summary>
	/// 0x001a; MaximumValueConstraint.SetParameters; Exactly one parameter required.
	/// </summary>
	MaximumValueConstraint_SetParameters_OnlyOneParam,

	/// <summary>
	/// 0x001b; MaximumValueConstraint.SetParameters; Parameter is invalid.
	/// </summary>
	MaximumValueConstraint_SetParameters_ParamInvalid,

	/// <summary>
	/// 0x001c; MaximumValueConstraint.CheckDataType; Data type is not supported by constraint.
	/// </summary>
	MaximumValueConstraint_CheckDataType_TypeNotSupported,

	/// <summary>
	/// 0x001d; MaximumValueConstraint.Validate; Data type not comparable, and no conversion exists.
	/// </summary>
	MaximumValueConstraint_Validate_TypeMismatch,

	/// <summary>
	/// 0x001e; MaximumValueConstraint.CheckValueType; Data type is not of the expected type, and cannot be converted.
	/// </summary>
	MaximumValueConstraint_CheckValueType_TypeMismatch,

	/// <summary>
	/// 0x001f; MinimumValueConstraint.ctor(ParameterDataType,object); minimumValue is null.
	/// </summary>
	MinimumValueConstraint_ctor_MinValueNull,

	/// <summary>
	/// 0x0020; MinimumValueConstraint.SetParameters; Parameter is invalid.
	/// </summary>
	MinimumValueConstraint_SetParameters_ParamInvalid,

	/// <summary>
	/// 0x0021; MinimumValueConstraint.Validate; value is less than minimum value.
	/// </summary>
	MinimumValueConstraint_Validate_TooSmall,

	/// <summary>
	/// 0x0022; MinimumValueConstraint.SetParameters; Exactly one parameter required.
	/// </summary>
	MinimumValueConstraint_SetParameters_OnlyOneParam,

	/// <summary>
	/// 0x0023; MinimumValueConstraint.Validate; Data type not comparable, and no conversion exists.
	/// </summary>
	MinimumValueConstraint_Validate_TypeMismatch,

	/// <summary>
	/// 0x0024; MinimumValueConstraint.CheckDataType; Data type is not supported by constraint.
	/// </summary>
	MinimumValueConstraint_CheckDataType_TypeNotSupported,

	/// <summary>
	/// 0x0025; MinimumValueConstraint.CheckValueType; Data type is not of the expected type, and cannot be converted.
	/// </summary>
	MinimumValueConstraint_CheckValueType_TypeMismatch,

	/// <summary>
	/// 0x0026; StepConstraint.ctor(ParameterDataType,object); stepSize is null.
	/// </summary>
	StepConstraint_ctor_StepSizeNull,

	/// <summary>
	/// 0x0027; StepConstraint.SetParameters; Exactly one parameter required.
	/// </summary>
	StepConstraint_SetParameters_OnlyOneParam,

	/// <summary>
	/// 0x0028; StepConstraint.SetParameters; Parameter is invalid.
	/// </summary>
	StepConstraint_SetParameters_ParamInvalid,

	/// <summary>
	/// 0x0029; StepConstraint.CheckDataType; Data type is not supported by constraint.
	/// </summary>
	StepConstraint_CheckDataType_TypeNotSupported,

	/// <summary>
	/// 0x002a; StepConstraint.CheckValueType; Data type is not of the expected type, and cannot be converted.
	/// </summary>
	StepConstraint_CheckValueType_TypeMismatch,

	/// <summary>
	/// 0x002b; DecimalPlacesConstraint.SetParameters; Exactly one parameter required.
	/// </summary>
	DecimalPlacesConstraint_SetParameters_OnlyOneParam,

	/// <summary>
	/// 0x002c; DecimalPlacesConstraint.SetParameters; Parameter is invalid.
	/// </summary>
	DecimalPlacesConstraint_SetParameters_ParamInvalid,

	/// <summary>
	/// 0x002d; LengthConstraint.ctor(int); length is less than 0.
	/// </summary>
	LengthConstraint_ctor_LengthNegative,

	/// <summary>
	/// 0x002e; MaximumLengthConstraint.ctor(int); length is less than 0.
	/// </summary>
	MaximumLengthConstraint_ctor_LengthNegative,

	/// <summary>
	/// 0x002f; MinimumLengthConstraint.ctor(int); length is less than 0.
	/// </summary>
	MinimumLengthConstraint_ctor_LengthNegative,

	/// <summary>
	/// 0x0030; LengthConstraint.SetParameters; Exactly one parameter required.
	/// </summary>
	LengthConstraint_SetParameters_OnlyOneParam,

	/// <summary>
	/// 0x0031; MaximumLengthConstraint.SetParameters; Exactly one parameter required.
	/// </summary>
	MaximumLengthConstraint_SetParameters_OnlyOneParam,

	/// <summary>
	/// 0x0032; MinimumLengthConstraint.SetParameters; Exactly one parameter required.
	/// </summary>
	MinimumLengthConstraint_SetParameters_OnlyOneParam,

	/// <summary>
	/// 0x0033; LengthConstraint.SetParameters; length is less than 0.
	/// </summary>
	LengthConstraint_SetParameters_LengthNegative,

	/// <summary>
	/// 0x0034; MaximumLengthConstraint.SetParameters; length is less than 0.
	/// </summary>
	MaximumLengthConstraint_SetParameters_LengthNegative,

	/// <summary>
	/// 0x0035; MinimumLengthConstraint.SetParameters; length is less than 0.
	/// </summary>
	MinimumLengthConstraint_SetParameters_LengthNegative,

	/// <summary>
	/// 0x0036; LengthConstraint.SetParameters; Parameter is invalid.
	/// </summary>
	LengthConstraint_SetParameters_ParamInvalid,

	/// <summary>
	/// 0x0037; MaximumLengthConstraint.SetParameters; Parameter is invalid.
	/// </summary>
	MaximumLengthConstraint_SetParameters_ParamInvalid,

	/// <summary>
	/// 0x0038; MinimumLengthConstraint.SetParameters; Parameter is invalid.
	/// </summary>
	MinimumLengthConstraint_SetParameters_ParamInvalid,

	/// <summary>
	/// 0x0039; LengthConstraint.Validate; Data type cannot be validated by constraint.
	/// </summary>
	LengthConstraint_Validate_TypeNotSupported,

	/// <summary>
	/// 0x003a; MaximumLengthConstraint.Validate; Data type cannot be validated by constraint.
	/// </summary>
	MaximumLengthConstraint_Validate_TypeNotSupported,

	/// <summary>
	/// 0x003b; MinimumLengthConstraint.Validate; Data type cannot be validated by constraint.
	/// </summary>
	MinimumLengthConstraint_Validate_TypeNotSupported,

	/// <summary>
	/// 0x003c; LengthConstraint.Validate; String or array length does not match.
	/// </summary>
	LengthConstraint_Validate_LengthInvalid,

	/// <summary>
	/// 0x003d; MaximumLengthConstraint.Validate; String or array is too long.
	/// </summary>
	MaximumLengthConstraint_Validate_TooLong,

	/// <summary>
	/// 0x003e; MinimumLengthConstraint.Validate; String or array is too short.
	/// </summary>
	MinimumLengthConstraint_Validate_TooShort,

	/// <summary>
	/// 0x003f; RegexConstraint.ctor(string,RegexOptions); regularExpression is null or empty.
	/// </summary>
	RegexConstraint_ctor_StringNullEmpty,

	/// <summary>
	/// 0x0040; RegexConstraint.ctor(string,RegexOptions); options are invalid.
	/// </summary>
	RegexConstraint_ctor_OptionsInvalid,

	/// <summary>
	/// 0x0041; RegexConstraint.ctor(string,RegexOptions); regularExpression is invalid.
	/// </summary>
	RegexConstraint_ctor_RegexInvalid,

	/// <summary>
	/// 0x0042; RegexConstraint.SetParameters; At least one parameter required.
	/// </summary>
	RegexConstraint_SetParameters_OneParam,

	/// <summary>
	/// 0x0043; RegexConstraint.SetParameters; regularExpression is invalid.
	/// </summary>
	RegexConstraint_SetParameters_RegexInvalid,

	/// <summary>
	/// 0x0044; RegexConstraint.SetParameters; options are invalid.
	/// </summary>
	RegexConstraint_SetParameters_OptionsInvalid,

	/// <summary>
	/// 0x0045; RegexConstraint.Validate; String does not match regular expression.
	/// </summary>
	RegexConstraint_Validate_NoMatch,

	/// <summary>
	/// 0x0046; TypeConstraint.ctor(string); typeName is null or empty.
	/// </summary>
	TypeConstraint_ctor_TypeNullEmpty,

	/// <summary>
	/// 0x0047; TypeConstraint.SetParameters; typeName is null or empty.
	/// </summary>
	TypeConstraint_SetParameters_TypeNullEmpty,

	/// <summary>
	/// 0x0048; EnumTypeConstraint.Validate; value is an enumeration of unexpected type.
	/// </summary>
	EnumTypeConstraint_Validate_WrongEnum,

	/// <summary>
	/// 0x0049; EnumValuesConstraint.ctor(ParameterDataType,bool,IDictionary); data type is not an integer.
	/// </summary>
	EnumValuesConstraint_ctor_NotInteger,

	/// <summary>
	/// 0x004a; EnumValuesConstraint.ctor(ParameterDataType,bool,IDictionary); values is null.
	/// </summary>
	EnumValuesConstraint_ctor_ValuesNull,

	/// <summary>
	/// 0x004b; EnumValuesConstraint.ctor(ParameterDataType,bool,IDictionary); Value type does not match underlying type of enumeration.
	/// </summary>
	EnumValuesConstraint_ctor_ValuesTypeMismatch,

	/// <summary>
	/// 0x004c; EnumValuesConstraint.SetParameters; First parameter is not a valid ParameterDataType.
	/// </summary>
	EnumValuesConstraint_SetParameters_InvalidDataType,

	/// <summary>
	/// 0x004d; EnumValuesConstraint.SetParameters; No enumeration values specified.
	/// </summary>
	EnumValuesConstraint_SetParameters_NoValues,

	/// <summary>
	/// 0x004e; EnumValuesConstraint.SetParameters; Invalid name=value pair.
	/// </summary>
	EnumValuesConstraint_SetParameters_ValuePairInvalid,

	/// <summary>
	/// 0x004f; EnumValuesConstraint.SetParameters; Invalid key value.
	/// </summary>
	EnumValuesConstraint_SetParameters_KeyInvalid,

	/// <summary>
	/// 0x0050; EnumValuesConstraint.SetParameters; Cannot convert into underlying data type.
	/// </summary>
	EnumValuesConstraint_SetParameters_DataTypeMismatch,

	/// <summary>
	/// 0x0051; EnumValuesConstraint.Validate; Not set up.
	/// </summary>
	EnumValuesConstraint_Validate_NotSetUp,

	/// <summary>
	/// 0x0052; EnumValuesConstraint.Validate; Value contains a flag not in the enumeration.
	/// </summary>
	EnumValuesConstraint_Validate_InvalidFlag,

	/// <summary>
	/// 0x0053; EnumTypeConstraint.Validate; Value contains a flag not in the enumeration.
	/// </summary>
	EnumTypeConstraint_Validate_InvalidFlag,

	/// <summary>
	/// 0x0054; EnumValuesConstraint.Validate; Value is not part of the enumeration.
	/// </summary>
	EnumValuesConstraint_Validate_NotInEnum,

	/// <summary>
	/// 0x0055; EnumTypeConstraint.Validate; Value is not part of the enumeration.
	/// </summary>
	EnumTypeConstraint_Validate_NotInEnum,

	/// <summary>
	/// 0x0056; EnumValuesConstraint.Validate; Data type is not supported.
	/// </summary>
	EnumValuesConstraint_Validate_TypeNotSupported,

	/// <summary>
	/// 0x0057; EnumTypeConstraint.Validate; Data type is not supported.
	/// </summary>
	EnumTypeConstraint_Validate_TypeNotSupported,

	/// <summary>
	/// 0x0058; ParameterConvert.Encrypt; Cannot encrypt value.
	/// </summary>
	ParameterConvert_Encrypt_Failed,

	/// <summary>
	/// 0x0059; ParameterConvert.Decrypt; Cannot decrypt value.
	/// </summary>
	ParameterConvert_Decrypt_Failed,

	/// <summary>
	/// 0x005a; ParameterConvert.ToBoolean; value is null.
	/// </summary>
	ParameterConvert_ToBoolean_ValueNull,

	/// <summary>
	/// 0x005b; ParameterConvert.ToBoolean; value string does not represent a bool value.
	/// </summary>
	ParameterConvert_ToBoolean_ParseFailed,

	/// <summary>
	/// 0x005c; ParameterConvert.ToByte; value is null.
	/// </summary>
	ParameterConvert_ToByte_ValueNull,

	/// <summary>
	/// 0x005d; ParameterConvert.ToByte; value string does not represent a byte value.
	/// </summary>
	ParameterConvert_ToByte_ParseFailed,

	/// <summary>
	/// 0x005e; ParameterConvert.ToByteArray; value is null.
	/// </summary>
	ParameterConvert_ToByteArray_ValueNull,

	/// <summary>
	/// 0x005f; ParameterConvert.ToByteArray; value string does not represent a byte array.
	/// </summary>
	ParameterConvert_ToByteArray_ParseFailed,

	/// <summary>
	/// 0x0060; ParameterConvert.ToDateTimeOffset; value is null.
	/// </summary>
	ParameterConvert_ToDateTimeOffset_ValueNull,

	/// <summary>
	/// 0x0061; ParameterConvert.ToDateTimeOffset; value string does not represent a DateTimeOffset.
	/// </summary>
	ParameterConvert_ToDateTimeOffset_ParseFailed,

	/// <summary>
	/// 0x0062; ParameterConvert.ToDecimal; value is null.
	/// </summary>
	ParameterConvert_ToDecimal_ValueNull,

	/// <summary>
	/// 0x0063; ParameterConvert.ToDecimal; value string does not represent a decimal.
	/// </summary>
	ParameterConvert_ToDecimal_ParseFailed,

	/// <summary>
	/// 0x0064; ParameterConvert.ToEnumeration; enumType is null.
	/// </summary>
	ParameterConvert_ToEnumeration_EnumTypeNull,

	/// <summary>
	/// 0x0065; ParameterConvert.ToEnumeration; enumType is not an enumeration.
	/// </summary>
	ParameterConvert_ToEnumeration_NotEnum,

	/// <summary>
	/// 0x0066; ParameterConvert.ToEnumeration; value is null.
	/// </summary>
	ParameterConvert_ToEnumeration_ValueNull,

	/// <summary>
	/// 0x0067; ParameterConvert.ToEnumeration; value string does not represent a value of the enumeration.
	/// </summary>
	ParameterConvert_ToEnumeration_ParseFailed,

	/// <summary>
	/// 0x0068; ParameterConvert.ToGuid; value is null.
	/// </summary>
	ParameterConvert_ToGuid_ValueNull,

	/// <summary>
	/// 0x0069; ParameterConvert.ToGuid; value string does not represent a Guid.
	/// </summary>
	ParameterConvert_ToGuid_ParseFailed,

	/// <summary>
	/// 0x006a; ParameterConvert.ToInt16; value is null.
	/// </summary>
	ParameterConvert_ToInt16_ValueNull,

	/// <summary>
	/// 0x006b; ParameterConvert.ToInt16; value string does not represent a Int16.
	/// </summary>
	ParameterConvert_ToInt16_ParseFailed,

	/// <summary>
	/// 0x006c; ParameterConvert.ToInt32; value is null.
	/// </summary>
	ParameterConvert_ToInt32_ValueNull,

	/// <summary>
	/// 0x006d; ParameterConvert.ToInt32; value string does not represent a Int32.
	/// </summary>
	ParameterConvert_ToInt32_ParseFailed,

	/// <summary>
	/// 0x006e; ParameterConvert.ToInt64; value is null.
	/// </summary>
	ParameterConvert_ToInt64_ValueNull,

	/// <summary>
	/// 0x006f; ParameterConvert.ToInt64; value string does not represent a Int64.
	/// </summary>
	ParameterConvert_ToInt64_ParseFailed,

	/// <summary>
	/// 0x0070; ParameterConvert.ToTimeSpan; value is null.
	/// </summary>
	ParameterConvert_ToTimeSpan_ValueNull,

	/// <summary>
	/// 0x0071; ParameterConvert.ToTimeSpan; value string does not represent a TimeSpan.
	/// </summary>
	ParameterConvert_ToTimeSpan_ParseFailed,

	/// <summary>
	/// 0x0072; ParameterConvert.ToUInt16; value is null.
	/// </summary>
	ParameterConvert_ToUInt16_ValueNull,

	/// <summary>
	/// 0x0073; ParameterConvert.ToUInt16; value string does not represent a UInt16.
	/// </summary>
	ParameterConvert_ToUInt16_ParseFailed,

	/// <summary>
	/// 0x0074; ParameterConvert.ToUInt32; value is null.
	/// </summary>
	ParameterConvert_ToUInt32_ValueNull,

	/// <summary>
	/// 0x0075; ParameterConvert.ToUInt32; value string does not represent a UInt32.
	/// </summary>
	ParameterConvert_ToUInt32_ParseFailed,

	/// <summary>
	/// 0x0076; ParameterConvert.ToUInt64; value is null.
	/// </summary>
	ParameterConvert_ToUInt64_ValueNull,

	/// <summary>
	/// 0x0077; ParameterConvert.ToUInt64; value string does not represent a UInt64.
	/// </summary>
	ParameterConvert_ToUInt64_ParseFailed,

	/// <summary>
	/// 0x0078; ParameterConvert.ToUri; value is null.
	/// </summary>
	ParameterConvert_ToUri_ValueNull,

	/// <summary>
	/// 0x0079; ParameterConvert.ToUri; value string does not represent a Uri.
	/// </summary>
	ParameterConvert_ToUri_ParseFailed,

	/// <summary>
	/// 0x007a; ParameterConvert.ToVersion; value is null.
	/// </summary>
	ParameterConvert_ToVersion_ValueNull,

	/// <summary>
	/// 0x007b; ParameterConvert.ToVersion; value string does not represent a Version.
	/// </summary>
	ParameterConvert_ToVersion_ParseFailed,

	/// <summary>
	/// 0x007c; ParameterConvert.ToSByte; value is null.
	/// </summary>
	ParameterConvert_ToSByte_ValueNull,

	/// <summary>
	/// 0x007d; ParameterConvert.ToSByte; value string does not represent a signed byte.
	/// </summary>
	ParameterConvert_ToSByte_ParseFailed,

	/// <summary>
	/// 0x007e; ParameterConvert.ToString(object,ParameterDataype,IList); data type is not supported.
	/// </summary>
	ParameterConvert_ToString_TypeNotSupported,

	/// <summary>
	/// 0x007f; ParameterConvert.ToString(object,ParameterDataype,IList); value type does not match parameter data type.
	/// </summary>
	ParameterConvert_ToString_TypeMismatch,

	/// <summary>
	/// 0x0080; ParameterConvert.ToDataType; No TypeConstraint found to deduce the type of enumeration.
	/// </summary>
	ParameterConvert_ToDataType_EnumNoTypeConstraint,

	/// <summary>
	/// 0x0081; ParameterConvert.ToDataType; A TypeConstraint was found, but the type of the enumeration could not be resolved.
	/// </summary>
	ParameterConvert_ToDataType_ResolveEnumFailed,

	/// <summary>
	/// 0x0082; ParameterConvert.ToDataType; No TypeConstraint found to deduce the type of the XML-serialized object.
	/// </summary>
	ParameterConvert_ToDataType_XmlNoTypeConstraint,

	/// <summary>
	/// 0x0083; ParameterConvert.ToDataType; A TypeConstraint was found, but the type of the XML-serialized object could not be resolved.
	/// </summary>
	ParameterConvert_ToDataType_ResolveXmlFailed,

	/// <summary>
	/// 0x0084; ParameterConvert.ToDataType; data type is not supported.
	/// </summary>
	ParameterConvert_ToDataType_TypeNotSupported,

	/// <summary>
	/// 0x0085; ParameterConvert.FromXml; value is null.
	/// </summary>
	ParameterConvert_FromXml_ValueNull,

	/// <summary>
	/// 0x0086; ParameterConvert.FromXml; type is null.
	/// </summary>
	ParameterConvert_FromXml_TypeNull,

	/// <summary>
	/// 0x0087; ParameterConvert.FromXml; value cannot be deserialized into an instance of type.
	/// </summary>
	ParameterConvert_FromXml_ParseFailed,

	/// <summary>
	/// 0x0088; ParameterConvert.ToXml; value cannot be serialized into an XML string.
	/// </summary>
	ParameterConvert_ToXml_SerializeFailed,

	/// <summary>
	/// 0x0089; HostNameConstraint.Validate; value is empty.
	/// </summary>
	HostNameConstraint_Validate_ValueEmpty,

	/// <summary>
	/// 0x008a; HostnameConstraint.Validate; Not a valid host name or IP address.
	/// </summary>
	HostnameConstraint_Validate_NotHostOrIP,

	/// <summary>
	/// 0x008b; EndpointConstraint.Validate; value is empty.
	/// </summary>
	EndpointConstraint_Validate_ValueEmpty,

	/// <summary>
	/// 0x008c; EndpointConstraint.Validate; Not a valid host name or IP address.
	/// </summary>
	EndpointConstraint_Validate_NotHostOrIP,

	/// <summary>
	/// 0x008d; EndpointConstraint.Validate; Not a valid port number.
	/// </summary>
	EndpointConstraint_Validate_PortInvalid,

	/// <summary>
	/// 0x008e; ConstraintParser.HandleInConstraint; String contains an empty constraint.
	/// </summary>
	ConstraintParser_HandleInConstraint_EmptyConstraint,

	/// <summary>
	/// 0x008f; ConstraintParser.CreateConstraint; At least one constraint parameter is invalid.
	/// </summary>
	ConstraintParser_CreateConstraint_ParamInvalid,

	/// <summary>
	/// 0x0090; ConstraintParser.Parse; Data type is none.
	/// </summary>
	ConstraintParser_Parse_DataTypeNone,

	/// <summary>
	/// 0x0091; ConstraintParser.HandleInConstraint; Invalid character in constraint name.
	/// </summary>
	ConstraintParser_HandleInConstraint_InvalidChar,

	/// <summary>
	/// 0x0092; ConstraintParser.HandleOutsideConstraint; Data outside of a constraint.
	/// </summary>
	ConstraintParser_HandleOutsideConstraint_DataOutside,

	/// <summary>
	/// 0x0093; ConstraintParser.HandleInParameters; Invalid character in parameter clause.
	/// </summary>
	ConstraintParser_HandleInParameters_ParamCharInvalid,

	/// <summary>
	/// 0x0094; ConstraintParser.HandleInParameter; Unmasked character in parameter.
	/// </summary>
	ConstraintParser_HandleInParameter_ParamUnmaskedChar,

	/// <summary>
	/// 0x0095; ConstraintParser.HandleAfterParameter; Invalid character after parameter value.
	/// </summary>
	ConstraintParser_HandleAfterParameter_CharAfterParam,

	/// <summary>
	/// 0x0096; ConstraintParser.HandleUnknownConstraint; Constraint name is unknown.
	/// </summary>
	ConstraintParser_HandleUnknownConstraint_UnknownConstraint,

	/// <summary>
	/// 0x0097; ConstraintParser.HandleUnknownConstraint; Constraint is not defined for this data type.
	/// </summary>
	ConstraintParser_HandleUnknownConstraint_NotDefinedForType,

	/// <summary>
	/// 0x0098; Constraint.Validate; memberName is null or empty.
	/// </summary>
	Constraint_Validate_NameNullEmpty,

	/// <summary>
	/// 0x0099; Constraint.GetParameters; parameters is null.
	/// </summary>
	Constraint_GetParameters_ArgNull,

	/// <summary>
	/// 0x009a; ParameterValidator.Validate; Validation failed.
	/// </summary>
	ParameterValidator_Validate_ValidationFailed,

	/// <summary>
	/// 0x009b; ConstraintAttribute.ctor; constraints are null or empty.
	/// </summary>
	ConstraintAttribute_ctor_ConstraintsNullEmpty,

	/// <summary>
	/// 0x009c; ConstraintAttribute.ctor; dataType may not be None.
	/// </summary>
	ConstraintAttribute_ctor_DataTypeNone,

	/// <summary>
	/// 0x009d; ParameterValidator.GetValidationResult; dataType may not be None.
	/// </summary>
	ParameterValidator_GetValidationResult_DataTypeNone,

	/// <summary>
	/// 0x009e; ParameterValidator.GetValidationResult; memberName may not be null or empty.
	/// </summary>
	ParameterValidator_GetValidationResult_NameNullEmpty,

	/// <summary>
	/// 0x009f; ParameterValidator.GetValidationResult; value is null, but no NullConstraint set.
	/// </summary>
	ParameterValidator_GetValidationResult_ValueNull,

	/// <summary>
	/// 0x00a0; Constraint.GetObjectData; info is null.
	/// </summary>
	Constraint_GetObjectData_InfoNull,

	/// <summary>
	/// 0x00a1; Constraint.AssertDataType; expectedTypes is null.
	/// </summary>
	Constraint_AssertDataType_TypesNull,

	/// <summary>
	/// 0x00a2; ConstraintParser.HandleAfterParameters; Invalid character after parameters.
	/// </summary>
	ConstraintParser_HandleAfterParameters_CharAfterParams,

	/// <summary>
	/// 0x00a3; ParameterValidationResult.GetObjectData; info is null.
	/// </summary>
	ParameterValidationResult_GetObjectData_InfoNull,

	/// <summary>
	/// 0x00a4; Constraint.OnValidation; results is null.
	/// </summary>
	Constraint_OnValidation_ResultsNull,

	/// <summary>
	/// 0x00a5; Constraint.OnValidation; value is null.
	/// </summary>
	Constraint_OnValidation_ValueNull,

	/// <summary>
	/// 0x00a6; ConstraintParser.Parse; Last constraint is incomplete.
	/// </summary>
	ConstraintParser_Parse_IncompleteConstraint,

	/// <summary>
	/// 0x00a7; ParameterValidationResult.ctor; validationResult is null.
	/// </summary>
	ParameterValidationResult_ctor_ResultNull,

	/// <summary>
	/// 0x00a8; DisplayHintConstraint.ctor(string); parameter is null or empty.
	/// </summary>
	DisplayHintConstraint_ctor_ArgNullEmpty,

	/// <summary>
	/// 0x00a9; DisplayHintConstraint.ctor(IEnumerable(Of string)); parameters is null.
	/// </summary>
	DisplayHintConstraint_ctor_ArgNull,

	/// <summary>
	/// 0x00aa; DisplayHintConstraint.CheckParameters; No parameters provided.
	/// </summary>
	DisplayHintConstraint_CheckParameters_NoParams,

	/// <summary>
	/// 0x00ab; DisplayHintConstraint.CheckParameters; At least one parameter is null, empty or whitespace.
	/// </summary>
	DisplayHintConstraint_CheckParameters_ParamNullEmpty,

	/// <summary>
	/// 0x00ac; ParameterConvert.ExamineEnumeration; type is null.
	/// </summary>
	ParameterConvert_ExamineEnumeration_TypeNull,

	/// <summary>
	/// 0x00ad; ParameterConvert.ExamineEnumeration; type is not an enumeration.
	/// </summary>
	ParameterConvert_ExamineEnumeration_NotEnum,

	/// <summary>
	/// 0x00ae; DatabaseConstraint.SetParameters; Constraint requires one to three parameters.
	/// </summary>
	DatabaseConstraint_SetParameters_ParamCountInvalid,

	/// <summary>
	/// 0x00af; EnumTypeConstraint.SetParameters; Exactly one parameter required.
	/// </summary>
	EnumTypeConstraint_SetParameters_OnlyOneParam,

	/// <summary>
	/// 0x00b0; EnumTypeConstraint.SetParameters; Type name is null or empty.
	/// </summary>
	EnumTypeConstraint_SetParameters_ParamNullEmpty,

	/// <summary>
	/// 0x00b1; EnumValuesConstraint.SetParameters; At least 2 parameters required.
	/// </summary>
	EnumValuesConstraint_SetParameters_ParamCountInvalid,

	/// <summary>
	/// 0x00b2; TypeConstraint.SetParameters; Exactly one parameter required.
	/// </summary>
	TypeConstraint_SetParameters_OnlyOneParam,

	/// <summary>
	/// 0x00b3; Constraint_ctor(SerializationInfo,StreamingContext); info is null.
	/// </summary>
	Constraint_ctor_InfoNull,

	/// <summary>
	/// 0x00b3; ParameterValidationErrorEventArgs_ctor(object,ParameterDataType,IReadOnlyList&lt;Constraint&gt;,string,string,IList6lt;ParameterValidationResult&gt;); validationResults is null.
	/// </summary>
	ParameterValidationErrorEventArgs_ctor_ResultsNull,

	/// <summary>
	/// Cannot retrieve strong name of assembly.
	/// </summary>
	ParameterConvert_AES_NoAssemblyName,

	ParameterConvert_FromXml_NoObjectDeserialized,

	/// <summary>
	/// 0x00b5; Last error code to check numeration. Test method: ConstraintTest.CheckErrorCodes.
	/// </summary>
	LastErrorCode
}
