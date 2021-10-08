#region Copyright
/*******************************************************************************
 * <copyright file="ConstraintAttributeTest.cs" owner="Daniel Kopp">
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
 * <file name="ConstraintAttributeTest.cs" date="2015-11-19">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.ConstraintAttribute class.
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
	/// Contains test methods to test the NerdyDuck.ParameterValidation.ConstraintAttribute class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class ConstraintAttributeTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_StringDataType_Success()
		{
			ConstraintAttribute ca = new ConstraintAttribute(Constants.SimpleConstraintString, ParameterDataType.Int32);
			Assert.AreEqual(ParameterDataType.Int32, ca.DataType);
			Assert.AreEqual(Constants.SimpleConstraintString, ca.Constraints);
			Assert.IsTrue(ca.RequiresValidationContext);
			Assert.IsNotNull(ca.ParsedConstraints);
			Assert.AreEqual(1, ca.ParsedConstraints.Count);
		}

		[TestMethod]
		public void Ctor_ConstraintsNull_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullOrWhiteSpaceException>(() =>
			{
				ConstraintAttribute ca = new ConstraintAttribute(null, ParameterDataType.Int32);
			});
			CustomAssert.ThrowsException<CodedArgumentNullOrWhiteSpaceException>(() =>
			{
				ConstraintAttribute ca = new ConstraintAttribute(null, ParameterDataType.Int32, Constants.ErrorMessage);
			});
			CustomAssert.ThrowsException<CodedArgumentNullOrWhiteSpaceException>(() =>
			{
				ConstraintAttribute ca = new ConstraintAttribute(null, ParameterDataType.Int32, new Func<string>(() => { return Constants.ErrorMessage; }));
			});
		}

		[TestMethod]
		public void Ctor_TypeInv_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				ConstraintAttribute ca = new ConstraintAttribute(Constants.SimpleConstraintString, ParameterDataType.None);
			});
			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				ConstraintAttribute ca = new ConstraintAttribute(Constants.SimpleConstraintString, ParameterDataType.None, Constants.ErrorMessage);
			});
			CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
			{
				ConstraintAttribute ca = new ConstraintAttribute(Constants.SimpleConstraintString, ParameterDataType.None, new Func<string>(() => { return Constants.ErrorMessage; }));
			});
		}

		[TestMethod]
		public void Ctor_StringDataTypeString_Success()
		{
			ConstraintAttribute ca = new ConstraintAttribute(Constants.SimpleConstraintString, ParameterDataType.Int32, Constants.ErrorMessage);
			Assert.AreEqual(ParameterDataType.Int32, ca.DataType);
			Assert.AreEqual(Constants.SimpleConstraintString, ca.Constraints);
			Assert.IsNotNull(ca.ParsedConstraints);
			Assert.AreEqual(1, ca.ParsedConstraints.Count);
			Assert.AreEqual(Constants.ErrorMessage, ca.FormatErrorMessage(null));
		}

		[TestMethod]
		public void Ctor_StringDataTypeFunc_Success()
		{
			ConstraintAttribute ca = new ConstraintAttribute(Constants.SimpleConstraintString, ParameterDataType.Int32, new Func<string>(() => { return Constants.ErrorMessage; }));
			Assert.AreEqual(ParameterDataType.Int32, ca.DataType);
			Assert.AreEqual(Constants.SimpleConstraintString, ca.Constraints);
			Assert.IsNotNull(ca.ParsedConstraints);
			Assert.AreEqual(1, ca.ParsedConstraints.Count);
			Assert.AreEqual(Constants.ErrorMessage, ca.FormatErrorMessage(null));
		}
		#endregion

		#region Public methods
		[TestMethod]
		public void IsValid_NoResults_Success()
		{
			ConstraintAttribute ca = new ConstraintAttribute(Constants.MinValueConstraintString, ParameterDataType.Int32);
			Assert.IsTrue(ca.IsValid(42));
		}

		[TestMethod]
		public void IsValid_Results_Success()
		{
			ConstraintAttribute ca = new ConstraintAttribute(Constants.MinValueConstraintString, ParameterDataType.Int32);
			Assert.IsFalse(ca.IsValid(13));
		}
		#endregion
	}
}
