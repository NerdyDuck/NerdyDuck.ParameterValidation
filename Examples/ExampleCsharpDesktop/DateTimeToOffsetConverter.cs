#region Copyright
/*******************************************************************************
 * <copyright file="DateTimeToOffsetConverter.cs" owner="Daniel Kopp">
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
 * <file name="DateTimeToOffsetConverter.cs" date="2016-01-19">
 * Converts a DateTimeOffset value to a DateTime value and vice versa.
 * </file>
 ******************************************************************************/
#endregion

using System;
using System.Globalization;
using System.Windows.Data;

namespace ExampleCsharpDesktop
{
	/// <summary>
	/// Converts a DateTimeOffset value to a DateTime value and vice versa.
	/// </summary>
	/// <remarks>Required because the DatePicker control only handles DateTime values.</remarks>
	public class DateTimeToOffsetConverter : IValueConverter
	{
		/// <summary>
		/// Converts a value from DateTimeOffset to DateTime.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="targetType">Not used.</param>
		/// <param name="parameter">Not used.</param>
		/// <param name="culture">Not used.</param>
		/// <returns>A DateTime value.</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				DateTimeOffset d = (DateTimeOffset)value;
				return d.DateTime.Date;
			}
			return null;
		}

		/// <summary>
		/// Converts a value from DateTime to DateTimeOffset.
		/// </summary>
		/// <param name="value">The value to convert.</param>
		/// <param name="targetType">Not used.</param>
		/// <param name="parameter">Not used.</param>
		/// <param name="culture">Not used.</param>
		/// <returns>A DateTimeOffset value.</returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value != null)
			{
				DateTime d = (DateTime)value;
				return new DateTimeOffset(d);
			}
			return null;
		}
	}
}
