#region Copyright
/*******************************************************************************
 * <copyright file="DisplayHintConstraintTest.cs" owner="Daniel Kopp">
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
 * <file name="DisplayHintConstraintTest.cs" date="2016-01-027">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.Constraints.DisplayHintConstraint class.
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
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.DisplayHintConstraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class DisplayHintConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			DisplayHintConstraint c = new DisplayHintConstraint();
			Assert.AreEqual(Constraint.DisplayHintConstraintName, c.Name);
			Assert.IsNull(c.Hints);
		}

		[TestMethod]
		public void Ctor_String_Success()
		{
			DisplayHintConstraint c = new DisplayHintConstraint(Constants.SchemeName);
			Assert.AreEqual(Constraint.DisplayHintConstraintName, c.Name);
			Assert.IsNotNull(c.Hints);
			List<string> TempSchemas = new List<string>(c.Hints);
			Assert.AreEqual(Constants.SchemeName, TempSchemas[0]);
		}

		[TestMethod]
		public void Ctor_StringEmpty_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullOrWhiteSpaceException>(() =>
			{
				DisplayHintConstraint c = new DisplayHintConstraint(string.Empty);
			});
		}

		[TestMethod]
		public void Ctor_IEnumerableString_Success()
		{
			DisplayHintConstraint c = new DisplayHintConstraint(Constants.SchemeNames);
			Assert.AreEqual(Constraint.DisplayHintConstraintName, c.Name);
			Assert.IsNotNull(c.Hints);
			List<string> TempSchemas = new List<string>(c.Hints);
			Assert.AreEqual(3, TempSchemas.Count);
		}

		[TestMethod]
		public void Ctor_IEnumerableNull_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				DisplayHintConstraint c = new DisplayHintConstraint((IEnumerable<string>)null);
			});
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			DisplayHintConstraint c = new DisplayHintConstraint(Constants.SchemeNames);
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			DisplayHintConstraint c2 = SerializationHelper.Deserialize<DisplayHintConstraint>(Buffer);

			Assert.AreEqual(Constraint.DisplayHintConstraintName, c2.Name);
			Assert.IsNotNull(c2.Hints);
			List<string> TempSchemas = new List<string>(c2.Hints);
			Assert.AreEqual(3, TempSchemas.Count);
		}
#endif
		#endregion

		#region Public methods

		[TestMethod]
		public void ToString_Success()
		{
			DisplayHintConstraint c = new DisplayHintConstraint(Constants.SchemeNames);
			Assert.AreEqual(string.Format("[DisplayHint({0},{1},{2})]", Constants.SchemeNames[0], Constants.SchemeNames[1], Constants.SchemeNames[2]), c.ToString());
		}

		[TestMethod]
		public void SetParameters_Success()
		{
			DisplayHintConstraint c = new DisplayHintConstraint();
			c.SetParametersInternal(new string[] { Constants.SchemeNames[0], Constants.SchemeNames[1], Constants.SchemeNames[2] }, ParameterDataType.String);
			Assert.IsNotNull(c.Hints);
			List<string> TempSchemas = new List<string>(c.Hints);
			Assert.AreEqual(3, TempSchemas.Count);

		}

		[TestMethod]
		public void SetParameters_NoParamsOrNull_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				DisplayHintConstraint c = new DisplayHintConstraint();
				c.SetParametersInternal(new string[0], ParameterDataType.String);
			});
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				DisplayHintConstraint c = new DisplayHintConstraint();
				c.SetParametersInternal(new string[] { "1", "2", null, "4" }, ParameterDataType.String);
			});
		}

		[TestMethod]
		public void Validate_Success()
		{
			DisplayHintConstraint c = new DisplayHintConstraint(Constants.SchemeNames);
			IEnumerable<ParameterValidationResult> result = c.Validate(42, ParameterDataType.Int32, Constants.MemberName);
			Assert.IsNotNull(result);
			List<ParameterValidationResult> Temp = new List<ParameterValidationResult>(result);
			Assert.AreEqual(0, Temp.Count);
		}
		#endregion
	}
}
