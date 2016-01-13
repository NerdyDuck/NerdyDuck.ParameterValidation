#region Copyright
/*******************************************************************************
 * <copyright file="ParameterValidationEventArgs.cs" owner="Daniel Kopp">
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
 * <file name="ParameterValidationEventArgs.cs" date="2015-10-29">
 * Provides data for events occurring during a parameter validation.
 * </file>
 ******************************************************************************/
#endregion

using System;
using System.Collections.Generic;

namespace NerdyDuck.ParameterValidation
{
	/// <summary>
	/// Provides data for events occurring during a parameter validation.
	/// </summary>
	public class ParameterValidationEventArgs : EventArgs
	{
		#region Private fields
		private object mValue;
		private ParameterDataType mDataType;
		private IReadOnlyList<Constraint> mConstraints;
		private string mMemberName;
		private string mDisplayName;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the value that is currently validated.
		/// </summary>
		/// <value>The value to validate.</value>
		public object Value
		{
			get { return mValue; }
		}

		/// <summary>
		/// Gets the data type of <see cref="Value"/>.
		/// </summary>
		/// <value>One of the <see cref="ParameterDataType"/> values.</value>
		public ParameterDataType DataType
		{
			get { return mDataType; }
		}

		/// <summary>
		/// Gets a list of <see cref="Constraint"/>s that are used to validate <see cref="Value"/>.
		/// </summary>
		/// <value>A list of one or more objects derived from <see cref="Constraint"/>.</value>
		public IReadOnlyList<Constraint> Constraints
		{
			get { return mConstraints; }
		}

		/// <summary>
		/// Gets the name of the property or control that contains <see cref="Value"/>.
		/// </summary>
		/// <value>The name of the property or control to validate.</value>
		public string MemberName
		{
			get { return mMemberName; }
		}

		/// <summary>
		/// Gets the display name of the control that displays <see cref="Value"/>.
		/// </summary>
		/// <value>the display name of the property or control to validate.</value>
		public string DisplayName
		{
			get { return mDisplayName; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterValidationEventArgs"/> class with the specified parameters.
		/// </summary>
		/// <param name="value">The value that is currently validated.</param>
		/// <param name="dataType">The data type of <paramref name="value"/>.</param>
		/// <param name="constraints">A list of <see cref="Constraint"/>s that are used to validate <paramref name="value"/>.</param>
		/// <param name="memberName">The name of the property or control that contains <paramref name="value"/>.</param>
		/// <param name="displayName">The display name of the control that displays <paramref name="value"/>.</param>
		public ParameterValidationEventArgs(object value, ParameterDataType dataType, IReadOnlyList<Constraint> constraints, string memberName, string displayName)
		{
			mValue = value;
			mDataType = dataType;
			mConstraints = constraints;
			mMemberName = memberName;
			mDisplayName = displayName;
		}
		#endregion
	}
}
