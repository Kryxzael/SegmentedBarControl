using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bars
{
    /// <summary>
    /// Represents settings that can be applied to a segmented bar using SegmentedBar.ApplySettings()
    /// </summary>
    public class BarSettings
    {
        /// <summary>
        /// Sets the normal fill color of the bar
        /// </summary>
        public Color FillColor { get; set; }

        /// <summary>
        /// Sets the overflow color of the bar
        /// </summary>
        public Color OverflowColor { get; set; }

        /// <summary>
        /// Sets the maximum value of the bar
        /// </summary>
        public float MaxValue { get; set; }

        /// <summary>
        /// Sets the interval of the segments of the abr
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Sets the predicate that will be used to get the bar's value. This can be set to null to use the normal value
        /// </summary>
        public Func<float> ValueOverride { get; set; }

        /// <summary>
        /// Creates a new bar-settings instance
        /// </summary>
        /// <param name="maxValue"></param>
        /// <param name="interval"></param>
        /// <param name="fillColor"></param>
        /// <param name="overflowColor"></param>
        public BarSettings(float maxValue, int interval, Color fillColor, Color overflowColor)
        {
            MaxValue = maxValue;
            Interval = interval;
            FillColor = fillColor;
            OverflowColor = overflowColor;
        }

        /// <summary>
        /// Creates a new bar-settings instance with a value override value
        /// </summary>
        /// <param name="valueOverride"></param>
        /// <param name="maxValue"></param>
        /// <param name="interval"></param>
        /// <param name="fillColor"></param>
        /// <param name="overflowColor"></param>
        public BarSettings(Func<float> valueOverride, float maxValue, int interval, Color fillColor, Color overflowColor) : this(maxValue, interval, fillColor, overflowColor)
        {
            ValueOverride = valueOverride;
        }

    }
}
