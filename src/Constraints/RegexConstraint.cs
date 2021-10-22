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

using System.Text.RegularExpressions;

namespace NerdyDuck.ParameterValidation.Constraints;

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
[Serializable]
public class RegexConstraint : Constraint
{
	private Regex _internalRegex;

	/// <summary>
	/// Gets the regular expression to enforce.
	/// </summary>
	/// <value>A valid regular expression.</value>
	public string RegularExpression { get; private set; }

	/// <summary>
	/// Gets the options to apply when evaluating the <see cref="RegularExpression"/>.
	/// </summary>
	/// <value>A combination of the <see cref="RegexOptions"/> values.</value>
	public RegexOptions Options { get; private set; }

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
			throw new CodedArgumentNullOrWhiteSpaceException(HResult.Create(ErrorCodes.RegexConstraint_ctor_StringNullEmpty), nameof(regularExpression));
		}

		RegularExpression = regularExpression;
		Options = options;
		try
		{
			_internalRegex = new Regex(RegularExpression, Options);
		}
		catch (ArgumentOutOfRangeException ex)
		{
			throw new CodedArgumentOutOfRangeException(HResult.Create(ErrorCodes.RegexConstraint_ctor_OptionsInvalid), TextResources.RegexConstraint_OptionsInvalid, ex);
		}
		catch (ArgumentException ex)
		{
			throw new CodedArgumentException(HResult.Create(ErrorCodes.RegexConstraint_ctor_RegexInvalid), TextResources.RegexConstraint_PatternInvalid, nameof(regularExpression), ex);
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="RegexConstraint"/> class with serialized data.
	/// </summary>
	/// <param name="info">The object that holds the serialized object data.</param>
	/// <param name="context">The contextual information about the source or destination.</param>
	/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
	/// <exception cref="SerializationException">The constraint could not be deserialized correctly.</exception>
	protected RegexConstraint(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
		RegularExpression = info.GetString(nameof(RegularExpression));
		Options = (RegexOptions)info.GetValue(nameof(Options), typeof(RegexOptions));
		_internalRegex = new Regex(RegularExpression, Options);
	}

	/// <summary>
	/// Sets the <see cref="SerializationInfo"/> with information about the <see cref="Constraint"/>.
	/// </summary>
	/// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data of the <see cref="Constraint"/>.</param>
	/// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue(nameof(RegularExpression), RegularExpression);
		info.AddValue(nameof(Options), Options);
	}

	/// <summary>
	/// Adds the parameters of the constraint to a list of strings.
	/// </summary>
	/// <param name="parameters">A list of strings to add the parameters to.</param>
	/// <remarks>Override this method, if the constraint makes use of parameters. Add the parameters in the order that they should be provided to <see cref="SetParameters"/>.</remarks>
	protected override void GetParameters(IList<string> parameters)
	{
		base.GetParameters(parameters);
		parameters.Add(RegularExpression);
		if (Options != RegexOptions.None)
		{
			foreach (RegexOptions opts in Enum.GetValues(typeof(RegexOptions)))
			{
				if (Options.HasFlag(opts) && opts != RegexOptions.None)
				{
					parameters.Add(Enum.GetName(typeof(RegexOptions), opts));
				}
			}
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
	protected override void SetParameters(IReadOnlyList<string> parameters, ParameterDataType dataType)
	{
		base.SetParameters(parameters, dataType);
		AssertDataType(dataType, ParameterDataType.String);
		if (parameters.Count < 1)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.RegexConstraint_SetParameters_OneParam), string.Format(CultureInfo.CurrentCulture, TextResources.Global_SetParameters_InvalidMinCount, Name, 1), this);
		}

		if (string.IsNullOrWhiteSpace(parameters[0]))
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.RegexConstraint_SetParameters_RegexInvalid), TextResources.RegexConstraint_PatternInvalid, this);
		}
		RegularExpression = parameters[0];

		Options = RegexOptions.None;
		if (parameters.Count > 1)
		{
			for (int i = 1; i < parameters.Count; i++)
			{
				if (!Enum.TryParse<RegexOptions>(parameters[i], out RegexOptions opts))
				{
					throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.RegexConstraint_SetParameters_OptionsInvalid), TextResources.RegexConstraint_OptionsInvalid, this);
				}
				Options |= opts;
			}
		}

		try
		{

			_internalRegex = new Regex(RegularExpression, Options);
		}
		catch (ArgumentOutOfRangeException ex)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.RegexConstraint_SetParameters_OptionsInvalid), TextResources.RegexConstraint_OptionsInvalid, this, ex);
		}
		catch (ArgumentException ex)
		{
			throw new ConstraintConfigurationException(HResult.Create(ErrorCodes.RegexConstraint_SetParameters_RegexInvalid), TextResources.RegexConstraint_PatternInvalid, this, ex);
		}
	}

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

		if (!_internalRegex.IsMatch((string)value))
		{
			results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.RegexConstraint_Validate_NoMatch), string.Format(CultureInfo.CurrentCulture, TextResources.RegexConstraint_Validate_Failed, displayName, RegularExpression), memberName, this));
		}
	}
}
