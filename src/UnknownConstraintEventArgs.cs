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
