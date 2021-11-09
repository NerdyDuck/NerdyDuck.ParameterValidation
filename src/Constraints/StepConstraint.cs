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
/// Specifies the increment and decrement step to use when displaying an integer or <see cref="decimal"/> value in a control e.g. a NumericUpDown control.
/// </summary>
/// <remarks>
/// <para>
/// <list type="bullet">
/// <item>The textual representation of the constraint is <c>[Step(value)]</c>. <c>value</c> must be a string representation of the same data type as the data type that the <see cref="StepConstraint"/> is applied to.</item>
/// <item>The constraint is applicable to the <see cref="ParameterDataType.Byte"/>, <see cref="ParameterDataType.Decimal"/>, <see cref="ParameterDataType.Int16"/>,
/// <see cref="ParameterDataType.Int32"/>, <see cref="ParameterDataType.Int64"/>, <see cref="ParameterDataType.SignedByte"/>, <see cref="ParameterDataType.UInt16"/>,
/// <see cref="ParameterDataType.UInt32"/> and <see cref="ParameterDataType.UInt64"/> data types.</item>
/// <item>The constraint is not used during validation or serialization, but is solely thought as a hint for display purposes.</item>
/// </list>
/// </para>
/// </remarks>
[Serializable]
public class StepConstraint : Constraint
{
	private Type _expectedNetType;
	private TypeCode _expectedTypeCode;

	/// <summary>
	/// Gets or sets the increment step.
	/// </summary>
	/// <value>A non-negative value matching the type of <see cref="DataType"/>.</value>
	public object StepSize { get; private set; }

	/// <summary>
	/// Gets the data type that the constraint can validate.
	/// </summary>
	/// <value>One of the <see cref="ParameterDataType"/> values.</value>
	public ParameterDataType DataType { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="StepConstraint"/> class.
	/// </summary>
	/// <param name="dataType">The data type that the constraint can validate.</param>
	/// <exception cref="CodedArgumentException"><paramref name="dataType"/> is not supported by the constraint.</exception>
#pragma warning disable IDE0079
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public StepConstraint(ParameterDataType dataType)
#pragma warning restore CS8618, IDE0079 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		: base(StepConstraintName)
	{
		DataType = dataType;
		StepSize = CheckType(dataType);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="StepConstraint"/> class with the specified maximum value.
	/// </summary>
	/// <param name="dataType">The data type that the constraint can validate.</param>
	/// <param name="stepSize">The increment step.</param>
	/// <exception cref="CodedArgumentNullException"><paramref name="stepSize"/> is <see langword="null"/>.</exception>
#pragma warning disable IDE0079
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public StepConstraint(ParameterDataType dataType, object stepSize)
#pragma warning restore CS8618, IDE0079 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		: base(StepConstraintName)
	{
		if (stepSize == null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.StepConstraint_ctor_StepSizeNull), nameof(stepSize));
		}

		DataType = dataType;
		StepSize = CheckType(dataType);
		StepSize = CheckValue(stepSize);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="StepConstraint"/> class with serialized data.
	/// </summary>
	/// <param name="info">The object that holds the serialized object data.</param>
	/// <param name="context">The contextual information about the source or destination.</param>
	/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
	/// <exception cref="SerializationException">The constraint could not be deserialized correctly.</exception>
#pragma warning disable IDE0079
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	protected StepConstraint(SerializationInfo info, StreamingContext context)
#pragma warning restore CS8618, IDE0079 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		: base(info, context)
	{
		DataType = (ParameterDataType)(info.GetValue(nameof(DataType), typeof(ParameterDataType)) ?? throw new CodedSerializationException(HResult.Create(ErrorCodes.StepConstraint_ctor_DataType), TextResources.StepConstraint_ctor_DataType));
		StepSize = CheckType(DataType);
		StepSize = CheckValue(info.GetValue(nameof(StepSize), _expectedNetType) ?? throw new CodedSerializationException(HResult.Create(ErrorCodes.StepConstraint_ctor_StepSize), TextResources.StepConstraint_ctor_StepSize));
	}

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
		info.AddValue(nameof(DataType), DataType);
		info.AddValue(nameof(StepSize), StepSize);
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
#pragma warning disable IDE0079
#pragma warning disable CS8604 // Possible null reference argument.
		parameters.Add(ParameterConvert.ToString(StepSize, DataType, null));
#pragma warning restore CS8604, IDE0079 // Possible null reference argument.
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
		if (parameters.Count != 1)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.StepConstraint_SetParameters_OnlyOneParam), string.Format(CultureInfo.CurrentCulture, TextResources.Global_SetParameters_InvalidCount, Name, 1), this);
		}

		try
		{
			StepSize = CheckValue(ParameterConvert.ToDataType(parameters[0], dataType, null)!);
		}
		catch (ParameterConversionException ex)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.StepConstraint_SetParameters_ParamInvalid), string.Format(CultureInfo.CurrentCulture, TextResources.Global_SetParameters_Invalid, Name), this, ex);
		}
	}

	/// <summary>
	/// Checks that the specified data type is supported by the <see cref="StepConstraint"/>, and sets the appropriate expected and alternative data types.
	/// </summary>
	/// <returns>The default value supported by the data type.</returns>
