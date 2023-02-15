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
using Point = System.Drawing.Point;

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
            
            for (int i = 0; i < own.curr_points.Count; i++){
                List<Point> area = new();
                foreach (var poi in own.owingpoints[i]) {
                    if (own.wave_de_points[i][poi.X,poi.Y] <= own.minrads[i])
                        area.Add(poi);
                }

                List<Point> front = new(own.get_frontiers(area));
                Point start = new();
                for (int j = 0; j < front.Count; j++){
                    if (calcrank(front,front[j]) == 2){
                        start = new(front[j].X, front[j].Y);
                        break;
                    }
                }

                List<Point> vis = new() { start};
                Point curr = new(start.X, start.Y);
                while (true) {
                    
                    if (calcrank(front, curr) == 2){
                        curr = choisir_proch(front, curr,vis);
                    }
                    else {
                        List<Point> neibs = get_neighbors(front, curr, vis);
                        decimal max = 0;
                        foreach (var el in neibs){
                            if (max < own.wave_de_points[i][el.X, el.Y]) { //  && own.wave_de_points[i][el.X, el.Y] <= own.minrads[i]
                                max = own.wave_de_points[i][el.X, el.Y];
                                curr = new(el.X, el.Y);
                            }
                        }
                    }
                    vis.Add(curr);
                    if (vis.Count > 2 && if_neighbors(vis[0], vis[^1])) { break; }
                }

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
            }

            pictureBox1.Image = bmp;
        }

        private bool if_neighbors(Point point1, Point point2) {
            Point c = new(point2.X - 1, point2.Y - 1);
            if (point1 == c) return true;
            c = new(point2.X - 1, point2.Y);
            if (point1 == c) return true;
            c = new(point2.X - 1, point2.Y + 1);
            if (point1 == c) return true;
            c = new(point2.X, point2.Y + 1);
            if (point1 == c) return true;
            c = new(point2.X + 1, point2.Y + 1);
            if (point1 == c) return true;
            c = new(point2.X + 1, point2.Y);
            if (point1 == c) return true;
            c = new(point2.X + 1, point2.Y - 1);
            if (point1 == c) return true;
            c = new Point(point2.X, point2.Y - 1);
            if (point1 == c) return true;
            return false;
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
