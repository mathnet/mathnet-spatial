### 0.6.1 - 2023-03-28
* Built with .Net 5.0.200
* Build: fix archive publishing of strong name packages *~cdrnet* 
* Bug fix: Vector2D.SignedAngleTo *~jkalias*
* Bug fix: Fix order of YPR (yaw-pitch-roll) transformation *~jakehedlund*
* Increase PolyLine performance when indexing Vertices *~bradtglass*
* Bug fix: Fix Line2D.TryIntersect to pass nullable Point2D *~f-frhs*
* Bug fix: Vector3DHomogeneous.ToVector3D() and Point3DHomogeneous.ToVector3D() *~osbordh*

### 0.6.0 - 2020-05-09
* BUG: Circle2D.FromPoints giving incorrect results for special points *~Jong Hyun Kim*
* Number formatting consistent between .NET Core and Framework
* Update dependencies, built with .NET Core SDK 3.1.1

### 0.5.0 - 2019-06-02
* Many changes on API and implementation *~Johan Larsson, Adam Jones*
* Breaking: Support for .Net Standard 2.0 or newer and .Net Framework 4.6.1 or newer
* Update to Math.NET Numerics v4.7
* New Types: Circle2D, LineSegment2D, LineSegment3D
* Improved implemention of Polygon2D.GetConvexHullFromPoints
* Angle now supports sexagesimal format
* Polygon2D now offers edges iterator
* Breaking: Line2D/3D obsolete in favor of LineSegement2D/3D
* Breaking: Polygon2D, PolyLine2D, Polyline3D enumerators dropped
* Breaking: Equality has default no tolerance with a tolerance overload for all types
* Breaking: UnitParser, UnitConverter, XmlExt, Parser, IUnit dropped
* Breaking: Constructors in multiple places replaced by factory methods
* Breaking: Parse methods throws FormatException, was ArgumentException
* Breaking: Make implementation of IXmlSerializable explicit, adds noise to the API.
* Breaking: use nobreaking space, \u00A0, in Angle.ToString().
* Breaking: require length to be 1 - 0.1 when parsing UnitVector3D
* Updated documentation

### 0.4.0 - 2017-05-01
* Build: extra *.Signed packages with strong named assemblies for legacy use cases
* Polygon: order points on the convex hull clockwise *~Per Kuijpers*

### 0.3.0 - 2016-11-05
* Various additions and fixes

### 0.2.0-alpha - 2015-04-20
* Coordinate System: Rotation overloads
* Coordinate System: OffsetBy
* Homogeneous Coordinates *~luisllamasbinaburo*
* Split into Euclidean and Projective namespaces
* BUG: fix Vector3D.ProjectOn *~Ralle*
* Angle: various improvements and fixes
* Compatibility: downgraded to .Net 4.0 (from .Net 4.5)

### 0.1.1-alpha - 2014-10-08
* Initial version
