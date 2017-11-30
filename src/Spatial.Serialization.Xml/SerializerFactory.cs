using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MathNet.Spatial.Serialization.Xml
{
    public static class SerializerFactory
    {
        internal class ContractConvertor
        {
            public Type Source;
            public Type Surrogate;
            public ISerializationSurrogate Serializer;

            public ContractConvertor(Type source, Type surrogate, ISerializationSurrogate serializer)
            {
                Source = source;
                Surrogate = surrogate;
                Serializer = serializer;
            }
        }

        internal static List<ContractConvertor> SurrogateMap = new List<ContractConvertor>()
        {
            new ContractConvertor(typeof(Point2D), typeof(Point2DSurrogate), new Point2DSerializer()),
            new ContractConvertor(typeof(Point3D), typeof(Point3DSurrogate), new Point3DSerializer()),
            new ContractConvertor(typeof(Vector2D), typeof(Vector2DSurrogate), new Vector2DSerializer()),
            new ContractConvertor(typeof(Vector3D), typeof(Vector3DSurrogate), new Vector3DSerializer()),
            new ContractConvertor(typeof(UnitVector3D), typeof(UnitVector3DSurrogate), new UnitVector3DSerializer()),
            new ContractConvertor(typeof(Angle), typeof(AngleSurrogate), new AngleSerializer()),
            new ContractConvertor(typeof(EulerAngles), typeof(EulerAnglesSurrogate), new EulerAnglesSerializer()),
            new ContractConvertor(typeof(Line2D), typeof(Line2DSurrogate), new Line2DSerializer()),
            new ContractConvertor(typeof(Line3D), typeof(Line3DSurrogate), new Line3DSerializer()),
            new ContractConvertor(typeof(Quaternion), typeof(QuaternionSurrogate), new QuaternionSerializer()),
            new ContractConvertor(typeof(Circle3D), typeof(Circle3DSurrogate), new Circle3DSerializer()),
            new ContractConvertor(typeof(Polygon2D), typeof(Polygon2DSurrogate), new Polygon2DSerializer()),
            new ContractConvertor(typeof(PolyLine2D), typeof(PolyLine2DSurrogate), new PolyLine2DSerializer()),
            new ContractConvertor(typeof(PolyLine3D), typeof(PolyLine3DSurrogate), new PolyLine3DSerializer()),
            new ContractConvertor(typeof(Ray3D), typeof(Ray3DSurrogate), new Ray3DSerializer()),
            new ContractConvertor(typeof(Plane), typeof(PlaneSurrogate), new PlaneSerializer())
        };

        public static SurrogateSelector CreateSurrogateSelector()
        {
            SurrogateSelector s = new SurrogateSelector();
            for (int i = 0; i < SerializerFactory.SurrogateMap.Count; i++)
            {
                s.AddSurrogate(SerializerFactory.SurrogateMap[i].Source, new StreamingContext(StreamingContextStates.All), SerializerFactory.SurrogateMap[i].Serializer);
            }
            return s;
        }

        public static List<Tuple<Type, Type>> KnownSurrogates()
        {
            return SurrogateMap.Select(t => new Tuple<Type, Type>(t.Source, t.Surrogate)).ToList();
        }

        public static DataContractSerializer CreateDataContractSerializer(object item)
        {
            var serializer = new DataContractSerializer(item.GetType());
            serializer.SetSerializationSurrogateProvider(new SpatialSerializationProvider());
            return serializer;
        }
    }
}
