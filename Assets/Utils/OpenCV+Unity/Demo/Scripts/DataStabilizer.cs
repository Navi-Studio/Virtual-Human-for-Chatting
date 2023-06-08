namespace OpenCvSharp.Demo
{
	using System;
	using System.Collections.Generic;
	using OpenCvSharp;

	/// <summary>
	/// Data stabilizer general parameters
	/// </summary>
	class DataStabilizerParams
    {
        /// <summary>
        /// Should this stabilizer just push data through (false value) or do some work before (true value)?
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Maximum ignored point distance
        /// </summary>
        public double Threshold { get; set; }

        /// <summary>
        /// Threshold scale factor (should processing space be scaled)
        /// </summary>
        public double ThresholdFactor { get; set; }
        
        /// <summary>
        /// Accumulated samples count
        /// </summary>
        public int SamplesCount { get; set; }

        /// <summary>
        /// Returns scaled threshold
        /// </summary>
        /// <returns></returns>
        public double GetScaledThreshold()
        {
            return Threshold * ThresholdFactor;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataStabilizerParams()
        {
            Enabled = true;
            Threshold = 1.0;
            ThresholdFactor = 1.0;
            SamplesCount = 10;
        }
    }

    /// <summary>
    /// Base DataStabilizer generic interface
    /// </summary>
    /// <typeparam name="T">Any data type</typeparam>
    interface IDataStabilizer<T>
    {
        /// <summary>
        /// Parameters, see corresponding class
        /// </summary>
        DataStabilizerParams Params { get; set; }

        /// <summary>
        /// Stabilized data
        /// </summary>
        T Sample { get; set; }

        /// <summary>
        /// Signals whether last data chunk changed anything
        /// </summary>
        bool LastApplied { get; set; }
    }

    /// <summary>
    /// Basic data stabilizer abstract implementation, intended to be used on compact data sets
    /// </summary>
    abstract class DataStabilizerBase<T>
    {
        protected T result;                     // computer output sample
        protected bool dirty = true;            // flag signals whether "result" sample must be recomputed
        protected T[] samples = null;           // whole samples set
        protected long inputSamples = 0;        // processed samples count

        /// <summary>
        /// Parameters, see corresponding class
        /// </summary>
        public DataStabilizerParams Params { get; set; }

        /// <summary>
        /// Stabilized data
        /// </summary>
        public virtual T Sample
        {
            get
            {
                // requires update
                if (dirty)
                {
                    // samples count changed
                    if (samples.Length != Params.SamplesCount)
                    {
                        T[] data = new T[Params.SamplesCount];
                        Array.Copy(samples, data, Math.Min(samples.Length, Params.SamplesCount));
                        samples = data;

                        // drop result
                        result = DefaultValue();
                    }

                    // prepare to compute
                    LastApplied = true;

                    // process samples
                    if (Params.Enabled)
                        LastApplied = PrepareStabilizedSample();
                    // stabilizer is disabled - simply grab the fresh-most sample
                    else
                        result = samples[0];
                    dirty = false;
                }
                return result;
            }
            set
            {
                ValidateSample(value);

                // shift and push new value to the top
                T[] data = new T[Params.SamplesCount];
                Array.Copy(samples, 0, data, 1, Params.SamplesCount - 1);
                data[0] = value;
                samples = data;
                inputSamples++;

                // mark
                dirty = true;
            }
        }

        /// <summary>
        /// Signals whether last data chunk changed anything
        /// </summary>
        public bool LastApplied { get; private set; }

        /// <summary>
        /// Constructs base data stabilizer
        /// </summary>
        protected DataStabilizerBase(DataStabilizerParams parameters)
        {
            Params = parameters;
            samples = new T[Params.SamplesCount];
            result = DefaultValue();
        }

        /// <summary>
        /// Computes stabilized data sample
        /// </summary>
        /// <returns>True if data has been recomputed, false if nothing changed for returned sample</returns>
        protected abstract bool PrepareStabilizedSample();

        /// <summary>
        /// Computes average data sample
        /// </summary>
        /// <returns></returns>
        protected abstract T ComputeAverageSample();

        /// <summary>
        /// Tests sample validity, must throw on unexpected value
        /// </summary>
        /// <param name="sample">Sample to test</param>
        protected abstract void ValidateSample(T sample);

        /// <summary>
        /// Gets default value for the output sample
        /// </summary>
        /// <returns></returns>
        protected abstract T DefaultValue();
    }

    /// <summary>
    /// On top of various OpenCV stabilizers for video itself (like optical flow) we might
    /// need a simpler one "stabilizer" for some data reacquired each frame like face rects,
    /// face landmarks etc.
    /// 
    /// This class is designed to be fast, so it basically applies some threshold and simplest heuristics
    /// to decide whether to update it's data set with new data chunk
    /// </summary>
    class PointsDataStabilizer : DataStabilizerBase<Point[]>
    {
        /// <summary>
        /// Flag signaling whether data set is interpreted as whole (Triangle, Rectangle) or independent (independent points array)
        /// </summary>
        public bool PerPointProcessing { get; set; }

        /// <summary>
        /// Creates DataStabilizer instance
        /// </summary>
        /// <param name="parameters">Stabilizer general parameters</param>
        public PointsDataStabilizer(DataStabilizerParams parameters)
            : base(parameters)
        {
            PerPointProcessing = true;
        }

        /// <summary>
        /// Validate sample
        /// </summary>
        /// <param name="sample"></param>
        protected override void ValidateSample(Point[] sample)
        {
            if (null == sample || sample.Length == 0)
                throw new ArgumentException("sample: is null or empty array.");

            foreach (Point[] data in samples)
            {
                if (data != null && data.Length != sample.Length)
                    throw new ArgumentException("sample: invalid input data, length does not match.");
            }
        }

        /// <summary>
        /// Computes average data sample
        /// </summary>
        /// <returns></returns>
        protected override Point[] ComputeAverageSample()
        {
            // we need full stack to run
            if (inputSamples < Params.SamplesCount)
                return null;

            // accumulate average
            int sampleSize = samples[0].Length;
            Point[] average = new Point[sampleSize];
            for (int s = 0; s < Params.SamplesCount; ++s)
            {
                Point[] data = samples[s];
                for (int i = 0; i < sampleSize; ++i)
                    average[i] += data[i];
            }

            // normalize
            double inv = 1.0 / Params.SamplesCount;
            for (int i = 0; i < sampleSize; ++i)
                average[i] = new Point(average[i].X * inv + 0.5, average[i].Y * inv);
            return average;
        }

        /// <summary>
        /// Computes data sample
        /// </summary>
        /// <returns>True if final sample changed, false if current frame is the same as the last one</returns>
        protected override bool PrepareStabilizedSample()
        {
            // get average
            Point[] average = ComputeAverageSample();
            if (null == average)
                return false;

            // if we have no saved result at all - average will do
            if (DefaultValue() == result)
            {
                result = average;
                return true;
            }
            
            // we have new average and saved data as well - test it
            double dmin = double.MaxValue, dmax = double.MinValue, dmean = 0.0;
            double[] distance = new double[result.Length];
            for (int i = 0; i < result.Length; ++i)
            {
                double d = Point.Distance(result[i], average[i]);

                dmean += d;
                dmax = Math.Max(dmax, d);
                dmin = Math.Min(dmin, d);
                distance[i] = d;
            }
            dmean /= result.Length;

            // check whether it's OK to apply
            double edge = Params.Threshold;
            if (dmean > edge)
            {
                result = average;
                return true;
            }

            // per-item process
            bool anyChanges = false;
            if (PerPointProcessing)
            {
                for (int i = 0; i < result.Length; ++i)
                {
                    if (distance[i] > edge)
                    {
                        anyChanges = true;
                        result[i] = average[i];
                    }
                }
            }
            return anyChanges;
        }

        /// <summary>
        /// Gets default value for the output sample
        /// </summary>
        /// <returns></returns>
        protected override Point[] DefaultValue()
        {
            return null;
        }
    }

    /// <summary>
    /// Data stabilizer designed for OpenCv Rect (Object tracking, face detection etc.)
    /// </summary>
    class RectStabilizer : DataStabilizerBase<Rect>
    {
        /// <summary>
        /// Constructs Rectangle stabilizer
        /// </summary>
        /// <param name="parameters">Data stabilizer general parameters</param>
        public RectStabilizer(DataStabilizerParams parameters)
            : base(parameters)
        {}
        
        /// <summary>
        /// Computes average data sample
        /// </summary>
        /// <returns></returns>
        protected override Rect ComputeAverageSample()
        {
            Rect average = new Rect();
            if (inputSamples < Params.SamplesCount)
                return average;

            foreach (Rect rc in samples)
                average = average + rc;
            return average * (1.0 / Params.SamplesCount);
        }

        /// <summary>
        /// For Rect stabilizer any sample is valid
        /// </summary>
        /// <param name="sample">Sample to test</param>
        protected override void ValidateSample(Rect sample)
        {}

        /// <summary>
        /// Prepares stabilized sample (Rectangle)
        /// </summary>
        protected override bool PrepareStabilizedSample()
        {
            Rect average = ComputeAverageSample();

            // quick check
            if (DefaultValue() == result)
            {
                result = average;
                return true;
            }

            // compute per-corner distance between the frame we have and new one
            double dmin = double.MaxValue, dmax = double.MinValue, dmean = 0.0;
            Point[] our = result.ToArray(), their = average.ToArray();
            for (int i = 0; i < 4; ++i)
            {
                double distance = Point.Distance(our[i], their[i]);
                dmin = Math.Min(distance, dmin);
                dmax = Math.Max(distance, dmax);
                dmean += distance;
            }
            dmean /= their.Length;

            // apply conditions
            if (dmin > Params.GetScaledThreshold())
            {
                result = average;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets default value for the output sample
        /// </summary>
        /// <returns></returns>
        protected override Rect DefaultValue()
        {
            return new Rect();
        }
    }
}