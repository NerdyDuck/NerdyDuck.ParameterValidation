#Region "Copyright"
'*****************************************************************************
' <copyright file="SystemSettings.vb" owner="Daniel Kopp">
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
' <file name="SystemSettings.vb" date="2016-01-19">
' Wrapper class to xml-serialize a SystemSetting list into the format
' <systemSettings><setting /><setting/>...</systemSettings>.
' </file>
'*****************************************************************************
#End Region

Imports System.Xml.Serialization

''' <summary>
''' Wrapper class to xml-serialize a SystemSetting list into the format <systemSettings><setting /><setting/>...</systemSettings>.
''' </summary>
''' <remarks>The XmlRoot and XmlElement attributes define the XML element names during serialization.</remarks>
<XmlRoot("systemSettings", Namespace:="http://www.contoso.com")>
Public Class SystemSettings
	''' <summary>
	''' A list of settings to serialize or deserialize.
	''' </summary>
	<XmlElement("setting")>
	Public Property Settings() As List(Of SystemSetting)
		Get
			Return m_Settings
		End Get
		Set
			m_Settings = Value
		End Set
	End Property
	Private m_Settings As List(Of SystemSetting)

	''' <summary>
	''' Initializes a new instance of the SystemSettings class.
	''' </summary>
	Public Sub New()
		Settings = New List(Of SystemSetting)()
	End Sub
End Class
