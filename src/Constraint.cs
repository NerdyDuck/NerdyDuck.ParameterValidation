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

namespace NerdyDuck.ParameterValidation;

/// <summary>
/// The base class for all parameter constraints.
/// </summary>
[Serializable]
public abstract class Constraint : ISerializable
{
	internal const string Namespace = "http://www.nerdyduck.de/ParameterValidation";
	internal const string AllowedSchemeConstraintName = "AllowedScheme";
	internal const string CharacterSetConstraintName = "CharSet";
	internal const string DatabaseConstraintName = "Database";
	internal const string DecimalPlacesConstraintName = "DecimalPlaces";
	internal const string DisplayHintConstraintName = "DisplayHint";
	internal const string EncryptedConstraintName = "Encrypted";
	internal const string EndpointConstraintName = "Endpoint";
	internal const string EnumValuesConstraintName = "Values";
	internal const string FileNameConstraintName = "FileName";
	internal const string HostNameConstraintName = "Host";
	internal const string LengthConstraintName = "Length";
	internal const string LowercaseConstraintName = "Lowercase";
	internal const string MaximumLengthConstraintName = "MaxLength";
	internal const string MaximumValueConstraintName = "MaxValue";
	internal const string MinimumLengthConstraintName = "MinLength";
	internal const string MinimumValueConstraintName = "MinValue";
	internal const string NullConstraintName = "Null";
	internal const string PasswordConstraintName = "Password";
	internal const string PathConstraintName = "Path";
	internal const string ReadOnlyConstraintName = "ReadOnly";
	internal const string RegexConstraintName = "Regex";
	internal const string StepConstraintName = "Step";
	internal const string TypeConstraintName = "Type";
	internal const string UppercaseConstraintName = "Uppercase";

	private static readonly char[] Separators = new char[] { '[', ']', '(', ')', ',', '\'' };

	private readonly string _name;

	/// <summary>
	/// Gets the name of the <see cref="Constraint"/>, as it is used in a constraint string.
	/// </summary>
	/// <value>The textual name of the constraint.</value>
	public string Name => _name;

	/// <summary>
	/// Initializes a new instance of the <see cref="Constraint"/> class with the specified name.
	/// </summary>
	/// <param name="name">The name of the <see cref="Constraint"/>, as it is used in a constraint string.</param>
	/// <exception cref="CodedArgumentNullOrWhiteSpaceException"><paramref name="name"/> is <see langword="null"/> or empty or white-space.</exception>
	protected Constraint(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new CodedArgumentNullOrWhiteSpaceException(HResult.Create(ErrorCodes.Constraint_ctor_ArgNull), nameof(name));
		}

