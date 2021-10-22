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

using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NerdyDuck.ParameterValidation;

/// <summary>
/// Represents a container for the result of a validation using <see cref="Constraint"/>s.
/// </summary>
[Serializable]
public class ParameterValidationResult : ValidationResult, ISerializable
{
	private static readonly Lazy<ParameterValidationResult> _success = new(() => new ParameterValidationResult(ParameterValidation.HResult.Create(ErrorCodes.ParameterValidationResult_Success), TextResources.ParameterValidationResult_Success, null));

	/// <summary>
	/// Gets a <see cref="ParameterValidationResult"/> that specifies a successful validation.
	/// </summary>
	/// <value>A <see cref="ParameterValidationResult"/>.</value>
	public static new ParameterValidationResult Success => _success.Value;

	/// <summary>
	/// Gets the <see cref="Constraint"/> that raised the validation error.
	/// </summary>
	/// <value>The <see cref="Constraint"/> that raised the validation error. May be <see langword="null"/>.</value>
	public Constraint? Constraint { get; private set; }

	/// <summary>
	/// Gets an integer value that identifies the validation error.
	/// </summary>
	/// <value>A custom integer identifier.</value>
	public int HResult { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ParameterValidationResult"/> class with the specified HRESULT, error message and constraint.
	/// </summary>
	/// <param name="hresult">An integer value that identifies the validation error.</param>
	/// <param name="errorMessage"></param>
	/// <param name="constraint">The <see cref="Constraint"/> that raised the validation error. May be <see langword="null"/>.</param>
	public ParameterValidationResult(int hresult, string errorMessage, Constraint? constraint)
		: base(errorMessage)
	{
		HResult = hresult;
		Constraint = constraint;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ParameterValidationResult"/> by using another <see cref="ParameterValidationResult"/> object.
	/// </summary>
	/// <param name="validationResult">The validation result object to copy.</param>
	public ParameterValidationResult(ParameterValidationResult validationResult)
		: base(validationResult)
	{
		if (validationResult == null)
		{
			throw new CodedArgumentNullException(ParameterValidation.HResult.Create(ErrorCodes.ParameterValidationResult_ctor_ResultNull), nameof(validationResult));
		}
		HResult = validationResult.HResult;
		Constraint = validationResult.Constraint;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ParameterValidationResult"/> class with the specified HRESULT, error message, member names and constraint.
	/// </summary>
	/// <param name="hresult">An integer value that identifies the validation error.</param>
	/// <param name="errorMessage"></param>
	/// <param name="memberNames">The list of member names that have validation errors.</param>
	/// <param name="constraint">The <see cref="Constraint"/> that raised the validation error. May be <see langword="null"/>.</param>
	public ParameterValidationResult(int hresult, string errorMessage, IEnumerable<string> memberNames, Constraint? constraint)
		: base(errorMessage, memberNames)
	{
		HResult = hresult;
		Constraint = constraint;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ParameterValidationResult"/> class with the specified HRESULT, error message, member name and constraint.
	/// </summary>
	/// <param name="hresult">An integer value that identifies the validation error.</param>
	/// <param name="errorMessage"></param>
	/// <param name="memberName">The name of a member that has validation errors.</param>
	/// <param name="constraint">The <see cref="Constraint"/> that raised the validation error. May be <see langword="null"/>.</param>
	public ParameterValidationResult(int hresult, string errorMessage, string memberName, Constraint? constraint)
		: base(errorMessage, new string[] { memberName })
	{
		HResult = hresult;
		Constraint = constraint;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ParameterValidationResult"/> class with serialized data.
	/// </summary>
	/// <param name="info">The object that holds the serialized object data.</param>
	/// <param name="context">The contextual information about the source or destination.</param>
	/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
	/// <exception cref="SerializationException">The exception could not be deserialized correctly.</exception>
	protected ParameterValidationResult(SerializationInfo info, StreamingContext context)
		: base(info.GetString(nameof(ErrorMessage)), (List<string>)info.GetValue(nameof(MemberNames), typeof(List<string>)))
	{
		Constraint = (Constraint)info.GetValue(nameof(Constraint), typeof(Constraint));
		HResult = info.GetInt32(nameof(HResult));
	}

	/// <summary>
	/// Sets the <see cref="SerializationInfo"/> with information about the exception.
	/// </summary>
	/// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (info == null)
		{
			throw new CodedArgumentNullException(ParameterValidation.HResult.Create(ErrorCodes.ParameterValidationResult_GetObjectData_InfoNull), nameof(info));
		}
		info.AddValue(nameof(ErrorMessage), ErrorMessage);
		info.AddValue(nameof(MemberNames), MemberNames.ToList());
		info.AddValue(nameof(Constraint), Constraint);
		info.AddValue(nameof(HResult), HResult);
	}
}
