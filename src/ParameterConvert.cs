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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NerdyDuck.ParameterValidation.Constraints;

namespace NerdyDuck.ParameterValidation;

/// <summary>
/// Provides methods to convert common language runtime types to strings for data store independent storage of parameter values. Also provides methods to encrypt and decrypt values.
/// </summary>
public static class ParameterConvert
{
	// UTF-8 encoding without byte order mark.
	private static readonly Encoding s_utf8NoBOM = new UTF8Encoding(false);

	// AES encryption for Encrypt and Decrypt methods, as a Singleton with Lazy initialization
	private static readonly Lazy<Aes> s_aes = new(() =>
	{
		Aes aes = Aes.Create();
		aes.BlockSize = 128;
		aes.KeySize = 256;
		aes.Padding = PaddingMode.PKCS7;
		aes.Mode = CipherMode.CBC;
		byte[] key = new byte[aes.KeySize / 8];
		byte[] iv = new byte[aes.BlockSize / 8];
		byte[] pk = typeof(ParameterConvert).Assembly.GetName()?.GetPublicKey() ?? throw new CodedInvalidOperationException(HResult.Create(ErrorCodes.ParameterConvert_AES_NoAssemblyName), TextResources.ParameterConvert_AES_NoAssemblyName);
		Buffer.BlockCopy(pk, 0, key, 0, key.Length);
		Buffer.BlockCopy(pk, key.Length, iv, 0, iv.Length);
		aes.Key = key;
		aes.IV = iv;
		return aes;
	});

	/// <summary>
	/// Convert a string value to the specified data type, using the specified constraints.
	/// </summary>
	/// <param name="value">A string that can be deserialized into the specified <paramref name="dataType"/>.</param>
	/// <param name="dataType">The data type to convert the <paramref name="value"/> into.</param>
	/// <param name="constraints">A list of <see cref="Constraint"/>s, that may be used to aid in the conversion.</param>
	/// <returns>A value of the type specified in <paramref name="dataType"/>, converted the specified <paramref name="value"/>.</returns>
	/// <remarks><paramref name="constraints"/> are only required, if <paramref name="dataType"/> is <see cref="ParameterDataType.Enum"/> or <see cref="ParameterDataType.Enum"/>.
	/// In these cases the list is browsed for <see cref="Constraint"/>s that may give a hint to the actual type to convert to.</remarks>
	/// <exception cref="ParameterConversionException">The value could not be converted into the specified data type, or no constraint was available to specify the actual type to convert to.</exception>
	public static object? ToDataType(string? value, ParameterDataType dataType, IReadOnlyList<Constraint>? constraints)
	{
		if (value == null)
		{
			return HasNullConstraint(constraints)
				? null
				: throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToDataType_ValueNull), TextResources.ParameterConvert_ToDataType_ValueNull);
		}
		if (HasEncryptedConstraint(constraints))
		{
			value = Decrypt(value);
		}

#if NETSTD20
#pragma warning disable CS8604 // Possible null reference argument.
#endif
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
				TypeConstraint? tc1;
				if (TryGetTypeConstraint(constraints, out tc1))
				{
					return tc1?.ResolvedType != null
						? ToEnumeration(value, tc1!.ResolvedType)
						: throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToDataType_ResolveEnumFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_ToDataType_ResolveFailed, tc1?.TypeName ?? string.Empty), dataType, value);
				}
				throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToDataType_EnumNoTypeConstraint), TextResources.ParameterConvert_ToDataType_NoTypeConstraint, dataType, value);
			case ParameterDataType.Xml:
				TypeConstraint? tc2;
				if (TryGetTypeConstraint(constraints, out tc2))
				{
					return tc2?.ResolvedType != null
						? FromXml(value, tc2.ResolvedType)
						: throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToDataType_ResolveXmlFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_ToDataType_ResolveFailed, tc2?.TypeName), dataType, value);
				}
				throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToDataType_XmlNoTypeConstraint), TextResources.ParameterConvert_ToDataType_NoTypeConstraint, dataType, value);
			default:
				throw new CodedInvalidOperationException(HResult.Create(ErrorCodes.ParameterConvert_ToDataType_TypeNotSupported), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_DataTypeNotSupported, dataType));
		}
