#region Copyright
/*******************************************************************************
 * <copyright file="ParameterValidationResultTest.cs" owner="Daniel Kopp">
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
 * <file name="ParameterValidationResultTest.cs" date="2015-11-19">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.ParameterValidationResult class.
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
	/// Contains test methods to test the NerdyDuck.ParameterValidation.ParameterValidationResult class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class ParameterValidationResultTest
	{
		#region Constructors
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

#if WINDOWS_DESKTOP
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
#endif
		#endregion

		#region Public methods
#if WINDOWS_DESKTOP
		[TestMethod]
		public void GetObjectData_InfoNull_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				ParameterValidationResult result = new ParameterValidationResult(42, Constants.ErrorMessage, new DummyConstraint());
				result.GetObjectData(null, new System.Runtime.Serialization.StreamingContext());
			});
		}
#endif
		#endregion
	}
}
