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
        [DataMember]
        public Angle Alpha;
        [DataMember]
        public Angle Beta;
        [DataMember]
        public Angle Gamma;
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

        public static EulerAnglesSurrogate TranslateToSurrogate(EulerAngles source)
        {
            return new EulerAnglesSurrogate { Alpha = source.Alpha, Beta = source.Beta, Gamma = source.Gamma };
        }

        public static EulerAngles TranslateToSource(EulerAnglesSurrogate surrogate)
        {
            return new EulerAngles(surrogate.Alpha, surrogate.Beta, surrogate.Gamma);
        }
    }
}
