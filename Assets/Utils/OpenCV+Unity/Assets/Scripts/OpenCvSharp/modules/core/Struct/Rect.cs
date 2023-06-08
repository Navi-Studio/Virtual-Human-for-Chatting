using System;
using System.Runtime.InteropServices;

namespace OpenCvSharp
{
    /// <summary>
    /// Stores a set of four integers that represent the location and size of a rectangle
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect : IEquatable<Rect>
    {
        #region Field

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
        public int Width;

        /// <summary>
        /// 
        /// </summary>
        public int Height;

        /// <summary>
        /// sizeof(Rect)
        /// </summary>
        public const int SizeOf = sizeof (int)*4;
        
        /// <summary>
        /// Represents a Rect structure with its properties left uninitialized. 
        /// </summary>
        public static readonly Rect Empty = new Rect();

        #endregion

        /// <summary>
        /// Initializes a new instance of the Rectangle class with the specified location and size.
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the rectangle.</param>
        /// <param name="width">The width of the rectangle.</param>
        /// <param name="height">The height of the rectangle.</param>
        public Rect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the Rectangle class with the specified location and size.
        /// </summary>
        /// <param name="location">A Point that represents the upper-left corner of the rectangular region.</param>
        /// <param name="size">A Size that represents the width and height of the rectangular region.</param>
        public Rect(Point location, Size size)
        {
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
        }

        /// <summary>
        /// Creates a Rectangle structure with the specified edge locations.
        /// </summary>
        /// <param name="left">The x-coordinate of the upper-left corner of this Rectangle structure.</param>
        /// <param name="top">The y-coordinate of the upper-left corner of this Rectangle structure.</param>
        /// <param name="right">The x-coordinate of the lower-right corner of this Rectangle structure.</param>
        /// <param name="bottom">The y-coordinate of the lower-right corner of this Rectangle structure.</param>
        public static Rect FromLTRB(int left, int top, int right, int bottom)
        {
            Rect r = new Rect
            {
                X = left,
                Y = top,
                Width = right - left + 1,
                Height = bottom - top + 1
            };

            if (r.Width < 0)
                throw new ArgumentException("right > left");
            if (r.Height < 0)
                throw new ArgumentException("bottom > top");
            return r;
        }

        /// <summary>
        /// Creates a Rectangle as a bounding box for a set of points (i.e. minimum Rect containing all the points)
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Rect BoundingBoxForPoints(Point[] points)
        {
            int left = int.MaxValue;
            int right = int.MinValue;
            int top = int.MaxValue;
            int bottom = int.MinValue;

            foreach (Point pt in points)
            {
                left = Math.Min(left, pt.X);
                right = Math.Max(right, pt.X);
                top = Math.Min(top, pt.Y);
                bottom = Math.Max(bottom, pt.Y);
            }

            return FromLTRB(left, top, right, bottom);
        }

        #region Operators
        
        /// <summary>
        /// Specifies whether this object contains the same members as the specified Object.
        /// </summary>
        /// <param name="obj">The Object to test.</param>
        /// <returns>This method returns true if obj is the same type as this object and has the same members as this object.</returns>
        public bool Equals(Rect obj)
        {
            return (X == obj.X && Y == obj.Y && Width == obj.Width && Height == obj.Height);
        }
        
        /// <summary>
        /// Compares two Rect objects. The result specifies whether the members of each object are equal.
        /// </summary>
        /// <param name="lhs">A Point to compare.</param>
        /// <param name="rhs">A Point to compare.</param>
        /// <returns>This operator returns true if the members of left and right are equal; otherwise, false.</returns>
        public static bool operator ==(Rect lhs, Rect rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Compares two Rect objects. The result specifies whether the members of each object are unequal.
        /// </summary>
        /// <param name="lhs">A Point to compare.</param>
        /// <param name="rhs">A Point to compare.</param>
        /// <returns>This operator returns true if the members of left and right are unequal; otherwise, false.</returns>
        public static bool operator !=(Rect lhs, Rect rhs)
        {
            return !lhs.Equals(rhs);
        }
        
        /// <summary>
        /// Shifts rectangle by a certain offset
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static Rect operator + (Rect rect, Point pt)
        {
            return new Rect(rect.X + pt.X, rect.Y + pt.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Sums rectangles
        /// </summary>
        /// <param name="rect1"></param>
        /// <param name="rect2"></param>
        /// <returns></returns>
        public static Rect operator + (Rect rect1, Rect rect2)
        {
            return new Rect(rect1.X + rect2.X, rect1.Y + rect2.Y, rect1.Width + rect2.Width, rect1.Height + rect2.Height);
        }

        /// <summary>
        /// Shifts rectangle by a certain offset
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static Rect operator -(Rect rect, Point pt)
        {
            return new Rect(rect.X - pt.X, rect.Y - pt.Y, rect.Width, rect.Height);
        }
        
        /// <summary>
        /// Expands or shrinks rectangle by a certain amount
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Rect operator +(Rect rect, Size size)
        {
            return new Rect(rect.X, rect.Y, rect.Width + size.Width, rect.Height + size.Height);
        }
        
        /// <summary>
        /// Expands or shrinks rectangle by a certain amount
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Rect operator -(Rect rect, Size size)
        {
            return new Rect(rect.X, rect.Y, rect.Width - size.Width, rect.Height - size.Height);
        }
        
        /// <summary>
        /// Determines the Rect structure that represents the intersection of two rectangles. 
        /// </summary>
        /// <param name="a">A rectangle to intersect. </param>
        /// <param name="b">A rectangle to intersect. </param>
        /// <returns></returns>
        public static Rect operator &(Rect a, Rect b)
        {
            return Intersect(a, b);
        }
        
        /// <summary>
        /// Gets a Rect structure that contains the union of two Rect structures. 
        /// </summary>
        /// <param name="a">A rectangle to union. </param>
        /// <param name="b">A rectangle to union. </param>
        /// <returns></returns>
        public static Rect operator |(Rect a, Rect b)
        {
            return Union(a, b);
        }

        /// <summary>
        /// Multiplies Rect with given factor value
        /// </summary>
        /// <param name="a">A rectangle to scale</param>
        /// <param name="f">Scale factor</param>
        /// <returns></returns>
        public static Rect operator * (Rect a, double f)
        {
            return new Rect((int)(a.X * f + 0.5), (int)(a.Y * f + 0.5), (int)(a.Width * f + 0.5), (int)(a.Height * f + 0.5));
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets the y-coordinate of the top edge of this Rect structure. 
        /// </summary>
        public int Top
        {
            get { return Y; }
            set { Y = value; }
        }
        
        /// <summary>
        /// Gets the y-coordinate that is the sum of the Y and Height property values of this Rect structure.
        /// </summary>
        public int Bottom
        {
            get { return Y + Height - 1; }
        }
        
        /// <summary>
        /// Gets the x-coordinate of the left edge of this Rect structure. 
        /// </summary>
        public int Left
        {
            get { return X; }
            set { X = value; }
        }
        
        /// <summary>
        /// Gets the x-coordinate that is the sum of X and Width property values of this Rect structure. 
        /// </summary>
        public int Right
        {
            get { return X + Width - 1; }
        }
        
        /// <summary>
        /// Coordinate of the left-most rectangle corner [Point(X, Y)]
        /// </summary>
        public Point Location
        {
            get { return new Point(X, Y); }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        
        /// <summary>
        /// Size of the rectangle [CvSize(Width, Height)]
        /// </summary>
        public Size Size
        {
            get { return new Size(Width, Height); }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        /// Coordinate of the rectangle center
        /// </summary>
        public Point Center
        {
            get { return new Point(X + Width / 2, Y + Height / 2); }
        }

        /// <summary>
        /// Coordinate of the left-most rectangle corner [Point(X, Y)]
        /// </summary>
        public Point TopLeft
        {
            get { return new Point(X, Y); }
        }

        /// <summary>
        /// Coordinate of the right-top rectangle corner [Point(X + Width - 1, Y)]
        /// </summary>
        public Point TopRight
        {
            get { return new Point(X + Width - 1, Y); }
        }

        /// <summary>
        /// Coordinate of the right-most rectangle corner [Point(X + Width - 1, Y + Height - 1)]
        /// </summary>
        public Point BottomRight
        {
            get { return new Point(X + Width - 1, Y + Height - 1); }
        }

        /// <summary>
        /// Coordinate of the left-bottom rectangle corner [Point(X, Y + Height - 1)]
        /// </summary>
        public Point BottomLeft
        {
            get { return new Point(X, Y + Height - 1); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks whether rect exists
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return Width <= 0 || Height <= 0;
        }

        /// <summary>
        /// Determines if the specified point is contained within the rectangular region defined by this Rectangle. 
        /// </summary>
        /// <param name="x">x-coordinate of the point</param>
        /// <param name="y">y-coordinate of the point</param>
        /// <returns></returns>
        public bool Contains(int x, int y)
        {
            return (X <= x && Y <= y && X + Width - 1 > x && Y + Height - 1 > y);
        }
        
        /// <summary>
        /// Determines if the specified point is contained within the rectangular region defined by this Rectangle. 
        /// </summary>
        /// <param name="pt">point</param>
        /// <returns></returns>
        public bool Contains(Point pt)
        {
            return Contains(pt.X, pt.Y);
        }

        /// <summary>
        /// Determines if all specified points are contained within the rectangular region defined by this Rectangle. 
        /// </summary>
        /// <param name="pts">Points array</param>
        /// <returns></returns>
        public bool Contains(Point[] pts)
        {
            foreach (Point pt in pts)
                if (!Contains(pt))
                    return false;
            return true;
        }
        
        /// <summary>
        /// Determines if the specified rectangle is contained within the rectangular region defined by this Rectangle. 
        /// </summary>
        /// <param name="rect">rectangle</param>
        /// <returns></returns>
        public bool Contains(Rect rect)
        {
            return X <= rect.X &&
                   (rect.X + rect.Width) <= (X + Width) &&
                   Y <= rect.Y &&
                   (rect.Y + rect.Height) <= (Y + Height);
        }
        
        /// <summary>
        /// Inflates this Rect by the specified amount. 
        /// </summary>
        /// <param name="width">The amount to inflate this Rectangle horizontally. </param>
        /// <param name="height">The amount to inflate this Rectangle vertically. </param>
        public void Inflate(int width, int height)
        {
            X -= width;
            Y -= height;
            Width += (2*width);
            Height += (2*height);
        }
        
        /// <summary>
        /// Inflates this Rect by the specified amount. 
        /// </summary>
        /// <param name="size">The amount to inflate this rectangle. </param>
        public void Inflate(Size size)
        {

            Inflate(size.Width, size.Height);
        }
        
        /// <summary>
        /// Creates and returns an inflated copy of the specified Rect structure.
        /// </summary>
        /// <param name="rect">The Rectangle with which to start. This rectangle is not modified. </param>
        /// <param name="x">The amount to inflate this Rectangle horizontally. </param>
        /// <param name="y">The amount to inflate this Rectangle vertically. </param>
        /// <returns></returns>
        public static Rect Inflate(Rect rect, int x, int y)
        {
            rect.Inflate(x, y);
            return rect;
        }
        
        /// <summary>
        /// Determines the Rect structure that represents the intersection of two rectangles. 
        /// </summary>
        /// <param name="a">A rectangle to intersect. </param>
        /// <param name="b">A rectangle to intersect. </param>
        /// <returns></returns>
        public static Rect Intersect(Rect a, Rect b)
        {
            int x1 = Math.Max(a.X, b.X);
            int x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            int y1 = Math.Max(a.Y, b.Y);
            int y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1 && y2 >= y1)
                return new Rect(x1, y1, x2 - x1, y2 - y1);
            return Empty;
        }
        
        /// <summary>
        /// Determines the Rect structure that represents the intersection of two rectangles. 
        /// </summary>
        /// <param name="rect">A rectangle to intersect. </param>
        /// <returns></returns>
        public Rect Intersect(Rect rect)
        {
            return Intersect(this, rect);
        }
        
        /// <summary>
        /// Determines if this rectangle intersects with rect. 
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <returns></returns>
        public bool IntersectsWith(Rect rect)
        {
            return (
                (X < rect.X + rect.Width) &&
                (X + Width > rect.X) &&
                (Y < rect.Y + rect.Height) &&
                (Y + Height > rect.Y)
                );
        }
        
        /// <summary>
        /// Gets a Rect structure that contains the union of two Rect structures. 
        /// </summary>
        /// <param name="rect">A rectangle to union. </param>
        /// <returns></returns>
        public Rect Union(Rect rect)
        {
            return Union(this, rect);
        }
        
        /// <summary>
        /// Gets a Rect structure that contains the union of two Rect structures. 
        /// </summary>
        /// <param name="a">A rectangle to union. </param>
        /// <param name="b">A rectangle to union. </param>
        /// <returns></returns>
        public static Rect Union(Rect a, Rect b)
        {
            int x1 = Math.Min(a.X, b.X);
            int x2 = Math.Max(a.X + a.Width, b.X + b.Width);
            int y1 = Math.Min(a.Y, b.Y);
            int y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new Rect(x1, y1, x2 - x1, y2 - y1);
        }
        
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
            return X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
        }
        
        /// <summary>
        /// Converts this object to a human readable string.
        /// </summary>
        /// <returns>A string that represents this object.</returns>
        public override string ToString()
        {
            return string.Format("(x:{0} y:{1} width:{2} height:{3})", X, Y, Width, Height);
        }

        /// <summary>
        /// Converts this object to the Point[] array clockwise starting with the top-left corner
        /// </summary>
        /// <returns>Array of 4 points with rectangle corners</returns>
        public Point[] ToArray()
        {
            return new Point[] { TopLeft, TopRight, BottomRight, BottomLeft };
        }

        #endregion
    }
}