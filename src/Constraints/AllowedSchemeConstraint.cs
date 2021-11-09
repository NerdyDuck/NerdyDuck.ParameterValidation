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

namespace NerdyDuck.ParameterValidation.Constraints;

/// <summary>
/// A constraint specifying the allowed schemes for a <see cref="Uri"/>.
/// </summary>
/// <remarks>
/// <para>
/// <list type="bullet">
/// <item>The textual representation of the constraint is <c>[AllowedScheme(scheme[,scheme,...])]</c>. <c>scheme</c> can be any string, and can occur more than once.</item>
/// <item>The constraint is only applicable to the <see cref="ParameterDataType.Uri"/> data type.</item>
/// <item>If a <see cref="Uri"/> parameter with that constraint uses a scheme that is not allowed, a <see cref="ParameterValidationResult"/> is added to the output of <see cref="Constraint.Validate(object, ParameterDataType, string, string)"/>.</item>
/// </list>
/// </para>
/// </remarks>
[Serializable]
public class AllowedSchemeConstraint : Constraint
{
	private List<string> _allowedSchemes;

	/// <summary>
	/// Gets a collection of allowed <see cref="Uri"/> schemes.
	/// </summary>
	/// <value>One or more URI scheme strings.</value>
	public IList<string> AllowedSchemes => _allowedSchemes;

	/// <summary>
	/// Initializes a new instance of the <see cref="AllowedSchemeConstraint"/> class.
	/// </summary>
	public AllowedSchemeConstraint()
			: base(AllowedSchemeConstraintName) => _allowedSchemes = new List<string>();

	/// <summary>
	/// Initializes a new instance of the <see cref="AllowedSchemeConstraint"/> class with the specified <see cref="Uri"/> scheme.
	/// </summary>
	/// <param name="scheme">The allowed scheme.</param>
	/// <exception cref="CodedArgumentNullException"><paramref name="scheme"/> is <see langword="null"/>, empty or white-space.</exception>
	public AllowedSchemeConstraint(string scheme)
			: base(AllowedSchemeConstraintName)
	{
		if (string.IsNullOrWhiteSpace(scheme))
		{
			throw new CodedArgumentNullOrWhiteSpaceException(HResult.Create(ErrorCodes.AllowedSchemeConstraint_ctor_StringNullEmpty), nameof(scheme));
		}

		_allowedSchemes = new List<string> { scheme };
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AllowedSchemeConstraint"/> class with a collection of <see cref="Uri"/> schemes.
	/// </summary>
	/// <param name="schemes">A collection of allowed schemes.</param>
	public AllowedSchemeConstraint(IEnumerable<string> schemes)
		: base(AllowedSchemeConstraintName)
	{
		if (schemes == null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.AllowedSchemeConstraint_ctor_EnumNull), nameof(schemes));
		}

		List<string> Temp = new(schemes);
		CheckSchemes(Temp);
		_allowedSchemes = Temp;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AllowedSchemeConstraint"/> class with serialized data.
	/// </summary>
	/// <param name="info">The object that holds the serialized object data.</param>
	/// <param name="context">The contextual information about the source or destination.</param>
	/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
	/// <exception cref="SerializationException">The constraint could not be deserialized correctly.</exception>
	protected AllowedSchemeConstraint(SerializationInfo info, StreamingContext context)
		: base(info, context) => _allowedSchemes = (List<string>)(info.GetValue(nameof(AllowedSchemes), typeof(List<string>)) ?? throw new CodedSerializationException(HResult.Create(ErrorCodes.AllowedSchemeConstraint_ctor_AllowedSchemesNull), TextResources.AllowedSchemeConstraint_ctor_AllowedSchemesNull));

	/// <summary>
	/// Sets the <see cref="SerializationInfo"/> with information about the <see cref="Constraint"/>.
	/// </summary>
	/// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data of the <see cref="Constraint"/>.</param>
	/// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
#if NETSTD20
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
#else
	public override void GetObjectData([System.Diagnostics.CodeAnalysis.NotNull] SerializationInfo info, StreamingContext context)
#endif
	{
		base.GetObjectData(info, context);
		info.AddValue(nameof(AllowedSchemes), _allowedSchemes);
	}

	/// <summary>
	/// Adds the parameters of the constraint to a list of strings.
	/// </summary>
	/// <param name="parameters">A list of strings to add the parameters to.</param>
	/// <remarks>Override this method, if the constraint makes use of parameters. Add the parameters in the order that they should be provided to <see cref="SetParameters"/>.</remarks>
#if NETSTD20
	protected override void GetParameters(IList<string> parameters)
#else
	protected override void GetParameters([System.Diagnostics.CodeAnalysis.NotNull] IList<string> parameters)
#endif
	{
		base.GetParameters(parameters);
		foreach (string scheme in _allowedSchemes)
		{
			parameters.Add(scheme);
		}
	}

