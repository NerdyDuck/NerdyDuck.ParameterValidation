#region Copyright
/*******************************************************************************
 * <copyright file="RegexConstraint.cs" owner="Daniel Kopp">
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
 * <file name="RegexConstraint.cs" date="2015-10-10">
 * A constraint specifying a regular expression to match a string to.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NerdyDuck.ParameterValidation.Constraints
{
	/// <summary>
	/// A constraint specifying a regular expression to match a <see cref="string"/> to.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <list type="bullet">
	/// <item>The textual representation of the constraint is <c>[Regex(expr [,option,option...])]</c>. <c>expr</c> must be a valid regular expression. <c>option</c> is one of the <see cref="RegexOptions"/> values.</item>
	/// <item>The constraint is applicable to the <see cref="ParameterDataType.String"/> data type only.</item>
	/// <item>If a parameter with that constraint has a string that does not match <see cref="RegularExpression"/>, a <see cref="ParameterValidationResult"/> is added to the output of <see cref="Constraint.Validate(object, ParameterDataType, string, string)"/>.</item>
	/// </list>
	/// </para>
	/// </remarks>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	public class RegexConstraint : Constraint
	{
		#region Private fields
		private string mRegularExpression;
		private RegexOptions mOptions;
		private Regex InternalRegex;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the regular expression to enforce.
		/// </summary>
		/// <value>A valid regular expression.</value>
		public string RegularExpression
		{
			get { return mRegularExpression; }
		}

		/// <summary>
		/// Gets the options to apply when evaluating the <see cref="RegularExpression"/>.
		/// </summary>
		/// <value>A combination of the <see cref="RegexOptions"/> values.</value>
		public RegexOptions Options
		{
			get { return mOptions; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="RegexConstraint"/> class.
		/// </summary>
		public RegexConstraint()
			: this(".*", RegexOptions.None)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RegexConstraint"/> class with the specified regular expression and options.
		/// </summary>
		/// <param name="regularExpression">The length to enforce.</param>
		/// <param name="options">The options to apply when evaluating the <paramref name="regularExpression"/>.</param>
		/// <exception cref="CodedArgumentNullException"><paramref name="regularExpression"/> is<see langword="null"/>, empty or white-space.</exception>
		/// <exception cref="CodedArgumentException"><paramref name="regularExpression"/> is not a valid regular expression.</exception>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="options"/> are invalid.</exception>
		public RegexConstraint(string regularExpression, RegexOptions options)
			: base(RegexConstraintName)
		{
			if (string.IsNullOrWhiteSpace(regularExpression))
			{
				throw new CodedArgumentNullOrWhiteSpaceException(Errors.CreateHResult(0x3a), nameof(regularExpression));
			}

			mRegularExpression = regularExpression;
			mOptions = options;
			try
			{
				InternalRegex = new Regex(mRegularExpression, mOptions);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x3b), Properties.Resources.RegexConstraint_OptionsInvalid, ex);
			}
			catch (ArgumentException ex)
			{
				throw new CodedArgumentException(Errors.CreateHResult(0x3c), Properties.Resources.RegexConstraint_PatternInvalid, nameof(regularExpression), ex);
			}
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="RegexConstraint"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The constraint could not be deserialized correctly.</exception>
		protected RegexConstraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			mRegularExpression = info.GetString(nameof(RegularExpression));
			mOptions = (RegexOptions)info.GetValue(nameof(Options), typeof(RegexOptions));
			InternalRegex = new Regex(mRegularExpression, mOptions);
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
			info.AddValue(nameof(RegularExpression), mRegularExpression);
			info.AddValue(nameof(Options), mOptions);
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
			parameters.Add(mRegularExpression);
			if (mOptions != RegexOptions.None)
			{
				foreach (RegexOptions opts in Enum.GetValues(typeof(RegexOptions)))
				{
					if (mOptions.HasFlag(opts) && opts != RegexOptions.None)
					{
						parameters.Add(Enum.GetName(typeof(RegexOptions), opts));
					}
				}
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
			AssertDataType(dataType, ParameterDataType.String);
			if (parameters.Count < 1)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x3d), string.Format(Properties.Resources.Global_SetParameters_InvalidMinCount, this.Name, 1), this);
			}

			if (string.IsNullOrWhiteSpace(parameters[0]))
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x3e), Properties.Resources.RegexConstraint_PatternInvalid, this);
			}
			mRegularExpression = parameters[0];

			mOptions = RegexOptions.None;
			if (parameters.Count > 1)
			{
				for (int i = 1; i < parameters.Count; i++)
				{
					RegexOptions opts;
					if (!Enum.TryParse<RegexOptions>(parameters[i], out opts))
					{
						throw new ConstraintConfigurationException(Errors.CreateHResult(0x3f), Properties.Resources.RegexConstraint_OptionsInvalid, this);
					}
					mOptions |= opts;
				}
			}

			try
			{

				InternalRegex = new Regex(mRegularExpression, mOptions);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x3f), Properties.Resources.RegexConstraint_OptionsInvalid, this, ex);
			}
			catch (ArgumentException ex)
			{
				throw new ConstraintConfigurationException(Errors.CreateHResult(0x3e), Properties.Resources.RegexConstraint_PatternInvalid, this, ex);
			}
		}
		#endregion

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
			base.OnValidation(results, value, dataType, memberName, displayName);
			AssertDataType(dataType, ParameterDataType.String);

			if (!InternalRegex.IsMatch((string)value))
			{
				results.Add(new ParameterValidationResult(Errors.CreateHResult(0x40), string.Format(Properties.Resources.RegexConstraint_Validate_Failed, displayName, mRegularExpression), memberName, this));
			}
		}
		#endregion
		#endregion
	}
}
