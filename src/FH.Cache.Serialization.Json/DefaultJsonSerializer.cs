﻿using FH.Cache.Core.Internal;
using FH.Cache.Core.Serialization;
using FH.Cache.Serialization.Json.Configurations;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace FH.Cache.Serialization.Json
{
    /// <summary>
    /// Default json serializer.
    /// </summary>
    public class DefaultJsonSerializer : ICachingSerializer
    {
        /// <summary>
        /// The json serializer.
        /// </summary>
        private readonly JsonSerializer jsonSerializer;

        /// <summary>
        /// The name.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EasyCaching.Serialization.Json.DefaultJsonSerializer"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="options">Options.</param>
        public DefaultJsonSerializer(string name, CachingJsonSerializerOptions options)
        {
            _name = name;
            jsonSerializer = new JsonSerializer
            {
                ReferenceLoopHandling = options.ReferenceLoopHandling,
                TypeNameHandling = options.TypeNameHandling,
                MetadataPropertyHandling = options.MetadataPropertyHandling,
                MissingMemberHandling = options.MissingMemberHandling,
                NullValueHandling = options.NullValueHandling,
                DefaultValueHandling = options.DefaultValueHandling,
                ObjectCreationHandling = options.ObjectCreationHandling,
                PreserveReferencesHandling = options.PreserveReferencesHandling,
                ConstructorHandling = options.ConstructorHandling
            };
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name => _name;

        /// <summary>
        /// Deserialize the specified bytes.
        /// </summary>
        /// <returns>The deserialize.</returns>
        /// <param name="bytes">Bytes.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public T Deserialize<T>(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            using (var sr = new StreamReader(ms, Encoding.UTF8))
            using (var jtr = new JsonTextReader(sr))
            {
                return jsonSerializer.Deserialize<T>(jtr);
            }
        }

        /// <summary>
        /// Deserialize the specified bytes.
        /// </summary>
        /// <returns>The deserialize.</returns>
        /// <param name="bytes">Bytes.</param>
        /// <param name="type">Type.</param>
        public object Deserialize(byte[] bytes, Type type)
        {
            using (var ms = new MemoryStream(bytes))
            using (var sr = new StreamReader(ms, Encoding.UTF8))
            using (var jtr = new JsonTextReader(sr))
            {
                return jsonSerializer.Deserialize(jtr, type);
            }
        }

        /// <summary>
        /// Serialize the specified value.
        /// </summary>
        /// <returns>The serialize.</returns>
        /// <param name="value">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public byte[] Serialize<T>(T value)
        {
            using (var ms = new MemoryStream())
            {
                using (var sr = new StreamWriter(ms, Encoding.UTF8))
                using (var jtr = new JsonTextWriter(sr))
                {
                    jsonSerializer.Serialize(jtr, value);
                }
                return ms.ToArray();
            }
        }
        #region Mainly For Memcached  
        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="obj">Object.</param>
        public ArraySegment<byte> SerializeObject(object obj)
        {
            var typeName = TypeHelper.BuildTypeName(obj.GetType()); // Get type 

            using (var ms = new MemoryStream())
            using (var tw = new StreamWriter(ms))
            using (var jw = new JsonTextWriter(tw))
            {
                jw.WriteStartArray(); // [
                jw.WriteValue(typeName); // "type",
                jsonSerializer.Serialize(jw, obj); // obj

                jw.WriteEndArray(); // ]

                jw.Flush();

                return new ArraySegment<byte>(ms.ToArray(), 0, (int)ms.Length);
            }
        }

        /// <summary>
        /// Deserializes the object.
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="value">Value.</param>
        public object DeserializeObject(ArraySegment<byte> value)
        {
            using (var ms = new MemoryStream(value.Array, value.Offset, value.Count, writable: false))
            using (var tr = new StreamReader(ms))
            using (var jr = new JsonTextReader(tr))
            {
                jr.Read();
                if (jr.TokenType == JsonToken.StartArray)
                {
                    // read type
                    var typeName = jr.ReadAsString();
                    var type = Type.GetType(typeName, throwOnError: true);// Get type

                    // read object
                    jr.Read();
                    return jsonSerializer.Deserialize(jr, type);
                }
                else
                {
                    throw new InvalidDataException("JsonTranscoder only supports [\"TypeName\", object]");
                }
            }
        }

        #endregion
    }
}
