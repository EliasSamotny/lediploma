using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static l_application_pour_diploma.Classes;
using Point = System.Drawing.Point;
using System.Linq;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace l_application_pour_diploma{
    public partial class Packs : Form{
        Vran own;
        int d;
        public Packs(Vran own) {
            InitializeComponent();
            this.own = own;
            renew();
        }
        internal List<Point> get_frontiers(List<Point> set) {
            List<Point> frontl = new();
            foreach (var el in set) {
                if (if_front(set, el)) {
                    frontl.Add(el);
                }
            }
            return frontl;
        }
        public double Distance(Point p1, Point p2){
            int dx = p1.X - p2.X;
            int dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        internal void renew(){
            own.own.insert_log("Refreshing the packs...", this);
            Bitmap bmp = new(pictureBox1.Width, pictureBox1.Height);
            Graphics carte = Graphics.FromImage(bmp);
            d = Math.Min(pictureBox1.Width, pictureBox1.Height) / Math.Max(own.dataGridView2.RowCount, own.dataGridView2.ColumnCount);
            own.own.insert_log("Drawing the table...", this);
            for (int i = 0; i < own.dataGridView2.RowCount; i++){
                for (int j = 0; j < own.dataGridView2.Columns.Count; j++) {
                    carte.DrawRectangle(new Pen(System.Drawing.Color.White), j * d, i * d, d, d);
                    carte.FillRectangle(new SolidBrush(own.dataGridView2.Rows[i].Cells[j].Style.BackColor), j * d + 1, i * d + 1, d - 1, d - 1);
                }

            }
            
            List<Point> front = new();
            for (int i = 0; i < own.curr_points.Count; i++){
                List<Point> area = new();
                var minrad = own.minrads[i];
                var cent = own.curr_points[i];
                foreach (var poi in own.owingpoints[i]){
                    if (own.wave_de_points[i][poi.X, poi.Y] <= minrad)
                        area.Add(new(poi.X, poi.Y));
                }
                front = new(get_frontiers(area));

                /*Point centconv = new (cent.Y * d + d / 2, cent.X * d + d / 2);
                do {
                    centconv = new(centconv.Y + d , centconv.X);

                } while (!front.Contains(centconv));

                var pix = new List<Point> { centconv };
                centconv = new(cent.Y * d + d / 2, cent.X * d + d / 2);
                
                do {
                    //own.wave_de_points
                    var currpoi = pix[^1];


                } while (pix[^1] != pix[0] && pix.Count > 3);*/
                
                if (area.All(poi => own.own.source[0][area[0].X, area[0].Y] == own.own.source[0][poi.X, poi.Y] && own.own.checkBox4.Checked || 
                    !own.own.checkBox4.Checked && own.variants.All(vari => vari[poi.X, poi.Y] == own.variants.First()[poi.X, poi.Y]))) {
                    own.own.insert_log("Drawing the circle...", this);
                    carte.DrawEllipse(Pens.Black, cent.Y * d + d / 2 - (int)minrad * d, cent.X * d + d / 2 - (int)minrad * d, 2 * (int)minrad * d, 2 * (int)minrad * d);
                }
                else {
                    own.own.insert_log("Drawing the \"circle\"...", this);
                    Point last, closest;
                    //front = front.Select(el => new Point(el.X * d + d / 2, el.Y * d + d / 2)).ToList();
                    List<Point> ordered = new() { front[0] };
                    do {
                        last = ordered[^1];
                        closest = front
                            .Where(p => !ordered.Contains(p) && if_neighbors(p, last)) // && if_neighbors(p, last)
                            .OrderBy(p => Distance(last, p))
                            //.OrderByDescending(p => Distance(last, own.curr_points[i]))
                            .First();
                        ordered.Add(closest);
                    } while (front.Where(p => !ordered.Contains(p) && if_neighbors(p, closest)).Any()); // && if_neighbors(p, closest)
                                                                                                        //ordered.Add(ordered[0]);

                    ordered = ordered.Select(el => new Point(el.Y * d + d / 2, el.X * d + d / 2)).ToList();
                    carte.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    carte.DrawClosedCurve(Pens.Black, ordered.ToArray());
                }
            }

            pictureBox1.Image = bmp;
            own.own.insert_log("The packs refreshed.", this);
        }
        private bool if_neighbors(Point p1, Point p2) {
            return Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2) <= 2;
            
        }
        internal void toFrancais(){
            Text = "Les forfaits";
        }
        internal void toRusse(){
            Text = "Упаковки";
        }
        private bool if_front(List<Point> set, Point point){
            if (!set.Contains(new Point(point.X - 1, point.Y - 1))) return true;
            if (!set.Contains(new Point(point.X - 1, point.Y))) return true;
            if (!set.Contains(new Point(point.X - 1, point.Y + 1))) return true;
            if (!set.Contains(new Point(point.X, point.Y + 1))) return true;
            if (!set.Contains(new Point(point.X + 1, point.Y + 1))) return true;
            if (!set.Contains(new Point(point.X + 1, point.Y))) return true;
            if (!set.Contains(new Point(point.X + 1, point.Y - 1))) return true;
            if (!set.Contains(new Point(point.X, point.Y - 1))) return true;
            return false;
        }
        internal int calcrank(List<Point> set, Point point){
            int count = 0;
            if (set.Contains(new Point(point.X - 1, point.Y - 1))) count++;
            if (set.Contains(new Point(point.X - 1, point.Y))) count += 1;
            if (set.Contains(new Point(point.X - 1, point.Y + 1))) count++;
            if (set.Contains(new Point(point.X, point.Y + 1))) count += 1;
            if (set.Contains(new Point(point.X + 1, point.Y + 1))) count++;
            if (set.Contains(new Point(point.X + 1, point.Y))) count += 1;
            if (set.Contains(new Point(point.X + 1, point.Y - 1))) count++;
            if (set.Contains(new Point(point.X, point.Y - 1))) count += 1;
            return count;
        }
        private void Packs_FormClosing(object sender, FormClosingEventArgs e) { own.forfait = null; }
        private void Packs_ResizeBegin(object sender, EventArgs e) { renew(); }
        private void Packs_ResizeEnd(object sender, EventArgs e) { renew(); }
        private void pictureBox1_Resize(object sender, EventArgs e) { renew(); }
    }
}
