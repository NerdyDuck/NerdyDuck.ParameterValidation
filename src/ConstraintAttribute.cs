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

using System.ComponentModel.DataAnnotations;

namespace NerdyDuck.ParameterValidation;

/// <summary>
/// A <see cref="ValidationAttribute"/> that uses <see cref="Constraint"/>s to validate a field or property.
/// </summary>
/// <remarks>A <see cref="ValidationAttribute"/> can only return one <see cref="ValidationResult"/>, so a <see cref="ConstraintAttribute"/> can only return the first validation error of its constraints.
/// If you want to show all validation errors, you can use multiple <see cref="ConstraintAttribute"/>s, each containing a single <see cref="Constraint"/> (and a <see cref="Constraints.NullConstraint"/>,
/// if the data allows <see langword="null"/> values.</remarks>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1813:Avoid unsealed attributes", Justification = "Intentional, to override parser and validator.")]
public class ConstraintAttribute : ValidationAttribute
{
	private IReadOnlyList<Constraint>? _parsedConstraints;
	private readonly object _lock;

	/// <summary>
	/// Gets a string containing one or more constraints.
	/// </summary>
	/// <value>A string with one more constraints, e.g. "[Null][MaxValue(5)]".</value>
	public string Constraints { get; private set; }

	/// <summary>
	/// Gets the data type of the value to validate.
	/// </summary>
	/// <value>One of the <see cref="ParameterDataType"/> values.</value>
	public ParameterDataType DataType { get; private set; }

	/// <summary>
	/// Gets a read-only list of <see cref="Constraint"/>s parsed from <see cref="Constraints"/>.
	/// </summary>
	/// <value>One or more <see cref="Constraint"/>s.</value>
	public IReadOnlyList<Constraint> ParsedConstraints
	{
		get
		{
			if (_parsedConstraints == null)
			{
				lock (_lock)
				{
					if (_parsedConstraints == null)
					{
						_parsedConstraints = Parser.Parse(Constraints, DataType);
					}
				}
			}
			return _parsedConstraints;
		}
	}

	/// <summary>
	/// Gets a value that indicates whether the attribute requires validation context.
	/// </summary>
	/// <value><see langword="true"/> if the attribute requires validation context; otherwise, <see langword="false"/>.</value>
	/// <remarks>This implementation always returns <see langword="true"/>, as the <see cref="ConstraintAttribute"/> requires a context.</remarks>
	public override bool RequiresValidationContext => true;

	/// <summary>
	/// Gets the <see cref="ConstraintParser"/> to parse the <see cref="Constraints"/>.
	/// </summary>
	/// <value>The default implementation returns the <see cref="ConstraintParser.Parser"/> singleton.</value>
	protected virtual ConstraintParser Parser => ConstraintParser.Parser;

	/// <summary>
	/// Gets the <see cref="ParameterValidator"/> used to validate a value.
	/// </summary>
	/// <value>The default implementation returns the <see cref="ConstraintParser.Parser"/> singleton.</value>
	protected virtual ParameterValidator Validator => ParameterValidator.Validator;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConstraintAttribute"/> class with the specified constraints and data type.
	/// </summary>
	/// <param name="constraints">A string containing one or more constraints.</param>
	/// <param name="dataType">The data type of the value to validate.</param>
	/// <exception cref="CodedArgumentNullOrWhiteSpaceException"><paramref name="constraints"/> is <see langword="null"/> or empty or white-space.</exception>
	/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
	public ConstraintAttribute(string constraints, ParameterDataType dataType)
		: base()
	{
		if (string.IsNullOrWhiteSpace(constraints))
		{
			throw new CodedArgumentNullOrWhiteSpaceException(HResult.Create(ErrorCodes.ConstraintAttribute_ctor_ConstraintsNullEmpty), nameof(constraints));
		}
		if (dataType == ParameterDataType.None)
		{
			throw new CodedArgumentOutOfRangeException(HResult.Create(ErrorCodes.ConstraintAttribute_ctor_DataTypeNone), nameof(dataType), TextResources.Global_ParameterDataType_None);
		}
		Constraints = constraints;
		DataType = dataType;
		_parsedConstraints = null;
		_lock = new();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ConstraintAttribute"/> class with the specified constraints, data type and the error message to associate with a validation control.
	/// </summary>
	/// <param name="constraints">A string containing one or more constraints.</param>
	/// <param name="dataType">The data type of the value to validate.</param>
	/// <param name="errorMessage">The error message to associate with a validation control.</param>
	/// <exception cref="CodedArgumentNullOrWhiteSpaceException"><paramref name="constraints"/> is <see langword="null"/> or empty or white-space.</exception>
	/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
	public ConstraintAttribute(string constraints, ParameterDataType dataType, string errorMessage)
		: base(errorMessage)
	{
		if (string.IsNullOrWhiteSpace(constraints))
		{
			throw new CodedArgumentNullOrWhiteSpaceException(HResult.Create(ErrorCodes.ConstraintAttribute_ctor_ConstraintsNullEmpty), nameof(constraints));
		}
		if (dataType == ParameterDataType.None)
		{
			throw new CodedArgumentOutOfRangeException(HResult.Create(ErrorCodes.ConstraintAttribute_ctor_DataTypeNone), nameof(dataType), TextResources.Global_ParameterDataType_None);
		}
		Constraints = constraints;
		DataType = dataType;
		_parsedConstraints = null;
		_lock = new();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ConstraintAttribute"/> class with the specified constraints, data type and the function that enables access to validation resources..
	/// </summary>
	/// <param name="constraints">A string containing one or more constraints.</param>
	/// <param name="dataType">The data type of the value to validate.</param>
	/// <param name="errorMessageAccessor">The function that enables access to validation resources.</param>
	/// <exception cref="CodedArgumentNullOrWhiteSpaceException"><paramref name="constraints"/> is <see langword="null"/> or empty or white-space.</exception>
	/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
#pragma warning disable CA1019 // Define accessors for attribute arguments: errorMessageAccessor is part of base class
	public ConstraintAttribute(string constraints, ParameterDataType dataType, Func<string> errorMessageAccessor)
#pragma warning restore CA1019 // Define accessors for attribute arguments
		: base(errorMessageAccessor)
	{
		if (string.IsNullOrWhiteSpace(constraints))
		{
			throw new CodedArgumentNullOrWhiteSpaceException(HResult.Create(ErrorCodes.ConstraintAttribute_ctor_ConstraintsNullEmpty), nameof(constraints));
		}
		if (dataType == ParameterDataType.None)
		{
			throw new CodedArgumentOutOfRangeException(HResult.Create(ErrorCodes.ConstraintAttribute_ctor_DataTypeNone), nameof(dataType), TextResources.Global_ParameterDataType_None);
		}
		Constraints = constraints;
		DataType = dataType;
		_parsedConstraints = null;
		_lock = new();
	}

	/// <summary>
	/// Validates the specified value with respect to the current validation attribute.
	/// </summary>
	/// <param name="value">The value to validate.</param>
	/// <param name="validationContext">The context information about the validation operation.</param>
	/// <returns>An instance of the ValidationResult class.</returns>
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		IEnumerable<ParameterValidationResult> results = Validator.GetValidationResult(value, DataType, ParsedConstraints, validationContext?.MemberName ?? "Value", validationContext?.DisplayName);
		foreach (ParameterValidationResult result in results)
		{
			// Can only return one ...
			return result;
		}
		return ValidationResult.Success;
	}
}
