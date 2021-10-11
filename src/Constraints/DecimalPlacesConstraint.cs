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
using System;
using System.Collections.Generic;

namespace NerdyDuck.ParameterValidation.Constraints
{
	/// <summary>
	/// Specifies the number of decimal places to write when displaying a <see cref="decimal"/> parameter.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <list type="bullet">
	/// <item>The textual representation of the constraint is <c>[DecimalPlaces(places)]</c>. <c>places</c> must be a string representation of an integer. A negative value indicates to display all relevant places.</item>
	/// <item>The constraint is applicable to the <see cref="ParameterDataType.Decimal"/> data type only.</item>
	/// <item>The constraint is not used during validation or serialization, but is solely thought as a hint for display purposes.</item>
	/// </list>
	/// </para>
	/// </remarks>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	public class DecimalPlacesConstraint : Constraint
	{
		#region Private fields
		private int mDecimalPlaces;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the number of decimal places to display.
		/// </summary>
		/// <value>A negative value indicates to display all relevant places. The default value is 2.</value>
		public int DecimalPlaces
		{
			get { return mDecimalPlaces; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="DecimalPlacesConstraint"/> class.
		/// </summary>
		/// <remarks>This constructor sets <see cref="DecimalPlaces"/> to 2.</remarks>
		public DecimalPlacesConstraint()
			: this(2)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DecimalPlacesConstraint"/> class with the specified number of decimal places to display.
		/// </summary>
		/// <param name="decimalPlaces">The number of decimal places to display.</param>
		public DecimalPlacesConstraint(int decimalPlaces)
			: base(DecimalPlacesConstraintName)
		{
			mDecimalPlaces = decimalPlaces;
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="DecimalPlacesConstraint"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The constraint could not be deserialized correctly.</exception>
		protected DecimalPlacesConstraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			mDecimalPlaces = info.GetInt32(nameof(DecimalPlaces));
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
			info.AddValue(nameof(DecimalPlaces), mDecimalPlaces);
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
			parameters.Add(ParameterConvert.ToString(mDecimalPlaces));
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
			AssertDataType(dataType, ParameterDataType.Decimal);
			if (parameters.Count != 1)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(ErrorCodes.DecimalPlacesConstraint_SetParameters_OnlyOneParam), string.Format(Properties.Resources.Global_SetParameters_InvalidCount, this.Name, 1), this);
			}

			try
			{
				mDecimalPlaces = ParameterConvert.ToInt32(parameters[0]);
			}
			catch (ParameterConversionException ex)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(ErrorCodes.DecimalPlacesConstraint_SetParameters_ParamInvalid), string.Format(Properties.Resources.Global_SetParameters_Invalid, this.Name), this, ex);
			}
		}
		#endregion
		#endregion
	}
}