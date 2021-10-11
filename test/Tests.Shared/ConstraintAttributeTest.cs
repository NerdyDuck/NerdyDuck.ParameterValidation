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
