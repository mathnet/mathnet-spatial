using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace MathNet.Spatial.Serialization.Xml
{
    [DataContract(Name = "EulerAngles")]
    public class EulerAnglesSurrogate
    {
        [DataMember(Order = 1)]
        public Angle Alpha;
        [DataMember(Order = 2)]
        public Angle Beta;
        [DataMember(Order = 3)]
        public Angle Gamma;

        public static implicit operator EulerAnglesSurrogate(EulerAngles angles) => new EulerAnglesSurrogate { Alpha = angles.Alpha, Beta = angles.Beta, Gamma = angles.Gamma };
        public static implicit operator EulerAngles(EulerAnglesSurrogate angles) => new EulerAngles(angles.Alpha, angles.Beta, angles.Gamma);
    }

    internal class EulerAnglesSerializer : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            EulerAngles point = (EulerAngles)obj;
            info.AddValue("Alpha", point.Alpha);
            info.AddValue("Beta", point.Beta);
            info.AddValue("Gamma", point.Gamma);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Angle alpha = (Angle)info.GetValue("Alpha", typeof(Angle));
            Angle beta = (Angle)info.GetValue("Beta", typeof(Angle));
            Angle gamma = (Angle)info.GetValue("Gamma", typeof(Angle));
            return new EulerAngles(alpha, beta, gamma);
        }

    }
}
