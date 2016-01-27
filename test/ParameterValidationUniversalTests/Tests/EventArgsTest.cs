#region Copyright
/*******************************************************************************
 * <copyright file="EventArgsTest.cs" owner="Daniel Kopp">
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
 * <file name="EventArgsTest.cs" date="2015-11-19">
 * Contains test methods to test the
 * Contains test methods to test various event argument classes in
 * NerdyDuck.ParameterValidation.
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
