### 0.5.0-beta05 - 2019-02-17
* Packaging fixes

### 0.5.0-beta04 - 2018-11-18
* Many changes on API and implementation *~Johan Larsson, Adam Jones*
* Support for .Net Standard 1.3 and 2.0 or newer and .Net Framework 4.0 or newer
* Update to Math.NET Numerics v4.7

### 0.5.0-beta03 - 2018-02-13
* Update to released version of Mathnet.Numerics 4.0.0
* Cleanup Paket config

### 0.5.0-beta02 - 2018-02-09
* Update to Mathnet.Numerics 4.0.0-beta06

### 0.5.0-beta01 - 2018-01-16
* New Types: Circle2D, LineSegment2D, LineSegment3D
* Improved implemention of Polygon2D.GetConvexHullFromPoints
* Angle now supports sexagesimal format
* Polygon2D now offers edges iterator
* Breaking: Line2D/3D obsolete in favor of LineSegement2D/3D
* Breaking: Polygon2D, PolyLine2D, Polyline3D enumerators are obsolete
* Breaking: Equality has default no tolerance with a tolerance overload for all types
* Breaking: UnitParser, UnitConverter, XmlExt, Parser, IUnit all made obsolete
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
