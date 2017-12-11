using System.Reflection;
using System.Runtime.InteropServices;
using Gu.Roslyn.Asserts;
using MathNet.Spatial.Euclidean;

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
[assembly: MetadataReference(typeof(System.Diagnostics.Debug), new[] { "global", "System" })]
[assembly: MetadataReferences(
    typeof(UnitVector3D),
    typeof(System.Linq.Enumerable),
    typeof(System.Net.WebClient),
    typeof(System.Drawing.Bitmap),
    typeof(System.Data.Common.DbConnection),
    typeof(System.Xml.Serialization.XmlSerializer),
    typeof(System.Runtime.Serialization.DataContractSerializer),
    typeof(NUnit.Framework.Assert))]
