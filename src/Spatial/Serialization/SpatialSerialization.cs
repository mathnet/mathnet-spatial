namespace MathNet.Spatial.Serialization
{
#if NETSTANDARD1_3 == false
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Euclidean;
    using MathNet.Spatial.Units;

    /// <summary>
    /// Provides serialization helper methods for Spatial types.
    /// </summary>
    public static class SpatialSerialization
    {
        /// <summary>
        /// Map of types
        /// </summary>
        internal static List<ContractConvertor> SurrogateMap = new List<ContractConvertor>()
        {
            new ContractConvertor(typeof(Point2D), typeof(Point2DSurrogate)),
            new ContractConvertor(typeof(Point3D), typeof(Point3DSurrogate)),
            new ContractConvertor(typeof(Vector2D), typeof(Vector2DSurrogate)),
            new ContractConvertor(typeof(Vector3D), typeof(Vector3DSurrogate)),
            new ContractConvertor(typeof(UnitVector3D), typeof(UnitVector3DSurrogate)),
            new ContractConvertor(typeof(Angle), typeof(AngleSurrogate)),
            new ContractConvertor(typeof(EulerAngles), typeof(EulerAnglesSurrogate)),
            new ContractConvertor(typeof(LineSegment2D), typeof(LineSegment2DSurrogate)),
            new ContractConvertor(typeof(LineSegment3D), typeof(LineSegment3DSurrogate)),
            new ContractConvertor(typeof(Quaternion), typeof(QuaternionSurrogate)),
            new ContractConvertor(typeof(Circle2D), typeof(Circle2DSurrogate)),
            new ContractConvertor(typeof(Circle3D), typeof(Circle3DSurrogate)),
            new ContractConvertor(typeof(Polygon2D), typeof(Polygon2DSurrogate)),
            new ContractConvertor(typeof(PolyLine2D), typeof(PolyLine2DSurrogate)),
            new ContractConvertor(typeof(PolyLine3D), typeof(PolyLine3DSurrogate)),
            new ContractConvertor(typeof(Euclidean.CoordinateSystem), typeof(CoordinateSystemSurrogate)),
            new ContractConvertor(typeof(Ray3D), typeof(Ray3DSurrogate)),
            new ContractConvertor(typeof(Plane), typeof(PlaneSurrogate))
        };

        /// <summary>
        /// Provides a list of known spatial surrogates in type pairs
        /// </summary>
        /// <returns>a list of type pairs</returns>
        public static List<Tuple<Type, Type>> KnownSurrogates()
        {
            return SurrogateMap.Select(t => new Tuple<Type, Type>(t.Source, t.Surrogate)).ToList();
        }

        /// <summary>
        /// Gets a value which indicates if a surrogate exists for a given type.
        /// </summary>
        /// <param name="type">A type</param>
        /// <returns>True if there is a spatial surrogate for that type; otherwise false</returns>
        public static bool CanConvert(Type type)
        {
            for (int i = 0; i < SpatialSerialization.SurrogateMap.Count; i++)
            {
                if (SurrogateMap[i].Source == type)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the serialization surrogate for a given spatial type
        /// </summary>
        /// <param name="type">a type</param>
        /// <returns>A surrogate type</returns>
        public static Type GetSurrogateType(Type type)
        {
            if (SpatialSerialization.SurrogateMap.Exists(t => t.Source == type))
            {
                return SpatialSerialization.SurrogateMap.Where(t => t.Source == type).First().Surrogate;
            }
            else
            {
                return type;
            }
        }

        /// <summary>
        /// Converts a given spatial type and data to the equivelent surrgate type if a conversion exists
        /// </summary>
        /// <param name="obj">The object to convert</param>
        /// <returns>A surrogate object equivelent to the provided spatial object if a conversion exists; otherwise the object passed</returns>
        public static object GetObjectToSerialize(object obj)
        {
            Type type = obj.GetType();
            if (SpatialSerialization.SurrogateMap.Exists(t => t.Source == type))
            {
                var y = SpatialSerialization.SurrogateMap.Where(t => t.Source == type).First();
                var conversionmethod = y.Surrogate.GetMethod("op_Implicit", new[] { y.Source });
                return conversionmethod.Invoke(null, new[] { obj });
            }

            return obj;
        }

        /// <summary>
        /// Converts a surrogate type and data to an equivelent Spatial object
        /// </summary>
        /// <param name="obj">Surrogate data object</param>
        /// <returns>A Spatial object coresponding to the provided surrgate object if a conversion exists; otherwise the object passed</returns>
        public static object GetDeserializedObject(object obj)
        {
            Type surrogateType = obj.GetType();
            if (SpatialSerialization.SurrogateMap.Exists(t => t.Surrogate == surrogateType))
            {
                var y = SpatialSerialization.SurrogateMap.Where(t => t.Surrogate == surrogateType).First();
                var conversionmethod = y.Surrogate.GetMethod("op_Implicit", new[] { y.Surrogate });
                return conversionmethod.Invoke(null, new[] { obj });
            }

            return obj;
        }

        /// <summary>
        /// Internal class for storing the map from source type to surrogate
        /// </summary>
        internal class ContractConvertor
        {
            /// <summary>
            /// A spatial library type
            /// </summary>
            public Type Source;

            /// <summary>
            /// A surrogate type
            /// </summary>
            public Type Surrogate;

            /// <summary>
            /// Initializes a new instance of the <see cref="ContractConvertor"/> class.
            /// </summary>
            /// <param name="source">A spatial library type</param>
            /// <param name="surrogate">A surrogate type</param>
            public ContractConvertor(Type source, Type surrogate)
            {
                this.Source = source;
                this.Surrogate = surrogate;
            }
        }
    }
#endif
}
