#Region "Copyright"
'*****************************************************************************
' <copyright file="DoubleObjectInt32Converter.vb" owner="Daniel Kopp">
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
' <file name="DoubleObjectInt32Converter.vb" date="2016-01-19">
' Converts a integer value to a double value And vice versa.
' </file>
'*****************************************************************************
#End Region

Imports System.Globalization

''' <summary>
''' Converts an integer value to a double value and vice versa.
''' </summary>
''' <remarks>Required because WPF binding cannot simply convert integers to doubles when they are boxed in a property of type System.Object.</remarks>
Public Class DoubleObjectInt32Converter
	Implements IValueConverter
	''' <summary>
	''' Converts a integer value (or any value implementing IConvertible) into a double value.
	''' </summary>
	''' <param name="value">The value to convert.</param>
	''' <param name="targetType">Not used.</param>
	''' <param name="parameter">Not used</param>
	''' <param name="culture">Not used.</param>
	''' <returns>A double value.</returns>
	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		Dim conv As IConvertible = TryCast(value, IConvertible)
		If conv Is Nothing Then
			Return Nothing
		End If

		Return conv.ToDouble(CultureInfo.InvariantCulture)
	End Function

	''' <summary>
	''' Converts a double value (or any value implementing IConvertible) into an integer value.
	''' </summary>
	''' <param name="value">The value to convert.</param>
	''' <param name="targetType">Not used.</param>
	''' <param name="parameter">Not used</param>
	''' <param name="culture">Not used.</param>
	''' <returns>An integer value.</returns>
	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		Dim conv As IConvertible = TryCast(value, IConvertible)
		If conv Is Nothing Then
			Return Nothing
		End If

		Return conv.ToInt32(CultureInfo.InvariantCulture)
	End Function
End Class
