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

using System.Reflection;

namespace NerdyDuck.ParameterValidation.Constraints;

/// <summary>
/// Specifies valid values of an enumeration parameter.
/// </summary>
/// <remarks>
/// <para>
/// <list type="bullet">
/// <item>The textual representation of the constraint is <c>[Values([underlyingType,][Flags,]enum1=value1[,enum2=value2,...)]</c>. <c>underlyingType</c> must one of the <see cref="ParameterDataType"/> values specifying an integer type. If <c>Flags</c> is specified, then the values of the enumeration may be combined. <c>enum1=value1</c> defines a key/value pair, where <c>enum1</c> is the textual representation of the value, and <c>value1</c> is the underlying integer value, either in decimal or in hexadecimal representation.</item>
/// <item>The constraint is applicable to the <see cref="ParameterDataType.Enum"/> data type only.</item>
/// <item>If an enumeration parameter with that constraint contains a value that is not defined in the list of enumeration values, a <see cref="ParameterValidationResult"/> is added to the output of <see cref="Constraint.Validate(object, ParameterDataType, string, string)"/>.</item>
/// <item>If the <c>Flags</c> parameter is set, the validated value may also be a combination of enumeration values.</item>
/// <item>This <see cref="Constraint"/> can work on its own, especially in scenarios where actual enumeration type is not available on all systems that need to handle the parameter value. But it can also work in combination with the <see cref="EnumTypeConstraint"/> to further limit the number of valid enumeration values.</item>
/// <item>The <see cref="EnumValuesConstraint"/> can also be used to populate a combo box or radio buttons when displaying the parameter.</item>
/// </list>
/// </para>
/// </remarks>
[Serializable]
public class EnumValuesConstraint : Constraint
{
	private const string FlagsParameter = "Flags";
	private const string CountName = "ValueCount";
	private const string NamePrefix = "Name_";
	private const string ValuePrefix = "Value_";

	private Dictionary<string, object>? _enumValues;
	private long _flagMask;

	/// <summary>
	/// Gets the underlying type of the enumeration.
	/// </summary>
	/// <value>The integer type the enumeration is based on.</value>
	public Type? UnderlyingType { get; private set; }

	/// <summary>
	/// Gets the underlying data type of the enumeration.
	/// </summary>
	/// <value>The integer type the enumeration is based on.</value>
	public ParameterDataType UnderlyingDataType { get; private set; }

	/// <summary>
	/// Gets a value indicating if the enumeration type represented by the <see cref="EnumValuesConstraint"/> has the <see cref="FlagsAttribute"/>.
	/// </summary>
	/// <value><see langword="true"/>, if the type has the <see cref="FlagsAttribute"/>; otherwise, <see langword="false"/>.</value>
	public bool HasFlags { get; private set; }

	/// <summary>
	/// Gets a read-only dictionary of enumeration names and values.
	/// </summary>
	/// <value>A read-only dictionary of string keys and integer values.</value>
	public IReadOnlyDictionary<string, object>? EnumValues => _enumValues;

	/// <summary>
	/// Initializes a new instance of the <see cref="EnumValuesConstraint"/> class.
	/// </summary>
	public EnumValuesConstraint()
		: base(EnumValuesConstraintName) => ResetFields();

