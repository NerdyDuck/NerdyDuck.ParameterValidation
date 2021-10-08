#region Copyright
/*******************************************************************************
 * <copyright file="DecimalPlacesConstraintTest.cs" owner="Daniel Kopp">
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
 * <file name="DecimalPlacesConstraintTest.cs" date="2015-11-11">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.Constraints.DecimalPlacesConstraint class.
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
	public class DecimalPlacesConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			DecimalPlacesConstraint c = new DecimalPlacesConstraint();
			Assert.AreEqual(Constraint.DecimalPlacesConstraintName, c.Name);
			Assert.AreEqual(2, c.DecimalPlaces);
		}

		[TestMethod]
		public void Ctor_Int_Success()
		{
			DecimalPlacesConstraint c = new DecimalPlacesConstraint(0);
			Assert.AreEqual(Constraint.DecimalPlacesConstraintName, c.Name);
			Assert.AreEqual(0, c.DecimalPlaces);
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			DecimalPlacesConstraint c = new DecimalPlacesConstraint();
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			DecimalPlacesConstraint c2 = SerializationHelper.Deserialize<DecimalPlacesConstraint>(Buffer);

			Assert.AreEqual(Constraint.DecimalPlacesConstraintName, c2.Name);
			Assert.AreEqual(2, c2.DecimalPlaces);
		}
#endif
		#endregion

		#region Public methods
		[TestMethod]
		public void Validate_Success()
		{
			DecimalPlacesConstraint c = new DecimalPlacesConstraint();
			IEnumerable<ParameterValidationResult> result = c.Validate(42m, ParameterDataType.Decimal, Constants.MemberName);
			Assert.IsNotNull(result);
			List<ParameterValidationResult> Temp = new List<ParameterValidationResult>(result);
			Assert.AreEqual(0, Temp.Count);
		}

		[TestMethod]
		public void ToString_Success()
		{
			DecimalPlacesConstraint c = new DecimalPlacesConstraint(1);
			Assert.AreEqual("[DecimalPlaces(1)]", c.ToString());
		}

		[TestMethod]
		public void SetParameters_Success()
		{
			DecimalPlacesConstraint c = new DecimalPlacesConstraint();
			c.SetParametersInternal(new string[] { "1" }, ParameterDataType.Decimal);
			Assert.AreEqual(1, c.DecimalPlaces);
		}

		[TestMethod]
		public void SetParameters_TooManyParams_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				DecimalPlacesConstraint c = new DecimalPlacesConstraint();
				c.SetParametersInternal(new string[] { "1", "2" }, ParameterDataType.Decimal);
			});
		}

		[TestMethod]
		public void SetParameters_ParamInv_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				DecimalPlacesConstraint c = new DecimalPlacesConstraint();
				c.SetParametersInternal(new string[] { "narf" }, ParameterDataType.Decimal);
			});
		}
		#endregion
	}
}
