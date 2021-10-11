#region Copyright
/*******************************************************************************
 * NerdyDuck.Tests.Collections - Unit tests for the
 * NerdyDuck.ParameterValidation assembly
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NerdyDuck.CodedExceptions;
using NerdyDuck.ParameterValidation;
using NerdyDuck.ParameterValidation.Constraints;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NerdyDuck.Tests.ParameterValidation.Constraints
{
#if NET60
	namespace Net60
#elif NET50
	namespace Net50
#elif NETCORE31
	namespace NetCore31
#elif NET48
	namespace Net48
#endif
	{
		/// <summary>
		/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.MinimumValueConstraint class.
		/// </summary>
		[ExcludeFromCodeCoverage]
		[TestClass]
		public class MinimumValueConstraintTest
		{
			[TestMethod]
			public void Ctor_ByteNoValue_Success()
			{
				ParameterDataType type = ParameterDataType.Byte;
				MinimumValueConstraint c = new MinimumValueConstraint(type);
				Assert.AreEqual(Constraint.MinimumValueConstraintName, c.Name);
				Assert.AreEqual(type, c.DataType);
				Assert.AreEqual(byte.MaxValue, c.MinimumValue);
			}

			[TestMethod]
			public void Ctor_ByteWithValue_Success()
			{
				ParameterDataType type = ParameterDataType.Byte;
				object value = (byte)42;
				MinimumValueConstraint c = new MinimumValueConstraint(type, value);
				Assert.AreEqual(Constraint.MinimumValueConstraintName, c.Name);
				Assert.AreEqual(type, c.DataType);
				Assert.AreEqual(value, c.MinimumValue);
			}

			[TestMethod]
			public void Ctor_ByteWithConversion_Success()
			{
				ParameterDataType type = ParameterDataType.Byte;
				object value = (int)42;
				MinimumValueConstraint c = new MinimumValueConstraint(type, value);
				Assert.AreEqual(Constraint.MinimumValueConstraintName, c.Name);
				Assert.AreEqual(type, c.DataType);
				Assert.AreEqual((byte)42, c.MinimumValue);
			}

			[TestMethod]
			public void Ctor_ByteWithAltConversion_Success()
			{
				ParameterDataType type = ParameterDataType.Int16;
				object value = (long)42;
				MinimumValueConstraint c = new MinimumValueConstraint(type, value);
				Assert.AreEqual(Constraint.MinimumValueConstraintName, c.Name);
				Assert.AreEqual(type, c.DataType);
				Assert.AreEqual((short)42, c.MinimumValue);
			}

			[TestMethod]
			public void Ctor_TypeNotSupported_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentException>(() =>
				{
					ParameterDataType type = ParameterDataType.Uri;
					MinimumValueConstraint c = new MinimumValueConstraint(type);
				});
			}

			[TestMethod]
			public void Ctor_ValueNull_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
				{
					ParameterDataType type = ParameterDataType.Byte;
					object value = null;
					MinimumValueConstraint c = new MinimumValueConstraint(type, value);
				});
			}

			[TestMethod]
			public void Ctor_NotConvertible_Error()
			{
				CustomAssert.ThrowsException<ParameterConversionException>(() =>
				{
					ParameterDataType type = ParameterDataType.DateTimeOffset;
					object value = 42;
					MinimumValueConstraint c = new MinimumValueConstraint(type, value);
				});
			}

			[TestMethod]
			public void Ctor_ConversionFailed_Error()
			{
				CustomAssert.ThrowsException<ParameterConversionException>(() =>
				{
					ParameterDataType type = ParameterDataType.Byte;
					object value = 300;
					MinimumValueConstraint c = new MinimumValueConstraint(type, value);
				});
			}

			[TestMethod]
			public void Ctor_SerializationInfo_Success()
			{
				MinimumValueConstraint c = new MinimumValueConstraint(ParameterDataType.Int32, 42);
				System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
				MinimumValueConstraint c2 = SerializationHelper.Deserialize<MinimumValueConstraint>(Buffer);

				Assert.AreEqual(Constraint.MinimumValueConstraintName, c2.Name);
				Assert.AreEqual(ParameterDataType.Int32, c.DataType);
				Assert.AreEqual(42, c2.MinimumValue);
			}

			[TestMethod]
			public void ToString_Success()
			{
				MinimumValueConstraint c = new MinimumValueConstraint(ParameterDataType.Int32, 42);
				Assert.AreEqual("[MinValue(42)]", c.ToString());
			}

			[TestMethod]
			public void SetParameters_Success()
			{
				MinimumValueConstraint c = new MinimumValueConstraint(ParameterDataType.Int32);
				c.SetParametersInternal(new string[] { "1" }, ParameterDataType.Int32);
				Assert.AreEqual(1, c.MinimumValue);
			}

			[TestMethod]
			public void SetParameters_TooManyParams_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					MinimumValueConstraint c = new MinimumValueConstraint(ParameterDataType.Int32);
					c.SetParametersInternal(new string[] { "1", "2" }, ParameterDataType.Int32);
				});
			}

			[TestMethod]
			public void SetParameters_ParamInv_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					MinimumValueConstraint c = new MinimumValueConstraint(ParameterDataType.Int32);
					c.SetParametersInternal(new string[] { "narf" }, ParameterDataType.Int32);
				});
			}

			[TestMethod]
			public void Validate_Byte_Success()
			{
				ParameterDataType type = ParameterDataType.Byte;
				object minValue = (byte)42;
				object value = (byte)50;
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_ByteWithConvAltValue_Success()
			{
				ParameterDataType type = ParameterDataType.Byte;
				object minValue = (byte)42;
				object value = (int)44;
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_Int16WithSmallAltValue2_Success()
			{
				ParameterDataType type = ParameterDataType.Int16;
				object minValue = (short)42;
				object value = (int)-100000;
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_Int16WithSmallAltValue_Success()
			{
				ParameterDataType type = ParameterDataType.Int16;
				object minValue = (short)42;
				object value = (long)-100000;
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_DateTimeOffset_Success()
			{
				ParameterDataType type = ParameterDataType.DateTimeOffset;
				object minValue = DateTimeOffset.Now;
				object value = DateTimeOffset.Now.AddDays(1.0);
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_Decimal_Success()
			{
				ParameterDataType type = ParameterDataType.Decimal;
				object minValue = 42m;
				object value = 50m;
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_Int16_Success()
			{
				ParameterDataType type = ParameterDataType.Int16;
				object minValue = (short)42;
				object value = (short)50;
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_Int16Alt_Success()
			{
				ParameterDataType type = ParameterDataType.Int16;
				object minValue = (short)42;
				object value = (long)50;
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_Int32_Success()
			{
				ParameterDataType type = ParameterDataType.Int32;
				object minValue = 42;
				object value = 50;
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_Int32WithAltValue_Success()
			{
				ParameterDataType type = ParameterDataType.Int32;
				object minValue = 42;
				object value = (long)43;
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_Int32TooSmall_Success()
			{
				ParameterDataType type = ParameterDataType.Int32;
				object minValue = 51;
				object value = 50;
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_Int64_Success()
			{
				ParameterDataType type = ParameterDataType.Int64;
				object minValue = (long)42;
				object value = (long)50;
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_SignedByte_Success()
			{
				ParameterDataType type = ParameterDataType.SignedByte;
				object minValue = (sbyte)42;
				object value = (sbyte)50;
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_TimeSpan_Success()
			{
				ParameterDataType type = ParameterDataType.TimeSpan;
				object minValue = TimeSpan.FromSeconds(42.0);
				object value = TimeSpan.FromSeconds(50.0);
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_UInt16_Success()
			{
				ParameterDataType type = ParameterDataType.UInt16;
				object minValue = (ushort)42;
				object value = (ushort)50;
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_UInt32_Success()
			{
				ParameterDataType type = ParameterDataType.UInt32;
				object minValue = (uint)42;
				object value = (uint)50;
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_UInt64_Success()
			{
				ParameterDataType type = ParameterDataType.UInt64;
				object minValue = (ulong)42;
				object value = (ulong)50;
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_Version_Success()
			{
				ParameterDataType type = ParameterDataType.Version;
				object minValue = new Version(1, 1, 0, 0);
				object value = new Version(1, 1, 2, 0);
				MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_NotConvertible_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentException>(() =>
				{
					ParameterDataType type = ParameterDataType.DateTimeOffset;
					object minValue = DateTimeOffset.Now.AddDays(1.0);
					object value = 42;
					MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
					IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				});
			}

			[TestMethod]
			public void Validate_ConversionFailed_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentException>(() =>
				{
					ParameterDataType type = ParameterDataType.UInt16;
					object minValue = (ushort)50;
					object value = -1;
					MinimumValueConstraint c = new MinimumValueConstraint(type, minValue);
					IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
				});
			}
		}
	}
}
