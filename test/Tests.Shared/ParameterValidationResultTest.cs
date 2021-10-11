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
		/// Contains test methods to test the NerdyDuck.ParameterValidation.ParameterValidationResult class.
		/// </summary>
		[ExcludeFromCodeCoverage]
		[TestClass]
		public class ParameterValidationResultTest
		{
			[TestMethod]
			public void Ctor_IntStringConstraint_Success()
			{
				ParameterValidationResult result = new ParameterValidationResult(42, Constants.ErrorMessage, new DummyConstraint());
				Assert.AreEqual(42, result.HResult);
				Assert.AreEqual(Constants.ErrorMessage, result.ErrorMessage);
				Assert.IsNotNull(result.Constraint);
				Assert.IsFalse(result.MemberNames.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Ctor_ValidationResult_Success()
			{
				ParameterValidationResult result = new ParameterValidationResult(42, Constants.ErrorMessage, new DummyConstraint());
				ParameterValidationResult result2 = new ParameterValidationResult(result);
				Assert.AreEqual(42, result2.HResult);
				Assert.AreEqual(Constants.ErrorMessage, result2.ErrorMessage);
				Assert.IsNotNull(result2.Constraint);
				Assert.IsFalse(result2.MemberNames.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Ctor_ResultNull_Error()
			{
				CustomAssert.ThrowsException<ArgumentNullException>(() =>
				{
					ParameterValidationResult result = new ParameterValidationResult(null);
				});
			}

			[TestMethod]
			public void Ctor_IntStringStringsConstraint_Success()
			{
				ParameterValidationResult result = new ParameterValidationResult(42, Constants.ErrorMessage, new string[] { Constants.MemberName, Constants.MemberName }, new DummyConstraint());
				Assert.AreEqual(42, result.HResult);
				Assert.AreEqual(Constants.ErrorMessage, result.ErrorMessage);
				Assert.IsNotNull(result.Constraint);
				Assert.IsNotNull(result.MemberNames);
				Assert.IsTrue(result.MemberNames.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Ctor_IntStringStringConstraint_Success()
			{
				ParameterValidationResult result = new ParameterValidationResult(42, Constants.ErrorMessage, Constants.MemberName, new DummyConstraint());
				Assert.AreEqual(42, result.HResult);
				Assert.AreEqual(Constants.ErrorMessage, result.ErrorMessage);
				Assert.IsNotNull(result.Constraint);
				Assert.IsNotNull(result.MemberNames);
				Assert.IsTrue(result.MemberNames.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Ctor_SerializationInfo_Success()
			{
				ParameterValidationResult result = new ParameterValidationResult(42, Constants.ErrorMessage, new NerdyDuck.ParameterValidation.Constraints.NullConstraint());
				System.IO.MemoryStream Buffer = SerializationHelper.Serialize(result);
				ParameterValidationResult result2 = SerializationHelper.Deserialize<ParameterValidationResult>(Buffer);

				Assert.AreEqual(42, result2.HResult);
				Assert.AreEqual(Constants.ErrorMessage, result2.ErrorMessage);
				Assert.IsNotNull(result2.Constraint);
			}

			[TestMethod]
			public void GetObjectData_InfoNull_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
				{
					ParameterValidationResult result = new ParameterValidationResult(42, Constants.ErrorMessage, new DummyConstraint());
					result.GetObjectData(null, new System.Runtime.Serialization.StreamingContext());
				});
			}
		}
	}
}
