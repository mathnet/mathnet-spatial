namespace MathNet.Spatial.Euclidean
{
    using System;
    using System.Diagnostics.Contracts;
    using MathNet.Numerics;
    using MathNet.Spatial.Internals;
    using MathNet.Spatial.Units;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="EulerAngles"/> struct.
        /// Constructs a EulerAngles from three provided angles
        /// </summary>
        /// <param name="alpha">The alpha angle is the rotation around the z axis</param>
        /// <param name="beta">The beta angle is the rotation around the N axis</param>
        /// <param name="gamma">The gamma angle is the rotation around the Z axis</param>
        public EulerAngles(Angle alpha, Angle beta, Angle gamma)
        {
            this.Alpha = alpha;
            this.Beta = beta;
            this.Gamma = gamma;
        }

        /// <summary>
        /// Returns a value that indicates whether each pair of elements in two specified EulerAngles is equal.
        /// </summary>
        /// <param name="left">The first EularAngle to compare</param>
        /// <param name="right">The second EularAngle to compare</param>
        /// <returns>True if the EulerAngles are the same; otherwise false.</returns>
        public static bool operator ==(EulerAngles left, EulerAngles right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Returns a value that indicates whether any pair of elements in two specified EulerAngles is not equal.
        /// </summary>
        /// <param name="left">The first EularAngle to compare</param>
        /// <param name="right">The second EularAngle to compare</param>
        /// <returns>True if the EulerAngles are different; otherwise false.</returns>
        public static bool operator !=(EulerAngles left, EulerAngles right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Checks if the EulerAngles are empty
        /// </summary>
        /// <returns>true if the angles have not been set</returns>
        [Pure]
        public bool IsEmpty()
        {
            return double.IsNaN(this.Alpha.Radians) && double.IsNaN(this.Beta.Radians) && double.IsNaN(this.Gamma.Radians);
        }

        /// <summary>
        /// Returns a value to indicate if this EulerAngles is equivalent to a given EulerAngles
        /// </summary>
        /// <param name="other">The EulerAngles to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>true if the EulerAngles are equal; otherwise false</returns>
        [Pure]
        public bool Equals(EulerAngles other, double tolerance)
        {
            if (tolerance < 0)
            {
                throw new ArgumentException("epsilon < 0");
            }

            return this.Alpha.Equals(other.Alpha, tolerance) &&
                   this.Beta.Equals(other.Beta, tolerance) &&
                   this.Gamma.Equals(other.Gamma, tolerance);
        }

        /// <summary>
        /// Returns a value to indicate if this EulerAngles is equivalent to a given EulerAngles
        /// </summary>
        /// <param name="other">The EulerAngles to compare against.</param>
        /// <param name="tolerance">A tolerance (epsilon) to adjust for floating point error</param>
        /// <returns>true if the EulerAngles are equal; otherwise false</returns>
        [Pure]
        public bool Equals(EulerAngles other, Angle tolerance)
        {
            return this.Alpha.Equals(other.Alpha, tolerance) &&
                   this.Beta.Equals(other.Beta, tolerance) &&
                   this.Gamma.Equals(other.Gamma, tolerance);
        }

        /// <inheritdoc/>
        [Pure]
        public bool Equals(EulerAngles other)
        {
            return this.Alpha.Equals(other.Alpha) && this.Beta.Equals(other.Beta) && this.Gamma.Equals(other.Gamma);
        }

        /// <inheritdoc/>
        [Pure]
        public override bool Equals(object obj) => obj is EulerAngles angles && this.Equals(angles);

        /// <inheritdoc/>
        [Pure]
        public override int GetHashCode() => HashCode.Combine(this.Alpha, this.Beta, this.Gamma);
    }
}
