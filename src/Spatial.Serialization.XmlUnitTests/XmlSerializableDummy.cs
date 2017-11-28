using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MathNet.Spatial.Serialization.Xml.UnitTests
{
    public class XmlSerializableDummy
    {
        private string _name;
        private XmlSerializableDummy()
        {

        }
        public XmlSerializableDummy(string name, int age)
        {
            Age = age;
            _name = name;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public int Age { get; set; }
    }
}
