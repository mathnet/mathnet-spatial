﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathNet.Spatial.Euclidean
{
    /// <summary>
    /// This interface represents a line in cartesian space
    /// </summary>
    public interface ILine2D : IEquatable<ILine2D>
    {
        /// <summary>
        /// Returns the points at which the line crosses on the x axis
        /// </summary>
        /// <param name="x">An optional x coordinate</param>
        /// <returns>A list of crossing points</returns>
        Point2D[] XIntercept(double x = 0);

        /// <summary>
        /// Returns the points at which the line crosses on the y axis.
        /// </summary>
        /// <param name="y">An optional y coordinate</param>
        /// <returns>A list of crossing points</returns>
        Point2D[] YIntercept(double y = 0);

        /// <summary>
        /// Determines if a given point is on the line
        /// </summary>
        /// <param name="point">The point to be tested</param>
        /// <returns>True if the point is on the line</returns>
        bool IsThrough(Point2D point);

        /// <summary>
        /// Determines if a given point is on the line
        /// </summary>
        /// <param name="x">The x coordinate of the point</param>
        /// <param name="y">The y coordinate of the point</param>
        /// <returns>True if the point is on the line</returns>
        bool IsThrough(double x, double y);

        /// <summary>
        /// Determines if a line is parallel to another line
        /// </summary>
        /// <param name="line">the line to be compared against</param>
        /// <returns>True if the lines are parallel</returns>
        bool IsParallel<T>(T line) where T : ILine2D;
    }
}