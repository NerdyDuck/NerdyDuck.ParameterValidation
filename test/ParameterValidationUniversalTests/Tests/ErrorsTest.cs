﻿#region Copyright
/*******************************************************************************
 * <copyright file="ErrorsTest.cs" owner="Daniel Kopp">
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
 * <file name="ErrorsTest.cs" date="2015-11-10">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.ConstraintParserException class.
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
using System;

namespace NerdyDuck.Tests.ParameterValidation
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.Errors class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class ErrorsTest
	{
		#region FacilityId
		[TestMethod]
		public void FacilityId_Success()
		{
			Assert.AreEqual<int>(2, Errors.FacilityId);
		}
		#endregion

		#region HResultBase
		[TestMethod]
		public void HResultBase_Success()
		{
			Assert.AreEqual<int>(unchecked((int)0xa0020000), Errors.HResultBase);
		}
		#endregion

		#region CreateHResult
		[TestMethod]
		public void CreateHResult_Success()
		{
			Assert.AreEqual<int>(unchecked((int)0xa0020042), Errors.CreateHResult(0x42));
		}
		#endregion
	}
}