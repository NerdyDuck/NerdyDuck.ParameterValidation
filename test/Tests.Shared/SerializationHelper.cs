#region Copyright
/*******************************************************************************
 * NerdyDuck.Tests.Collections - Unit tests for the
 * NerdyDuck.ParameterValidation assembly
 * 
 * The MIT License (MIT)
 *
 * Copyright (c) Daniel Kopp, dak@nerdyduck.de
 *
 * All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 ******************************************************************************/
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NerdyDuck.Tests.ParameterValidation
{
	/// <summary>
	/// Contains methods to serialize and deserialize objects using the SerializableAttribute and/or ISerializable.
	/// </summary>
	[ExcludeFromCodeCoverage]
	public static class SerializationHelper
	{
		/// <summary>
		/// Serializes an object.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="ex">The exception to serialize.</param>
		/// <returns>A MemoryStream containing the serialized object data.</returns>
		public static MemoryStream Serialize<T>(T ex) where T : class
		{
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream buffer = new MemoryStream();
			formatter.Serialize(buffer, ex);
			buffer.Seek(0, SeekOrigin.Begin);

			return buffer;
		}

		/// <summary>
		/// Deserializes an object.
		/// </summary>
		/// <typeparam name="T">The type of object to deserialize.</typeparam>
		/// <param name="buffer">The MemoryStream containing the serialized object data.</param>
		/// <returns>The deserialized object.</returns>
		public static T Deserialize<T>(MemoryStream buffer) where T : class
		{
			BinaryFormatter formatter = new BinaryFormatter();
			return (T)formatter.Deserialize(buffer);
		}
	}
}
