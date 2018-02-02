namespace MathNet.Spatial.UnitTests
{

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Serialization;
#if NETCOREAPP2_0 == true
    using ExtendedXmlSerializer.Configuration;
    using ExtendedXmlSerializer.ExtensionModel;
    using ExtendedXmlSerializer.ExtensionModel.Xml;
    using MathNet.Spatial.Serialization;
#endif
    using NUnit.Framework;

    public static class AssertXml
    {
        public static XmlWriterSettings Settings
        {
            get
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineHandling = NewLineHandling.Entitize,
                    OmitXmlDeclaration = true,
                    ////NamespaceHandling = NamespaceHandling.Default
                };
                return settings;
            }
        }

        public static void AreEqual(string first, string other)
        {
            var x1 = CleanupXml(first);
            var x2 = CleanupXml(other);
            Assert.AreEqual(x1, x2);
        }


        /// <summary>
        /// Serializes using XmlSerializer & DataContractSerializer
        /// Compares the generated xml
        /// Then asserts that the deserialized is the same as input (item)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="expectedXml"></param>
        /// <param name="assert"></param>
        public static void XmlRoundTrips<T>(T item, string expectedXml, Action<T, T> assert)
        {
            var roundtrips = new[]
            {
                XmlSerializerRoundTrip(item, expectedXml)
            };
            foreach (var roundtrip in roundtrips)
            {
                assert(item, roundtrip);
            }
        }

        public static T XmlSerializerRoundTrip<T>(T item, string expected)
        {
#if NETCOREAPP2_0 == true
            var serializer = new ConfigurationContainer().Register(new SpatialSerializationSurrogateProvider())
                                                         .Create();
#else
            var serializer = new XmlSerializer(item.GetType());
#endif
            string xml;

            using (var sw = new StringWriter())
            using (var writer = XmlWriter.Create(sw, Settings))
            {
#if NETCOREAPP2_0 == true
                xml = serializer.Serialize(item);
#else
                serializer.Serialize(writer, item);
                xml = sw.ToString();
#endif
                Debug.WriteLine("XmlSerializer");
                Debug.Write(xml);
                Debug.WriteLine(string.Empty);
                AreEqual(expected, xml);
            }

            using (var reader = new StringReader(xml))
            {
#if NETCOREAPP2_0 == true
                return serializer.Deserialize<T>(reader);
#else
                return (T)serializer.Deserialize(reader);
#endif
            }
        }


        private static string Normalize(XElement e)
        {
            using (var sw = new StringWriter())
            using (var writer = XmlWriter.Create(sw, Settings))
            {
                e.WriteTo(writer);
                writer.Flush();
                return sw.ToString();
            }
        }

        private static string CleanupXml(string xml)
        {
            var e = XElement.Parse(xml);
            var clean = RemoveAllNamespaces(e);
            return Normalize(clean);
        }

        /// <summary>
        /// Core recursion function
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static XElement RemoveAllNamespaces(XElement e)
        {
            var ne = new XElement(e.Name.LocalName, e.HasElements ? null : e.Value);
            ne.Add(e.Attributes().Where(a => !a.IsNamespaceDeclaration));
            ne.Add(e.Elements().Select(RemoveAllNamespaces));
            return ne;
        }

        public class Container<T>
        {
            public T Value1 { get; set; }

            public T Value2 { get; set; }
        }
    }
}
