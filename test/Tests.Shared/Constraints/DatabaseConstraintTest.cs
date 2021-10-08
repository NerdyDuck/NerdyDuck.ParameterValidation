#region Copyright
/*******************************************************************************
 * <copyright file="DatabaseConstraintTest.cs" owner="Daniel Kopp">
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
 * <file name="DatabaseConstraintTest.cs" date="2015-11-11">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.Constraints.DatabaseConstraint class.
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
	public class DatabaseConstraintTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			DatabaseConstraint c = new DatabaseConstraint();
			Assert.AreEqual(Constraint.DatabaseConstraintName, c.Name);
			Assert.AreEqual(string.Empty, c.DisplayProperty);
			Assert.AreEqual(string.Empty, c.Entity);
			Assert.AreEqual(string.Empty, c.KeyProperty);
		}

		[TestMethod]
		public void Ctor_StringStringString_Success()
		{
			DatabaseConstraint c = new DatabaseConstraint(Constants.EntityName, Constants.KeyName, Constants.DisplayName);
			Assert.AreEqual(Constraint.DatabaseConstraintName, c.Name);
			Assert.AreEqual(Constants.DisplayName, c.DisplayProperty);
			Assert.AreEqual(Constants.EntityName, c.Entity);
			Assert.AreEqual(Constants.KeyName, c.KeyProperty);
		}

		[TestMethod]
		public void Ctor_NullNullNull_Success()
		{
			DatabaseConstraint c = new DatabaseConstraint(null, null, null);
			Assert.AreEqual(Constraint.DatabaseConstraintName, c.Name);
			Assert.AreEqual(string.Empty, c.DisplayProperty);
			Assert.AreEqual(string.Empty, c.Entity);
			Assert.AreEqual(string.Empty, c.KeyProperty);
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			DatabaseConstraint c = new DatabaseConstraint(Constants.EntityName, Constants.KeyName, Constants.DisplayName);
			System.IO.MemoryStream Buffer = SerializationHelper.Serialize(c);
			DatabaseConstraint c2 = SerializationHelper.Deserialize<DatabaseConstraint>(Buffer);

			Assert.AreEqual(Constraint.DatabaseConstraintName, c2.Name);
			Assert.AreEqual(Constants.DisplayName, c2.DisplayProperty);
			Assert.AreEqual(Constants.EntityName, c2.Entity);
			Assert.AreEqual(Constants.KeyName, c2.KeyProperty);
		}
#endif
		#endregion

		#region Public methods

		[TestMethod]
		public void ToString_Success()
		{
			DatabaseConstraint c = new DatabaseConstraint(Constants.EntityName, Constants.KeyName, Constants.DisplayName);
			Assert.AreEqual(string.Format("[Database({0},{1},{2})]", Constants.EntityName, Constants.KeyName, Constants.DisplayName), c.ToString());
		}

		[TestMethod]
		public void SetParameters_Success()
		{
			DatabaseConstraint c = new DatabaseConstraint();
			c.SetParametersInternal(new string[] { Constants.EntityName, Constants.KeyName, Constants.DisplayName }, ParameterDataType.Int32);
			Assert.AreEqual(Constants.DisplayName, c.DisplayProperty);
			Assert.AreEqual(Constants.EntityName, c.Entity);
			Assert.AreEqual(Constants.KeyName, c.KeyProperty);
		}

		[TestMethod]
		public void SetParameters_Null_Success()
		{
			DatabaseConstraint c = new DatabaseConstraint();
			c.SetParametersInternal(new string[] { null, null, null }, ParameterDataType.Int32);
			Assert.AreEqual(string.Empty, c.DisplayProperty);
			Assert.AreEqual(string.Empty, c.Entity);
			Assert.AreEqual(string.Empty, c.KeyProperty);
		}

		[TestMethod]
		public void SetParameters_TooManyFewParams_Error()
		{
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				DatabaseConstraint c = new DatabaseConstraint();
				c.SetParametersInternal(new string[0], ParameterDataType.Int32);
			});
			CustomAssert.ThrowsException<ConstraintConfigurationException>(() =>
			{
				DatabaseConstraint c = new DatabaseConstraint();
				c.SetParametersInternal(new string[] { "1", "2", "3", "4" }, ParameterDataType.Int32);
			});
		}
		#endregion
	}
}
