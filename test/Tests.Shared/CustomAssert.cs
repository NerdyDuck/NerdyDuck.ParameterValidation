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

#if WINDOWS_UWP
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif
#if WINDOWS_DESKTOP
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
#endif
using System;

namespace NerdyDuck.Tests.ParameterValidation
{
	/// <summary>
	/// Verifies conditions in unit tests using true/false propositions.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static class CustomAssert
	{
#if WINDOWS_UWP
		public static T ThrowsException<T>(Action action) where T : Exception
		{
			return Assert.ThrowsException<T>(action);
		}

		public static T ThrowsException<T>(Func<object> action) where T : Exception
		{
			return Assert.ThrowsException<T>(action);
		}

		public static T ThrowsException<T>(Action action, string message) where T : Exception
		{
			return Assert.ThrowsException<T>(action, message);
		}

		public static T ThrowsException<T>(Func<object> action, string message) where T : Exception
		{
			return Assert.ThrowsException<T>(action, message);
		}

		public static T ThrowsException<T>(Action action, string message, params object[] parameters) where T : Exception
		{
			return Assert.ThrowsException<T>(action, message, parameters);
		}
#endif

#if WINDOWS_DESKTOP
		#region Public methods
		public static T ThrowsException<T>(Action action) where T : Exception
		{
			return ThrowsException<T>(action, string.Empty, null);
		}

		public static T ThrowsException<T>(Func<object> action) where T : Exception
		{
			return ThrowsException<T>(action, string.Empty, null);
		}

		public static T ThrowsException<T>(Action action, string message) where T : Exception
		{
			return ThrowsException<T>(action, message, null);
		}

		public static T ThrowsException<T>(Func<object> action, string message) where T : Exception
		{
			return ThrowsException<T>(action, message, null);
		}

		public static T ThrowsException<T>(Action action, string message, params object[] parameters) where T : Exception
		{
			string str = string.Empty;
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}

			try
			{
				action();
			}
			catch (Exception exception)
			{
				if (!typeof(T).Equals(exception.GetType()))
				{
					object[] objArray1 = new object[] { (message == null) ? string.Empty : ReplaceNulls(message), typeof(T).Name, exception.GetType().Name, exception.Message, exception.StackTrace };
					str = string.Format((IFormatProvider)CultureInfo.CurrentCulture, "Threw exception {2}, but exception {1} was expected. {0}\nException Message: {3}\nStack Trace: {4}", objArray1);
					HandleFail("Assert.ThrowsException", str, parameters);
				}
				return (T)exception;
			}

			object[] objArray2 = new object[] { (message == null) ? string.Empty : ReplaceNulls(message), typeof(T).Name };
			str = string.Format((IFormatProvider)CultureInfo.CurrentCulture, "No exception thrown. {1} exception was expected. {0}", objArray2);
			HandleFail("Assert.ThrowsException", str, parameters);
			return default(T);
		}

		public static T ThrowsException<T>(Func<object> action, string message, params object[] parameters) where T : Exception
		{
			return ThrowsException<T>(delegate { action(); }, message, parameters);
		}
		#endregion

		#region Private methods
		private static void HandleFail(string assertionName, string message, params object[] parameters)
		{
			string str = string.Empty;
			if (!string.IsNullOrEmpty(message))
			{
				if (parameters == null)
				{
					str = ReplaceNulls(message);
				}
				else
				{
					str = string.Format((IFormatProvider)CultureInfo.CurrentCulture, ReplaceNulls(message), parameters);
				}
			}
			object[] objArray1 = new object[] { assertionName, str };
			throw new AssertFailedException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "{0} failed. {1}", objArray1));
		}


		private static string ReplaceNulls(object input)
		{
			if (input == null)
			{
				return "(null)";
			}
			string str = input.ToString();
			if (str == null)
			{
				return "(object)";
			}
			return Assert.ReplaceNullChars(str);
		}
		#endregion
#endif
	}
}
