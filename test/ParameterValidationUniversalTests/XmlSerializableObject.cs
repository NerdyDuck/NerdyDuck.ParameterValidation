#region Copyright
/*******************************************************************************
 * <copyright file="XmlSerializableObject.cs" owner="Daniel Kopp">
 * Copyright 2015 Daniel Kopp
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
 * <assembly name="NerdyDuck.Tests.ParameterValidation">
 * Unit tests for NerdyDuck.ParameterValidation assembly.
 * </assembly>
 * <file name="XmlSerializableObject.cs" date="2015-11-26">
 * Class to test xml serialization.
 * </file>
 ******************************************************************************/
#endregion

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NerdyDuck.Tests.ParameterValidation
{
	/// <summary>
	/// Class to test xml serialization.
	/// </summary>
#if WINDOWS_DESKTOP
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#endif
	public class XmlSerializableObject : IXmlSerializable
	{
		private bool ThrowException;

		public string MyValue { get; set; }

		public XmlSerializableObject()
		{
			ThrowException = false;
		}

		public XmlSerializableObject(bool throwException)
		{
			ThrowException = throwException;
		}

		public XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(XmlReader reader)
		{
			if (ThrowException)
				throw new InvalidOperationException("User exception");

			MyValue = reader.GetAttribute("MyValue");
		}

		public void WriteXml(XmlWriter writer)
		{
			if (ThrowException)
				throw new InvalidOperationException("User exception");

			writer.WriteAttributeString("MyValue", MyValue);
		}
	}
}
