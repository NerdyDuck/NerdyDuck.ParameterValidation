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
using System.Runtime.Serialization;

namespace NerdyDuck.ParameterValidation
{
	/// <summary>
	/// Specifies the data type of a parameter.
	/// </summary>
#if WINDOWS_DESKTOP
	[System.Serializable]
#endif
	[DataContract(Namespace = Constraint.Namespace)]
	public enum ParameterDataType
	{
		/// <summary>
		/// Unknown parameter type. Do not use.
		/// </summary>
		[EnumMember]
		None = 0,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Boolean"/>.
		/// </summary>
		[EnumMember]
		Bool = 1,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Byte"/>.
		/// </summary>
		[EnumMember]
		Byte = 2,

		/// <summary>
		/// The parameter has a value representing an array of bytes.
		/// </summary>
		[EnumMember]
		Bytes = 3,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.DateTimeOffset"/>.
		/// </summary>
		[EnumMember]
		DateTimeOffset = 4,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Decimal"/>.
		/// </summary>
		[EnumMember]
		Decimal = 5,

		/// <summary>
		/// The parameter has a value representing an enumeration.
		/// </summary>
		[EnumMember]
		Enum = 6,

		/// <summary>
		/// The parameter has a value representing a globally unique identifier <see cref="System.Guid"/>.
		/// </summary>
		[EnumMember]
		Guid = 7,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Int16"/>.
		/// </summary>
		[EnumMember]
		Int16 = 8,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Int32"/>.
		/// </summary>
		[EnumMember]
		Int32 = 9,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Int64"/>.
		/// </summary>
		[EnumMember]
		Int64 = 10,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.SByte"/>.
		/// </summary>
		[EnumMember]
		SignedByte = 11,

		/// <summary>
		/// The parameter has a value representing a string.
		/// </summary>
		[EnumMember]
		String = 12,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.TimeSpan"/>.
		/// </summary>
		[EnumMember]
		TimeSpan = 13,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.UInt16"/>.
		/// </summary>
		[EnumMember]
		UInt16 = 14,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.UInt32"/>.
		/// </summary>
		[EnumMember]
		UInt32 = 15,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.UInt64"/>.
		/// </summary>
		[EnumMember]
		UInt64 = 16,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Uri"/>.
		/// </summary>
		[EnumMember]
		Uri = 17,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Version"/>.
		/// </summary>
		[EnumMember]
		Version = 18,

		/// <summary>
		/// The parameter has a value representing an XML fragment or document.
		/// </summary>
		[EnumMember]
		Xml = 19
	}
}
