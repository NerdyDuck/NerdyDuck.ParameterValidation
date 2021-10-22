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

namespace NerdyDuck.ParameterValidation.Constraints;

/// <summary>
/// A constraint specifying the minimum value for an integer, <see cref="byte"/>, <see cref="decimal"/>, <see cref="DateTimeOffset"/>, <see cref="TimeSpan"/> or <see cref="Version"/>.
/// </summary>
/// <remarks>
/// <para>
/// <list type="bullet">
/// <item>The textual representation of the constraint is <c>[MinValue(value)]</c>. <c>value</c> must be a string representation of the same data type as the data type that the <see cref="MinimumValueConstraint"/> is applied to.</item>
/// <item>The constraint is applicable to the <see cref="ParameterDataType.Byte"/>, <see cref="ParameterDataType.DateTimeOffset"/>, <see cref="ParameterDataType.Decimal"/>, <see cref="ParameterDataType.Int16"/>,
/// <see cref="ParameterDataType.Int32"/>, <see cref="ParameterDataType.Int64"/>, <see cref="ParameterDataType.SignedByte"/>, <see cref="ParameterDataType.TimeSpan"/>, <see cref="ParameterDataType.UInt16"/>,
/// <see cref="ParameterDataType.UInt32"/>, <see cref="ParameterDataType.UInt64"/> and <see cref="ParameterDataType.Version"/> data types.</item>
/// <item>If a parameter with that constraint has a value smaller than <see cref="MinimumValue"/>, a <see cref="ParameterValidationResult"/> is added to the output of <see cref="Constraint.Validate(object, ParameterDataType, string, string)"/>.</item>
/// </list>
/// </para>
/// </remarks>
[Serializable]
public class MinimumValueConstraint : Constraint
{
	private object _altMinimumValue;
	private Type? _expectedNetType;
	private Type? _altNetType;
	private TypeCode _expectedTypeCode;
	private TypeCode _altTypeCode;

	/// <summary>
	/// Gets the minimum value to enforce.
	/// </summary>
	/// <value>A value matching the type of <see cref="DataType"/>.</value>
	public object MinimumValue { get; private set; }

	/// <summary>
	/// Gets the data type that the constraint can validate.
	/// </summary>
	/// <value>One of the <see cref="ParameterDataType"/> values.</value>
	public ParameterDataType DataType { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="MinimumValueConstraint"/> class.
	/// </summary>
	/// <param name="dataType">The data type that the constraint can validate.</param>
	/// <exception cref="CodedArgumentException"><paramref name="dataType"/> is not supported by the constraint.</exception>
	public MinimumValueConstraint(ParameterDataType dataType)
		: base(MinimumValueConstraintName)
	{
		DataType = dataType;
		MinimumValue = CheckDataType();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MinimumValueConstraint"/> class with the specified minimum value.
	/// </summary>
	/// <param name="dataType">The data type that the constraint can validate.</param>
	/// <param name="minimumValue">The minimum value to enforce.</param>
	/// <exception cref="CodedArgumentNullException"><paramref name="minimumValue"/> is <see langword="null"/>.</exception>
	public MinimumValueConstraint(ParameterDataType dataType, object minimumValue)
		: base(MinimumValueConstraintName)
	{
		if (minimumValue == null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.MinimumValueConstraint_ctor_MinValueNull), nameof(minimumValue));
		}

		DataType = dataType;
		MinimumValue = CheckDataType();
		MinimumValue = CheckValueType(minimumValue);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MinimumValueConstraint"/> class with serialized data.
	/// </summary>
	/// <param name="info">The object that holds the serialized object data.</param>
	/// <param name="context">The contextual information about the source or destination.</param>
	/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
	/// <exception cref="SerializationException">The constraint could not be deserialized correctly.</exception>
	protected MinimumValueConstraint(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		DataType = (ParameterDataType)info.GetValue(nameof(DataType), typeof(ParameterDataType));
		MinimumValue = CheckDataType();
		MinimumValue = CheckValueType(info.GetValue(nameof(MinimumValue), _expectedNetType));
	}

	/// <summary>
	/// Sets the <see cref="SerializationInfo"/> with information about the <see cref="Constraint"/>.
	/// </summary>
	/// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data of the <see cref="Constraint"/>.</param>
	/// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue(nameof(DataType), DataType);
		info.AddValue(nameof(MinimumValue), MinimumValue);
	}

	/// <summary>
	/// Adds the parameters of the constraint to a list of strings.
	/// </summary>
	/// <param name="parameters">A list of strings to add the parameters to.</param>
	/// <remarks>Override this method, if the constraint makes use of parameters. Add the parameters in the order that they should be provided to <see cref="SetParameters"/>.</remarks>
	protected override void GetParameters(IList<string> parameters)
	{
		base.GetParameters(parameters);
		parameters.Add(ParameterConvert.ToString(MinimumValue, DataType, null));
	}

