namespace MathNet.Spatial.Tests
{
    using System.Reflection;
    using NUnitLite;

    public class Program
    {
        /// <summary>
        /// The main program executes the tests. Output may be routed to
        /// various locations, depending on the arguments passed.
        /// </summary>
        /// <remarks>Run with --help for a full list of arguments supported</remarks>
        /// <param name="args">Arguments</param>
        /// <returns>0 if successful, otherwise an error code</returns>
        public static int Main(string[] args)
        {
            return new AutoRun(typeof(Program).GetTypeInfo().Assembly).Execute(args);
        }
    }
}