#if NET50
	[System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(_expectedNetType))]
	[System.Diagnostics.CodeAnalysis.MemberNotNull(nameof(_expectedTypeCode))]
	[return: System.Diagnostics.CodeAnalysis.NotNull]
#endif
	internal object CheckType(ParameterDataType dataType)
	{
		switch (dataType)
		{
			case ParameterDataType.Byte:
				_expectedNetType = typeof(byte);
				_expectedTypeCode = TypeCode.Byte;
				return (byte)1;
			case ParameterDataType.Decimal:
				_expectedNetType = typeof(decimal);
				_expectedTypeCode = TypeCode.Decimal;
				return 1m;
			case ParameterDataType.Int16:
				_expectedNetType = typeof(short);
				_expectedTypeCode = TypeCode.Int16;
				return (short)1;
			case ParameterDataType.Int32:
				_expectedNetType = typeof(int);
				_expectedTypeCode = TypeCode.Int32;
				return 1;
			case ParameterDataType.Int64:
				_expectedNetType = typeof(long);
				_expectedTypeCode = TypeCode.Int64;
				return 1L;
			case ParameterDataType.SignedByte:
				_expectedNetType = typeof(sbyte);
				_expectedTypeCode = TypeCode.SByte;
				return (sbyte)1;
			case ParameterDataType.UInt16:
				_expectedNetType = typeof(ushort);
				_expectedTypeCode = TypeCode.UInt16;
				return (ushort)1;
			case ParameterDataType.UInt32:
				_expectedNetType = typeof(uint);
				_expectedTypeCode = TypeCode.UInt32;
				return 1u;
			case ParameterDataType.UInt64:
				_expectedNetType = typeof(ulong);
				_expectedTypeCode = TypeCode.UInt64;
				return (ulong)1;
			default:
				throw new CodedArgumentException(HResult.Create(ErrorCodes.StepConstraint_CheckDataType_TypeNotSupported), string.Format(CultureInfo.CurrentCulture, TextResources.Global_CheckDataType_NotSupported, Name), nameof(dataType));
		}
	}

#if NET50
	[return: System.Diagnostics.CodeAnalysis.NotNull]
#endif
	private object CheckValue(object value)
	{
		Type type = value.GetType();
		object checkedValue;

		if (type == _expectedNetType)
		{
			checkedValue = value;
		}
		else if (_expectedTypeCode != TypeCode.Empty)
		{
			try
			{
				checkedValue = Convert.ChangeType(value, _expectedTypeCode, CultureInfo.InvariantCulture);
			}
			catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
			{
				throw new ParameterConversionException(HResult.Create(ErrorCodes.StepConstraint_CheckValueType_TypeMismatch), string.Format(CultureInfo.CurrentCulture, TextResources.Global_CheckValueType_NotConvertible, type.Name, _expectedNetType.Name), DataType, value, ex);
			}
		}
		else
		{
			// DateTimeOffset, TimeSpan, Version cannot be converted from another type
			throw new ParameterConversionException(HResult.Create(ErrorCodes.StepConstraint_CheckValueType_TypeMismatch), string.Format(CultureInfo.CurrentCulture, TextResources.Global_CheckValueType_NotConvertible, type.Name, _expectedNetType.Name), DataType, value);
		}

		return checkedValue;
	}
}
