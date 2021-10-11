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
	/// Contains test methods to test various event argument classes in NerdyDuck.ParameterValidation.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class EventArgsTest
	{
		[TestMethod]
		public void UnknownConstraintEventArgs_ctor_Success()
		{
			UnknownConstraintEventArgs args = new UnknownConstraintEventArgs(Constants.MemberName, ParameterDataType.Guid);
			Assert.AreEqual(Constants.MemberName, args.ConstraintName);
			Assert.AreEqual(ParameterDataType.Guid, args.DataType);
			Assert.IsNull(args.Constraint);
			args.Constraint = new DummyConstraint();
			Assert.IsNotNull(args.Constraint);
		}

		[TestMethod]
		public void ParameterValidationEventArgs_ctor_Success()
		{
			ParameterValidationEventArgs args = new ParameterValidationEventArgs(42, ParameterDataType.Int32, new Constraint[] { new DummyConstraint() }, Constants.MemberName, Constants.DisplayName);
			Assert.AreEqual(Constants.MemberName, args.MemberName);
			Assert.AreEqual(Constants.DisplayName, args.DisplayName);
			Assert.AreEqual(ParameterDataType.Int32, args.DataType);
			Assert.AreEqual(42, args.Value);
			Assert.IsNotNull(args.Constraints);
			Assert.AreEqual(1, args.Constraints.Count);
		}

		[TestMethod]
		public void ParameterValidationErrorEventArgs_ctor_Success()
		{
			ParameterValidationErrorEventArgs args = new ParameterValidationErrorEventArgs(42, ParameterDataType.Int32, new Constraint[] { new DummyConstraint() }, Constants.MemberName, Constants.DisplayName, new ParameterValidationResult[] { ParameterValidationResult.Success });
			Assert.AreEqual(Constants.MemberName, args.MemberName);
			Assert.AreEqual(Constants.DisplayName, args.DisplayName);
			Assert.AreEqual(ParameterDataType.Int32, args.DataType);
			Assert.AreEqual(42, args.Value);
			Assert.IsNotNull(args.Constraints);
			Assert.AreEqual(1, args.Constraints.Count);
			Assert.IsNotNull(args.ValidationResults);
			Assert.AreEqual(1, args.ValidationResults.Count);
		}
	}
}
