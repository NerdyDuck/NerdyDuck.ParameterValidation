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
		public class DatabaseConstraintTest
		{
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
		}
	}
}
