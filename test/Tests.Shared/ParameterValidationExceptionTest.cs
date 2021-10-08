#region Copyright
/*******************************************************************************
 * <copyright file="ParameterValidationExceptionTest.cs" owner="Daniel Kopp">
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
 * <file name="ParameterValidationExceptionTest.cs" date="2015-11-10">
 * Contains test methods to test the
 * NerdyDuck.ParameterValidation.ParameterValidationException class.
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

namespace NerdyDuck.Tests.ParameterValidation
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.ParameterValidationException class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class ParameterValidationExceptionTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			try
			{
				throw new ParameterValidationException();
			}
			catch (ParameterValidationException ex)
			{
				Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
				Assert.IsNull(ex.InnerException);
				Assert.IsNull(ex.Results);
			}
		}

		[TestMethod]
		public void Ctor_String_Success()
		{
			try
			{
				throw new ParameterValidationException(Constants.TestMessage);
			}
			catch (ParameterValidationException ex)
			{
				Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
				Assert.IsNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
				Assert.IsNull(ex.Results);
			}
		}

		[TestMethod]
		public void Ctor_StringException_Success()
		{
			try
			{
				try
				{
					throw new FormatException();
				}
				catch (Exception ex)
				{
					throw new ParameterValidationException(Constants.TestMessage, ex);
				}
			}
			catch (ParameterValidationException ex)
			{
				Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
				Assert.IsNotNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
				Assert.IsNull(ex.Results);
			}
		}

		[TestMethod]
		public void Ctor_Int32_Success()
		{
			try
			{
				throw new ParameterValidationException(Constants.CustomHResult);
			}
			catch (ParameterValidationException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.IsNull(ex.InnerException);
				Assert.IsNull(ex.Results);
			}
		}

		[TestMethod]
		public void Ctor_IntString_Success()
		{
			try
			{
				throw new ParameterValidationException(Constants.CustomHResult, Constants.TestMessage);
			}
			catch (ParameterValidationException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.IsNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
				Assert.IsNull(ex.Results);
			}
		}

		[TestMethod]
		public void Ctor_IntStringException_Success()
		{
			try
			{
				try
				{
					throw new FormatException();
				}
				catch (Exception ex)
				{
					throw new ParameterValidationException(Constants.CustomHResult, Constants.TestMessage, ex);
				}
			}
			catch (ParameterValidationException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.IsNotNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
				Assert.IsNull(ex.Results);
			}
		}

		[TestMethod]
		public void Ctor_Int32ParameterValidationResult_Success()
		{
			try
			{
				throw new ParameterValidationException(Constants.CustomHResult, CreateResults());
			}
			catch (ParameterValidationException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.IsNull(ex.InnerException);
				Assert.IsNotNull(ex.Results);
			}
		}

		[TestMethod]
		public void Ctor_IntStringParameterValidationResult_Success()
		{
			try
			{
				throw new ParameterValidationException(Constants.CustomHResult, Constants.TestMessage, CreateResults());
			}
			catch (ParameterValidationException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.IsNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
				Assert.IsNotNull(ex.Results);
			}
		}

		[TestMethod]
		public void Ctor_IntStringParameterValidationResultException_Success()
		{
			try
			{
				try
				{
					throw new FormatException();
				}
				catch (Exception ex)
				{
					throw new ParameterValidationException(Constants.CustomHResult, Constants.TestMessage, CreateResults(), ex);
				}
			}
			catch (ParameterValidationException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.IsNotNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
				Assert.IsNotNull(ex.Results);
			}
		}

#if WINDOWS_DESKTOP
		[TestMethod]
		public void Ctor_SerializationInfo_Success()
		{
			try
			{
				try
				{
					throw new FormatException();
				}
				catch (Exception ex)
				{
					throw new ParameterValidationException(Constants.CustomHResult, Constants.TestMessage, CreateResults(), ex);
				}
			}
			catch (ParameterValidationException ex)
			{
				System.IO.MemoryStream Buffer = SerializationHelper.Serialize(ex);
				ParameterValidationException ex2 = SerializationHelper.Deserialize<ParameterValidationException>(Buffer);

				Assert.AreEqual(Constants.CustomHResult, ex2.HResult);
				Assert.IsNotNull(ex2.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex2.Message);
				Assert.IsNotNull(ex2.Results);
				Assert.IsTrue(ex2.Results.GetEnumerator().MoveNext());
			}
		}
#endif
		#endregion

		#region ToString
		[TestMethod]
		public void ToString_Success()
		{
			try
			{
				throw new ParameterValidationException(Constants.CustomHResult, Constants.TestMessage);
			}
			catch (Exception ex)
			{
				string str = ex.ToString();
				StringAssert.StartsWith(str, string.Format("{0}: ({1}) {2}", typeof(ParameterValidationException).FullName, Constants.CustomHResultString, Constants.TestMessage));
				StringAssert.Contains(str, "ToString_Success");
			}
		}
		#endregion

		#region Private methods
		private static IEnumerable<ParameterValidationResult> CreateResults()
		{
			List<ParameterValidationResult> Results = new List<ParameterValidationResult>();
			Results.Add(new ParameterValidationResult(42, Constants.TestMessage, "MyName", new NullConstraint()));
			return Results;
		}
		#endregion
	}
}
