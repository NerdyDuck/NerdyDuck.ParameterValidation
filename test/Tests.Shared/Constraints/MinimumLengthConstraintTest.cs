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
using NerdyDuck.ParameterValidation;
using NerdyDuck.ParameterValidation.Constraints;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NerdyDuck.CodedExceptions;

namespace NerdyDuck.Tests.ParameterValidation.Constraints
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.MinimumLengthConstraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class MinimumLengthConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			MinimumLengthConstraint c = new MinimumLengthConstraint();
			Assert.AreEqual(Constraint.MinimumLengthConstraintName, c.Name);
			Assert.AreEqual(0, c.MinimumLength);
		}

		[TestMethod]
		public void Ctor_Int_Success()
		{
			MinimumLengthConstraint c = new MinimumLengthConstraint(2);
			Assert.AreEqual(Constraint.MinimumLengthConstraintName, c.Name);
			Assert.AreEqual(2, c.MinimumLength);
		}

		[TestMethod]
		public void Ctor_IntNegative_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				MinimumLengthConstraint c = new MinimumLengthConstraint(-1);
			});
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			MinimumLengthConstraint c = new MinimumLengthConstraint(2);
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			MinimumLengthConstraint c2 = SerializationHelper.Deserialize<MinimumLengthConstraint>(Buffer);

			Assert.AreEqual(Constraint.MinimumLengthConstraintName, c2.Name);
			Assert.AreEqual(2, c2.MinimumLength);
		}
#endif
		#endregion

		#region Public methods
		[TestMethod]
		public void ToString_Success()
		{
			MinimumLengthConstraint c = new MinimumLengthConstraint(1);
			Assert.AreEqual("[MinLength(1)]", c.ToString());
		}

		[TestMethod]
		public void SetParameters_Success()
		{
			MinimumLengthConstraint c = new MinimumLengthConstraint();
			c.SetParametersInternal(new string[] { "1" }, ParameterDataType.String);
			Assert.AreEqual(1, c.MinimumLength);
		}

		[TestMethod]
		public void SetParameters_TooManyParams_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				MinimumLengthConstraint c = new MinimumLengthConstraint();
				c.SetParametersInternal(new string[] { "1", "2" }, ParameterDataType.String);
			});
		}

		[TestMethod]
		public void SetParameters_ParamNegative_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				MinimumLengthConstraint c = new MinimumLengthConstraint();
				c.SetParametersInternal(new string[] { "-1" }, ParameterDataType.String);
			});
		}


		[TestMethod]
		public void SetParameters_ParamInv_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				MinimumLengthConstraint c = new MinimumLengthConstraint();
				c.SetParametersInternal(new string[] { "narf" }, ParameterDataType.String);
			});
		}

		[TestMethod]
		public void Validate_String_Success()
		{
			MinimumLengthConstraint c = new MinimumLengthConstraint(4);
			IEnumerable<ParameterValidationResult> result = c.Validate("fours", ParameterDataType.String, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_Bytes_Success()
		{
			MinimumLengthConstraint c = new MinimumLengthConstraint(4);
			IEnumerable<ParameterValidationResult> result = c.Validate(new byte[] { 1, 2, 3, 4 }, ParameterDataType.Bytes, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		public void Validate_Uri_Success()
		{
			MinimumLengthConstraint c = new MinimumLengthConstraint(11);
			IEnumerable<ParameterValidationResult> result = c.Validate(new Uri("http://contoso.com"), ParameterDataType.Uri, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsFalse(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_StringTooShort_Success()
		{
			MinimumLengthConstraint c = new MinimumLengthConstraint(6);
			IEnumerable<ParameterValidationResult> result = c.Validate("four", ParameterDataType.String, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_BytesTooShort_Success()
		{
			MinimumLengthConstraint c = new MinimumLengthConstraint(4);
			IEnumerable<ParameterValidationResult> result = c.Validate(new byte[] { 1, 2, 3 }, ParameterDataType.Bytes, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_UriTooShort_Success()
		{
			MinimumLengthConstraint c = new MinimumLengthConstraint(15);
			IEnumerable<ParameterValidationResult> result = c.Validate(new Uri("http://co"), ParameterDataType.Uri, Constants.MemberName);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_InvalidData_Success()
		{
			CustomAssert.ThrowsException<CodedArgumentException>(() =>
			{
				MinimumLengthConstraint c = new MinimumLengthConstraint(4);
				IEnumerable<ParameterValidationResult> result = c.Validate(42, ParameterDataType.Bytes, Constants.MemberName);
			});

		}
		#endregion
	}
}