	/// <summary>
	/// Initializes a new instance of the <see cref="EnumValuesConstraint"/> class with the specified type name.
	/// </summary>
	/// <param name="type">The underlying type of the enumeration.</param>
	/// <param name="hasFlags">A value indicating if the enumeration type represented by the <see cref="EnumValuesConstraint"/> has the <see cref="FlagsAttribute"/>.</param>
	/// <param name="values">A dictionary of enumeration names and values.</param>
	public EnumValuesConstraint(ParameterDataType type, bool hasFlags, IDictionary<string, object> values)
		: base(EnumValuesConstraintName)
	{
		if (!ParameterConvert.IsIntegerType(type))
		{
			throw new CodedArgumentOutOfRangeException(HResult.Create(ErrorCodes.EnumValuesConstraint_ctor_NotInteger), nameof(type), type, TextResources.EnumValuesConstraint_NotInteger);
		}
		if (values == null || values.Count == 0)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.EnumValuesConstraint_ctor_ValuesNull), nameof(values));
		}
		ResetFields();
		UnderlyingDataType = type;
		UnderlyingType = ParameterConvert.ParameterToNetDataType(type);
		HasFlags = hasFlags;
		_enumValues = new Dictionary<string, object>();
		foreach (KeyValuePair<string, object> pair in values)
		{
			if (pair.Value.GetType() == UnderlyingType)
			{
				_enumValues.Add(pair.Key, pair.Value);
			}
			else
			{
				try
				{
					object ChangedType = Convert.ChangeType(pair.Value, UnderlyingType, CultureInfo.InvariantCulture);
					_enumValues.Add(pair.Key, ChangedType);
				}
				catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
				{
					throw new CodedArgumentException(HResult.Create(ErrorCodes.EnumValuesConstraint_ctor_ValuesTypeMismatch), string.Format(CultureInfo.CurrentCulture, TextResources.EnumValuesConstraint_InvalidValueType, pair.Value.GetType().Name, UnderlyingDataType), nameof(values), ex);
				}
			}
		}

		CreateMask();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="EnumValuesConstraint"/> class with serialized data.
	/// </summary>
	/// <param name="info">The object that holds the serialized object data.</param>
	/// <param name="context">The contextual information about the source or destination.</param>
	/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
	/// <exception cref="SerializationException">The constraint could not be deserialized correctly.</exception>
	protected EnumValuesConstraint(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		ResetFields();
		UnderlyingDataType = (ParameterDataType)(info.GetValue(nameof(UnderlyingDataType), typeof(ParameterDataType)) ?? throw new CodedSerializationException(HResult.Create(ErrorCodes.EnumValuesConstraint_ctor_NoUnderlyingDataType), TextResources.EnumValuesConstraint_ctor_NoUnderlyingDataType));
		UnderlyingType = ParameterConvert.ParameterToNetDataType(UnderlyingDataType);
		HasFlags = info.GetBoolean(nameof(HasFlags));
		int ValueCount = info.GetInt32(CountName);
		_enumValues = new Dictionary<string, object>();
		for (int i = 0; i < ValueCount; i++)
		{
			_enumValues.Add(info.GetString(NamePrefix + i.ToString(CultureInfo.InvariantCulture))
				?? throw new CodedSerializationException(HResult.Create(ErrorCodes.EnumValuesConstraint_ctor_KeyMissing), TextResources.EnumValuesConstraint_ctor_KeyMissing),
				info.GetValue(ValuePrefix + i.ToString(CultureInfo.InvariantCulture), UnderlyingType)
				?? throw new CodedSerializationException(HResult.Create(ErrorCodes.EnumValuesConstraint_ctor_ValueMissing), TextResources.EnumValuesConstraint_ctor_ValueMissing));
		}
		CreateMask();
	}

	/// <summary>
	/// Creates a <see cref="EnumValuesConstraint"/> from the specified type.
	/// </summary>
	/// <param name="type">The enumeration type to create a constraint for.</param>
	/// <returns>A <see cref="EnumValuesConstraint"/> containing all constraint values, the underlying type, and the Flags parameter, if appropriate.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
	/// <exception cref="CodedArgumentException"><paramref name="type"/> is not an enumeration.</exception>
	public static EnumValuesConstraint FromType(Type type)
	{

		// Also handles argument checking.
		Dictionary<string, object> enumValues = ParameterConvert.ExamineEnumeration(type, true, out Type underlyingType, out bool hasFlags);

		return new EnumValuesConstraint(ParameterConvert.NetToParameterDataType(underlyingType), hasFlags, enumValues);
	}

	/// <summary>
	/// Gets a dictionary of enumeration names and values from the specified enumeration type.
	/// </summary>
	/// <param name="type">The type to get enumeration values. of.</param>
	/// <returns>A dictionary keyed by the enumeration value names, with the actual enumeration values as values.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
	/// <exception cref="CodedArgumentException"><paramref name="type"/> is not an enumeration.</exception>
	public static IDictionary<string, object> GetEnumValuesDictionary(Type type) => ParameterConvert.ExamineEnumeration(type, true, out Type underlyingType, out bool hasFlags);

	/// <summary>
	/// Sets the <see cref="SerializationInfo"/> with information about the <see cref="Constraint"/>.
	/// </summary>
	/// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data of the <see cref="Constraint"/>.</param>
	/// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
#if NETSTD20
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
#else
	public override void GetObjectData([System.Diagnostics.CodeAnalysis.NotNull] SerializationInfo info, StreamingContext context)
#endif
	{
		base.GetObjectData(info, context);
		info.AddValue(nameof(UnderlyingDataType), UnderlyingDataType);
		info.AddValue(nameof(HasFlags), HasFlags);
		info.AddValue(CountName, _enumValues?.Count ?? 0);
		if (_enumValues is not null)
		{
			int i = 0;
			foreach (KeyValuePair<string, object> pair in _enumValues)
			{
				info.AddValue(NamePrefix + i.ToString(CultureInfo.InvariantCulture), pair.Key);
				info.AddValue(ValuePrefix + i.ToString(CultureInfo.InvariantCulture), pair.Value);
				i++;
			}
		}
	}

	/// <summary>
	/// Adds the parameters of the constraint to a list of strings.
	/// </summary>
	/// <param name="parameters">A list of strings to add the parameters to.</param>
	/// <remarks>Override this method, if the constraint makes use of parameters. Add the parameters in the order that they should be provided to <see cref="SetParameters"/>.</remarks>
