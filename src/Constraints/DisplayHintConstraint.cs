﻿#region Copyright
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
using System.Collections.Generic;

namespace NerdyDuck.ParameterValidation.Constraints
{
	/// <summary>
	/// A constraint specifying how a parameter should be displayed.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <list type="bullet">
	/// <item>The textual representation of the constraint is <c>[DisplayHint(parameter[,parameter,...])]</c>. It can take any number of arbitrary strings as parameters.</item>
	/// <item>The constraint is applicable to all data types in <see cref="ParameterDataType"/>.</item>
	/// <item>The constraint is not used during validation or serialization, but is solely thought as a hint for display purposes.
	/// Instead of creating your own constraints to specify a new behavior in your user interface, you can use this constraint to specify any application-specific information to define your UI.</item>
	/// </list>
	/// </para>
	/// </remarks>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	public class DisplayHintConstraint : Constraint
	{
		#region Private fields
		private List<string> mHints;
		#endregion

		#region Properties
		/// <summary>
		/// Gets a collection of hints to modify user interface behavior.
		/// </summary>
		/// <value>One or more parameters.</value>
		public IEnumerable<string> Hints
		{
			get { return mHints; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="DisplayHintConstraint"/> class.
		/// </summary>
		public DisplayHintConstraint()
			: base(DisplayHintConstraintName)
		{
			mHints = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DisplayHintConstraint"/> class with the specified parameter.
		/// </summary>
		/// <param name="hint">An arbitrary string.</param>
		/// <exception cref="CodedArgumentNullException"><paramref name="hint"/> is <see langword="null"/>, empty or white-space.</exception>
		public DisplayHintConstraint(string hint)
			: base(DisplayHintConstraintName)
		{
			if (string.IsNullOrWhiteSpace(hint))
			{
				throw new CodedArgumentNullOrWhiteSpaceException(Errors.CreateHResult(ErrorCodes.DisplayHintConstraint_ctor_ArgNullEmpty), nameof(hint));
			}

			mHints = new List<string>();
			mHints.Add(hint);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DisplayHintConstraint"/> class with a collection of parameters.
		/// </summary>
		/// <param name="hints">A collection of arbitrary string parameters.</param>
		public DisplayHintConstraint(IEnumerable<string> hints)
			: base(DisplayHintConstraintName)
		{
			if (hints == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(ErrorCodes.DisplayHintConstraint_ctor_ArgNull), nameof(hints));
			}

			List<string> Temp = new List<string>(hints);
			CheckParameters(Temp);
			mHints = Temp;
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="DisplayHintConstraint"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The constraint could not be deserialized correctly.</exception>
		protected DisplayHintConstraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			mHints = (List<string>)info.GetValue(nameof(Hints), typeof(List<string>));
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
			info.AddValue(nameof(Hints), mHints);
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
			foreach (string parameter in mHints)
			{
				parameters.Add(parameter);
			}
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
			CheckParameters(parameters);
			mHints = new List<string>(parameters);
		}
		#endregion
		#endregion

		#region Private methods
		/// <summary>
		/// Checks that the specified list of parameters is not empty, and does not contain null or whitespace values.
		/// </summary>
		/// <param name="parameters">A list of parameters.</param>
		private static void CheckParameters(IReadOnlyList<string> parameters)
		{
			if (parameters.Count == 0)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(ErrorCodes.DisplayHintConstraint_CheckParameters_NoParams), Properties.Resources.DisplayHintConstraint_CheckParameters_NoParameter);
			}
			foreach (string parameter in parameters)
			{
				if (string.IsNullOrWhiteSpace(parameter))
				{
					throw new ConstraintConfigurationException(Errors.CreateHResult(ErrorCodes.DisplayHintConstraint_CheckParameters_ParamNullEmpty), Properties.Resources.DisplayHintConstraint_CheckParameters_InvalidParameter);
				}
			}
		}
		#endregion
	}
}