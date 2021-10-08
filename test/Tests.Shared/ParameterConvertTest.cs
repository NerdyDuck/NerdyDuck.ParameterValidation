#region Copyright
/*******************************************************************************
 * <copyright file="ParameterConvertTest.cs" owner="Daniel Kopp">
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
 * <assembly name="NerdyDuck.Tests.ParameterValidation">
 * Unit tests for NerdyDuck.ParameterValidation assembly.
 * </assembly>
 * <file name="ParameterConvertTest.cs" date="2015-11-19">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.ParameterConvert class.
 * </file>
 ******************************************************************************/
#endregion

#if WINDOWS_UWP
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif
#if WINDOWS_DESKTOP
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
#endif
using NerdyDuck.CodedExceptions;
using NerdyDuck.ParameterValidation;
using NerdyDuck.ParameterValidation.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NerdyDuck.Tests.ParameterValidation
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.ParameterConvert class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class ParameterConvertTest
	{
		#region ToString Bool
		[TestMethod]
		public void ToString_Bool_Success()
		{
			bool? value = true;
			string str = ParameterConvert.ToString(value);
			Assert.AreEqual("true", str);
		}

		[TestMethod]
		public void ToString_BoolNull_Success()
		{
			bool? value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString Byte
		[TestMethod]
		public void ToString_Byte_Success()
		{
			byte? value = 42;
			string str = ParameterConvert.ToString(value);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_ByteNull_Success()
		{
			byte? value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString Bytes
		[TestMethod]
		public void ToString_Bytes_Success()
		{
			byte[] value = new byte[] { 42, 27, 10 };
			string str = ParameterConvert.ToString(value);
			Assert.AreEqual("KhsK", str);
		}

		[TestMethod]
		public void ToString_BytesEmpty_Success()
		{
			byte[] value = new byte[0];
			string str = ParameterConvert.ToString(value);
			Assert.AreEqual("", str);
		}

		[TestMethod]
		public void ToString_BytesNull_Success()
		{
			byte[] value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString DateTimeOffset
		[TestMethod]
		public void ToString_DateTimeOffset_Success()
		{
			DateTimeOffset? value = DateTimeOffset.Now;
			string str = ParameterConvert.ToString(value);
			StringAssert.StartsWith(str, value.Value.Year.ToString());
		}

		[TestMethod]
		public void ToString_DateTimeOffsetNull_Success()
		{
			DateTimeOffset? value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString Decimal
		[TestMethod]
		public void ToString_Decimal_Success()
		{
			decimal? value = 42.27m;
			string str = ParameterConvert.ToString(value);
			StringAssert.StartsWith(str, "42");
		}

		[TestMethod]
		public void ToString_DecimalNull_Success()
		{
			decimal? value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString Enum
		[TestMethod]
		public void ToString_Enum_Success()
		{
			System.DayOfWeek value = DayOfWeek.Friday;
			string str = ParameterConvert.ToString(value);
			Assert.AreEqual("5", str);
		}

		[TestMethod]
		public void ToString_EnumNull_Success()
		{
			Enum value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString Guid
		[TestMethod]
		public void ToString_Guid_Success()
		{
			Guid? value = new Guid(new byte[] { 0x42, 0x27, 0x10, 0x17, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c });
			string str = ParameterConvert.ToString(value);
			StringAssert.StartsWith(str, "17102742-");
		}

		[TestMethod]
		public void ToString_GuidNull_Success()
		{
			Guid? value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString Int16
		[TestMethod]
		public void ToString_Int16_Success()
		{
			short? value = 42;
			string str = ParameterConvert.ToString(value);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_Int16Null_Success()
		{
			short? value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString Int32
		[TestMethod]
		public void ToString_Int32_Success()
		{
			int? value = 42;
			string str = ParameterConvert.ToString(value);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_Int32Null_Success()
		{
			int? value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString Int64
		[TestMethod]
		public void ToString_Int64_Success()
		{
			long? value = 42;
			string str = ParameterConvert.ToString(value);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_Int64Null_Success()
		{
			long? value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString SByte
		[TestMethod]
		public void ToString_SByte_Success()
		{
			sbyte? value = 42;
			string str = ParameterConvert.ToString(value);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_SByteNull_Success()
		{
			sbyte? value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString TimeSpan
		[TestMethod]
		public void ToString_TimeSpan_Success()
		{
			TimeSpan? value = TimeSpan.FromMinutes(1.5);
			string str = ParameterConvert.ToString(value);
			StringAssert.StartsWith(str, "PT1M30S");
		}

		[TestMethod]
		public void ToString_TimeSpanNull_Success()
		{
			TimeSpan? value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString UInt16
		[TestMethod]
		public void ToString_UInt16_Success()
		{
			ushort? value = 42;
			string str = ParameterConvert.ToString(value);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_UInt16Null_Success()
		{
			ushort? value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString UInt32
		[TestMethod]
		public void ToString_UInt32_Success()
		{
			uint? value = 42;
			string str = ParameterConvert.ToString(value);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_UInt32Null_Success()
		{
			uint? value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString UInt64
		[TestMethod]
		public void ToString_UInt64_Success()
		{
			ulong? value = 42;
			string str = ParameterConvert.ToString(value);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_UInt64Null_Success()
		{
			ulong? value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString Uri
		[TestMethod]
		public void ToString_Uri_Success()
		{
			Uri value = new Uri(Constants.TestUri);
			string str = ParameterConvert.ToString(value);
			Assert.AreEqual(Constants.TestUri, str);
		}

		[TestMethod]
		public void ToString_UriNull_Success()
		{
			Uri value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString Version
		[TestMethod]
		public void ToString_Version_Success()
		{
			Assert.AreEqual("1.2", ParameterConvert.ToString(new Version(1, 2)));
			Assert.AreEqual("1.2.3", ParameterConvert.ToString(new Version(1, 2, 3)));
			Assert.AreEqual("1.2.3.4", ParameterConvert.ToString(new Version(1, 2, 3, 4)));
		}

		[TestMethod]
		public void ToString_VersionNull_Success()
		{
			Version value = null;
			string str = ParameterConvert.ToString(value);
			Assert.IsNull(str);
		}
		#endregion

		#region ToString Object
		[TestMethod]
		public void ToString_ObjectNull_Success()
		{
			object value = null;
			string str = ParameterConvert.ToString(value, ParameterDataType.Int32, null);
			Assert.IsNull(str);
		}

		[TestMethod]
		public void ToString_ObjectTypeMismatch_Error()
		{
			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				object value = 42;
				string str = ParameterConvert.ToString(value, ParameterDataType.Guid, null);
			});
		}

		[TestMethod]
		public void ToString_ObjectBool_Success()
		{
			bool value = true;
			string str = ParameterConvert.ToString(value, ParameterDataType.Bool, null);
			Assert.AreEqual("true", str);
		}

		[TestMethod]
		public void ToString_ObjectByte_Success()
		{
			byte value = 42;
			string str = ParameterConvert.ToString(value, ParameterDataType.Byte, null);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_ObjectBytes_Success()
		{
			byte[] value = new byte[] { 42, 27, 10 };
			string str = ParameterConvert.ToString(value, ParameterDataType.Bytes, null);
			Assert.AreEqual("KhsK", str);
		}

		[TestMethod]
		public void ToString_ObjectDateTimeOffset_Success()
		{
			DateTimeOffset value = DateTimeOffset.Now;
			string str = ParameterConvert.ToString(value, ParameterDataType.DateTimeOffset, null);
			StringAssert.StartsWith(str, value.Year.ToString());
		}

		[TestMethod]
		public void ToString_ObjectDecimal_Success()
		{
			decimal value = 42.27m;
			string str = ParameterConvert.ToString(value, ParameterDataType.Decimal, null);
			StringAssert.StartsWith(str, "42");
		}

		[TestMethod]
		public void ToString_ObjectEnum_Success()
		{
			System.DayOfWeek value = DayOfWeek.Friday;
			string str = ParameterConvert.ToString(value, ParameterDataType.Enum, null);
			Assert.AreEqual("5", str);
		}

		[TestMethod]
		public void ToString_ObjectGuid_Success()
		{
			Guid value = new Guid(new byte[] { 0x42, 0x27, 0x10, 0x17, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c });
			string str = ParameterConvert.ToString(value, ParameterDataType.Guid, null);
			StringAssert.StartsWith(str, "17102742-");
		}

		[TestMethod]
		public void ToString_ObjectInt16_Success()
		{
			short value = 42;
			string str = ParameterConvert.ToString(value, ParameterDataType.Int16, null);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_ObjectInt32_Success()
		{
			int value = 42;
			string str = ParameterConvert.ToString(value, ParameterDataType.Int32, null);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_ObjectInt64_Success()
		{
			long value = 42;
			string str = ParameterConvert.ToString(value, ParameterDataType.Int64, null);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_ObjectSByte_Success()
		{
			sbyte value = 42;
			string str = ParameterConvert.ToString(value, ParameterDataType.SignedByte, null);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_ObjectString_Success()
		{
			string value = "narf";
			string str = ParameterConvert.ToString(value, ParameterDataType.String, null);
			Assert.AreEqual("narf", str);
		}

		[TestMethod]
		public void ToString_ObjectTimeSpan_Success()
		{
			TimeSpan value = TimeSpan.FromMinutes(1.5);
			string str = ParameterConvert.ToString(value, ParameterDataType.TimeSpan, null);
			StringAssert.StartsWith(str, "PT1M30S");
		}

		[TestMethod]
		public void ToString_ObjectUInt16_Success()
		{
			ushort value = 42;
			string str = ParameterConvert.ToString(value, ParameterDataType.UInt16, null);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_ObjectUInt32_Success()
		{
			uint value = 42;
			string str = ParameterConvert.ToString(value, ParameterDataType.UInt32, null);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_ObjectUInt64_Success()
		{
			ulong value = 42;
			string str = ParameterConvert.ToString(value, ParameterDataType.UInt64, null);
			Assert.AreEqual("42", str);
		}

		[TestMethod]
		public void ToString_ObjectUri_Success()
		{
			Uri value = new Uri(Constants.TestUri);
			string str = ParameterConvert.ToString(value, ParameterDataType.Uri, null);
			Assert.AreEqual(Constants.TestUri, str);
		}

		[TestMethod]
		public void ToString_ObjectVersion_Success()
		{
			Version value = new Version(1, 2);
			string str = ParameterConvert.ToString(value, ParameterDataType.Version, null);
			Assert.AreEqual("1.2", str);
		}

		[TestMethod]
		public void ToString_ObjectEncrypted_Success()
		{
			EncryptedConstraint c = new EncryptedConstraint();
			string str = ParameterConvert.ToString(Constants.TestMessage, ParameterDataType.String, new Constraint[] { c });
			Assert.AreEqual(Constants.EncryptedTestMessage, str);
		}

		[TestMethod]
		public void ToString_ObjectXml_Success()
		{
			XmlSerializableObject value = new XmlSerializableObject();
			value.MyValue = "narf";
			string str = ParameterConvert.ToString(value, ParameterDataType.Xml, null);
			StringAssert.Contains(str, "MyValue");
			StringAssert.Contains(str, "narf");
		}
		#endregion

		#region ToBoolean
		[TestMethod]
		public void ToBoolean_Success()
		{
			string value = "true";
			bool result = ParameterConvert.ToBoolean(value);
			Assert.AreEqual(true, result);
		}

		[TestMethod]
		public void ToBoolean_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				bool result = ParameterConvert.ToBoolean(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				bool result = ParameterConvert.ToBoolean("narf");
			});
		}
		#endregion

		#region ToByte
		[TestMethod]
		public void ToByte_Success()
		{
			string value = "42";
			byte result = ParameterConvert.ToByte(value);
			Assert.AreEqual(42, result);
		}

		[TestMethod]
		public void ToByte_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				byte result = ParameterConvert.ToByte(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				byte result = ParameterConvert.ToByte("narf");
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				byte result = ParameterConvert.ToByte("1000");
			});
		}
		#endregion

		#region ToByteArray
		[TestMethod]
		public void ToByteArray_Success()
		{
			string value = "KhsK";
			byte[] result = ParameterConvert.ToByteArray(value);
			byte[] testval = new byte[] { 42, 27, 10 };
			CollectionAssert.AreEqual(testval, result);
		}

		[TestMethod]
		public void ToByteArray_Empty_Success()
		{
			string value = "";
			byte[] result = ParameterConvert.ToByteArray(value);
			Assert.AreEqual(0, result.Length);
		}

		[TestMethod]
		public void ToByteArray_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				byte[] result = ParameterConvert.ToByteArray(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				byte[] result = ParameterConvert.ToByteArray("narf!");
			});
		}
		#endregion

		#region ToDateTimeOffset
		[TestMethod]
		public void ToDateTimeOffset_Success()
		{
			string value = "2007-08-03T22:05:13-07:00";
			DateTimeOffset result = ParameterConvert.ToDateTimeOffset(value);
			Assert.AreEqual(2007, result.Year);
		}

		[TestMethod]
		public void ToDateTimeOffset_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				DateTimeOffset result = ParameterConvert.ToDateTimeOffset(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				DateTimeOffset result = ParameterConvert.ToDateTimeOffset("narf");
			});
		}
		#endregion

		#region ToDecimal
		[TestMethod]
		public void ToDecimal_Success()
		{
			string value = "42.27";
			decimal result = ParameterConvert.ToDecimal(value);
			Assert.AreEqual(42.27m, result);
		}

		[TestMethod]
		public void ToDecimal_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				decimal result = ParameterConvert.ToDecimal(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				decimal result = ParameterConvert.ToDecimal("narf");
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				decimal result = ParameterConvert.ToDecimal("1" + System.Xml.XmlConvert.ToString(decimal.MaxValue));
			});
		}
		#endregion

		#region ToEnumeration
		[TestMethod]
		public void ToEnumeration_Success()
		{
			string value = "5";
			object result = ParameterConvert.ToEnumeration(value, typeof(System.DayOfWeek));
			Assert.AreEqual(DayOfWeek.Friday, result);
			value = "Friday";
			result = ParameterConvert.ToEnumeration<System.DayOfWeek>(value);
			Assert.AreEqual(DayOfWeek.Friday, result);
		}

		[TestMethod]
		public void ToEnumeration_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				object result = ParameterConvert.ToEnumeration("5", null);
			});

			CustomAssert.ThrowsException<CodedArgumentException>(() =>
			{
				object result = ParameterConvert.ToEnumeration("5", typeof(Uri));
			});

			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				object result = ParameterConvert.ToEnumeration(null, typeof(System.DayOfWeek));
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				object result = ParameterConvert.ToEnumeration("999999999999", typeof(System.DayOfWeek));
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				object result = ParameterConvert.ToEnumeration("narf", typeof(System.DayOfWeek));
			});
		}
		#endregion

		#region ToGuid
		[TestMethod]
		public void ToGuid_Success()
		{
			string value = "01020304-0506-0708-090a-0b0c0d0e0f00";
			Guid result = ParameterConvert.ToGuid(value);
			Assert.AreEqual(4, result.ToByteArray()[0]);
		}

		[TestMethod]
		public void ToGuid_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				Guid result = ParameterConvert.ToGuid(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				Guid result = ParameterConvert.ToGuid("narf");
			});
		}
		#endregion

		#region ToInt16
		[TestMethod]
		public void ToInt16_Success()
		{
			string value = "42";
			short result = ParameterConvert.ToInt16(value);
			Assert.AreEqual(42, result);
		}

		[TestMethod]
		public void ToInt16_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				short result = ParameterConvert.ToInt16(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				short result = ParameterConvert.ToInt16("narf");
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				short result = ParameterConvert.ToInt16("999999999999999");
			});
		}
		#endregion

		#region ToInt32
		[TestMethod]
		public void ToInt32_Success()
		{
			string value = "42";
			int result = ParameterConvert.ToInt32(value);
			Assert.AreEqual(42, result);
		}

		[TestMethod]
		public void ToInt32_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				int result = ParameterConvert.ToInt32(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				int result = ParameterConvert.ToInt32("narf");
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				int result = ParameterConvert.ToInt32("999999999999999");
			});
		}
		#endregion

		#region ToInt64
		[TestMethod]
		public void ToInt64_Success()
		{
			string value = "42";
			long result = ParameterConvert.ToInt64(value);
			Assert.AreEqual(42, result);
		}

		[TestMethod]
		public void ToInt64_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				long result = ParameterConvert.ToInt64(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				long result = ParameterConvert.ToInt64("narf");
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				long result = ParameterConvert.ToInt64("999999999999999999999");
			});
		}
		#endregion

		#region ToSByte
		[TestMethod]
		public void ToSByte_Success()
		{
			string value = "42";
			sbyte result = ParameterConvert.ToSByte(value);
			Assert.AreEqual(42, result);
		}

		[TestMethod]
		public void ToSByte_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				sbyte result = ParameterConvert.ToSByte(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				sbyte result = ParameterConvert.ToSByte("narf");
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				sbyte result = ParameterConvert.ToSByte("1000");
			});
		}
		#endregion

		#region ToTimeSpan
		[TestMethod]
		public void ToTimeSpan_Success()
		{
			string value = "PT1M30S";
			TimeSpan result = ParameterConvert.ToTimeSpan(value);
			Assert.AreEqual(90.0, result.TotalSeconds);
		}

		[TestMethod]
		public void ToTimeSpan_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				TimeSpan result = ParameterConvert.ToTimeSpan(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				TimeSpan result = ParameterConvert.ToTimeSpan("narf");
			});
		}
		#endregion

		#region ToUInt16
		[TestMethod]
		public void ToUInt16_Success()
		{
			string value = "42";
			ushort result = ParameterConvert.ToUInt16(value);
			Assert.AreEqual(42, result);
		}

		[TestMethod]
		public void ToUInt16_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				ushort result = ParameterConvert.ToUInt16(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				ushort result = ParameterConvert.ToUInt16("narf");
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				ushort result = ParameterConvert.ToUInt16("999999999999999");
			});
		}
		#endregion

		#region ToInt32
		[TestMethod]
		public void ToUInt32_Success()
		{
			string value = "42";
			uint result = ParameterConvert.ToUInt32(value);
			Assert.AreEqual((uint)42, result);
		}

		[TestMethod]
		public void ToUInt32_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				uint result = ParameterConvert.ToUInt32(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				uint result = ParameterConvert.ToUInt32("narf");
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				uint result = ParameterConvert.ToUInt32("999999999999999");
			});
		}
		#endregion

		#region ToInt64
		[TestMethod]
		public void ToUInt64_Success()
		{
			string value = "42";
			ulong result = ParameterConvert.ToUInt64(value);
			Assert.AreEqual((ulong)42, result);
		}

		[TestMethod]
		public void ToUInt64_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				ulong result = ParameterConvert.ToUInt64(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				ulong result = ParameterConvert.ToUInt64("narf");
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				ulong result = ParameterConvert.ToUInt64("999999999999999999999");
			});
		}
		#endregion

		#region ToUri
		[TestMethod]
		public void ToUri_Success()
		{
			Uri result = ParameterConvert.ToUri(Constants.TestUri);
			Assert.AreEqual(Constants.TestUri, result.ToString());
		}

		[TestMethod]
		public void ToUri_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				Uri result = ParameterConvert.ToUri(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				Uri result = ParameterConvert.ToUri("narf");
			});
		}
		#endregion

		#region ToURi
		[TestMethod]
		public void ToVersion_Success()
		{
			string value = "1.2";
			Version result = ParameterConvert.ToVersion(value);
			Assert.AreEqual(1, result.Major);
		}

		[TestMethod]
		public void ToVersion_NullOrInvalid_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				Version result = ParameterConvert.ToVersion(null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				Version result = ParameterConvert.ToVersion("narf");
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				Version result = ParameterConvert.ToVersion("1");
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				Version result = ParameterConvert.ToVersion("1.-2");
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				Version result = ParameterConvert.ToVersion("1.999999999999");
			});
		}
		#endregion

		#region ToDataType
		[TestMethod]
		public void ToDataType_Boolean_Success()
		{
			string value = "true";
			object result = ParameterConvert.ToDataType(value, ParameterDataType.Bool, null);
			Assert.AreEqual(true, result);
		}

		[TestMethod]
		public void ToDataType_Byte_Success()
		{
			string value = "42";
			object result = ParameterConvert.ToDataType(value, ParameterDataType.Byte, null);
			Assert.AreEqual((byte)42, result);
		}

		[TestMethod]
		public void ToDataType_Bytes_Success()
		{
			string value = "KhsK";
			byte[] result = (byte[])ParameterConvert.ToDataType(value, ParameterDataType.Bytes, null);
			byte[] testval = new byte[] { 42, 27, 10 };
			CollectionAssert.AreEqual(testval, result);
		}

		[TestMethod]
		public void ToDataType_DateTimeOffset_Success()
		{
			string value = "2007-08-03T22:05:13-07:00";
			DateTimeOffset result = (DateTimeOffset)ParameterConvert.ToDataType(value, ParameterDataType.DateTimeOffset, null);
			Assert.AreEqual(2007, result.Year);
		}

		[TestMethod]
		public void ToDataType_Decimal_Success()
		{
			string value = "42.27";
			object result = ParameterConvert.ToDataType(value, ParameterDataType.Decimal, null);
			Assert.AreEqual(42.27m, result);
		}

		[TestMethod]
		public void ToDataType_Enumeration_Success()
		{
			string value = "5";
			EnumTypeConstraint c = new EnumTypeConstraint(typeof(DayOfWeek).AssemblyQualifiedName);
			object result = ParameterConvert.ToDataType(value, ParameterDataType.Enum, new Constraint[] { c });
			Assert.AreEqual(DayOfWeek.Friday, result);
		}

		[TestMethod]
		public void ToDataType_EnumerationInvType_Error()
		{
			EnumTypeConstraint c = new EnumTypeConstraint("Hi.I.Am.Not, A.Real.Type");
			string value = "5";

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				object result = ParameterConvert.ToDataType(value, ParameterDataType.Enum, new Constraint[] { c });
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				object result = ParameterConvert.ToDataType(value, ParameterDataType.Enum, new Constraint[0]);
			});
		}

		[TestMethod]
		public void ToDataType_Guid_Success()
		{
			string value = "01020304-0506-0708-090a-0b0c0d0e0f00";
			Guid result = (Guid)ParameterConvert.ToDataType(value, ParameterDataType.Guid, null);
			Assert.AreEqual(4, result.ToByteArray()[0]);
		}

		[TestMethod]
		public void ToDataType_Int16_Success()
		{
			string value = "42";
			object result = ParameterConvert.ToDataType(value, ParameterDataType.Int16, null);
			Assert.AreEqual((short)42, result);
		}

		[TestMethod]
		public void ToDataType_Int32_Success()
		{
			string value = "42";
			object result = ParameterConvert.ToDataType(value, ParameterDataType.Int32, null);
			Assert.AreEqual(42, result);
		}

		[TestMethod]
		public void ToDataType_Int64_Success()
		{
			string value = "42";
			object result = ParameterConvert.ToDataType(value, ParameterDataType.Int64, null);
			Assert.AreEqual(42L, result);
		}

		[TestMethod]
		public void ToDataType_String_Success()
		{
			string value = "narf";
			object result = ParameterConvert.ToDataType(value, ParameterDataType.String, null);
			Assert.AreEqual("narf", result);
		}

		[TestMethod]
		public void ToDataType_SByte_Success()
		{
			string value = "42";
			object result = ParameterConvert.ToDataType(value, ParameterDataType.SignedByte, null);
			Assert.AreEqual((sbyte)42, result);
		}

		[TestMethod]
		public void ToDataType_TimeSpan_Success()
		{
			string value = "PT1M30S";
			TimeSpan result = (TimeSpan)ParameterConvert.ToDataType(value, ParameterDataType.TimeSpan, null);
			Assert.AreEqual(90.0, result.TotalSeconds);
		}

		[TestMethod]
		public void ToDataType_UInt16_Success()
		{
			string value = "42";
			object result = ParameterConvert.ToDataType(value, ParameterDataType.UInt16, null);
			Assert.AreEqual((ushort)42, result);
		}

		[TestMethod]
		public void ToDataType_UInt32_Success()
		{
			string value = "42";
			object result = ParameterConvert.ToDataType(value, ParameterDataType.UInt32, null);
			Assert.AreEqual((uint)42, result);
		}

		[TestMethod]
		public void ToDataType_UInt64_Success()
		{
			string value = "42";
			object result = ParameterConvert.ToDataType(value, ParameterDataType.UInt64, null);
			Assert.AreEqual((ulong)42, result);
		}

		[TestMethod]
		public void ToDataType_Uri_Success()
		{
			Uri result = (Uri)ParameterConvert.ToDataType(Constants.TestUri, ParameterDataType.Uri, null);
			Assert.AreEqual(Constants.TestUri, result.ToString());
		}

		[TestMethod]
		public void ToDataType_Version_Success()
		{
			string value = "1.2";
			Version result = (Version)ParameterConvert.ToDataType(value, ParameterDataType.Version, null);
			Assert.AreEqual(1, result.Major);
		}

		[TestMethod]
		public void ToDataType_Xml_Success()
		{
			TypeConstraint c = new TypeConstraint(typeof(XmlSerializableObject).AssemblyQualifiedName);
			object result = ParameterConvert.ToDataType(Constants.SerializedObjectString, ParameterDataType.Xml, new Constraint[] { c });
			Assert.IsNotNull(result);
			Assert.AreEqual("narf", ((XmlSerializableObject)result).MyValue);
		}

		[TestMethod]
		public void ToDataType_Encrypted_Success()
		{
			EncryptedConstraint c = new EncryptedConstraint();
			object result = ParameterConvert.ToDataType(Constants.EncryptedTestMessage, ParameterDataType.String, new Constraint[] { c });
			Assert.AreEqual(Constants.TestMessage, result);
		}

		[TestMethod]
		public void ToDataType_XmlInvType_Error()
		{
			TypeConstraint c = new TypeConstraint("Hi.I.Am.Not, A.Real.Type");

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				object result = ParameterConvert.ToDataType(Constants.SerializedObjectString, ParameterDataType.Xml, new Constraint[] { c });
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				object result = ParameterConvert.ToDataType(Constants.SerializedObjectString, ParameterDataType.Xml, new Constraint[0]);
			});
		}
		#endregion

		#region ToXml
		[TestMethod]
		public void ToXml_Success()
		{
			XmlSerializableObject value = new XmlSerializableObject();
			value.MyValue = "narf";
			string result = ParameterConvert.ToXml(value);
			StringAssert.Contains(result, "MyValue");
			StringAssert.Contains(result, "narf");
		}

		[TestMethod]
		public void ToXml_Null_Success()
		{
			string result = ParameterConvert.ToXml(null);
			Assert.IsNull(result);
		}

		[TestMethod]
		public void ToXml_Invalid_Error()
		{
			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				XmlSerializableObject value = new XmlSerializableObject(true);
				value.MyValue = "narf";
				string result = ParameterConvert.ToXml(value);
			});
		}
		#endregion

		#region FromXml
		[TestMethod]
		public void FromXml_Success()
		{
			XmlSerializableObject result = (XmlSerializableObject)ParameterConvert.FromXml(Constants.SerializedObjectString, typeof(XmlSerializableObject));
			Assert.AreEqual("narf", result.MyValue);
			result = ParameterConvert.FromXml<XmlSerializableObject>(Constants.SerializedObjectString);
			Assert.AreEqual("narf", result.MyValue);
		}

		[TestMethod]
		public void FromXml_NullOrInv_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				ParameterConvert.FromXml(null, typeof(XmlSerializableObject));
			});

			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				ParameterConvert.FromXml(Constants.SerializedObjectString, null);
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				ParameterConvert.FromXml(Constants.SerializedObjectString.Replace("XmlSerializableObject", "AnotherObject"), typeof(XmlSerializableObject));
			});
		}
		#endregion

		#region GetEnumUnderlyingType
		[TestMethod]
		public void GetEnumUnderlyingType_NoEnum_Success()
		{
			Assert.IsNull(ParameterConvert.GetEnumUnderlyingType(typeof(XmlSerializableObject)));
		}
		#endregion

		#region IsIntegerType
		[TestMethod]
		public void IsIntegerType_Success()
		{
			Assert.IsTrue(ParameterConvert.IsIntegerType(typeof(int)));
			Assert.IsTrue(ParameterConvert.IsIntegerType(typeof(long)));
			Assert.IsTrue(ParameterConvert.IsIntegerType(typeof(short)));
			Assert.IsTrue(ParameterConvert.IsIntegerType(typeof(byte)));
			Assert.IsTrue(ParameterConvert.IsIntegerType(typeof(uint)));
			Assert.IsTrue(ParameterConvert.IsIntegerType(typeof(ulong)));
			Assert.IsTrue(ParameterConvert.IsIntegerType(typeof(ushort)));
			Assert.IsTrue(ParameterConvert.IsIntegerType(typeof(sbyte)));
			Assert.IsFalse(ParameterConvert.IsIntegerType(typeof(decimal)));
		}
		#endregion

		#region NetToParameterDataType
		[TestMethod]
		public void NetToParameterDataType_Success()
		{
			Assert.AreEqual(ParameterDataType.Bool, ParameterConvert.NetToParameterDataType(typeof(bool)));
			Assert.AreEqual(ParameterDataType.Byte, ParameterConvert.NetToParameterDataType(typeof(byte)));
			Assert.AreEqual(ParameterDataType.Bytes, ParameterConvert.NetToParameterDataType(typeof(byte[])));
			Assert.AreEqual(ParameterDataType.DateTimeOffset, ParameterConvert.NetToParameterDataType(typeof(DateTimeOffset)));
			Assert.AreEqual(ParameterDataType.Decimal, ParameterConvert.NetToParameterDataType(typeof(decimal)));
			Assert.AreEqual(ParameterDataType.Enum, ParameterConvert.NetToParameterDataType(typeof(DayOfWeek)));
			Assert.AreEqual(ParameterDataType.Guid, ParameterConvert.NetToParameterDataType(typeof(Guid)));
			Assert.AreEqual(ParameterDataType.Int16, ParameterConvert.NetToParameterDataType(typeof(short)));
			Assert.AreEqual(ParameterDataType.Int32, ParameterConvert.NetToParameterDataType(typeof(int)));
			Assert.AreEqual(ParameterDataType.Int64, ParameterConvert.NetToParameterDataType(typeof(long)));
			Assert.AreEqual(ParameterDataType.SignedByte, ParameterConvert.NetToParameterDataType(typeof(sbyte)));
			Assert.AreEqual(ParameterDataType.String, ParameterConvert.NetToParameterDataType(typeof(string)));
			Assert.AreEqual(ParameterDataType.TimeSpan, ParameterConvert.NetToParameterDataType(typeof(TimeSpan)));
			Assert.AreEqual(ParameterDataType.UInt16, ParameterConvert.NetToParameterDataType(typeof(ushort)));
			Assert.AreEqual(ParameterDataType.UInt32, ParameterConvert.NetToParameterDataType(typeof(uint)));
			Assert.AreEqual(ParameterDataType.UInt64, ParameterConvert.NetToParameterDataType(typeof(ulong)));
			Assert.AreEqual(ParameterDataType.Uri, ParameterConvert.NetToParameterDataType(typeof(Uri)));
			Assert.AreEqual(ParameterDataType.Version, ParameterConvert.NetToParameterDataType(typeof(Version)));
			Assert.AreEqual(ParameterDataType.Xml, ParameterConvert.NetToParameterDataType(typeof(XmlSerializableObject)));
		}

		[TestMethod]
		public void NetToParameterDataType_NullOrInv_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				ParameterConvert.NetToParameterDataType(null);
			});

			CustomAssert.ThrowsException<CodedArgumentException>(() =>
			{
				ParameterConvert.NetToParameterDataType(typeof(System.UriBuilder));
			});
		}
		#endregion

		#region ParameterToNetDataType
		[TestMethod]
		public void ParameterToNetDataType_Success()
		{
			Assert.AreEqual(typeof(object), ParameterConvert.ParameterToNetDataType(ParameterDataType.Xml));
		}

		[TestMethod]
		public void ParameterToNetDataType_TypeNon_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentException>(() =>
			{
				ParameterConvert.ParameterToNetDataType(ParameterDataType.None);
			});
		}
		#endregion

		#region TryGetTypeConstraint
		[TestMethod]
		public void TryGetTypeConstraint_Null_Success()
		{
			TypeConstraint c;
			Assert.IsFalse(ParameterConvert.TryGetTypeConstraint(null, out c));
		}
		#endregion

		#region Encrypt
		[TestMethod]
		public void Encrypt_Success()
		{
			string result = ParameterConvert.Encrypt(Constants.TestMessage);
			Assert.AreEqual(Constants.EncryptedTestMessage, result);
		}

		[TestMethod]
		public void Encrypt_Null_Success()
		{
			Assert.IsNull(ParameterConvert.Encrypt(null));
		}
		#endregion

		#region Decrypt
		[TestMethod]
		public void Decrypt_Success()
		{
			string result = ParameterConvert.Decrypt(Constants.EncryptedTestMessage);
			Assert.AreEqual(Constants.TestMessage, result);
		}

		[TestMethod]
		public void Decrypt_Null_Success()
		{
			Assert.IsNull(ParameterConvert.Decrypt(null));
		}

		[TestMethod]
		public void Decrypt_InvData_Error()
		{
			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				ParameterConvert.Decrypt("narf"); // CryptographicException
			});

			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				ParameterConvert.Decrypt("narf!"); // FormatException base64
			});
		}
		#endregion

		#region ExamineEnumeration
		[TestMethod]
		public void ExamineEnumeration_Success()
		{
			Type u;
			bool f;

			Dictionary<string, object> enums = ParameterConvert.ExamineEnumeration(typeof(System.DayOfWeek), false, out u, out f);
			Assert.IsNotNull(enums);
			Assert.AreEqual(7, enums.Count);
			Assert.AreEqual(typeof(int), u);
			Assert.IsFalse(f);
			Assert.IsInstanceOfType(enums["Monday"], typeof(System.DayOfWeek));
		}

		[TestMethod]
		public void ExamineEnumeration_Convert_Success()
		{
			Type u;
			bool f;

			Dictionary<string, object> enums = ParameterConvert.ExamineEnumeration(typeof(System.DayOfWeek), true, out u, out f);
			Assert.IsNotNull(enums);
			Assert.AreEqual(7, enums.Count);
			Assert.AreEqual(typeof(int), u);
			Assert.IsFalse(f);
			Assert.IsInstanceOfType(enums["Monday"], typeof(int));
		}

		[TestMethod]
		public void ExamineEnumeration_TypeNull_Error()
		{
			Type u;
			bool f;

			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				Dictionary<string, object> enums = ParameterConvert.ExamineEnumeration(null, false, out u, out f);
			});
		}
		#endregion
	}
}
