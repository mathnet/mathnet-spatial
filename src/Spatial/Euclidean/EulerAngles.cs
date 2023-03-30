using System;
using System.Diagnostics.Contracts;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Serialization;
using MathNet.Spatial.Internals;
using MathNet.Spatial.Units;
using HashCode = MathNet.Spatial.Internals.HashCode;
using System.Xml.Linq;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// A means of representing spatial orientation of any reference frame.
    /// More information can be found https://en.wikipedia.org/wiki/Euler_angles
    /// </summary>
    [Serializable]
    public struct EulerAngles : IEquatable<EulerAngles>, IXmlSerializable
    {
        /// <summary>
        /// Alpha (or phi) is the rotation around the z axis
        /// </summary>
        public Angle Alpha { get; private set; } // phi

        /// <summary>
        /// Beta (or theta) is the rotation around the N axis
        /// </summary>
        public Angle Beta { get; private set; } // theta

        /// <summary>
        /// Gamma (or psi) is the rotation around the Z axis
        /// </summary>
        public Angle Gamma { get; private set; } // psi

        /// <summary>
        /// Initializes a new instance of the <see cref="EulerAngles"/> struct.
        /// Constructs a EulerAngles from three provided angles
        /// </summary>
        /// <param name="alpha">The alpha angle is the rotation around the z axis</param>
        /// <param name="beta">The beta angle is the rotation around the N axis</param>
        /// <param name="gamma">The gamma angle is the rotation around the Z axis</param>
        public EulerAngles(Angle alpha, Angle beta, Angle gamma)
        {
            Alpha = alpha;
            Beta = beta;
            Gamma = gamma;
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
            return double.IsNaN(Alpha.Radians)
                   && double.IsNaN(Beta.Radians)
                   && double.IsNaN(Gamma.Radians);
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

            return Alpha.Equals(other.Alpha, tolerance)
                   && Beta.Equals(other.Beta, tolerance)
                   && Gamma.Equals(other.Gamma, tolerance);
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
            return Alpha.Equals(other.Alpha, tolerance)
                   && Beta.Equals(other.Beta, tolerance)
                   && Gamma.Equals(other.Gamma, tolerance);
        }

        /// <inheritdoc/>
        [Pure]
        public bool Equals(EulerAngles other)
        {
            return Alpha.Equals(other.Alpha)
                   && Beta.Equals(other.Beta)
                   && Gamma.Equals(other.Gamma);
        }

        /// <inheritdoc/>
        [Pure]
        public override bool Equals(object obj) => obj is EulerAngles angles && Equals(angles);

        /// <inheritdoc/>
        [Pure]
        public override int GetHashCode() => HashCode.Combine(Alpha, Beta, Gamma);

        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema() => null;

        /// <inheritdoc/>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var e = (XElement)XNode.ReadFrom(reader);
            this = new EulerAngles(
                Angle.ReadFrom(e.SingleElement("Alpha").CreateReader()),
                Angle.ReadFrom(e.SingleElement("Beta").CreateReader()),
                Angle.ReadFrom(e.SingleElement("Gamma").CreateReader()));
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteElement("Alpha", Alpha);
            writer.WriteElement("Beta", Beta);
            writer.WriteElement("Gamma", Gamma);
        }
    }
}
