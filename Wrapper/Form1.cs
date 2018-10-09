using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bars;

namespace Wrapper
{
    public partial class Form1 : Form
    {
        private SegmentedBar _bar = new SegmentedBar()
        {
            Dock = DockStyle.Fill
        };

        public Form1()
        {
            InitializeComponent();
            Controls.Add(_bar);
            _bar.ApplySettings(new BarSettings(() => DateTime.Now.Second, 60, 5, Color.Green, Color.Red));

            Timer tmr = new Timer();
            tmr.Tick += (s, e) => _bar.Refresh();
            tmr.Interval = 1000;
            tmr.Start();
        }
    }
}
