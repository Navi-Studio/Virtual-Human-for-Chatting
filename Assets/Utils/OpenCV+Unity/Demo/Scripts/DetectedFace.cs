namespace OpenCvSharp.Demo
{
	using System;
	using System.Collections.Generic;
	using OpenCvSharp;

	/// <summary>
	/// Detected object data
	/// </summary>
	class DetectedObject
    {
        PointsDataStabilizer marksStabilizer = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="stabilizerParameters">Data stabilizer params</param>
        public DetectedObject(DataStabilizerParams stabilizerParameters)
        {
            marksStabilizer = new PointsDataStabilizer(stabilizerParameters);
            marksStabilizer.PerPointProcessing = false;

            Marks = null;
            Elements = new DetectedObject[0];
        }

        /// <summary>
        /// Constructs object with name and region
        /// </summary>
        /// <param name="name">Detected objetc name</param>
        /// <param name="region">Detected object ROI on the source image</param>
        /// /// <param name="stabilizerParameters">Data stabilizer params</param>
        public DetectedObject(DataStabilizerParams stabilizerParameters, String name, Rect region)
            : this(stabilizerParameters)
        {
            Name = name;
            Region = region;
        }

        /// <summary>
        /// Constructs object with name and marks
        /// </summary>
        /// <param name="name">Detected object name</param>
        /// <param name="marks">Object landmarks (in the source image space)</param>
        /// /// <param name="stabilizerParameters">Data stabilizer params</param>
        public DetectedObject(DataStabilizerParams stabilizerParameters, String name, OpenCvSharp.Point[] marks)
            : this(stabilizerParameters)
        {
            Name = name;

            marksStabilizer.Sample = marks;
            Marks = marksStabilizer.Sample;

            Region = Rect.BoundingBoxForPoints(marks);
        }

        /// <summary>
        /// Object name
        /// </summary>
        public String Name { get; protected set; }

        /// <summary>
        /// Object region on the source image
        /// </summary>
        public Rect Region { get; protected set; }

        /// <summary>
        /// Object key points
        /// </summary>
        public OpenCvSharp.Point[] Marks { get; protected set; }

        /// <summary>
        /// Sub-objects
        /// </summary>
        public DetectedObject[] Elements { get; set; }

        /// <summary>
        /// Applies new marks
        /// </summary>
        /// <param name="marks">New points set</param>
        /// <param name="stabilizer">Signals whether we should apply stabilizer</param>
        /// <returns>True is new data applied, false if stabilizer rejected new data</returns>
        public virtual bool SetMarks(Point[] marks)
        {
            marksStabilizer.Sample = marks;

            Marks = marksStabilizer.Sample;
            return marksStabilizer.LastApplied;
        }
    }

    /// <summary>
    /// Detected face is a bit more complicated object with some extra "lazy" stuff
    /// </summary>
    class DetectedFace : DetectedObject
    {
        /// <summary>
        /// Face elements
        /// </summary>
        public enum FaceElements
        {
            Jaw = 0,

            LeftEyebrow,
            RightEyebrow,

            NoseBridge,
            Nose,

            LeftEye,
            RightEye,

            OuterLip,
            InnerLip
        }

        /// <summary>
        /// Simple 2d integer triangle
        /// </summary>
        public struct Triangle
        {
            public Point i;
            public Point j;
            public Point k;

            /// <summary>
            /// Special constructor
            /// </summary>
            /// <param name="vec">Vec containing triangle (like those returned by Subdiv.gettrianglesList() method)</param>
            public Triangle(Vec6f vec)
            {
                i = new Point((int)(vec[0] + 0.5), (int)(vec[1] + 0.5));
                j = new Point((int)(vec[2] + 0.5), (int)(vec[3] + 0.5));
                k = new Point((int)(vec[4] + 0.5), (int)(vec[5] + 0.5));
            }

            /// <summary>
            /// Converts triangle to points array
            /// </summary>
            /// <returns>Array of triangle points</returns>
            public Point[] ToArray()
            {
                return new Point[] { i, j, k };
            }
        }

        /// <summary>
        /// Face data like convex hull, delaunay triangulation etc.
        /// </summary>
        public sealed class FaceInfo
        {
            /// <summary>
            /// Face shape convex hull
            /// </summary>
            public Point[] ConvexHull { get; private set; }

            /// <summary>
            /// Face shape triangulation
            /// </summary>
            public Triangle[] DelaunayTriangles { get; private set; }

            /// <summary>
            /// Constructs face info
            /// </summary>
            /// <param name="hull">Convex hull</param>
            /// <param name="triangles">Delaunay triangulation data</param>
            internal FaceInfo(Point[] hull, Triangle[] triangles)
            {
                ConvexHull = hull;
                DelaunayTriangles = triangles;
            }
        }

        /// <summary>
        /// Face info, heavy and lazy-computed data
        /// </summary>
        public FaceInfo Info
        {
            get
            {
                if (null == faceInfo)
                {
                    // it's valid to have no marks (no shape predictor used)
                    if (null == Marks)
                        return null;

                    // convex hull
                    Point[] hull = Cv2.ConvexHull(Marks);

                    // compute triangles
                    Rect bounds = Rect.BoundingBoxForPoints(hull);
                    Subdiv2D subdiv = new Subdiv2D(bounds);
                    foreach (Point pt in Marks)
                        subdiv.Insert(pt);

                    Vec6f[] vecs = subdiv.GetTriangleList();
                    List<Triangle> triangles = new List<Triangle>();
                    for (int i = 0; i < vecs.Length; ++i)
                    {
                        Triangle t = new Triangle(vecs[i]);
                        if (bounds.Contains(t.ToArray()))
                            triangles.Add(t);
                    }

                    // save
                    faceInfo = new FaceInfo(hull, triangles.ToArray());
                }
                return faceInfo;
            }
        }

        protected FaceInfo faceInfo = null;
        RectStabilizer faceStabilizer = null;

        /// <summary>
        /// Constructs DetectedFace object
        /// </summary>
        /// <param name="roi">Face roi (rectangle) in the source image space</param>
        /// /// <param name="stabilizerParameters">Data stabilizer params</param>
        public DetectedFace(DataStabilizerParams stabilizerParameters, Rect roi)
            : base(stabilizerParameters, "Face", roi)
        {
            faceStabilizer = new RectStabilizer(stabilizerParameters);
        }

        /// <summary>
        /// Sets face rect
        /// </summary>
        /// <param name="roi">Face rect</param>
        public void SetRegion(Rect roi)
        {
            faceStabilizer.Sample = roi;

            Region = faceStabilizer.Sample;
            faceInfo = null;
        }

        /// <summary>
        /// Creates new sub-object
        /// </summary>
        /// <param name="element">Face element type</param>
        /// <param name="name">New object name</param>
        /// <param name="fromMark">Starting mark index</param>
        /// <param name="toMark">Ending mark index</param>
        /// <param name="factor">Scale factor</param>
        /// <param name="updateMarks">[optional] Signals whether we should apply new marks</param>
        public bool DefineSubObject(FaceElements element, string name, int fromMark, int toMark, bool updateMarks = true)
        {
            int index = (int)element;
            Point[] subset = Marks.SubsetFromTo(fromMark, toMark);
            DetectedObject obj = Elements[index];

            // first instance
            bool applied = false;
            if (null == obj)
            {
                applied = true;
                obj = new DetectedObject(faceStabilizer.Params, name, subset);
                Elements[index] = obj;
            }
            // updated
            else
            {
                if (updateMarks || null == obj.Marks || 0 == obj.Marks.Length)
                    applied = obj.SetMarks(subset);
            }

            return applied;
        }

        /// <summary>
        /// Sets face landmarks
        /// </summary>
        /// <param name="points">New landmarks set</param>
        public void SetLandmarks(Point[] points)
        {
            // set marks
            Marks = points;

            // apply subs
            if (null == Elements || Elements.Length < 9)
                Elements = new DetectedObject[9];
            int keysApplied = 0;

            // key elements
            if (null != Marks)
            {
                keysApplied += DefineSubObject(FaceElements.Nose, "Nose", 30, 35) ? 1 : 0;
                keysApplied += DefineSubObject(FaceElements.LeftEye, "Eye", 36, 41) ? 1 : 0;
                keysApplied += DefineSubObject(FaceElements.RightEye, "Eye", 42, 47) ? 1 : 0;

                // non-key but independent
                DefineSubObject(FaceElements.OuterLip, "Lip", 48, 59);
                DefineSubObject(FaceElements.InnerLip, "Lip", 60, 67);

                // dependent
                bool updateDependants = keysApplied > 0;
                DefineSubObject(FaceElements.LeftEyebrow, "Eyebrow", 17, 21, updateDependants);
                DefineSubObject(FaceElements.RightEyebrow, "Eyebrow", 22, 26, updateDependants);
                DefineSubObject(FaceElements.NoseBridge, "Nose bridge", 27, 30, updateDependants);
                DefineSubObject(FaceElements.Jaw, "Jaw", 0, 16, updateDependants);
            }

            // re-fetch marks from sub-objects as they have separate stabilizers
            List<Point> fetched = new List<Point>();
            foreach (DetectedObject obj in Elements)
                if (obj.Marks != null)
                    fetched.AddRange(obj.Marks);
            Marks = fetched.ToArray();

            // drop cache
            faceInfo = null;
        }
    }
}
