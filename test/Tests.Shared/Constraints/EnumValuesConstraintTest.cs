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
		/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.EnumValuesConstraint class.
		/// </summary>
		[ExcludeFromCodeCoverage]
		[TestClass]
		public class EnumValuesConstraintTest
		{
			[TestMethod]
			public void Ctor_Void_Success()
			{
				EnumValuesConstraint c = new EnumValuesConstraint();
				Assert.AreEqual(Constraint.EnumValuesConstraintName, c.Name);
				Assert.AreEqual(ParameterDataType.None, c.UnderlyingDataType);
				Assert.IsNull(c.UnderlyingType);
				Assert.IsFalse(c.HasFlags);
				Assert.IsNull(c.EnumValues);
			}

			[TestMethod]
			public void Ctor_Values_Success()
			{
				EnumValuesConstraint c = new EnumValuesConstraint(ParameterDataType.Int32, false, CreateValues());
				Assert.AreEqual(Constraint.EnumValuesConstraintName, c.Name);
				Assert.AreEqual(ParameterDataType.Int32, c.UnderlyingDataType);
				Assert.AreEqual(typeof(int), c.UnderlyingType);
				Assert.IsFalse(c.HasFlags);
				Assert.IsNotNull(c.EnumValues);
				Assert.AreEqual(3, c.EnumValues.Count);
			}

			[TestMethod]
			public void Ctor_FlagValues_Success()
			{
				EnumValuesConstraint c = new EnumValuesConstraint(ParameterDataType.Int32, true, CreateFlagValues());
				Assert.AreEqual(Constraint.EnumValuesConstraintName, c.Name);
				Assert.AreEqual(ParameterDataType.Int32, c.UnderlyingDataType);
				Assert.AreEqual(typeof(int), c.UnderlyingType);
				Assert.IsTrue(c.HasFlags);
				Assert.IsNotNull(c.EnumValues);
				Assert.AreEqual(8, c.EnumValues.Count);
			}

			[TestMethod]
			public void Ctor_InvalidType_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
				{
					EnumValuesConstraint c = new EnumValuesConstraint(ParameterDataType.String, false, CreateValues());
				});
			}

			[TestMethod]
			public void Ctor_ValuesNull_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
				{
					EnumValuesConstraint c = new EnumValuesConstraint(ParameterDataType.Int32, false, null);
				});
			}

			[TestMethod]
			public void Ctor_ValuesNotConvertable_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentException>(() =>
				{
					Dictionary<string, object> Values = new Dictionary<string, object>();
					Values.Add("RelativeOrAbsolute", 1000);
					Values.Add("Absolute", 2000);
					Values.Add("Relative", 3000);
					EnumValuesConstraint c = new EnumValuesConstraint(ParameterDataType.Byte, false, Values);
				});
			}

			[TestMethod]
			public void Ctor_ChangedType_Success()
			{
				EnumValuesConstraint c = new EnumValuesConstraint(ParameterDataType.Int64, false, CreateValues());
				Assert.AreEqual(Constraint.EnumValuesConstraintName, c.Name);
				Assert.AreEqual(ParameterDataType.Int64, c.UnderlyingDataType);
				Assert.AreEqual(typeof(long), c.UnderlyingType);
				Assert.IsFalse(c.HasFlags);
				Assert.IsNotNull(c.EnumValues); // To trigger retrieval of already resolved type
				Assert.AreEqual(3, c.EnumValues.Count);
			}

			[TestMethod]
			public void Ctor_SerializationInfo_Success()
			{
				EnumValuesConstraint c = new EnumValuesConstraint(ParameterDataType.Int32, false, CreateValues());
				System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
				EnumValuesConstraint c2 = SerializationHelper.Deserialize<EnumValuesConstraint>(Buffer);

				Assert.AreEqual(Constraint.EnumValuesConstraintName, c2.Name);
				Assert.AreEqual(ParameterDataType.Int32, c2.UnderlyingDataType);
				Assert.AreEqual(typeof(int), c2.UnderlyingType);
				Assert.IsFalse(c2.HasFlags);
				Assert.IsNotNull(c2.EnumValues); // To trigger retrieval of already resolved type
				Assert.AreEqual(3, c2.EnumValues.Count);
			}

			[TestMethod]
			public void ToString_Success()
			{
				EnumValuesConstraint c = new EnumValuesConstraint(ParameterDataType.Int32, false, CreateValues());
				Assert.AreEqual("[Values(Int32,RelativeOrAbsolute=1,Absolute=2,Relative=3)]", c.ToString());
				c = new EnumValuesConstraint(ParameterDataType.Int32, true, CreateFlagValues());
				StringAssert.StartsWith(c.ToString(), "[Values(Int32,Flags,ReadOnly=1,Hidden=2,System=4");
			}

			[TestMethod]
			public void SetParameters_Success()
			{
				EnumValuesConstraint c = new EnumValuesConstraint();
				c.SetParametersInternal(new string[] { "Int32", "RelativeOrAbsolute=1", "Absolute=2", "Relative=3" }, ParameterDataType.Enum);
				Assert.AreEqual(ParameterDataType.Int32, c.UnderlyingDataType);
				Assert.AreEqual(typeof(int), c.UnderlyingType);
				Assert.IsFalse(c.HasFlags);
				Assert.IsNotNull(c.EnumValues);
				Assert.AreEqual(3, c.EnumValues.Count);
			}

			[TestMethod]
			public void SetParameters_Flags_Success()
			{
				EnumValuesConstraint c = new EnumValuesConstraint();
				c.SetParametersInternal(new string[] { "Int32", "Flags", "ReadOnly=1", "Hidden=2", "System=4" }, ParameterDataType.Enum);
				Assert.AreEqual(ParameterDataType.Int32, c.UnderlyingDataType);
				Assert.AreEqual(typeof(int), c.UnderlyingType);
				Assert.IsTrue(c.HasFlags);
				Assert.IsNotNull(c.EnumValues);
				Assert.AreEqual(3, c.EnumValues.Count);
				Assert.AreEqual(4, c.EnumValues["System"]);
			}

			[TestMethod]
			public void SetParameters_FlagsHex_Success()
			{
				EnumValuesConstraint c = new EnumValuesConstraint();
				c.SetParametersInternal(new string[] { "Int32", "Flags", "ReadOnly=0x01", "Hidden=0x02", "System=0x04" }, ParameterDataType.Enum);
				Assert.AreEqual(ParameterDataType.Int32, c.UnderlyingDataType);
				Assert.AreEqual(typeof(int), c.UnderlyingType);
				Assert.IsTrue(c.HasFlags);
				Assert.IsNotNull(c.EnumValues);
				Assert.AreEqual(3, c.EnumValues.Count);
				Assert.AreEqual(4, c.EnumValues["System"]);
			}

			[TestMethod]
			public void SetParameters_TooFewParams_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					EnumValuesConstraint c = new EnumValuesConstraint();
					c.SetParametersInternal(new string[] { "Int32" }, ParameterDataType.Enum);
				});

				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					EnumValuesConstraint c = new EnumValuesConstraint();
					c.SetParametersInternal(new string[] { "Int32", "Flags" }, ParameterDataType.Enum);
				});
			}

			[TestMethod]
			public void SetParameters_InvDataType_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					EnumValuesConstraint c = new EnumValuesConstraint();
					c.SetParametersInternal(new string[] { "narf", "RelativeOrAbsolute=1", "Absolute=2", "Relative=3" }, ParameterDataType.Enum);
				});
			}

			[TestMethod]
			public void SetParameters_InvKeyValue_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					EnumValuesConstraint c = new EnumValuesConstraint();
					c.SetParametersInternal(new string[] { "Int32", "RelativeOrAbsolute1", "Absolute=2", "Relative=3" }, ParameterDataType.Enum);
				});
			}

			[TestMethod]
			public void SetParameters_InvValue_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					EnumValuesConstraint c = new EnumValuesConstraint();
					c.SetParametersInternal(new string[] { "Int32", "RelativeOrAbsolute=narf", "Absolute=2", "Relative=3" }, ParameterDataType.Enum);
				});
			}

			[TestMethod]
			public void SetParameters_InvHexValue_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					EnumValuesConstraint c = new EnumValuesConstraint();
					c.SetParametersInternal(new string[] { "Int32", "RelativeOrAbsolute=0xnarf", "Absolute=2", "Relative=3" }, ParameterDataType.Enum);
				});
			}

			[TestMethod]
			public void SetParameters_InvHexTooLarge_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					EnumValuesConstraint c = new EnumValuesConstraint();
					c.SetParametersInternal(new string[] { "Byte", "Flags", "RelativeOrAbsolute=0x1000", "Absolute=0x2000", "Relative=0x4000" }, ParameterDataType.Enum);
				});
			}

			[TestMethod]
			public void Validate_Success()
			{
				EnumValuesConstraint c = new EnumValuesConstraint(ParameterDataType.Int32, false, CreateValues());
				IEnumerable<ParameterValidationResult> result = c.Validate(System.UriKind.Absolute, ParameterDataType.Enum, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_NotFound_Success()
			{
				EnumValuesConstraint c = new EnumValuesConstraint(ParameterDataType.Int32, false, CreateValues());
				IEnumerable<ParameterValidationResult> result = c.Validate(5, ParameterDataType.Enum, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_DifferentType_Success()
			{
				EnumValuesConstraint c = new EnumValuesConstraint(ParameterDataType.Int32, false, CreateValues());
				IEnumerable<ParameterValidationResult> result = c.Validate((byte)2, ParameterDataType.Enum, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_Flags_Success()
			{
				EnumValuesConstraint c = new EnumValuesConstraint(ParameterDataType.Int32, true, CreateFlagValues());
				IEnumerable<ParameterValidationResult> result = c.Validate(System.IO.FileAttributes.Device | System.IO.FileAttributes.System, ParameterDataType.Enum, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_InvFlags_Success()
			{
				EnumValuesConstraint c = new EnumValuesConstraint(ParameterDataType.Int32, true, CreateFlagValues());
				IEnumerable<ParameterValidationResult> result = c.Validate(System.IO.FileAttributes.Device | System.IO.FileAttributes.ReparsePoint, ParameterDataType.Enum, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_NotConfigured_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					EnumValuesConstraint c = new EnumValuesConstraint();
					IEnumerable<ParameterValidationResult> result = c.Validate(System.UriKind.Absolute, ParameterDataType.Enum, Constants.MemberName);
				});
			}

			[TestMethod]
			public void Validate_ValueNotConvertible_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentException>(() =>
				{
					EnumValuesConstraint c = new EnumValuesConstraint(ParameterDataType.Int32, false, CreateValues());
					IEnumerable<ParameterValidationResult> result = c.Validate(long.MaxValue, ParameterDataType.Enum, Constants.MemberName);
				});
			}

			[TestMethod]
			public void FromType_Success()
			{
				EnumValuesConstraint c = EnumValuesConstraint.FromType(typeof(System.DayOfWeek));
				Assert.AreEqual(7, c.EnumValues.Count);
				Assert.AreEqual(typeof(int), c.UnderlyingType);
				Assert.IsFalse(c.HasFlags);
			}

			[TestMethod]
			public void GetEnumValuesDictionary_Success()
			{
				IDictionary<string, object> enums = EnumValuesConstraint.GetEnumValuesDictionary(typeof(System.DayOfWeek));
				Assert.AreEqual(7, enums.Count);
			}

			private Dictionary<string, object> CreateValues()
			{
				// From System.UriKing
				Dictionary<string, object> Values = new Dictionary<string, object>();
				Values.Add("RelativeOrAbsolute", 1);
				Values.Add("Absolute", 2);
				Values.Add("Relative", 3);
				return Values;
			}

			private Dictionary<string, object> CreateFlagValues()
			{
				// From System.IO.FileAttributes
				Dictionary<string, object> Values = new Dictionary<string, object>();
				Values.Add("ReadOnly", 1);
				Values.Add("Hidden", 2);
				Values.Add("System", 4);
				Values.Add("Directory", 16);
				Values.Add("Archive", 32);
				Values.Add("Device", 64);
				Values.Add("Normal", 128);
				Values.Add("Temporary", 256);
				return Values;
			}
		}
	}
}
