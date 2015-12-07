#region Copyright
/*******************************************************************************
 * <copyright file="TypeExtensionsTest.cs" owner="Daniel Kopp">
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
 * <file name="TypeExtensionsTest.cs" date="2015-11-19">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.TypeExtensions class.
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

namespace NerdyDuck.Tests.ParameterValidation
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.TypeExtensions class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class TypeExtensionsTest
	{
		[TestMethod]
		public void ToStringAssemblyNameOnly_Success()
		{
			string str = TypeExtensions.ToStringAssemblyNameOnly(typeof(Constraint));
			Assert.AreEqual("NerdyDuck.ParameterValidation.Constraint, NerdyDuck.ParameterValidation", str);
		}

		[TestMethod]
		public void ToStringAssemblyNameOnly_Null_Error()
		{
			CustomAssert.ThrowsException<CodedArgumentNullException>(() =>
			{
				TypeExtensions.ToStringAssemblyNameOnly(null);
			});
		}
	}
}
