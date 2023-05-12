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
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
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

            List<Point> front = new();
            for (int i = 0; i < own.curr_points.Count; i++){
                List<Point> area = new();
                foreach (var poi in own.owingpoints[i]){
                    if (own.wave_de_points[i][poi.X, poi.Y] <= own.minrads[i])
                        area.Add(new(poi.X, poi.Y));
                }

                front = new(get_frontiers(area));

                Point last, closest;
                //front = front.Select(el => new Point(el.X * d + d / 2, el.Y * d + d / 2)).ToList();
                List<Point> ordered = new () { front[0] };
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
            
            /*
            for (int i = 0; i < own.curr_points.Count; i++){
                List<Point> area = new();
                foreach (var poi in own.owingpoints[i]) {
                    if (own.wave_de_points[i][poi.X,poi.Y] <= own.minrads[i])
                        area.Add(poi);
                }

                List<Point> front = new(own.get_frontiers(area));
                Point start = new();
                bool defed = false;
                for (int j = 0; j < front.Count; j++){
                    if (calcrank(front,front[j]) == 2){
                        start = new(front[j].X, front[j].Y);
                        defed = true;
                        break;
                    }
                }
                if (!defed) {
                    decimal max = 0;
                    foreach (var el in front){
                        if (max < own.wave_de_points[i][el.X, el.Y])
                            max = own.wave_de_points[i][el.X, el.Y];
                            start = new(el.X, el.Y);
                        }
                    }

                List<Point> vis = new() { start };
                if (front.Count > 10){

                    Point curr = new(start.X, start.Y);
                    while (true) {
                        if (calcrank(front, curr) == 2) {
                            curr = choisir_proch(front, curr, vis);
                        }
                        else{
                            List<Point> neibs = get_neighbors(front, curr, vis);
                            decimal max = 0;
                            foreach (var el in neibs){
                                if (max < own.wave_de_points[i][el.X, el.Y] && !vis.Contains(el)){ //  && own.wave_de_points[i][el.X, el.Y] <= own.minrads[i]
                                    max = own.wave_de_points[i][el.X, el.Y];
                                    curr = new(el.X, el.Y);
                                }
                            }
                        }
                        if (curr != vis[0])
                            vis.Add(curr);
                        else break;
                        if (vis[^1] == vis[^2]) break;
                        if (vis.Count > 3 && if_neighbors(vis[0], vis[^1])) { break; }
                    }
                }
                else 
                    vis = new(front);
                foreach (var el in vis){
                    carte.FillEllipse(new SolidBrush(System.Drawing.Color.Black), new System.Drawing.Rectangle(el.Y * d + d / 5, el.X * d + d / 5, 2 * d / 5, 2 * d / 5));
                }

                Point[] p1i = new Point[] {
                        new (vis[0].Y * d + d / 2 - 1, vis[0].X * d + d / 2),
                        new (vis[0].Y * d + d / 2 - 1, vis[0].X * d + 1 + d / 2),
                        new (vis[0].Y * d + d / 2 - 1, vis[0].X * d + 2 + d / 2),
                        new (vis[0].Y * d + d / 2, vis[0].X * d + d / 2),
                        new (vis[0].Y * d + d / 2, vis[0].X * d + 1 + d / 2),
                        new (vis[0].Y * d + d / 2, vis[0].X * d + 2 + d / 2),
                        new (vis[0].Y * d + d / 2 + 1, vis[0].X * d + d / 2),
                        new (vis[0].Y * d + d / 2 + 1, vis[0].X * d + 1 + d / 2),
                        new (vis[0].Y * d + d / 2 + 1, vis[0].X * d + 2 + d / 2)
                    };
                Point[] p2i = new Point[] {
                        new(vis[^1].Y * d + d/2 - 1, vis[^ 1].X * d + d / 2),
                        new(vis[^1].Y * d + d/2 - 1, vis[^ 1].X * d + 1 + d / 2),
                        new(vis[^ 1].Y * d + d/2 - 1, vis[^ 1].X * d + 2 + d / 2),
                        new(vis[^ 1].Y * d + d/2, vis[^ 1].X * d + d / 2),
                        new(vis[^ 1].Y * d + d/2, vis[^ 1].X * d + 1 + d / 2),
                        new(vis[^ 1].Y * d + d/2, vis[^ 1].X * d + 2 + d / 2),
                        new(vis[^ 1].Y * d + d/2 + 1, vis[^ 1].X * d + d / 2),
                        new(vis[^ 1].Y * d + d/2 + 1, vis[^ 1].X * d + 1 + d / 2),
                        new(vis[^ 1].Y * d + d/2 + 1, vis[^ 1].X * d + 2 + d / 2)
                    };
                for (int k = 0; k < p1i.Length; k++){
                    carte.DrawLine(new Pen(System.Drawing.Color.Black), p1i[k], p2i[k]);
                }
                for (int j = 1; j < vis.Count; j++){
                    p1i = new Point[] {
                        new (vis[j].Y * d + d / 2 - 1, vis[j].X * d + d / 2),
                        new (vis[j].Y * d + d / 2 - 1, vis[j].X * d + 1 + d / 2),
                        new (vis[j].Y * d + d / 2 - 1, vis[j].X * d + 2 + d / 2),
                        new (vis[j].Y * d + d / 2, vis[j].X * d + d / 2),
                        new (vis[j].Y * d + d / 2, vis[j].X * d + 1 + d / 2),
                        new (vis[j].Y * d + d / 2, vis[j].X * d + 2 + d / 2),
                        new (vis[j].Y * d + d / 2 + 1, vis[j].X * d + d / 2),
                        new (vis[j].Y * d + d / 2 + 1, vis[j].X * d + 1 + d / 2),
                        new (vis[j].Y * d + d / 2 + 1, vis[j].X * d + 2 + d / 2)
                    };
                    p2i = new Point[] {
                        new(vis[j - 1].Y * d + d/2 - 1, vis[j - 1].X * d + d / 2),
                        new(vis[j - 1].Y * d + d/2 - 1, vis[j - 1].X * d + 1 + d / 2),
                        new(vis[j - 1].Y * d + d/2 - 1, vis[j - 1].X * d + 2 + d / 2),
                        new(vis[j - 1].Y * d + d/2, vis[j - 1].X * d + d / 2),
                        new(vis[j - 1].Y * d + d/2, vis[j - 1].X * d + 1 + d / 2),
                        new(vis[j - 1].Y * d + d/2, vis[j - 1].X * d + 2 + d / 2),
                        new(vis[j - 1].Y * d + d/2 + 1, vis[j - 1].X * d + d / 2),
                        new(vis[j - 1].Y * d + d/2 + 1, vis[j - 1].X * d + 1 + d / 2),
                        new(vis[j - 1].Y * d + d/2 + 1, vis[j - 1].X * d + 2 + d / 2)
                    };
                    for (int k = 0; k < p1i.Length; k++) {
                        carte.DrawLine(new Pen(System.Drawing.Color.Black), p1i[k], p2i[k]);
                    }
                }
            }*/

            pictureBox1.Image = bmp;
        }
        private bool if_neighbors(Point p1, Point p2) {
            return Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2) <= 2;
            
        }
        private List<Point> get_neighbors(List<Point> set, Point curr, List<Point> vis){
            List<Point> neighbors = new List<Point>();
            Point c = new(curr.X - 1, curr.Y - 1);
            if (set.Contains(c) && !vis.Contains(c)) neighbors.Add(c);
            c = new(curr.X - 1, curr.Y);
            if (set.Contains(c) && !vis.Contains(c)) neighbors.Add(c);
            c = new(curr.X - 1, curr.Y + 1);
            if (set.Contains(c) && !vis.Contains(c)) neighbors.Add(c);
            c = new(curr.X, curr.Y + 1);
            if (set.Contains(c) && !vis.Contains(c)) neighbors.Add(c);
            c = new(curr.X + 1, curr.Y + 1);
            if (set.Contains(c) && !vis.Contains(c)) neighbors.Add(c);
            c = new(curr.X + 1, curr.Y);
            if (set.Contains(c) && !vis.Contains(c)) neighbors.Add(c);
            c = new(curr.X + 1, curr.Y - 1);
            if (set.Contains(c) && !vis.Contains(c)) neighbors.Add(c);
            c = new(curr.X    , curr.Y - 1);
            if (set.Contains(c) && !vis.Contains(c)) neighbors.Add(c);
            return neighbors;
        }
        private List<Point> get_neighbors(List<Point> set, Point curr){
            List<Point> neighbors = new List<Point>();
            Point c = new(curr.X - 1, curr.Y - 1);
            if (set.Contains(c)) neighbors.Add(c);
            c = new(curr.X - 1, curr.Y);
            if (set.Contains(c)) neighbors.Add(c);
            c = new(curr.X - 1, curr.Y + 1);
            if (set.Contains(c)) neighbors.Add(c);
            c = new(curr.X, curr.Y + 1);
            if (set.Contains(c)) neighbors.Add(c);
            c = new(curr.X + 1, curr.Y + 1);
            if (set.Contains(c)) neighbors.Add(c);
            c = new(curr.X + 1, curr.Y);
            if (set.Contains(c)) neighbors.Add(c);
            c = new(curr.X + 1, curr.Y - 1);
            if (set.Contains(c)) neighbors.Add(c);
            c = new(curr.X, curr.Y - 1);
            if (set.Contains(c)) neighbors.Add(c);
            return neighbors;
        }
        private Point choisir_proch(List<Point> set, Point curr, List<Point> vis){
            Point c = new(curr.X - 1, curr.Y - 1);
            if (set.Contains(c) && !vis.Contains(c)) return c;
            c = new(curr.X - 1, curr.Y);
            if (set.Contains(c) && !vis.Contains(c)) return c;
            c = new(curr.X - 1, curr.Y + 1);
            if (set.Contains(c) && !vis.Contains(c)) return c;
            c = new(curr.X, curr.Y + 1);
            if (set.Contains(c) && !vis.Contains(c)) return c;
            c = new(curr.X + 1, curr.Y + 1);
            if (set.Contains(c) && !vis.Contains(c)) return c;
            c = new(curr.X + 1, curr.Y);
            if (set.Contains(c) && !vis.Contains(c)) return c;
            c = new(curr.X + 1, curr.Y - 1);
            if (set.Contains(c) && !vis.Contains(c)) return c;
            return new Point(curr.X, curr.Y - 1);
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
