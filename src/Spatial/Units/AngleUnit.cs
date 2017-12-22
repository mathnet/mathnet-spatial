﻿namespace MathNet.Spatial.Units
{
    using System;

    /// <summary>
    /// Utility class providing static units for angles
    /// </summary>
    [Obsolete("No longer required, use Angle methods directly; Obsolete since 2017-12-18")]
    public static class AngleUnit
    {
        /// <summary>
        /// A degree or degree of arc typically denoted by °.  It is defined such that a full rotation is 360 degrees.
        /// </summary>
        public static readonly Degrees Degrees = default(Degrees);

        /// <summary>
        /// The SI unit of angular measure is the Radian.
        /// </summary>
        public static readonly Radians Radians = default(Radians);
    }
}
