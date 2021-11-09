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
/// A constraint specifying the maximum length of a <see cref="string"/>, <see cref="byte"/> array or <see cref="Uri"/>.
/// </summary>
/// <remarks>
/// <para>
/// <list type="bullet">
/// <item>The textual representation of the constraint is <c>[MaxLength(value)]</c>. <c>value</c> must be a string representation of an integer. The minimum value is 0.</item>
/// <item>The constraint is applicable to the <see cref="ParameterDataType.Bytes"/> and <see cref="ParameterDataType.String"/> data types.</item>
/// <item>If a parameter with that constraint has a string or array length larger than <see cref="MaximumLength"/>, or a <see cref="Uri"/> with a length larger than <see cref="MaximumLength"/>, a <see cref="ParameterValidationResult"/> is added to the output of <see cref="Constraint.Validate(object, ParameterDataType, string, string)"/>.</item>
/// </list>
/// </para>
/// </remarks>
[Serializable]
public class MaximumLengthConstraint : Constraint
{
	/// <summary>
	/// Gets the maximum length to enforce.
	/// </summary>
	/// <value>A non-negative integer value.</value>
	public int MaximumLength { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="MaximumLengthConstraint"/> class.
	/// </summary>
	public MaximumLengthConstraint()
		: base(MaximumLengthConstraintName) => MaximumLength = int.MaxValue;

	/// <summary>
	/// Initializes a new instance of the <see cref="MaximumLengthConstraint"/> class with the specified maximum length.
	/// </summary>
	/// <param name="maximumLength">The maximum length to enforce.</param>
	/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="maximumLength"/> is less than 0.</exception>
	public MaximumLengthConstraint(int maximumLength)
		: base(MaximumLengthConstraintName)
	{
		if (maximumLength < 0)
		{
			throw new CodedArgumentOutOfRangeException(HResult.Create(ErrorCodes.MaximumLengthConstraint_ctor_LengthNegative), nameof(maximumLength), maximumLength, TextResources.MaximumLengthConstraint_LengthNegative);
		}

		MaximumLength = maximumLength;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MaximumLengthConstraint"/> class with serialized data.
	/// </summary>
	/// <param name="info">The object that holds the serialized object data.</param>
	/// <param name="context">The contextual information about the source or destination.</param>
	/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
	/// <exception cref="SerializationException">The constraint could not be deserialized correctly.</exception>
	protected MaximumLengthConstraint(SerializationInfo info, StreamingContext context)
		: base(info, context) => MaximumLength = info.GetInt32(nameof(MaximumLength));

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
		info.AddValue(nameof(MaximumLength), MaximumLength);
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
		parameters.Add(ParameterConvert.ToString(MaximumLength));
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
		AssertDataType(dataType, ParameterDataType.Bytes, ParameterDataType.String, ParameterDataType.Uri);
		if (parameters.Count != 1)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.MaximumLengthConstraint_SetParameters_OnlyOneParam), string.Format(CultureInfo.CurrentCulture, TextResources.Global_SetParameters_InvalidCount, Name, 1), this);
		}

		try
		{
			MaximumLength = ParameterConvert.ToInt32(parameters[0]);
			if (MaximumLength < 0)
			{
				throw new ParameterConversionException(HResult.Create(ErrorCodes.MaximumLengthConstraint_SetParameters_LengthNegative), TextResources.MaximumLengthConstraint_LengthNegative, ParameterDataType.Int32, MaximumLength);
			}
		}
		catch (ParameterConversionException ex)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.MaximumLengthConstraint_SetParameters_ParamInvalid), string.Format(CultureInfo.CurrentCulture, TextResources.Global_SetParameters_Invalid, Name), this, ex);
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
#if NETSTD20
	protected override void OnValidation(IList<ParameterValidationResult> results, object value, ParameterDataType dataType, string memberName, string displayName)
#else
	protected override void OnValidation([System.Diagnostics.CodeAnalysis.NotNull] IList<ParameterValidationResult> results, [System.Diagnostics.CodeAnalysis.NotNull] object value, ParameterDataType dataType, [System.Diagnostics.CodeAnalysis.NotNull] string memberName, string? displayName)
#endif
	{
		base.OnValidation(results, value, dataType, memberName, displayName);
		AssertDataType(dataType, ParameterDataType.Bytes, ParameterDataType.String, ParameterDataType.Uri);
		bool isString = false;
		Type type = value.GetType();

		int currentLength;
		if (type == typeof(byte[]))
		{
			currentLength = ((byte[])value).Length;
		}
		else if (type == typeof(string))
		{
			isString = true;
			currentLength = ((string)value).Length;
		}
		else if (type == typeof(Uri))
		{
			isString = true;
			currentLength = ((Uri)value).ToString().Length;
		}
		else
		{
			throw new CodedArgumentException(HResult.Create(ErrorCodes.MaximumLengthConstraint_Validate_TypeNotSupported), string.Format(CultureInfo.CurrentCulture, TextResources.Global_Validate_TypeMismatch, type.Name, Name));
		}

		if (currentLength > MaximumLength)
		{
			results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.MaximumLengthConstraint_Validate_TooLong), string.Format(CultureInfo.CurrentCulture,
				isString ? TextResources.MaximumLengthConstraint_Validate_FailedString
				: TextResources.MaximumLengthConstraint_Validate_FailedBytes,
				displayName, MaximumLength, currentLength), memberName, this));
		}
	}
}
