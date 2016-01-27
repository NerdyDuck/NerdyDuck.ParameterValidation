#region Copyright
/*******************************************************************************
 * <copyright file="DoubleObjectInt32Converter.cs" owner="Daniel Kopp">
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
 * <assembly name="ExampleCsharpDesktop">
 * Example application for NerdyDuck.ParameterValidation library.
 * </assembly>
 * <file name="DoubleObjectInt32Converter.cs" date="2016-01-19">
 * Converts a integer value to a double value and vice versa.
 * </file>
 ******************************************************************************/
#endregion

using System;
using System.Globalization;
using System.Windows.Data;

namespace ExampleCsharpDesktop
{
	/// <summary>
	/// Converts an integer value to a double value and vice versa.
	/// </summary>
	/// <remarks>Required because WPF binding cannot simply convert integers to doubles when they are boxed in a property of type System.Object.</remarks>
	public class DoubleObjectInt32Converter : IValueConverter
	{
		/// <summary>
		/// Converts a integer value (or any value implementing IConvertible) into a double value.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="targetType">Not used.</param>
		/// <param name="parameter">Not used</param>
		/// <param name="culture">Not used.</param>
		/// <returns>A double value.</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			IConvertible conv = value as IConvertible;
			if (conv == null)
			{
				return null;
			}

			return conv.ToDouble(System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converts a double value (or any value implementing IConvertible) into an integer value.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="targetType">Not used.</param>
		/// <param name="parameter">Not used</param>
		/// <param name="culture">Not used.</param>
		/// <returns>An integer value.</returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			IConvertible conv = value as IConvertible;
			if (conv == null)
			{
				return null;
			}

			return conv.ToInt32(System.Globalization.CultureInfo.InvariantCulture);
		}
	}
}
