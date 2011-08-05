using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DailyQueue
{
    class ClipboardGrabber
    {
        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public ClipboardGrabber()
        {
            this.text = "";
        }


        public string grab()
        {
            ClipboardGrabberWorker grabberWorker = new ClipboardGrabberWorker(this);
            Thread worker = new Thread(grabberWorker.run);
            worker.SetApartmentState(ApartmentState.STA);
            worker.Start();
            worker.Join();
            return text;
        }
    }
}
