#Region "Copyright"
'******************************************************************************
' <copyright file="DateTimeToOffsetConverter.vb" owner="Daniel Kopp">
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
' <file name="DateTimeToOffsetConverter.vb" date="2016-01-19">
' Converts a DateTimeOffset value to a DateTime value And vice versa.
' </file>
'*****************************************************************************
#End Region

Imports System
Imports System.Globalization
Imports System.Windows.Data

''' <summary>
''' Converts a DateTimeOffset value to a DateTime value And vice versa.
''' </summary>
''' <remarks>Required because the DatePicker control only handles DateTime values.</remarks>
Public Class DateTimeToOffsetConverter
	Implements IValueConverter
	''' <summary>
	''' Converts a value from DateTimeOffset to DateTime.
	''' </summary>
	''' <param name="value">The value to convert.</param>
	''' <param name="targetType">Not used.</param>
	''' <param name="parameter">Not used.</param>
	''' <param name="culture">Not used.</param>
	''' <returns>A DateTime value.</returns>
	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		If value IsNot Nothing Then
			Dim d As DateTimeOffset = DirectCast(value, DateTimeOffset)
			Return d.DateTime.Date
		End If
		Return Nothing
	End Function

	''' <summary>
	''' Converts a value from DateTime to DateTimeOffset.
	''' </summary>
	''' <param name="value">The value to convert.</param>
	''' <param name="targetType">Not used.</param>
	''' <param name="parameter">Not used.</param>
	''' <param name="culture">Not used.</param>
	''' <returns>A DateTimeOffset value.</returns>
	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		If value IsNot Nothing Then
			Dim d As DateTime = DirectCast(value, DateTime)
			Return New DateTimeOffset(d)
		End If
		Return Nothing
	End Function
End Class

