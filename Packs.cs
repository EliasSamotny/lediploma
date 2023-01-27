using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace l_application_pour_diploma{
    public partial class Packs : Form{
        Vran own;
        int d;
        public Packs(Vran own) {
            InitializeComponent();
            this.own = own;
            renew();
        }
        internal void renew(){
            Bitmap bmp = new(pictureBox1.Width, pictureBox1.Height);
            Graphics carte = Graphics.FromImage(bmp);
            d = Math.Min(pictureBox1.Width, pictureBox1.Height) / Math.Max(own.dataGridView2.RowCount, own.dataGridView2.ColumnCount);
            for (int i = 0; i < own.dataGridView2.RowCount; i++){
                for (int j = 0; j < own.dataGridView2.Columns.Count; j++) {
                    carte.DrawRectangle(new Pen(System.Drawing.Color.White), j * d, i * d, d, d);
                    carte.FillRectangle(new SolidBrush(own.dataGridView2.Rows[i].Cells[j].Style.BackColor), j * d + 1, i * d + 1, d - 1, d - 1);
                }

            }
            pictureBox1.Image = bmp;
        }
        internal void toFrancais(){
            Text = "Les forfaits";
        }
        internal void toRusse(){
            Text = "Упаковки";
        }
        private void Packs_FormClosing(object sender, FormClosingEventArgs e) { own.forfait = null; }
        private void Packs_ResizeBegin(object sender, EventArgs e) { renew(); }
        private void Packs_ResizeEnd(object sender, EventArgs e) { renew(); }
        private void pictureBox1_Resize(object sender, EventArgs e) { renew(); }
    }
}
