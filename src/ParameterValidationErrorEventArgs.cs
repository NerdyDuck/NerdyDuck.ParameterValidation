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
