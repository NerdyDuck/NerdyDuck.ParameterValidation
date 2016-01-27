#Region "Copyright"
'******************************************************************************
' <copyright file="SystemSettingViewModel.vb" owner="Daniel Kopp">
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
' <file name="SystemSettingViewModel.vb" date="2016-01-19">
' View model for the SystemSetting class.
' </file>
'*****************************************************************************
#End Region

Imports System.ComponentModel
Imports NerdyDuck.ParameterValidation
Imports NerdyDuck.ParameterValidation.Constraints

''' <summary>
''' View model for the SystemSetting class.
''' </summary>
''' <remarks>Using a DependencyObject with dependency properties facilitates binding.
''' Also, extracting relevant values from the Constraints into properties makes binding easier.
''' Implementing INotifyDataErrorInfo is the easiest way to work with ParameterValidator and ParameterValidationResults to validate values entered by the user.</remarks>
Public Class SystemSettingViewModel
	Inherits DependencyObject
	Implements INotifyDataErrorInfo

	' As all other properties are read-only, we only store ValidationResults for the Value property.
	Private ValueErrors As IEnumerable

	' The dependency properties defining the properties of the class.
	Public Shared ReadOnly NameProperty As DependencyProperty = DependencyProperty.Register("Name", GetType(String), GetType(SystemSettingViewModel), New PropertyMetadata(String.Empty))
	Public Shared ReadOnly DisplayNameProperty As DependencyProperty = DependencyProperty.Register("DisplayName", GetType(String), GetType(SystemSettingViewModel), New PropertyMetadata(String.Empty))
	Public Shared ReadOnly DataTypeProperty As DependencyProperty = DependencyProperty.Register("DataType", GetType(ParameterDataType), GetType(SystemSettingViewModel), New PropertyMetadata(ParameterDataType.None))
	' The CoerceValueCallback is used to validate the value if it is set directly via the dependency property mechanism (e.g. when set via binding).
	Public Shared ReadOnly ValueProperty As DependencyProperty = DependencyProperty.Register("Value", GetType(Object), GetType(SystemSettingViewModel), New FrameworkPropertyMetadata() With {.DefaultValue = Nothing, .CoerceValueCallback = New CoerceValueCallback(AddressOf CoerceConstraints)})
	Public Shared ReadOnly ConstraintsProperty As DependencyProperty = DependencyProperty.Register("Constraints", GetType(IReadOnlyList(Of Constraint)), GetType(SystemSettingViewModel), New PropertyMetadata(Nothing))
	Public Shared ReadOnly EnumValuesProperty As DependencyProperty = DependencyProperty.Register("EnumValues", GetType(IReadOnlyDictionary(Of String, Object)), GetType(SystemSettingViewModel), New PropertyMetadata(Nothing))
	Public Shared ReadOnly MinimumValueProperty As DependencyProperty = DependencyProperty.Register("MinimumValue", GetType(Object), GetType(SystemSettingViewModel), New PropertyMetadata(Nothing))
	Public Shared ReadOnly MaximumValueProperty As DependencyProperty = DependencyProperty.Register("MaximumValue", GetType(Object), GetType(SystemSettingViewModel), New PropertyMetadata(Nothing))
	Public Shared ReadOnly StepProperty As DependencyProperty = DependencyProperty.Register("Step", GetType(Object), GetType(SystemSettingViewModel), New PropertyMetadata(Nothing))

	''' <summary>
	''' The event that is raised when the list of validation errors changes.
	''' </summary>
	''' <remarks>Required for INotifyDataErrorInfo.</remarks>
	Public Event ErrorsChanged As EventHandler(Of DataErrorsChangedEventArgs) Implements INotifyDataErrorInfo.ErrorsChanged

	''' <summary>
	''' Gets the name of the setting.
	''' </summary>
	Public Property Name() As String
		Get
			Return DirectCast(GetValue(NameProperty), String)
		End Get
		Private Set
			SetValue(NameProperty, Value)
		End Set
	End Property

	''' <summary>
	''' Gets the display name of the setting.
	''' </summary>
	Public Property DisplayName() As String
		Get
			Return DirectCast(GetValue(DisplayNameProperty), String)
		End Get
		Private Set
			SetValue(DisplayNameProperty, Value)
		End Set
	End Property

	''' <summary>
	''' Gets the data type of the setting.
	''' </summary>
	Public Property DataType() As ParameterDataType
		Get
			Return DirectCast(GetValue(DataTypeProperty), ParameterDataType)
		End Get
		Private Set
			SetValue(DataTypeProperty, Value)
		End Set
	End Property

	''' <summary>
	''' Gets a list of Constraints specifying the boundaries of Value. May be null.
	''' </summary>
	Public Property Constraints() As IReadOnlyList(Of Constraint)
		Get
			Return DirectCast(GetValue(ConstraintsProperty), IReadOnlyList(Of Constraint))
		End Get
		Private Set
			SetValue(ConstraintsProperty, Value)
		End Set
	End Property

	''' <summary>
	''' Gets or sets the setting value.
	''' </summary>
	Public Property Value() As Object
		Get
			Return GetValue(ValueProperty)
		End Get
		Set
			' Using a ParameterValidator to apply the Constraints to the new value and store the validation results. The new value is stored in any case.
			SetErrors(ParameterValidator.Validator.GetValidationResult(Value, DataType, Constraints, Name, DisplayName))
			SetValue(ValueProperty, Value)
		End Set
	End Property

	''' <summary>
	''' Gets a dictionary of enumeration names and values, if Value is an enumeration, and the Constraints contain a EnumTypeConstraint. May be null.
	''' </summary>
	''' <remarks>Helper property for easy binding.</remarks>
	Public Property EnumValues() As IReadOnlyDictionary(Of String, Object)
		Get
			Return DirectCast(GetValue(EnumValuesProperty), IReadOnlyDictionary(Of String, Object))
		End Get
		Private Set
			SetValue(EnumValuesProperty, Value)
		End Set
	End Property

	''' <summary>
	''' Gets the minimum value of Value, if the Constraints contain a MinimumValueConstraint. May be null.
	''' </summary>
	''' <remarks>Helper property for easy binding.</remarks>
	Public Property MinimumValue() As Object
		Get
			Return GetValue(MinimumValueProperty)
		End Get
		Set
			SetValue(MinimumValueProperty, Value)
		End Set
	End Property

	''' <summary>
	''' Gets the maximum value of Value, if the Constraints contain a MaximumValueConstraint. May be null.
	''' </summary>
	''' <remarks>Helper property for easy binding.</remarks>
	Public Property MaximumValue() As Object
		Get
			Return GetValue(MaximumValueProperty)
		End Get
		Set
			SetValue(MaximumValueProperty, Value)
		End Set
	End Property

	''' <summary>
	''' Gets the step to use in a Slider control, if the Constraints contain a StepConstraint. May be null.
	''' </summary>
	''' <remarks>Helper property for easy binding.</remarks>
	Public Property [Step]() As Object
		Get
			Return GetValue(StepProperty)
		End Get
		Set
			SetValue(StepProperty, Value)
		End Set
	End Property

	''' <summary>
	''' Gets a value indicating that the setting has validation errors.
	''' </summary>
	''' <remarks>Required for INotifyDataErrorInfo.</remarks>
	Public ReadOnly Property HasErrors() As Boolean Implements INotifyDataErrorInfo.HasErrors
		Get
			Dim errors As IEnumerable = ValueErrors
			If errors Is Nothing Then
				Return False
			End If
			Return errors.GetEnumerator().MoveNext()
		End Get
	End Property

	''' <summary>
	''' Initializes a new instance of the SystemSettingViewModel class with the values of a SystemSetting.
	''' </summary>
	''' <param name="setting">The SystemSetting that the view model represents.</param>
	Public Sub New(ByRef setting As SystemSetting)
		Name = setting.Name
		DisplayName = setting.DisplayName
		DataType = setting.DataType
		Constraints = setting.Constraints
		Value = setting.Value

		If setting.Constraints IsNot Nothing Then
			If setting.DataType = ParameterDataType.Enum Then
				' Try to get the list of enumeration values. For a complete application, you should also look for an EnumValuesConstraint.
				' If both exist, filter the list from the EnumTypeConstraint so that it only contains entries that also exist in the EnumValuesConstraint.
				Dim c As EnumTypeConstraint = setting.Constraints.OfType(Of EnumTypeConstraint).FirstOrDefault()
				EnumValues = c?.EnumValues
			Else
				' Try to retrieve Constraint settings for binding.
				' This list Is Not complete. More Constraints with value usable to modify control behavior exist.
				' Also, values should be retrieved more specific to the DataType, to prevent unnecessary searches (e.g. Strings have neither minimum nor maximum values, nor steps).
				MinimumValue = setting.Constraints.OfType(Of MinimumValueConstraint).FirstOrDefault()?.MinimumValue
				MaximumValue = setting.Constraints.OfType(Of MaximumValueConstraint).FirstOrDefault()?.MaximumValue
				Me.Step = setting.Constraints.OfType(Of StepConstraint).FirstOrDefault()?.StepSize
			End If
		End If
	End Sub

	''' <summary>
	''' Gets a list of ValidationResults for the specified property.
	''' </summary>
	''' <param name="propertyName">The name of the property to get the validation results for. If the value is null, the validation results for all properties are returned.</param>
	''' <returns>A list of ValidationResults, or null, if no errors exist.</returns>
	''' <remarks>Required for INotifyDataErrorInfo.</remarks>
	Public Function GetErrors(propertyName As String) As IEnumerable Implements INotifyDataErrorInfo.GetErrors
		' We only validate Values, so null is returned for everything except "Value" and null.
		If String.IsNullOrEmpty(propertyName) OrElse propertyName = NameOf(Value) Then
			Return ValueErrors
		End If
		Return Nothing
	End Function

	''' <summary>
	''' Sets or resets the validation errors for Value, and raises the ErrorsChanged event.
	''' </summary>
	''' <param name="errors"></param>
	Private Sub SetErrors(errors As IEnumerable)
		' Set to null if enumeration Is empty.
		If errors IsNot Nothing AndAlso Not errors.GetEnumerator().MoveNext() Then
			errors = Nothing
		End If

		' Only raise event if an actual change happened.
		If ValueErrors IsNot errors Then
			ValueErrors = errors
			RaiseEvent ErrorsChanged(Me, New DataErrorsChangedEventArgs(NameOf(Value)))
		End If
	End Sub

	''' <summary>
	''' Validate Value if it is set via dependency property mechanism.
	''' </summary>
	''' <param name="d">The SystemSettingViewModel to update.</param>
	''' <param name="value">The new value to set.</param>
	''' <returns></returns>
	Private Shared Function CoerceConstraints(d As DependencyObject, value As Object) As Object
		Dim setting As SystemSettingViewModel = DirectCast(d, SystemSettingViewModel)
		' Using a ParameterValidator to apply the Constraints to the new value and store the validation results.
		setting.SetErrors(ParameterValidator.Validator.GetValidationResult(value, setting.DataType, setting.Constraints, setting.Name, setting.DisplayName))

		' Return the value so it is stored even with validation errors. No coercion.
		Return value
	End Function
End Class
