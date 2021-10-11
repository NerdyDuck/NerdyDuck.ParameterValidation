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

namespace NerdyDuck.Tests.ParameterValidation.Constraints
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.NullConstraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class StepConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Byte_Success()
		{
			ParameterDataType type = ParameterDataType.Byte;
			StepConstraint c = new StepConstraint(type);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual((byte)1, c.StepSize);
		}

		[TestMethod]
		public void Ctor_Decimal_Success()
		{
			ParameterDataType type = ParameterDataType.Decimal;
			StepConstraint c = new StepConstraint(type);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual(1m, c.StepSize);
		}

		[TestMethod]
		public void Ctor_Int16_Success()
		{
			ParameterDataType type = ParameterDataType.Int16;
			StepConstraint c = new StepConstraint(type);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual((short)1, c.StepSize);
		}

		[TestMethod]
		public void Ctor_Int32_Success()
		{
			ParameterDataType type = ParameterDataType.Int32;
			StepConstraint c = new StepConstraint(type);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual(1, c.StepSize);
		}

		[TestMethod]
		public void Ctor_Int64_Success()
		{
			ParameterDataType type = ParameterDataType.Int64;
			StepConstraint c = new StepConstraint(type);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual(1L, c.StepSize);
		}

		[TestMethod]
		public void Ctor_SByte_Success()
		{
			ParameterDataType type = ParameterDataType.SignedByte;
			StepConstraint c = new StepConstraint(type);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual((sbyte)1, c.StepSize);
		}

		[TestMethod]
		public void Ctor_UInt16_Success()
		{
			ParameterDataType type = ParameterDataType.UInt16;
			StepConstraint c = new StepConstraint(type);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual((ushort)1, c.StepSize);
		}

		[TestMethod]
		public void Ctor_UInt32_Success()
		{
			ParameterDataType type = ParameterDataType.UInt32;
			StepConstraint c = new StepConstraint(type);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual(1u, c.StepSize);
		}

		[TestMethod]
		public void Ctor_UInt64_Success()
		{
			ParameterDataType type = ParameterDataType.UInt64;
			StepConstraint c = new StepConstraint(type);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual((ulong)1, c.StepSize);
		}

		[TestMethod]
		public void Ctor_ByteObject_Success()
		{
			ParameterDataType type = ParameterDataType.Byte;
			StepConstraint c = new StepConstraint(type, 2);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual((byte)2, c.StepSize);
		}

		[TestMethod]
		public void Ctor_DecimalObject_Success()
		{
			ParameterDataType type = ParameterDataType.Decimal;
			StepConstraint c = new StepConstraint(type, 2);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual(2m, c.StepSize);
		}

		[TestMethod]
		public void Ctor_Int16Object_Success()
		{
			ParameterDataType type = ParameterDataType.Int16;
			StepConstraint c = new StepConstraint(type, 2);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual((short)2, c.StepSize);
		}

		[TestMethod]
		public void Ctor_Int32Object_Success()
		{
			ParameterDataType type = ParameterDataType.Int32;
			StepConstraint c = new StepConstraint(type, 2);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual(2, c.StepSize);
		}

		[TestMethod]
		public void Ctor_Int64Object_Success()
		{
			ParameterDataType type = ParameterDataType.Int64;
			StepConstraint c = new StepConstraint(type, 2);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual(2L, c.StepSize);
		}

		[TestMethod]
		public void Ctor_SByteObject_Success()
		{
			ParameterDataType type = ParameterDataType.SignedByte;
			StepConstraint c = new StepConstraint(type, 2);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual((sbyte)2, c.StepSize);
		}

		[TestMethod]
		public void Ctor_UInt16Object_Success()
		{
			ParameterDataType type = ParameterDataType.UInt16;
			StepConstraint c = new StepConstraint(type, 2);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual((ushort)2, c.StepSize);
		}

		[TestMethod]
		public void Ctor_UInt32Object_Success()
		{
			ParameterDataType type = ParameterDataType.UInt32;
			StepConstraint c = new StepConstraint(type, 2);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual((uint)2, c.StepSize);
		}

		[TestMethod]
		public void Ctor_UInt64Object_Success()
		{
			ParameterDataType type = ParameterDataType.UInt64;
			StepConstraint c = new StepConstraint(type, 2);
			Assert.AreEqual(Constraint.StepConstraintName, c.Name);
			Assert.AreEqual(type, c.DataType);
			Assert.AreEqual((ulong)2, c.StepSize);
		}

		[TestMethod]
		public void Ctor_InvalidType_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentException>(() =>
			{
				StepConstraint c = new StepConstraint(ParameterDataType.Enum);
			});
		}

		[TestMethod]
		public void Ctor_InvalidStep_Error()
		{
			CustomAssert.ThrowsException<ParameterConversionException>(() =>
			{
				StepConstraint c = new StepConstraint(ParameterDataType.Int32, DateTime.Now);
			});
		}

		[TestMethod]
		public void Ctor_StepNull_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				StepConstraint c = new StepConstraint(ParameterDataType.Int32, null);
			});
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			ParameterDataType type = ParameterDataType.Int32;
			StepConstraint c = new StepConstraint(type, 2);
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			StepConstraint c2 = SerializationHelper.Deserialize<StepConstraint>(Buffer);

			Assert.AreEqual(Constraint.StepConstraintName, c2.Name);
			Assert.AreEqual(type, c2.DataType);
			Assert.AreEqual(2, c2.StepSize);
		}
#endif
		#endregion

		#region Public methods
		[TestMethod]
		public void ToString_Success()
		{
			StepConstraint c = new StepConstraint(ParameterDataType.Int32, 42);
			Assert.AreEqual("[Step(42)]", c.ToString());
		}

		[TestMethod]
		public void SetParameters_Success()
		{
			StepConstraint c = new StepConstraint(ParameterDataType.Int32);
			c.SetParametersInternal(new string[] { "2" }, ParameterDataType.Int32);
			Assert.AreEqual(2, c.StepSize);
		}

		[TestMethod]
		public void SetParameters_TooManyParams_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				StepConstraint c = new StepConstraint(ParameterDataType.Int32);
				c.SetParametersInternal(new string[] { "1", "2" }, ParameterDataType.Int32);
			});
		}

		[TestMethod]
		public void SetParameters_ParamInv_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				StepConstraint c = new StepConstraint(ParameterDataType.Int32);
				c.SetParametersInternal(new string[] { "narf" }, ParameterDataType.Int32);
			});
		}
		#endregion
	}
}
