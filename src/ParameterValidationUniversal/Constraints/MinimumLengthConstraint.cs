#region Copyright
/*******************************************************************************
 * <copyright file="MinimumLengthConstraint.cs" owner="Daniel Kopp">
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
 * <file name="MinimumLengthConstraint.cs" date="2015-10-10">
 * A constraint specifying the minimum length of a string, byte array or Uri.
 * </file>
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
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x2a), nameof(minimumLength), minimumLength, Properties.Resources.MinimumLengthConstraint_LengthNegative);
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
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x2d), string.Format(Properties.Resources.Global_SetParameters_InvalidCount, this.Name, 1), this);
			}

			try
			{
				mMinimumLength = ParameterConvert.ToInt32(parameters[0]);
				if (mMinimumLength < 0)
				{
					throw new ParameterConversionException(Errors.CreateHResult(0x30), Properties.Resources.MinimumLengthConstraint_LengthNegative, ParameterDataType.Int32, mMinimumLength);
				}
			}
			catch (ParameterConversionException ex)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x33), string.Format(Properties.Resources.Global_SetParameters_Invalid, this.Name), this, ex);
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
				throw new CodedArgumentException(Errors.CreateHResult(0x36), string.Format(Properties.Resources.Global_Validate_TypeMismatch, type.Name, this.Name));
			}

			if (CurrentLength < mMinimumLength)
			{
				results.Add(new ParameterValidationResult(Errors.CreateHResult(0x39), string.Format(
					IsString ? Properties.Resources.MinimumLengthConstraint_Validate_FailedString
					: Properties.Resources.MinimumLengthConstraint_Validate_FailedBytes,
					displayName, mMinimumLength, CurrentLength), memberName, this));
			}
		}
		#endregion
		#endregion
	}
}
