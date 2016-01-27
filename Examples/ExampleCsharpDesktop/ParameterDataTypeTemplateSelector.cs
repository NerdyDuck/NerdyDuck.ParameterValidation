#region Copyright
/*******************************************************************************
 * <copyright file="ParameterDataTypeTemplateSelector.cs" owner="Daniel Kopp">
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
 * <file name="ParameterDataTypeTemplateSelector.cs" date="2016-01-19">
 * Retrieves the correct item template based on the DataType of the
 * SystemSettingViewModel.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.ParameterValidation;
using System.Windows;
using System.Windows.Controls;

namespace ExampleCsharpDesktop
{
	/// <summary>
	/// Retrieves the correct item template based on the DataType of the SystemSettingViewModel.
	/// </summary>
	public class ParameterDataTypeTemplateSelector : DataTemplateSelector
	{
		/// <summary>
		/// Selects DataTemplate based on the DataType of s SystemSettingViewModel.
		/// </summary>
		/// <param name="item">The SystemSettingViewModel to display.</param>
		/// <param name="container">The control that displays the SystemSettingViewModel.</param>
		/// <returns></returns>
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			SystemSettingViewModel setting = item as SystemSettingViewModel;
			FrameworkElement element = container as FrameworkElement;
			if (setting == null || element == null)
				return null;

			// The templates with the appropriate names must be a resource discoverable from the context of the control.
			// This is only a small selection of templates for some data types. More templates could be created for other data types,
			// or different templates depending on the Constraints attached to a value.
			switch (setting.DataType)
			{
				case ParameterDataType.String:
					return element.FindResource("StringListItemTemplate") as DataTemplate;
				case ParameterDataType.Int32:
					return element.FindResource("IntegerListItemTemplate") as DataTemplate;
				case ParameterDataType.DateTimeOffset:
					return element.FindResource("DateTimeListItemTemplate") as DataTemplate;
				case ParameterDataType.Enum:
					if (setting.EnumValues != null)
						return element.FindResource("EnumListItemTemplate") as DataTemplate;
					else
						return element.FindResource("StringListItemTemplate") as DataTemplate;
				default:
					return element.FindResource("StringListItemTemplate") as DataTemplate;
			}
		}
	}
}
