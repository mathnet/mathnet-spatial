namespace MathNet.Spatial.Serialization.Json
{
    using System;
    using MathNet.Spatial.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Provides support for json serialization of spatial types
    /// </summary>
    public class SpatialJsonConverter : JsonConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialJsonConverter"/> class.
        /// </summary>
        public SpatialJsonConverter()
        {
        }

        /// <inheritdoc />
        public override bool CanRead => true;

        /// <inheritdoc />
        public override bool CanWrite => true;

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type surrogateType = SpatialSerialization.GetSurrogateType(objectType);
            JToken token = JToken.Load(reader);
            object surrogateData = token.ToObject(surrogateType, serializer);
            return SpatialSerialization.GetDeserializedObject(surrogateData);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            object surrogate = SpatialSerialization.GetObjectToSerialize(value);
            JToken t = JToken.FromObject(surrogate, serializer);
            t.WriteTo(writer);
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return SpatialSerialization.CanConvert(objectType);
        }
    }
}
