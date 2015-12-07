#region Copyright
/*******************************************************************************
 * <copyright file="EncryptedConstraintTest.cs" owner="Daniel Kopp">
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
 * <file name="EncryptedConstraintTest.cs" date="2015-11-11">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.Constraints.EncryptedConstraint class.
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
	public class EncryptedConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			EncryptedConstraint c = new EncryptedConstraint();
			Assert.AreEqual(Constraint.EncryptedConstraintName, c.Name);
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			EncryptedConstraint c = new EncryptedConstraint();
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			EncryptedConstraint c2 = SerializationHelper.Deserialize<EncryptedConstraint>(Buffer);

			Assert.AreEqual(Constraint.EncryptedConstraintName, c2.Name);
		}
#endif
		#endregion

		#region Public methods
		[TestMethod]
		public void Validate_Success()
		{
			EncryptedConstraint c = new EncryptedConstraint();
			IEnumerable<ParameterValidationResult> result = c.Validate(42, ParameterDataType.Int32, Constants.MemberName);
			Assert.IsNotNull(result);
			List<ParameterValidationResult> Temp = new List<ParameterValidationResult>(result);
			Assert.AreEqual(0, Temp.Count);
		}
		#endregion
	}
}
