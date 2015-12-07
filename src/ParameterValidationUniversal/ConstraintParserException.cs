#region Copyright
/*******************************************************************************
 * <copyright file="ConstraintParserException.cs" owner="Daniel Kopp">
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
 * <file name="ConstraintParserException.cs" date="2015-10-19">
 * The exception that is thrown when the parsing of a string of Constraints
 * fails.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;

namespace NerdyDuck.ParameterValidation
{
	/// <summary>
	/// The exception that is thrown when the parsing of a string of <see cref="Constraint"/>s fails.
	/// </summary>
#if WINDOWS_DESKTOP
	[Serializable]
#endif
	[CodedException]
	public class ConstraintParserException : CodedFormatException
	{
		#region Private fields
		private int mPosition;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the position of the constraint in the string that contains a syntactical error.
		/// </summary>
		/// <value>If the value is -1, then no specific location is given.</value>
		public int Position
		{
			get { return mPosition; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ConstraintParserException"/> class.
		/// </summary>
		/// <remarks>This constructor initializes the <see cref="Exception.Message"/> property of the new instance to a system-supplied message that describes the error, such as "Cannot parse constraint string." This message takes into account the current system culture.</remarks>
		public ConstraintParserException()
			: base(Properties.Resources.ConstraintParserException_Message)
		{
			mPosition = -1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstraintParserException"/> class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <remarks>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</remarks>
		public ConstraintParserException(string message)
			: base(message)
		{
			mPosition = -1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstraintParserException"/> class with a specified error message and position.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="position">The position of the constraint in the string that contains a syntactical error.</param>
		/// <remarks>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</remarks>
		public ConstraintParserException(string message, int position)
			: base(message)
		{
			mPosition = position;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstraintParserException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
		/// <remarks>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</remarks>
		public ConstraintParserException(string message, Exception innerException)
			: base(message, innerException)
		{
			mPosition = -1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstraintParserException"/> class with a specified error message, position and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="position">The position of the constraint in the string that contains a syntactical error.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
		/// <remarks>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</remarks>
		public ConstraintParserException(string message, int position, Exception innerException)
			: base(message, innerException)
		{
			mPosition = position;
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="ConstraintParserException"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is <see langword="null"/>.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The exception could not be deserialized correctly.</exception>
		protected ConstraintParserException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			mPosition = info.GetInt32(nameof(Position));
		}
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstraintParserException"/> class with a specified HRESULT value.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <remarks><para>This constructor initializes the Message property of the new instance to a system-supplied message that describes the error, such as "Cannot parse constraint string." This message takes into account the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public ConstraintParserException(int hresult)
			: base(hresult, Properties.Resources.ConstraintConfigurationException_Message)
		{
			mPosition = -1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstraintParserException"/> class with a specified HRESULT value and error message.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <param name="message">The message that describes the error.</param>
		/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public ConstraintParserException(int hresult, string message)
			: base(hresult, message)
		{
			mPosition = -1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstraintParserException"/> class with a specified HRESULT value, error message and position.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="position">The position of the constraint in the string that contains a syntactical error.</param>
		/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public ConstraintParserException(int hresult, string message, int position)
			: base(hresult, message)
		{
			mPosition = position;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstraintParserException"/> class with a specified HRESULT value, error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
		/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public ConstraintParserException(int hresult, string message, Exception innerException)
			: base(hresult, message, innerException)
		{
			mPosition = -1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConstraintParserException"/> class with a specified HRESULT value, error message, position, and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="position">The position of the constraint in the string that contains a syntactical error.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
		/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public ConstraintParserException(int hresult, string message, int position, Exception innerException)
			: base(hresult, message, innerException)
		{
			mPosition = position;
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
			info.AddValue(nameof(Position), mPosition);
		}
#endif
		#endregion
	}
}
