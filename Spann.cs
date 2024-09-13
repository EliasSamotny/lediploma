using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static l_application_pour_diploma.Classes;
using static System.Windows.Forms.DataFormats;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Color = System.Drawing.Color;
using ex = Microsoft.Office.Interop.Excel;

namespace l_application_pour_diploma{
    public partial class Spann : Form
    {
        private ex.Application? excelapp;
        private ex.Workbooks? excelappworkbooks;
        private ex.Workbook? excelappworkbook;
        private ex.Sheets? excelsheets;
        private ex.Worksheet? excelworksheet;
        private ex.Range? excelcells;
        List<Point[]> links; //for Fermat - detected, for Kruskal - all
        List<decimal> links_length, curr_links_length, curr_links_dist;
        List<List<Point>> forest; //contains trees
        Commencement own;
        List<Point> waved, rawPointList;
        decimal[,] wavessum;
        List<decimal[,]> waves;
        List<Point[,]> prevs;
        internal List<List<Point>> owingpoints;
        int d;
        List<Point[]> KruskalEdges;
        List<Point> voisins = new List<Point>() {
            new(- 1, - 1), new( 0, - 1), new(1, - 1), new(1, 0),   new(1, 1),   new(0, 1),   new(- 1, 1),   new(- 1, 0),
            new(- 1, - 2), new( 1, - 2), new(2, - 1), new(2, 1),   new(1, 2),   new(- 1, 2), new(- 2, 1),   new(- 2, - 1),
            new(- 2, - 3), new(- 1,- 3), new(1, - 3), new(2, - 3), new(3, - 2), new(3, - 1), new(3, 1),     new(3, 2),
            new(  2,   3), new( 1, 3  ), new(- 1, 3), new(- 2, 3), new(- 3, 2), new(- 3, 1), new(- 3, - 1), new(- 3, - 2)};
        internal List<Point> curr_points;
        List<Point> points;
        private int size_rayon;
        List<List<Point>> routes, curr_routes;
        List<decimal> zero_length;

        public Spann(Commencement o)
        {
            InitializeComponent();
            own = o;
            curr_points = new();
            refr(true);
            comboBox1.SelectedIndex = 2;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.SelectedIndex = 0;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox5.SelectedIndex = 1;
            comboBox5.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        internal void refr(bool nouv)
        {
            Bitmap bmp = new(pictureBox1.Width, pictureBox1.Height);
            Graphics carte = Graphics.FromImage(bmp);
            d = Math.Min(pictureBox1.Width, pictureBox1.Height) / Math.Max((int)own.numericUpDown1.Value, (int)own.numericUpDown2.Value);
            for (int i = 0; i < (int)own.numericUpDown1.Value; i++)
            {
                for (int j = 0; j < (int)own.numericUpDown2.Value; j++)
                {
                    carte.DrawRectangle(new Pen(Color.Black), j * d, i * d, d, d);
                    carte.FillRectangle(new SolidBrush(own.dataGridView1.Rows[i].Cells[j].Style.BackColor), j * d + 1, i * d + 1, d - 1, d - 1);
                }

            }
            if (nouv){
                waves = new();
                links = new();
                waved = new();
                links_length = new();
                label1.Text = "0";
                routes = new();
                curr_links_length = new();
                curr_routes = new();
                if (comboBox2.SelectedIndex == 0) {
                    label1.Text = Convert.ToString(0);
                    wavessum = new decimal[(int)own.numericUpDown1.Value, (int)own.numericUpDown2.Value];

                    List<decimal[,]> destins;
                    List<Point[,]> previos;
                    owingpoints = new();
                    List<Point> points = dataGridView1.Rows.Cast<DataGridViewRow>()
                                      .Select(row => new Point(Convert.ToInt32(row.Cells[0].Value) - 1, Convert.ToInt32(row.Cells[1].Value) - 1))
                                      .ToList();
                }
                else if (comboBox2.SelectedIndex == 1) {
                    KruskalEdges = new();
                    rawPointList = new();
                    prevs = new();
                    curr_links_dist = new();
                    if (dataGridView1.Rows.Count > 1)
                    {//waving et getting all edges
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {//from datagridview to list <Point>
                            rawPointList.Add(new(Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) - 1, Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) - 1));
                        }
                        
                        if (comboBox5.SelectedIndex == 0){// no main
                            for (int i = 0; i < dataGridView1.Rows.Count; i++){
                                Point currpoint = rawPointList[i];
                                waved.Add(currpoint);
                                var resultwaving = waving_in_domain_from_point(currpoint);
                                var wave = resultwaving.destinl;
                                var prev = resultwaving.previosl;
                                prevs.Add(prev);
                                for (int j = i + 1; j < dataGridView1.Rows.Count; j++) {
                                    var link = new Point[2];
                                    link[0] = rawPointList[i];
                                    link[1] = rawPointList[j];
                                    links.Add(link);
                                    links_length.Add(wave[rawPointList[j].X, rawPointList[j].Y]);
                                    List<Point> route = new List<Point> { link[1] };
                                    while (route[^1] != link[0])
                                    {
                                        int xl = route[^1].X, yl = route[^1].Y;
                                        route.Add(prev[xl, yl]);

                                    }
                                    route.Reverse();
                                    routes.Add(route);
                                }
                                var sortedData = links
                                     .Select((l, i) => new { Link = l, Length = links_length[i], route = routes[i] })
                                     .OrderBy(x => x.Length)
                                     .ToList();

                                // Выделяем отсортированные списки в отдельные переменные
                                links = sortedData.Select(x => x.Link).ToList();
                                links_length = sortedData.Select(x => x.Length).ToList();
                                routes = sortedData.Select(x => x.route).ToList();
                            }

                        }
                        if (comboBox5.SelectedIndex == 1){//first is main
                            Point currpoint = rawPointList[0];
                            waved.Add(currpoint);
                            var resultwaving = waving_in_domain_from_point(currpoint);
                            var wave = resultwaving.destinl;
                            var prev = resultwaving.previosl;
                            prevs.Add(prev);
                            for (int j = 1; j < dataGridView1.Rows.Count; j++){
                                var link = new Point[2];
                                link[0] = rawPointList[0];
                                link[1] = rawPointList[j];
                                links.Add(link);
                                links_length.Add(wave[rawPointList[j].X, rawPointList[j].Y]);
                                List<Point> route = new List<Point> { link[1] };
                                while (route[^1] != link[0]){
                                    int xl = route[^1].X, yl = route[^1].Y;
                                    route.Add(prev[xl, yl]);
                                }
                                route.Reverse();
                                routes.Add(route);
                            }

                            var sortedData = links
                                     .Select((l, i) => new { Link = l, Length = links_length[i], route = routes[i] })
                                     .OrderBy(x => x.Length)
                                     .ToList();

                            // Выделяем отсортированные списки в отдельные переменные
                            links = sortedData.Select(x => x.Link).ToList();
                            links_length = sortedData.Select(x => x.Length).ToList();
                            routes = sortedData.Select(x => x.route).ToList();

                        }

                        
                        forest = new();
                    }
                }
            }


