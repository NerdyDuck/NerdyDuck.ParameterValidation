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

using NerdyDuck.CodedExceptions;
using System;

namespace NerdyDuck.ParameterValidation
{
	/// <summary>
	/// The exception that is thrown when the validation of a parameter value fails.
	/// </summary>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	[CodedException]
	public class InvalidDataTypeException : CodedFormatException
	{
		#region Private fields
		private Constraint mConstraint;
		private ParameterDataType mDataType;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the <see cref="Constraint"/> that caused the exception.
		/// </summary>
		/// <value>A object derived from the <see cref="Constraint"/> class.</value>
		public Constraint Constraint
		{
			get { return mConstraint; }
		}

		/// <summary>
		/// Gets the data type that caused the exception.
		/// </summary>
		/// <value>One of the <see cref="ParameterDataType"/> values.</value>
		public ParameterDataType DataType
		{
			get { return mDataType; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidDataTypeException"/> class.
		/// </summary>
		/// <remarks>This constructor initializes the <see cref="Exception.Message"/> property of the new instance to a system-supplied message that describes the error, such as "The data type is not supported." This message takes into account the current system culture.</remarks>
		public InvalidDataTypeException()
			: base(Properties.Resources.InvalidDataTypeException_MessageDefault)
		{
			mConstraint = null;
			mDataType = ParameterDataType.None;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidDataTypeException"/> class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <remarks>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</remarks>
		public InvalidDataTypeException(string message)
			: base(message)
		{
			mConstraint = null;
			mDataType = ParameterDataType.None;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidDataTypeException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
		/// <remarks>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</remarks>
		public InvalidDataTypeException(string message, Exception innerException)
			: base(message, innerException)
		{
			mConstraint = null;
			mDataType = ParameterDataType.None;
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidDataTypeException"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The exception could not be deserialized correctly.</exception>
		protected InvalidDataTypeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			mConstraint = (Constraint)info.GetValue(nameof(Constraint), typeof(Constraint));
			mDataType = (ParameterDataType)info.GetValue(nameof(DataType), typeof(ParameterDataType));
		}
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidDataTypeException"/> class with a specified HRESULT value.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <remarks><para>This constructor initializes the Message property of the new instance to a system-supplied message that describes the error, such as "The data type is not supported." This message takes into account the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public InvalidDataTypeException(int hresult)
			: base(hresult, Properties.Resources.InvalidDataTypeException_MessageDefault)
		{
			mConstraint = null;
			mDataType = ParameterDataType.None;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidDataTypeException"/> class with a specified HRESULT value and error message.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <param name="message">The message that describes the error.</param>
		/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public InvalidDataTypeException(int hresult, string message)
			: base(hresult, message)
		{
			mConstraint = null;
			mDataType = ParameterDataType.None;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidDataTypeException"/> class with a specified HRESULT value, error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
		/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public InvalidDataTypeException(int hresult, string message, Exception innerException)
			: base(hresult, message, innerException)
		{
			mConstraint = null;
			mDataType = ParameterDataType.None;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidDataTypeException"/> class with a specified HRESULT value, constraint and data type.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <param name="constraint">The constraint that caused the exception.</param>
		/// <param name="dataType">The data type that caused the exception.</param>
		/// <remarks><para>This constructor initializes the Message property of the new instance to a system-supplied message that describes the error, such as "The data type is not supported." This message takes into account the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public InvalidDataTypeException(int hresult, Constraint constraint, ParameterDataType dataType)
			: base(hresult, CreateMessage(constraint, dataType))
		{
			mConstraint = constraint;
			mDataType = dataType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidDataTypeException"/> class with a specified HRESULT value, error message, constraint and data type.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="constraint">The constraint that caused the exception.</param>
		/// <param name="dataType">The data type that caused the exception.</param>
		/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public InvalidDataTypeException(int hresult, string message, Constraint constraint, ParameterDataType dataType)
			: base(hresult, message)
		{
			mConstraint = constraint;
			mDataType = dataType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InvalidDataTypeException"/> class with a specified HRESULT value, error message, constraint, data type and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="constraint">The constraint that caused the exception.</param>
		/// <param name="dataType">The data type that caused the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
		/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public InvalidDataTypeException(int hresult, string message, Constraint constraint, ParameterDataType dataType, Exception innerException)
			: base(hresult, message, innerException)
		{
			mConstraint = constraint;
			mDataType = dataType;
		}
		#endregion

		#region Public methods
#if WINDOWS_DESKTOP
		/// <summary>
		/// Sets the <see cref="System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
		/// </summary>
		/// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(nameof(Constraint), mConstraint);
			info.AddValue(nameof(DataType), mDataType);
		}
#endif
		#endregion

		#region Private methods
		/// <summary>
		/// Creates an error message using the constraint name and data type.
		/// </summary>
		/// <param name="constraint">The constraint that caused the exception.</param>
		/// <param name="dataType">The data type that caused the exception.</param>
		/// <returns>An error message.</returns>
		private static string CreateMessage(Constraint constraint, ParameterDataType dataType)
		{
			if (constraint == null)
				return Properties.Resources.InvalidDataTypeException_MessageDefault;

			return string.Format(Properties.Resources.InvalidDataTypeException_Message, constraint.Name, dataType);
		}
		#endregion
	}
}