		_name = name;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Constraint"/> class with serialized data.
	/// </summary>
	/// <param name="info">The object that holds the serialized object data.</param>
	/// <param name="context">The contextual information about the source or destination.</param>
	/// <exception cref="CodedArgumentNullException">The <paramref name="info"/> argument is null.</exception>
	/// <exception cref="SerializationException">The constraint could not be deserialized correctly.</exception>
	protected Constraint(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.Constraint_ctor_InfoNull), nameof(info));
		}

		_name = info.GetString(nameof(Name));
	}

	/// <summary>
	/// Checks that the provided value is within the bounds of the constraint.
	/// </summary>
	/// <param name="value">The value to check.</param>
	/// <param name="dataType">The data type of the value.</param>
	/// <param name="memberName">The name of the property or field that is validated.</param>
	/// <remarks>The default implementation of the method performs no validation.</remarks>
	/// <returns>An enumeration of <see cref="ParameterValidationResult"/>s. The enumeration is be empty if the <paramref name="value"/> is within the constraint's boundaries.</returns>
	/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
	/// <exception cref="CodedArgumentNullOrWhiteSpaceException"><paramref name="memberName"/> is <see langword="null"/> or empty or white-space.</exception>
	/// <exception cref="InvalidDataTypeException"><paramref name="dataType"/> is not supported by the <see cref="Constraint"/>.</exception>
	public IEnumerable<ParameterValidationResult> Validate(object? value, ParameterDataType dataType, string memberName) => Validate(value, dataType, memberName, memberName);

	/// <summary>
	/// Checks that the provided value is within the bounds of the constraint.
	/// </summary>
	/// <param name="value">The value to check.</param>
	/// <param name="dataType">The data type of the value.</param>
	/// <param name="memberName">The name of the property or field that is validated.</param>
	/// <param name="displayName">The (localized) display name of the property or field that is validated. May be <see langword="null"/>.</param>
	/// <remarks>The default implementation of the method performs no validation.</remarks>
	/// <returns>An enumeration of <see cref="ParameterValidationResult"/>s. The enumeration is be empty if the <paramref name="value"/> is within the constraint's boundaries.</returns>
	/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
	/// <exception cref="CodedArgumentNullOrWhiteSpaceException"><paramref name="memberName"/> is <see langword="null"/> or empty or white-space.</exception>
	/// <exception cref="InvalidDataTypeException"><paramref name="dataType"/> is not supported by the <see cref="Constraint"/>.</exception>
	public IEnumerable<ParameterValidationResult> Validate(object? value, ParameterDataType dataType, string memberName, string displayName)
	{
		if (dataType == ParameterDataType.None)
		{
			throw new CodedArgumentOutOfRangeException(HResult.Create(ErrorCodes.Constraint_Validate_TypeNone), nameof(dataType), TextResources.Global_ParameterDataType_None);
		}

		if (string.IsNullOrWhiteSpace(memberName))
		{
			throw new CodedArgumentNullOrWhiteSpaceException(HResult.Create(ErrorCodes.Constraint_Validate_NameNullEmpty), nameof(memberName));
		}
		if (displayName == null)
		{
			displayName = memberName;
		}

		List<ParameterValidationResult> ReturnValue = new();
		OnValidation(ReturnValue, value, dataType, memberName, displayName);
		return ReturnValue;
	}

	/// <summary>
	/// Returns a string that represents the current <see cref="Constraint"/>.
	/// </summary>
	/// <returns>A string formatted [Constraint] or [Constraint(Parameters...)].</returns>
	public override string ToString()
	{
		List<string> Parameters = new();
		GetParameters(Parameters);
		List<string> EscapedParameters = null;
		if (Parameters.Count > 0)
		{
			EscapedParameters = new List<string>();
			foreach (string param in Parameters)
			{
				if (param.IndexOfAny(Separators) > -1)
				{
#if NETSTD20
					EscapedParameters.Add('\'' + param.Replace("'", "''") + '\'');
#else
					EscapedParameters.Add('\'' + param.Replace("'", "''", StringComparison.Ordinal) + '\'');
#endif
				}
				else
				{
					EscapedParameters.Add(param);
				}
			}
		}

		return EscapedParameters != null && EscapedParameters.Count > 0
			? string.Format(CultureInfo.InvariantCulture, "[{0}({1})]", _name, string.Join(",", EscapedParameters))
			: string.Format(CultureInfo.InvariantCulture, "[{0}]", _name);
	}

	/// <summary>
	/// Sets the <see cref="SerializationInfo"/> with information about the <see cref="Constraint"/>.
	/// </summary>
	/// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data of the <see cref="Constraint"/>.</param>
	/// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.Constraint_GetObjectData_InfoNull), nameof(info));
		}
		info.AddValue(nameof(Name), _name);
	}

	/// <summary>
	/// Adds the parameters of the constraint to a list of strings.
	/// </summary>
	/// <param name="parameters">A list of strings to add the parameters to.</param>
	/// <remarks>Override this method, if the constraint makes use of parameters. Add the parameters in the order that they should be provided to <see cref="SetParameters"/>.</remarks>
	/// <exception cref="CodedArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
	protected virtual void GetParameters(IList<string> parameters)
	{
		if (parameters == null)
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.Constraint_GetParameters_ArgNull), nameof(parameters));
	}

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
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.Constraint_SetParameters_ParametersNull), nameof(parameters));
		}

		if (dataType == ParameterDataType.None)
		{
			throw new CodedArgumentOutOfRangeException(HResult.Create(ErrorCodes.Constraint_SetParameters_TypeNone), nameof(dataType), TextResources.Global_ParameterDataType_None);
		}
	}

	/// <summary>
	/// Checks if the specified data type is supported by the <see cref="Constraint"/>.
	/// </summary>
	/// <param name="dataType">The data type to check.</param>
	/// <param name="expectedType">The data type supported by the <see cref="Constraint"/>.</param>
	/// <exception cref="InvalidDataTypeException"><paramref name="dataType"/> is not supported by the <see cref="Constraint"/>.</exception>
	protected void AssertDataType(ParameterDataType dataType, ParameterDataType expectedType)
	{
		if (dataType == expectedType)
			return;
		throw new InvalidDataTypeException(HResult.Create(ErrorCodes.Constraint_AssertDataType_TypeNotSupported), this, dataType);
	}

	/// <summary>
	/// Checks if the specified data type is supported by the <see cref="Constraint"/>.
	/// </summary>
	/// <param name="dataType">The data type to check.</param>
	/// <param name="expectedTypes">The data types supported by the <see cref="Constraint"/>.</param>
	/// <exception cref="InvalidDataTypeException"><paramref name="dataType"/> is not supported by the <see cref="Constraint"/>.</exception>
	protected void AssertDataType(ParameterDataType dataType, params ParameterDataType[] expectedTypes)
	{
		if (expectedTypes == null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.Constraint_AssertDataType_TypesNull), nameof(expectedTypes));
		}
		foreach (ParameterDataType expectedType in expectedTypes)
		{
			if (dataType == expectedType)
				return;
		}
		throw new InvalidDataTypeException(HResult.Create(ErrorCodes.Constraint_AssertDataType_TypeNotSupported), this, dataType);
	}

	/// <summary>
	/// When overridden by a deriving class, checks that the provided value is within the bounds of the constraint.
	/// </summary>
	/// <param name="results">A list that of <see cref="ParameterValidationResult"/>s; when the method returns, it contains the validation errors generated by the method.</param>
	/// <param name="value">The value to check.</param>
	/// <param name="dataType">The data type of the value.</param>
	/// <param name="memberName">The name of the property or field that is validated.</param>
	/// <param name="displayName">The (localized) display name of the property or field that is validated. May be <see langword="null"/>.</param>
	protected virtual void OnValidation(IList<ParameterValidationResult> results, object? value, ParameterDataType dataType, string memberName, string displayName)
	{
		if (results == null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.Constraint_OnValidation_ResultsNull), nameof(results));
		}
		if (value == null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.Constraint_OnValidation_ValueNull), nameof(value));
		}
	}

	/// <summary>
	/// Sets the parameters that the <see cref="Constraint"/> requires to work.
	/// </summary>
	/// <param name="parameters">A enumeration of string parameters.</param>
	/// <param name="dataType">The data type that the constraint needs to restrict.</param>
	/// <exception cref="CodedArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
	/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
	internal void SetParametersInternal(IReadOnlyList<string> parameters, ParameterDataType dataType) => SetParameters(parameters, dataType);
}
