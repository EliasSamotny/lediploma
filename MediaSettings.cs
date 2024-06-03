using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace l_application_pour_diploma{
    public partial class MediaSettings : Form
    {
        Commencement own;
        public MediaSettings(Commencement o)
        {
            own = o;
            InitializeComponent();
        }

        internal void ToFrancais()
        {
           

        }

        internal void ToRusse()
        {
            
        }

        

        private void MediaSettings_Load(object sender, EventArgs e) {}
        
        private void MediaSettings_FormClosing(object sender, FormClosingEventArgs e) { own.mediaset = null; }
    }
}
