using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MathNet.Spatial.Serialization.Xml
{
    public class SpatialSerializationProvider : ISerializationSurrogateProvider
    {

        public object GetDeserializedObject(object obj, Type targetType)
        {
            if (SerializerFactory.SurrogateMap.Exists(t => t.Source == targetType))
                return SerializerFactory.SurrogateMap.Where(t => t.Source == targetType).First().ConvertToSource(obj);
            throw new SerializationException("Unable to Map " + nameof(targetType));
        }

        public object GetObjectToSerialize(object obj, Type targetType)
        {
            if (SerializerFactory.SurrogateMap.Exists(t => t.Surrogate == targetType))
                return SerializerFactory.SurrogateMap.Where(t => t.Surrogate == targetType).First().ConvertToSurrogate(obj);
            throw new SerializationException("Unable to Map " + nameof(targetType));
        }

        public Type GetSurrogateType(Type type)
        {
            return SerializerFactory.SurrogateMap.Where(t => t.Source == type).First().Surrogate;
        }

    }
}
