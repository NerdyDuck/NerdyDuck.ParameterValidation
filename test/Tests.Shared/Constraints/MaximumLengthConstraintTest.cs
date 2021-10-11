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
		/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.MaximumLengthConstraint class.
		/// </summary>
		[ExcludeFromCodeCoverage]
		[TestClass]
		public class MaximumLengthConstraintTest
		{
			[TestMethod]
			public void Ctor_Void_Success()
			{
				MaximumLengthConstraint c = new MaximumLengthConstraint();
				Assert.AreEqual(Constraint.MaximumLengthConstraintName, c.Name);
				Assert.AreEqual(int.MaxValue, c.MaximumLength);
			}

			[TestMethod]
			public void Ctor_Int_Success()
			{
				MaximumLengthConstraint c = new MaximumLengthConstraint(0);
				Assert.AreEqual(Constraint.MaximumLengthConstraintName, c.Name);
				Assert.AreEqual(0, c.MaximumLength);
			}

			[TestMethod]
			public void Ctor_IntNegative_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
				{
					MaximumLengthConstraint c = new MaximumLengthConstraint(-1);
				});
			}

			[TestMethod]
			public void Ctor_SerializationInfo_Success()
			{
				MaximumLengthConstraint c = new MaximumLengthConstraint(2);
				System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
				MaximumLengthConstraint c2 = SerializationHelper.Deserialize<MaximumLengthConstraint>(Buffer);

				Assert.AreEqual(Constraint.MaximumLengthConstraintName, c2.Name);
				Assert.AreEqual(2, c2.MaximumLength);
			}

			[TestMethod]
			public void ToString_Success()
			{
				MaximumLengthConstraint c = new MaximumLengthConstraint(1);
				Assert.AreEqual("[MaxLength(1)]", c.ToString());
			}

			[TestMethod]
			public void SetParameters_Success()
			{
				MaximumLengthConstraint c = new MaximumLengthConstraint();
				c.SetParametersInternal(new string[] { "1" }, ParameterDataType.String);
				Assert.AreEqual(1, c.MaximumLength);
			}

			[TestMethod]
			public void SetParameters_TooManyParams_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					MaximumLengthConstraint c = new MaximumLengthConstraint();
					c.SetParametersInternal(new string[] { "1", "2" }, ParameterDataType.String);
				});
			}

			[TestMethod]
			public void SetParameters_ParamNegative_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					MaximumLengthConstraint c = new MaximumLengthConstraint();
					c.SetParametersInternal(new string[] { "-1" }, ParameterDataType.String);
				});
			}


			[TestMethod]
			public void SetParameters_ParamInv_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					MaximumLengthConstraint c = new MaximumLengthConstraint();
					c.SetParametersInternal(new string[] { "narf" }, ParameterDataType.String);
				});
			}

			[TestMethod]
			public void Validate_String_Success()
			{
				MaximumLengthConstraint c = new MaximumLengthConstraint(5);
				IEnumerable<ParameterValidationResult> result = c.Validate("four", ParameterDataType.String, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_Bytes_Success()
			{
				MaximumLengthConstraint c = new MaximumLengthConstraint(5);
				IEnumerable<ParameterValidationResult> result = c.Validate(new byte[] { 1, 2, 3, 4 }, ParameterDataType.Bytes, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			public void Validate_Uri_Success()
			{
				MaximumLengthConstraint c = new MaximumLengthConstraint(20);
				IEnumerable<ParameterValidationResult> result = c.Validate(new Uri("http://contoso.com"), ParameterDataType.Uri, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_StringTooLong_Success()
			{
				MaximumLengthConstraint c = new MaximumLengthConstraint(4);
				IEnumerable<ParameterValidationResult> result = c.Validate("four and a bit", ParameterDataType.String, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_BytesTooLong_Success()
			{
				MaximumLengthConstraint c = new MaximumLengthConstraint(4);
				IEnumerable<ParameterValidationResult> result = c.Validate(new byte[] { 1, 2, 3, 4, 5, 6 }, ParameterDataType.Bytes, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_UriTooLong_Success()
			{
				MaximumLengthConstraint c = new MaximumLengthConstraint(4);
				IEnumerable<ParameterValidationResult> result = c.Validate(new Uri("http://contoso.om"), ParameterDataType.Uri, Constants.MemberName);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_InvalidData_Success()
			{
				CustomAssert.ThrowsException<CodedArgumentException>(() =>
				{
					MaximumLengthConstraint c = new MaximumLengthConstraint(4);
					IEnumerable<ParameterValidationResult> result = c.Validate(42, ParameterDataType.Bytes, Constants.MemberName);
				});

			}
		}
	}
}
