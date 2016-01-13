#region Copyright
/*******************************************************************************
 * <copyright file="ParameterValidationErrorEventArgs.cs" owner="Daniel Kopp">
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
 * <file name="ParameterValidationErrorEventArgs.cs" date="2015-10-29">
 * Provides data for events occurring when a parameter validation fails.
 * </file>
 ******************************************************************************/
#endregion

using System.Collections.Generic;

namespace NerdyDuck.ParameterValidation
{
	/// <summary>
	/// Provides data for events occurring when a parameter validation fails.
	/// </summary>
	public class ParameterValidationErrorEventArgs : ParameterValidationEventArgs
	{
		#region Private fields
		private IList<ParameterValidationResult> mValidationResults;
		#endregion

		#region Properties
		/// <summary>
		/// Gets a list of <see cref="ParameterValidationResult"/>s.
		/// </summary>
		/// <value>One or more <see cref="ParameterValidationResult"/>s.</value>
		public IList<ParameterValidationResult> ValidationResults
		{
			get { return mValidationResults; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterValidationErrorEventArgs"/> class with the specified parameters.
		/// </summary>
		/// <param name="value">The value that is currently validated.</param>
		/// <param name="dataType">The data type of <paramref name="value"/>.</param>
		/// <param name="constraints">A list of <see cref="Constraint"/>s that are used to validate <paramref name="value"/>.</param>
		/// <param name="memberName">The name of the property or control that contains <paramref name="value"/>.</param>
		/// <param name="displayName">The display name of the control that displays <paramref name="value"/>.</param>
		/// <param name="validationResults">A list of <see cref="ParameterValidationResult"/>s.</param>
		public ParameterValidationErrorEventArgs(object value, ParameterDataType dataType, IReadOnlyList<Constraint> constraints, string memberName, string displayName, IList<ParameterValidationResult> validationResults)
			: base(value, dataType, constraints, memberName, displayName)
		{
			mValidationResults = validationResults;
		}
		#endregion
	}
}
