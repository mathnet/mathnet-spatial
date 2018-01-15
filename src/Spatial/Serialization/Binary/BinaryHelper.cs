namespace MathNet.Spatial.Serialization.Binary
{
#if NETSTANDARD1_3 == false
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides static methods for supporting binary serialization
    /// </summary>
    public static class BinaryHelper
    {
        /// <summary>
        /// Creates a surrogate selector with all the spatial types and thier surrogates
        /// </summary>
        /// <returns>A surrogate selector</returns>
        public static SurrogateSelector CreateSurrogateSelector()
        {
            SurrogateSelector s = new SurrogateSelector();
            for (int i = 0; i < SpatialSerialization.SurrogateMap.Count; i++)
            {
                s.AddSurrogate(SpatialSerialization.SurrogateMap[i].Source, new StreamingContext(StreamingContextStates.All), new SpatialBinarySerializer());
            }

            return s;
        }
    }
#endif
}
