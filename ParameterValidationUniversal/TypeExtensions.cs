#region Copyright
/*******************************************************************************
 * <copyright file="TypeExtensions.cs" owner="Daniel Kopp">
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
 * <assembly name="NerdyDuck.ParameterValidation">
 * Validation and serialization of parameter values for .NET
 * </assembly>
 * <file name="TypeExtensions.cs" date="2015-09-29">
 * Provides extension methods for the Type class.
 * </file>
 ******************************************************************************/
#endregion

using NerdyDuck.CodedExceptions;
using System;
using System.ComponentModel;
using System.Reflection;

namespace NerdyDuck.ParameterValidation
{
	/// <summary>
	/// Provides extension methods for the <see cref="Type"/> class.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class TypeExtensions
	{
		/// <summary>
		/// Returns the partially qualified name of the specified type, only containing the full type name, and the assembly name.
		/// </summary>
		/// <param name="type">The type to get the name of.</param>
		/// <returns>A string containing the full type name and the assembly name. Assembly version, culture and public key are omitted.</returns>
		public static string ToStringAssemblyNameOnly(this Type type)
		{
			if (type == null)
			{
				throw new CodedArgumentNullException(Errors.CreateHResult(0x01), nameof(type));
			}

			TypeInfo info = type.GetTypeInfo();
			AssemblyName an = info.Assembly.GetName();

			return info.FullName + ", " + an.Name;
		}
	}
}
