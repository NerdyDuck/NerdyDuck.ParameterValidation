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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NerdyDuck.Tests.ParameterValidation
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
		/// Contains test methods to test the NerdyDuck.ParameterValidation.ParameterValidator class.
		/// </summary>
		[ExcludeFromCodeCoverage]
		[TestClass]
		public class ParameterValidatorTest
		{
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

			private List<Constraint> CreateSimpleConstraints()
			{
				List<Constraint> ReturnValue = new List<Constraint>();
				ReturnValue.Add(new NullConstraint());
				ReturnValue.Add(new MinimumValueConstraint(ParameterDataType.Int32, 40));
				return ReturnValue;
			}
		}
	}
}
