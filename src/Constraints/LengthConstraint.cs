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
	/// A constraint specifying the length of a <see cref="string"/> or <see cref="byte"/> array.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <list type="bullet">
	/// <item>The textual representation of the constraint is <c>[Length(value)]</c>. <c>value</c> must be a string representation of an integer. The minimum value is 0.</item>
	/// <item>The constraint is applicable to the <see cref="ParameterDataType.Bytes"/> and <see cref="ParameterDataType.String"/> data types.</item>
	/// <item>If a parameter with that constraint has a string or array length smaller or larger than <see cref="Length"/>, a <see cref="ParameterValidationResult"/> is added to the output of <see cref="Constraint.Validate(object, ParameterDataType, string, string)"/>.</item>
	/// </list>
	/// </para>
	/// </remarks>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	public class LengthConstraint : Constraint
	{
		#region Private fields
		private int mLength;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the length to enforce.
		/// </summary>
		/// <value>A non-negative integer value.</value>
		public int Length
		{
			get { return mLength; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="LengthConstraint"/> class.
		/// </summary>
		public LengthConstraint()
			: base(LengthConstraintName)
		{
			mLength = int.MaxValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LengthConstraint"/> class with the specified length.
		/// </summary>
		/// <param name="length">The length to enforce.</param>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="length"/> is less than 0.</exception>
		public LengthConstraint(int length)
			: base(LengthConstraintName)
		{
			if (length < 0)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(ErrorCodes.LengthConstraint_ctor_LengthNegative), nameof(length), length, Properties.Resources.LengthConstraint_LengthNegative);
			}

			mLength = length;
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="LengthConstraint"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The constraint could not be deserialized correctly.</exception>
		protected LengthConstraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			mLength = info.GetInt32(nameof(Length));
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
			info.AddValue(nameof(Length), mLength);
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
			parameters.Add(ParameterConvert.ToString(mLength));
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
			AssertDataType(dataType, ParameterDataType.Bytes, ParameterDataType.String);
			if (parameters.Count != 1)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(ErrorCodes.LengthConstraint_SetParameters_OnlyOneParam), string.Format(Properties.Resources.Global_SetParameters_InvalidCount, this.Name, 1), this);
			}

			try
			{
				mLength = ParameterConvert.ToInt32(parameters[0]);
				if (mLength < 0)
				{
					throw new ParameterConversionException(Errors.CreateHResult(ErrorCodes.LengthConstraint_SetParameters_LengthNegative), Properties.Resources.LengthConstraint_LengthNegative, ParameterDataType.Int32, mLength);
				}
			}
			catch (ParameterConversionException ex)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(ErrorCodes.LengthConstraint_SetParameters_ParamInvalid), string.Format(Properties.Resources.Global_SetParameters_Invalid, this.Name), this, ex);
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
			AssertDataType(dataType, ParameterDataType.Bytes, ParameterDataType.String);

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
			else
			{
				throw new CodedArgumentException(Errors.CreateHResult(ErrorCodes.LengthConstraint_Validate_TypeNotSupported), string.Format(Properties.Resources.Global_Validate_TypeMismatch, type.Name, this.Name));
			}

			if (CurrentLength != mLength)
			{
				results.Add(new ParameterValidationResult(Errors.CreateHResult(ErrorCodes.LengthConstraint_Validate_LengthInvalid), string.Format(
					IsString ? Properties.Resources.LengthConstraint_Validate_FailedString
					: Properties.Resources.LengthConstraint_Validate_FailedBytes,
					displayName, mLength, CurrentLength), memberName, this));
			}
		}
		#endregion
		#endregion
	}
}
