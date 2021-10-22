#region Copyright
/*******************************************************************************
 * NerdyDuck.Collections - Validation and serialization of parameter values
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

using System.ComponentModel;
using System.Reflection;

namespace NerdyDuck.ParameterValidation;

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
			throw new CodedArgumentNullException(HResult.Create(ErrorCodes.TypeExtensions_ToStringAssemblyNameOnly_ArgNull), nameof(type));
		}

		TypeInfo info = type.GetTypeInfo();
		AssemblyName an = info.Assembly.GetName();

		return info.FullName + ", " + an.Name;
	}
}
