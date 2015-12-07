#region Copyright
/*******************************************************************************
 * <copyright file="ParameterValidator.cs" owner="Daniel Kopp">
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
 * <file name="ParameterValidator.cs" date="2015-10-21">
 * Validates a parameter value using a set of Constraints.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyDuck.ParameterValidation
{
	/// <summary>
	/// Validates a parameter value using a set of <see cref="Constraint"/>s.
	/// </summary>
	public class ParameterValidator
	{
		#region Events
		/// <summary>
		/// The event that is raised before a parameter value is validated.
		/// </summary>
		public event EventHandler<ParameterValidationEventArgs> Validating;

		/// <summary>
		/// The event that is raised when a parameter validation fails.
		/// </summary>
		/// <remarks>The event is raised once after every constraint was evaluated. You can use <see cref="ParameterValidationErrorEventArgs.ValidationResults"/> to evaluate and modify the results, if required.</remarks>
		public event EventHandler<ParameterValidationErrorEventArgs> ValidationError;
		#endregion

		#region Private fields
		private static readonly Lazy<ParameterValidator> mValidator = new Lazy<ParameterValidator>(() =>
		{
			return new ParameterValidator();
		});

		private static readonly List<Constraint> NoConstraints = new List<Constraint>();

		#endregion

		#region Properties
		/// <summary>
		/// Gets a global instance of the <see cref="ParameterValidator"/>.
		/// </summary>
		public static ParameterValidator Validator
		{
			get { return mValidator.Value; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterValidator"/> class.
		/// </summary>
		public ParameterValidator()
		{
		}
		#endregion

		#region Public methods
		#region GetValidationResult
		/// <summary>
		/// Validates a value according to the specified constraints.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="dataType">The data type of <paramref name="value"/>.</param>
		/// <param name="constraints">A list of <see cref="Constraint"/>s to validate <paramref name="value"/> against.</param>
		/// <param name="memberName">The name of the property or control containing the <paramref name="value"/> to validate.</param>
		/// <returns>An enumeration of <see cref="ParameterValidationResult"/>s. If the enumeration is empty, no validation error was found.</returns>
		public IEnumerable<ParameterValidationResult> GetValidationResult(object value, ParameterDataType dataType, IReadOnlyList<Constraint> constraints, string memberName)
		{
			return GetValidationResult(value, dataType, constraints, memberName, null);
		}

		/// <summary>
		/// Validates a value according to the specified constraints.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="dataType">The data type of <paramref name="value"/>.</param>
		/// <param name="constraints">A list of <see cref="Constraint"/>s to validate <paramref name="value"/> against.</param>
		/// <param name="memberName">The name of the property or control containing the <paramref name="value"/> to validate.</param>
		/// <param name="displayName">The display name of the control that contains or displays the <paramref name="value"/> to validate. May be <see langword="null"/>.</param>
		/// <returns>An enumeration of <see cref="ParameterValidationResult"/>s. If the enumeration is empty, no validation error was found.</returns>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
		/// <exception cref="CodedArgumentNullOrEmptyException"><paramref name="memberName"/> is <see langword="null"/> or empty.</exception>
		public IEnumerable<ParameterValidationResult> GetValidationResult(object value, ParameterDataType dataType, IReadOnlyList<Constraint> constraints, string memberName, string displayName)
		{
			if (dataType == ParameterDataType.None)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x99), nameof(dataType), Properties.Resources.Global_ParameterDataType_None);
			}
			if (string.IsNullOrEmpty(memberName))
			{
				throw new CodedArgumentNullOrEmptyException(Errors.CreateHResult(0x9a), nameof(memberName));
			}

			if (constraints == null)
			{
				constraints = NoConstraints;
			}
			if (displayName == null)
			{
				displayName = memberName;
			}

			OnValidating(value, dataType, constraints, memberName, displayName);

			List<ParameterValidationResult> ReturnValue = new List<ParameterValidationResult>();
			// Use constraints only on non-null values
			if (value == null)
			{
				if (constraints.FirstOrDefault(c => c.Name == Constraint.NullConstraintName) == null)
				{
					ReturnValue.Add(new ParameterValidationResult(Errors.CreateHResult(0x9b), string.Format(Properties.Resources.ParameterValidator_Validate_ValueNull, displayName), memberName, new Constraints.NullConstraint()));
				}
			}
			else
			{
				foreach (Constraint constraint in constraints)
				{
					ReturnValue.AddRange(constraint.Validate(value, dataType, memberName, displayName));
				}
			}

			if (ReturnValue.Count != 0)
			{
				OnValidationError(value, dataType, constraints, memberName, displayName, ReturnValue);
			}

			return ReturnValue;
		}
		#endregion

		#region IsValid
		/// <summary>
		/// Validates a value according to the specified constraints, and throws an exception, if a validation error was found.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="dataType">The data type of <paramref name="value"/>.</param>
		/// <param name="constraints">A list of <see cref="Constraint"/>s to validate <paramref name="value"/> against.</param>
		/// <param name="memberName">The name of the property or control containing the <paramref name="value"/> to validate.</param>
		public bool IsValid(object value, ParameterDataType dataType, IReadOnlyList<Constraint> constraints, string memberName)
		{
			return IsValid(value, dataType, constraints, memberName, null);
		}

		/// <summary>
		/// Validates a value according to the specified constraints, and throws an exception, if a validation error was found.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="dataType">The data type of <paramref name="value"/>.</param>
		/// <param name="constraints">A list of <see cref="Constraint"/>s to validate <paramref name="value"/> against.</param>
		/// <param name="memberName">The name of the property or control containing the <paramref name="value"/> to validate.</param>
		/// <param name="displayName">The display name of the control that contains or displays the <paramref name="value"/> to validate. May be <see langword="null"/>.</param>
		public bool IsValid(object value, ParameterDataType dataType, IReadOnlyList<Constraint> constraints, string memberName, string displayName)
		{
			return !GetValidationResult(value, dataType, constraints, memberName, displayName).GetEnumerator().MoveNext();
		}
		#endregion

		#region Validate
		/// <summary>
		/// Validates a value according to the specified constraints, and throws an exception, if a validation error was found.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="dataType">The data type of <paramref name="value"/>.</param>
		/// <param name="constraints">A list of <see cref="Constraint"/>s to validate <paramref name="value"/> against.</param>
		/// <param name="memberName">The name of the property or control containing the <paramref name="value"/> to validate.</param>
		public void Validate(object value, ParameterDataType dataType, IReadOnlyList<Constraint> constraints, string memberName)
		{
			Validate(value, dataType, constraints, memberName, null);
		}

		/// <summary>
		/// Validates a value according to the specified constraints, and throws an exception, if a validation error was found.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="dataType">The data type of <paramref name="value"/>.</param>
		/// <param name="constraints">A list of <see cref="Constraint"/>s to validate <paramref name="value"/> against.</param>
		/// <param name="memberName">The name of the property or control containing the <paramref name="value"/> to validate.</param>
		/// <param name="displayName">The display name of the control that contains or displays the <paramref name="value"/> to validate. May be <see langword="null"/>.</param>
		public void Validate(object value, ParameterDataType dataType, IReadOnlyList<Constraint> constraints, string memberName, string displayName)
		{
			IEnumerable<ParameterValidationResult> Results = GetValidationResult(value, dataType, constraints, memberName, displayName);
			if (Results.GetEnumerator().MoveNext())
			{
				throw new ParameterValidationException(Errors.CreateHResult(0x96), Results);
			}
		}
		#endregion
		#endregion

		#region Protected methods
		#region OnValidating
		/// <summary>
		/// Raises the <see cref="Validating"/> event.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="dataType">The data type of <paramref name="value"/>.</param>
		/// <param name="constraints">A list of <see cref="Constraint"/>s to validate <paramref name="value"/> against.</param>
		/// <param name="memberName">The name of the property or control containing the <paramref name="value"/> to validate.</param>
		/// <param name="displayName">The display name of the control that contains or displays the <paramref name="value"/> to validate.</param>
		protected virtual void OnValidating(object value, ParameterDataType dataType, IReadOnlyList<Constraint> constraints, string memberName, string displayName)
		{
			Validating?.Invoke(this, new ParameterValidationEventArgs(value, dataType, constraints, memberName, displayName));
		}
		#endregion

		#region OnValidationError
		/// <summary>
		/// Raises the <see cref="ValidationError"/> event.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="dataType">The data type of <paramref name="value"/>.</param>
		/// <param name="constraints">A list of <see cref="Constraint"/>s to validate <paramref name="value"/> against.</param>
		/// <param name="memberName">The name of the property or control containing the <paramref name="value"/> to validate.</param>
		/// <param name="displayName">The display name of the control that contains or displays the <paramref name="value"/> to validate.</param>
		/// <param name="validationResults">The results of the current validation.</param>
		protected virtual void OnValidationError(object value, ParameterDataType dataType, IReadOnlyList<Constraint> constraints, string memberName, string displayName, IList<ParameterValidationResult> validationResults)
		{
			ValidationError?.Invoke(this, new ParameterValidationErrorEventArgs(value, dataType, constraints, memberName, displayName, validationResults));
		}
		#endregion
		#endregion
	}
}
