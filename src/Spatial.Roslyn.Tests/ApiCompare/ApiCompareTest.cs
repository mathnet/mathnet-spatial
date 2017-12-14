namespace Spatial.Roslyn.Tests.ApiCompare
{
    using System.Diagnostics;
    using System.IO;
    using NUnit.Framework;

    public class ApiCompareTest
    {
        [Explicit("Script")]
        [Test]
        public void CompareAPIs()
        {
            Reflector reflect = new Reflector();
            if (!reflect.LoadAssemblies(
                @"D:\temp\ApiComp\mathnet.spatial.0.4.0\MathNet.Spatial.dll",
                @"D:\temp\ApiComp\mathnet-spatial.0.5.0-alpha\MathNet.Spatial.dll"))
            {
                Assert.Fail("Fatal error occured loading - Cannot Continue");
            }

            var output = reflect.Analyse();

            using (var file = File.CreateText(@"D:\temp\ApiComp\APIChange.md"))
            {
                foreach (var type in output)
                {
                    if (type.IsChanged)
                    {
                        file.WriteLine("For Type {0} {1}", type.OriginalType.Name, type.GetTransitionString());
                        foreach (var methodchange in type.MethodChanges)
                        {
                            file.WriteLine("  {0}", methodchange);
                        }
                    }
                }
            }

            Debug.WriteLine("API Classes Removed: " + reflect.RemovedTypes);
            Debug.WriteLine("API Classes Changed: " + reflect.ChangedTypes);
            Assert.Pass();
        }
    }
}
