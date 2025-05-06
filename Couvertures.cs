using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static l_application_pour_diploma.Classes;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Color = System.Drawing.Color;
using ex = Microsoft.Office.Interop.Excel;

namespace l_application_pour_diploma{
    public partial class Couvertures : Form
    {
        Commencement own;
        int d;
        List<Point> waved, rawPointList;
        decimal[,] wavessum;
        List<decimal[,]> waves;
        List<Point[,]> prevs;
        internal List<List<Point>> owingpoints;
        List<Point> points;
        private int size_rayon;
        List<Point> curr_centres;
        List<Point> voisins = new List<Point>() {
            new(- 1, - 1), new( 0, - 1), new(1, - 1), new(1, 0),   new(1, 1),   new(0, 1),   new(- 1, 1),   new(- 1, 0),
            new(- 1, - 2), new( 1, - 2), new(2, - 1), new(2, 1),   new(1, 2),   new(- 1, 2), new(- 2, 1),   new(- 2, - 1),
            new(- 2, - 3), new(- 1,- 3), new(1, - 3), new(2, - 3), new(3, - 2), new(3, - 1), new(3, 1),     new(3, 2),
            new(  2,   3), new( 1, 3  ), new(- 1, 3), new(- 2, 3), new(- 3, 2), new(- 3, 1), new(- 3, - 1), new(- 3, - 2)};
        internal List<Point> curr_points;
        private bool select_mode;

        public Couvertures(Commencement own)
        {
            this.own = own;
            InitializeComponent();
            curr_points = new();
            //refr(true);

            //comboBox5.SelectedIndex = 1;
            //comboBox5.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        internal void toRusse()
        {

        }
        internal void toFrancais()
        {

        }
        internal void refr(bool nouv)
        {
            Bitmap bmp = new(pictureBox1.Width, pictureBox1.Height);
            Graphics carte = Graphics.FromImage(bmp);
            d = Math.Min(pictureBox1.Width, pictureBox1.Height) / Math.Max((int)own.numericUpDown1.Value, (int)own.numericUpDown2.Value);
            for (int i = 0; i < (int)own.numericUpDown1.Value; i++)
            { //coloring cells
                for (int j = 0; j < (int)own.numericUpDown2.Value; j++)
                {
                    carte.DrawRectangle(new Pen(Color.Black), j * d, i * d, d, d);
                    carte.FillRectangle(new SolidBrush(own.dataGridView1.Rows[i].Cells[j].Style.BackColor), j * d + 1, i * d + 1, d - 1, d - 1);
                }

            }
            //carte.DrawRectangle(new Pen(Color.MediumOrchid), (int)numericUpDown9.Value * d, (int)numericUpDown10.Value * d, ((int)numericUpDown7.Value - (int)numericUpDown9.Value) * d, ((int)numericUpDown8.Value - (int)numericUpDown10.Value) * d); //draw domain coverage

            List<int> randcol = new List<int>();
            own.insert_log("Choosing colours...", this);
            for (int i = 0; i < dataGridView1.RowCount; i++)
            { //choosing les coleurs
                Random r = new Random();
                int cur = r.Next(ColeurList.Count);
                while (randcol.Contains(cur))
                {
                    cur = r.Next(ColeurList.Count);
                }
                randcol.Add(cur);
            }
            textBox1.Text = "0";
            if (nouv)
            {
                if (comboBox2.SelectedIndex == 0)
                {
                    label1.Text = Convert.ToString(0);
                    wavessum = new decimal[(int)own.numericUpDown1.Value, (int)own.numericUpDown2.Value];
                }
                else if (comboBox2.SelectedIndex == 1)
                {
                    rawPointList = new();
                    prevs = new();
                    if (dataGridView1.Rows.Count > 1)
                    {//waving et getting all edges
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {//from datagridview to list <Point>
                            rawPointList.Add(new(Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) - 1, Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) - 1));
                        }
                    }
                }

            }
            waves = new();
            waved = new();
            List<decimal[,]> destins;
            List<Point[,]> previos;
            owingpoints = new();
            List<Point> points = dataGridView1.Rows
                .Cast<DataGridViewRow>()
                .Select(row => new Point(Convert.ToInt32(row.Cells[0].Value) - 1, Convert.ToInt32(row.Cells[1].Value) - 1))
                .ToList();