#if NETSTD20
	protected override void GetParameters(IList<string> parameters)
#else
	protected override void GetParameters([System.Diagnostics.CodeAnalysis.NotNull] IList<string> parameters)
#endif
	{
		base.GetParameters(parameters);
		parameters.Add(Enum.GetName(typeof(ParameterDataType), UnderlyingDataType) ?? throw new ParameterConversionException(HResult.Create(ErrorCodes.EnumValuesConstraint_GetParameters_DataType), TextResources.EnumValuesConstraint_GetParameters_DataType));
		if (HasFlags)
			parameters.Add(FlagsParameter);
		if (_enumValues is not null)
		{
			foreach (KeyValuePair<string, object> pair in _enumValues)
			{
				parameters.Add(string.Format(CultureInfo.InvariantCulture, "{0}={1}", pair.Key, ParameterConvert.ToString(pair.Value, UnderlyingDataType, null)));
			}
		}
	}

	/// <summary>
	/// Sets the parameters that the <see cref="Constraint"/> requires to work.
	/// </summary>
	/// <param name="parameters">A enumeration of string parameters.</param>
	/// <param name="dataType">The data type that the constraint needs to restrict.</param>
	/// <exception cref="CodedArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
	/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
	/// <exception cref="ConstraintConfigurationException"><paramref name="parameters"/> contains no elements, or an invalid element.</exception>
#if NETSTD20
	protected override void SetParameters(IReadOnlyList<string> parameters, ParameterDataType dataType)
#else
	protected override void SetParameters([System.Diagnostics.CodeAnalysis.NotNull] IReadOnlyList<string> parameters, ParameterDataType dataType)
#endif
	{
		base.SetParameters(parameters, dataType);
		AssertDataType(dataType, ParameterDataType.Enum);
		ResetFields();
		if (parameters.Count < 2)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.EnumValuesConstraint_SetParameters_ParamCountInvalid), string.Format(CultureInfo.CurrentCulture, TextResources.Global_SetParameters_InvalidMinCount, Name, 2), this);
		}

		if (!Enum.TryParse<ParameterDataType>(parameters[0], out ParameterDataType underlyingDataType))
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.EnumValuesConstraint_SetParameters_InvalidDataType), TextResources.EnumValuesConstraint_SetParameters_NotDataType, this);
		}
		UnderlyingDataType = underlyingDataType;
		UnderlyingType = ParameterConvert.ParameterToNetDataType(UnderlyingDataType);

		int enumStart = 1;
		if (parameters[1] == FlagsParameter)
		{
			HasFlags = true;
			enumStart = 2;
		}

		if (parameters.Count <= enumStart)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.EnumValuesConstraint_SetParameters_NoValues), string.Format(CultureInfo.CurrentCulture, TextResources.EnumValuesConstraint_SetParameters_NoValues, Name), this);
		}

		string enumName, enumValue;
		_enumValues = new Dictionary<string, object>();
		for (int i = enumStart; i < parameters.Count; i++)
		{
			string[] tokens = parameters[i].Trim().Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
			if (tokens.Length != 2)
			{
				throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.EnumValuesConstraint_SetParameters_ValuePairInvalid), string.Format(CultureInfo.CurrentCulture, TextResources.EnumValuesConstraint_SetParameters_InvalidValue, parameters[i]), this);
			}
			enumName = tokens[0].Trim();
			enumValue = tokens[1].Trim();
			if (enumValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
			{
#if NETSTD20
				if (!ulong.TryParse(enumValue.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out ulong hexValue))
#else
				if (!ulong.TryParse(enumValue.AsSpan(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out ulong hexValue))
#endif
				{
					throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.EnumValuesConstraint_SetParameters_KeyInvalid), string.Format(CultureInfo.CurrentCulture, TextResources.EnumValuesConstraint_SetParameters_InvalidHex, enumValue), this);
				}
				try
				{
					_enumValues.Add(enumName, Convert.ChangeType(hexValue, UnderlyingType, CultureInfo.InvariantCulture));
				}
				catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
				{
					throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.EnumValuesConstraint_SetParameters_DataTypeMismatch), string.Format(CultureInfo.CurrentCulture, TextResources.EnumValuesConstraint_InvalidValueType, enumValue, UnderlyingType), this, ex);
				}
			}
			else
			{
				try
				{
					_enumValues.Add(enumName, Convert.ChangeType(enumValue, UnderlyingType, CultureInfo.InvariantCulture));
				}
				catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
				{
					throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.EnumValuesConstraint_SetParameters_DataTypeMismatch), string.Format(CultureInfo.CurrentCulture, TextResources.EnumValuesConstraint_InvalidValueType, enumValue, UnderlyingType), this, ex);
				}
			}
		}

		CreateMask();
	}

	/// <summary>
	/// When implemented by a deriving class, checks that the provided value is within the bounds of the constraint.
	/// </summary>
	/// <param name="results">A list that of <see cref="ParameterValidationResult"/>s; when the method returns, it contains the validation errors generated by the method.</param>
	/// <param name="value">The value to check.</param>
	/// <param name="dataType">The data type of the value.</param>
	/// <param name="memberName">The name of the property or field that is validated.</param>
	/// <param name="displayName">The (localized) display name of the property or field that is validated. May be <see langword="null"/>.</param>
