#region Copyright
/*******************************************************************************
 * <copyright file="ParameterDataType.cs" owner="Daniel Kopp">
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
		[DataMember]
		None = 0,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Boolean"/>.
		/// </summary>
		[DataMember]
		Bool = 1,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Byte"/>.
		/// </summary>
		[DataMember]
		Byte = 2,

		/// <summary>
		/// The parameter has a value representing an array of bytes.
		/// </summary>
		[DataMember]
		Bytes = 3,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.DateTimeOffset"/>.
		/// </summary>
		[DataMember]
		DateTimeOffset = 4,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Decimal"/>.
		/// </summary>
		[DataMember]
		Decimal = 5,

		/// <summary>
		/// The parameter has a value representing an enumeration.
		/// </summary>
		[DataMember]
		Enum = 6,

		/// <summary>
		/// The parameter has a value representing a globally unique identifier <see cref="System.Guid"/>.
		/// </summary>
		[DataMember]
		Guid = 7,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Int16"/>.
		/// </summary>
		[DataMember]
		Int16 = 8,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Int32"/>.
		/// </summary>
		[DataMember]
		Int32 = 9,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Int64"/>.
		/// </summary>
		[DataMember]
		Int64 = 10,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.SByte"/>.
		/// </summary>
		[DataMember]
		SignedByte = 11,

		/// <summary>
		/// The parameter has a value representing a string.
		/// </summary>
		[DataMember]
		String = 12,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.TimeSpan"/>.
		/// </summary>
		[DataMember]
		TimeSpan = 13,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.UInt16"/>.
		/// </summary>
		[DataMember]
		UInt16 = 14,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.UInt32"/>.
		/// </summary>
		[DataMember]
		UInt32 = 15,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.UInt64"/>.
		/// </summary>
		[DataMember]
		UInt64 = 16,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Uri"/>.
		/// </summary>
		[DataMember]
		Uri = 17,

		/// <summary>
		/// The parameter has a value representing a <see cref="System.Version"/>.
		/// </summary>
		[DataMember]
		Version = 18,

		/// <summary>
		/// The parameter has a value representing an XML fragment or document.
		/// </summary>
		[DataMember]
		Xml = 19
	}
}
