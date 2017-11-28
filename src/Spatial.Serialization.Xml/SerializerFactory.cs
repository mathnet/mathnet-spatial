using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;
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
            new ContractConvertor(typeof(Line3D), typeof(Line3DSurrogate), new Line3DSerializer(), (o) => Line3DSerializer.TranslateToSurrogate((Line3D)o), (o) => Line3DSerializer.TranslateToSource((Line3DSurrogate)o) ),
            new ContractConvertor(typeof(Ray3D), typeof(Ray3DSurrogate), new Ray3DSerializer(), (o) => Ray3DSerializer.TranslateToSurrogate((Ray3D)o), (o) => Ray3DSerializer.TranslateToSource((Ray3DSurrogate)o) )
        };

        public static IConfigurationContainer CreateSpatialSerializer()
        {
            var serializer = new ConfigurationContainer().EnableAllConstructors().EnableParameterizedContent()
                
                .Type<Point2D>().CustomSerializer(new Point2DSerializer())
                .Type<Point3D>().CustomSerializer<Point3D>(new Point3DSerializer())
                .Type<Vector2D>().CustomSerializer(new Vector2DSerializer())
                .Type<Vector3D>().CustomSerializer(new Vector3DSerializer())
                .Type<UnitVector3D>().CustomSerializer(new UnitVector3DSerializer())
                .Type<Angle>().CustomSerializer(new AngleSerializer())
                .ConfigureType<Plane>()
                    .OnlyConfiguredProperties()
                    .Member(x => x.RootPoint)
                    .Member(x => x.Normal)                    
                .Type<Ray3D>()
                    .Member(x => x.ThroughPoint)
                    .Member(x => x.Direction)
                .Type<Line3D>()
                    .Member(x => x.StartPoint)
                    .Member(x => x.EndPoint)
                .Type<CoordinateSystem>()
                    .Member(x => x.Origin)
                    .Member(x => x.XAxis)
                    .Member(x => x.YAxis)
                    .Member(x => x.ZAxis);
            return serializer;
        }

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
