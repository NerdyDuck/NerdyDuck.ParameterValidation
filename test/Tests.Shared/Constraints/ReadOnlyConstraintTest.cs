#region Copyright
/*******************************************************************************
 * <copyright file="ReadOnlyConstraintTest.cs" owner="Daniel Kopp">
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
 * <file name="ReadOnlyConstraintTest.cs" date="2016-01-27">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.Constraints.ReadOnlyConstraint class.
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
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Constraints.ReadOnlyConstraint class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class ReadOnlyConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			ReadOnlyConstraint c = new ReadOnlyConstraint();
			Assert.AreEqual(Constraint.ReadOnlyConstraintName, c.Name);
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			ReadOnlyConstraint c = new ReadOnlyConstraint();
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			ReadOnlyConstraint c2 = SerializationHelper.Deserialize<ReadOnlyConstraint>(Buffer);

			Assert.AreEqual(Constraint.ReadOnlyConstraintName, c2.Name);
		}
#endif
		#endregion

		#region Public methods
		[TestMethod]
		public void Validate_Success()
		{
			ReadOnlyConstraint c = new ReadOnlyConstraint();
			IEnumerable<ParameterValidationResult> result = c.Validate(42, ParameterDataType.Int32, Constants.MemberName);
			Assert.IsNotNull(result);
			List<ParameterValidationResult> Temp = new List<ParameterValidationResult>(result);
			Assert.AreEqual(0, Temp.Count);
		}
		#endregion
	}
}
