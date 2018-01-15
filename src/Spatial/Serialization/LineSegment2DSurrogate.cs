namespace MathNet.Spatial.Serialization
{
#pragma warning disable SA1600
    using System;
    using System.Runtime.Serialization;
    using MathNet.Spatial.Euclidean;

    [DataContract(Name = "LineSegment2D")]
    [Serializable]
    public class LineSegment2DSurrogate
    {
        [DataMember(Order = 1)]
        public Point2D StartPoint;
        [DataMember(Order = 2)]
        public Point2D EndPoint;

        public static implicit operator LineSegment2DSurrogate(LineSegment2D line) => new LineSegment2DSurrogate { StartPoint = line.StartPoint, EndPoint = line.EndPoint };

        public static implicit operator LineSegment2D(LineSegment2DSurrogate line) => new LineSegment2D(line.StartPoint, line.EndPoint);
    }
}
