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
            public Func<object, object> ConvertToSurrogate;
            public Func<Object, object> ConvertToSource;

            public ContractConvertor(Type source, Type surrogate, ISerializationSurrogate serializer, Func<object, object> convertToSurrogate, Func<object, object> convertToSource)
            {
                Source = source;
                Surrogate = surrogate;
                Serializer = serializer;
                ConvertToSurrogate = convertToSurrogate;
                ConvertToSource = convertToSource;
            }
        }

        internal static List<ContractConvertor> SurrogateMap = new List<ContractConvertor>()
        {
            new ContractConvertor(typeof(Point2D), typeof(Point2DSurrogate), new Point2DSerializer(), (o) => Point2DSerializer.TranslateToSurrogate((Point2D)o), (o) => Point2DSerializer.TranslateToSource((Point2DSurrogate)o) ),
            new ContractConvertor(typeof(Point3D), typeof(Point3DSurrogate), new Point3DSerializer(), (o) => Point3DSerializer.TranslateToSurrogate((Point3D)o), (o) => Point3DSerializer.TranslateToSource((Point3DSurrogate)o) ),
            new ContractConvertor(typeof(Vector2D), typeof(Vector2DSurrogate), new Vector2DSerializer(), (o) => Vector2DSerializer.TranslateToSurrogate((Vector2D)o), (o) => Vector2DSerializer.TranslateToSource((Vector2DSurrogate)o) ),
            new ContractConvertor(typeof(Vector3D), typeof(Vector3DSurrogate), new Vector3DSerializer(), (o) => Vector3DSerializer.TranslateToSurrogate((Vector3D)o), (o) => Vector3DSerializer.TranslateToSource((Vector3DSurrogate)o) ),
            new ContractConvertor(typeof(UnitVector3D), typeof(UnitVector3DSurrogate), new UnitVector3DSerializer(), (o) => UnitVector3DSerializer.TranslateToSurrogate((UnitVector3D)o), (o) => UnitVector3DSerializer.TranslateToSource((UnitVector3DSurrogate)o) ),
            new ContractConvertor(typeof(Angle), typeof(AngleSurrogate), new AngleSerializer(), (o) => AngleSerializer.TranslateToSurrogate((Angle)o), (o) => AngleSerializer.TranslateToSource((AngleSurrogate)o) ),
            new ContractConvertor(typeof(EulerAngles), typeof(EulerAnglesSurrogate), new EulerAnglesSerializer(), (o) => EulerAnglesSerializer.TranslateToSurrogate((EulerAngles)o), (o) => EulerAnglesSerializer.TranslateToSource((EulerAnglesSurrogate)o) ),
            new ContractConvertor(typeof(Line2D), typeof(Line2DSurrogate), new Line2DSerializer(), (o) => Line2DSerializer.TranslateToSurrogate((Line2D)o), (o) => Line2DSerializer.TranslateToSource((Line2DSurrogate)o) ),
            new ContractConvertor(typeof(Line3D), typeof(Line3DSurrogate), new Line3DSerializer(), (o) => Line3DSerializer.TranslateToSurrogate((Line3D)o), (o) => Line3DSerializer.TranslateToSource((Line3DSurrogate)o) ),
            new ContractConvertor(typeof(Quaternion), typeof(QuaternionSurrogate), new QuaternionSerializer(), (o) => QuaternionSerializer.TranslateToSurrogate((Quaternion)o), (o) => QuaternionSerializer.TranslateToSource((QuaternionSurrogate)o) ),
            new ContractConvertor(typeof(Circle3D), typeof(Circle3DSurrogate), new Circle3DSerializer(), (o) => Circle3DSerializer.TranslateToSurrogate((Circle3D)o), (o) => Circle3DSerializer.TranslateToSource((Circle3DSurrogate)o) ),
            new ContractConvertor(typeof(Polygon2D), typeof(Polygon2DSurrogate), new Polygon2DSerializer(), (o) => Polygon2DSerializer.TranslateToSurrogate((Polygon2D)o), (o) => Polygon2DSerializer.TranslateToSource((Polygon2DSurrogate)o) ),
            new ContractConvertor(typeof(PolyLine2D), typeof(PolyLine2DSurrogate), new PolyLine2DSerializer(), (o) => PolyLine2DSerializer.TranslateToSurrogate((PolyLine2D)o), (o) => PolyLine2DSerializer.TranslateToSource((PolyLine2DSurrogate)o) ),
            new ContractConvertor(typeof(PolyLine3D), typeof(PolyLine3DSurrogate), new PolyLine3DSerializer(), (o) => PolyLine3DSerializer.TranslateToSurrogate((PolyLine3D)o), (o) => PolyLine3DSerializer.TranslateToSource((PolyLine3DSurrogate)o) ),
            new ContractConvertor(typeof(Ray3D), typeof(Ray3DSurrogate), new Ray3DSerializer(), (o) => Ray3DSerializer.TranslateToSurrogate((Ray3D)o), (o) => Ray3DSerializer.TranslateToSource((Ray3DSurrogate)o) )
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

        public static DataContractSerializer CreateDataContractSerializer(object item)
        {
            var serializer = new DataContractSerializer(item.GetType());
            serializer.SetSerializationSurrogateProvider(new SpatialSerializationProvider());
            return serializer;
        }
    }
}
