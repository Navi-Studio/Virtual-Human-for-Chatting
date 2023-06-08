using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenCvSharp
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Point : IEquatable<Point>
    {
        /// <summary>
        /// 
        /// </summary>
        public int X;

        /// <summary>
        /// 
        /// </summary>
        public int Y;

        /// <summary>
        /// 
        /// </summary>
        public const int SizeOf = sizeof (int) + sizeof (int);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(double x, double y)
        {
            X = (int) x;
            Y = (int) y;
        }

        #region Cast

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static implicit operator Vec2i(Point point)
        {
            return new Vec2i(point.X, point.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static implicit operator Point(Vec2i vec)
        {
            return new Point(vec.Item0, vec.Item1);
        }

        #endregion

        #region Operators
        
        /// <summary>
        /// Specifies whether this object contains the same members as the specified Object.
        /// </summary>
        /// <param name="obj">The Object to test.</param>
        /// <returns>This method returns true if obj is the same type as this object and has the same members as this object.</returns>
        public bool Equals(Point obj)
        {
            return (this.X == obj.X && this.Y == obj.Y);
        }
        
        /// <summary>
        /// Compares two Point objects. The result specifies whether the values of the X and Y properties of the two Point objects are equal.
        /// </summary>
        /// <param name="lhs">A Point to compare.</param>
        /// <param name="rhs">A Point to compare.</param>
        /// <returns>This operator returns true if the X and Y values of left and right are equal; otherwise, false.</returns>
        public static bool operator ==(Point lhs, Point rhs)
        {
            return lhs.Equals(rhs);
        }
        /// <summary>
        /// Compares two Point objects. The result specifies whether the values of the X or Y properties of the two Point objects are unequal.
        /// </summary>
        /// <param name="lhs">A Point to compare.</param>
        /// <param name="rhs">A Point to compare.</param>
        /// <returns>This operator returns true if the values of either the X properties or the Y properties of left and right differ; otherwise, false.</returns>
        public static bool operator !=(Point lhs, Point rhs)
        {
            return !lhs.Equals(rhs);
        }
        
        /// <summary>
        /// Unary plus operator
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static Point operator + (Point pt)
        {
            return pt;
        }
        
        /// <summary>
        /// Unary minus operator
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static Point operator - (Point pt)
        {
            return new Point(-pt.X, -pt.Y);
        }
        
        /// <summary>
        /// Shifts point by a certain offset
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point operator + (Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }
        
        /// <summary>
        /// Shifts point by a certain offset
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Point operator - (Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }
        
        /// <summary>
        /// Multiplies point with scale factor
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Point operator * (Point pt, double scale)
        {
            return new Point((int) (pt.X * scale + 0.5), (int) (pt.Y * scale + 0.5));
        }

        /// <summary>
        /// Divides point with factor
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Point operator / (Point pt, double scale)
        {
            return new Point((int)(pt.X / scale + 0.5), (int)(pt.Y / scale + 0.5));
        }

        #endregion

        #region Override

        /// <summary>
        /// Specifies whether this object contains the same members as the specified Object.
        /// </summary>
        /// <param name="obj">The Object to test.</param>
        /// <returns>This method returns true if obj is the same type as this object and has the same members as this object.</returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        
        /// <summary>
        /// Returns a hash code for this object.
        /// </summary>
        /// <returns>An integer value that specifies a hash value for this object.</returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
        
        /// <summary>
        /// Converts this object to a human readable string.
        /// </summary>
        /// <returns>A string that represents this object.</returns>
        public override string ToString()
        {
            return string.Format("(x:{0} y:{1})", X, Y);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns point length ^ 2
        /// </summary>
        /// <returns></returns>
        public int LengthPow2()
        {
            return X * X + Y * Y;
        }

        /// <summary>
        /// Returns point (vector) geometrical length
        /// </summary>
        /// <returns></returns>
        public double Length()
        {
            return Math.Sqrt(LengthPow2());
        }

        /// <summary>
        /// Returns the distance^2 between the specified two points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static int DistancePow2(Point p1, Point p2)
        {
            return (p1 - p2).LengthPow2();
        }

        /// <summary>
        /// Returns the distance between the specified two points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(DistancePow2(p1, p2));
        }

        /// <summary>
        /// Returns the distance between the specified two points
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double DistanceTo(Point p)
        {
            return Distance(this, p);
        }
        
        /// <summary>
        /// Calculates the dot product of two 2D vectors.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double DotProduct(Point p1, Point p2)
        {
            return p1.X*p2.X + p1.Y*p2.Y;
        }
        
        /// <summary>
        /// Calculates the dot product of two 2D vectors.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double DotProduct(Point p)
        {
            return DotProduct(this, p);
        }
        
        /// <summary>
        /// Calculates the cross product of two 2D vectors.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double CrossProduct(Point p1, Point p2)
        {
            return p1.X*p2.Y - p2.X*p1.Y;
        }
        
        /// <summary>
        /// Calculates the cross product of two 2D vectors.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public double CrossProduct(Point p)
        {
            return CrossProduct(this, p);
        }

        #endregion
    }
}