#if NETSTD20
#pragma warning restore CS8604 // Possible null reference argument.
#endif
	}

	/// <summary>
	/// Converts an <see cref="object"/> of the specified data type into a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <param name="dataType">The data type of <paramref name="value"/>.</param>
	/// <param name="constraints">A list of <see cref="Constraints"/> that may aid in the conversion.</param>
	/// <returns>A string representation of <paramref name="value"/>. The content may vary.</returns>
	/// <exception cref="ParameterConversionException">The value could not be converted into a string.</exception>
	/// <exception cref="CodedInvalidOperationException">The data type is not supported.</exception>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(object? value, ParameterDataType dataType, IReadOnlyList<Constraint>? constraints)
	{
		if (value == null)
			return null;

		if (dataType is not ParameterDataType.Enum and not ParameterDataType.Xml)
		{
			if (value.GetType() != ParameterToNetDataType(dataType))
			{
				throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToString_TypeMismatch), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_ToString_TypeMismatch, value.GetType().FullName, dataType));
			}
		}

		string? result = dataType switch
		{
			ParameterDataType.Bool => ToString((bool)value),
			ParameterDataType.Byte => ToString((byte)value),
			ParameterDataType.Bytes => ToString((byte[])value),
			ParameterDataType.DateTimeOffset => ToString((DateTimeOffset)value),
			ParameterDataType.Decimal => ToString((decimal)value),
			ParameterDataType.Guid => ToString((Guid)value),
			ParameterDataType.Int16 => ToString((short)value),
			ParameterDataType.Int32 => ToString((int)value),
			ParameterDataType.Int64 => ToString((long)value),
			ParameterDataType.SignedByte => ToString((sbyte)value),
			ParameterDataType.String => (string)value,
			ParameterDataType.TimeSpan => ToString((TimeSpan)value),
			ParameterDataType.UInt16 => ToString((ushort)value),
			ParameterDataType.UInt32 => ToString((uint)value),
			ParameterDataType.UInt64 => ToString((ulong)value),
			ParameterDataType.Uri => ToString((Uri)value),
			ParameterDataType.Version => ToString((Version)value),
			ParameterDataType.Enum => ToString((Enum)value),
			ParameterDataType.Xml => ToXml(value),
			_ => throw new CodedInvalidOperationException(HResult.Create(ErrorCodes.ParameterConvert_ToString_TypeNotSupported), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_DataTypeNotSupported, dataType)),
		};
		if (HasEncryptedConstraint(constraints))
		{
			result = Encrypt(result);
		}

		return result;
	}

	/// <summary>
	/// Converts a nullable <see cref="bool"/> into a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <see langword="true"/> or <see langword="false"/>.</returns>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(bool? value) => !value.HasValue ? null : XmlConvert.ToString(value.Value);

	/// <summary>
	/// Converts a nullable <see cref="byte"/> into a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(byte? value) => !value.HasValue ? null : XmlConvert.ToString(value.Value);

	/// <summary>
	/// Converts a <see cref="byte"/> array into a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; <see cref="string.Empty"/>, if the <paramref name="value"/> has a length of 0; otherwise, a base64-encoded string representing <paramref name="value"/>.</returns>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(byte[]? value) => value == null ? null : value.Length == 0 ? string.Empty : Convert.ToBase64String(value);

	/// <summary>
	/// Converts a nullable <see cref="DateTimeOffset"/> into a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(DateTimeOffset? value) => !value.HasValue ? null : XmlConvert.ToString(value.Value);

	/// <summary>
	/// Converts a nullable <see cref="decimal"/> into a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(decimal? value) => !value.HasValue ? null : XmlConvert.ToString(value.Value);

	/// <summary>
	/// Converts an enumeration value into a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, <paramref name="value"/> is its underlying data type in string representation (i.e. as an integer).</returns>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(Enum? value)
	{
		if (value == null)
			return null;
		Type? underlyingType = ParameterConvert.GetEnumUnderlyingType(value.GetType());
		return underlyingType == null
			? throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToString_UnknownEnumType), string.Format(CultureInfo.CurrentCulture, TextResources.Global_UnknownEnumType, value.GetType()))
			: ToString(Convert.ChangeType(value, underlyingType, CultureInfo.InvariantCulture), NetToParameterDataType(underlyingType), null);
	}

	/// <summary>
	/// Converts a nullable <see cref="Guid"/> to a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of a <see cref="Guid"/>.</returns>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(Guid? value) => !value.HasValue ? null : XmlConvert.ToString(value.Value);

	/// <summary>
	/// Converts a nullable <see cref="short"/> into a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(short? value) => !value.HasValue ? null : XmlConvert.ToString(value.Value);

	/// <summary>
	/// Converts a nullable <see cref="int"/> into a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(int? value) => !value.HasValue ? null : XmlConvert.ToString(value.Value);

	/// <summary>
	/// Converts a nullable <see cref="long"/> into a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(long? value) => !value.HasValue ? null : XmlConvert.ToString(value.Value);

	/// <summary>
	/// Converts a nullable <see cref="sbyte"/> into a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
	[CLSCompliant(false)]
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(sbyte? value) => !value.HasValue ? null : XmlConvert.ToString(value.Value);

	/// <summary>
	/// Converts a nullable <see cref="TimeSpan"/> into a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(TimeSpan? value) => !value.HasValue ? null : XmlConvert.ToString(value.Value);

	/// <summary>
	/// Converts a nullable <see cref="ushort"/> into a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
	[CLSCompliant(false)]
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(ushort? value) => !value.HasValue ? null : XmlConvert.ToString(value.Value);

	/// <summary>
	/// Converts a nullable <see cref="uint"/> to a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
	[CLSCompliant(false)]
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(uint? value) => !value.HasValue ? null : XmlConvert.ToString(value.Value);

	/// <summary>
	/// Converts a nullable <see cref="ulong"/> to a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
	[CLSCompliant(false)]
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(ulong? value) => !value.HasValue ? null : XmlConvert.ToString(value.Value);

	/// <summary>
	/// Converts a <see cref="Uri"/> to a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-compatible string representation of <paramref name="value"/>.</returns>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(Uri? value) => value?.ToString();

	/// <summary>
	/// Converts a <see cref="Version"/> to a <see cref="string"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, a version string.</returns>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToString(Version? value)
	{
		if (value is null)
			return null;

		int FieldCount = 4;
		if (value.Build == -1)
			FieldCount = 2;
		else if (value.Revision == -1)
			FieldCount = 3;

		return value.ToString(FieldCount);
	}

	/// <summary>
	/// Converts a <see cref="string"/> into a <see cref="bool"/> value.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <returns>A <see cref="bool"/> value, that is, <see langword="true"/> or <see langword="false"/>.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> does not represent a <see cref="bool"/> value.</exception>
	public static bool ToBoolean(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToBoolean_ValueNull), nameof(value));
		}
		try
		{
			return XmlConvert.ToBoolean(value);
		}
		catch (FormatException ex)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToBoolean_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.Bool), ParameterDataType.Bool, value, ex);
		}
	}

	/// <summary>
	/// Converts a <see cref="string"/> into a <see cref="byte"/>.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <returns>A <see cref="byte"/> equivalent of the string.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format or
	/// <paramref name="value"/> represents a number less than <see cref="byte.MinValue"/> or greater than <see cref="byte.MaxValue"/>.</exception>
	public static byte ToByte(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToByte_ValueNull), nameof(value));
		}
		try
		{
			return XmlConvert.ToByte(value);
		}
		catch (Exception ex) when (ex is FormatException or OverflowException)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToByte_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.Byte), ParameterDataType.Bytes, value, ex);
		}
	}

	/// <summary>
	/// Converts a <see cref="string"/> into a byte array.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <returns>A byte array equivalent to the string. If <paramref name="value"/> is <see cref="string.Empty"/>, an empty byte array is returned.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format.</exception>
	/// <remarks><paramref name="value"/> must be a base64-formatted string.</remarks>
	public static byte[] ToByteArray(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToByteArray_ValueNull), nameof(value));
		}
		if (string.IsNullOrEmpty(value))
		{
			return Array.Empty<byte>();
		}
		try
		{
			return Convert.FromBase64String(value);
		}
		catch (FormatException ex)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToByteArray_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.Bytes), ParameterDataType.Bytes, value, ex);
		}
	}

	/// <summary>
	/// Converts a <see cref="string"/> into a <see cref="DateTimeOffset"/>.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <returns>The <see cref="DateTimeOffset"/> equivalent of the supplied string.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is empty or outside the range of allowed values or is not in the correct format.</exception>
	public static DateTimeOffset ToDateTimeOffset(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToDateTimeOffset_ValueNull), nameof(value));
		}
		try
		{
			return XmlConvert.ToDateTimeOffset(value);
		}
		catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToDateTimeOffset_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.DateTimeOffset), ParameterDataType.DateTimeOffset, value, ex);
		}
	}

	/// <summary>
	/// Converts a <see cref="string"/> into a <see cref="decimal"/>.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <returns>The <see cref="decimal"/> equivalent of the supplied string.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format or
	/// <paramref name="value"/> represents a number less than <see cref="decimal.MinValue"/> or greater than <see cref="decimal.MaxValue"/>.</exception>
	public static decimal ToDecimal(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToDecimal_ValueNull), nameof(value));
		}
		try
		{
			return XmlConvert.ToDecimal(value);
		}
		catch (Exception ex) when (ex is FormatException or OverflowException)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToDecimal_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.Decimal), ParameterDataType.Decimal, value, ex);
		}
	}

	/// <summary>
	/// Converts a <see cref="string"/> into the specified enumeration.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <param name="enumType">The type of enumeration to convert to.</param>
	/// <returns>An enumeration value of the specified type.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="enumType"/> or <paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="CodedArgumentException"><paramref name="enumType"/> is not an enumeration.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is empty, or contains a value outside the range of the data type underlying the enumeration, or is a name but not one of the named constants defined for the enumeration.</exception>
	public static object ToEnumeration(string value, Type enumType)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToEnumeration_ValueNull), nameof(value));
		}
		if (enumType == null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToEnumeration_EnumTypeNull), nameof(enumType));
		}
		if (!enumType.GetTypeInfo().IsEnum)
		{
			throw new CodedArgumentException(HResult.Create(ErrorCodes.ParameterConvert_ToEnumeration_NotEnum), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_ToEnumeration_NotEnum, enumType.Name), nameof(enumType));
		}

		try
		{
			return Enum.Parse(enumType, value);
		}
		catch (Exception ex) when (ex is OverflowException or ArgumentException)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToEnumeration_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_ToEnumeration_Failed, value, enumType.Name), ParameterDataType.Enum, value, ex);
		}
	}

	/// <summary>
	/// Converts a <see cref="string"/> into the specified enumeration.
	/// </summary>
	/// <typeparam name="T">The type of enumeration to convert to.</typeparam>
	/// <param name="value">The string to convert.</param>
	/// <returns>An enumeration value of the specified type.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="CodedArgumentException"><typeparamref name="T"/> is not an enumeration.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is empty, or contains a value outside the range of the data type underlying the enumeration, or is a name but not one of the named constants defined for the enumeration.</exception>
	public static T ToEnumeration<T>(string value) => (T)ToEnumeration(value, typeof(T));

	/// <summary>
	/// Converts a <see cref="string"/> into a <see cref="Guid"/>.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <returns>The <see cref="System.Guid"/> equivalent of the supplied string.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format.</exception>
	public static Guid ToGuid(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToGuid_ValueNull), nameof(value));
		}
		try
		{
			return XmlConvert.ToGuid(value);
		}
		catch (FormatException ex)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToGuid_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.Guid), ParameterDataType.Guid, value, ex);
		}
	}

	/// <summary>
	/// Converts a <see cref="string"/> into a <see cref="short"/>.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <returns>An <see cref="short"/> equivalent of the string.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format, or represents a number less than <see cref="short.MinValue"/> or greater than <see cref="short.MaxValue"/>.</exception>
	public static short ToInt16(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToGuid_ValueNull), nameof(value));
		}
		try
		{
			return XmlConvert.ToInt16(value);
		}
		catch (Exception ex) when (ex is FormatException or OverflowException)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToInt16_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.Int16), ParameterDataType.Int16, value, ex);
		}
	}

	/// <summary>
	/// Converts a <see cref="string"/> into a <see cref="int"/>.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <returns>An <see cref="int"/> equivalent of the string.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format, or represents a number less than <see cref="int.MinValue"/> or greater than <see cref="int.MaxValue"/>.</exception>
	public static int ToInt32(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToInt32_ValueNull), nameof(value));
		}
		try
		{
			return XmlConvert.ToInt32(value);
		}
		catch (Exception ex) when (ex is FormatException or OverflowException)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToInt32_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.Int32), ParameterDataType.Int32, value, ex);
		}
	}

	/// <summary>
	/// Converts a <see cref="string"/> into a <see cref="long"/>.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <returns>An <see cref="long"/> equivalent of the string.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is <see langword="null"/>, or is not in the correct format, or represents a number less than <see cref="long.MinValue"/> or greater than <see cref="long.MaxValue"/>.</exception>
	public static long ToInt64(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToInt64_ValueNull), nameof(value));
		}
		try
		{
			return XmlConvert.ToInt64(value);
		}
		catch (Exception ex) when (ex is FormatException or OverflowException)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToInt64_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.Int64), ParameterDataType.Int64, value, ex);
		}
	}

	/// <summary>
	/// Converts a <see cref="string"/> into a <see cref="sbyte"/>.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <returns>An <see cref="sbyte"/> equivalent of the string.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is <see langword="null"/>, or is not in the correct format, or represents a number less than <see cref="sbyte.MinValue"/> or greater than <see cref="sbyte.MaxValue"/>.</exception>
	[CLSCompliant(false)]
	public static sbyte ToSByte(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToSByte_ValueNull), nameof(value));
		}
		try
		{
			return XmlConvert.ToSByte(value);
		}
		catch (Exception ex) when (ex is FormatException or OverflowException)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToSByte_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.Int64), ParameterDataType.Int64, value, ex);
		}
	}

	/// <summary>
	/// Converts the <see cref="string"/> to a <see cref="TimeSpan"/> equivalent.
	/// </summary>
	/// <param name="value">The string to convert. The string format must conform to the W3C XML Schema Part 2: Datatypes recommendation for duration.</param>
	/// <returns>A <see cref="TimeSpan"/> equivalent of the string.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format, or represents a time that is out of range of <see cref="TimeSpan"/>.</exception>
	public static TimeSpan ToTimeSpan(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToTimeSpan_ValueNull), nameof(value));
		}

		try
		{
			return XmlConvert.ToTimeSpan(value);
		}
		catch (Exception ex) when (ex is FormatException or OverflowException)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToTimeSpan_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.TimeSpan), ParameterDataType.TimeSpan, value, ex);
		}
	}

	/// <summary>
	/// Converts a <see cref="string"/> into a <see cref="ushort"/>.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <returns>An <see cref="ushort"/> equivalent of the string.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format, or represents a number less than <see cref="ushort.MinValue"/> or greater than <see cref="ushort.MaxValue"/>.</exception>
	[CLSCompliant(false)]
	public static ushort ToUInt16(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToUInt16_ValueNull), nameof(value));
		}
		try
		{
			return XmlConvert.ToUInt16(value);
		}
		catch (Exception ex) when (ex is FormatException or OverflowException)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToUInt16_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.UInt16), ParameterDataType.UInt16, value, ex);
		}
	}

	/// <summary>
	/// Converts a <see cref="string"/> into a <see cref="uint"/>.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <returns>An <see cref="uint"/> equivalent of the string.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format, or represents a number less than <see cref="uint.MinValue"/> or greater than <see cref="uint.MaxValue"/>.</exception>
	[CLSCompliant(false)]
	public static uint ToUInt32(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToUInt32_ValueNull), nameof(value));
		}
		try
		{
			return XmlConvert.ToUInt32(value);
		}
		catch (Exception ex) when (ex is FormatException or OverflowException)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToUInt32_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.UInt32), ParameterDataType.UInt32, value, ex);
		}
	}

	/// <summary>
	/// Converts a <see cref="string"/> into a <see cref="ulong"/>.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <returns>An <see cref="ulong"/> equivalent of the string.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is not in the correct format, or represents a number less than <see cref="ulong.MinValue"/> or greater than <see cref="ulong.MaxValue"/>.</exception>
	[CLSCompliant(false)]
	public static ulong ToUInt64(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToUInt64_ValueNull), nameof(value));
		}
		try
		{
			return XmlConvert.ToUInt64(value);
		}
		catch (Exception ex) when (ex is FormatException or OverflowException)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToUInt64_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.UInt64), ParameterDataType.UInt64, value, ex);
		}
	}

	/// <summary>
	/// Converts a <see cref="string"/> into a <see cref="Uri"/>.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <returns>An <see cref="Uri"/> equivalent of the string.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> is not a valid URI.</exception>
	public static Uri ToUri(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToUri_ValueNull), nameof(value));
		}
		try
		{
			return new Uri(value);
		}
		catch (FormatException ex)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToUri_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.Uri), ParameterDataType.Uri, value, ex);
		}
	}

	/// <summary>
	/// Converts a <see cref="string"/> into a <see cref="Version"/> object.
	/// </summary>
	/// <param name="value">The string to convert.</param>
	/// <returns>An <see cref="Version"/> equivalent of the string.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException"><paramref name="value"/> has fewer than two or more than 4 version components,
	/// or at least one component is less than zero, or at least one component is not an integer
	/// or at least one component is greater than <see cref="int.MaxValue"/>.</exception>
	public static Version ToVersion(string value)
	{
		if (value is null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ToVersion_ValueNull), nameof(value));
		}
		try
		{
			return Version.Parse(value);
		}
		catch (Exception ex) when (ex is FormatException or ArgumentException or ArgumentOutOfRangeException or OverflowException)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToVersion_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_To_Failed, value, ParameterDataType.Version), ParameterDataType.Version, value, ex);
		}
	}

	/// <summary>
	/// Creates an object from its XML representation.
	/// </summary>
	/// <param name="value">An XML string containing the data of the object to create. </param>
	/// <param name="type">The type of object to create.</param>
	/// <returns>An object created from the XML data in <paramref name="value"/>.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> or <paramref name="type"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException">An error occurred during deserialization.</exception>
	public static object FromXml(string value, Type type)
	{
		if (value == null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_FromXml_ValueNull), nameof(value));
		}

		if (type == null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_FromXml_TypeNull), nameof(type));
		}

		try
		{
			XmlSerializer Serializer = new(type);
			using StringReader reader = new(value);
			using XmlReader xmlReader = XmlReader.Create(reader);
			return Serializer.Deserialize(xmlReader) ?? throw new CodedSerializationException(HResult.Create(ErrorCodes.ParameterConvert_FromXml_NoObjectDeserialized), TextResources.ParameterConvert_FromXml_NoObjectDeserialized);
		}
		catch (InvalidOperationException ex)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_FromXml_ParseFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_FromXml_Failed, type.FullName), ParameterDataType.Xml, value, ex.InnerException);
		}
	}

	/// <summary>
	/// Creates an object from its XML representation.
	/// </summary>
	/// <typeparam name="T">The type of object to create.</typeparam>
	/// <param name="value">An XML string containing the data of the object to create. </param>
	/// <returns>An object created from the XML data in <paramref name="value"/>.</returns>
	/// <exception cref="CodedArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	/// <exception cref="ParameterConversionException">An error occurred during deserialization.</exception>
	public static T FromXml<T>(string value) => (T)FromXml(value, typeof(T));

	/// <summary>
	/// Converts the specified <see cref="object"/> into its XML representation.
	/// </summary>
	/// <param name="value">The object to convert.</param>
	/// <returns><see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>; otherwise, an XML-formatted string representing <paramref name="value"/>.</returns>
	/// <exception cref="ParameterConversionException">An error occurred during XML serialization. See inner exception for details.</exception>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? ToXml(object? value)
	{
		if (value == null)
			return null;

		try
		{
			XmlSerializer Serializer = new(value.GetType());
			using StringWriter writer = new();
			Serializer.Serialize(writer, value);
			writer.Flush();
			return writer.ToString();
		}
		catch (InvalidOperationException ex)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_ToXml_SerializeFailed), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_ToXml_Failed, value.GetType().FullName), ParameterDataType.Xml, value, ex);
		}
	}

	/// <summary>
	/// Encrypt the specified string using AES encryption.
	/// </summary>
	/// <param name="value">The value to encrypt.</param>
	/// <returns>A base64-encoded string, or <see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>.</returns>
	/// <remarks>This method is not supported in the portable version.</remarks>
	/// <exception cref="ParameterConversionException">Cannot encrypt <paramref name="value"/>.</exception>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? Encrypt(string? value)
	{
		if (value == null)
		{
			return null;
		}

		try
		{
			using ICryptoTransform transform = s_aes.Value.CreateEncryptor();
			using MemoryStream encryptedBuffer = new();
			using CryptoStream encryptingStream = new(encryptedBuffer, transform, CryptoStreamMode.Write);
			using StreamWriter writer = new(encryptingStream, s_utf8NoBOM);
			writer.Write(value);
			writer.Flush();
			encryptingStream.FlushFinalBlock();
			return Convert.ToBase64String(encryptedBuffer.GetBuffer(), 0, (int)encryptedBuffer.Position);
		}
		catch (Exception ex) when (ex is CryptographicException or IOException)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_Encrypt_Failed), TextResources.ParameterConvert_Encrypt_Failed, ex.InnerException);
		}
	}

	/// <summary>
	/// Decrypts the specified string using AES encryption.
	/// </summary>
	/// <param name="value">The value to encrypt. The string must be base64-encoded.</param>
	/// <returns>A string, or <see langword="null"/>, if <paramref name="value"/> is <see langword="null"/>.</returns>
	/// <remarks>This method is not supported in the portable version.</remarks>
	/// <exception cref="ParameterConversionException">Cannot decrypt <paramref name="value"/>.</exception>
