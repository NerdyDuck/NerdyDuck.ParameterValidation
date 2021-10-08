#region Copyright
/*******************************************************************************
 * <copyright file="ParameterDataType.cs" owner="Daniel Kopp">
 * Copyright 2015-2016 Daniel Kopp
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
 * <file name="ParameterDataType.cs" date="2015-09-29">
 * Specifies the data type of a parameter.
 * </file>
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
