﻿#region Copyright
/*******************************************************************************
 * <copyright file="UpperCaseConstraint.cs" owner="Daniel Kopp">
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
 * <file name="UpperCaseConstraint.cs" date="2015-10-01">
 * A constraint specifying that a string may only contain upper-case characters.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System.Collections.Generic;
using System.IO;

namespace NerdyDuck.ParameterValidation.Constraints
{
	/// <summary>
	/// A constraint specifying that a string may only contain upper-case characters.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <list type="bullet">
	/// <item>The textual representation of the constraint is <c>[UpperCase]</c>.</item>
	/// <item>The constraint is only applicable to the <see cref="ParameterDataType.String"/> data type.</item>
	/// <item>If a string parameter with that constraint contains lower-case characters, a <see cref="ParameterValidationResult"/> is added to the output of <see cref="Constraint.Validate(object, ParameterDataType, string, string)"/>.</item>
	/// </list>
	/// </para>
	/// </remarks>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	public class UpperCaseConstraint : Constraint
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="UpperCaseConstraint"/> class.
		/// </summary>
		public UpperCaseConstraint()
			: base(UpperCaseConstraintName)
		{
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="UpperCaseConstraint"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The constraint could not be deserialized correctly.</exception>
		protected UpperCaseConstraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
#endif
		#endregion

		#region Protected methods
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
			AssertDataType(dataType, ParameterDataType.String);

			string Temp = value as string;
			if (Temp.ToUpperInvariant().CompareTo(Temp) != 0)
			{
				results.Add(new ParameterValidationResult(Errors.CreateHResult(0x0c), Properties.Resources.UpperCaseConstraint_Validate_Failed, this));
			}
		}
		#endregion
		#endregion
	}
}