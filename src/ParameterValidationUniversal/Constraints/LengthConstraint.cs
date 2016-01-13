#region Copyright
/*******************************************************************************
 * <copyright file="LengthConstraint.cs" owner="Daniel Kopp">
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
 * <file name="LengthConstraint.cs" date="2015-10-10">
 * A constraint specifying the length of a string or byte array.
 * </file>
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
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x28), nameof(length), length, Properties.Resources.LengthConstraint_LengthNegative);
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
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x2b), string.Format(Properties.Resources.Global_SetParameters_InvalidCount, this.Name, 1), this);
			}

			try
			{
				mLength = ParameterConvert.ToInt32(parameters[0]);
				if (mLength < 0)
				{
					throw new ParameterConversionException(Errors.CreateHResult(0x2e), Properties.Resources.LengthConstraint_LengthNegative, ParameterDataType.Int32, mLength);
				}
			}
			catch (ParameterConversionException ex)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x31), string.Format(Properties.Resources.Global_SetParameters_Invalid, this.Name), this, ex);
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
				throw new CodedArgumentException(Errors.CreateHResult(0x34), string.Format(Properties.Resources.Global_Validate_TypeMismatch, type.Name, this.Name));
			}

			if (CurrentLength != mLength)
			{
				results.Add(new ParameterValidationResult(Errors.CreateHResult(0x37), string.Format(
					IsString ? Properties.Resources.LengthConstraint_Validate_FailedString
					: Properties.Resources.LengthConstraint_Validate_FailedBytes,
					displayName, mLength, CurrentLength), memberName, this));
			}
		}
		#endregion
		#endregion
	}
}
