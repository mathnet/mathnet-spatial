namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Euclidean;

    [DataContract(Name = "CoordinateSystem")]
    [Serializable]
    public class CoordinateSystemSurrogate
    {
        [DataMember(Order = 1)]
        public Point3D Origin;
        [DataMember(Order = 2)]
        public Vector3D XAxis;
        [DataMember(Order = 3)]
        public Vector3D YAxis;
        [DataMember(Order = 4)]
        public Vector3D ZAxis;

        public static implicit operator CoordinateSystemSurrogate(CoordinateSystem cs)
        {
            if (cs != null)
            {
                return new CoordinateSystemSurrogate { Origin = cs.Origin, XAxis = cs.XAxis, YAxis = cs.YAxis, ZAxis = cs.ZAxis };
            }

            return null;
        }

        public static implicit operator CoordinateSystem(CoordinateSystemSurrogate cs) => new CoordinateSystem(cs.Origin, cs.XAxis, cs.YAxis, cs.ZAxis);
    }
}
