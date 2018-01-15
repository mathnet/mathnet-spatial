namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Euclidean;

    [DataContract(Name = "LineSegment3D")]
    [Serializable]
    public class LineSegment3DSurrogate
    {
        [DataMember(Order = 1)]
        public Point3D StartPoint;
        [DataMember(Order = 2)]
        public Point3D EndPoint;

        public static implicit operator LineSegment3DSurrogate(LineSegment3D line) => new LineSegment3DSurrogate { StartPoint = line.StartPoint, EndPoint = line.EndPoint };

        public static implicit operator LineSegment3D(LineSegment3DSurrogate line) => new LineSegment3D(line.StartPoint, line.EndPoint);
    }
}
