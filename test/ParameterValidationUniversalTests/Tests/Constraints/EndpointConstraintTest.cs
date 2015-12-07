#region Copyright
/*******************************************************************************
 * <copyright file="EndpointConstraintTest.cs" owner="Daniel Kopp">
 * Copyright 2015 Daniel Kopp
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
 * <file name="EndpointConstraintTest.cs" date="2015-11-11">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.Constraints.EndpointConstraint class.
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

namespace NerdyDuck.Tests.ParameterValidation.Constraints
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.NullConstraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class EndpointConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			EndpointConstraint c = new EndpointConstraint();
			Assert.AreEqual(Constraint.EndpointConstraintName, c.Name);
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			EndpointConstraint c = new EndpointConstraint();
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			EndpointConstraint c2 = SerializationHelper.Deserialize<EndpointConstraint>(Buffer);

			Assert.AreEqual(Constraint.EndpointConstraintName, c2.Name);
		}
#endif
		#endregion

		#region Public methods

		[TestMethod]
		public void ToString_Success()
		{
			EndpointConstraint c = new EndpointConstraint();
			Assert.AreEqual("[Endpoint]", c.ToString());
		}

		[TestMethod]
		public void Validate_Success()
		{
			EndpointConstraint c = new EndpointConstraint();
			IEnumerable<ParameterValidationResult> res = c.Validate("google.com:80", ParameterDataType.String, Constants.MemberName);
			Assert.IsNotNull(res);
			Assert.IsFalse(res.GetEnumerator().MoveNext());
		}

		public void Validate_NoPort_Success()
		{
			EndpointConstraint c = new EndpointConstraint();
			IEnumerable<ParameterValidationResult> res = c.Validate("google.com", ParameterDataType.String, Constants.MemberName);
			Assert.IsNotNull(res);
			Assert.IsFalse(res.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_InvPort1_Success()
		{
			EndpointConstraint c = new EndpointConstraint();
			IEnumerable<ParameterValidationResult> res = c.Validate("google.com:77777", ParameterDataType.String, Constants.MemberName);
			Assert.IsNotNull(res);
			Assert.IsTrue(res.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_InvPort2_Success()
		{
			EndpointConstraint c = new EndpointConstraint();
			IEnumerable<ParameterValidationResult> res = c.Validate("google.com:-1", ParameterDataType.String, Constants.MemberName);
			Assert.IsNotNull(res);
			Assert.IsTrue(res.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_InvPort3_Success()
		{
			EndpointConstraint c = new EndpointConstraint();
			IEnumerable<ParameterValidationResult> res = c.Validate("google.com:narf", ParameterDataType.String, Constants.MemberName);
			Assert.IsNotNull(res);
			Assert.IsTrue(res.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_InvDomain_Success()
		{
			EndpointConstraint c = new EndpointConstraint();
			IEnumerable<ParameterValidationResult> res = c.Validate("goo+gle.com", ParameterDataType.String, Constants.MemberName);
			Assert.IsNotNull(res);
			Assert.IsTrue(res.GetEnumerator().MoveNext());
		}

		[TestMethod]
		public void Validate_Whitespace_Success()
		{
			EndpointConstraint c = new EndpointConstraint();
			IEnumerable<ParameterValidationResult> res = c.Validate("   ", ParameterDataType.String, Constants.MemberName);
			Assert.IsNotNull(res);
			Assert.IsTrue(res.GetEnumerator().MoveNext());
		}
		#endregion
	}
}
