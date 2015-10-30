#region Copyright
/*******************************************************************************
 * <copyright file="ParameterConvert.cs" owner="Daniel Kopp">
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
 * <file name="ParameterConvert.cs" date="2015-10-05">
 * Provides methods to convert common language runtime types to strings for data
 * store independent storage of parameter values. Also provides methods to
 * encrypt and decrypt values.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using NerdyDuck.ParameterValidation.Constraints;
#if WINDOWS_UWP
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
#endif
#if WINDOWS_DESKTOP
using System.Security.Cryptography;
#endif

namespace NerdyDuck.ParameterValidation
{
	/// <summary>
	/// Provides methods to convert common language runtime types to strings for data store independent storage of parameter values. Also provides methods to encrypt and decrypt values.
	/// </summary>
	public static class ParameterConvert
	{
		#region Private static fields
#if WINDOWS_DESKTOP
		// AES encryption for Encrypt and Decrypt methods, as a Singleton with Lazy initialization
		private static readonly Lazy<AesCryptoServiceProvider> CryptoProvider = new Lazy<AesCryptoServiceProvider>(() =>
		{
			AesCryptoServiceProvider csp = new AesCryptoServiceProvider();
			csp.BlockSize = 128;
			csp.KeySize = 256;
			csp.Padding = PaddingMode.PKCS7;
			csp.Mode = CipherMode.CBC;
			byte[] key = new byte[csp.KeySize / 8];
			byte[] iv = new byte[csp.BlockSize / 8];
			byte[] pk = typeof(ParameterConvert).Assembly.GetName().GetPublicKey();
			Array.Copy(pk, key, key.Length);
			Array.Copy(pk, key.Length, iv, 0, iv.Length);
			csp.Key = key;
			csp.IV = iv;
			return csp;
		});
#endif
#if WINDOWS_UWP
		private static readonly Lazy<Tuple<CryptographicKey, IBuffer>> CryptoSettings = new Lazy<Tuple<CryptographicKey, IBuffer>>(() =>
		{
			SymmetricKeyAlgorithmProvider csp = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7);
			int keySize = 32; // bytes
			byte[] keyMat = new byte[keySize];
			byte[] iv = new byte[csp.BlockLength];
			byte[] pk = typeof(ParameterConvert).GetTypeInfo().Assembly.GetName().GetPublicKey();
			Array.Copy(pk, keyMat, keyMat.Length);
			Array.Copy(pk, keyMat.Length, iv, 0, iv.Length);

			CryptographicKey key = csp.CreateSymmetricKey(keyMat.AsBuffer());
			return new Tuple<CryptographicKey, IBuffer>(key, iv.AsBuffer());
		});
#endif
		#endregion

		#region Public methods
		#region ToDataType
		/// <summary>
		/// Convert a string value to the specified data type, using the specified constraints.
		/// </summary>
		/// <param name="value">A string that can be deserialized into the specified <paramref name="dataType"/>.</param>
		/// <param name="dataType">The data type to convert the <paramref name="value"/> into.</param>
		/// <param name="constraints">A list of <see cref="Constraint"/>s, that may be used to aid in the conversion.</param>
		/// <returns>A value of the type specified in <paramref name="dataType"/>, converted the the specified <paramref name="value"/>.</returns>
		/// <remarks><paramref name="constraints"/> are only required, if <paramref name="dataType"/> is <see cref="ParameterDataType.Enum"/> or <see cref="ParameterDataType.Enum"/>.
		/// In these cases the list is browsed for <see cref="Constraint"/>s that may give a hint to the actual type to convert to.</remarks>
		/// <exception cref="ParameterConversionException">The value could not be converted into the specified data type, or no constraint was available to specify the actual type to convert to.</exception>
		public static object ToDataType(string value, ParameterDataType dataType, IList<Constraint> constraints)
		{
			if (HasEncryptedConstraint(constraints))
			{
				value = Decrypt(value);
			}

			switch (dataType)
			{
				case ParameterDataType.Bool:
					return ToBoolean(value);
				case ParameterDataType.Byte:
					return ToByte(value);
				case ParameterDataType.Bytes:
					return ToByteArray(value);
				case ParameterDataType.DateTimeOffset:
					return ToDateTimeOffset(value);
				case ParameterDataType.Decimal:
					return ToDecimal(value);
				case ParameterDataType.Guid:
					return ToGuid(value);
				case ParameterDataType.Int16:
					return ToInt16(value);
				case ParameterDataType.Int32:
					return ToInt32(value);
				case ParameterDataType.Int64:
					return ToInt64(value);
				case ParameterDataType.SignedByte:
					return ToSByte(value);
				case ParameterDataType.String:
					return value;
				case ParameterDataType.TimeSpan:
					return ToTimeSpan(value);
				case ParameterDataType.UInt16:
					return ToUInt16(value);
				case ParameterDataType.UInt32:
					return ToUInt32(value);
				case ParameterDataType.UInt64:
					return ToUInt64(value);
				case ParameterDataType.Uri:
					return ToUri(value);
				case ParameterDataType.Version:
					return ToVersion(value);
				case ParameterDataType.Enum:
					TypeConstraint tc1;
					if (TryGetTypeConstraint(constraints, out tc1))
					{
						if (tc1.ResolvedType != null)
						{
							return ToEnumeration(value, tc1.ResolvedType);
						}
						throw new ParameterConversionException(Errors.CreateHResult(0x7d), string.Format(Properties.Resources.ParameterConvert_ToDataType_ResolveFailed, tc1.TypeName), dataType, value);
					}
					throw new ParameterConversionException(Errors.CreateHResult(0x7c), Properties.Resources.ParameterConvert_ToDataType_NoTypeConstraint, dataType, value);
				case ParameterDataType.Xml:
					TypeConstraint tc2;
					if (TryGetTypeConstraint(constraints, out tc2))
					{
						if (tc2.ResolvedType != null)
						{
							return FromXml(value, tc2.ResolvedType);
						}
						throw new ParameterConversionException(Errors.CreateHResult(0x7f), string.Format(Properties.Resources.ParameterConvert_ToDataType_ResolveFailed, tc2.TypeName), dataType, value);
					}
					throw new ParameterConversionException(Errors.CreateHResult(0x7e), Properties.Resources.ParameterConvert_ToDataType_NoTypeConstraint, dataType, value);
				default:
					throw new CodedInvalidOperationException(Errors.CreateHResult(0x80), string.Format(Properties.Resources.ParameterConvert_To_DataTypeNotSupported, dataType));
			}
		}
		#endregion

		#region ToString overloads
		#region ToString(object, ParameterDataType)
		/// <summary>
		/// Converts an <see cref="Object"/> of the specified data type into a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="dataType">The data type of <paramref name="value"/>.</param>
		/// <param name="constraints">A list of <see cref="Constraints"/> that may aid in the conversion.</param>
		/// <returns>A string representation of <paramref name="value"/>. The content may vary.</returns>
		public static string ToString(object value, ParameterDataType dataType, IList<Constraint> constraints)
		{
			if (value == null)
				return null;

			if (dataType != ParameterDataType.Enum && dataType != ParameterDataType.Xml)
			{
				if (value.GetType() != ParameterToNetDataType(dataType))
				{
					throw new ParameterConversionException(Errors.CreateHResult(0x7b), string.Format(Properties.Resources.ParameterConvert_ToString_TypeMismatch, value.GetType().FullName, dataType));
				}
			}

			string ReturnValue;
			switch (dataType)
			{
				case ParameterDataType.Bool:
					ReturnValue = ToString((bool)value);
					break;
				case ParameterDataType.Byte:
					ReturnValue = ToString((byte)value);
					break;
				case ParameterDataType.Bytes:
					ReturnValue = ToString((byte[])value);
					break;
				case ParameterDataType.DateTimeOffset:
					ReturnValue = ToString((DateTimeOffset)value);
					break;
				case ParameterDataType.Decimal:
					ReturnValue = ToString((decimal)value);
					break;
				case ParameterDataType.Guid:
					ReturnValue = ToString((Guid)value);
					break;
				case ParameterDataType.Int16:
					ReturnValue = ToString((short)value);
					break;
				case ParameterDataType.Int32:
					ReturnValue = ToString((int)value);
					break;
				case ParameterDataType.Int64:
					ReturnValue = ToString((long)value);
					break;
				case ParameterDataType.SignedByte:
					ReturnValue = ToString((sbyte)value);
					break;
				case ParameterDataType.String:
					ReturnValue = (string)value;
					break;
				case ParameterDataType.TimeSpan:
					ReturnValue = ToString((TimeSpan)value);
					break;
				case ParameterDataType.UInt16:
					ReturnValue = ToString((ushort)value);
					break;
				case ParameterDataType.UInt32:
					ReturnValue = ToString((uint)value);
					break;
				case ParameterDataType.UInt64:
					ReturnValue = ToString((ulong)value);
					break;
				case ParameterDataType.Uri:
					ReturnValue = ToString((Uri)value);
					break;
				case ParameterDataType.Version:
					ReturnValue = ToString((Version)value);
					break;
				case ParameterDataType.Enum:
					ReturnValue = ToString((Enum)value);
					break;
				case ParameterDataType.Xml:
					ReturnValue = ToXml(value);
					break;
				default:
					throw new CodedInvalidOperationException(Errors.CreateHResult(0x7a), string.Format(Properties.Resources.ParameterConvert_To_DataTypeNotSupported, dataType));
			}

			if (HasEncryptedConstraint(constraints))
			{
				ReturnValue = Encrypt(ReturnValue);
			}

			return ReturnValue;
		}
		#endregion

		#region ToString(Bool)
		/// <summary>
		/// Converts a nullable <see cref="Boolean"/> into a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <see langword="true"/> or <see langword="false"/>.</returns>
		public static string ToString(bool? value)
		{
			if (!value.HasValue)
				return null;

			return XmlConvert.ToString(value.Value);
		}
		#endregion

		#region ToString(byte)
		/// <summary>
		/// Converts a nullable <see cref="Byte"/> into a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
		public static string ToString(byte? value)
		{
			if (!value.HasValue)
				return null;

			return XmlConvert.ToString(value.Value);
		}
		#endregion

		#region ToString(byte[])
		/// <summary>
		/// Converts a <see cref="Byte"/> array into a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; <see cref="string.Empty"/>, if the <paramref name="value"/> has a length of 0; otherwise, a base64-encoded string representing <paramref name="value"/>.</returns>
		public static string ToString(byte[] value)
		{
			if (value == null)
				return null;
			if (value.Length == 0)
				return string.Empty;

			return Convert.ToBase64String(value);
		}
		#endregion

		#region ToString(DateTimeOffset)
		/// <summary>
		/// Converts a nullable <see cref="DateTimeOffset"/> into a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
		public static string ToString(DateTimeOffset? value)
		{
			if (!value.HasValue)
				return null;

			return XmlConvert.ToString(value.Value);
		}
		#endregion

		#region ToString(decimal)
		/// <summary>
		/// Converts a nullable <see cref="Decimal"/> into a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
		public static string ToString(decimal? value)
		{
			if (!value.HasValue)
				return null;

			return XmlConvert.ToString(value.Value);
		}
		#endregion

		#region ToString(Enum)
		/// <summary>
		/// Converts an enumeration value into a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, <paramref name="value"/> is its underlying data type in string representation (i.e. as an integer).</returns>
		public static string ToString(Enum value)
		{
			if (value == null)
				return null;
			Type UnderlyingType = ParameterConvert.GetEnumUnderlyingType(value.GetType());

			return ToString(Convert.ChangeType(value, UnderlyingType), NetToParameterDataType(UnderlyingType), null);
		}
		#endregion

		#region ToString(Guid)
		/// <summary>
		/// Converts a nullable <see cref="Guid"/> to a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of a <see cref="Guid"/>.</returns>
		public static string ToString(Guid? value)
		{
			if (!value.HasValue)
				return null;

			return XmlConvert.ToString(value.Value);
		}
		#endregion

		#region ToString(Int16)
		/// <summary>
		/// Converts a nullable <see cref="Int16"/> into a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
		public static string ToString(short? value)
		{
			if (!value.HasValue)
				return null;

			return XmlConvert.ToString(value.Value);
		}
		#endregion

		#region ToString(Int32)
		/// <summary>
		/// Converts a nullable <see cref="Int32"/> into a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
		public static string ToString(int? value)
		{
			if (!value.HasValue)
				return null;

			return XmlConvert.ToString(value.Value);
		}
		#endregion

		#region ToString(Int64)
		/// <summary>
		/// Converts a nullable <see cref="Int64"/> into a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
		public static string ToString(long? value)
		{
			if (!value.HasValue)
				return null;

			return XmlConvert.ToString(value.Value);
		}
		#endregion

		#region ToString(SByte)
		/// <summary>
		/// Converts a nullable <see cref="SByte"/> into a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
		[CLSCompliant(false)]
		public static string ToString(sbyte? value)
		{
			if (!value.HasValue)
				return null;

			return XmlConvert.ToString(value.Value);
		}
		#endregion

		#region ToString(TimeSpan)
		/// <summary>
		/// Converts a nullable <see cref="TimeSpan"/> into a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
		public static string ToString(TimeSpan? value)
		{
			if (!value.HasValue)
				return null;

			return XmlConvert.ToString(value.Value);
		}
		#endregion

		#region ToString(UInt16)
		/// <summary>
		/// Converts a nullable <see cref="UInt16"/> into a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
		[CLSCompliant(false)]
		public static string ToString(ushort? value)
		{
			if (!value.HasValue)
				return null;

			return XmlConvert.ToString(value.Value);
		}
		#endregion

		#region ToString(UInt32)
		/// <summary>
		/// Converts a nullable <see cref="UInt32"/> to a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
		[CLSCompliant(false)]
		public static string ToString(uint? value)
		{
			if (!value.HasValue)
				return null;

			return XmlConvert.ToString(value.Value);
		}
		#endregion

		#region ToString(UInt64)
		/// <summary>
		/// Converts a nullable <see cref="UInt64"/> to a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
		[CLSCompliant(false)]
		public static string ToString(ulong? value)
		{
			if (!value.HasValue)
				return null;

			return XmlConvert.ToString(value.Value);
		}
		#endregion

		#region ToString(Uri)
		/// <summary>
		/// Converts a <see cref="Uri"/> to a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
		public static string ToString(Uri value)
		{
			if (value == null)
				return null;

			return value.ToString();
		}
		#endregion

		#region ToString(Version)
		/// <summary>
		/// Converts a <see cref="Version"/> to a <see cref="String"/>.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, a version string.</returns>
		public static string ToString(Version value)
		{
			if (value == null)
				return null;

			int FieldCount = 4;
			if (value.Build == -1)
				FieldCount = 2;
			else if (value.Revision == -1)
				FieldCount = 3;

			return value.ToString(FieldCount);
		}
		#endregion
		#endregion

		#region ToXXX methods
		#region ToBoolean
		/// <summary>
		/// Converts a <see cref="String"/> into a <see cref="Boolean"/> value.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>A <see cref="Boolean"/> value, that is, <see langword="true"/> or <see langword="false"/>.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> does not represent a <see cref="Boolean"/> value.</exception>
		public static bool ToBoolean(string value)
		{
			try
			{
				return XmlConvert.ToBoolean(value);
			}
			catch (NullReferenceException)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x56), nameof(value));
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x57), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Bool), ParameterDataType.Bool, value, ex);
			}
		}
		#endregion

		#region ToByte
		/// <summary>
		/// Converts a <see cref="String"/> into a <see cref="Byte"/>.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>A <see cref="Byte"/> equivalent of the string.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format or
		/// <paramref name="value"/> represents a number less than <see cref="Byte.MinValue"/> or greater than <see cref="Byte.MaxValue"/>.</exception>
		public static byte ToByte(string value)
		{
			try
			{
				return XmlConvert.ToByte(value);
			}
			catch (ArgumentNullException)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x58), nameof(value));
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x59), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Byte), ParameterDataType.Bytes, value, ex);
			}
			catch (OverflowException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x59), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Byte), ParameterDataType.Bytes, value, ex);
			}
		}
		#endregion

		#region ToByteArray
		/// <summary>
		/// Converts a <see cref="String"/> into a byte array.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>A byte array equivalent to the string. If <paramref name="value"/> is <see cref="string.Empty"/>, an empty byte array is returned.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format.</exception>
		/// <remarks><paramref name="value"/> must be a base64-formatted string.</remarks>
		public static byte[] ToByteArray(string value)
		{
			if (value == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x5a), nameof(value));
			}
			if (value == string.Empty)
			{
				return new byte[0];
			}
			try
			{
				return Convert.FromBase64String(value);
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x5b), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Bytes), ParameterDataType.Bytes, value, ex);
			}
		}
		#endregion

		#region ToDateTimeOffset
		/// <summary>
		/// Converts a <see cref="String"/> into a <see cref="DateTimeOffset"/>.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>The <see cref="DateTimeOffset"/> equivalent of the supplied string.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is empty or outside the range of allowed values or is not in the correct format.</exception>
		public static DateTimeOffset ToDateTimeOffset(string value)
		{
			try
			{
				return XmlConvert.ToDateTimeOffset(value);
			}
			catch (ArgumentNullException)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x5c), nameof(value));
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x5d), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.DateTimeOffset), ParameterDataType.DateTimeOffset, value, ex);
			}
			catch (ArgumentOutOfRangeException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x5d), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.DateTimeOffset), ParameterDataType.DateTimeOffset, value, ex);
			}
		}
		#endregion

		#region ToDecimal
		/// <summary>
		/// Converts a <see cref="String"/> into a <see cref="Decimal"/>.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>The <see cref="System.Decimal"/> equivalent of the supplied string.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format or
		/// <paramref name="value"/> represents a number less than <see cref="Decimal.MinValue"/> or greater than <see cref="Decimal.MaxValue"/>.</exception>
		public static decimal ToDecimal(string value)
		{
			try
			{
				return XmlConvert.ToDecimal(value);
			}
			catch (ArgumentNullException)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x5e), nameof(value));
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x5f), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Decimal), ParameterDataType.Decimal, value, ex);
			}
			catch (OverflowException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x5f), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Decimal), ParameterDataType.Decimal, value, ex);
			}
		}
		#endregion

		#region ToEnumeration
		/// <summary>
		/// Converts a <see cref="String"/> into the specified enumeration.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <param name="enumType">The type of enumeration to convert to.</param>
		/// <returns>An enumeration value of the specified type.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="enumType"/> or <paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="CodedArgumentException"><paramref name="enumType"/> is not an enumeration.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is empty, or contains a value outside the range of the data type underlying the enumeration, or is a name but not one of the named constants defined for the enumeration.</exception>
		public static object ToEnumeration(string value, Type enumType)
		{
			if (enumType == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x60), nameof(enumType));
			}


			if (!enumType.GetTypeInfo().IsEnum)
			{
				throw new CodedArgumentException(Errors.CreateHResult(0x61), string.Format(Properties.Resources.ParameterConvert_ToEnumeration_NotEnum, enumType.Name), nameof(enumType));
			}

			try
			{
				return Enum.Parse(enumType, value);
			}
			catch (ArgumentNullException)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x62), nameof(value));
			}
			catch (OverflowException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x63), string.Format(Properties.Resources.ParameterConvert_ToEnumeration_Failed, value, enumType.Name), ParameterDataType.Enum, value, ex);
			}
			catch (ArgumentException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x63), string.Format(Properties.Resources.ParameterConvert_ToEnumeration_Failed, value, enumType.Name), ParameterDataType.Enum, value, ex);
			}
		}

		/// <summary>
		/// Converts a <see cref="String"/> into the specified enumeration.
		/// </summary>
		/// <typeparam name="T">The type of enumeration to convert to.</typeparam>
		/// <param name="value">The string to convert.</param>
		/// <returns>An enumeration value of the specified type.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="CodedArgumentException"><typeparamref name="T"/> is not an enumeration.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is empty, or contains a value outside the range of the data type underlying the enumeration, or is a name but not one of the named constants defined for the enumeration.</exception>
		public static T ToEnumeration<T>(string value)
		{
			return (T)ToEnumeration(value, typeof(T));
		}
		#endregion

		#region ToGuid
		/// <summary>
		/// Converts a <see cref="String"/> into a <see cref="Guid"/>.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>The <see cref="System.Guid"/> equivalent of the supplied string.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format or
		/// <paramref name="value"/> represents a number less than <see cref="Decimal.MinValue"/> or greater than <see cref="Decimal.MaxValue"/>.</exception>
		public static Guid ToGuid(string value)
		{
			try
			{
				return XmlConvert.ToGuid(value);
			}
			catch (ArgumentNullException)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x64), nameof(value));
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x65), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Guid), ParameterDataType.Guid, value, ex);
			}
		}
		#endregion

		#region ToInt16
		/// <summary>
		/// Converts a <see cref="String"/> into a <see cref="Int16"/>.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>An <see cref="Int16"/> equivalent of the string.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format, or represents a number less than <see cref="Int16.MinValue"/> or greater than <see cref="Int16.MaxValue"/>.</exception>
		public static short ToInt16(string value)
		{
			try
			{
				return XmlConvert.ToInt16(value);
			}
			catch (ArgumentNullException)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x66), nameof(value));
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x67), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Int16), ParameterDataType.Int16, value, ex);
			}
			catch (OverflowException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x67), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Int16), ParameterDataType.Int16, value, ex);
			}
		}
		#endregion

		#region ToInt32
		/// <summary>
		/// Converts a <see cref="String"/> into a <see cref="Int32"/>.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>An <see cref="Int32"/> equivalent of the string.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format, or represents a number less than <see cref="Int32.MinValue"/> or greater than <see cref="Int32.MaxValue"/>.</exception>
		public static int ToInt32(string value)
		{
			try
			{
				return XmlConvert.ToInt32(value);
			}
			catch (ArgumentNullException)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x68), nameof(value));
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x69), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Int32), ParameterDataType.Int32, value, ex);
			}
			catch (OverflowException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x69), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Int32), ParameterDataType.Int32, value, ex);
			}
		}
		#endregion

		#region ToInt64
		/// <summary>
		/// Converts a <see cref="String"/> into a <see cref="Int64"/>.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>An <see cref="Int64"/> equivalent of the string.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is <see langword="null"/>, or is not in the correct format, or represents a number less than <see cref="Int64.MinValue"/> or greater than <see cref="Int64.MaxValue"/>.</exception>
		public static long ToInt64(string value)
		{
			try
			{
				return XmlConvert.ToInt64(value);
			}
			catch (ArgumentNullException)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x6a), nameof(value));
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x6b), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Int64), ParameterDataType.Int64, value, ex);
			}
			catch (OverflowException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x6b), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Int64), ParameterDataType.Int64, value, ex);
			}
		}
		#endregion

		#region ToSByte
		/// <summary>
		/// Converts a <see cref="String"/> into a <see cref="System.SByte"/>.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>An <see cref="System.SByte"/> equivalent of the string.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is <see langword="null"/>, or is not in the correct format, or represents a number less than <see cref="Int64.MinValue"/> or greater than <see cref="Int64.MaxValue"/>.</exception>
		[CLSCompliant(false)]
		public static sbyte ToSByte(string value)
		{
			try
			{
				return XmlConvert.ToSByte(value);
			}
			catch (ArgumentNullException)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x78), nameof(value));
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x79), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Int64), ParameterDataType.Int64, value, ex);
			}
			catch (OverflowException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x79), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Int64), ParameterDataType.Int64, value, ex);
			}
		}
		#endregion

		#region ToTimeSpan
		/// <summary>
		/// Converts the <see cref="String"/> to a <see cref="TimeSpan"/> equivalent.
		/// </summary>
		/// <param name="value">The string to convert. The string format must conform to the W3C XML Schema Part 2: Datatypes recommendation for duration.</param>
		/// <returns>A <see cref="TimeSpan"/> equivalent of the string.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format, or represents a time that is out of range of <see cref="TimeSpan"/>.</exception>
		public static TimeSpan ToTimeSpan(string value)
		{
			if (value == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x6c), nameof(value));
			}

			try
			{
				return XmlConvert.ToTimeSpan(value);
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x6d), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.TimeSpan), ParameterDataType.TimeSpan, value, ex);
			}
			catch (OverflowException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x6d), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.TimeSpan), ParameterDataType.TimeSpan, value, ex);
			}
		}
		#endregion

		#region ToUInt16
		/// <summary>
		/// Converts a <see cref="String"/> into a <see cref="UInt16"/>.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>An <see cref="UInt16"/> equivalent of the string.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format, or represents a number less than <see cref="UInt16.MinValue"/> or greater than <see cref="UInt16.MaxValue"/>.</exception>
		[CLSCompliant(false)]
		public static ushort ToUInt16(string value)
		{
			try
			{
				return XmlConvert.ToUInt16(value);
			}
			catch (ArgumentNullException)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x6e), nameof(value));
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x6f), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.UInt16), ParameterDataType.UInt16, value, ex);
			}
			catch (OverflowException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x6f), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.UInt16), ParameterDataType.UInt16, value, ex);
			}
		}
		#endregion

		#region ToUInt32
		/// <summary>
		/// Converts a <see cref="String"/> into a <see cref="UInt32"/>.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>An <see cref="UInt32"/> equivalent of the string.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format, or represents a number less than <see cref="UInt32.MinValue"/> or greater than <see cref="UInt32.MaxValue"/>.</exception>
		[CLSCompliant(false)]
		public static uint ToUInt32(string value)
		{
			try
			{
				return XmlConvert.ToUInt32(value);
			}
			catch (ArgumentNullException)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x70), nameof(value));
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x71), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.UInt32), ParameterDataType.UInt32, value, ex);
			}
			catch (OverflowException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x71), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.UInt32), ParameterDataType.UInt32, value, ex);
			}
		}
		#endregion

		#region ToUInt64
		/// <summary>
		/// Converts a <see cref="String"/> into a <see cref="UInt64"/>.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>An <see cref="UInt64"/> equivalent of the string.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format, or represents a number less than <see cref="UInt64.MinValue"/> or greater than <see cref="UInt64.MaxValue"/>.</exception>
		[CLSCompliant(false)]
		public static ulong ToUInt64(string value)
		{
			try
			{
				return XmlConvert.ToUInt64(value);
			}
			catch (ArgumentNullException)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x72), nameof(value));
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x73), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.UInt64), ParameterDataType.UInt64, value, ex);
			}
			catch (OverflowException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x73), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.UInt64), ParameterDataType.UInt64, value, ex);
			}
		}
		#endregion

		#region ToUri
		/// <summary>
		/// Converts a <see cref="String"/> into a <see cref="Uri"/>.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>An <see cref="Uri"/> equivalent of the string.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> is not a valid URI.</exception>
		public static Uri ToUri(string value)
		{
			try
			{
				return new Uri(value);
			}
			catch (ArgumentNullException)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x74), nameof(value));
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x75), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Uri), ParameterDataType.Uri, value, ex);
			}
		}
		#endregion

		#region ToVersion
		/// <summary>
		/// Converts a <see cref="String"/> into a <see cref="Version"/> object.
		/// </summary>
		/// <param name="value">The string to convert.</param>
		/// <returns>An <see cref="Version"/> equivalent of the string.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException"><paramref name="value"/> has fewer than two or more than 4 version components,
		/// or at least one component is less than zero, or at least one component is not an integer
		/// or at least one component is greater than <see cref="Int32.MaxValue"/>.</exception>
		public static Version ToVersion(string value)
		{
			try
			{
				return Version.Parse(value);
			}
			catch (ArgumentNullException)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x76), nameof(value));
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x77), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Version), ParameterDataType.Version, value, ex);
			}
			catch (ArgumentException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x77), string.Format(Properties.Resources.ParameterConvert_To_Failed, value, ParameterDataType.Version), ParameterDataType.Version, value, ex);
			}
		}
		#endregion
		#endregion

		#region FromXml
		/// <summary>
		/// Creates an object from its XML representation.
		/// </summary>
		/// <param name="value">An XML string containing the data of the object to create. </param>
		/// <param name="type">The type of object to create.</param>
		/// <returns>An object created from the XML data in <paramref name="value"/>.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> or <paramref name="type"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException">An error occured during deserialization.</exception>
		public static object FromXml(string value, Type type)
		{
			if (value == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x81), nameof(value));
			}

			if (type == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x82), nameof(type));
			}

			try
			{
				XmlSerializer Serializer = new XmlSerializer(type);
				using (StringReader reader = new StringReader(value))
				{
					return Serializer.Deserialize(reader);
				}
			}
			catch (InvalidOperationException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x83), string.Format(Properties.Resources.ParameterConvert_FromXml_Failed, type.FullName), ParameterDataType.Xml, value, ex.InnerException);
			}
		}

		/// <summary>
		/// Creates an object from its XML representation.
		/// </summary>
		/// <typeparam name="T">The type of object to create.</typeparam>
		/// <param name="value">An XML string containing the data of the object to create. </param>
		/// <returns>An object created from the XML data in <paramref name="value"/>.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
		/// <exception cref="ParameterConversionException">An error occured during deserialization.</exception>
		public static T FromXml<T>(string value)
		{
			return (T)FromXml(value, typeof(T));
		}
		#endregion

		#region ToXml
		/// <summary>
		/// Converts the specified <see cref="Object"/> into its XML representation.
		/// </summary>
		/// <param name="value">The object to convert.</param>
		/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-formatted string representing <paramref name="value"/>.</returns>
		/// <exception cref="ParameterConversionException">An error occurred during XML serialization. See inner exception for details.</exception>
		public static string ToXml(object value)
		{
			if (value == null)
				return null;

			try
			{
				XmlSerializer Serializer = new XmlSerializer(value.GetType());
				using (StringWriter writer = new StringWriter())
				{
					Serializer.Serialize(writer, value);
					writer.Flush();
					return writer.ToString();
				}
			}
			catch (InvalidOperationException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x84), string.Format(Properties.Resources.ParameterConvert_ToXml_Failed, value.GetType().FullName), ParameterDataType.Xml, value, ex);
			}
		}
		#endregion

#if WINDOWS_DESKTOP
		#region Encrypt
		/// <summary>
		/// Encrypt the specified string using AES encryption.
		/// </summary>
		/// <param name="value">The value to encrypt.</param>
		/// <returns>A base64-encoded string, or <see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>.</returns>
		/// <remarks>This method is not supported in the portable version.</remarks>
		/// <exception cref="ParameterConversionException">Cannot encrypt <paramref name="value"/>.</exception>
		public static string Encrypt(string value)
		{
			if (value == null)
			{
				return null;
			}

			MemoryStream EncryptedBuffer = null;
			CryptoStream EncryptingStream = null;
			StreamWriter Writer = null;
			try
			{
				using (ICryptoTransform transform = CryptoProvider.Value.CreateEncryptor())
				{
					EncryptedBuffer = new MemoryStream();
					EncryptingStream = new CryptoStream(EncryptedBuffer, transform, CryptoStreamMode.Write);
					Writer = new StreamWriter(EncryptingStream, System.Text.Encoding.UTF8);
					Writer.Write(value);
					Writer.Flush();
					EncryptingStream.FlushFinalBlock();

					return Convert.ToBase64String(EncryptedBuffer.ToArray());
				}
			}
			catch (CryptographicException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x54), Properties.Resources.ParameterConvert_Encrypt_Failed, ex.InnerException);
			}
			catch (IOException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x54), Properties.Resources.ParameterConvert_Encrypt_Failed, ex.InnerException);
			}
		}
		#endregion

		#region Decrypt
		/// <summary>
		/// Decrypts the specified string using AES encryption.
		/// </summary>
		/// <param name="value">The value to encrypt. The string must be base64-encoded.</param>
		/// <returns>A string, or <see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>.</returns>
		/// <remarks>This method is not supported in the portable version.</remarks>
		/// <exception cref="ParameterConversionException">Cannot decrpyt <paramref name="value"/>.</exception>
		public static string Decrypt(string value)
		{
			if (value == null)
			{
				return null;
			}

			try
			{
				byte[] encbuffer = Convert.FromBase64String(value);
				using (ICryptoTransform transform = CryptoProvider.Value.CreateDecryptor())
				using (MemoryStream EncryptedBuffer = new MemoryStream(encbuffer))
				using (CryptoStream DecryptingStream = new CryptoStream(EncryptedBuffer, transform, CryptoStreamMode.Read))
				using (StreamReader Reader = new StreamReader(DecryptingStream, System.Text.Encoding.UTF8))
				{
					return Reader.ReadToEnd();
				}
			}
			catch (CryptographicException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x55), Properties.Resources.ParameterConvert_Decrypt_Failed, ex.InnerException);
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x55), Properties.Resources.ParameterConvert_Decrypt_Failed, ex.InnerException);
			}
			catch (IOException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x55), Properties.Resources.ParameterConvert_Decrypt_Failed, ex.InnerException);
			}
		}
		#endregion
#endif

#if WINDOWS_UWP
		#region Encrypt
		/// <summary>
		/// Encrypt the specified string using AES encryption.
		/// </summary>
		/// <param name="value">The value to encrypt.</param>
		/// <returns>A base64-encoded string, or <see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>.</returns>
		/// <remarks>This method is not supported in the portable version.</remarks>
		/// <exception cref="ParameterConversionException">Cannot encrypt <paramref name="value"/>.</exception>
		public static string Encrypt(string value)
		{
			if (value == null)
			{
				return null;
			}

			try
			{
				IBuffer SourceBuffer = CryptographicBuffer.ConvertStringToBinary(value, BinaryStringEncoding.Utf8);
				IBuffer EncryptedBuffer = CryptographicEngine.Encrypt(CryptoSettings.Value.Item1, SourceBuffer.ToArray().AsBuffer(), CryptoSettings.Value.Item2);
				return CryptographicBuffer.EncodeToBase64String(EncryptedBuffer);
			}
			catch (IOException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x54), Properties.Resources.ParameterConvert_Encrypt_Failed, ex.InnerException);
			}
			// TODO Cryptogaphic exception?
		}
		#endregion

		#region Decrypt
		/// <summary>
		/// Decrypts the specified string using AES encryption.
		/// </summary>
		/// <param name="value">The value to encrypt. The string must be base64-encoded.</param>
		/// <returns>A string, or <see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>.</returns>
		/// <remarks>This method is not supported in the portable version.</remarks>
		/// <exception cref="ParameterConversionException">Cannot decrpyt <paramref name="value"/>.</exception>
		public static string Decrypt(string value)
		{
			if (value == null)
			{
				return null;
			}

			try
			{
				IBuffer EncryptedBuffer = CryptographicBuffer.DecodeFromBase64String(value);
				IBuffer DecryptedBuffer = CryptographicEngine.Decrypt(CryptoSettings.Value.Item1, EncryptedBuffer, CryptoSettings.Value.Item2);
				return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, DecryptedBuffer);
			}
			catch (FormatException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x55), Properties.Resources.ParameterConvert_Decrypt_Failed, ex.InnerException);
			}
			catch (IOException ex)
			{
				throw new ParameterConversionException(Errors.CreateHResult(0x55), Properties.Resources.ParameterConvert_Decrypt_Failed, ex.InnerException);
			}
			// TODO Cryptogaphic exception? Base64 format exception
		}
		#endregion
#endif

		#region TryGetTypeConstraint
		/// <summary>
		/// Tries to find a <see cref="TypeConstraint"/> or <see cref="EnumTypeConstraint"/> in a list of constraints.
		/// </summary>
		/// <param name="constraints">A list of <see cref="Constraint"/>s to search in.</param>
		/// <param name="constraint">When the method returns, contains the <see cref="TypeConstraint"/> found in <paramref name="constraints"/>, or <see langword="null"/>, if no matching constraint was found.</param>
		/// <returns><see langword="true"/>, if a <see cref="TypeConstraint"/> was found in <paramref name="constraints"/>; <see langword="false"/>, otherwise.</returns>
		public static bool TryGetTypeConstraint(IList<Constraint> constraints, out TypeConstraint constraint)
		{
			constraint = (TypeConstraint)constraints.FirstOrDefault(c => c.Name == Constraint.TypeConstraintName);
			return (constraint != null);
		}
		#endregion

		#region HasEncryptedConstraint
		/// <summary>
		/// Checks if the specified list contains a <see cref="EncryptedConstraint"/>.
		/// </summary>
		/// <param name="constraints">A list of <see cref="Constraint"/>s to search in. May be <see langword="null"/>.</param>
		/// <returns><see langword="true"/>, if <paramref name="constraints"/> contains an <see cref="EncryptedConstraint"/>; otherwise, or if <paramref name="constraints"/> is <see langword="null"/>, <see langword="false"/>.</returns>
		public static bool HasEncryptedConstraint(IList<Constraint> constraints)
		{
			if (constraints == null)
				return false;

			return (constraints.FirstOrDefault(c => c.Name == Constraint.EncryptedConstraintName) != null);
		}
		#endregion

		#region NetToParameterDataType
		/// <summary>
		/// Converts a .Net type to a <see cref="ParameterDataType"/>.
		/// </summary>
		/// <param name="type">A type.</param>
		/// <returns>A <see cref="ParameterDataType"/> that represents the .Net type.</returns>
		/// <exception cref="CodedArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
		/// <exception cref="CodedArgumentException">There is no matching representation of <paramref name="type"/> in <see cref="ParameterDataType"/>.</exception>
		public static ParameterDataType NetToParameterDataType(Type type)
		{
			if (type == null)
				throw new CodedArgumentNullException(Errors.CreateHResult(0x15), nameof(type));
			if (type == typeof(bool))
				return ParameterDataType.Bool;
			if (type == typeof(byte))
				return ParameterDataType.Byte;
			if (type == typeof(byte[]))
				return ParameterDataType.Bytes;
			if (type == typeof(DateTimeOffset))
				return ParameterDataType.DateTimeOffset;
			if (type == typeof(decimal))
				return ParameterDataType.Decimal;
			if (type == typeof(Guid))
				return ParameterDataType.Guid;
			if (type == typeof(short))
				return ParameterDataType.Int16;
			if (type == typeof(int))
				return ParameterDataType.Int32;
			if (type == typeof(long))
				return ParameterDataType.Int64;
			if (type == typeof(string))
				return ParameterDataType.String;
			if (type == typeof(sbyte))
				return ParameterDataType.SignedByte;
			if (type == typeof(TimeSpan))
				return ParameterDataType.TimeSpan;
			if (type == typeof(ushort))
				return ParameterDataType.UInt16;
			if (type == typeof(uint))
				return ParameterDataType.UInt32;
			if (type == typeof(ulong))
				return ParameterDataType.UInt64;
			if (type == typeof(Uri))
				return ParameterDataType.Uri;
			if (type == typeof(Version))
				return ParameterDataType.Version;
			TypeInfo info = type.GetTypeInfo();
			if (info.IsEnum)
				return ParameterDataType.Enum;
			if (info.ImplementedInterfaces.Contains(typeof(IXmlSerializable)))
				return ParameterDataType.Xml;

			throw new CodedArgumentException(Errors.CreateHResult(0x16), string.Format(Properties.Resources.ParameterConvert_NetToParameterDataType_NoMatch, type.FullName), "type");
		}
#endregion

		#region ParameterToNetDataType
		/// <summary>
		/// Converts a <see cref="ParameterDataType"/> to a .Net type.
		/// </summary>
		/// <param name="type">A type.</param>
		/// <returns>A .Net type that is represented by <see cref="ParameterDataType"/>.</returns>
		/// <exception cref="CodedArgumentException">There is no matching representation of <paramref name="type"/>, e.g. for <see cref="ParameterDataType.Xml"/>.</exception>
		public static Type ParameterToNetDataType(ParameterDataType type)
		{
			switch (type)
			{
				case ParameterDataType.Bool:
					return typeof(bool);
				case ParameterDataType.Byte:
					return typeof(byte);
				case ParameterDataType.Bytes:
					return typeof(byte[]);
				case ParameterDataType.DateTimeOffset:
					return typeof(DateTimeOffset);
				case ParameterDataType.Decimal:
					return typeof(decimal);
				case ParameterDataType.Guid:
					return typeof(Guid);
				case ParameterDataType.Int16:
					return typeof(short);
				case ParameterDataType.Int32:
					return typeof(int);
				case ParameterDataType.Int64:
					return typeof(long);
				case ParameterDataType.String:
					return typeof(string);
				case ParameterDataType.SignedByte:
					return typeof(sbyte);
				case ParameterDataType.TimeSpan:
					return typeof(TimeSpan);
				case ParameterDataType.UInt16:
					return typeof(ushort);
				case ParameterDataType.UInt32:
					return typeof(uint);
				case ParameterDataType.UInt64:
					return typeof(ulong);
				case ParameterDataType.Uri:
					return typeof(Uri);
				case ParameterDataType.Version:
					return typeof(Version);
				case ParameterDataType.Xml:
					return typeof(object);
				default:
					throw new CodedArgumentException(Errors.CreateHResult(0x17), string.Format(Properties.Resources.ParameterConvert_ParameterToNetDataType_NoMatch, type), nameof(type));
			}
		}
		#endregion
		#endregion

		#region Internal methods
		#region GetEnumUnderlyingType
		/// <summary>
		/// Gets the integer type that the enumeration is based on.
		/// </summary>
		/// <param name="enumType">The enumeration type.</param>
		/// <returns>An integer type.</returns>
		internal static Type GetEnumUnderlyingType(Type enumType)
		{
			FieldInfo[] fields = enumType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (fields == null || fields.Length != 1)
			{
				return null;
			}
			return fields[0].FieldType;
		}
		#endregion

		#region IsIntegerType
		/// <summary>
		/// Checks if the specified type represents a an integer type.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns><see langword="true"/>, if the type is an integer type; <see langword="false"/>, otherwise.</returns>
		internal static bool IsIntegerType(ParameterDataType type)
		{
			switch (type)
			{
				case ParameterDataType.Int32:
				case ParameterDataType.Byte:
				case ParameterDataType.Int16:
				case ParameterDataType.Int64:
				case ParameterDataType.SignedByte:
				case ParameterDataType.UInt16:
				case ParameterDataType.UInt32:
				case ParameterDataType.UInt64:
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Checks if the specified type represents a an integer type.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns><see langword="true"/>, if the type is an integer type; <see langword="false"/>, otherwise.</returns>
		internal static bool IsIntegerType(Type type)
		{
			return (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte) ||
				type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort) || type == typeof(sbyte));
		}
		#endregion
		#endregion
	}
}
