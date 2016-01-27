#Region "Copyright"
'******************************************************************************
' <copyright file="MainWindow.xaml.vb" owner="Daniel Kopp">
' Copyright 2015-2016 Daniel Kopp
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may Not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'     http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law Or agreed to in writing, software
' distributed under the License Is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES Or CONDITIONS OF ANY KIND, either express Or implied.
' See the License for the specific language governing permissions And
' limitations under the License.
' </copyright>
' <author name="Daniel Kopp" email="dak@nerdyduck.de" />
' <assembly name="ExampleVBDesktop">
' Example application for NerdyDuck.ParameterValidation library.
' </assembly>
' <file name="MainWindow.xaml.vb" date="2016-01-19">
' The main application window.
' </file>
'*****************************************************************************
#End Region

Imports System.Collections.ObjectModel
Imports NerdyDuck.ParameterValidation
Imports NerdyDuck.ParameterValidation.Constraints

''' <summary>
''' The main application window.
''' </summary>
Class MainWindow

	' The name of the file where the settings are stored. File is created when the application first runs.
	Private Const SettingsFileName As String = "SystemSettings.xml"

	''' <summary>
	''' The original settings read from SettingsFileName.
	''' </summary>
	Private SourceSettings As List(Of SystemSetting)

	''' <summary>
	''' Dependency property back-end for Settings.
	''' </summary>
	Public Shared ReadOnly SettingsProperty As DependencyProperty = DependencyProperty.Register("Settings", GetType(ObservableCollection(Of SystemSettingViewModel)), GetType(MainWindow), New PropertyMetadata(Nothing))

	''' <summary>
	''' Gets or sets a observable list of SystemSettingViewModel.
	''' </summary>
	''' <remarks>Reflects the contents of SourceSettings.</remarks>
	Public Property Settings() As ObservableCollection(Of SystemSettingViewModel)
		Get
			Return DirectCast(GetValue(SettingsProperty), ObservableCollection(Of SystemSettingViewModel))
		End Get
		Set
			SetValue(SettingsProperty, Value)
		End Set
	End Property

	''' <summary>
	''' Initializes a new instance of the MainWindow class.
	''' </summary>
	Public Sub New()
		InitializeComponent()
		Settings = New ObservableCollection(Of SystemSettingViewModel)()
	End Sub

	''' <summary>
	''' Handles the Loaded event. Creates and/or reads the settings file and creates the corresponding view model objects for the settings.
	''' </summary>
	''' <param name="sender"></param>
	''' <param name="e"></param>
	Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
		' Create sample settings if the file does not yet exist.
		If Not System.IO.File.Exists(SettingsFileName) Then
			SaveSettingsFile(CreateSampleSettings())
		End If

		SourceSettings = ReadSettingsFile()
		For Each setting As SystemSetting In SourceSettings
			Settings.Add(New SystemSettingViewModel(setting))
		Next
	End Sub

	''' <summary>
	''' Create a simple list of settings with constraints.
	''' </summary>
	''' <returns>A list of settings.</returns>
	Private Function CreateSampleSettings() As List(Of SystemSetting)
		Dim settings As New List(Of SystemSetting)()
		settings.Add(New SystemSetting("FirstName", "Given name", ParameterDataType.[String], New Constraint() {New MinimumLengthConstraint(1), New MaximumLengthConstraint(10)}, "Joe"))
		settings.Add(New SystemSetting("LastName", "Family name", ParameterDataType.[String], New Constraint() {New MinimumLengthConstraint(1), New MaximumLengthConstraint(10)}, "Average"))
		settings.Add(New SystemSetting("Age", "Age", ParameterDataType.Int32, New Constraint() {New MinimumValueConstraint(ParameterDataType.Int32, 18), New MaximumValueConstraint(ParameterDataType.Int32, 88), New StepConstraint(ParameterDataType.Int32, 10)}, 18))
		settings.Add(New SystemSetting("Gender", "Gender", ParameterDataType.[Enum], New Constraint() {New EnumTypeConstraint(GetType(Gender).AssemblyQualifiedName)}, Gender.Male))
		settings.Add(New SystemSetting("DateJoined", "Joined at", ParameterDataType.DateTimeOffset, Nothing, DateTimeOffset.Now.AddMonths(-1)))

		Return settings

	End Function

	''' <summary>
	''' Deserialize the settings file.
	''' </summary>
	''' <returns>A list of SystemSettings.</returns>
	Private Function ReadSettingsFile() As List(Of SystemSetting)
		' The SystemSettings class is just a wrapper for the serialization of a list of SystemSetting objects.
		Dim ser As New System.Xml.Serialization.XmlSerializer(GetType(SystemSettings))
		Dim stream As System.IO.FileStream = System.IO.File.OpenRead(SettingsFileName)
		Dim settings As SystemSettings = DirectCast(ser.Deserialize(stream), SystemSettings)
		stream.Close()
		Return settings.Settings
	End Function

	''' <summary>
	''' Serialize the settings to an XML file.
	''' </summary>
	''' <param name="settings">The settings to serialize.</param>
	Private Sub SaveSettingsFile(settings As List(Of SystemSetting))
		Dim ser As New System.Xml.Serialization.XmlSerializer(GetType(SystemSettings))
		Dim stream As System.IO.FileStream = System.IO.File.OpenWrite(SettingsFileName)
		stream.SetLength(0)
		' The SystemSettings class is just a wrapper for the serialization of a list of SystemSetting objects.
		Dim s As New SystemSettings()
		s.Settings = settings
		ser.Serialize(stream, s)
		stream.Flush()
		stream.Close()
	End Sub

	''' <summary>
	''' Save the values to the settings file.
	''' </summary>
	''' <param name="sender"></param>
	''' <param name="e"></param>
	Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs)
		' Only save settings if no validation errors exist.
		If Settings.Where(Function(p) p.HasErrors).Any() Then
			MessageBox.Show(Me, "One or more settings are invalid!", Me.Title, MessageBoxButton.OK, MessageBoxImage.Error)
			Return
		End If

		' Update source settings with values from view model and safe to file.
		For Each setting As SystemSettingViewModel In Settings
			SourceSettings.Where(Function(p) p.Name = setting.Name).Single().Value = setting.Value
		Next
		SaveSettingsFile(SourceSettings)
		MessageBox.Show(Me, "Settings saved!", Me.Title, MessageBoxButton.OK, MessageBoxImage.Information)
	End Sub

	''' <summary>
	''' Reset the values to the ones read from the settings file.
	''' </summary>
	''' <param name="sender"></param>
	''' <param name="e"></param>
	Private Sub btnReset_Click(sender As Object, e As RoutedEventArgs)
		For Each setting As SystemSetting In SourceSettings
			Settings.Where(Function(p) p.Name = setting.Name).Single().Value = setting.Value
		Next
	End Sub

	''' <summary>
	''' Reset the settings to the original sample settings.
	''' </summary>
	''' <param name="sender"></param>
	''' <param name="e"></param>
	Private Sub btnOriginal_Click(sender As Object, e As RoutedEventArgs)
		Settings.Clear()
		For Each setting As SystemSetting In CreateSampleSettings()
			Settings.Add(New SystemSettingViewModel(setting))
		Next
	End Sub

End Class
