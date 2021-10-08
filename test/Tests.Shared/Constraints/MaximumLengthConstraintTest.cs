#region Copyright
/*******************************************************************************
 * <copyright file="MaximumLengthConstraintTest.cs" owner="Daniel Kopp">
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
 * <file name="MaximumLengthConstraintTest.cs" date="2015-11-16">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.Constraints.MaximumLengthConstraint class.
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
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.MaximumLengthConstraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class MaximumLengthConstraintTest
	{
		#region Constructors
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

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			MaximumLengthConstraint c = new MaximumLengthConstraint(2);
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			MaximumLengthConstraint c2 = SerializationHelper.Deserialize<MaximumLengthConstraint>(Buffer);

			Assert.AreEqual(Constraint.MaximumLengthConstraintName, c2.Name);
			Assert.AreEqual(2, c2.MaximumLength);
		}
#endif
		#endregion

		#region Public methods
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
		#endregion
	}
}
