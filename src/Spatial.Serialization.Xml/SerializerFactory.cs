using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.ExtensionModel.Content;
using ExtendedXmlSerializer.ExtensionModel.Types;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using MathNet.Spatial.Euclidean;
using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Spatial.Serialization.Xml
{
    public static class SerializerFactory
    {
        public static IConfigurationContainer CreateSpatialSerializer()
        {
            var serializer = new ConfigurationContainer().EnableAllConstructors().EnableParameterizedContent()
                .EnableImplicitTyping(typeof(Ray3D), typeof(Line3D))
                .Type<Point2D>().CustomSerializer(new Point2DSerializer())
                .Type<Point3D>().CustomSerializer(new Point3DSerializer())
                .Type<Vector2D>().CustomSerializer(new Vector2DSerializer())
                .Type<Vector3D>().CustomSerializer(new Vector3DSerializer())
                .Type<UnitVector3D>().CustomSerializer(new UnitVector3DSerializer())
                .Type<Ray3D>()
                    .Member(x => x.ThroughPoint)
                    .Member(x => x.Direction)
                .Type<Line3D>()
                    .Member(x => x.StartPoint)
                    .Member(x => x.EndPoint);
            return serializer;
        }
    }
}
