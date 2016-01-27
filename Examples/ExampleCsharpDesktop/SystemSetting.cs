#region Copyright
/*******************************************************************************
 * <copyright file="SystemSetting.cs" owner="Daniel Kopp">
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
 * <file name="SystemSetting.cs" date="2016-01-19">
 * Data model for a software setting parameter.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.ParameterValidation;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ExampleCsharpDesktop
{
	/// <summary>
	/// Data model for a software setting parameter.
	/// </summary>
	/// <remarks>The properties are decorated with XmlAttributeAttributes, so they serialize as attributes, with the exception of the value, which serializes as element.
	/// The ValueSyncState enumeration is used to remember which value (Value or SerializedValue) has the more current data, so serialization or deserialization only has
	/// to happen when necessary, not every time the data changes. While the Name, DisplayName, DataType and ConstraintString should not be modified by the application,
	/// they require a Setter to be serializable by XmlSerializer.</remarks>
	public class SystemSetting
	{
		private IReadOnlyList<Constraint> mConstraints = null;
		private bool AreConstraintsParsed = false;
		private string mConstraintString = null;
		private string mSerializedValue = null;
		private object mValue;
		private ValueSyncState SyncState = ValueSyncState.InSync;

		/// <summary>
		/// Gets or sets the name of the setting.
		/// </summary>
		/// <remarks>The name of the setting to access the value programmatically.</remarks>
		[XmlAttribute("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the display name of the setting.
		/// </summary>
		/// <remarks>The human-readable description of the setting. Might be stored with the setting, or a localized version retrieved during runtime.</remarks>
		[XmlAttribute("displayName")]
		public string DisplayName { get; set; }

		/// <summary>
		/// Gets or sets the data type of the setting value.
		/// </summary>
		[XmlAttribute("dataType")]
		public ParameterDataType DataType { get; set; }

		/// <summary>
		/// A string containing one or more constraints. May be null.
		/// </summary>
		/// <remarks>The is the serialized representation of the constraints in the Constraints property.</remarks>
		[XmlAttribute("constraints")]
		public string ConstraintString
		{
			get { return mConstraintString; }
			set
			{
				mConstraintString = value;
				AreConstraintsParsed = false;
			}
		}

		/// <summary>
		/// Gets a list of objects derived from Constraint that specify the specific boundaries of Value. May be null.
		/// </summary>
		/// <remarks>This is the deserialized representation of the constraints in the ConstraintString property. This property is not serialized into the XML by the XmlSerializer.</remarks>
		[XmlIgnore]
		public IReadOnlyList<Constraint> Constraints
		{
			get
			{
				if (!AreConstraintsParsed)
				{
					// Constraints are only set once, and only parsed once.
					mConstraints = ConstraintParser.Parser.Parse(ConstraintString, DataType);
					AreConstraintsParsed = true;
				}
				return mConstraints;
			}
		}

		/// <summary>
		/// Gets or sets the serialized settings value.
		/// </summary>
		/// <remarks>This is the serialized representation of the Value property.</remarks>
		[XmlElement("value")]
		public string SerializedValue
		{
			get
			{
				// Only re-serialize the Value if it was changed (or during first serialization).
				if (SyncState == ValueSyncState.ActualNewer)
				{
					mSerializedValue = ParameterConvert.ToString(mValue, DataType, Constraints);
					SyncState = ValueSyncState.InSync;
				}
				return mSerializedValue;
			}
			set
			{
				mSerializedValue = value;
				SyncState = ValueSyncState.SerializedNewer;
			}
		}

		/// <summary>
		/// Gets or sets the settings value.
		/// </summary>
		/// <remarks>This is the deserialized representation of the SerializedValue property. This property is not serialized into the XML by the XmlSerializer.</remarks>
		[XmlIgnore]
		public object Value
		{
			get
			{
				// Only deserialize SerializedValue if it was changed (or when Value is first accessed)
				if (SyncState == ValueSyncState.SerializedNewer)
				{
					mValue = ParameterConvert.ToDataType(mSerializedValue, DataType, Constraints);
					SyncState = ValueSyncState.InSync;
				}
				return mValue;
			}
			set
			{
				// Checking the data type to match DataType was omitted here, but may be a good idea.
				mValue = value;
				SyncState = ValueSyncState.ActualNewer;
			}
		}

		/// <summary>
		/// Initializes a new instance of the SystemSetting class.
		/// </summary>
		/// <remarks>Default constructor is necessary for XML serialization using XmlSerializer.</remarks>
		public SystemSetting()
			: this(string.Empty, string.Empty, ParameterDataType.None, null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the SystemSetting class with the specified parameters.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="displayName">The display name of the setting.</param>
		/// <param name="dataType">The data type of the setting value.</param>
		/// <param name="constraints">A list of objects derived from Constraint that specify the specific boundaries of Value. May be null.</param>
		/// <param name="value">The settings value.</param>
		/// <remarks>This constructor is useful to create new settings.</remarks>
		public SystemSetting(string name, string displayName, ParameterDataType dataType, IReadOnlyList<Constraint> constraints, object value)
		{
			Name = name;
			DisplayName = displayName;
			DataType = dataType;
			mConstraints = constraints;
			// Create the constraint string right away for later serialization, so we don't need a separate update check as it is used for Value and SerializedValue.
			if (mConstraints != null)
				mConstraintString = ConstraintParser.ConcatConstraints(mConstraints);
			AreConstraintsParsed = true;
			if (value != null)
			{
				// Checking the data type to match DataType was omitted here, but may be a good idea.
				Value = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the SystemSetting class with the specified parameters.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="displayName">The display name of the setting.</param>
		/// <param name="dataType">The data type of the setting value.</param>
		/// <param name="constraints">A string of constraints that specify the specific boundaries of Value. May be null.</param>
		/// <param name="value">The serialized settings value.</param>
		/// <remarks>This constructor is useful to read a setting from the data store (e.g. XML file, database, etc.).</remarks>
		public SystemSetting(string name, string displayName, ParameterDataType dataType, string constraints, string value)
		{
			Name = name;
			DisplayName = displayName;
			DataType = dataType;
			mConstraintString = constraints;
			if (value != null)
			{
				SerializedValue = value;
			}
		}

		/// <summary>
		/// Specifies the state of Value and SerializedValue.
		/// </summary>
		private enum ValueSyncState
		{
			/// <summary>
			/// Value and SerializedValue contain the same value.
			/// </summary>
			InSync,

			/// <summary>
			/// SerializedValue has the newer (=current) value, or it has not yet been deserialized and stored in Value.
			/// </summary>
			SerializedNewer,

			/// <summary>
			/// Value has the newer (=current) value, or it has not yet been serialized and stored in SerializedValue.
			/// </summary>
			ActualNewer
		}
	}
}
