#region Copyright
/*******************************************************************************
 * <copyright file="SystemSettingViewModel.cs" owner="Daniel Kopp">
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
 * <file name="SystemSettingViewModel.cs" date="2016-01-19">
 * View model for the SystemSetting class.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.ParameterValidation;
using NerdyDuck.ParameterValidation.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace ExampleCsharpDesktop
{
	/// <summary>
	/// View model for the SystemSetting class.
	/// </summary>
	/// <remarks>Using a DependencyObject with dependency properties facilitates binding.
	/// Also, extracting relevant values from the Constraints into properties makes binding easier.
	/// Implementing INotifyDataErrorInfo is the easiest way to work with ParameterValidator and ParameterValidationResults to validate values entered by the user.</remarks>
	public class SystemSettingViewModel : DependencyObject, INotifyDataErrorInfo
	{
		// As all other properties are read-only, we only store ValidationResults for the Value property.
		private IEnumerable ValueErrors;

		// The dependency properties defining the properties of the class.
		public static readonly DependencyProperty NameProperty =
			DependencyProperty.Register("Name", typeof(string), typeof(SystemSettingViewModel), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty DisplayNameProperty =
			DependencyProperty.Register("DisplayName", typeof(string), typeof(SystemSettingViewModel), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty DataTypeProperty =
			DependencyProperty.Register("DataType", typeof(ParameterDataType), typeof(SystemSettingViewModel), new PropertyMetadata(ParameterDataType.None));
		// The CoerceValueCallback is used to validate the value if it is set directly via the dependency property mechanism (e.g. when set via binding).
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(object), typeof(SystemSettingViewModel), new FrameworkPropertyMetadata() { DefaultValue = null, CoerceValueCallback = new CoerceValueCallback(CoerceConstraints) });
		public static readonly DependencyProperty ConstraintsProperty =
			DependencyProperty.Register("Constraints", typeof(IReadOnlyList<Constraint>), typeof(SystemSettingViewModel), new PropertyMetadata(null));
		public static readonly DependencyProperty EnumValuesProperty =
			DependencyProperty.Register("EnumValues", typeof(IReadOnlyDictionary<string, object>), typeof(SystemSettingViewModel), new PropertyMetadata(null));
		public static readonly DependencyProperty MinimumValueProperty =
			DependencyProperty.Register("MinimumValue", typeof(object), typeof(SystemSettingViewModel), new PropertyMetadata(null));
		public static readonly DependencyProperty MaximumValueProperty =
			DependencyProperty.Register("MaximumValue", typeof(object), typeof(SystemSettingViewModel), new PropertyMetadata(null));
		public static readonly DependencyProperty StepProperty =
			DependencyProperty.Register("Step", typeof(object), typeof(SystemSettingViewModel), new PropertyMetadata(null));

		/// <summary>
		/// The event that is raised when the list of validation errors changes.
		/// </summary>
		/// <remarks>Required for INotifyDataErrorInfo.</remarks>
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		/// <summary>
		/// Gets the name of the setting.
		/// </summary>
		public string Name
		{
			get { return (string)GetValue(NameProperty); }
			private set { SetValue(NameProperty, value); }
		}

		/// <summary>
		/// Gets the display name of the setting.
		/// </summary>
		public string DisplayName
		{
			get { return (string)GetValue(DisplayNameProperty); }
			private set { SetValue(DisplayNameProperty, value); }
		}

		/// <summary>
		/// Gets the data type of the setting.
		/// </summary>
		public ParameterDataType DataType
		{
			get { return (ParameterDataType)GetValue(DataTypeProperty); }
			private set { SetValue(DataTypeProperty, value); }
		}

		/// <summary>
		/// Gets a list of Constraints specifying the boundaries of Value. May be null.
		/// </summary>
		public IReadOnlyList<Constraint> Constraints
		{
			get { return (IReadOnlyList<Constraint>)GetValue(ConstraintsProperty); }
			private set { SetValue(ConstraintsProperty, value); }
		}

		/// <summary>
		/// Gets or sets the setting value.
		/// </summary>
		public object Value
		{
			get { return GetValue(ValueProperty); }
			set
			{
				// Using a ParameterValidator to apply the Constraints to the new value and store the validation results. The new value is stored in any case.
				SetErrors(ParameterValidator.Validator.GetValidationResult(value, DataType, Constraints, Name, DisplayName));
				SetValue(ValueProperty, value);
			}
		}

		/// <summary>
		/// Gets a dictionary of enumeration names and values, if Value is an enumeration, and the Constraints contain a EnumTypeConstraint. May be null.
		/// </summary>
		/// <remarks>Helper property for easy binding.</remarks>
		public IReadOnlyDictionary<string, object> EnumValues
		{
			get { return (IReadOnlyDictionary<string, object>)GetValue(EnumValuesProperty); }
			private set { SetValue(EnumValuesProperty, value); }
		}

		/// <summary>
		/// Gets the minimum value of Value, if the Constraints contain a MinimumValueConstraint. May be null.
		/// </summary>
		/// <remarks>Helper property for easy binding.</remarks>
		public object MinimumValue
		{
			get { return GetValue(MinimumValueProperty); }
			set { SetValue(MinimumValueProperty, value); }
		}

		/// <summary>
		/// Gets the maximum value of Value, if the Constraints contain a MaximumValueConstraint. May be null.
		/// </summary>
		/// <remarks>Helper property for easy binding.</remarks>
		public object MaximumValue
		{
			get { return GetValue(MaximumValueProperty); }
			set { SetValue(MaximumValueProperty, value); }
		}

		/// <summary>
		/// Gets the step to use in a Slider control, if the Constraints contain a StepConstraint. May be null.
		/// </summary>
		/// <remarks>Helper property for easy binding.</remarks>
		public object Step
		{
			get { return GetValue(StepProperty); }
			set { SetValue(StepProperty, value); }
		}

		/// <summary>
		/// Gets a value indicating that the setting has validation errors.
		/// </summary>
		/// <remarks>Required for INotifyDataErrorInfo.</remarks>
		public bool HasErrors
		{
			get
			{
				IEnumerable errors = ValueErrors;
				if (errors == null)
				{
					return false;
				}
				return errors.GetEnumerator().MoveNext();
			}
		}

		/// <summary>
		/// Initializes a new instance of the SystemSettingViewModel class with the values of a SystemSetting.
		/// </summary>
		/// <param name="setting">The SystemSetting that the view model represents.</param>
		public SystemSettingViewModel(SystemSetting setting)
		{
			Name = setting.Name;
			DisplayName = setting.DisplayName;
			DataType = setting.DataType;
			Constraints = setting.Constraints;
			Value = setting.Value;

			if (setting.Constraints != null)
			{
				if (setting.DataType == ParameterDataType.Enum)
				{
					// Try to get the list of enumeration values. For a complete application, you should also look for an EnumValuesConstraint.
					// If both exist, filter the list from the EnumTypeConstraint so that it only contains entries that also exist in the EnumValuesConstraint.
					EnumTypeConstraint c = setting.Constraints.OfType<EnumTypeConstraint>().FirstOrDefault();
					EnumValues = c?.EnumValues;
				}
				else
				{
					// Try to retrieve Constraint settings for binding.
					// This list is not complete. More Constraints with value usable to modify control behavior exist.
					// Also, values should be retrieved more specific to the DataType, to prevent unnecessary searches (e.g. Strings have neither minimum nor maximum values, nor steps).
					MinimumValue = setting.Constraints.OfType<MinimumValueConstraint>().FirstOrDefault()?.MinimumValue;
					MaximumValue = setting.Constraints.OfType<MaximumValueConstraint>().FirstOrDefault()?.MaximumValue;
					Step = setting.Constraints.OfType<StepConstraint>().FirstOrDefault()?.StepSize;
				}
			}
		}

		/// <summary>
		/// Gets a list of ValidationResults for the specified property.
		/// </summary>
		/// <param name="propertyName">The name of the property to get the validation results for. If the value is null, the validation results for all properties are returned.</param>
		/// <returns>A list of ValidationResults, or null, if no errors exist.</returns>
		/// <remarks>Required for INotifyDataErrorInfo.</remarks>
		public IEnumerable GetErrors(string propertyName)
		{
			// We only validate Values, so null is returned for everything except "Value" and null.
			if (string.IsNullOrEmpty(propertyName) || propertyName == nameof(Value))
			{
				return ValueErrors;
			}
			return null;
		}

		/// <summary>
		/// Sets or resets the validation errors for Value, and raises the ErrorsChanged event.
		/// </summary>
		/// <param name="errors"></param>
		private void SetErrors(IEnumerable errors)
		{
			// Set to null if enumeration is empty.
			if (errors != null && !errors.GetEnumerator().MoveNext())
			{
				errors = null;
			}

			// Only raise event if an actual change happened.
			if (ValueErrors != errors)
			{
				ValueErrors = errors;
				ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Value)));
			}
		}

		/// <summary>
		/// Validate Value if it is set via dependency property mechanism.
		/// </summary>
		/// <param name="d">The SystemSettingViewModel to update.</param>
		/// <param name="value">The new value to set.</param>
		/// <returns></returns>
		private static object CoerceConstraints(DependencyObject d, object value)
		{
			SystemSettingViewModel setting = (SystemSettingViewModel)d;
			// Using a ParameterValidator to apply the Constraints to the new value and store the validation results.
			setting.SetErrors(ParameterValidator.Validator.GetValidationResult(value, setting.DataType, setting.Constraints, setting.Name, setting.DisplayName));

			// Return the value so it is stored even with validation errors. No coercion.
			return value;
		}
	}
}