            if (dataGridView1.Rows.Count > 0)
            {//drawing points
                owingpoints = new List<List<Point>>();
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    int x = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) - 1;
                    int y = Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) - 1;
                    carte.FillRectangle(new SolidBrush(Color.Brown), y * d + 1, x * d + 1, d - 1, d - 1);
                    var curr_wave = waving_in_domain_from_point(points[i]);//waving
                    var curr_dest = curr_wave.destinl;
                    var curr_prev = curr_wave.previosl;
                    waves.Add(curr_dest);
                    waved.Add(points[i]);
                    owingpoints.Add(new List<Point> { points[i] });
                }
                for (int i = 0; i < waves[0].GetLength(0); i++)
                {
                    for (int j = 0; j < waves[0].GetLength(1); j++)
                    {
                        if (own.source[0][i, j] <= 0)
                            carte.FillRectangle(new SolidBrush(Color.Black), j * d + 1, i * d + 1, d - 1, d - 1);
                        else if (!ifacentrepoint(i, j))
                        {
                            int h = mindest(i, j, waves);
                            carte.FillRectangle(new SolidBrush(ColeurList[randcol[h % randcol.Count]]), j * d + 1, i * d + 1, d - 1, d - 1);
                            owingpoints[h].Add(new Point(i, j));
                        }

                    }

                }
                var frontiers = new List<List<Point>>();
                //List<(Point[], decimal)> pairs = new();
                Point center = new();
                curr_centres = new();
                for (int i = 0; i < owingpoints.Count; i++)
                {
                    //Parallel.For(0, owingpoints.Count, i => { 
                    frontiers.Add(get_frontiers(owingpoints[i]));  //get frontier point list
                    if (frontiers[i].Count % 2 == 1)
                    {
                        frontiers.Select(frontier => frontier
                            .Where(point => Enumerable.Range(-1, 3)
                                .SelectMany(dx => Enumerable.Range(-1, 3), (dx, dy) => new Point(point.X + dx, point.Y + dy))
                                .Count(neighbor => frontier.Contains(neighbor) && !(neighbor.X == point.X && neighbor.Y == point.Y)) >= 3)
                            .ToList())
                        .ToList();
                        if (frontiers[i].Count % 2 == 1)
                            frontiers[i].RemoveAt(0);
                    }
                    (Point, Point, decimal) pair_points; //starting determining the diametre
                    //while () { }
                    pair_points = new(); //starting determining the diametre
                    pair_points.Item1 = new Point(frontiers[i][0].X, frontiers[i][0].Y);
                    List<Point> routel = new List<Point>();
                    var wavel = waving_in_domain_from_point(frontiers[i][0], owingpoints[i]);
                    int indl = 0;
                    //Parallel.For(0, frontiers[i].Count, j => {
                    for (int j = 1; j < frontiers[i].Count; j++)
                    {
                        if (wavel.destinl[frontiers[i][j].X, frontiers[i][j].Y] > pair_points.Item3)
                        {
                            indl = j;
                            pair_points.Item2 = new Point(frontiers[i][j].X, frontiers[i][j].Y);
                            pair_points.Item3 = wavel.destinl[frontiers[i][j].X, frontiers[i][j].Y];
                            routel = new List<Point> { pair_points.Item2 };

                        }
                    }
                    //);
                    routel = new List<Point> { pair_points.Item2 };
                    while (routel[^1] != pair_points.Item1 && routel[^1] != new Point(-2, -2))
                    { //route constructing
                        routel.Add(wavel.previosl[routel[^1].X, routel[^1].Y]);
                    }
                    //need to find centre

                    center = new(routel[routel.Count / 2].X, routel[routel.Count / 2].Y);// initial center
                    decimal delta = Math.Abs(pair_points.Item3 / 2 - wavel.destinl[center.X, center.Y]);

                    foreach (Point p in routel)
                    {
                        if (delta > Math.Abs(pair_points.Item3 / 2 - wavel.destinl[p.X, p.Y]))
                        {
                            delta = Math.Abs(pair_points.Item3 / 2 - wavel.destinl[p.X, p.Y]);
                            center = new(p.X, p.Y);
                        }
                        if (delta! < 0.0001M) break;
                    }
                    (Point, Point, decimal) pair_pointsl = new(); //temp tuple
                    frontiers[i].RemoveAt(indl);
                    frontiers[i].RemoveAt(0);
                    while (frontiers[i].Count > 1)
                    { //determining the longest pair - diametre
                        wavel = waving_in_domain_from_point(frontiers[i][0], owingpoints[i]);
                        pair_pointsl.Item1 = new Point(frontiers[i][0].X, frontiers[i][0].Y);
                        pair_pointsl.Item3 = 0;
                        var destinl = wavel.destinl;
                        var prevsl = wavel.previosl;
                        int ind = -1;
                        for (int n = 0; n < frontiers[i].Count; n++)
                        {
                            if (wavel.destinl[frontiers[i][n].X, frontiers[i][n].Y] > pair_pointsl.Item3)
                            {
                                indl = n;
                                pair_pointsl.Item2 = new Point(frontiers[i][n].X, frontiers[i][n].Y);
                                pair_pointsl.Item3 = wavel.destinl[frontiers[i][n].X, frontiers[i][n].Y];
                            }
                        }
                        if (pair_points.Item3 < pair_pointsl.Item3)
                        {
                            pair_points = new(pair_pointsl.Item1, pair_pointsl.Item2, pair_pointsl.Item3);
                            routel = new List<Point> { pair_points.Item2 };
                            while (routel[^1] != pair_pointsl.Item1/* && routel[^1] != new Point(-2, -2)*/)
                            {
                                routel.Add(prevsl[routel[^1].X, routel[^1].Y]);
                            }
                            delta = Math.Abs(pair_points.Item3 / 2 - wavel.destinl[routel[routel.Count / 2].X, routel[routel.Count / 2].Y]);
                            center = new(routel[routel.Count / 2].X, routel[routel.Count / 2].Y);
                            foreach (Point p in routel)
                            {
                                if (delta > Math.Abs(pair_points.Item3 / 2 - wavel.destinl[p.X, p.Y]))
                                {
                                    delta = Math.Abs(pair_points.Item3 / 2 - wavel.destinl[p.X, p.Y]);
                                    center = new(p.X, p.Y);
                                }
                                if (delta! < 0.0001M) break;
                            }
                        }


                        if (ind != -1)
                            frontiers[i].RemoveAt(ind);
                        frontiers[i].RemoveAt(0);
                        //frontiers[i].RemoveAt(0);
                    }
                    textBox1.Text = Convert.ToString(Convert.ToDecimal(textBox1.Text) + pair_points.Item3 / 2);
                    curr_centres.Add(center);
                    carte.DrawLine(new Pen(Color.Black), new(pair_points.Item1.Y * d + d / 2, pair_points.Item1.X * d + d / 2), new(pair_points.Item2.Y * d + d / 2, pair_points.Item2.X * d + d / 2));
                    //});
                }
                dataGridView1.RowCount = 0;
                foreach (var c in curr_centres)
                {
                    dataGridView1.Rows.Add(c.X + 1, c.Y + 1);
                    carte.FillRectangle(new SolidBrush(Color.White), c.Y * d + 1, c.X * d + 1, d - 1, d - 1);
                }

            }
            pictureBox1.Image = bmp;

        }
        private List<Color> ColeurList = new List<Color> {
            Color.Red, Color.Orange, Color.Yellow, Color.YellowGreen, Color.GreenYellow,
            Color.Green, Color.DarkGreen, Color.SkyBlue, Color.Cyan, Color.BlueViolet,
            Color.OrangeRed, Color.MediumBlue, Color.DarkBlue, Color.Violet,
            Color.Gold, Color.SeaGreen, Color.Silver,  Color.DarkViolet,
            Color.DeepSkyBlue, Color.DeepPink, Color.DarkOrchid, //Color.AliceBlue, Color.Azure,
        };
        private bool if_front(List<Point> set, Point point)
        {
            for (int i = 0; i < 8; i++)
                if (!set.Contains(new(point.X + voisins[i].X, point.Y + voisins[i].Y)))
                    return true;
            return false;
        }
        private bool ifacentrepoint(int x, int y)
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == x + 1 && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == y + 1)
                    return true;
            }
            return false;
        }
        private int mindest(int x, int y, List<decimal[,]> curr_destins)
        {
            decimal minv = curr_destins[0][x, y];
            int ind = 0;
            for (int i = 1; i < curr_destins.Count; i++)
            {
                if (minv > curr_destins[i][x, y])
                {
                    minv = curr_destins[i][x, y];
                    ind = i;
                }
            }
            return ind;
        }
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private (decimal[,] destinl, Point[,] previosl) waving_in_domain_from_point(Point commenc, List<Point> domain = null, decimal startadd = 0)
        {
            decimal[,] destinl = new decimal[(int)own.numericUpDown1.Value, (int)own.numericUpDown2.Value];
            Point[,] previosl = new Point[(int)own.numericUpDown1.Value, (int)own.numericUpDown2.Value];
            int xlc = commenc.X; int xlc1 = xlc;
            int ylc = commenc.Y; int ylc1 = ylc;

            bool[,] vis = new bool[(int)own.numericUpDown1.Value, (int)own.numericUpDown2.Value];
            bool[,] accessl = new bool[(int)own.numericUpDown1.Value, (int)own.numericUpDown2.Value];

            Parallel.For(0, (int)own.numericUpDown1.Value, i =>
            {
                for (int j = 0; j < (int)own.numericUpDown2.Value; j++)
                {
                    if (domain == null && own.source.All(el => el[i, j] > 0) || (domain != null && domain.Contains(new Point(i, j))))
                    {
                        accessl[i, j] = true;
                        destinl[i, j] = -2;
                        vis[i, j] = false;
                    }
                    else
                    {
                        accessl[i, j] = false;
                        vis[i, j] = true;
                        destinl[i, j] = -1;
                        previosl[i, j] = new Point(-1, -1);
                    }
                }
            });
            vis[xlc, ylc] = true;
            previosl[xlc, ylc] = new(-2, -2);
            destinl[xlc, ylc] = startadd;

            while (!checkallvis(vis))
            {
                vis[xlc1, ylc1] = true;
                //treads get lost here
                Parallel.For(0, size_rayon, h => { calculcell(ref accessl, ref destinl, ref previosl, xlc1 + voisins[h].X, ylc1 + voisins[h].Y, xlc1, ylc1, domain); });
                //for (int h = 0; h < size_rayon(); h++) calculcell(ref accessl, ref destinl, ref previosl, xlc1 + voisins[h].X, ylc1 + voisins[h].Y, xlc1, ylc1);

                //transmission le point actuel au point le plus proche et minimum
                var frontiers = new List<Points>();
                for (int i = 0; i < (int)own.numericUpDown1.Value; i++)//searching frontier points
                    for (int j = 0; j < (int)own.numericUpDown2.Value; j++)
                        if (destinl[i, j] != -2 && !vis[i, j])
                            frontiers.Add(new Points(i, j, destinl[i, j]));
                decimal mindest = decimal.MaxValue;
                foreach (var p in frontiers)//searching the minimum points
                    if (p.dest < mindest)
                        mindest = p.dest;
                var points = new List<Points>();
                foreach (var p in frontiers)
                    if (p.dest == mindest)
                        points.Add(new Points(p.xl, p.yl, (decimal)Math.Sqrt(Math.Pow(xlc1 - p.xl, 2) + Math.Pow(ylc1 - p.yl, 2))));

                if (points.Count == 1) { xlc1 = points[0].xl; ylc1 = points[0].yl; }//if it is alone to choose, equal et continue
                else if (points.Count > 1)
                { //if not, search nearest to (0,0) 
                    mindest = Decimal.MaxValue;
                    foreach (var p in points)
                        if (p.dest < mindest)
                            mindest = p.dest;
                    var points1 = new List<Points>();
                    foreach (var p in points)
                        if (p.dest == mindest)
                            points1.Add(new Points(p.xl, p.yl, (decimal)Math.Sqrt(Math.Pow(p.xl, 2) + Math.Pow(p.yl, 2))));
                    if (points1.Count == 1) { xlc1 = points1[0].xl; ylc1 = points1[0].yl; }//if it is alone to choose, equal et continue
                    else if (points1.Count > 1)
                    {//if not, choosing with least x
                        int minx = (int)own.numericUpDown1.Value;
                        foreach (var p in points1)
                            if (p.xl < minx)
                                minx = p.xl;
                        foreach (var p in points1)
                            if (p.xl == minx)
                            {
                                xlc1 = points1[0].xl;
                                ylc1 = points1[0].yl;
                                break;
                            }
                    }
                }
            }

            return (destinl, previosl);
        }
        private int currst(decimal dest)
        {
            int shift = own.stateShift;
            //own.transitions[]
            int currstage = 0;
            dest %= own.transitions.Sum();
            if (own.transitions.Count > 1)
                for (int i = 0; i < own.transitions.Count; i++)
                {
                    if (dest >= own.transitions[(i + shift) % own.transitions.Count])
                    {
                        dest -= own.transitions[(i + shift) % own.transitions.Count];
                    }
                    else
                    {
                        currstage = (i + shift) % own.transitions.Count;
                        break;
                    }
                }
            return (currstage) % own.source.Count;
        }
        private void calculcell(ref bool[,] access, ref decimal[,] dest, ref Point[,] prev, int xc, int yc, int xl, int yl, List<Point> domain = null)
        {
            if (xc >= 0 && yc >= 0 && xc < (int)own.numericUpDown1.Value && yc < (int)own.numericUpDown2.Value && access[xc, yc])
            {//if consists
                int currstage = currst(dest[xl, yl]);
                decimal cur = Convert.ToDecimal((Convert.ToDouble(own.source[currstage][xc, yc]) + Convert.ToDouble(own.source[currstage][xl, yl])) / 2 * Math.Sqrt(Math.Pow(xc - xl, 2) + Math.Pow(yc - yl, 2)) + Convert.ToDouble(dest[xl, yl]));
                if (currstage != currst(cur) && own.source.Count > 1 && own.checkBox5.Checked)
                {
                    int next = (currstage + 1) % own.source.Count;
                    cur = Convert.ToDecimal((Convert.ToDouble(own.source[next][xc, yc]) + Convert.ToDouble(own.source[next][xl, yl])) / 2 * Math.Sqrt(Math.Pow(xc - xl, 2) + Math.Pow(yc - yl, 2)) + Convert.ToDouble(dest[xl, yl]));
                }
                if (dest[xc, yc] == -2)
                {//if not visited at all
                    dest[xc, yc] = cur;
                    prev[xc, yc] = new Point(xl, yl);
                }
                else if (cur < dest[xc, yc])
                {//if from this point is shorter than known path
                    dest[xc, yc] = cur;
                    prev[xc, yc] = new Point(xl, yl);
                }
                else if (Convert.ToDecimal(own.source[0][xc, yc]) == -1)
                {  //if point unpassable
                    dest[xc, yc] = -1;
                    prev[xc, yc] = new Point(-1, -1);
                }
            }
        }
        internal List<Point> get_frontiers(List<Point> set)
        {
            List<Point> frontl = new();
            object lock_front = new();
            Parallel.ForEach(set, el =>
            {
                if (if_front(set, el))
                {
                    lock (lock_front)
                        frontl.Add(el);
                }
            });
            return frontl;
        }
        private bool checkallvis(bool[,] set)
        {
            for (int i = 0; i < (int)own.numericUpDown1.Value; i++)
                for (int j = 0; j < (int)own.numericUpDown2.Value; j++)
                    if (!set[i, j]) return false;
            return true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0) size_rayon = 8;
            else if (comboBox1.SelectedIndex == 1) size_rayon = 16;
            else size_rayon = 32;
            refr(true);
        }

        private void Couvertures_Load(object sender, EventArgs e)
        {
            int coeffl = 4;
            numericUpDown9.Value = own.numericUpDown1.Value / coeffl;
            numericUpDown10.Value = own.numericUpDown2.Value / coeffl;
            numericUpDown7.Value = own.numericUpDown1.Value * (coeffl - 1) / coeffl;
            numericUpDown8.Value = own.numericUpDown2.Value * (coeffl - 1) / coeffl;
            comboBox1.SelectedIndex = 2;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.SelectedIndex = 1;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            refr(true);
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Bitmap bmp = new(pictureBox1.Width, pictureBox1.Height);
            Graphics carte = Graphics.FromImage(bmp);
            int x = Math.Min(e.Y / d + 1, (int)own.numericUpDown1.Value);
            int y = Math.Min(e.X / d + 1, (int)own.numericUpDown2.Value);
            bool toadd = true;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == x && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == y)
                {
                    toadd = false;
                    dataGridView1.Rows.RemoveAt(i);
                    //refr(true);
                    break;
                }
            }
            if (toadd)
            {
                dataGridView1.Rows.Add(new object[] { x, y });
                //refr(true);
                carte.FillRectangle(new SolidBrush(Color.Brown), y * d + 1, x * d + 1, d - 1, d - 1);
            }

        }

        private void buttonDomain_Click(object sender, EventArgs e)
        {
            //select_mode = !select_mode;
            //Point first_angle = new Point(-1, -1);
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            changed_domain();
        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            changed_domain();
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            changed_domain();
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            changed_domain();
        }
        private void changed_domain()
        {
            refr(true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            refr(false);
        }

        private void Couvertures_FormClosing(object sender, FormClosingEventArgs e)
        {
            own.couver = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
