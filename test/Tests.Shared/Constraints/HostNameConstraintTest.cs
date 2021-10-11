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
using System.Diagnostics.CodeAnalysis;

namespace NerdyDuck.Tests.ParameterValidation.Constraints
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
		/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.NullConstraint class.
		/// </summary>
		[ExcludeFromCodeCoverage]
		[TestClass]
		public class HostNameConstraintTest
		{
			[TestMethod]
			public void Ctor_Void_Success()
			{
				HostNameConstraint c = new HostNameConstraint();
				Assert.AreEqual(Constraint.HostNameConstraintName, c.Name);
			}

			[TestMethod]
			public void Ctor_SerializationInfo_Success()
			{
				HostNameConstraint c = new HostNameConstraint();
				System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
				HostNameConstraint c2 = SerializationHelper.Deserialize<HostNameConstraint>(Buffer);

				Assert.AreEqual(Constraint.HostNameConstraintName, c2.Name);
			}

			[TestMethod]
			public void ToString_Success()
			{
				HostNameConstraint c = new HostNameConstraint();
				Assert.AreEqual("[Host]", c.ToString());
			}

			[TestMethod]
			public void Validate_Domain_Success()
			{
				HostNameConstraint c = new HostNameConstraint();
				IEnumerable<ParameterValidationResult> res = c.Validate("google.com", ParameterDataType.String, Constants.MemberName);
				Assert.IsNotNull(res);
				Assert.IsFalse(res.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_Local_Success()
			{
				HostNameConstraint c = new HostNameConstraint();
				IEnumerable<ParameterValidationResult> res = c.Validate("MyComputer", ParameterDataType.String, Constants.MemberName);
				Assert.IsNotNull(res);
				Assert.IsFalse(res.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_IPv4_Success()
			{
				HostNameConstraint c = new HostNameConstraint();
				IEnumerable<ParameterValidationResult> res = c.Validate("192.168.1.42", ParameterDataType.String, Constants.MemberName);
				Assert.IsNotNull(res);
				Assert.IsFalse(res.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_IPv6_Success()
			{
				HostNameConstraint c = new HostNameConstraint();
				IEnumerable<ParameterValidationResult> res = c.Validate("fe80::a1cc:f083:8a4f:f80c", ParameterDataType.String, Constants.MemberName);
				Assert.IsNotNull(res);
				Assert.IsFalse(res.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_WithPort_Success()
			{
				HostNameConstraint c = new HostNameConstraint();
				IEnumerable<ParameterValidationResult> res = c.Validate("google.com:80", ParameterDataType.String, Constants.MemberName);
				Assert.IsNotNull(res);
				Assert.IsTrue(res.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_InvDomain_Success()
			{
				HostNameConstraint c = new HostNameConstraint();
				IEnumerable<ParameterValidationResult> res = c.Validate("goo+gle.com", ParameterDataType.String, Constants.MemberName);
				Assert.IsNotNull(res);
				Assert.IsTrue(res.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_Whitespace_Success()
			{
				HostNameConstraint c = new HostNameConstraint();
				IEnumerable<ParameterValidationResult> res = c.Validate("   ", ParameterDataType.String, Constants.MemberName);
				Assert.IsNotNull(res);
				Assert.IsTrue(res.GetEnumerator().MoveNext());
			}
		}
	}
}
