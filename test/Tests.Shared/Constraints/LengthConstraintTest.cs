﻿#region Copyright
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
		/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.LengthConstraint class.
		/// </summary>
		[ExcludeFromCodeCoverage]
		[TestClass]
		public class LengthConstraintTest
		{
			[TestMethod]
			public void Ctor_Void_Success()
			{
				LengthConstraint c = new LengthConstraint();
				Assert.AreEqual(Constraint.LengthConstraintName, c.Name);
				Assert.AreEqual(int.MaxValue, c.Length);
			}

			[TestMethod]
			public void Ctor_Int_Success()
			{
				LengthConstraint c = new LengthConstraint(0);
				Assert.AreEqual(Constraint.LengthConstraintName, c.Name);
				Assert.AreEqual(0, c.Length);
			}

			[TestMethod]
			public void Ctor_IntNegative_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
				{
					LengthConstraint c = new LengthConstraint(-1);
				});
			}

			[TestMethod]
			public void Ctor_SerializationInfo_Success()
			{
				LengthConstraint c = new LengthConstraint(2);
				System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
				LengthConstraint c2 = SerializationHelper.Deserialize<LengthConstraint>(Buffer);

				Assert.AreEqual(Constraint.LengthConstraintName, c2.Name);
				Assert.AreEqual(2, c2.Length);
			}

			[TestMethod]
			public void ToString_Success()
			{
				LengthConstraint c = new LengthConstraint(1);
				Assert.AreEqual("[Length(1)]", c.ToString());
			}

			[TestMethod]
			public void SetParameters_Success()
			{
				LengthConstraint c = new LengthConstraint();
				c.SetParametersInternal(new string[] { "1" }, ParameterDataType.String);
				Assert.AreEqual(1, c.Length);
			}

			[TestMethod]
			public void SetParameters_TooManyParams_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					LengthConstraint c = new LengthConstraint();
					c.SetParametersInternal(new string[] { "1", "2" }, ParameterDataType.String);
				});
			}

			[TestMethod]
			public void SetParameters_ParamNegative_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					LengthConstraint c = new LengthConstraint();
					c.SetParametersInternal(new string[] { "-1" }, ParameterDataType.String);
				});
			}


			[TestMethod]
			public void SetParameters_ParamInv_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					LengthConstraint c = new LengthConstraint();
					c.SetParametersInternal(new string[] { "narf" }, ParameterDataType.String);
				});
			}

			[TestMethod]
			public void Validate_String_Success()
			{
				LengthConstraint c = new LengthConstraint(4);
				IEnumerable<ParameterValidationResult> result = c.Validate("four", ParameterDataType.String, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_Bytes_Success()
			{
				LengthConstraint c = new LengthConstraint(4);
				IEnumerable<ParameterValidationResult> result = c.Validate(new byte[] { 1, 2, 3, 4 }, ParameterDataType.Bytes, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_StringTooLong_Success()
			{
				LengthConstraint c = new LengthConstraint(4);
				IEnumerable<ParameterValidationResult> result = c.Validate("four and a bit", ParameterDataType.String, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_BytesTooLong_Success()
			{
				LengthConstraint c = new LengthConstraint(4);
				IEnumerable<ParameterValidationResult> result = c.Validate(new byte[] { 1, 2, 3, 4, 5, 6 }, ParameterDataType.Bytes, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_InvalidData_Success()
			{
				CustomAssert.ThrowsException<CodedArgumentException>(() =>
				{
					LengthConstraint c = new LengthConstraint(4);
					IEnumerable<ParameterValidationResult> result = c.Validate(42, ParameterDataType.Bytes, Constants.MemberName);
				});

			}
		}
	}
}
