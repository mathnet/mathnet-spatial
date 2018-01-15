namespace MathNet.Spatial.Serialization.DataContracts
{
    using System;
#if NETSTANDARD2_0 == true
    using System.Runtime.Serialization;
    using MathNet.Spatial.Serialization;

    /// <summary>
    /// An implementation of ISerializationSurrogateProvider to support data contract serialization
    /// </summary>
    public class SpatialSerializationProvider : ISerializationSurrogateProvider
    {
        /// <summary>
        /// Converts a surrogate object into a known Spatial object.  If type is unknown the original object is simply returned
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <param name="targetType">The Type of the serialization target</param>
        /// <returns>A converted object if a convertor exists; otherwise the object passed</returns>
        public object GetDeserializedObject(object obj, Type targetType)
        {
            return SpatialSerialization.GetDeserializedObject(obj);
        }

        /// <summary>
        /// Converts a Spatial object into a surrogate for serialization
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <param name="targetType">The Type of the serialization target</param>
        /// <returns>A converted object if a convertor exists; otherwise the object passed</returns>
        public object GetObjectToSerialize(object obj, Type targetType)
        {
            return SpatialSerialization.GetObjectToSerialize(obj);
        }

        /// <summary>
        /// returns the surrogate type for a given type.  If type is unknown it returns the type provided
        /// </summary>
        /// <param name="type">A spatial library type</param>
        /// <returns>The type of a surrogate type</returns>
        public Type GetSurrogateType(Type type)
        {
            return SpatialSerialization.GetSurrogateType(type);
        }
    }
#endif
}
