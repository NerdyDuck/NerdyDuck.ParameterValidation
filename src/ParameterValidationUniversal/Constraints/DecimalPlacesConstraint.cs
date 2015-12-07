#region Copyright
/*******************************************************************************
 * <copyright file="DecimalPlacesConstraint.cs" owner="Daniel Kopp">
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
 * <file name="DecimalPlacesConstraint.cs" date="2015-10-01">
 * Specifies the number of decimal places to write when displaying a decimal
 * parameter.
 * </file>
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
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x2a), string.Format(Properties.Resources.Global_SetParameters_InvalidCount, this.Name, 1), this);
			}

			try
			{
				mDecimalPlaces = ParameterConvert.ToInt32(parameters[0]);
			}
			catch (ParameterConversionException ex)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x2b), string.Format(Properties.Resources.Global_SetParameters_Invalid, this.Name), this, ex);
			}
		}
		#endregion
		#endregion
	}
}