#if !NETSTD20
	[return: NotNullIfNotNull("value")]
#endif
	public static string? Decrypt(string? value)
	{
		if (value == null)
		{
			return null;
		}

		try
		{
			byte[] encbuffer = Convert.FromBase64String(value);
			using ICryptoTransform transform = s_aes.Value.CreateDecryptor();
			using MemoryStream encryptedBuffer = new(encbuffer);
			using CryptoStream decryptingStream = new(encryptedBuffer, transform, CryptoStreamMode.Read);
			using StreamReader Reader = new(decryptingStream, s_utf8NoBOM);
			return Reader.ReadToEnd();
		}
		catch (Exception ex) when (ex is CryptographicException or FormatException or IOException)
		{
			throw new ParameterConversionException(HResult.Create(ErrorCodes.ParameterConvert_Decrypt_Failed), TextResources.ParameterConvert_Decrypt_Failed, ex.InnerException);
		}
	}

	/// <summary>
	/// Tries to find a <see cref="TypeConstraint"/> or <see cref="EnumTypeConstraint"/> in a list of constraints.
	/// </summary>
	/// <param name="constraints">A list of <see cref="Constraint"/>s to search in.</param>
	/// <param name="constraint">When the method returns, contains the <see cref="TypeConstraint"/> found in <paramref name="constraints"/>, or <see langword="null"/>, if no matching constraint was found.</param>
	/// <returns><see langword="true"/>, if a <see cref="TypeConstraint"/> was found in <paramref name="constraints"/>; <see langword="false"/>, otherwise.</returns>
	public static bool TryGetTypeConstraint(IReadOnlyList<Constraint>? constraints, out TypeConstraint? constraint)
	{
		if (constraints == null)
		{
			constraint = null;
			return false;
		}
		constraint = (TypeConstraint?)constraints.FirstOrDefault(c => c.Name == Constraint.TypeConstraintName);
		return constraint != null;
	}

	/// <summary>
	/// Checks if the specified list contains a <see cref="EncryptedConstraint"/>.
	/// </summary>
	/// <param name="constraints">A list of <see cref="Constraint"/>s to search in. May be <see langword="null"/>.</param>
	/// <returns><see langword="true"/>, if <paramref name="constraints"/> contains an <see cref="EncryptedConstraint"/>; otherwise, or if <paramref name="constraints"/> is <see langword="null"/>, <see langword="false"/>.</returns>
	public static bool HasEncryptedConstraint(IReadOnlyList<Constraint>? constraints) => constraints != null && constraints.Any(c => c.Name == Constraint.EncryptedConstraintName);

	/// <summary>
	/// Checks if the specified list contains a <see cref="EncryptedConstraint"/>.
	/// </summary>
	/// <param name="constraints">A list of <see cref="Constraint"/>s to search in. May be <see langword="null"/>.</param>
	/// <returns><see langword="true"/>, if <paramref name="constraints"/> contains an <see cref="EncryptedConstraint"/>; otherwise, or if <paramref name="constraints"/> is <see langword="null"/>, <see langword="false"/>.</returns>
	public static bool HasNullConstraint(IReadOnlyList<Constraint>? constraints) => constraints != null && constraints.Any(c => c.Name == Constraint.NullConstraintName);

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
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_NetToParameterDataType_ArgNull), nameof(type));
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
		return info.IsEnum
			? ParameterDataType.Enum
			: info.ImplementedInterfaces.Contains(typeof(IXmlSerializable))
			? ParameterDataType.Xml
			: throw new CodedArgumentException(HResult.Create(ErrorCodes.ParameterConvert_NetToParameterDataType_TypeNotSupported), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_NetToParameterDataType_NoMatch, type.FullName), nameof(type));
	}

	/// <summary>
	/// Converts a <see cref="ParameterDataType"/> to a .Net type.
	/// </summary>
	/// <param name="type">A type.</param>
	/// <returns>A .Net type that is represented by <see cref="ParameterDataType"/>.</returns>
	/// <exception cref="CodedArgumentException">There is no matching representation of <paramref name="type"/>, e.g. for <see cref="ParameterDataType.Xml"/>.</exception>
	public static Type ParameterToNetDataType(ParameterDataType type) => type switch
	{
		ParameterDataType.Bool => typeof(bool),
		ParameterDataType.Byte => typeof(byte),
		ParameterDataType.Bytes => typeof(byte[]),
		ParameterDataType.DateTimeOffset => typeof(DateTimeOffset),
		ParameterDataType.Decimal => typeof(decimal),
		ParameterDataType.Guid => typeof(Guid),
		ParameterDataType.Int16 => typeof(short),
		ParameterDataType.Int32 => typeof(int),
		ParameterDataType.Int64 => typeof(long),
		ParameterDataType.String => typeof(string),
		ParameterDataType.SignedByte => typeof(sbyte),
		ParameterDataType.TimeSpan => typeof(TimeSpan),
		ParameterDataType.UInt16 => typeof(ushort),
		ParameterDataType.UInt32 => typeof(uint),
		ParameterDataType.UInt64 => typeof(ulong),
		ParameterDataType.Uri => typeof(Uri),
		ParameterDataType.Version => typeof(Version),
		ParameterDataType.Xml => typeof(object),
		_ => throw new CodedArgumentException(HResult.Create(ErrorCodes.ParameterConvert_ParameterToNetDataType_NoMatch), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_ParameterToNetDataType_NoMatch, type), nameof(type)),
	};

	/// <summary>
	/// Gets the integer type that the enumeration is based on.
	/// </summary>
	/// <param name="enumType">The enumeration type.</param>
	/// <returns>An integer type.</returns>
	internal static Type? GetEnumUnderlyingType(Type enumType)
	{
		FieldInfo[] fields = enumType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		return fields == null || fields.Length != 1 ? null : fields[0].FieldType;
	}

	/// <summary>
	/// Examines and returns the values, underlying type and FlagsAttribute of an enumeration type.
	/// </summary>
	/// <param name="enumType">The enumeration type to examine.</param>
	/// <param name="convertValues">A value indicating if the value in the returned dictionary should be converted to the enumeration's underlying type, instead of the original enumeration type.</param>
	/// <param name="underlyingType">When the method returns, the underlying type of the enumeration specified by <paramref name="enumType"/>.</param>
	/// <param name="hasFlags">When the method returns, a value indicating if the enumeration has a FlagsAttribute.</param>
	/// <returns></returns>
	internal static Dictionary<string, object> ExamineEnumeration(Type enumType, bool convertValues, out Type underlyingType, out bool hasFlags)
	{
		hasFlags = false;

		if (enumType == null)
		{
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.ParameterConvert_ExamineEnumeration_TypeNull), nameof(enumType));
		}

		TypeInfo EnumInfo = enumType.GetTypeInfo();
		if (!EnumInfo.IsEnum)
		{
			throw new CodedArgumentException(HResult.Create(ErrorCodes.ParameterConvert_ExamineEnumeration_NotEnum), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_ExamineEnumeration_NotEnum, enumType.FullName), nameof(enumType), null);
		}

		underlyingType = GetEnumUnderlyingType(enumType) ?? throw new CodedInvalidOperationException(HResult.Create(ErrorCodes.ParameterConvert_ExamineEnumeration_UnknownType), string.Format(CultureInfo.CurrentCulture, TextResources.Global_UnknownEnumType, enumType.GetType()));

		if (EnumInfo.GetCustomAttribute(typeof(FlagsAttribute)) != null)
		{
			hasFlags = true;
		}

		Dictionary<string, object> result = new();
		foreach (FieldInfo field in EnumInfo.DeclaredFields)
		{
			if (field.IsLiteral)
			{
				if (convertValues)
				{
					result.Add(field.Name, field.GetRawConstantValue()
						?? throw new CodedInvalidOperationException(HResult.Create(ErrorCodes.ParameterConvert_ExamineEnumeration_NoFieldValue), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_ExamineEnumeration_NoFieldValue, enumType.GetType())));
				}
				else
				{
					result.Add(field.Name, field.GetValue(null)
						?? throw new CodedInvalidOperationException(HResult.Create(ErrorCodes.ParameterConvert_ExamineEnumeration_NoFieldValue), string.Format(CultureInfo.CurrentCulture, TextResources.ParameterConvert_ExamineEnumeration_NoFieldValue, enumType.GetType())));
				}
			}
		}

		return result;

	}

	/// <summary>
	/// Checks if the specified type represents a an integer type.
	/// </summary>
	/// <param name="type">The type to check.</param>
	/// <returns><see langword="true"/>, if the type is an integer type; <see langword="false"/>, otherwise.</returns>
	internal static bool IsIntegerType(ParameterDataType type) => type switch
	{
		ParameterDataType.Int32 or ParameterDataType.Byte or ParameterDataType.Int16 or ParameterDataType.Int64 or ParameterDataType.SignedByte or ParameterDataType.UInt16 or ParameterDataType.UInt32 or ParameterDataType.UInt64 => true,
		_ => false,
	};

	/// <summary>
	/// Checks if the specified type represents a an integer type.
	/// </summary>
	/// <param name="type">The type to check.</param>
	/// <returns><see langword="true"/>, if the type is an integer type; <see langword="false"/>, otherwise.</returns>
	internal static bool IsIntegerType(Type type) => type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte) ||
			type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort) || type == typeof(sbyte);
}
