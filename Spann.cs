using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.VariantTypes;
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

namespace l_application_pour_diploma{
    public partial class Spann : Form{
        List<Point[]> links;
        Commencement own;
        List<Point> waved;
        decimal[,] wavessum;
        List<decimal[,]> waves;
        internal List<List<Point>> owingpoints;
        int d;
        List<Point> voisins = new List<Point>() {
            new(- 1, - 1), new( 0, - 1), new(1, - 1), new(1, 0),   new(1, 1),   new(0, 1),   new(- 1, 1),   new(- 1, 0),
            new(- 1, - 2), new( 1, - 2), new(2, - 1), new(2, 1),   new(1, 2),   new(- 1, 2), new(- 2, 1),   new(- 2, - 1),
            new(- 2, - 3), new(- 1,- 3), new(1, - 3), new(2, - 3), new(3, - 2), new(3, - 1), new(3, 1),     new(3, 2),
            new(  2,   3), new( 1, 3  ), new(- 1, 3), new(- 2, 3), new(- 3, 2), new(- 3, 1), new(- 3, - 1), new(- 3, - 2)};
        internal List<Point> curr_points;
        private int size_rayon;

        public Spann(Commencement o){
            InitializeComponent();
            own = o;
            curr_points = new();
            refr(true);
            comboBox1.SelectedIndex = 2;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        internal void refr(bool nouv){
            if (nouv) {
                waved = new();
                waves = new();
                wavessum = new decimal[(int)own.numericUpDown1.Value,(int)own.numericUpDown2.Value];
                links = new();
                List<decimal[,]> destins;
                List<Point[,]> previos;
                owingpoints = new();
                
            }
            
            Bitmap bmp = new(pictureBox1.Width, pictureBox1.Height);
            Graphics carte = Graphics.FromImage(bmp);
            d = Math.Min(pictureBox1.Width, pictureBox1.Height) / Math.Max((int)own.numericUpDown1.Value, (int)own.numericUpDown2.Value);
            for (int i = 0; i < (int)own.numericUpDown1.Value; i++)
            {
                for (int j = 0; j < (int)own.numericUpDown2.Value; j++)
                {
                    carte.DrawRectangle(new Pen(Color.Black), j * d, i * d, d, d);
                    carte.FillRectangle(new SolidBrush(Color.White), j * d + 1, i * d + 1, d - 1, d - 1);
                    //carte.FillRectangle(new SolidBrush(own.dataGridView1.Rows[i].Cells[j].Style.BackColor), j * d + 1, i * d + 1, d - 1, d - 1);
                }

            }
            if (dataGridView1.Rows.Count > 0){
                
                for (int i = 0; i < dataGridView1.RowCount; i++){
                    int x = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) - 1;
                    int y = Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) - 1;
                    carte.FillRectangle(new SolidBrush(Color.Brown), y * d + 1, x * d + 1, d - 1, d - 1);
                }
                if (!nouv && links.Count > 0) {//to redraw links 
                    foreach (var new_link in links){
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
        public void build_tree_finis() {
            while (waved.Count < dataGridView1.RowCount)
                build_tree_step();
        }
        public void build_tree_step(){
            
            
            if (dataGridView1.Rows.Count > 0){
                Bitmap bmp = new(pictureBox1.Image);
                Graphics carte = Graphics.FromImage(bmp);
                d = Math.Min(pictureBox1.Width, pictureBox1.Height) / Math.Max((int)own.numericUpDown1.Value, (int)own.numericUpDown2.Value);
                if (waved.Count == 0) {
                    Point point = new(Convert.ToInt32(dataGridView1.Rows[0].Cells[0].Value)-1, Convert.ToInt32(dataGridView1.Rows[0].Cells[1].Value)-1);
                    waved.Add(point);
                    var resultwaving = waving_in_domain_from_point(point);
                    var wave = resultwaving.destinl;
                    waves.Add(wave);
                    Parallel.For(0, wavessum.GetLength(0), i => {
                        for (int j = 0; j < wavessum.GetLength(1); j++){
                            wavessum[i, j] = wave[i, j];
                        }
                    });                    
                    int x = Convert.ToInt32(dataGridView1.Rows[0].Cells[0].Value) - 1;
                    int y = Convert.ToInt32(dataGridView1.Rows[0].Cells[1].Value) - 1;
                    carte.FillRectangle(new SolidBrush(Color.Chocolate), y * d + 1, x * d + 1, d - 1, d - 1);
                }
                else{
                    List<Point> points = dataGridView1.Rows.Cast<DataGridViewRow>()
                                  .Select(row => new Point(Convert.ToInt32(row.Cells[0].Value) - 1,Convert.ToInt32(row.Cells[1].Value) - 1))
                                  .ToList();
                    List<Point> unvisited = points.Except(waved).ToList();
                    if (unvisited.Count > 0) {
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
                        Point[] new_link = new Point [2];

                        decimal min = waves[0][next.X,next.Y];
                        Point neartonext = waved[0];
                        for (int i = 1; i < waves.Count; i++) {
                            if (min > waves[i][next.X, next.Y] && waves[i][next.X, next.Y] > 0)
                            {
                                min = waves[i][next.X, next.Y];
                                neartonext = waved[i];
                            }

                        }

                        new_link[0] = neartonext;
                        new_link[1] = next;
                        links.Add(new_link);
                        List<Point> route = new List<Point>{ neartonext };
                        while (route[^1] != next){
                            int xl = route[^1].X, yl = route[^1].Y;
                            route.Add(prev[xl,yl]);

                        }
                        
                        for (int i = 0; i < route.Count - 1; i++) {
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
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e){
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
        private (decimal[,] destinl, Point[,] previosl) waving_in_domain_from_point(Point commenc){
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
                    if (own.source.All(el => el[i,j] > 0 ))
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
            destinl[xlc, ylc] = 0;

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
        private void calculcell(ref bool[,] access, ref decimal[,] dest, ref Point[,] prev, int xc, int yc, int xl, int yl){
            if (xc >= 0 && yc >= 0 && xc < (int)own.numericUpDown1.Value && yc < (int)own.numericUpDown2.Value && access[xc, yc])
            {//if consists
                int currstage = currst(dest[xl, yl]);
                decimal cur = Convert.ToDecimal((Convert.ToDouble(own.source[currstage][xc, yc]) + Convert.ToDouble(own.source[currstage][xl, yl])) / 2 * Math.Sqrt(Math.Pow(xc - xl, 2) + Math.Pow(yc - yl, 2)) + Convert.ToDouble(dest[xl, yl]));
                if (currstage != currst(cur) && own.source.Count > 1){
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
        private int currst(decimal dest){
            int shift = own.stateShift;
            //own.transitions[]
            int currstage = 0;
            dest %= own.transitions.Sum();
            if (own.transitions.Count > 1)
                for (int i = 0; i < own.transitions.Count; i++){
                    if (dest >= own.transitions[(i + shift) % own.transitions.Count]){
                        dest -= own.transitions[(i + shift) % own.transitions.Count];
                    }
                    else{
                        currstage = (i + shift) % own.transitions.Count;
                        break;
                    }
                }
            return (currstage) % own.source.Count;
        }
        internal List<Point> get_frontiers(List<Point> set){
            List<Point> frontl = new();
            object lock_front = new();
            Parallel.ForEach(set, el =>{
                if (if_front(set, el)){
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
        private void button4_Click(object sender, EventArgs e) { build_tree_step(); }

        private void button3_Click(object sender, EventArgs e) { build_tree_finis(); }
        internal void ToRusse()
        {
            Text = "Остовное дерево";
            groupBox2.Text = "Кол-во направлений поиска";
            button5.Text = "Удалить";
            button6.Text = "Добавить";
            comboBox1.Items[2] = "III радиус (32 направления)";
            comboBox1.Items[1] = "II радиус (16 направления)";
            comboBox1.Items[0] = "I радиус (8 направлений)";

            Column1.HeaderText = "Строка";
            Column2.HeaderText = "Стоблец";
        }
        internal void ToFrancais()
        {
            Text = "Un arbre couvrant";

            button3.Text = "Finir les calculations";
            button4.Text = "Le pas";
            button5.Text = "Suppremer";
            button6.Text = "Aujouter";

            comboBox1.Items[0] = "III rayon (32 directions)";
            comboBox1.Items[1] = "II rayon (16 directions)";
            comboBox1.Items[2] = "I rayon (8 directions)";

            Column1.HeaderText = "La ligne";
            Column2.HeaderText = "La colonne";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0) size_rayon = 8;
            else if (comboBox1.SelectedIndex == 1) size_rayon = 16;
            else size_rayon = 32;
        }
    }
}
