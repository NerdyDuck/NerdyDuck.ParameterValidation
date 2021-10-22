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
/// Provides data for events occurring during a parameter validation.
/// </summary>
public class ParameterValidationEventArgs : EventArgs
{
	/// <summary>
	/// Gets the value that is currently validated.
	/// </summary>
	/// <value>The value to validate.</value>
	public object? Value { get; private set; }

	/// <summary>
	/// Gets the data type of <see cref="Value"/>.
	/// </summary>
	/// <value>One of the <see cref="ParameterDataType"/> values.</value>
	public ParameterDataType DataType { get; private set; }

	/// <summary>
	/// Gets a list of <see cref="Constraint"/>s that are used to validate <see cref="Value"/>.
	/// </summary>
	/// <value>A list of one or more objects derived from <see cref="Constraint"/>.</value>
	public IReadOnlyList<Constraint>? Constraints { get; private set; }

	/// <summary>
	/// Gets the name of the property or control that contains <see cref="Value"/>.
	/// </summary>
	/// <value>The name of the property or control to validate.</value>
	public string? MemberName { get; private set; }

	/// <summary>
	/// Gets the display name of the control that displays <see cref="Value"/>.
	/// </summary>
	/// <value>the display name of the property or control to validate.</value>
	public string? DisplayName { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ParameterValidationEventArgs"/> class with the specified parameters.
	/// </summary>
	/// <param name="value">The value that is currently validated.</param>
	/// <param name="dataType">The data type of <paramref name="value"/>.</param>
	/// <param name="constraints">A list of <see cref="Constraint"/>s that are used to validate <paramref name="value"/>.</param>
	/// <param name="memberName">The name of the property or control that contains <paramref name="value"/>.</param>
	/// <param name="displayName">The display name of the control that displays <paramref name="value"/>.</param>
	public ParameterValidationEventArgs(object? value, ParameterDataType dataType, IReadOnlyList<Constraint>? constraints, string? memberName, string? displayName)
	{
		Value = value;
		DataType = dataType;
		Constraints = constraints;
		MemberName = memberName;
		DisplayName = displayName;
	}
}
