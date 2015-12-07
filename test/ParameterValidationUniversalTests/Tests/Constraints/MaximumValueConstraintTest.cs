#region Copyright
/*******************************************************************************
 * <copyright file="MaximumValueConstraintTest.cs" owner="Daniel Kopp">
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
 * <assembly name="NerdyDuck.Tests.ParameterValidation">
 * Unit tests for NerdyDuck.ParameterValidation assembly.
 * </assembly>
 * <file name="MaximumValueConstraintTest.cs" date="2015-11-16">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.Constraints.MaximumValueConstraint class.
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
using NerdyDuck.ParameterValidation;
using NerdyDuck.ParameterValidation.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NerdyDuck.CodedExceptions;

namespace NerdyDuck.Tests.ParameterValidation.Constraints
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.MaximumValueConstraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class MaximumValueConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_ByteNoValue_Success()
		{
			ParameterDataType type = ParameterDataType.Byte;
			MaximumValueConstraint c = new MaximumValueConstraint(type);
			Assert.AreEqual(Constraint.MaximumValueConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual(byte.MaxValue, c.MaximumValue);
		}

		[TestMethod]
		public void Ctor_ByteWithValue_Success()
		{
			ParameterDataType type = ParameterDataType.Byte;
			object value = (byte)42;
			MaximumValueConstraint c = new MaximumValueConstraint(type, value);
			Assert.AreEqual(Constraint.MaximumValueConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual(value, c.MaximumValue);
		}

		[TestMethod]
		public void Ctor_ByteWithConversion_Success()
		{
			ParameterDataType type = ParameterDataType.Byte;
			object value = (int)42;
			MaximumValueConstraint c = new MaximumValueConstraint(type, value);
			Assert.AreEqual(Constraint.MaximumValueConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual((byte)42, c.MaximumValue);
		}

		[TestMethod]
		public void Ctor_ByteWithAltConversion_Success()
		{
			ParameterDataType type = ParameterDataType.Int16;
			object value = (long)42;
			MaximumValueConstraint c = new MaximumValueConstraint(type, value);
			Assert.AreEqual(Constraint.MaximumValueConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual((short)42, c.MaximumValue);
		}

		[TestMethod]
		public void Ctor_TypeNotSupported_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentException>(() =>
			{
				ParameterDataType type = ParameterDataType.Uri;
				MaximumValueConstraint c = new MaximumValueConstraint(type);
			});
		}

		[TestMethod]
		public void Ctor_ValueNull_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				ParameterDataType type = ParameterDataType.Byte;
				object value = null;
				MaximumValueConstraint c = new MaximumValueConstraint(type, value);
			});
		}

		[TestMethod]
		public void Ctor_NotConvertible_Error()
		{
			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				ParameterDataType type = ParameterDataType.DateTimeOffset;
				object value = 42;
				MaximumValueConstraint c = new MaximumValueConstraint(type, value);
			});
		}

		[TestMethod]
		public void Ctor_ConversionFailed_Error()
		{
			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				ParameterDataType type = ParameterDataType.Byte;
				object value = 300;
				MaximumValueConstraint c = new MaximumValueConstraint(type, value);
			});
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			MaximumValueConstraint c = new MaximumValueConstraint(ParameterDataType.Int32, 42);
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			MaximumValueConstraint c2 = SerializationHelper.Deserialize<MaximumValueConstraint>(Buffer);

			Assert.AreEqual(Constraint.MaximumValueConstraintName, c2.Name);
			Assert.AreEqual(ParameterDataType.Int32, c.DataType);
			Assert.AreEqual(42, c2.MaximumValue);
		}
#endif
		#endregion

		#region Public methods
		[TestMethod]
		public void ToString_Success()
		{
			MaximumValueConstraint c = new MaximumValueConstraint(ParameterDataType.Int32, 42);
			Assert.AreEqual("[MaxValue(42)]", c.ToString());
		}

		[TestMethod]
		public void SetParameters_Success()
		{
			MaximumValueConstraint c = new MaximumValueConstraint(ParameterDataType.Int32);
			c.SetParametersInternal(new string[] { "1" }, ParameterDataType.Int32);
			Assert.AreEqual(1, c.MaximumValue);
		}

		[TestMethod]
		public void SetParameters_TooManyParams_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				MaximumValueConstraint c = new MaximumValueConstraint(ParameterDataType.Int32);
				c.SetParametersInternal(new string[] { "1", "2" }, ParameterDataType.Int32);
			});
		}

		[TestMethod]
		public void SetParameters_ParamInv_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				MaximumValueConstraint c = new MaximumValueConstraint(ParameterDataType.Int32);
				c.SetParametersInternal(new string[] { "narf" }, ParameterDataType.Int32);
			});
		}

		[TestMethod]
		public void Validate_Byte_Success()
		{
			ParameterDataType type = ParameterDataType.Byte;
			object value = (byte)42;
			object maxValue = (byte)50;
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_ByteWithConvAltValue_Success()
		{
			ParameterDataType type = ParameterDataType.Byte;
			object value = 42;
			object maxValue = (byte)50;
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_ByteWithLargeAltValue_Success()
		{
			ParameterDataType type = ParameterDataType.Byte;
			object value = 300;
			object maxValue = (byte)50;
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_DateTimeOffset_Success()
		{
			ParameterDataType type = ParameterDataType.DateTimeOffset;
			object value = DateTimeOffset.Now;
			object maxValue = DateTimeOffset.Now.AddDays(1.0);
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_Decimal_Success()
		{
			ParameterDataType type = ParameterDataType.Decimal;
			object value = 42m;
			object maxValue = 50m;
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_Int16_Success()
		{
			ParameterDataType type = ParameterDataType.Int16;
			object value = (short)42;
			object maxValue = (short)50;
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_Int16Alt_Success()
		{
			ParameterDataType type = ParameterDataType.Int16;
			object value = (long)42;
			object maxValue = (short)50;
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_Int32_Success()
		{
			ParameterDataType type = ParameterDataType.Int32;
			object value = 42;
			object maxValue = 50;
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_Int32WithAltValue_Success()
		{
			ParameterDataType type = ParameterDataType.Int32;
			object value = 42;
			object maxValue = (long)50;
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_Int32TooLarge_Success()
		{
			ParameterDataType type = ParameterDataType.Int32;
			object value = 51;
			object maxValue = 50;
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_Int64_Success()
		{
			ParameterDataType type = ParameterDataType.Int64;
			object value = (long)42;
			object maxValue = (long)50;
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_SignedByte_Success()
		{
			ParameterDataType type = ParameterDataType.SignedByte;
			object value = (sbyte)42;
			object maxValue = (sbyte)50;
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_TimeSpan_Success()
		{
			ParameterDataType type = ParameterDataType.TimeSpan;
			object value = TimeSpan.FromSeconds(42.0);
			object maxValue = TimeSpan.FromSeconds(50.0);
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_UInt16_Success()
		{
			ParameterDataType type = ParameterDataType.UInt16;
			object value = (ushort)42;
			object maxValue = (ushort)50;
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_UInt32_Success()
		{
			ParameterDataType type = ParameterDataType.UInt32;
			object value = (uint)42;
			object maxValue = (uint)50;
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_UInt64_Success()
		{
			ParameterDataType type = ParameterDataType.UInt64;
			object value = (ulong)42;
			object maxValue = (ulong)50;
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
			IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_Version_Success()
		{
			ParameterDataType type = ParameterDataType.Version;
			object value = new Version(1, 1, 0, 0);
			object maxValue = new Version(1, 1, 2, 0);
			MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
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
				object value = 42;
				object maxValue = DateTimeOffset.Now.AddDays(1.0);
				MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			});
		}

		[TestMethod]
		public void Validate_ConversionFailed_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentException>(() =>
			{
				ParameterDataType type = ParameterDataType.UInt16;
				object value = -1;
				object maxValue = (ushort)50;
				MaximumValueConstraint c = new MaximumValueConstraint(type, maxValue);
				IEnumerable<ParameterValidationResult> result = c.Validate(value, type, Constants.MemberName);
			});
		}
		#endregion
	}
}
