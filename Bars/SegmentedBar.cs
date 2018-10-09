using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bars
{
    /// <summary>
    /// a sort of progressbar with segments 
    /// </summary>
    public class SegmentedBar : Control
    {
        private float _value;
        private int _interval = 1;
        private float _maxValue = 1f;
        private Func<float> _valueOverr;
        private Color _fillColor;
        private Color _overflowColor;

        #region Control Properties
        /// <summary>
        /// Gets or sets the value of the bar. Not that if ValueOverrideCode is set, settings this value does nothing
        /// </summary>
        public float Value
        {
            get
            {
                if (_valueOverr == null)
                {
                    return _value;
                }
                else
                {
                    return _valueOverr();
                }
                
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Value cannot be negative");
                }
                _value = value;
                Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the maximum value of the bar
        /// </summary>
        public float MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                if (_maxValue <= 0)
                {
                    throw new ArgumentOutOfRangeException("Maxvalue cannot be negative or zero");
                }
                _maxValue = value;
                Refresh();
            }
        }

        /// <summary>
        /// Gets or sets a function that overrides the value of the bar. If this bar is set
        /// </summary>
        public Func<float> OverrideValueCode
        {
            get
            {
                return _valueOverr;
            }
            set
            {
                _valueOverr = value;
                Refresh();
            }
        }

        public int Interval
        {
            get
            {
                return _interval;
            }
            set
            {
                if (_interval <= 0)
                {
                    throw new ArgumentOutOfRangeException("Interval must be atleast 1");
                }

                _interval = value;
                Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the color that will be used to fill the contents of the bar
        /// </summary>
        public Color FillColor
        {
            get
            {
                return _fillColor;
            }
            set
            {
                _fillColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the color of any overflowing value of this bar
        /// </summary>
        public Color OverflowColor
        {
            get
            {
                return _overflowColor;
            }
            set
            {
                _overflowColor = value;
                Refresh();
            }
        }

        #endregion

        #region Public functions
        /// <summary>
        /// Applies a bar-settings instance to this bar
        /// </summary>
        /// <param name="settings"></param>
        public void ApplySettings(BarSettings settings)
        {
            _interval = settings.Interval;
            _maxValue = settings.MaxValue;
            _valueOverr = settings.ValueOverride;
            _fillColor = settings.FillColor;
            _overflowColor = settings.OverflowColor;

        }
        #endregion

        #region Native Windows Events
        protected override bool DoubleBuffered
        {
            get
            {
                return true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Brush BGBrush, FGBrush, OFBrush, STBrush;

            BGBrush = new SolidBrush(BackColor); //The background brush
            FGBrush = new SolidBrush(FillColor); //The filling brush
            OFBrush = new SolidBrush(OverflowColor); //The overflow brush
            STBrush = new SolidBrush(ForeColor); //The string brush

            e.Graphics.Clear(Color.LightGray);
            e.Graphics.DrawRectangle(Pens.Silver, new Rectangle(0, 0, Width - 1, Height - 1));

            if (Value > 0)
            {
                if (Value > MaxValue)
                {
                    e.Graphics.FillRectangle(OFBrush, 1, 1, Width - 2, Height - 2);
                    e.Graphics.FillRectangle(FGBrush, 1, 1, MaxValue / Value * Width, Height - 2);
                }
                else
                {
                    e.Graphics.FillRectangle(FGBrush, 1, 1, (int)Math.Ceiling((float)Value / MaxValue * Width) - 2, Height - 2);
                }
            }
            else
            {
                if (Math.Abs(Value) > MaxValue)
                {
                    e.Graphics.FillRectangle(Brushes.OrangeRed, 1, 1, Width - 2, Height - 2);
                    e.Graphics.FillRectangle(Brushes.Yellow, 1, 1, MaxValue / Math.Abs(Value) * Width, Height - 2);
                }
                else
                {
                    e.Graphics.FillRectangle(Brushes.Yellow, 1, 1, (int)Math.Ceiling((float)Math.Abs(Value) / MaxValue * Width) - 2, Height - 2);
                }
            }


            float localMaxValue = Math.Max(MaxValue, Math.Abs(Value));

            for (int i = Interval; i < localMaxValue; i++)
            {
                if (i % Interval == 0)
                {
                    e.Graphics.DrawLine(Pens.Silver, new PointF((i / localMaxValue * Width) - 1, 0), new PointF((i / localMaxValue * Width) - 1, Height));
                    e.Graphics.DrawLine(SystemPens.Control, new PointF((i / localMaxValue * Width), 0), new PointF((i / localMaxValue * Width), Height - 1));
                    e.Graphics.DrawLine(Pens.Silver, new PointF((i / localMaxValue * Width) + 1, 0), new PointF((i / localMaxValue * Width) + 1, Height));
                    e.Graphics.DrawString(GetStringForBarSegment(i), SystemFonts.DefaultFont, STBrush, new PointF((i / localMaxValue * Width) - 1, 1), new StringFormat(StringFormatFlags.DirectionRightToLeft));
                }
            }

            base.OnPaint(e);

            FGBrush.Dispose();
            BGBrush.Dispose();
            OFBrush.Dispose();
            STBrush.Dispose();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Refresh();
        }

        /// <summary>
        /// Gets the text that will be displayed on the top-right of a bar-segment
        /// </summary>
        /// <param name="segmentValue">The segment that is being drawn</param>
        /// <returns></returns>
        protected virtual string GetStringForBarSegment(int segmentValue)
        {
            return segmentValue.ToString();
        }
        #endregion

    }
}
