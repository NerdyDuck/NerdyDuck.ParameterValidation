#region Copyright
/*******************************************************************************
 * <copyright file="ParameterValidationResult.cs" owner="Daniel Kopp">
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
 * <file name="ParameterValidationResult.cs" date="2015-10-01">
 * Represents a container for the result of a validation using
 * <see cref="Constraint"/>s.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NerdyDuck.ParameterValidation
{
	/// <summary>
	/// Represents a container for the result of a validation using <see cref="Constraint"/>s.
	/// </summary>
#if WINDOWS_DESKTOP
	[Serializable]
	public class ParameterValidationResult : ValidationResult, System.Runtime.Serialization.ISerializable
#endif
#if WINDOWS_UWP
	public class ParameterValidationResult : ValidationResult
#endif
	{
		#region Private fields
		private Constraint mConstraint;
		private int mHResult;
		private static readonly Lazy<ParameterValidationResult> mSuccess = new Lazy<ParameterValidationResult>(() =>
		{
			return new ParameterValidationResult(Errors.CreateHResult(0x00), Properties.Resources.ParameterValidationResult_Success, null);
		});
		#endregion

		#region Properties
		/// <summary>
		/// Gets a <see cref="ParameterValidationResult"/> that specifies a successful validation.
		/// </summary>
		public static new ParameterValidationResult Success
		{
			get { return mSuccess.Value; }
		}

		/// <summary>
		/// Gets the <see cref="Constraint"/> that raised the validation error.
		/// </summary>
		/// <value>The <see cref="Constraint"/> that raised the validation error. May be <see langword="null"/>.</value>
		public Constraint Constraint
		{
			get { return mConstraint; }
		}

		/// <summary>
		/// Gets an integer value that identifies the validation error.
		/// </summary>
		/// <value>A custom integer identifier.</value>
		public int HResult
		{
			get { return mHResult; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterValidationResult"/> class with the specified HRESULT, error message and constraint.
		/// </summary>
		/// <param name="hresult">An integer value that identifies the validation error.</param>
		/// <param name="errorMessage"></param>
		/// <param name="constraint">The <see cref="Constraint"/> that raised the validation error. May be <see langword="null"/>.</param>
		public ParameterValidationResult(int hresult, string errorMessage, Constraint constraint)
			: base(errorMessage)
		{
			mHResult = hresult;
			mConstraint = constraint;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterValidationResult"/> by using another <see cref="ParameterValidationResult"/> object.
		/// </summary>
		/// <param name="validationResult">The validation result object to copy.</param>
		public ParameterValidationResult(ParameterValidationResult validationResult)
			: base(validationResult)
		{
			if (validationResult == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0xa2), nameof(validationResult));
			}
			mHResult = validationResult.HResult;
			mConstraint = validationResult.Constraint;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterValidationResult"/> class with the specified HRESULT, error message, member names and constraint.
		/// </summary>
		/// <param name="hresult">An integer value that identifies the validation error.</param>
		/// <param name="errorMessage"></param>
		/// <param name="memberNames">The list of member names that have validation errors.</param>
		/// <param name="constraint">The <see cref="Constraint"/> that raised the validation error. May be <see langword="null"/>.</param>
		public ParameterValidationResult(int hresult, string errorMessage, IEnumerable<string> memberNames, Constraint constraint)
			: base(errorMessage, memberNames)
		{
			mHResult = hresult;
			mConstraint = constraint;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterValidationResult"/> class with the specified HRESULT, error message, member name and constraint.
		/// </summary>
		/// <param name="hresult">An integer value that identifies the validation error.</param>
		/// <param name="errorMessage"></param>
		/// <param name="memberName">The name of a member that has validation errors.</param>
		/// <param name="constraint">The <see cref="Constraint"/> that raised the validation error. May be <see langword="null"/>.</param>
		public ParameterValidationResult(int hresult, string errorMessage, string memberName, Constraint constraint)
			: base(errorMessage, new string[] { memberName })
		{
			mHResult = hresult;
			mConstraint = constraint;
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterValidationResult"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The exception could not be deserialized correctly.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Must be handled by base class.")]
		protected ParameterValidationResult(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info.GetString(nameof(ErrorMessage)), (List<string>)info.GetValue(nameof(MemberNames), typeof(List<string>)))
		{
			mConstraint = (Constraint)info.GetValue(nameof(Constraint), typeof(Constraint));
			mHResult = info.GetInt32(nameof(HResult));
		}
#endif
		#endregion

#if WINDOWS_DESKTOP
		#region Public methods
		/// <summary>
		/// Sets the <see cref="System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
		/// </summary>
		/// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			if (info == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x9e), nameof(info));
			}
			info.AddValue(nameof(ErrorMessage), ErrorMessage);
			info.AddValue(nameof(MemberNames), MemberNames.ToList());
			info.AddValue(nameof(Constraint), mConstraint);
			info.AddValue(nameof(HResult), mHResult);
		}
		#endregion
#endif
	}
}
