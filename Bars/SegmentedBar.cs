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
        private int _margin;

        #region Control Properties
        /// <summary>
        /// Gets or sets the value of the bar. Not that if ValueOverrideCode is set, settings this value does nothing
        /// </summary>
        public float Value
        {
            get => _valueOverr?.Invoke() ?? _value;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Value cannot be negative");

                _value = value;
                Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the maximum value of the bar
        /// </summary>
        public float MaxValue
        {
            get => _maxValue;
            set
            {
                if (_maxValue <= 0)
                    throw new ArgumentOutOfRangeException("Maxvalue cannot be negative or zero");

                _maxValue = value;
                Refresh();
            }
        }

        /// <summary>
        /// Gets or sets a function that overrides the value of the bar. If this bar is set
        /// </summary>
        public Func<float> OverrideValueCode
        {
            get => _valueOverr;
            set
            {
                _valueOverr = value;
                Refresh();
            }
        }

        public int Interval
        {
            get => _interval;
            set
            {
                if (_interval <= 0)
                    throw new ArgumentOutOfRangeException("Interval must be atleast 1");

                _interval = value;
                Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the color that will be used to fill the contents of the bar
        /// </summary>
        public Color FillColor
        {
            get => _fillColor;
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
            get => _overflowColor;
            set
            {
                _overflowColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the amount of pixels of margin the bar has
        /// </summary>
        public int BarMargin
        {
            get => _margin;
            set
            {
                _margin = value;
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
            _margin = settings.Margin;

            Refresh();

        }
        #endregion

        #region Native Windows Events
        protected override bool DoubleBuffered => true;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Brush BGBrush = new SolidBrush(BackColor); //The background brush
            Brush FGBrush = new SolidBrush(FillColor); //The filling brush
            Brush OFBrush = new SolidBrush(OverflowColor); //The overflow brush
            Brush STBrush = new SolidBrush(ForeColor); //The string brush

            Rectangle bounds = new Rectangle(BarMargin, BarMargin, Width - (2 * BarMargin), Height - (2 * BarMargin));
            e.Graphics.Clear(ColorTranslator.FromHtml("#222"));
            const int K = 3;
            e.Graphics.FillRectangle(Brushes.Silver, bounds.X - K, bounds.Y - K, bounds.Width + 2 * K, bounds.Height + 2 * K);

            if (Value > 0)
            {
                if (Value > MaxValue)
                {
                    e.Graphics.FillRectangle(OFBrush, bounds);
                    e.Graphics.FillRectangle(FGBrush, bounds.X, bounds.Y, MaxValue / Value * bounds.Width, bounds.Height);
                }
                else
                {
                    e.Graphics.FillRectangle(FGBrush, bounds.X, bounds.Y, (int)Math.Ceiling(Value / MaxValue * bounds.Width), bounds.Height);
                }
            }
            else
            {
                if (Math.Abs(Value) > MaxValue)
                {
                    e.Graphics.FillRectangle(Brushes.OrangeRed, bounds);
                    e.Graphics.FillRectangle(Brushes.Yellow, bounds.X, bounds.Y, MaxValue / Math.Abs(Value) * bounds.Width, bounds.Height);
                }
                else
                {
                    e.Graphics.FillRectangle(Brushes.Yellow, bounds.X, bounds.Y, (int)Math.Ceiling(Math.Abs(Value) / MaxValue * bounds.Width), bounds.Height);
                }
            }


            float localMaxValue = Math.Max(MaxValue, Math.Abs(Value));

            for (int i = Interval; i < localMaxValue; i++)
            {
                if (i % Interval == 0)
                {
                    e.Graphics.DrawLine(
                        pen: Pens.Silver, 
                        x1: (i / localMaxValue * bounds.Width) - 1 + BarMargin, 
                        y1: bounds.Top, 
                        x2: (i / localMaxValue * bounds.Width) - 1 + BarMargin, 
                        y2: bounds.Bottom    
                    );

                    e.Graphics.DrawLine(
                        pen: SystemPens.Control, 
                        x1: (i / localMaxValue * bounds.Width) + BarMargin, 
                        y1: bounds.Top, 
                        x2: (i / localMaxValue * bounds.Width) + BarMargin, 
                        y2: bounds.Bottom
                    );

                    e.Graphics.DrawLine(
                        pen: Pens.Silver, 
                        x1: (i / localMaxValue * bounds.Width) + 1 + BarMargin,
                        y1: bounds.Top, 
                        x2: (i / localMaxValue * bounds.Width) + 1 + BarMargin, 
                        y2: bounds.Bottom
                    );

                    e.Graphics.DrawString(
                        s: GetStringForBarSegment(i), 
                        font: SystemFonts.DefaultFont, 
                        brush: STBrush, 
                        point: new PointF(
                            x: (i / localMaxValue * bounds.Width) - 1, 
                            y: BarMargin), 
                        format: new StringFormat(StringFormatFlags.DirectionRightToLeft)
                    );
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
            => segmentValue.ToString();
        #endregion

    }
}
