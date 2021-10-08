#region Copyright
/*******************************************************************************
 * <copyright file="UnknownConstraintEventArgs.cs" owner="Daniel Kopp">
 * Copyright 2015-2016 Daniel Kopp
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
 * <file name="UnknownConstraintEventArgs.cs" date="2015-10-19">
 * Provides data for the ConstraintParser.UnknownConstraint event.
 * </file>
 ******************************************************************************/
#endregion

using System;

namespace NerdyDuck.ParameterValidation
{
	/// <summary>
	/// Provides data for the <see cref="ConstraintParser.UnknownConstraint"/> event.
	/// </summary>
	public class UnknownConstraintEventArgs : EventArgs
	{
		#region Private fields
		private string mConstraintName;
		private ParameterDataType mDataType;
		private Constraint mConstraint;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of the constraint that is not known or not by default defined for the specified <see cref="DataType"/>.
		/// </summary>
		/// <value>The name of the constraint in its textual representation.</value>
		public string ConstraintName
		{
			get { return mConstraintName; }
		}

		/// <summary>
		/// Gets the data type of the parameter the unknown constraint is intended for.
		/// </summary>
		/// <value>One of the <see cref="ParameterDataType"/> values.</value>
		public ParameterDataType DataType
		{
			get { return mDataType; }
		}

		/// <summary>
		/// Gets or sets the constraint that was resolved by the <see cref="ConstraintParser.UnknownConstraint"/> event.
		/// </summary>
		/// <value>An object derived from <see cref="Constraint"/>.</value>
		/// <remarks>When handling the <see cref="ConstraintParser.UnknownConstraint"/> event, set this property if the event handler was able to resolve the constraint.</remarks>
		public Constraint Constraint
		{
			get { return mConstraint; }
			set { mConstraint = value; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="UnknownConstraintEventArgs"/> with the specified constraint name and data type.
		/// </summary>
		/// <param name="constraintName">The name of the constraint that is not known or not by default defined for the specified <paramref name="dataType"/>.</param>
		/// <param name="dataType">The data type of the parameter the unknown constraint is intended for.</param>
		public UnknownConstraintEventArgs(string constraintName, ParameterDataType dataType)
		{
			mConstraintName = constraintName;
			mDataType = dataType;
			mConstraint = null;
		}
		#endregion
	}
}
