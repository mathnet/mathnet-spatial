using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Gu.Roslyn.Asserts;
using MathNet.Spatial.Euclidean;
using NUnit.Framework;

[assembly: AssemblyTitle("Math.NET Spatial Unit Tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Math.NET Project")]
[assembly: AssemblyProduct("Math.NET Spatial")]
[assembly: AssemblyCopyright("Copyright (c) Math.NET Project")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: Guid("59cf0a44-16b3-4451-b4aa-ce5d25433139")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: MetadataReference(typeof(object), new[] { "global", "mscorlib" })]
[assembly: MetadataReference(typeof(Debug), new[] { "global", "System" })]
[assembly: MetadataReferences(
    typeof(UnitVector3D),
    typeof(Enumerable),
    typeof(WebClient),
    typeof(Bitmap),
    typeof(DbConnection),
    typeof(XmlSerializer),
    typeof(DataContractSerializer),
    typeof(Assert))]
