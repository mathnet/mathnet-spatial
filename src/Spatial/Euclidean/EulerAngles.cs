using System;
using MathNet.Numerics;
using MathNet.Spatial.Units;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// A means of representing spatial orientation of any reference frame. 
    /// More information can be found https://en.wikipedia.org/wiki/Euler_angles
    /// </summary>
    public struct EulerAngles : IEquatable<EulerAngles>
    {
        /// <summary>
        /// Alpha (or phi) is the rotation around the z axis
        /// </summary>
        public readonly Angle Alpha; // phi
        /// <summary>
        /// Beta (or theta) is the rotation around the N axis
        /// </summary>
        public readonly Angle Beta; // theta
        /// <summary>
        /// Gamma (or psi) is the rotation around the Z axis 
        /// </summary>
        public readonly Angle Gamma; // psi

        public EulerAngles(Angle alpha, Angle beta, Angle gamma)
        {
            Alpha = alpha;
            Beta = beta;
            Gamma = gamma;
        }

        public bool IsEmpty()
        {
            return double.IsNaN(Alpha.Radians) && double.IsNaN(Beta.Radians) && double.IsNaN(Gamma.Radians);
        }

        public bool Equals(EulerAngles other)
        {
            const double defaultAbsoluteError = 0.000001;
            return Alpha.Radians.AlmostEqual(other.Alpha.Radians, defaultAbsoluteError) &&
                   Beta.Radians.AlmostEqual(other.Beta.Radians, defaultAbsoluteError) &&
                   Gamma.Radians.AlmostEqual(other.Gamma.Radians, defaultAbsoluteError);
        }
    }
}
