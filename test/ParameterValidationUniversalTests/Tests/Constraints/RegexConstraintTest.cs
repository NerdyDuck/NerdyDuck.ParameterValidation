#region Copyright
/*******************************************************************************
 * <copyright file="RegexConstraintTest.cs" owner="Daniel Kopp">
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
 * <file name="RegexConstraintTest.cs" date="2015-11-16">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.Constraints.RegexConstraint class.
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
using System.Text.RegularExpressions;

namespace NerdyDuck.Tests.ParameterValidation.Constraints
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.NullConstraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class RegexConstraintTest
	{
		#region Constructors
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

#if WINDOWS_DESKTOP
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
#endif
		#endregion

		#region Public methods

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
		#endregion
	}
}
