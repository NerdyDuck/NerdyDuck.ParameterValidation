#region Copyright
/*******************************************************************************
 * <copyright file="ParameterConversionException.cs" owner="Daniel Kopp">
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
 * <file name="ParameterConversionException.cs" date="2015-10-01">
 * The exception that is thrown when a (string) parameter of a Constraint
 * cannot be parsed or converted into the expected type.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyDuck.ParameterValidation
{
	/// <summary>
	/// The exception that is thrown when a (string) parameter of a <see cref="Constraint"/> cannot be parsed or converted into the expected type.
	/// </summary>
	public class ParameterConversionException : CodedFormatException
	{
#if WINDOWS_DESKTOP
		#region Constants
		private const string ActualValueTypeName = "ActualValueType";
		#endregion
#endif
		#region Private fields
		private object mActualValue;
		private ParameterDataType mDataType;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the value that was attempted to convert.
		/// </summary>
		public object ActualValue
		{
			get { return mActualValue; }
		}

		/// <summary>
		/// Gets the data type that was attempted to convert from or into.
		/// </summary>
		public ParameterDataType DataType
		{
			get { return mDataType; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterConversionException"/> class.
		/// </summary>
		/// <remarks>This constructor initializes the Message property of the new instance to a system-supplied message that describes the error, such as "Parameter conversion failed." This message takes into account the current system culture.</remarks>
		public ParameterConversionException()
			: this(Properties.Resources.ParameterConversionException_Message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterConversionException"/> class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <remarks>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</remarks>
		public ParameterConversionException(string message)
			: this(message, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterConversionException"/> class with a specified error message, data type and actual value.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="dataType">The data type that was attempted to convert from or into.</param>
		/// <param name="actualValue">The value that was attempted to convert.</param>
		/// <remarks>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</remarks>
		public ParameterConversionException(string message, ParameterDataType dataType, object actualValue)
			: this(message, dataType, actualValue, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterConversionException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
		/// <remarks>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</remarks>
		public ParameterConversionException(string message, Exception innerException)
			: this(message, ParameterDataType.String, null, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterConversionException"/> class with a specified error message,
		/// data type, actual value and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
		/// <param name="dataType">The data type that was attempted to convert from or into.</param>
		/// <param name="actualValue">The value that was attempted to convert.</param>
		/// <remarks>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</remarks>
		public ParameterConversionException(string message, ParameterDataType dataType, object actualValue, Exception innerException)
			: base(message, innerException)
		{
			mDataType = dataType;
			mActualValue = actualValue;
		}

#if WINDOWS_DESKTOP
		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterConversionException"/> class with serialized data.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="info"/> argument is null.</exception>
		/// <exception cref="System.Runtime.Serialization.SerializationException">The exception could not be deserialized correctly.</exception>
		protected ParameterConversionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			mDataType = (ParameterDataType)info.GetValue(nameof(DataType), typeof(ParameterDataType));
			mActualValue = info.GetValue(nameof(ActualValue), (Type)info.GetValue(ActualValueTypeName, typeof(Type)));
		}
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterConversionException"/> class with a specified HRESULT value.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <remarks><para>This constructor initializes the Message property of the new instance to a system-supplied message that describes the error, such as "Parameter conversion failed." This message takes into account the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public ParameterConversionException(int hresult)
			: this(hresult, Properties.Resources.ParameterConversionException_Message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterConversionException"/> class with a specified HRESULT value and error message.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <param name="message">The message that describes the error.</param>
		/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public ParameterConversionException(int hresult, string message)
			: this(hresult, message, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterConversionException"/> class with a specified HRESULT value, error message, data type and actual value.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="dataType">The data type that was attempted to convert from or into.</param>
		/// <param name="actualValue">The value that was attempted to convert.</param>
		/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public ParameterConversionException(int hresult, string message, ParameterDataType dataType, object actualValue)
			: this(hresult, message, dataType, actualValue, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterConversionException"/> class with a specified HRESULT value, error message and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
		/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public ParameterConversionException(int hresult, string message, Exception innerException)
			: this(hresult, message, ParameterDataType.String, null, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterConversionException"/> class with a specified HRESULT value, error message,
		/// data type, actual value and a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="hresult">The HRESULT that describes the error.</param>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
		/// <param name="dataType">The data type that was attempted to convert from or into.</param>
		/// <param name="actualValue">The value that was attempted to convert.</param>
		/// <remarks><para>This constructor initializes the <see cref="Exception.Message"/> property of the new instance using the value of the <paramref name="message"/> parameter. The content of the message parameter is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</para>
		/// <para>See the <a href="http://msdn.microsoft.com/en-us/library/cc231198.aspx">HRESULT definition at MSDN</a> for
		/// more information about the definition of HRESULT values.</para></remarks>
		public ParameterConversionException(int hresult, string message, ParameterDataType dataType, object actualValue, Exception innerException)
			: base(hresult, message, innerException)
		{
			mDataType = dataType;
			mActualValue = actualValue;
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
			info.AddValue(nameof(DataType), mDataType);
			info.AddValue(ActualValueTypeName, mActualValue.GetType());
			info.AddValue(nameof(ActualValue), mActualValue);
		}
#endif
		#endregion
	}
}
