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

namespace NerdyDuck.ParameterValidation.Constraints;

/// <summary>
/// A constraint specifying that a string is a valid IP end point, e.g. an IPv4 or IPv6 address, a domain name, or a local machine name, with or without a port number.
/// </summary>
/// <remarks>
/// <para>
/// <list type="bullet">
/// <item>The textual representation of the constraint is <c>[Endpoint]</c>.</item>
/// <item>The constraint is only applicable to the <see cref="ParameterDataType.String"/> data type.</item>
/// <item>If a string parameter with that constraint is empty or contains characters invalid for a host name, or an invalid IP address (checked by <see cref="Uri.CheckHostName(string)"/>), or an invalid port number, a <see cref="ParameterValidationResult"/> is added to the output of <see cref="Constraint.Validate(object, ParameterDataType, string, string)"/>.</item>
/// </list>
/// </para>
/// </remarks>
[Serializable]
public class EndpointConstraint : Constraint
{
	/// <summary>
	/// Initializes a new instance of the <see cref="EndpointConstraint"/> class.
	/// </summary>
	public EndpointConstraint()
		: base(EndpointConstraintName)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="EndpointConstraint"/> class with serialized data.
	/// </summary>
	/// <param name="info">The object that holds the serialized object data.</param>
	/// <param name="context">The contextual information about the source or destination.</param>
	/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
	/// <exception cref="SerializationException">The constraint could not be deserialized correctly.</exception>
	protected EndpointConstraint(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
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

		string Temp = value as string;
		if (string.IsNullOrWhiteSpace(Temp))
		{
			results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.EndpointConstraint_Validate_ValueEmpty), string.Format(CultureInfo.CurrentCulture, TextResources.Global_Validate_StringEmpty, displayName), memberName, this));
		}
		else
		{
#if NETSTD20
			int colonPos = Temp.IndexOf(':');
			if (colonPos > -1)
			{
				string portString = Temp.Substring(colonPos + 1);
				Temp = Temp.Substring(0, colonPos);
#else
			int colonPos = Temp.IndexOf(':', StringComparison.Ordinal);
			if (colonPos > -1)
			{
				string portString = Temp[(colonPos + 1)..];
				Temp = Temp[..colonPos];
#endif
				if (!int.TryParse(portString, out int Port) || Port < 0 || Port > 65535)
				{
					results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.EndpointConstraint_Validate_PortInvalid), string.Format(CultureInfo.CurrentCulture, TextResources.EndpointConstraint_Validate_FailedPort, displayName, Temp), memberName, this));
				}
			}
			if (Uri.CheckHostName(Temp) == UriHostNameType.Unknown)
			{
				results.Add(new ParameterValidationResult(HResult.Create(ErrorCodes.EndpointConstraint_Validate_NotHostOrIP), string.Format(CultureInfo.CurrentCulture, TextResources.EndpointConstraint_Validate_Failed, displayName, Temp), memberName, this));
			}
		}
	}
}