#if NETSTD20
	protected override void OnValidation(IList<ParameterValidationResult> results, object value, ParameterDataType dataType, string memberName, string displayName)
#else
	protected override void OnValidation([System.Diagnostics.CodeAnalysis.NotNull] IList<ParameterValidationResult> results, [System.Diagnostics.CodeAnalysis.NotNull] object value, ParameterDataType dataType, [System.Diagnostics.CodeAnalysis.NotNull] string memberName, string? displayName)
#endif
	{
		base.OnValidation(results, value, dataType, memberName, displayName);
		AssertDataType(dataType, ParameterDataType.Enum);
		if (UnderlyingType == null)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.EnumValuesConstraint_Validate_NotSetUp), TextResources.EnumValuesConstraint_Validate_NotConfigured, this);
		}

		Type valType = value.GetType();
		TypeInfo valTypeInfo = valType.GetTypeInfo();
		if (valTypeInfo.IsEnum)
		{
			valType = ParameterConvert.GetEnumUnderlyingType(valType) ?? throw new CodedArgumentException(HResult.Create(ErrorCodes.EnumValuesConstraint_Validate_ValueType), TextResources.EnumValuesConstraint_Validate_ValueType, nameof(value));
			value = Convert.ChangeType(value, valType, CultureInfo.InvariantCulture);
		}

		if (ParameterConvert.IsIntegerType(valType))
		{
			if (HasFlags)
			{
				long value2 = Convert.ToInt64(value, CultureInfo.InvariantCulture);
				if (((value2 ^ _flagMask) & value2) != 0)
				{
					results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.EnumValuesConstraint_Validate_InvalidFlag), string.Format(CultureInfo.CurrentCulture, TextResources.EnumConstraint_Validate_InvalidFlag, displayName), memberName, this));
				}
			}
			else
			{
				if (valType != UnderlyingType)
				{
					try
					{
						value = Convert.ChangeType(value, UnderlyingType, CultureInfo.InvariantCulture);
					}
					catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
					{
						throw new CodedArgumentException(HResult.Create(ErrorCodes.EnumTypeConstraint_Validate_InvalidFlag), string.Format(CultureInfo.CurrentCulture, TextResources.EnumValuesConstraint_InvalidValueType, value, UnderlyingType), ex);
					}

				}
				if (!_enumValues?.ContainsValue(value) ?? false)
				{
					results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.EnumValuesConstraint_Validate_NotInEnum), string.Format(CultureInfo.CurrentCulture, TextResources.EnumConstraint_Validate_NotDefined, displayName, value), memberName, this));
				}
			}
		}
		else
		{
			// Cannot happen in C# or most other languages, but some support non-integer enums.
			results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.EnumValuesConstraint_Validate_TypeNotSupported), string.Format(CultureInfo.CurrentCulture, TextResources.EnumConstraint_Validate_NotSupported, displayName, valType.Name), memberName, this));
		}
	}

	/// <summary>
	/// Resets all fields set by ResolveType.
	/// </summary>
	private void ResetFields()
	{
		UnderlyingType = null;
		HasFlags = false;
		_enumValues = null;
		_flagMask = 0;
		UnderlyingDataType = ParameterDataType.None;
	}

	/// <summary>
	/// If the enumeration has the Flags attribute, a mask containing all flags is stored in FlagMask.
	/// </summary>
	private void CreateMask()
	{
		if (HasFlags && _enumValues is not null)
		{
			foreach (object value in _enumValues.Values)
			{
				_flagMask |= Convert.ToInt64(value, CultureInfo.InvariantCulture);
			}
		}
	}
}