            if (dataGridView1.Rows.Count > 0)
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    int x = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) - 1;
                    int y = Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) - 1;
                    carte.FillRectangle(new SolidBrush(Color.Brown), y * d + 1, x * d + 1, d - 1, d - 1);
                }
                if (!nouv && links.Count > 0)
                {//to redraw links 
                    foreach (var new_link in links)
                    {
                        Point[] p1i = new Point[] {
                            new (new_link[0].Y * d + d / 2 - 1, new_link[0].X * d + d / 2),
                            new (new_link[0].Y * d + d / 2 - 1, new_link[0].X * d + 1 + d / 2),
                            new (new_link[0].Y * d + d / 2 - 1, new_link[0].X * d + 2 + d / 2),
                            new (new_link[0].Y * d + d / 2, new_link[0].X * d + d / 2),
                            new (new_link[0].Y * d + d / 2, new_link[0].X * d + 1 + d / 2),
                            new (new_link[0].Y * d + d / 2, new_link[0].X * d + 2 + d / 2),
                            new (new_link[0].Y * d + d / 2 + 1, new_link[0].X * d + d / 2),
                            new (new_link[0].Y * d + d / 2 + 1, new_link[0].X * d + 1 + d / 2),
                            new (new_link[0].Y * d + d / 2 + 1, new_link[0].X * d + 2 + d / 2)
                        };
                        Point[] p2i = new Point[] {
                            new(new_link[1].Y * d + d/2 - 1, new_link[1].X * d + d / 2),
                            new(new_link[1].Y * d + d/2 - 1, new_link[1].X * d + 1 + d / 2),
                            new(new_link[1].Y * d + d/2 - 1, new_link[1].X * d + 2 + d / 2),
                            new(new_link[1].Y * d + d/2, new_link[1].X * d + d / 2),
                            new(new_link[1].Y * d + d/2, new_link[1].X * d + 1 + d / 2),
                            new(new_link[1].Y * d + d/2, new_link[1].X * d + 2 + d / 2),
                            new(new_link[1].Y * d + d/2 + 1, new_link[1].X * d + d / 2),
                            new(new_link[1].Y * d + d/2 + 1, new_link[1].X * d + 1 + d / 2),
                            new(new_link[1].Y * d + d/2 + 1, new_link[1].X * d + 2 + d / 2)
                        };
                        for (int j = 0; j < p1i.Length; j++)
                        {
                            carte.DrawLine(new Pen(Color.Black), p1i[j], p2i[j]);
                        }

                    }


                }
            }
            pictureBox1.Image = bmp;

        }
        public void build_tree_finis()
        {
            while (waved.Count < dataGridView1.RowCount)
                build_tree_step();
        }
        public void build_tree_step() {
            if (dataGridView1.Rows.Count > 0){
                Bitmap bmp = new(pictureBox1.Image);
                Graphics carte = Graphics.FromImage(bmp);
                d = Math.Min(pictureBox1.Width, pictureBox1.Height) / Math.Max((int)own.numericUpDown1.Value, (int)own.numericUpDown2.Value);
                if (waved.Count == 0){
                    Point point = new(Convert.ToInt32(dataGridView1.Rows[0].Cells[0].Value) - 1, Convert.ToInt32(dataGridView1.Rows[0].Cells[1].Value) - 1);
                    waved.Add(point);
                    var resultwaving = waving_in_domain_from_point(point);
                    var wave = resultwaving.destinl;
                    waves.Add(wave);
                    Parallel.For(0, wavessum.GetLength(0), i => {
                        for (int j = 0; j < wavessum.GetLength(1); j++)
                        {
                            wavessum[i, j] = wave[i, j];
                        }
                    });
                    int x = Convert.ToInt32(dataGridView1.Rows[0].Cells[0].Value) - 1;
                    int y = Convert.ToInt32(dataGridView1.Rows[0].Cells[1].Value) - 1;
                    carte.FillRectangle(new SolidBrush(Color.Chocolate), y * d + 1, x * d + 1, d - 1, d - 1);
                }
                else{
                    List<Point> points = dataGridView1.Rows.Cast<DataGridViewRow>()
                                  .Select(row => new Point(Convert.ToInt32(row.Cells[0].Value) - 1, Convert.ToInt32(row.Cells[1].Value) - 1))
                                  .ToList();
                    List<Point> unvisited = points.Except(waved).ToList();
                    if (unvisited.Count > 0){
                        Point next = unvisited.Where(p => wavessum[p.X, p.Y] > 0)
                                .OrderBy(p => wavessum[p.X, p.Y])
                                .DefaultIfEmpty(unvisited[0])
                                .FirstOrDefault();
                        waved.Add(next);
                        var resultwaving = waving_in_domain_from_point(next);
                        var wave = resultwaving.destinl;
                        var prev = resultwaving.previosl;
                        waves.Add(wave);
                        Parallel.For(0, wavessum.GetLength(0), i => {
                            for (int j = 0; j < wavessum.GetLength(1); j++){
                                wavessum[i, j] += wave[i, j];
                            }

                        });
                        Point[] new_link = new Point[2];

                        decimal min = waves[0][next.X, next.Y];//wavessum
                        Point neartonext = waved[0];
                        for (int i = 1; i < waves.Count - 1; i++){
                            if (min > waves[i][next.X, next.Y] && waves[i][next.X, next.Y] > 0){
                                min = waves[i][next.X, next.Y];
                                neartonext = waved[i];
                            }
                        }
                        new_link[0] = neartonext;
                        new_link[1] = next;
                        links_length.Add(min);
                        label1.Text = Convert.ToString(Convert.ToDecimal(label1.Text) + min);
                        links.Add(new_link);
                        List<Point> route = new List<Point> { neartonext };
                        while (route[^1] != next){
                            int xl = route[^1].X, yl = route[^1].Y;
                            route.Add(prev[xl, yl]);

                        }
                        routes.Add(route);
                        for (int i = 0; i < route.Count - 1; i++)
                        {
                            Point[] p1i = new Point[] {
                                    //new (new_link[0].Y * d + d / 2 - 1, new_link[0].X * d + d / 2),
                                    //new (new_link[0].Y * d + d / 2 - 1, new_link[0].X * d + 1 + d / 2),
                                    //new (new_link[0].Y * d + d / 2 - 1, new_link[0].X * d + 2 + d / 2),
                                    new (route[i].Y * d + d / 2,     route[i].X * d + d / 2),
                                    new (route[i].Y * d + d / 2,     route[i].X * d + 1 + d / 2),
                                    //new (new_link[0].Y * d + d / 2,     new_link[0].X * d + 2 + d / 2),
                                    new (route[i].Y * d + d / 2 + 1, route[i].X * d + d / 2),
                                    new (route[i].Y * d + d / 2 + 1, route[i].X * d + 1 + d / 2),
                                    new (route[i].Y * d + d / 2 + 1, route[i].X * d + 2 + d / 2)
                                };
                            Point[] p2i = new Point[] {
                                    //new(new_link[1].Y * d + d/2 - 1, new_link[1].X * d + d / 2),
                                    //new(new_link[1].Y * d + d/2 - 1, new_link[1].X * d + 1 + d / 2),
                                    //new(new_link[1].Y * d + d/2 - 1, new_link[1].X * d + 2 + d / 2),
                                    new(route[i + 1].Y * d + d/2,     route[i + 1].X * d + d / 2),
                                    new(route[i + 1].Y * d + d/2,     route[i + 1].X * d + 1 + d / 2),
                                    //new(new_link[1].Y * d + d/2,     new_link[1].X * d + 2 + d / 2),
                                    new(route[i + 1].Y * d + d/2 + 1, route[i + 1].X * d + d / 2),
                                    new(route[i + 1].Y * d + d/2 + 1, route[i + 1].X * d + 1 + d / 2),
                                    new(route[i + 1].Y * d + d/2 + 1, route[i + 1].X * d + 2 + d / 2)
                                };
                            for (int j = 0; j < 4/*p1i.Length*/; j++)
                            {
                                carte.DrawLine(new Pen(Color.Black), p1i[j], p2i[j]);
                            }
                        }
                    }
                }
                pictureBox1.Image = bmp;
            }
            //else MessageBox.Show();

        }
        private void button6_Click(object sender, EventArgs e)
        {
            bool toadd = true;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == (int)numericUpDown3.Value && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == (int)numericUpDown4.Value)
                {
                    toadd = false;
                    break;
                }
            }
            if (toadd)
                dataGridView1.Rows.Add(new object[] { numericUpDown3.Value, numericUpDown4.Value });
            refr(true);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                for (int i = dataGridView1.SelectedRows.Count - 1; i >= 0; i--)
                {
                    dataGridView1.Rows.Remove(dataGridView1.SelectedRows[i]);
                }
            }
            refr(true);
        }
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.Y;
            int y = e.X;
            numericUpDown3.Value = Math.Min(x / d + 1, (int)own.numericUpDown1.Value);
            numericUpDown4.Value = Math.Min(y / d + 1, (int)own.numericUpDown2.Value);
        }

        private void Spann_FormClosing(object sender, FormClosingEventArgs e) { own.span = null; }
        private void pictureBox1_Resize(object sender, EventArgs e) { refr(false); }
        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int x = Math.Min(e.Y / d + 1, (int)own.numericUpDown1.Value);
            int y = Math.Min(e.X / d + 1, (int)own.numericUpDown2.Value);
            bool toadd = true;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == x && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == y)
                {
                    toadd = false;
                    dataGridView1.Rows.RemoveAt(i);
                    break;
                }
            }
            if (toadd)
                dataGridView1.Rows.Add(new object[] { x, y });
            refr(true);
        }
        private (decimal[,] destinl, Point[,] previosl) waving_in_domain_from_point(Point commenc, decimal startadd = 0)
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
                    if (own.source.All(el => el[i, j] > 0))
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
                Parallel.For(0, size_rayon, h => { calculcell(ref accessl, ref destinl, ref previosl, xlc1 + voisins[h].X, ylc1 + voisins[h].Y, xlc1, ylc1); });
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
        private void calculcell(ref bool[,] access, ref decimal[,] dest, ref Point[,] prev, int xc, int yc, int xl, int yl)
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

        private void reduce_links() {
            Bitmap bmp = new(pictureBox1.Image);
            Graphics carte = Graphics.FromImage(bmp);
            d = Math.Min(pictureBox1.Width, pictureBox1.Height) / Math.Max((int)own.numericUpDown1.Value, (int)own.numericUpDown2.Value);
            if (comboBox5.SelectedIndex == 0) {
                if (KruskalEdges.Count == 0){
                    KruskalEdges.Add(links[0]);
                    curr_links_length.Add(links_length[0]);
                    curr_routes.Add(routes[0]);
                    List<Point> tree = new() { links[0][0], links[0][1] };
                    forest.Add(new(tree));
                    links.RemoveAt(0);
                    //links_length.RemoveAt(0);
                    //routes.RemoveAt(0);
                    List<Point> route = new List<Point> { tree[0] };
                    int ind = waved.IndexOf(tree[1]);
                    while (route[^1] != tree[1]){
                        int xl = route[^1].X, yl = route[^1].Y;
                        route.Add(prevs[ind][xl, yl]);

                    }
                    for (int i = 0; i < route.Count - 1; i++){
                        Point[] p1i = new Point[] {
                                    //new (new_link[0].Y * d + d / 2 - 1, new_link[0].X * d + d / 2),
                                    //new (new_link[0].Y * d + d / 2 - 1, new_link[0].X * d + 1 + d / 2),
                                    //new (new_link[0].Y * d + d / 2 - 1, new_link[0].X * d + 2 + d / 2),
                                    new (route[i].Y * d + d / 2,     route[i].X * d + d / 2),
                                    new (route[i].Y * d + d / 2,     route[i].X * d + 1 + d / 2),
                                    //new (new_link[0].Y * d + d / 2,     new_link[0].X * d + 2 + d / 2),
                                    new (route[i].Y * d + d / 2 + 1, route[i].X * d + d / 2),
                                    new (route[i].Y * d + d / 2 + 1, route[i].X * d + 1 + d / 2),
                                    new (route[i].Y * d + d / 2 + 1, route[i].X * d + 2 + d / 2)
                                };
                        Point[] p2i = new Point[] {
                                    //new(new_link[1].Y * d + d/2 - 1, new_link[1].X * d + d / 2),
                                    //new(new_link[1].Y * d + d/2 - 1, new_link[1].X * d + 1 + d / 2),
                                    //new(new_link[1].Y * d + d/2 - 1, new_link[1].X * d + 2 + d / 2),
                                    new(route[i + 1].Y * d + d/2,     route[i + 1].X * d + d / 2),
                                    new(route[i + 1].Y * d + d/2,     route[i + 1].X * d + 1 + d / 2),
                                    //new(new_link[1].Y * d + d/2,     new_link[1].X * d + 2 + d / 2),
                                    new(route[i + 1].Y * d + d/2 + 1, route[i + 1].X * d + d / 2),
                                    new(route[i + 1].Y * d + d/2 + 1, route[i + 1].X * d + 1 + d / 2),
                                    new(route[i + 1].Y * d + d/2 + 1, route[i + 1].X * d + 2 + d / 2)
                                };
                        for (int j = 0; j < 4/*p1i.Length*/; j++){ carte.DrawLine(new Pen(Color.Black), p1i[j], p2i[j]); }
                    }
                    label1.Text = Convert.ToString(Convert.ToDecimal(label1.Text) + links_length[0]);
                    links_length.RemoveAt(0);
                    routes.RemoveAt(0);
                }
                else
                {
                    if (KruskalEdges.Count + 1 < dataGridView1.Rows.Count)
                    {
                        List<Point> link = new() { links[0][0], links[0][1] };
                        while (check_edge_vertices(link[0], link[1]) == 0)
                        { // if in the same tree
                            links.RemoveAt(0);
                            links_length.RemoveAt(0);
                            routes.RemoveAt(0);
                            link = new() { links[0][0], links[0][1] };
                            //if (links.Count > 0) reduce_links();
                        }
                        //link = new() { links[0][0], links[0][1] };
                        if (check_edge_vertices(link[0], link[1]) == 1)
                        { // first in a tree, other - not
                            KruskalEdges.Add(links[0]);
                            curr_links_length.Add(links_length[0]);
                            curr_routes.Add(routes[0]);
                            forest[forest.FindIndex(f => f.Contains(link[0]))].Add(link[1]);
                        }
                        if (check_edge_vertices(link[0], link[1]) == 2)
                        { // second in a tree, other - not
                            KruskalEdges.Add(links[0]);
                            curr_links_length.Add(links_length[0]);
                            curr_routes.Add(routes[0]);
                            forest[forest.FindIndex(f => f.Contains(link[1]))].Add(link[0]);
                        }
                        if (check_edge_vertices(link[0], link[1]) == 3)
                        { // tree to create
                            KruskalEdges.Add(links[0]);
                            curr_links_length.Add(links_length[0]);
                            curr_routes.Add(routes[0]);
                            forest.Add(new(link));
                        }
                        if (check_edge_vertices(link[0], link[1]) == 4)
                        { // merging trees
                            KruskalEdges.Add(links[0]);
                            curr_links_length.Add(links_length[0]);
                            curr_routes.Add(routes[0]);
                            int ind0 = forest.FindIndex(f => f.Contains(link[0]));
                            int ind1 = forest.FindIndex(f => f.Contains(link[1]));
                            foreach (var el in forest[ind1])
                                forest[ind0].Add(el);
                            forest.RemoveAt(ind1);
                        }

                        List<Point> route = new List<Point> { link[0] };
                        int ind = waved.IndexOf(link[1]);
                        while (route[^1] != link[1])
                        {
                            int xl = route[^1].X, yl = route[^1].Y;
                            route.Add(prevs[ind][xl, yl]);
                        }
                        for (int i = 0; i < route.Count - 1; i++)
                        {
                            Point[] p1i = new Point[] {
                                    //new (new_link[0].Y * d + d / 2 - 1, new_link[0].X * d + d / 2),
                                    //new (new_link[0].Y * d + d / 2 - 1, new_link[0].X * d + 1 + d / 2),
                                    //new (new_link[0].Y * d + d / 2 - 1, new_link[0].X * d + 2 + d / 2),
                                    new (route[i].Y * d + d / 2,     route[i].X * d + d / 2),
                                    new (route[i].Y * d + d / 2,     route[i].X * d + 1 + d / 2),
                                    //new (new_link[0].Y * d + d / 2,     new_link[0].X * d + 2 + d / 2),
                                    new (route[i].Y * d + d / 2 + 1, route[i].X * d + d / 2),
                                    new (route[i].Y * d + d / 2 + 1, route[i].X * d + 1 + d / 2),
                                    new (route[i].Y * d + d / 2 + 1, route[i].X * d + 2 + d / 2)
                                };
                            Point[] p2i = new Point[] {
                                    //new(new_link[1].Y * d + d/2 - 1, new_link[1].X * d + d / 2),
                                    //new(new_link[1].Y * d + d/2 - 1, new_link[1].X * d + 1 + d / 2),
                                    //new(new_link[1].Y * d + d/2 - 1, new_link[1].X * d + 2 + d / 2),
                                    new(route[i + 1].Y * d + d/2,     route[i + 1].X * d + d / 2),
                                    new(route[i + 1].Y * d + d/2,     route[i + 1].X * d + 1 + d / 2),
                                    //new(new_link[1].Y * d + d/2,     new_link[1].X * d + 2 + d / 2),
                                    new(route[i + 1].Y * d + d/2 + 1, route[i + 1].X * d + d / 2),
                                    new(route[i + 1].Y * d + d/2 + 1, route[i + 1].X * d + 1 + d / 2),
                                    new(route[i + 1].Y * d + d/2 + 1, route[i + 1].X * d + 2 + d / 2)
                                };
                            for (int j = 0; j < 4/*p1i.Length*/; j++)
                            {
                                carte.DrawLine(new Pen(Color.Black), p1i[j], p2i[j]);
                            }
                        }
                        label1.Text = Convert.ToString(Convert.ToDecimal(label1.Text) + links_length[0]);


                        links.RemoveAt(0);
                        links_length.RemoveAt(0);
                        routes.RemoveAt(0);
                    }
                }
            }
            else if (comboBox5.SelectedIndex == 1) {
                if (KruskalEdges.Count + 1 < dataGridView1.Rows.Count){ //when tree is not complete
                    List<Point> link = new() { links[0][0], links[0][1] };
                    while (check_edge_vertices(link[0], link[1]) == 0){ // if in the same tree, just skip aand delete
                        links.RemoveAt(0);
                        links_length.RemoveAt(0);
                        routes.RemoveAt(0);
                        link = new() { links[0][0], links[0][1] };
                    }
                    //link = new() { links[0][0], links[0][1] };
                    if (check_edge_vertices(link[0], link[1]) == 3){ // tree to create
                        KruskalEdges.Add(links[0]);
                        curr_links_length.Add(links_length[0]);
                        curr_links_dist.Add(curr_links_dist_find(route_on_tree(link[1])));
                        curr_routes.Add(routes[0]);
                        forest.Add(new(link));
                    }
                    if (check_edge_vertices(link[0], link[1]) == 1){ // first in a tree, other - not
                        KruskalEdges.Add(links[0]);
                        curr_links_length.Add(links_length[0]);
                        curr_links_dist.Add(curr_links_dist_find(route_on_tree(link[1])));
                        curr_routes.Add(routes[0]);
                        forest[0].Add(link[1]);
                    }
                    var route = curr_routes[^1];
                        for (int i = 0; i < route.Count - 1; i++){
                            Point[] p1i = new Point[] {
                                    //new (new_link[0].Y * d + d / 2 - 1, new_link[0].X * d + d / 2),
                                    //new (new_link[0].Y * d + d / 2 - 1, new_link[0].X * d + 1 + d / 2),
                                    //new (new_link[0].Y * d + d / 2 - 1, new_link[0].X * d + 2 + d / 2),
                                    new (route[i].Y * d + d / 2,     route[i].X * d + d / 2),
                                    new (route[i].Y * d + d / 2,     route[i].X * d + 1 + d / 2),
                                    //new (new_link[0].Y * d + d / 2,     new_link[0].X * d + 2 + d / 2),
                                    new (route[i].Y * d + d / 2 + 1, route[i].X * d + d / 2),
                                    new (route[i].Y * d + d / 2 + 1, route[i].X * d + 1 + d / 2),
                                    new (route[i].Y * d + d / 2 + 1, route[i].X * d + 2 + d / 2)
                                };
                            Point[] p2i = new Point[] {
                                    //new(new_link[1].Y * d + d/2 - 1, new_link[1].X * d + d / 2),
                                    //new(new_link[1].Y * d + d/2 - 1, new_link[1].X * d + 1 + d / 2),
                                    //new(new_link[1].Y * d + d/2 - 1, new_link[1].X * d + 2 + d / 2),
                                    new(route[i + 1].Y * d + d/2,     route[i + 1].X * d + d / 2),
                                    new(route[i + 1].Y * d + d/2,     route[i + 1].X * d + 1 + d / 2),
                                    //new(new_link[1].Y * d + d/2,     new_link[1].X * d + 2 + d / 2),
                                    new(route[i + 1].Y * d + d/2 + 1, route[i + 1].X * d + d / 2),
                                    new(route[i + 1].Y * d + d/2 + 1, route[i + 1].X * d + 1 + d / 2),
                                    new(route[i + 1].Y * d + d/2 + 1, route[i + 1].X * d + 2 + d / 2)
                                };
                            for (int j = 0; j < 4/*p1i.Length*/; j++){
                                carte.DrawLine(new Pen(Color.Black), p1i[j], p2i[j]);
                            }
                        }
                    
                    /*if (check_edge_vertices(link[0], link[1]) == 2)
                    { // second in a tree, other - not
                        KruskalEdges.Add(links[0]);
                        curr_links_length.Add(links_length[0]);
                        curr_routes.Add(routes[0]);
                        forest[0].Add(link[0]);
                        curr_links_dist.Add(links_length[0]);
                    }*/

                    Point currpoint = link[1];
                    waved.Add(currpoint);
                    int addind = KruskalEdges.FindIndex(llink => llink[1] == currpoint);
                    var addition = curr_links_dist[addind];
                    var resultwaving = waving_in_domain_from_point(currpoint, addition);
                    var wave = resultwaving.destinl;
                    var realwave = resultwaving.destinl;
                    for (int a = 0; realwave.GetLength(0) > a; a++){
                        for (int b = 0; realwave.GetLength(1) > b; b++) {
                            realwave[a, b] -= addition;
                        }
                    }
                    var prev = resultwaving.previosl;
                    prevs.Add(prev);
                    for (int j = 1; j < dataGridView1.Rows.Count; j++){
                        var currlink = new Point[2];
                        currlink[0] = currpoint;
                        if (!forest[0].Contains(rawPointList[j])) {
                            currlink[1] = rawPointList[j];
                            links.Add(currlink);
                            links_length.Add(realwave[rawPointList[j].X, rawPointList[j].Y]);
                            var routen = new List<Point> { currlink[1] };
                            while (routen[^1] != currlink[0]){
                                int xl = routen[^1].X, yl = routen[^1].Y;
                                routen.Add(prev[xl, yl]);
                            }
                            routen.Reverse();
                            routes.Add(routen);
                        }
                    }
                    links.RemoveAt(0);
                    links_length.RemoveAt(0);
                    routes.RemoveAt(0);
                    var sortedData = links
                             .Select((l, i) => new { Link = l, Length = links_length[i], route = routes[i] })
                             .OrderBy(x => x.Length)
                             .ToList();
                    // Выделяем отсортированные списки в отдельные переменные
                    links = sortedData.Select(x => x.Link).ToList();
                    links_length = sortedData.Select(x => x.Length).ToList();
                    routes = sortedData.Select(x => x.route).ToList();


                    label1.Text = Convert.ToString(Convert.ToDecimal(label1.Text) + curr_links_length[^1]);
                }

            }

            pictureBox1.Image = bmp;
        }

        private decimal curr_links_dist_find(List<Point> route){
            decimal dist = 0;
            route.Remove(rawPointList[0]);
            for (int i = 0; i < route.Count; i++){
                dist += curr_links_length[KruskalEdges.FindIndex(line => line[1] == route[i])];
            }
            return dist;
        }

        private List<Point> route_on_tree(Point p1){
            List<Point> routet = new List<Point>{ p1 };
            while (routet[^1] != rawPointList[0]){
                var nextpoint = KruskalEdges.Find(edge => edge[1] == routet[^1])[0];
                if (nextpoint != default)
                    routet.Add(nextpoint);
                else { return new List<Point>(); }
            }
            return routet;
        }
        private List<Point> route_on_tree(Point p1, Point p2){
            List<Point> routet = new List<Point> { p1 };
            while (routet[^1] != p1) {
                var nextpoint = KruskalEdges.Find(edge => edge[1] == routet[^1])[0];
                if (nextpoint != default) { 
                    routet.Add(nextpoint);
                    if (nextpoint != rawPointList[0] && p1 != rawPointList[0]){
                        routet.Add(KruskalEdges.Find(edge => edge[0] == routet[^1])[1]);
                    }
                }
                
                else { return new List<Point>(); }
            }
            return routet;
        }

        private int check_edge_vertices(Point p1, Point p2)
        {// 0 - same, 1 - first in a tree, other - not, 2 - second in a tree, other - not, 3 - tree to create, 4 - merge trees
            //if (forest.FindIndex(f => f.Contains(p1)) == forest.FindIndex(f => f.Contains(p2))) return 0; //discard
            if (forest.Any(f => f.Contains(p1)) && !forest.Any(f => f.Contains(p2))) return 1; //add sec to first
            if (forest.Any(f => f.Contains(p2)) && !forest.Any(f => f.Contains(p1))) return 2; //add first to sec
            if (!forest.Any(f => f.Contains(p2)) && !forest.Any(f => f.Contains(p1))) return 3; //new tree
            if (forest.FindIndex(f => f.Contains(p1)) != forest.FindIndex(f => f.Contains(p2))) return 4; //merging
            return 0;//-1 ;
        }

        private void reduce_links_finis()
        {
            while (KruskalEdges.Count + 1 < dataGridView1.RowCount)
                reduce_links();
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
        private bool if_front(List<Point> set, Point point)
        {
            for (int i = 0; i < 8; i++)
                if (!set.Contains(new(point.X + voisins[i].X, point.Y + voisins[i].Y)))
                    return true;
            return false;
        }
        private bool checkallvis(bool[,] set)
        {
            for (int i = 0; i < (int)own.numericUpDown1.Value; i++)
                for (int j = 0; j < (int)own.numericUpDown2.Value; j++)
                    if (!set[i, j]) return false;
            return true;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0)
                build_tree_step();
            if (comboBox2.SelectedIndex == 1)
                reduce_links();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0) build_tree_finis();
            if (comboBox2.SelectedIndex == 1) reduce_links_finis();
            make_report();
        }
        public string? Filename;
        internal void make_report()
        {
            own.insert_log("Choosing file for saving report...", this);
            if (dataGridView1.Rows.Count > 1)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Table | *.xlsx| Table | *.xls| All files | *.*";
                saveFileDialog1.Title = "Saving table";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != "")
                {
                    own.insert_log("Saving media...", this);
                    Filename = saveFileDialog1.FileName;


                    excelapp = new ex.Application();
                    excelapp.SheetsInNewWorkbook = 1;
                    excelapp.Workbooks.Add(Type.Missing);
                    excelappworkbooks = excelapp.Workbooks;
                    excelappworkbook = excelappworkbooks[1];
                    excelsheets = excelappworkbook.Sheets;

                    excelworksheet = excelsheets[1];
                    excelworksheet.Name = $"MST report";
                    //List points
                    excelcells = excelworksheet.get_Range(numColu(1) + Convert.ToString(1), Type.Missing);
                    excelcells.set_Value(Type.Missing, String.Format("{0}", "Points"));
                    excelcells = excelworksheet.get_Range(numColu(1) + Convert.ToString(2), Type.Missing);
                    excelcells.set_Value(Type.Missing, String.Format("{0}", "#"));
                    excelcells = excelworksheet.get_Range(numColu(2) + Convert.ToString(2), Type.Missing);
                    excelcells.set_Value(Type.Missing, String.Format("{0}", "Coordinates"));
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        excelcells = excelworksheet.get_Range(numColu(1) + Convert.ToString(i + 3), Type.Missing);
                        excelcells.set_Value(Type.Missing, String.Format("{0}", i + 1));
                        excelcells = excelworksheet.get_Range(numColu(2) + Convert.ToString(i + 3), Type.Missing);
                        excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"({dataGridView1.Rows[i].Cells[0].Value}, {dataGridView1.Rows[i].Cells[1].Value})")));
                    }

                    excelcells = excelworksheet.get_Range(numColu(4) + Convert.ToString(1), Type.Missing);
                    excelcells.set_Value(Type.Missing, String.Format("{0}", "Edges"));
                    excelcells = excelworksheet.get_Range(numColu(4) + Convert.ToString(2), Type.Missing);
                    excelcells.set_Value(Type.Missing, String.Format("{0}", "Points"));
                    excelcells = excelworksheet.get_Range(numColu(5) + Convert.ToString(2), Type.Missing);
                    excelcells.set_Value(Type.Missing, String.Format("{0}", "Calculated destination"));
                    excelcells = excelworksheet.get_Range(numColu(6) + Convert.ToString(2), Type.Missing);
                    excelcells.set_Value(Type.Missing, String.Format("{0}", "Euclid destination"));
                    excelcells = excelworksheet.get_Range(numColu(7) + Convert.ToString(2), Type.Missing);
                    excelcells.set_Value(Type.Missing, String.Format("{0}", "Δ abs"));
                    excelcells = excelworksheet.get_Range(numColu(8) + Convert.ToString(2), Type.Missing);
                    excelcells.set_Value(Type.Missing, String.Format("{0}", "Δ rel, %"));


                    excelcells = excelworksheet.get_Range(numColu(11) + Convert.ToString(1), Type.Missing);
                    excelcells.set_Value(Type.Missing, String.Format("{0}", "Routes"));
                    List<decimal> euclid = new List<decimal>();

                    for (int k = 0; k < routes.Select(list => list.Count).Max(); k++)
                    { //routes points numbers init
                        excelcells = excelworksheet.get_Range(numColu(11 + k) + Convert.ToString(2), Type.Missing);
                        excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"{k + 1}")));
                    }
                    if (comboBox2.SelectedIndex == 0)
                    { //fermat
                        excelcells = excelworksheet.get_Range(numColu(5) + Convert.ToString(1), Type.Missing);
                        excelcells.set_Value(Type.Missing, String.Format("{0}", "Fermat"));
                        for (int i = 0; i < links.Count; i++)
                        {
                            excelcells = excelworksheet.get_Range(numColu(4) + Convert.ToString(i + 3), Type.Missing);
                            excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"({links[i][0].X + 1}, {links[i][0].Y + 1})")));//1 point
                            excelcells = excelworksheet.get_Range(numColu(5) + Convert.ToString(i + 3), Type.Missing);
                            excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"({links[i][1].X + 1}, {links[i][1].Y + 1})")));//2 point
                            excelcells = excelworksheet.get_Range(numColu(6) + Convert.ToString(i + 3), Type.Missing);//calc length
                            string val = Convert.ToString(links_length[i]).Replace(',', '.');
                            excelcells.set_Value(Type.Missing, String.Format("{0}", val));
                            excelcells = excelworksheet.get_Range(numColu(7) + Convert.ToString(i + 3), Type.Missing);//euclid dest
                            decimal valu = (own.source[(int)own.numericUpDown8.Value - 1][links[i][0].X, links[i][0].Y] + own.source[(int)own.numericUpDown8.Value - 1][links[i][1].X, links[i][1].Y]) / 2 *
                                (decimal)Math.Sqrt(Math.Pow(links[i][1].X - links[i][0].X, 2) + Math.Pow(links[i][1].Y - links[i][0].Y, 2));
                            euclid.Add(valu);
                            excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"{valu}").Replace(',', '.')));

                            excelcells = excelworksheet.get_Range(numColu(8) + Convert.ToString(3 + i), Type.Missing); //delta abs
                            excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"{links_length[i] - valu}").Replace(',', '.')));
                            excelcells = excelworksheet.get_Range(numColu(9) + Convert.ToString(i + 3), Type.Missing); //delta rel
                            excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"{(links_length[i] - valu) / links_length[i] * 100}%").Replace(',', '.')));

                            for (int k = 0; k < routes[i].Count; k++)
                            { //route constucting
                                excelcells = excelworksheet.get_Range(numColu(11 + k) + Convert.ToString(3 + i), Type.Missing);
                                excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"( {routes[i][k].X + 1} ,  {routes[i][k].Y + 1} )")));
                            }
                        }
                    }
                    if (comboBox2.SelectedIndex == 1)
                    { //kruskal
                        excelcells = excelworksheet.get_Range(numColu(5) + Convert.ToString(1), Type.Missing);
                        excelcells.set_Value(Type.Missing, String.Format("{0}", "Kruskal"));
                        for (int i = 0; i < KruskalEdges.Count; i++)
                        {
                            excelcells = excelworksheet.get_Range(numColu(4) + Convert.ToString(i + 3), Type.Missing);
                            excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"({KruskalEdges[i][0].X + 1}, {KruskalEdges[i][0].Y + 1}), ({KruskalEdges[i][1].X + 1}, {KruskalEdges[i][1].Y + 1})")));
                            excelcells = excelworksheet.get_Range(numColu(5) + Convert.ToString(i + 3), Type.Missing);
                            excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"{curr_links_length[i]}").Replace(',', '.')));
                            excelcells = excelworksheet.get_Range(numColu(6) + Convert.ToString(i + 3), Type.Missing);
                            decimal val = (own.source[(int)own.numericUpDown8.Value - 1][KruskalEdges[i][0].X, KruskalEdges[i][0].Y] + own.source[(int)own.numericUpDown8.Value - 1][KruskalEdges[i][1].X, KruskalEdges[i][1].Y]) / 2 *
                                (decimal)Math.Sqrt(Math.Pow(KruskalEdges[i][1].X - KruskalEdges[i][0].X, 2) + Math.Pow(KruskalEdges[i][1].Y - KruskalEdges[i][0].Y, 2));
                            euclid.Add(val);
                            excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"{val}").Replace(',', '.')));

                            excelcells = excelworksheet.get_Range(numColu(7) + Convert.ToString(3 + i), Type.Missing); //delta abs
                            excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"{curr_links_length[i] - val}").Replace(',', '.')));
                            excelcells = excelworksheet.get_Range(numColu(8) + Convert.ToString(i + 3), Type.Missing); //delta rel
                            excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"{(curr_links_length[i] - val) / links_length[i] * 100}").Replace(',', '.')));

                            for (int k = 0; k < curr_routes[i].Count; k++)
                            { //route constucting
                                excelcells = excelworksheet.get_Range(numColu(11 + k) + Convert.ToString(3 + i), Type.Missing);
                                excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"( {curr_routes[i][k].X + 1}, {curr_routes[i][k].Y + 1} )")));
                            }
                        }

                    }
                    excelcells = excelworksheet.get_Range(numColu(4) + Convert.ToString(dataGridView1.Rows.Count + 2), Type.Missing);
                    excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"Total")));
                    excelcells = excelworksheet.get_Range(numColu(5) + Convert.ToString(dataGridView1.Rows.Count + 2), Type.Missing);
                    excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"{curr_links_length.Sum()}").Replace(',', '.')));
                    excelcells = excelworksheet.get_Range(numColu(6) + Convert.ToString(dataGridView1.Rows.Count + 2), Type.Missing);
                    excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"{euclid.Sum()}").Replace(',', '.')));
                    excelcells = excelworksheet.get_Range(numColu(7) + Convert.ToString(dataGridView1.Rows.Count + 2), Type.Missing);
                    excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString($"{Math.Abs(curr_links_length.Sum()-euclid.Sum())}").Replace(',', '.')));
                    excelapp.DisplayAlerts = true;
                    excelappworkbook.SaveAs(Filename, Type.Missing, Type.Missing,
                   Type.Missing, Type.Missing, Type.Missing, ex.XlSaveAsAccessMode.xlNoChange,
                   Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    excelappworkbook.Close();
                    MessageBox.Show("Report saved.");
                    own.insert_log("Report saved.", this);
                }
                else MessageBox.Show("There should be chosen at least 2 points");
            }
        }
        private string numColu(int f){
            int div = f;
            string colLetter = String.Empty;
            int mod;
            while (div > 0){
                mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = ((div - mod) / 26);
            }
            return colLetter;
        }


        internal void ToRusse(){
            Text = "Остовное дерево";
            groupBox2.Text = "Кол-во направлений поиска";

            button1.Text = "Задать начальную точку";
            button3.Text = "Достроить дерево";
            button4.Text = "Шаговое";
            button5.Text = "Удалить";
            button6.Text = "Добавить";

            comboBox1.Items[2] = "III радиус (32 направления)";
            comboBox1.Items[1] = "II радиус (16 направления)";
            comboBox1.Items[0] = "I радиус (8 направлений)";

            Column1.HeaderText = "Строка";
            Column2.HeaderText = "Стоблец";

            groupBox4.Text = "Алгоритм";
            comboBox2.Items[1] = "Крускала";
            comboBox2.Items[0] = "Ферма";

            groupBox6.Text = "Политика маршрутизации (метод Крускала)";
            comboBox5.Items[0] = "Без главной точки";
            comboBox5.Items[1] = "Первая точка - начальная";
            groupBox1.Text = "Длина дерева";
        }
        internal void ToFrancais(){
            Text = "Un arbre couvrant";

            button1.Text = "Définir par défaut";
            button3.Text = "Finir les calculations";
            button4.Text = "Un pas";
            button5.Text = "Suppremer";
            button6.Text = "Aujouter";

            comboBox1.Items[0] = "III rayon (32 directions)";
            comboBox1.Items[1] = "II rayon (16 directions)";
            comboBox1.Items[2] = "I rayon (8 directions)";

            Column1.HeaderText = "La ligne";
            Column2.HeaderText = "La colonne";

            groupBox2.Text = "Le destin de chercher de chemins";
            groupBox4.Text = "L'algorythme";
            comboBox2.Items[1] = "Kruskal";
            comboBox2.Items[0] = "Fermat";

            groupBox6.Text = "Politique de routage (pour Krouskal)";
            comboBox5.Items[0] = "Pas de point principal";
            comboBox5.Items[1] = "Premier point est principal";
            groupBox1.Text = "Longueur d'arbre";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e){
            if (comboBox1.SelectedIndex == 0) size_rayon = 8;
            else if (comboBox1.SelectedIndex == 1) size_rayon = 16;
            else size_rayon = 32;
            refr(true);
        }

        private void button1_Click(object sender, EventArgs e){
            if (dataGridView1.SelectedRows.Count > 0){
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                // Remove the selected row
                dataGridView1.Rows.RemoveAt(selectedRow.Index);
                // Insert the selected row at the beginning (index 0)
                dataGridView1.Rows.Insert(0, selectedRow);
            }
            else if (dataGridView1.SelectedCells.Count > 0){
                DataGridViewRow selectedRow = dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex];
                // Remove the selected row
                dataGridView1.Rows.RemoveAt(selectedRow.Index);
                // Insert the selected row at the beginning (index 0)
                dataGridView1.Rows.Insert(0, selectedRow);
            }
            refr(true);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e){refr(true);}

        private void Spann_Load(object sender, EventArgs e){

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e){ refr(true);}
    }
}
