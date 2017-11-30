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
        /// <summary>
        /// Converts a surrogate object into a known Spatial object.  If type is unknown the original object is simply returned
        /// </summary>
        public object GetDeserializedObject(object obj, Type targetType)
        {
            if (SerializerFactory.SurrogateMap.Exists(t => t.Source == targetType))
            {
                var y = SerializerFactory.SurrogateMap.Where(t => t.Source == targetType).First();
                var conversionmethod = y.Surrogate.GetMethod("op_Implicit", new[] { y.Surrogate });
                return conversionmethod.Invoke(null, new[] { obj });
            }
            return obj;
        }

        /// <summary>
        /// Converts a Spatial object into a surrogate for serialization
        /// </summary>
        public object GetObjectToSerialize(object obj, Type targetType)
        {
            if (SerializerFactory.SurrogateMap.Exists(t => t.Surrogate == targetType))
            {
                var y = SerializerFactory.SurrogateMap.Where(t => t.Surrogate == targetType).First();
                var conversionmethod = y.Surrogate.GetMethod("op_Implicit", new[] { y.Source });
                return conversionmethod.Invoke(null, new[] { obj });
            }
            return obj;
        }

        /// <summary>
        /// returns the surrogate type for a given type.  If type is unknown it returns the type provided
        /// </summary>
        public Type GetSurrogateType(Type type)
        {
            if (SerializerFactory.SurrogateMap.Exists(t => t.Source == type))
                return SerializerFactory.SurrogateMap.Where(t => t.Source == type).First().Surrogate;
            else
                return type;
        }

    }
}
