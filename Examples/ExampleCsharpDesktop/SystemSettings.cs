#region Copyright
/*******************************************************************************
 * <copyright file="SystemSettings.cs" owner="Daniel Kopp">
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
 * <file name="SystemSettings.cs" date="2016-01-19">
 * Wrapper class to xml-serialize a SystemSetting list into the format
 * <systemSettings><setting /><setting/>...</systemSettings>.
 * </file>
 ******************************************************************************/
#endregion

using System.Collections.Generic;
using System.Xml.Serialization;

namespace ExampleCsharpDesktop
{
	/// <summary>
	/// Wrapper class to xml-serialize a SystemSetting list into the format <systemSettings><setting /><setting/>...</systemSettings>.
	/// </summary>
	/// <remarks>The XmlRoot and XmlElement attributes define the XML element names during serialization.</remarks>
	[XmlRoot("systemSettings", Namespace = "http://www.contoso.com")]
	public class SystemSettings
	{
		/// <summary>
		/// A list of settings to serialize or deserialize.
		/// </summary>
		[XmlElement("setting")]
		public List<SystemSetting> Settings { get; set; }

		/// <summary>
		/// Initializes a new instance of the SystemSettings class.
		/// </summary>
		public SystemSettings()
		{
			Settings = new List<SystemSetting>();
		}
	}
}
