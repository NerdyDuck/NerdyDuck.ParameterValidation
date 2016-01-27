#Region "Copyright"
'*****************************************************************************
' <copyright file="ParameterDataTypeTemplateSelector.vb" owner="Daniel Kopp">
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
' <file name="ParameterDataTypeTemplateSelector.vb" date="2016-01-19">
' Retrieves the correct item template based on the DataType of the
' SystemSettingViewModel.
' </file>
'*****************************************************************************
#End Region

Imports NerdyDuck.ParameterValidation

''' <summary>
''' Retrieves the correct item template based on the DataType of the SystemSettingViewModel.
''' </summary>
Public Class ParameterDataTypeTemplateSelector
	Inherits DataTemplateSelector
	''' <summary>
	''' Selects DataTemplate based on the DataType of s SystemSettingViewModel.
	''' </summary>
	''' <param name="item">The SystemSettingViewModel to display.</param>
	''' <param name="container">The control that displays the SystemSettingViewModel.</param>
	''' <returns></returns>
	Public Overrides Function SelectTemplate(item As Object, container As DependencyObject) As DataTemplate
		Dim setting As SystemSettingViewModel = TryCast(item, SystemSettingViewModel)
		Dim element As FrameworkElement = TryCast(container, FrameworkElement)
		If setting Is Nothing OrElse element Is Nothing Then
			Return Nothing
		End If

		' The templates with the appropriate names must be a resource discoverable from the context of the control.
		' This is only a small selection of templates for some data types. More templates could be created for other data types,
		' or different templates depending on the Constraints attached to a value.
		Select Case setting.DataType
			Case ParameterDataType.String
				Return TryCast(element.FindResource("StringListItemTemplate"), DataTemplate)
			Case ParameterDataType.Int32
				Return TryCast(element.FindResource("IntegerListItemTemplate"), DataTemplate)
			Case ParameterDataType.DateTimeOffset
				Return TryCast(element.FindResource("DateTimeListItemTemplate"), DataTemplate)
			Case ParameterDataType.Enum
				If setting.EnumValues IsNot Nothing Then
					Return TryCast(element.FindResource("EnumListItemTemplate"), DataTemplate)
				Else
					Return TryCast(element.FindResource("StringListItemTemplate"), DataTemplate)
				End If
			Case Else
				Return TryCast(element.FindResource("StringListItemTemplate"), DataTemplate)
		End Select
	End Function
End Class