	/// <summary>
	/// Sets the parameters that the <see cref="Constraint"/> requires to work.
	/// </summary>
	/// <param name="parameters">A enumeration of string parameters.</param>
	/// <param name="dataType">The data type that the constraint needs to restrict.</param>
	/// <exception cref="CodedArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
	/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
	/// <exception cref="ConstraintConfigurationException"><paramref name="parameters"/> contains no elements, or an invalid element.</exception>
	protected override void SetParameters(IReadOnlyList<string> parameters, ParameterDataType dataType)
	{
		base.SetParameters(parameters, dataType);
		if (parameters.Count != 1)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.MinimumValueConstraint_SetParameters_OnlyOneParam), string.Format(CultureInfo.CurrentCulture, TextResources.Global_SetParameters_InvalidCount, Name, 1), this);
		}

		try
		{
			MinimumValue = CheckValueType(ParameterConvert.ToDataType(parameters[0], dataType, null));
		}
		catch (ParameterConversionException ex)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.MinimumValueConstraint_SetParameters_ParamInvalid), string.Format(CultureInfo.CurrentCulture, TextResources.Global_SetParameters_Invalid, Name), this, ex);
		}
	}

	/// <summary>
	/// When implemented by a deriving class, checks that the provided value is within the bounds of the constraint.
	/// </summary>
	/// <param name="results">A list that of <see cref="ParameterValidationResult"/>s; when the method returns, it contains the validation errors generated by the method.</param>
	/// <param name="value">The value to check.</param>
	/// <param name="dataType">The data type of the value.</param>
	/// <param name="memberName">The name of the property or field that is validated.</param>
	/// <param name="displayName">The (localized) display name of the property or field that is validated. May be <see langword="null"/>.</param>
	protected override void OnValidation(IList<ParameterValidationResult> results, object value, ParameterDataType dataType, string memberName, string displayName)
	{
		base.OnValidation(results, value, dataType, memberName, displayName);
		AssertDataType(dataType, DataType);
		Type type = value.GetType();

		int compareResult;
		if (type == _expectedNetType)
		{
			// Type matches
			compareResult = ((IComparable)MinimumValue).CompareTo(value);
		}
		else
		{
			if (IsConvertibleType(DataType))
			{
				if (type == _altNetType)
				{
					// Type is already equal to alternative data type, no conversion necessary.
					compareResult = ((IComparable)_altMinimumValue).CompareTo(value);
				}
				else
				{
					object? convertedValue = null;
					// Try to convert and compare to actual data type
					try
					{
						convertedValue = Convert.ChangeType(value, _expectedTypeCode, CultureInfo.InvariantCulture);
					}
					catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
					{
					}

					if (convertedValue == null)
					{
						// Conversion to actual data type failed, try to convert and compare to alternative data type.
						try
						{
							convertedValue = Convert.ChangeType(value, _altTypeCode, CultureInfo.InvariantCulture);
						}
						catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
						{
							throw new CodedArgumentException(HResult.Create(ErrorCodes.MinimumValueConstraint_Validate_TypeMismatch), string.Format(CultureInfo.CurrentCulture, TextResources.Global_Validate_TypeNotConvertible, type.Name, _expectedNetType.Name));
						}
						compareResult = ((IComparable)_altMinimumValue).CompareTo(convertedValue);
					}
					else
					{
						// Converting to actual data type successful
						compareResult = ((IComparable)MinimumValue).CompareTo(convertedValue);
					}
				}
			}
			else
			{
				throw new CodedArgumentException(HResult.Create(ErrorCodes.MinimumValueConstraint_Validate_TypeMismatch), string.Format(CultureInfo.CurrentCulture, TextResources.Global_Validate_TypeNotConvertible, type.Name, _expectedNetType.Name));
			}

		}

		if (compareResult > 0)
		{
			results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.MinimumValueConstraint_Validate_TooSmall), string.Format(CultureInfo.CurrentCulture, TextResources.MinimumValueConstraint_Validate_Failed, displayName, MinimumValue, value), memberName, this));
		}
	}

	/// <summary>
	/// Checks if the data type can be converted into another data type before comparison.
	/// </summary>
	/// <param name="dataType">The data type to check.</param>
	/// <returns><see langword="false"/>, if <paramref name="dataType"/> <see cref="ParameterDataType.DateTimeOffset"/>, <see cref="ParameterDataType.TimeSpan"/> or <see cref="ParameterDataType.Version"/>; otherwise, <see langword="true"/>.</returns>
	private static bool IsConvertibleType(ParameterDataType dataType) => dataType is not (ParameterDataType.DateTimeOffset or ParameterDataType.TimeSpan or ParameterDataType.Version);

	/// <summary>
	/// Checks that the specified data type is supported by the <see cref="MinimumValueConstraint"/>, and sets the appropriate expected and alternative data types.
	/// </summary>
	/// <returns>The maximum value supported by the data type.</returns>
	private object CheckDataType()
	{
		switch (DataType)
		{
			case ParameterDataType.Byte:
				_expectedNetType = typeof(byte);
				_altNetType = typeof(ulong);
				_expectedTypeCode = TypeCode.Byte;
				_altTypeCode = TypeCode.UInt64;
				return byte.MaxValue;
			case ParameterDataType.DateTimeOffset:
				_expectedNetType = typeof(DateTimeOffset);
				_altNetType = null;
				_expectedTypeCode = TypeCode.Empty;
				_altTypeCode = TypeCode.Empty;
				return DateTimeOffset.MaxValue;
			case ParameterDataType.Decimal:
				_expectedNetType = typeof(decimal);
				_altNetType = typeof(decimal);
				_expectedTypeCode = TypeCode.Decimal;
				_altTypeCode = TypeCode.Decimal;
				return decimal.MaxValue;
			case ParameterDataType.Int16:
				_expectedNetType = typeof(short);
				_altNetType = typeof(long);
				_expectedTypeCode = TypeCode.Int16;
				_altTypeCode = TypeCode.Int64;
				return short.MaxValue;
			case ParameterDataType.Int32:
				_expectedNetType = typeof(int);
				_altNetType = typeof(long);
				_expectedTypeCode = TypeCode.Int32;
				_altTypeCode = TypeCode.Int64;
				return int.MaxValue;
			case ParameterDataType.Int64:
				_expectedNetType = typeof(long);
				_altNetType = typeof(long);
				_expectedTypeCode = TypeCode.Int64;
				_altTypeCode = TypeCode.Int64;
				return long.MaxValue;
			case ParameterDataType.SignedByte:
				_expectedNetType = typeof(sbyte);
				_altNetType = typeof(long);
				_expectedTypeCode = TypeCode.SByte;
				_altTypeCode = TypeCode.Int64;
				return sbyte.MaxValue;
			case ParameterDataType.TimeSpan:
				_expectedNetType = typeof(TimeSpan);
				_altNetType = null;
				_expectedTypeCode = TypeCode.Empty;
				_altTypeCode = TypeCode.Empty;
				return TimeSpan.MaxValue;
			case ParameterDataType.UInt16:
				_expectedNetType = typeof(ushort);
				_altNetType = typeof(ulong);
				_expectedTypeCode = TypeCode.UInt16;
				_altTypeCode = TypeCode.UInt64;
				return ushort.MaxValue;
			case ParameterDataType.UInt32:
				_expectedNetType = typeof(uint);
				_altNetType = typeof(ulong);
				_expectedTypeCode = TypeCode.UInt32;
				_altTypeCode = TypeCode.UInt64;
				return uint.MaxValue;
			case ParameterDataType.UInt64:
				_expectedNetType = typeof(ulong);
				_altNetType = typeof(ulong);
				_expectedTypeCode = TypeCode.UInt64;
				_altTypeCode = TypeCode.UInt64;
				return ulong.MaxValue;
			case ParameterDataType.Version:
				_expectedNetType = typeof(Version);
				_altNetType = null;
				_expectedTypeCode = TypeCode.Empty;
				_altTypeCode = TypeCode.Empty;
				return new Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
			default:
				throw new CodedArgumentException(HResult.Create(ErrorCodes.MinimumValueConstraint_CheckDataType_TypeNotSupported), string.Format(CultureInfo.CurrentCulture, TextResources.Global_CheckDataType_NotSupported, Name), "dataType");
		}
	}

	/// <summary>
	/// Checks if the value is of the expected type, or can be converted into the type.
	/// </summary>
	/// <param name="value">The value to check.</param>
	/// <returns>The value, either in its original form, or converted.</returns>
	private object CheckValueType(object value)
	{
		Type type = value.GetType();
		object checkedValue;

		if (type == _expectedNetType)
		{
			checkedValue = value;
		}
		else
		{
			if (_expectedTypeCode == TypeCode.Empty)
			{
				// DateTimeOffset, TimeSpan, Version cannot be converted from another type
				throw new ParameterConversionException(HResult.Create(ErrorCodes.MinimumValueConstraint_CheckValueType_TypeMismatch), string.Format(CultureInfo.CurrentCulture, TextResources.Global_CheckValueType_NotConvertible, type.Name, _expectedNetType.Name), DataType, value);
			}
			else
			{
				try
				{
					checkedValue = Convert.ChangeType(value, _expectedTypeCode, System.Globalization.CultureInfo.InvariantCulture);
				}
				catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
				{
					throw new ParameterConversionException(HResult.Create(ErrorCodes.MinimumValueConstraint_CheckValueType_TypeMismatch), string.Format(CultureInfo.CurrentCulture, TextResources.Global_CheckValueType_NotConvertible, type.Name, _expectedNetType.Name), DataType, value, ex);
				}
			}
		}

		if (_altNetType != null)
		{
			_altMinimumValue = type == _altNetType ? value : Convert.ChangeType(value, _altTypeCode, CultureInfo.InvariantCulture);
		}
		return checkedValue;
	}
}
