using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private bool _drawHatched;
        private bool _drawHatchedOverflow;

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
                Invalidate();
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
                Invalidate();
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
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the size of each segment
        /// </summary>
        public int Interval
        {
            get => _interval;
            set
            {
                if (_interval <= 0)
                    throw new ArgumentOutOfRangeException("Interval must be atleast 1");

                _interval = value;
                Invalidate();
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
                Invalidate();
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
                Invalidate();
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
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether the normal fill of the bar will be drawn with a hatched brush
        /// </summary>
        public bool DrawHatched 
        {
            get => _drawHatched;
            set 
            {
                _drawHatched = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets whether the overflow fill of the bar will be drawn with a hatched brush
        /// </summary>
        public bool DrawHatchedOverflow
        {
            get => _drawHatchedOverflow;
            set
            {
                _drawHatchedOverflow = value;
                Invalidate();
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

            Invalidate();
        }
        #endregion

        #region Native Windows Events
        protected override bool DoubleBuffered => true;

        protected sealed override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //Set smoothing mode to anti-alias for smoothness
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            /*
             * Create brushes based on settings
             */

            Brush backgroundBrush = new SolidBrush(BackColor);
            Brush textBrush = new SolidBrush(Color.Black);
            Brush textBrushOnDark = new SolidBrush(Color.GhostWhite);
            Brush fillBrush, overflowBrush;

            if (DrawHatched)
            {
                fillBrush = new HatchBrush(
                    hatchstyle: HatchStyle.BackwardDiagonal,
                    foreColor:  Color.FromArgb(0xFF - FillColor.R, 0xFF - FillColor.G, 0xFF - FillColor.B),
                    backColor:  FillColor
                );
            }
            else
            {
                fillBrush = new SolidBrush(FillColor);
            }

            if (DrawHatchedOverflow)
            {
                overflowBrush = new HatchBrush(
                    hatchstyle: HatchStyle.BackwardDiagonal,
                    foreColor: Color.FromArgb(0xFF - OverflowColor.R, 0xFF - OverflowColor.G, 0xFF - OverflowColor.B),
                    backColor: OverflowColor
                );
            }
            else
            {
                overflowBrush = new SolidBrush(OverflowColor);
            }
            
            //Gets the bounds to draw to
            Rectangle bounds = new Rectangle(
                x: BarMargin, 
                y: BarMargin, 
                width: Width - (2 * BarMargin), 
                height: Height - (2 * BarMargin)
            );

            //Clears background with a dark color and draws the main frame
            e.Graphics.Clear(ColorTranslator.FromHtml("#222"));

            //What even is this constant?
            const int BORDER_OFFSET = 3;
            e.Graphics.FillRectangle(
                brush: Brushes.Silver, 
                x: bounds.X - BORDER_OFFSET, 
                y: bounds.Y - BORDER_OFFSET, 
                width: bounds.Width + 2 * BORDER_OFFSET, 
                height: bounds.Height + 2 * BORDER_OFFSET
            );

            //Draw backgrounds of bars
            if (Value > 0)
            {
                //Exceeding max value
                if (Value > MaxValue)
                {
                    //First draw with the overflow brush, then over-draw normal fill where applicable
                    e.Graphics.FillRectangle(overflowBrush, bounds);

                    e.Graphics.FillRectangle(
                        brush: fillBrush, 
                        x: bounds.X, 
                        y: bounds.Y, 
                        width: MaxValue / Value * bounds.Width, 
                        height: bounds.Height
                    );
                }

                //In range
                else
                {
                    //Just draw the fill
                    e.Graphics.FillRectangle(
                        brush: fillBrush, 
                        x: bounds.X, 
                        y: bounds.Y, 
                        width: (int)Math.Ceiling(Value / MaxValue * bounds.Width), 
                        height: bounds.Height
                    );
                }
            }

            //Draw background bars when negative (Not really supported well)
            else
            {
                //Subceeding -max value
                if (Math.Abs(Value) > MaxValue)
                {
                    //First draw with the overflow brush, then over-draw normal fill where applicable
                    e.Graphics.FillRectangle(Brushes.OrangeRed, bounds);

                    e.Graphics.FillRectangle(
                        brush: Brushes.Yellow, 
                        x: bounds.X, 
                        y: bounds.Y, 
                        width: MaxValue / Math.Abs(Value) * bounds.Width, 
                        height: bounds.Height
                    );
                }

                //In negative range
                else
                {
                    e.Graphics.FillRectangle(
                        brush: Brushes.Yellow, 
                        x: bounds.X, 
                        y: bounds.Y, 
                        width: (int)Math.Ceiling(Math.Abs(Value) / MaxValue * bounds.Width), 
                        height: bounds.Height
                    );
                }
            }

            //This is either the actual max value, or the value that is overflowing
            float effectiveMaxValue = Math.Max(MaxValue, Math.Abs(Value));

            /*
             * Draw segment separators and strings
             */
            for (int i = Interval; i < effectiveMaxValue + Interval; i += Interval)
            {
                //Draws the separator between each segment (Behind)
                e.Graphics.DrawLine(
                    pen: new Pen(Color.Silver, 3),
                    x1: (i / effectiveMaxValue * bounds.Width) + BarMargin,
                    y1: bounds.Top,
                    x2: (i / effectiveMaxValue * bounds.Width) + BarMargin,
                    y2: bounds.Bottom
                );

                //Draws the separator between each segment (Middle Front)
                e.Graphics.DrawLine(
                    pen: SystemPens.Control,
                    x1: (i / effectiveMaxValue * bounds.Width) + BarMargin,
                    y1: bounds.Top,
                    x2: (i / effectiveMaxValue * bounds.Width) + BarMargin,
                    y2: bounds.Bottom
                );
            }

            /*
            * Custom painting
            */

            DrawOnBar(e);

            for (int i = Interval; i < effectiveMaxValue + Interval; i += Interval)
            { 
                /*
                 * Draws the text
                 */
                //Difference between the current item's separator and the previous item's separator (Making sure not to overflow the width)
                float segmentWidth = Math.Min(bounds.Width, (i / effectiveMaxValue * bounds.Width)) - ((i - Interval) / effectiveMaxValue * bounds.Width) - BarMargin;

                string segmentText = "";
                SizeF segmentTextSize = default;

                //Choose the first string that fits (or blank by default)
                foreach (string segmentStringCandidate in GetStringsForBarSegment((int)Math.Min(i, Math.Ceiling(effectiveMaxValue))))
                {
                    segmentTextSize = e.Graphics.MeasureString(segmentStringCandidate, SystemFonts.DefaultFont);

                    if (segmentTextSize.Width <= segmentWidth && segmentTextSize.Height < bounds.Height)
                    {
                        segmentText = segmentStringCandidate;
                        break;
                    }
                }

                float x = Math.Min(bounds.Width, (i / effectiveMaxValue * bounds.Width));
                float y = BarMargin;

                if (segmentTextSize.Width > segmentWidth - 1)
                    x += segmentTextSize.Width - segmentWidth + 1;

                Brush brsh;
                Brush shadowBrsh;

                if (i > MaxValue)
                {
                    if (OverflowColor.GetBrightness() > 0.5f)
                    {
                        brsh = textBrush;
                        shadowBrsh = textBrushOnDark;
                    }
                    else
                    {
                        brsh = textBrushOnDark;
                        shadowBrsh = textBrush;
                    }
                }
                else
                {
                    if (FillColor.GetBrightness() > 0.5f)
                    {
                        brsh = textBrush;
                        shadowBrsh = textBrushOnDark;
                    }
                    else
                    {
                        brsh = textBrushOnDark;
                        shadowBrsh = textBrush;
                    }
                }

                e.Graphics.DrawString(
                    s: segmentText,
                    font: SystemFonts.DefaultFont,
                    brush: shadowBrsh,
                    point: new PointF(x + 1.5f, y + 1.5f),
                    format: new StringFormat(StringFormatFlags.DirectionRightToLeft)
                );

                e.Graphics.DrawString(
                    s: segmentText,
                    font: SystemFonts.DefaultFont,
                    brush: brsh,
                    point: new PointF(x, y),
                    format: new StringFormat(StringFormatFlags.DirectionRightToLeft)
                );
            }

            //Done with the brushes
            fillBrush.Dispose();
            backgroundBrush.Dispose();
            overflowBrush.Dispose();
            textBrush.Dispose();
            textBrushOnDark.Dispose();
        }

        /// <summary>
        /// Allows derived classes to draw textures on the bar before the text
        /// </summary>
        /// <param name="e"></param>
        protected virtual void DrawOnBar(PaintEventArgs e)
        {

        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Refresh();
        }

        /// <summary>
        /// Gets the text that will be displayed on the top-right of a bar-segment. The first fitting string will be selected
        /// </summary>
        /// <param name="segmentValue">The segment that is being drawn</param>
        /// <returns></returns>
        protected virtual string[] GetStringsForBarSegment(int segmentValue) 
            => new string[] { segmentValue.ToString() };

        #endregion

    }
}
