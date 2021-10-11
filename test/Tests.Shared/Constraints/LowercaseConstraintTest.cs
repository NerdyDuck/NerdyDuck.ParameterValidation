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

namespace NerdyDuck.Tests.ParameterValidation.Constraints
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.NullConstraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class LowercaseConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			LowercaseConstraint c = new LowercaseConstraint();
			Assert.AreEqual(Constraint.LowercaseConstraintName, c.Name);
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			LowercaseConstraint c = new LowercaseConstraint();
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			LowercaseConstraint c2 = SerializationHelper.Deserialize<LowercaseConstraint>(Buffer);

			Assert.AreEqual(Constraint.LowercaseConstraintName, c2.Name);
		}
#endif
		#endregion

		#region Public methods
		[TestMethod]
		public void ToString_Success()
		{
			LowercaseConstraint c = new LowercaseConstraint();
			Assert.AreEqual("[Lowercase]", c.ToString());
		}

		[TestMethod]
		public void Validate_Success()
		{
			LowercaseConstraint c = new LowercaseConstraint();
			IEnumerable<ParameterValidationResult> res = c.Validate("some lowercase and 3 numbers and punctuation ...!", ParameterDataType.String, Constants.MemberName);
			Assert.IsNotNull(res);
			Assert.IsFalse(res.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_Uppercase_Success()
		{
			LowercaseConstraint c = new LowercaseConstraint();
			IEnumerable<ParameterValidationResult> res = c.Validate("This Is Uppercase!", ParameterDataType.String, Constants.MemberName);
			Assert.IsNotNull(res);
			Assert.IsTrue(res.GetEnumerator().MoveNext());
		}
		#endregion
	}
}
