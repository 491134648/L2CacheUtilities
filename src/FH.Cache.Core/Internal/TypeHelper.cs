﻿using System;
using System.Text.RegularExpressions;

namespace FH.Cache.Core.Internal
{
    /// <summary>
    /// borrowed from https://github.com/neuecc/MemcachedTranscoder/blob/master/Common/TypeHelper.cs
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// The subtract full name regex.
        /// </summary>
        static readonly Regex SubtractFullNameRegex = new Regex(@", Version=\d+.\d+.\d+.\d+, Culture=\w+, PublicKeyToken=\w+", RegexOptions.Compiled);

        /// <summary>
        /// Builds the name of the type.
        /// </summary>     
        /// <returns>The type name.</returns>
        /// <param name="type">Type.</param>
        public static string BuildTypeName(Type type)
        {
            return SubtractFullNameRegex.Replace(type.AssemblyQualifiedName, "");
        }
    }
}
