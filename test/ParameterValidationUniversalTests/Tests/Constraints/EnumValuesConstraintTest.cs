#region Copyright
/*******************************************************************************
 * <copyright file="EnumValuesConstraintTest.cs" owner="Daniel Kopp">
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
 * <file name="EnumValuesConstraintTest.cs" date="2015-11-16">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.Constraints.EnumValuesConstraint class.
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
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.EnumValuesConstraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class EnumValuesConstraintTest
	{
		#region Constructors
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

#if WINDOWS_DESKTOP
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
#endif
		#endregion

		#region Public methods
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

		#endregion

		#region Private methods
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
		#endregion
	}
}
