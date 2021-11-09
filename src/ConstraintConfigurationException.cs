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
/// The exception that is thrown when the configuration or parametrization of a <see cref="Constraint"/> fails.
/// </summary>
[Serializable]
[CodedException]
public class ConstraintConfigurationException : CodedFormatException
{
	/// <summary>
	/// Gets the <see cref="Constraint"/> that failed to configure.
	/// </summary>
	/// <value>An object derived from <see cref="ParameterValidation.Constraint"/>, or <see langword="null"/>.</value>
	public Constraint? Constraint { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ConstraintConfigurationException"/> class.
	/// </summary>
	/// <remarks>This constructor initializes the <see cref="Exception.Message"/> property of the new instance to a system-supplied message that describes the error, such as "Parameter validation failed." This message takes into account the current system culture.</remarks>
	public ConstraintConfigurationException()
		: base(TextResources.ConstraintConfigurationException_Message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ConstraintConfigurationException"/> class with a specified error message.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	/// <remarks>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</remarks>
	public ConstraintConfigurationException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ConstraintConfigurationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
	/// <remarks>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</remarks>
	public ConstraintConfigurationException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ConstraintConfigurationException"/> class with serialized data.
	/// </summary>
	/// <param name="info">The object that holds the serialized object data.</param>
	/// <param name="context">The contextual information about the source or destination.</param>
	/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
	/// <exception cref="SerializationException">The exception could not be deserialized correctly.</exception>
	protected ConstraintConfigurationException(SerializationInfo info, StreamingContext context)
		: base(info, context) => Constraint = (Constraint?)info.GetValue(nameof(Constraint), typeof(Constraint));

	/// <summary>
	/// Initializes a new instance of the <see cref="ConstraintConfigurationException"/> class with a specified HRESULT value.
	/// </summary>
	/// <param name="hresult">The HRESULT that describes the error.</param>
	/// <remarks><para>This constructor initializes the Message property of the new instance to a system-supplied message that describes the error, such as "Parameter validation failed." This message takes into account the current system culture.</para>
	/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
	/// more information about the definition of HRESULT values.</para></remarks>
	public ConstraintConfigurationException(int hresult)
		: base(hresult, TextResources.ConstraintConfigurationException_Message) => Constraint = null;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConstraintConfigurationException"/> class with a specified HRESULT value and error message.
	/// </summary>
	/// <param name="hresult">The HRESULT that describes the error.</param>
	/// <param name="message">The message that describes the error.</param>
	/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
	/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
	/// more information about the definition of HRESULT values.</para></remarks>
	public ConstraintConfigurationException(int hresult, string message)
		: base(hresult, message) => Constraint = null;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConstraintConfigurationException"/> class with a specified HRESULT value, error message and a reference to the inner exception that is the cause of this exception.
	/// </summary>
	/// <param name="hresult">The HRESULT that describes the error.</param>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
	/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
	/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
	/// more information about the definition of HRESULT values.</para></remarks>
	public ConstraintConfigurationException(int hresult, string message, Exception innerException)
		: base(hresult, message, innerException) => Constraint = null;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConstraintConfigurationException"/> class with a specified HRESULT value and constraint.
	/// </summary>
	/// <param name="hresult">The HRESULT that describes the error.</param>
	/// <param name="constraint">The <see cref="Constraint"/> that failed to configure.</param>
	/// <remarks><para>This constructor initializes the Message property of the new instance to a system-supplied message that describes the error, such as "Parameter validation failed." This message takes into account the current system culture.</para>
	/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
	/// more information about the definition of HRESULT values.</para></remarks>
	public ConstraintConfigurationException(int hresult, Constraint constraint)
		: base(hresult, TextResources.ConstraintConfigurationException_Message) => Constraint = constraint;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConstraintConfigurationException"/> class with a specified HRESULT value, error message and constraint.
	/// </summary>
	/// <param name="hresult">The HRESULT that describes the error.</param>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="constraint">The <see cref="Constraint"/> that failed to configure.</param>
	/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
	/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
	/// more information about the definition of HRESULT values.</para></remarks>
	public ConstraintConfigurationException(int hresult, string message, Constraint constraint)
		: base(hresult, message) => Constraint = constraint;

	/// <summary>
	/// Initializes a new instance of the <see cref="ConstraintConfigurationException"/> class with a specified HRESULT value, error message, constraint and a reference to the inner exception that is the cause of this exception.
	/// </summary>
	/// <param name="hresult">The HRESULT that describes the error.</param>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="constraint">The <see cref="Constraint"/> that failed to configure.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
	/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
	/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
	/// more information about the definition of HRESULT values.</para></remarks>
	public ConstraintConfigurationException(int hresult, string message, Constraint constraint, Exception innerException)
		: base(hresult, message, innerException) => Constraint = constraint;

	/// <summary>
	/// Sets the <see cref="SerializationInfo"/> with information about the exception.
	/// </summary>
	/// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue(nameof(Constraint), Constraint);
	}
}
