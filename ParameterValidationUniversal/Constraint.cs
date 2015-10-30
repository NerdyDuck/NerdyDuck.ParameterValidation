#region Copyright
/*******************************************************************************
 * <copyright file="Constraint.cs" owner="Daniel Kopp">
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
 * <file name="Constraint.cs" date="2015-09-29">
 * The base class for all parameter constraints.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System.Collections.Generic;

namespace NerdyDuck.ParameterValidation
{
	/// <summary>
	/// The base class for all parameter constraints.
	/// </summary>
#if WINDOWS_DESKTOP
	[System.Serializable]
	public abstract class Constraint : System.Runtime.Serialization.ISerializable
#endif
#if WINDOWS_UWP
	public abstract class Constraint
#endif
	{
		#region Constants
		internal const string Namespace = "http://www.nerdyduck.de/ParameterValidation";
		internal const string AllowedSchemeConstraintName = "AllowedScheme";
		internal const string CharacterSetConstraintName = "CharSet";
		internal const string DatabaseConstraintName = "Database";
		internal const string DecimalPlacesConstraintName = "DecimalPlaces";
		internal const string EncryptedConstraintName = "Encrypted";
		internal const string EndpointConstraintName = "Endpoint";
		internal const string EnumValuesConstraintName = "Values";
		internal const string FileNameConstraintName = "FileName";
		internal const string HostnameConstraintName = "Host";
		internal const string LengthConstraintName = "Length";
		internal const string LowerCaseConstraintName = "LowerCase";
		internal const string MaximumLengthConstraintName = "MaxLength";
		internal const string MaximumValueConstraintName = "MaxValue";
		internal const string MinimumLengthConstraintName = "MinLength";
		internal const string MinimumValueConstraintName = "MinValue";
		internal const string NullConstraintName = "Null";
		internal const string PasswordConstraintName = "Password";
		internal const string PathConstraintName = "Path";
		internal const string RegexConstraintName = "Regex";
		internal const string StepConstraintName = "Step";
		internal const string TypeConstraintName = "Type";
		internal const string UpperCaseConstraintName = "UpperCase";

		private static readonly char[] Separators = new char[] { '[', ']', '(', ')', ',', '\'' };
		#endregion

		#region Private fields
		private string mName;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of the <see cref="Constraint"/>, as it is used in a constraint string.
		/// </summary>
		public string Name
		{
			get { return mName; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Constraint"/> class with the specified name.
		/// </summary>
		/// <param name="name">The name of the <see cref="Constraint"/>, as it is used in a constraint string.</param>
		protected Constraint(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x02), nameof(name));
			}

			mName = name;
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="Constraint"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="System.ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The constraint could not be deserialized correctly.</exception>
		protected Constraint(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			if (info == null)
			{
				throw new CodedArgumentNullException(nameof(info));
			}

			mName = info.GetString(nameof(Name));
		}
#endif
		#endregion

		#region Public methods
		#region Validate
		/// <summary>
		/// Checks that the provided value is within the bounds of the constraint.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <param name="dataType">The data type of the value.</param>
		/// <param name="memberName">The name of the property or field that is validated.</param>
		/// <exception cref="ParameterValidationException">The value is not conform to the constraint.</exception>
		/// <remarks>The default implementation of the method performs no validation.</remarks>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
		public IEnumerable<ParameterValidationResult> Validate(object value, ParameterDataType dataType, string memberName)
		{
			return Validate(value, dataType, memberName, memberName);
		}

		/// <summary>
		/// Checks that the provided value is within the bounds of the constraint.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <param name="dataType">The data type of the value.</param>
		/// <param name="memberName">The name of the property or field that is validated.</param>
		/// <param name="displayName">The (localized) display name of the property or field that is validated. May be <see langword="null"/>.</param>
		/// <exception cref="ParameterValidationException">The value is not conform to the constraint.</exception>
		/// <remarks>The default implementation of the method performs no validation.</remarks>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
		public IEnumerable<ParameterValidationResult> Validate(object value, ParameterDataType dataType, string memberName, string displayName)
		{
			if (dataType == ParameterDataType.None)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x03), nameof(dataType), Properties.Resources.Global_ParameterDataType_None);
			}

			if (string.IsNullOrWhiteSpace(memberName))
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x94), nameof(memberName));
			}
			if (displayName == null)
			{
				displayName = memberName;
			}

			List<ParameterValidationResult> ReturnValue = new List<ParameterValidationResult>();
			OnValidation(ReturnValue, value, dataType, memberName, displayName);
			return ReturnValue;
		}
		#endregion

		#region ToString
		/// <summary>
		/// Returns a string that represents the current <see cref="Constraint"/>.
		/// </summary>
		/// <returns>A string formatted [Constraint] or [Constraint(Parameters...)].</returns>
		public override string ToString()
		{
			List<string> Parameters = new List<string>();
			GetParameters(Parameters);
			List<string> EscapedParameters = null;
			if (Parameters.Count > 0)
			{
				EscapedParameters = new List<string>();
				foreach (string param in Parameters)
				{
					if (param.IndexOfAny(Separators) > -1)
					{
						EscapedParameters.Add('\'' + param.Replace("'", "''") + '\'');
					}
					else
					{
						EscapedParameters.Add(param);
					}
				}
			}

			if (EscapedParameters != null && EscapedParameters.Count > 0)
			{
				return string.Format("[{0}({1})]", mName, string.Join(",", EscapedParameters));
			}
			else
			{
				return string.Format("[{0}]", mName);
			}
		}
		#endregion

#if WINDOWS_DESKTOP
		#region GetObjectData
		/// <summary>
		/// Sets the <see cref="System.Runtime.Serialization.SerializationInfo"/> with information about the <see cref="Constraint"/>.
		/// </summary>
		/// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data of the <see cref="Constraint"/>.</param>
		/// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue(nameof(Name), mName);
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
		protected virtual void GetParameters(IList<string> parameters)
		{
			if (parameters == null)
				throw new CodedArgumentNullException(Errors.CreateHResult(0x95), nameof(parameters));
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
		protected virtual void SetParameters(IReadOnlyList<string> parameters, ParameterDataType dataType)
		{
			if (parameters == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x05), nameof(parameters));
			}

			if (dataType == ParameterDataType.None)
			{
				throw new CodedArgumentOutOfRangeException(Errors.CreateHResult(0x04), nameof(dataType), Properties.Resources.Global_ParameterDataType_None);
			}
		}
		#endregion

		#region AssertDataType
		/// <summary>
		/// Checks if the specified data type is supported by the <see cref="Constraint"/>.
		/// </summary>
		/// <param name="dataType">The data type to check.</param>
		/// <param name="expectedType">The data type supported by the <see cref="Constraint"/>.</param>
		/// <exception cref="ParameterValidationException"><paramref name="dataType"/> is not supported by the <see cref="Constraint"/>.</exception>
		protected void AssertDataType(ParameterDataType dataType, ParameterDataType expectedType)
		{
			if (dataType == expectedType)
				return;
			throw new InvalidDataTypeException(Errors.CreateHResult(0x06), string.Format(Properties.Resources.Constraint_AssertDataType_Failed, dataType, mName), this);
		}

		/// <summary>
		/// Checks if the specified data type is supported by the <see cref="Constraint"/>.
		/// </summary>
		/// <param name="dataType">The data type to check.</param>
		/// <param name="expectedTypes">The data types supported by the <see cref="Constraint"/>.</param>
		/// <exception cref="ParameterValidationException"><paramref name="dataType"/> is not supported by the <see cref="Constraint"/>.</exception>
		protected void AssertDataType(ParameterDataType dataType, params ParameterDataType[] expectedTypes)
		{
			foreach (ParameterDataType expectedType in expectedTypes)
			{
				if (dataType == expectedType)
					return;
			}
			throw new InvalidDataTypeException(Errors.CreateHResult(0x06), string.Format(Properties.Resources.Constraint_AssertDataType_Failed, dataType, mName), this);
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
		protected abstract void OnValidation(IList<ParameterValidationResult> results, object value, ParameterDataType dataType, string memberName, string displayName);
		#endregion
		#endregion

		#region Internal methods
		#region SetParametersInternal
		/// <summary>
		/// Sets the parameters that the <see cref="Constraint"/> requires to work.
		/// </summary>
		/// <param name="parameters">A enumeration of string parameters.</param>
		/// <param name="dataType">The data type that the constraint needs to restrict.</param>
		/// <exception cref="CodedArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
		/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
		internal void SetParametersInternal(IReadOnlyList<string> parameters, ParameterDataType dataType)
		{
			SetParameters(parameters, dataType);
		}
		#endregion
		#endregion
	}
}
