using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImViewLite.Misc
{
    public class TIMER : System.Windows.Forms.Timer
    {
        public TIMER() : base()
        { base.Enabled = true; }

        public TIMER(System.ComponentModel.IContainer container) : base(container)
        { base.Enabled = true; }

        private bool _Enabled;
        public override bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; }
        }

        protected override void OnTick(System.EventArgs e)
        { if (this.Enabled) base.OnTick(e); }
    }
}
