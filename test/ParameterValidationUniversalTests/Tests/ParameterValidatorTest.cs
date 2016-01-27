#region Copyright
/*******************************************************************************
 * <copyright file="ParameterValidatorTest.cs" owner="Daniel Kopp">
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
 * <file name="ParameterValidatorTest.cs" date="2015-11-19">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.ParameterValidator class.
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
	/// Contains test methods to test the NerdyDuck.ParameterValidation.ParameterValidator class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class ParameterValidatorTest
	{
		#region Public methods
		[TestMethod]
		public void GetValidationResult_NoDisplay_Success()
		{
			ParameterValidator validator = new ParameterValidator();
			IEnumerable<ParameterValidationResult> results = validator.GetValidationResult(42, ParameterDataType.Int32, CreateSimpleConstraints(), Constants.MemberName);
			Assert.IsFalse(results.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void GetValidationResult_Success()
		{
			ParameterValidator validator = new ParameterValidator();
			IEnumerable<ParameterValidationResult> results = validator.GetValidationResult(42, ParameterDataType.Int32, CreateSimpleConstraints(), Constants.MemberName, Constants.DisplayName);
			Assert.IsFalse(results.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void GetValidationResult_TypeInt_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				ParameterValidator validator = new ParameterValidator();
				IEnumerable<ParameterValidationResult> results = validator.GetValidationResult(42, ParameterDataType.None, CreateSimpleConstraints(), Constants.MemberName, Constants.DisplayName);
			});
		}

		[TestMethod]
		public void GetValidationResult_MemberNull_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullOrEmptyException>(() =>
			{
				ParameterValidator validator = new ParameterValidator();
				IEnumerable<ParameterValidationResult> results = validator.GetValidationResult(42, ParameterDataType.Int32, CreateSimpleConstraints(), null, null);
			});
		}

		[TestMethod]
		public void GetValidationResult_NoConstraints_Success()
		{
			ParameterValidator validator = new ParameterValidator();
			IEnumerable<ParameterValidationResult> results = validator.GetValidationResult(42, ParameterDataType.Int32, null, Constants.MemberName, Constants.DisplayName);
			Assert.IsFalse(results.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void GetValidationResult_ValueNullNoConstraint_Success()
		{
			ParameterValidator validator = new ParameterValidator();
			IEnumerable<ParameterValidationResult> results = validator.GetValidationResult(null, ParameterDataType.Int32, null, Constants.MemberName, Constants.DisplayName);
			Assert.IsTrue(results.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void GetValidationResult_ValueNull_Success()
		{
			ParameterValidator validator = new ParameterValidator();
			IEnumerable<ParameterValidationResult> results = validator.GetValidationResult(null, ParameterDataType.Int32, CreateSimpleConstraints(), Constants.MemberName, Constants.DisplayName);
			Assert.IsFalse(results.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void GetValidationResult_WithEvents_Success()
		{
			ParameterValidator validator = new ParameterValidator();
			validator.Validating += (sender, e) =>
			{
				Assert.AreSame(validator, sender);
				Assert.AreEqual(2, e.Constraints.Count);
				Assert.AreEqual(Constants.DisplayName, e.DisplayName);
				Assert.AreEqual(Constants.MemberName, e.MemberName);
				Assert.AreEqual(1, e.Value);
			};
			validator.ValidationError += (sender, e) =>
			{
				Assert.AreSame(validator, sender);
				Assert.AreEqual(2, e.Constraints.Count);
				Assert.AreEqual(Constants.DisplayName, e.DisplayName);
				Assert.AreEqual(Constants.MemberName, e.MemberName);
				Assert.AreEqual(1, e.Value);
				List<ValidationResult> res = new List<ValidationResult>(e.ValidationResults);
				Assert.AreEqual(1, res.Count);
			};
			IEnumerable<ParameterValidationResult> results = validator.GetValidationResult(1, ParameterDataType.Int32, CreateSimpleConstraints(), Constants.MemberName, Constants.DisplayName);
			Assert.IsTrue(results.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void IsValid_NoDisplay_Success()
		{
			ParameterValidator validator = new ParameterValidator();
			Assert.IsTrue(validator.IsValid(42, ParameterDataType.Int32, CreateSimpleConstraints(), Constants.MemberName));
		}

		[TestMethod]
		public void IsValid_Success()
		{
			ParameterValidator validator = new ParameterValidator();
			Assert.IsTrue(validator.IsValid(42, ParameterDataType.Int32, CreateSimpleConstraints(), Constants.MemberName, Constants.DisplayName));
		}

		[TestMethod]
		public void Validate_NoDisplay_Success()
		{
			ParameterValidator validator = new ParameterValidator();
			validator.Validate(42, ParameterDataType.Int32, CreateSimpleConstraints(), Constants.MemberName);
		}

		[TestMethod]
		public void Validate_Success()
		{
			ParameterValidator validator = new ParameterValidator();
			validator.Validate(42, ParameterDataType.Int32, CreateSimpleConstraints(), Constants.MemberName, Constants.DisplayName);
		}

		[TestMethod]
		public void Validate_Error()
		{
			CustomAssert.ThrowsException<ParameterValidationException>(() =>
			{
				ParameterValidator validator = new ParameterValidator();
				validator.Validate(1, ParameterDataType.Int32, CreateSimpleConstraints(), Constants.MemberName, Constants.DisplayName);
			});
		}
		#endregion

		#region Private methods
		private List<Constraint> CreateSimpleConstraints()
		{
			List<Constraint> ReturnValue = new List<Constraint>();
			ReturnValue.Add(new NullConstraint());
			ReturnValue.Add(new MinimumValueConstraint(ParameterDataType.Int32, 40));
			return ReturnValue;
		}
		#endregion
	}
}
