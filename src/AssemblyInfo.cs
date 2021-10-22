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

global using NerdyDuck.CodedExceptions;
global using System;
global using System.Collections.Generic;
global using System.Globalization;
global using System.Runtime.Serialization;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General information
[assembly: CLSCompliant(true)]
[assembly: ComVisible(true)]
[assembly: AssemblyTrademark("Covered by MIT License")]
[assembly: InternalsVisibleTo("NerdyDuck.Tests.ParameterValidation, PublicKey=002400000480000094000000060200000024000052534131000400000100010079440425b3de32da845d1b26a5ff0b688641b56e1725928ea0f2c4c3d7547418016ace0daad6fb91f47f4426e6d665e030284c000f5809b0cac932d052f850cb706f657fb6669eae2678da5ec0a4bd376f7f420e4016f8d0afde7d943fae633f629841d9f0dac46cc6137f35b129dc81b6f4720a80fb18e994b7b864802eced4")]
[assembly: AssemblyFacilityIdentifier(0x0002)]
