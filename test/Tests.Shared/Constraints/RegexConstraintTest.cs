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
		public class RegexConstraintTest
		{
			[TestMethod]
			public void Ctor_Void_Success()
			{
				RegexConstraint c = new RegexConstraint();
				Assert.AreEqual(Constraint.RegexConstraintName, c.Name);
				Assert.AreEqual(".*", c.RegularExpression);
				Assert.AreEqual(RegexOptions.None, c.Options);
			}

			[TestMethod]
			public void Ctor_String_Success()
			{
				RegexConstraint c = new RegexConstraint(Constants.RegexString, RegexOptions.IgnoreCase);
				Assert.AreEqual(Constraint.RegexConstraintName, c.Name);
				Assert.AreEqual(Constants.RegexString, c.RegularExpression);
				Assert.AreEqual(RegexOptions.IgnoreCase, c.Options);
			}

			[TestMethod]
			public void Ctor_StringEmpty_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentNullOrWhiteSpaceException>(() =>
				{
					RegexConstraint c = new RegexConstraint(string.Empty, RegexOptions.None);
				});
			}

			[TestMethod]
			public void Ctor_InvRegex_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentException>(() =>
				{
					RegexConstraint c = new RegexConstraint("[", RegexOptions.None);
				});
			}

			[TestMethod]
			public void Ctor_InvOptions_Error()
			{
				CustomAssert.ThrowsException<CodedArgumentOutOfRangeException>(() =>
				{
					RegexConstraint c = new RegexConstraint(Constants.RegexString, RegexOptions.ECMAScript | RegexOptions.Singleline);
				});
			}

			[TestMethod]
			public void Ctor_SerializationInfo_Success()
			{
				RegexConstraint c = new RegexConstraint(Constants.RegexString, RegexOptions.IgnoreCase);
				System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
				RegexConstraint c2 = SerializationHelper.Deserialize<RegexConstraint>(Buffer);

				Assert.AreEqual(Constraint.RegexConstraintName, c2.Name);
				Assert.AreEqual(Constants.RegexString, c2.RegularExpression);
				Assert.AreEqual(RegexOptions.IgnoreCase, c2.Options);
			}

			[TestMethod]
			public void ToString_Success()
			{
				RegexConstraint c = new RegexConstraint("[a-z]*", RegexOptions.IgnoreCase);
				Assert.AreEqual("[Regex('[a-z]*',IgnoreCase)]", c.ToString());
			}

			[TestMethod]
			public void SetParameters_Success()
			{
				RegexConstraint c = new RegexConstraint();
				c.SetParametersInternal(new string[] { Constants.RegexString, "IgnoreCase" }, ParameterDataType.String);
				Assert.AreEqual(Constants.RegexString, c.RegularExpression);
				Assert.AreEqual(RegexOptions.IgnoreCase, c.Options);

			}

			[TestMethod]
			public void SetParameters_NoParamsOrNull_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					RegexConstraint c = new RegexConstraint();
					c.SetParametersInternal(new string[0], ParameterDataType.String);
				});
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					RegexConstraint c = new RegexConstraint();
					c.SetParametersInternal(new string[] { null, "IgnoreCase" }, ParameterDataType.String);
				});
			}

			[TestMethod]
			public void SetParameters_InvOptionString_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					RegexConstraint c = new RegexConstraint();
					c.SetParametersInternal(new string[] { Constants.RegexString, "Ignxxxxx" }, ParameterDataType.String);
				});
			}

			[TestMethod]
			public void SetParameters_InvRegex_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					RegexConstraint c = new RegexConstraint();
					c.SetParametersInternal(new string[] { "[", "IgnoreCase" }, ParameterDataType.String);
				});
			}

			[TestMethod]
			public void SetParameters_InvOptions_Error()
			{
				CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
				{
					RegexConstraint c = new RegexConstraint();
					c.SetParametersInternal(new string[] { Constants.RegexString, "ECMAScript", "Singleline" }, ParameterDataType.String);
				});
			}

			[TestMethod]
			public void Validate_Success()
			{
				RegexConstraint c = new RegexConstraint(Constants.RegexString, RegexOptions.IgnoreCase);
				IEnumerable<ParameterValidationResult> res = c.Validate("Heretherebedragons", ParameterDataType.String, Constants.MemberName);
				Assert.IsNotNull(res);
				Assert.IsFalse(res.GetEnumerator().MoveNext());
			}

			[TestMethod]
			public void Validate_NoMatch_Success()
			{
				RegexConstraint c = new RegexConstraint(Constants.RegexString, RegexOptions.IgnoreCase);
				IEnumerable<ParameterValidationResult> res = c.Validate("Here there be dragons!", ParameterDataType.String, Constants.MemberName);
				Assert.IsNotNull(res);
				Assert.IsTrue(res.GetEnumerator().MoveNext());
			}
		}
	}
}
