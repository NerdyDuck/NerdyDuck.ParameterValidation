#region Copyright
/*******************************************************************************
 * <copyright file="MainWindow.xaml.cs" owner="Daniel Kopp">
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
 * <file name="MainWindow.xaml.cs" date="2016-01-19">
 * The main application window.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.ParameterValidation;
using NerdyDuck.ParameterValidation.Constraints;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ExampleCsharpDesktop
{
	/// <summary>
	/// The main application window.
	/// </summary>
	public partial class MainWindow : Window
	{
		// The name of the file where the settings are stored. File is created when the application first runs.
		private const string SettingsFileName = "SystemSettings.xml";

		/// <summary>
		/// The original settings read from SettingsFileName.
		/// </summary>
		private List<SystemSetting> SourceSettings;

		/// <summary>
		/// Dependency property back-end for Settings.
		/// </summary>
		public static readonly DependencyProperty SettingsProperty =
			DependencyProperty.Register("Settings", typeof(ObservableCollection<SystemSettingViewModel>), typeof(MainWindow), new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets a observable list of SystemSettingViewModel.
		/// </summary>
		/// <remarks>Reflects the contents of SourceSettings.</remarks>
		public ObservableCollection<SystemSettingViewModel> Settings
		{
			get { return (ObservableCollection<SystemSettingViewModel>)GetValue(SettingsProperty); }
			set { SetValue(SettingsProperty, value); }
		}

		/// <summary>
		/// Initializes a new instance of the MainWindow class.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
			Settings = new ObservableCollection<SystemSettingViewModel>();
		}

		/// <summary>
		/// Handles the Loaded event. Creates and/or reads the settings file and creates the corresponding view model objects for the settings.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Create sample settings if the file does not yet exist.
			if (!System.IO.File.Exists(SettingsFileName))
				SaveSettingsFile(CreateSampleSettings());

			SourceSettings = ReadSettingsFile();
			foreach (SystemSetting setting in SourceSettings)
			{
				Settings.Add(new SystemSettingViewModel(setting));
			}
		}

		/// <summary>
		/// Create a simple list of settings with constraints.
		/// </summary>
		/// <returns>A list of settings.</returns>
		private List<SystemSetting> CreateSampleSettings()
		{
			List<SystemSetting> settings = new List<SystemSetting>();
			settings.Add(new SystemSetting("FirstName", "Given name", ParameterDataType.String, new Constraint[] { new MinimumLengthConstraint(1), new MaximumLengthConstraint(10) }, "Joe"));
			settings.Add(new SystemSetting("LastName", "Family name", ParameterDataType.String, new Constraint[] { new MinimumLengthConstraint(1), new MaximumLengthConstraint(10) }, "Average"));
			settings.Add(new SystemSetting("Age", "Age", ParameterDataType.Int32, new Constraint[] { new MinimumValueConstraint(ParameterDataType.Int32, 18), new MaximumValueConstraint(ParameterDataType.Int32, 88), new StepConstraint(ParameterDataType.Int32, 10) }, 18));
			settings.Add(new SystemSetting("Gender", "Gender", ParameterDataType.Enum, new Constraint[] { new EnumTypeConstraint(typeof(Gender).AssemblyQualifiedName) }, Gender.Male));
			settings.Add(new SystemSetting("DateJoined", "Joined at", ParameterDataType.DateTimeOffset, null, DateTimeOffset.Now.AddMonths(-1)));

			return settings;

		}

		/// <summary>
		/// Deserialize the settings file.
		/// </summary>
		/// <returns>A list of SystemSettings.</returns>
		private List<SystemSetting> ReadSettingsFile()
		{
			// The SystemSettings class is just a wrapper for the serialization of a list of SystemSetting objects.
			System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(SystemSettings));
			System.IO.FileStream stream = System.IO.File.OpenRead(SettingsFileName);
			SystemSettings settings = (SystemSettings)ser.Deserialize(stream);
			stream.Close();
			return settings.Settings;
		}

		/// <summary>
		/// Serialize the settings to an XML file.
		/// </summary>
		/// <param name="settings">The settings to serialize.</param>
		private void SaveSettingsFile(List<SystemSetting> settings)
		{
			System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(SystemSettings));
			System.IO.FileStream stream = System.IO.File.OpenWrite(SettingsFileName);
			stream.SetLength(0);
			// The SystemSettings class is just a wrapper for the serialization of a list of SystemSetting objects.
			SystemSettings s = new SystemSettings();
			s.Settings = settings;
			ser.Serialize(stream, s);
			stream.Flush();
			stream.Close();
		}

		/// <summary>
		/// Save the values to the settings file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSave_Click(object sender, RoutedEventArgs e)
		{
			// Only save settings if no validation errors exist.
			if (Settings.Where(p => p.HasErrors).Any())
			{
				MessageBox.Show(this, "One or more settings are invalid!", this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// Update source settings with values from view model and safe to file.
			foreach (SystemSettingViewModel setting in Settings)
			{
				SourceSettings.Where(p => p.Name == setting.Name).Single().Value = setting.Value;
			}
			SaveSettingsFile(SourceSettings);
			MessageBox.Show(this, "Settings saved!", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
		}

		/// <summary>
		/// Reset the values to the ones read from the settings file.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnReset_Click(object sender, RoutedEventArgs e)
		{
			foreach (SystemSetting setting in SourceSettings)
			{
				Settings.Where(p => p.Name == setting.Name).Single().Value = setting.Value;
			}
		}

		/// <summary>
		/// Reset the settings to the original sample settings.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnOriginal_Click(object sender, RoutedEventArgs e)
		{
			Settings.Clear();
			foreach (SystemSetting setting in CreateSampleSettings())
			{
				Settings.Add(new SystemSettingViewModel(setting));
			}
		}
	}
}
