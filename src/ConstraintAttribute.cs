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

using NerdyDuck.CodedExceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NerdyDuck.ParameterValidation
{
	/// <summary>
	/// A <see cref="ValidationAttribute"/> that uses <see cref="Constraint"/>s to validate a field or property.
	/// </summary>
	/// <remarks>A <see cref="ValidationAttribute"/> can only return one <see cref="ValidationResult"/>, so a <see cref="ConstraintAttribute"/> can only return the first validation error of its constraints.
	/// If you want to show all validation errors, you can use multiple <see cref="ConstraintAttribute"/>s, each containing a single <see cref="Constraint"/> (and a <see cref="Constraints.NullConstraint"/>,
	/// if the data allows <see langword="null"/> values.</remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "Same behavior as base class")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "To support modification of validation and parsing")]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
	public class ConstraintAttribute : ValidationAttribute
	{
		#region Private fields
		private string mConstraints;
		private ParameterDataType mDataType;
		private IReadOnlyList<Constraint> mParsedConstraints;
		#endregion

		#region Properties
		/// <summary>
		/// Gets a string containing one or more constraints.
		/// </summary>
		/// <value>A string with one more constraints, e.g. "[Null][MaxValue(5)]".</value>
		public string Constraints
		{
			get { return mConstraints; }
		}

		/// <summary>
		/// Gets the data type of the value to validate.
		/// </summary>
		/// <value>One of the <see cref="ParameterDataType"/> values.</value>
		public ParameterDataType DataType
		{
			get { return mDataType; }
		}

		/// <summary>
		/// Gets a read-only list of <see cref="Constraint"/>s parsed from <see cref="Constraints"/>.
		/// </summary>
		/// <value>One or more <see cref="Constraint"/>s.</value>
		public IReadOnlyList<Constraint> ParsedConstraints
		{
			get
			{
				lock (this)
				{
					if (mParsedConstraints == null)
					{
						mParsedConstraints = Parser.Parse(mConstraints, mDataType);
					}
				}
				return mParsedConstraints;
			}
		}

		/// <summary>
		/// Gets a value that indicates whether the attribute requires validation context.
		/// </summary>
		/// <value><see langword="true"/> if the attribute requires validation context; otherwise, <see langword="false"/>.</value>
		/// <remarks>This implementation always returns <see langword="true"/>, as the <see cref="ConstraintAttribute"/> requires a context.</remarks>
		public override bool RequiresValidationContext
		{
			get { return true; }
		}
		#endregion

		#region Protected properties
		/// <summary>
		/// Gets the <see cref="ConstraintParser"/> to parse the <see cref="Constraints"/>.
		/// </summary>
		/// <value>The default implementation returns the <see cref="ConstraintParser.Parser"/> singleton.</value>
		protected virtual ConstraintParser Parser
		{
			get { return ConstraintParser.Parser; }
		}

		/// <summary>
		/// Gets the <see cref="ParameterValidator"/> used to validate a value.
		/// </summary>
		/// <value>The default implementation returns the <see cref="ConstraintParser.Parser"/> singleton.</value>
		protected virtual ParameterValidator Validator
		{
			get { return ParameterValidator.Validator; }
		}
		#endregion

		#region Constructors
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
				throw new CodedArgumentNullOrWhiteSpaceException(Errors.CreateHResult(ErrorCodes.ConstraintAttribute_ctor_ConstraintsNullEmpty), nameof(constraints));
			}
			if (dataType == ParameterDataType.None)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(ErrorCodes.ConstraintAttribute_ctor_DataTypeNone), nameof(dataType), Properties.Resources.Global_ParameterDataType_None);
			}
			mConstraints = constraints;
			mDataType = dataType;
			mParsedConstraints = null;
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
				throw new CodedArgumentNullOrWhiteSpaceException(Errors.CreateHResult(ErrorCodes.ConstraintAttribute_ctor_ConstraintsNullEmpty), nameof(constraints));
			}
			if (dataType == ParameterDataType.None)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(ErrorCodes.ConstraintAttribute_ctor_DataTypeNone), nameof(dataType), Properties.Resources.Global_ParameterDataType_None);
			}
			mConstraints = constraints;
			mDataType = dataType;
			mParsedConstraints = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstraintAttribute"/> class with the specified constraints, data type and the function that enables access to validation resources..
		/// </summary>
		/// <param name="constraints">A string containing one or more constraints.</param>
		/// <param name="dataType">The data type of the value to validate.</param>
		/// <param name="errorMessageAccessor">The function that enables access to validation resources.</param>
		/// <exception cref="CodedArgumentNullOrWhiteSpaceException"><paramref name="constraints"/> is <see langword="null"/> or empty or white-space.</exception>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
		public ConstraintAttribute(string constraints, ParameterDataType dataType, Func<string> errorMessageAccessor)
			: base(errorMessageAccessor)
		{
			if (string.IsNullOrWhiteSpace(constraints))
			{
				throw new CodedArgumentNullOrWhiteSpaceException(Errors.CreateHResult(ErrorCodes.ConstraintAttribute_ctor_ConstraintsNullEmpty), nameof(constraints));
			}
			if (dataType == ParameterDataType.None)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(ErrorCodes.ConstraintAttribute_ctor_DataTypeNone), nameof(dataType), Properties.Resources.Global_ParameterDataType_None);
			}
			mConstraints = constraints;
			mDataType = dataType;
			mParsedConstraints = null;
		}
		#endregion

		#region Protected methods
		/// <summary>
		/// Validates the specified value with respect to the current validation attribute.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="validationContext">The context information about the validation operation.</param>
		/// <returns>An instance of the ValidationResult class.</returns>
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			IEnumerable<ParameterValidationResult> Results = Validator.GetValidationResult(value, mDataType, ParsedConstraints, validationContext?.MemberName ?? "Value", validationContext?.DisplayName);
			foreach (ParameterValidationResult result in Results)
			{
				// Can only return one ...
				return result;
			}
			return ValidationResult.Success;
		}
		#endregion
	}
}
