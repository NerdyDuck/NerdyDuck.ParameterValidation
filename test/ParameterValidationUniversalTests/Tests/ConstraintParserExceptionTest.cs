#region Copyright
/*******************************************************************************
 * <copyright file="ConstraintParserExceptionTest.cs" owner="Daniel Kopp">
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
 * <file name="ConstraintParserExceptionTest.cs" date="2015-11-10">
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
using NerdyDuck.ParameterValidation.Constraints;

namespace NerdyDuck.Tests.ParameterValidation
{
	/// <summary>
	/// Contains test methods to test the NerdyDuck.ParameterValidation.ConstraintParserException class.
	/// </summary>
#if WINDOWS_DESKTOP
	[ExcludeFromCodeCoverage]
#endif
	[TestClass]
	public class ConstraintParserExceptionTest
	{
		#region Constructors
		[TestMethod]
		public void Ctor_Void_Success()
		{
			try
			{
				throw new ConstraintParserException();
			}
			catch (ConstraintParserException ex)
			{
				Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
				Assert.AreEqual(-1, ex.Position);
				Assert.IsNull(ex.InnerException);
			}
		}

		[TestMethod]
		public void Ctor_String_Success()
		{
			try
			{
				throw new ConstraintParserException(Constants.TestMessage);
			}
			catch (ConstraintParserException ex)
			{
				Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
				Assert.AreEqual(-1, ex.Position);
				Assert.IsNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
			}
		}

		[TestMethod]
		public void Ctor_StringInt_Success()
		{
			try
			{
				throw new ConstraintParserException(Constants.TestMessage, 42);
			}
			catch (ConstraintParserException ex)
			{
				Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
				Assert.AreEqual(42, ex.Position);
				Assert.IsNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
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
					throw new ConstraintParserException(Constants.TestMessage, ex);
				}
			}
			catch (ConstraintParserException ex)
			{
				Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
				Assert.AreEqual(-1, ex.Position);
				Assert.IsNotNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
			}
		}

		[TestMethod]
		public void Ctor_StringIntException_Success()
		{
			try
			{
				try
				{
					throw new FormatException();
				}
				catch (Exception ex)
				{
					throw new ConstraintParserException(Constants.TestMessage, 42, ex);
				}
			}
			catch (ConstraintParserException ex)
			{
				Assert.AreEqual(Constants.COR_E_FORMAT, ex.HResult);
				Assert.AreEqual(42, ex.Position);
				Assert.IsNotNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
			}
		}

		[TestMethod]
		public void Ctor_Int32_Success()
		{
			try
			{
				throw new ConstraintParserException(Constants.CustomHResult);
			}
			catch (ConstraintParserException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.AreEqual(-1, ex.Position);
				Assert.IsNull(ex.InnerException);
			}
		}

		[TestMethod]
		public void Ctor_IntString_Success()
		{
			try
			{
				throw new ConstraintParserException(Constants.CustomHResult, Constants.TestMessage);
			}
			catch (ConstraintParserException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.AreEqual(-1, ex.Position);
				Assert.IsNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
			}
		}

		[TestMethod]
		public void Ctor_IntStringInt_Success()
		{
			try
			{
				throw new ConstraintParserException(Constants.CustomHResult, Constants.TestMessage, 42);
			}
			catch (ConstraintParserException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.AreEqual(42, ex.Position);
				Assert.IsNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
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
					throw new ConstraintParserException(Constants.CustomHResult, Constants.TestMessage, ex);
				}
			}
			catch (ConstraintParserException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.AreEqual(-1, ex.Position);
				Assert.IsNotNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
			}
		}

		[TestMethod]
		public void Ctor_IntStringIntException_Success()
		{
			try
			{
				try
				{
					throw new FormatException();
				}
				catch (Exception ex)
				{
					throw new ConstraintParserException(Constants.CustomHResult, Constants.TestMessage, 42, ex);
				}
			}
			catch (ConstraintParserException ex)
			{
				Assert.AreEqual(Constants.CustomHResult, ex.HResult);
				Assert.AreEqual(42, ex.Position);
				Assert.IsNotNull(ex.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex.Message);
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
					throw new ConstraintParserException(Constants.CustomHResult, Constants.TestMessage, 42, ex);
				}
			}
			catch (ConstraintParserException ex)
			{
				System.IO.MemoryStream Buffer = SerializationHelper.Serialize(ex);
				ConstraintParserException ex2 = SerializationHelper.Deserialize<ConstraintParserException>(Buffer);

				Assert.AreEqual(Constants.CustomHResult, ex2.HResult);
				Assert.AreEqual(42, ex.Position);
				Assert.IsNotNull(ex2.InnerException);
				Assert.AreEqual(Constants.TestMessage, ex2.Message);
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
				throw new ConstraintParserException(Constants.CustomHResult, Constants.TestMessage);
			}
			catch (Exception ex)
			{
				string str = ex.ToString();
				StringAssert.StartsWith(str, string.Format("{0}: ({1}) {2}", typeof(ConstraintParserException).FullName, Constants.CustomHResultString, Constants.TestMessage));
				StringAssert.Contains(str, "ToString_Success");
			}
		}
		#endregion
	}
}
