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

namespace NerdyDuck.ParameterValidation.Constraints
{
	/// <summary>
	/// A constraint specifying the minimum length of a <see cref="string"/>, <see cref="byte"/> array or <see cref="Uri"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <list type="bullet">
	/// <item>The textual representation of the constraint is <c>[MinLength(value)]</c>. <c>value</c> must be a string representation of an integer. The minimum value is 0.</item>
	/// <item>The constraint is applicable to the <see cref="ParameterDataType.Bytes"/>, <see cref="ParameterDataType.String"/> and <see cref="ParameterDataType.Uri"/> data types.</item>
	/// <item>If a parameter with that constraint has a string or array length less than <see cref="MinimumLength"/>, or a <see cref="Uri"/> a length less than <see cref="MinimumLength"/>, a <see cref="ParameterValidationResult"/> is added to the output of <see cref="Constraint.Validate(object, ParameterDataType, string, string)"/>.</item>
	/// </list>
	/// </para>
	/// </remarks>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	public class MinimumLengthConstraint : Constraint
	{

		#region Private fields
		private int mMinimumLength;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the minimum length to enforce.
		/// </summary>
		/// <value>A non-negative integer value.</value>
		public int MinimumLength
		{
			get { return mMinimumLength; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MinimumLengthConstraint"/> class.
		/// </summary>
		public MinimumLengthConstraint()
			: base(MinimumLengthConstraintName)
		{
			mMinimumLength = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MinimumLengthConstraint"/> class with the specified minimum length.
		/// </summary>
		/// <param name="minimumLength">The minimum length to enforce.</param>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="minimumLength"/> is less than 0.</exception>
		public MinimumLengthConstraint(int minimumLength)
			: base(MinimumLengthConstraintName)
		{
			if (minimumLength < 0)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(ErrorCodes.MinimumLengthConstraint_ctor_LengthNegative), nameof(minimumLength), minimumLength, Properties.Resources.MinimumLengthConstraint_LengthNegative);
			}

			mMinimumLength = minimumLength;
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="MinimumLengthConstraint"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The constraint could not be deserialized correctly.</exception>
		protected MinimumLengthConstraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			mMinimumLength = info.GetInt32(nameof(MinimumLength));
		}
#endif
		#endregion

		#region Public methods
#if WINDOWS_DESKTOP
		#region GetObjectData
		/// <summary>
		/// Sets the <see cref="System.Runtime.Serialization.SerializationInfo"/> with information about the <see cref="Constraint"/>.
		/// </summary>
		/// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data of the <see cref="Constraint"/>.</param>
		/// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(nameof(MinimumLength), mMinimumLength);
		}
		#endregion
#endif
		#endregion

		#region Protected methods
		#region GetParameters
		/// <summary>
		/// Adds the parameters of the constraint to a list of strings.
		/// </summary>
		/// <param name="parameters">A list of strings to add the parameters to.</param>
		/// <remarks>Override this method, if the constraint makes use of parameters. Add the parameters in the order that they should be provided to <see cref="SetParameters"/>.</remarks>
		protected override void GetParameters(IList<string> parameters)
		{
			base.GetParameters(parameters);
			parameters.Add(ParameterConvert.ToString(mMinimumLength));
		}
		#endregion

		#region SetParameters
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
			AssertDataType(dataType, ParameterDataType.Bytes, ParameterDataType.String, ParameterDataType.Uri);
			if (parameters.Count != 1)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(ErrorCodes.MinimumLengthConstraint_SetParameters_OnlyOneParam), string.Format(Properties.Resources.Global_SetParameters_InvalidCount, this.Name, 1), this);
			}

			try
			{
				mMinimumLength = ParameterConvert.ToInt32(parameters[0]);
				if (mMinimumLength < 0)
				{
					throw new ParameterConversionException(Errors.CreateHResult(ErrorCodes.MinimumLengthConstraint_SetParameters_LengthNegative), Properties.Resources.MinimumLengthConstraint_LengthNegative, ParameterDataType.Int32, mMinimumLength);
				}
			}
			catch (ParameterConversionException ex)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(ErrorCodes.MinimumLengthConstraint_SetParameters_ParamInvalid), string.Format(Properties.Resources.Global_SetParameters_Invalid, this.Name), this, ex);
			}
		}
		#endregion

		#region OnValidation
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
			AssertDataType(dataType, ParameterDataType.Bytes, ParameterDataType.String, ParameterDataType.Uri);

			int CurrentLength = 0;
			bool IsString = false;
			Type type = value.GetType();
			if (type == typeof(byte[]))
			{
				CurrentLength = ((byte[])value).Length;
			}
			else if (type == typeof(string))
			{
				IsString = true;
				CurrentLength = ((string)value).Length;
			}
			else if (type == typeof(Uri))
			{
				IsString = true;
				CurrentLength = ((Uri)value).ToString().Length;
			}
			else
			{
				throw new CodedArgumentException(Errors.CreateHResult(ErrorCodes.MinimumLengthConstraint_Validate_TypeNotSupported), string.Format(Properties.Resources.Global_Validate_TypeMismatch, type.Name, this.Name));
			}

			if (CurrentLength < mMinimumLength)
			{
				results.Add(new ParameterValidationResult(Errors.CreateHResult(ErrorCodes.MinimumLengthConstraint_Validate_TooShort), string.Format(
					IsString ? Properties.Resources.MinimumLengthConstraint_Validate_FailedString
					: Properties.Resources.MinimumLengthConstraint_Validate_FailedBytes,
					displayName, mMinimumLength, CurrentLength), memberName, this));
			}
		}
		#endregion
		#endregion
	}
}
