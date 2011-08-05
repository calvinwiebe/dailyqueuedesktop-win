using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DailyQueue
{
    class ClipboardGrabberWorker
    {
        ClipboardGrabber owner;

        public ClipboardGrabberWorker(ClipboardGrabber owner)
        {
            this.owner = owner;
        }

        public void run()
        {
            try
            {
                owner.Text = Clipboard.GetText(TextDataFormat.Text);
            }

            catch (Exception ex)
            {
                Exception threadEx = ex;
            }
        }
    }
}
