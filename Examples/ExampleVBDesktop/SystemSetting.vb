#Region "Copyright"
'*****************************************************************************
' <copyright file="SystemSetting.vb" owner="Daniel Kopp">
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
' <file name="SystemSetting.vb" date="2016-01-19">
' Data model for a software setting parameter.
' </file>
'*****************************************************************************
#End Region

Imports System.Xml.Serialization
Imports NerdyDuck.ParameterValidation

''' <summary>
''' Data model for a software setting parameter.
''' </summary>
''' <remarks>The properties are decorated with XmlAttributeAttributes, so they serialize as attributes, with the exception of the value, which serializes as element.
''' The ValueSyncState enumeration is used to remember which value (Value or SerializedValue) has the more current data, so serialization or deserialization only has
''' to happen when necessary, not every time the data changes. While the Name, DisplayName, DataType and ConstraintString should not be modified by the application,
''' they require a Setter to be serializable by XmlSerializer.</remarks>
Public Class SystemSetting
	Private mConstraints As IReadOnlyList(Of Constraint) = Nothing
	Private AreConstraintsParsed As Boolean = False
	Private mConstraintString As String = Nothing
	Private mSerializedValue As String = Nothing
	Private mValue As Object
	Private SyncState As ValueSyncState = ValueSyncState.InSync

	''' <summary>
	''' Gets or sets the name of the setting.
	''' </summary>
	''' <remarks>The name of the setting to access the value programmatically.</remarks>
	<XmlAttribute("name")>
	Public Property Name() As String
		Get
			Return m_Name
		End Get
		Set
			m_Name = Value
		End Set
	End Property
	Private m_Name As String

	''' <summary>
	''' Gets or sets the display name of the setting.
	''' </summary>
	''' <remarks>The human-readable description of the setting. Might be stored with the setting, or a localized version retrieved during runtime.</remarks>
	<XmlAttribute("displayName")>
	Public Property DisplayName() As String
		Get
			Return m_DisplayName
		End Get
		Set
			m_DisplayName = Value
		End Set
	End Property
	Private m_DisplayName As String

	''' <summary>
	''' Gets or sets the data type of the setting value.
	''' </summary>
	<XmlAttribute("dataType")>
	Public Property DataType() As ParameterDataType
		Get
			Return m_DataType
		End Get
		Set
			m_DataType = Value
		End Set
	End Property
	Private m_DataType As ParameterDataType

	''' <summary>
	''' A string containing one or more constraints. May be null.
	''' </summary>
	''' <remarks>The is the serialized representation of the constraints in the Constraints property.</remarks>
	<XmlAttribute("constraints")>
	Public Property ConstraintString() As String
		Get
			Return mConstraintString
		End Get
		Set
			mConstraintString = Value
			AreConstraintsParsed = False
		End Set
	End Property

	''' <summary>
	''' Gets a list of objects derived from Constraint that specify the specific boundaries of Value. May be null.
	''' </summary>
	''' <remarks>This is the deserialized representation of the constraints in the ConstraintString property. This property is not serialized into the XML by the XmlSerializer.</remarks>
	<XmlIgnore>
	Public ReadOnly Property Constraints() As IReadOnlyList(Of Constraint)
		Get
			If Not AreConstraintsParsed Then
				' Constraints are only set once, and only parsed once.
				mConstraints = ConstraintParser.Parser.Parse(ConstraintString, DataType)
				AreConstraintsParsed = True
			End If
			Return mConstraints
		End Get
	End Property

	''' <summary>
	''' Gets or sets the serialized settings value.
	''' </summary>
	''' <remarks>This is the serialized representation of the Value property.</remarks>
	<XmlElement("value")>
	Public Property SerializedValue() As String
		Get
			' Only re-serialize the Value if it was changed (or during first serialization).
			If SyncState = ValueSyncState.ActualNewer Then
				mSerializedValue = ParameterConvert.ToString(mValue, DataType, Constraints)
				SyncState = ValueSyncState.InSync
			End If
			Return mSerializedValue
		End Get
		Set
			mSerializedValue = Value
			SyncState = ValueSyncState.SerializedNewer
		End Set
	End Property

	''' <summary>
	''' Gets or sets the settings value.
	''' </summary>
	''' <remarks>This is the deserialized representation of the SerializedValue property. This property is not serialized into the XML by the XmlSerializer.</remarks>
	<XmlIgnore>
	Public Property Value() As Object
		Get
			' Only deserialize SerializedValue if it was changed (or when Value is first accessed)
			If SyncState = ValueSyncState.SerializedNewer Then
				mValue = ParameterConvert.ToDataType(mSerializedValue, DataType, Constraints)
				SyncState = ValueSyncState.InSync
			End If
			Return mValue
		End Get
		Set
			' Checking the data type to match DataType was omitted here, but may be a good idea.
			mValue = Value
			SyncState = ValueSyncState.ActualNewer
		End Set
	End Property

	''' <summary>
	''' Initializes a new instance of the SystemSetting class.
	''' </summary>
	''' <remarks>Default constructor is necessary for XML serialization using XmlSerializer.</remarks>
	Public Sub New()
		Me.New(String.Empty, String.Empty, ParameterDataType.None, Nothing, Nothing)
	End Sub

	''' <summary>
	''' Initializes a new instance of the SystemSetting class with the specified parameters.
	''' </summary>
	''' <param name="_name">The name of the setting.</param>
	''' <param name="_displayName">The display name of the setting.</param>
	''' <param name="_dataType">The data type of the setting value.</param>
	''' <param name="_constraints">A list of objects derived from Constraint that specify the specific boundaries of Value. May be null.</param>
	''' <param name="_value">The settings value.</param>
	''' <remarks>This constructor is useful to create new settings.</remarks>
	Public Sub New(_name As String, _displayName As String, _dataType As ParameterDataType, _constraints As IReadOnlyList(Of Constraint), _value As Object)
		Name = _name
		DisplayName = _displayName
		DataType = _dataType
		mConstraints = _constraints
		' Create the constraint string right away for later serialization, so we don't need a separate update check as it is used for Value and SerializedValue.
		If mConstraints IsNot Nothing Then
			mConstraintString = ConstraintParser.ConcatConstraints(mConstraints)
		End If
		AreConstraintsParsed = True
		If _value IsNot Nothing Then
			' Checking the data type to match DataType was omitted here, but may be a good idea.
			Value = _value
		End If
	End Sub

	''' <summary>
	''' Initializes a new instance of the SystemSetting class with the specified parameters.
	''' </summary>
	''' <param name="_name">The name of the setting.</param>
	''' <param name="_displayName">The display name of the setting.</param>
	''' <param name="_dataType">The data type of the setting value.</param>
	''' <param name="_constraints">A string of constraints that specify the specific boundaries of Value. May be null.</param>
	''' <param name="_value">The serialized settings value.</param>
	''' <remarks>This constructor is useful to read a setting from the data store (e.g. XML file, database, etc.).</remarks>
	Public Sub New(_name As String, _displayName As String, _dataType As ParameterDataType, _constraints As String, _value As String)
		Name = _name
		DisplayName = _displayName
		DataType = _dataType
		mConstraintString = _constraints
		If _value IsNot Nothing Then
			SerializedValue = _value
		End If
	End Sub

	''' <summary>
	''' Specifies the state of Value and SerializedValue.
	''' </summary>
	Private Enum ValueSyncState
		''' <summary>
		''' Value and SerializedValue contain the same value.
		''' </summary>
		InSync

		''' <summary>
		''' SerializedValue has the newer (=current) value, or it has not yet been deserialized and stored in Value.
		''' </summary>
		SerializedNewer

		''' <summary>
		''' Value has the newer (=current) value, or it has not yet been serialized and stored in SerializedValue.
		''' </summary>
		ActualNewer
	End Enum
End Class