	/// <summary>
	/// Sets the parameters that the <see cref="Constraint"/> requires to work.
	/// </summary>
	/// <param name="parameters">A enumeration of string parameters.</param>
	/// <param name="dataType">The data type that the constraint needs to restrict.</param>
	/// <exception cref="CodedArgumentNullException"><paramref name="parameters"/> is <see langword="null"/>.</exception>
	/// <exception cref="CodedArgumentOutOfRangeException"><paramref name="dataType"/> is <see cref="ParameterDataType.None"/>.</exception>
	/// <exception cref="ConstraintConfigurationException"><paramref name="parameters"/> contains no elements, or an invalid element.</exception>
#if NETSTD20
	protected override void SetParameters(IReadOnlyList<string> parameters, ParameterDataType dataType)
#else
	protected override void SetParameters([System.Diagnostics.CodeAnalysis.NotNull] IReadOnlyList<string> parameters, ParameterDataType dataType)
#endif
	{
		base.SetParameters(parameters, dataType);
		AssertDataType(dataType, ParameterDataType.Uri);
		CheckSchemes(parameters);
		_allowedSchemes = new List<string>(parameters);
	}

	/// <summary>
	/// When implemented by a deriving class, checks that the provided value is within the bounds of the constraint.
	/// </summary>
	/// <param name="results">A list that of <see cref="ParameterValidationResult"/>s; when the method returns, it contains the validation errors generated by the method.</param>
	/// <param name="value">The value to check.</param>
	/// <param name="dataType">The data type of the value.</param>
	/// <param name="memberName">The name of the property or field that is validated.</param>
	/// <param name="displayName">The (localized) display name of the property or field that is validated. May be <see langword="null"/>.</param>
#if NETSTD20
	protected override void OnValidation(IList<ParameterValidationResult> results, object value, ParameterDataType dataType, string memberName, string displayName)
#else
	protected override void OnValidation([System.Diagnostics.CodeAnalysis.NotNull] IList<ParameterValidationResult> results, [System.Diagnostics.CodeAnalysis.NotNull] object value, ParameterDataType dataType, [System.Diagnostics.CodeAnalysis.NotNull] string memberName, string? displayName)
#endif
	{
		base.OnValidation(results, value, dataType, memberName, displayName);
		AssertDataType(dataType, ParameterDataType.Uri);

		if (_allowedSchemes == null)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.AllowedSchemeConstraint_CheckSchemes_SchemeNullEmpty), TextResources.AllowedSchemeConstraint_Validate_NotConfigured, this);
		}
		string scheme = ((Uri)value).Scheme;
		bool schemeFound = false;
		foreach (string lscheme in _allowedSchemes)
		{
			if (string.Equals(scheme, lscheme, StringComparison.OrdinalIgnoreCase))
			{
				schemeFound = true;
				break;
			}
		}
		if (!schemeFound)
		{
			results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.AllowedSchemeConstraint_Validate_SchemeNotAllowed), string.Format(CultureInfo.CurrentCulture, TextResources.AllowedSchemeConstraint_Validate_Failed, displayName, scheme, ConcatSchemes()), memberName, this));
		}
	}

	/// <summary>
	/// Checks that the specified list of schemes is not empty, and does not contain null or whitespace values.
	/// </summary>
	/// <param name="schemes">A list of schemes.</param>
	private static void CheckSchemes(IReadOnlyList<string> schemes)
	{
		if (schemes.Count == 0)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.AllowedSchemeConstraint_CheckSchemes_NoSchemes), TextResources.AllowedSchemeConstraint_CheckSchemes_NoScheme);
		}
		foreach (string scheme in schemes)
		{
			if (string.IsNullOrWhiteSpace(scheme))
			{
				throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.AllowedSchemeConstraint_CheckSchemes_SchemeNullEmpty), TextResources.AllowedSchemeConstraint_CheckSchemes_InvalidScheme);
			}
		}

	}

	/// <summary>
	/// Concatenates the schemes in AllowedSchemes, separating the values with a comma.
	/// </summary>
	/// <returns>A string containing all allowed schemes.</returns>
	private string ConcatSchemes()
	{
		string ReturnValue = string.Empty;
		bool AddComma = false;
		foreach (string scheme in _allowedSchemes)
		{
			if (AddComma)
			{
				ReturnValue += ",";
			}
			else
			{
				AddComma = true;
			}
			ReturnValue += scheme;
		}

		return ReturnValue;
	}
}
