using System.Runtime.ConstrainedExecution;
using System.Linq;
using static l_application_pour_diploma.Classes;
using Color = System.Drawing.Color;
using System.Windows.Forms;

namespace l_application_pour_diploma{
    public partial class Vran : Form{
        private Commencement own;
        private List<Color> ColeurList = new List<Color> { Color.Red, Color.Orange, Color.Yellow, Color.YellowGreen,
            Color.GreenYellow, Color.Green, Color.DarkGreen, Color.SkyBlue, Color.Cyan, Color.BlueViolet, //, Color.OrangeRed, Color.MediumBlue, Color.DarkBlue
            Color.Violet, Color.Silver, Color.Gold, Color.SeaGreen};
        List<decimal[,]> destins;
        internal List<decimal> minrads;
        internal List<Point[,]> previos;
        internal int x = 1, y = 1, x1 = 1, y1 = 1;
        internal bool[,] vis, vis1;
        internal List<Point> curr_points;
        internal Packs? forfait;
        internal List<decimal[,]> wave_de_points;
        internal List<List<Point>> owingpoints;
        internal List<List<Point>> submedia;
        bool onemedium = true;
        int curr_med = 0;
        internal List<decimal[,]> variants;
        public Vran(Commencement o){
            InitializeComponent();
            own = o;
            curr_points = new();
        }
        private bool availpoint(int u, int v) { return  own.source[u,v] >= 0; }
        private void Vran_Load(object sender, EventArgs e)
        {
            refr(true);
            reseachpoints(0, 0);
            dataGridView1.AutoResizeColumns();
        }
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            numericUpDown3.Value = Convert.ToDecimal(dataGridView2.SelectedCells[0].RowIndex) + 1;
            numericUpDown4.Value = Convert.ToDecimal(dataGridView2.SelectedCells[0].ColumnIndex) + 1;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            refr(false);
        }
        private decimal calculater_sum_pour_point(List<Point> medium, Point commenc)
        {
            decimal sum = 0;
            decimal[,] destinl = new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
            Point[,] previosl = new Point[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
            int xl = commenc.X; int xl1 = xl;
            int yl = commenc.Y; int yl1 = yl;

            vis = new bool[dataGridView2.RowCount, dataGridView2.ColumnCount];
            bool[,] accessl = new bool[dataGridView2.RowCount, dataGridView2.ColumnCount];

            for (int i = 0; i < own.dataGridView1.RowCount; i++)
                for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                {
                    if (medium.Contains(new(i, j)))
                    {
                        accessl[i, j] = true;
                        //vis1[i, j] = false;
                        destinl[i, j] = -2;
                        vis[i, j] = false;
                    }
                    else
                    {
                        accessl[i, j] = false;
                        //vis1[i, j] = true;
                        vis[i, j] = true;
                        destinl[i, j] = -1;
                        previosl[i, j] = new Point(-1, -1);
                    }
                }
            vis[xl, yl] = true;
            previosl[xl, yl] = new(-2, -2);
            destinl[xl, yl] = 0;

            while (!checkallvis(vis))
            {
                vis[xl1, yl1] = true;
                //ref bool[,] access, ref bool[,] vis, ref decimal[,] dest, ref Point[,] prev, int xc, int yc
                for (int h = 0; h < size_rayon(); h++)
                    calculcell(ref accessl, ref destinl, ref previosl, xl1 + voisins[h].X, yl1 + voisins[h].Y, xl1, yl1);

                //transmission le point actuel a le point le plus proche et minimum
                var frontiers = new List<Points>();
                for (int i = 0; i < own.dataGridView1.RowCount; i++)//searching frontier points
                    for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                        if (destinl[i, j] != -2 && !vis[i, j])
                            frontiers.Add(new Points(i, j, destinl[i, j]));
                decimal mindest = Decimal.MaxValue;
                foreach (var p in frontiers)//searching the minimum points
                    if (p.dest < mindest)
                        mindest = p.dest;
                var points = new List<Points>();
                foreach (var p in frontiers)
                    if (p.dest == mindest)
                        points.Add(new Points(p.xl, p.yl, (decimal)Math.Sqrt(Math.Pow(xl1 - p.xl, 2) + Math.Pow(yl1 - p.yl, 2))));

                if (points.Count == 1) { xl1 = points[0].xl; yl1 = points[0].yl; }//if it is alone to choose, equal et continue
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
                    if (points1.Count == 1) { xl1 = points1[0].xl; yl1 = points1[0].yl; }//if it is alone to choose, equal et continue
                    else if (points1.Count > 1)
                    {//if not, choosing with least x
                        int minx = own.dataGridView1.RowCount;
                        foreach (var p in points1)
                            if (p.xl < minx)
                                minx = p.xl;
                        foreach (var p in points1)
                            if (p.xl == minx)
                            {
                                xl1 = points1[0].xl;
                                yl1 = points1[0].yl;
                                break;
                            }
                    }
                }
            }

            foreach (var el in medium)
            {
                sum += destinl[el.X, el.Y];
            }
            return sum;
        }
        private int determ_submed(Point p)
        {
            for (int i = 0; i < submedia.Count; i++)
                if (submedia[i].Contains(p))
                    return i;
            return 0;
        }
        internal void refr(bool changed_med){
            wave_de_points = new();
            variants = new() { own.source};
            if (own.radioButton1.Checked && own.checkBox4.Checked) {
                decimal[,] inverseMatrix = new decimal[own.source.GetLength(0), own.source.GetLength(1)];
                for (int i = 0; i < own.source.GetLength(0); i++)
                    for (int j = 0; j < own.source.GetLength(1); j++)
                        if (!own.checkBox2.Checked && own.source[i, j] > 0 || own.checkBox2.Checked && own.domains.Any(l => l.Contains(new(i, j))))
                            inverseMatrix[i, j] = 1 / own.source[i, j];
               
                variants.Add(inverseMatrix) ;
            }
            else if ((own.radioButton2.Checked || own.radioButton3.Checked) && own.checkBox4.Checked) {
                decimal multi;
                if (own.radioButton2.Checked)
                    multi = (100 - own.numericUpDown9.Value) / 100;
                else
                    multi = (100 + own.numericUpDown9.Value) / 100;
                for (int t = 0; t < (int)own.numericUpDown8.Value; t++) {

                    decimal[,] next = new decimal[own.source.GetLength(0), own.source.GetLength(1)];
                    for (int i = 0; i < own.source.GetLength(0); i++)
                        for (int j = 0; j < own.source.GetLength(1); j++)
                            if (!own.checkBox2.Checked && own.source[i, j] > 0 || own.checkBox2.Checked && own.domains.Any(l => l.Contains(new(i, j))))
                                next[i, j] = variants[^1][i, j] * multi;
                            else next[i, j] = variants[^1][i, j];
                    variants.Add(next);
                }


                
            }


            dataGridView2.RowCount = own.dataGridView1.RowCount;
            dataGridView2.ColumnCount = own.dataGridView1.ColumnCount;
            for (int i = 0; i < dataGridView2.RowCount; i++)
                for (int j = 0; j < dataGridView2.ColumnCount; j++)
                {
                    dataGridView2.Rows[i].Cells[j].Value = own.source[i,j];
                    dataGridView2.Rows[i].Cells[j].Style.BackColor = Color.White;
                }
            dataGridView2.AutoResizeColumns();
            if (changed_med)
                reseachpoints(0, 0);
            if (dataGridView1.RowCount > 0)
            {
                if (curr_points.Count > 0 && curr_points.Count == dataGridView1.RowCount)
                {
                    dataGridView1.RowCount = 0;
                    foreach (var el in curr_points)
                        dataGridView1.Rows.Add(new object[] { (el.X + 1), (el.Y + 1) });
                }
                curr_points = new();
                List<int> randcol = new List<int>();
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
                destins = new();
                previos = new();

                Cursor.Current = Cursors.WaitCursor;
                for (int l = 0; l < dataGridView1.RowCount; l++)
                {
                    destins.Add(new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount]);
                    previos.Add(new Point[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount]);

                    x = Convert.ToInt32(dataGridView1.Rows[l].Cells[0].Value) - 1; x1 = x;
                    y = Convert.ToInt32(dataGridView1.Rows[l].Cells[1].Value) - 1; y1 = y;

                    vis = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    vis1 = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    for (int i = 0; i < own.dataGridView1.RowCount; i++)
                        for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                        {
                            vis1[i, j] = false;
                        }
                    vis1[x, y] = true;
                    curr_med = determ_submed(new(x, y));
                    previos[l][x, y] = new(-2, -2);
                    for (int i = 0; i < own.dataGridView1.RowCount; i++)
                        for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                        {
                            if (availpoint(i, j) && availpoint(x1, y1) && submedia[curr_med].Contains(new(i, j)))
                            {
                                destins[l][i, j] = -2;
                                vis[i, j] = false;
                            }
                            else
                            {
                                vis[i, j] = true;
                                destins[l][i, j] = -1;
                                previos[l][i, j] = new Point(-1, -1);
                            }
                        }
                    destins[l][x, y] = 0;
                    while (!checkallvis())
                    {
                        vis[x1, y1] = true;
                        for (int i = 0; i < size_rayon(); i++)
                            calculcell(l, x1 + voisins[i].X, y1 + voisins[i].Y);

                        //transmission le point actuel a le point le plus proche et minimum
                        var frontiers = new List<Points>();
                        for (int i = 0; i < own.dataGridView1.RowCount; i++)//searching frontier points
                            for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                                if (destins[l][i, j] != -2 && !vis[i, j])
                                    frontiers.Add(new Points(i, j, destins[l][i, j]));
                        decimal mindest = Decimal.MaxValue;
                        foreach (var p in frontiers)//searching the minimum points
                            if (p.dest < mindest)
                                mindest = p.dest;
                        var points = new List<Points>();
                        foreach (var p in frontiers)
                            if (p.dest == mindest)
                                points.Add(new Points(p.xl, p.yl, (decimal)Math.Sqrt(Math.Pow(x1 - p.xl, 2) + Math.Pow(y1 - p.yl, 2))));

                        if (points.Count == 1) { x1 = points[0].xl; y1 = points[0].yl; }//if it is alone to choose, equal et continue
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
                            if (points1.Count == 1) { x1 = points1[0].xl; y1 = points1[0].yl; }//if it is alone to choose, equal et continue
                            else if (points1.Count > 1)
                            {//if not, choosing with least x
                                int minx = own.dataGridView1.RowCount;
                                foreach (var p in points1)
                                    if (p.xl < minx)
                                        minx = p.xl;
                                foreach (var p in points1)
                                    if (p.xl == minx)
                                    {
                                        x1 = points1[0].xl;
                                        y1 = points1[0].yl;
                                        break;
                                    }
                            }
                        }
                    }
                }
                owingpoints = new List<List<Point>>(); //list of sets of chosen points
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    Point p = new Point(Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) - 1,
                            Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) - 1);
                    List<Point> lp = new List<Point> { p };
                    owingpoints.Add(lp);
                }
                //filling with a coleurs
                for (int i = 0; i < dataGridView2.RowCount; i++)
                    for (int j = 0; j < dataGridView2.ColumnCount; j++)
                    {
                        if ( own.source[i,j] <= 0)
                            dataGridView2.Rows[i].Cells[j].Style.BackColor = Color.Black;
                        else if (!ifacentrepoint(i, j))
                        {
                            int h = mindest(i, j);
                            dataGridView2.Rows[i].Cells[j].Style.BackColor = ColeurList[randcol[h % randcol.Count]];
                            owingpoints[h].Add(new Point(i, j));
                        }
                    }
                //var opersets = new List<List<Point>> (owingpoints);
                var opersets = new List<List<Point>>();
                foreach (var sub in owingpoints)
                {
                    List<Point> s = new();
                    foreach (var el in sub)
                    {
                        s.Add(el);
                    }
                    opersets.Add(s);
                }

                foreach (var sub in owingpoints)
                {//chosing the centre for each
                    var minsum = decimal.MaxValue;
                    Point centre = new();
                    foreach (var el in sub)
                    {
                        var currl = calculater_sum_pour_point(sub, el);
                        if (currl < minsum)
                        {
                            minsum = currl;
                            centre = el;
                        }
                    }
                    dataGridView2.Rows[centre.X].Cells[centre.Y].Style.BackColor = Color.DarkKhaki;
                    curr_points.Add(new(centre.X, centre.Y));
                }

                Cursor.Current = Cursors.Default;

                //waving from current centres to determine a min radius for each
                minrads = new();
                for (int t = 0; t < curr_points.Count; t++)
                {
                    var currfront = new List<Point>(get_frontiers(owingpoints[t]));
                    decimal[,] curr_values = new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    Point[,] previousl = new Point[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];

                    x = curr_points[t].X; x1 = x;
                    y = curr_points[t].Y; y1 = y;

                    vis1 = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    for (int i = 0; i < own.dataGridView1.RowCount; i++)
                        for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                        {
                            vis1[i, j] = !owingpoints[t].Contains(new Point(i, j));
                            curr_values[i, j] = -2;
                        }
                    vis1[x, y] = true;
                    previousl[x, y] = new(-2, -2);
                    curr_values[x, y] = 0;

                    while (!checkallvis(vis1))
                    {
                        for (int i = 0; i < size_rayon(); i++)
                            calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + voisins[i].X, y1 + voisins[i].Y);

                        //transmission le point actuel a le point le plus proche et minimum
                        var frontiers = new List<Points>();
                        for (int i = 0; i < own.dataGridView1.RowCount; i++)//searching frontier points
                            for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                                if (curr_values[i, j] != -2 && !vis1[i, j])
                                    frontiers.Add(new Points(i, j, curr_values[i, j]));
                        decimal mindest = Decimal.MaxValue;
                        foreach (var p in frontiers)//searching the minimum points
                            if (p.dest < mindest)
                                mindest = p.dest;
                        var points = new List<Points>();
                        foreach (var p in frontiers)
                            if (p.dest == mindest)
                                points.Add(new Points(p.xl, p.yl, (decimal)Math.Sqrt(Math.Pow(x1 - p.xl, 2) + Math.Pow(y1 - p.yl, 2))));

                        if (points.Count == 1) { x1 = points[0].xl; y1 = points[0].yl; }//if it is alone to choose, equal et continue
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
                            if (points1.Count == 1) { x1 = points1[0].xl; y1 = points1[0].yl; }//if it is alone to choose, equal et continue
                            else if (points1.Count > 1)
                            {//if not, choosing with least x
                                int minx = own.dataGridView1.RowCount;
                                foreach (var p in points1)
                                    if (p.xl < minx)
                                        minx = p.xl;
                                foreach (var p in points1)
                                    if (p.xl == minx)
                                    {
                                        x1 = points1[0].xl;
                                        y1 = points1[0].yl;
                                        break;
                                    }
                            }
                        }
                        vis1[x1, y1] = true;
                    }
                    wave_de_points.Add(curr_values);
                    decimal minv = curr_values[currfront[0].X, currfront[0].Y];
                    for (int u = 1; u < currfront.Count; u++)
                    {
                        if (minv > curr_values[currfront[u].X, currfront[u].Y])
                            minv = curr_values[currfront[u].X, currfront[u].Y];
                    }
                    string formater = "0.##", formate = "0"; ;
                    if (numericUpDown6.Value > 2)
                    {
                        for (int n = 2; n < numericUpDown6.Value; n++)
                            formater += "#";
                    }
                    if (numericUpDown6.Value > 0)
                    {
                        formate += ".";
                        for (int n = 2; n < numericUpDown6.Value; n++)
                            formate += "#";
                    }
                    minrads.Add(minv);
                    dataGridView2.Rows[x].Cells[y].Value =  own.source[x,y].ToString(formate) + "(" + minv.ToString(formater) + ")";
                    dataGridView2.AutoResizeColumns();
                    dataGridView2.AutoResizeRows();
                }

            }
            Focus();
            forfait?.renew();
        }
        internal List<Point> get_frontiers(List<Point> set)
        {
            List<Point> frontl = new();
            foreach (var el in set)
            {
                if (if_front(set, el))
                {
                    frontl.Add(el);
                }
            }
            return frontl;
        }
        private bool if_front(List<Point> set, Point point)
        {
            for (int i = 0; i < 8; i++)
                if (!set.Contains(new Point(point.X + voisins[i].X, point.Y + voisins[i].Y)))
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
        private int mindest(int x, int y){
            decimal minv = destins[0][x, y];
            int ind = 0;
            for (int i = 1; i < destins.Count; i++)
            {
                if (minv > destins[i][x, y])
                {
                    minv = destins[i][x, y];
                    ind = i;
                }
            }
            return ind;
        }
        private void reseachpoints(int u, int v)
        {
            submedia = new() { new() };
            submedia[0].Add(new(u, v));
            bool[,] known = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
            for (int i = 0; i < own.dataGridView1.RowCount; i++)
                for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                {
                    if ( own.source[i,j] > -1)
                        known[i, j] = false;
                    else known[i, j] = true;
                }
            known[u, v] = true;
            int currx = u, curry = v;

            List<Point> curr_front = new();
            for (int i = 0; i < 8; i++)
            {
                if (pointavailiter(currx, curry, currx + voisins[i].X, curry + voisins[i].Y))
                {
                    submedia[0].Add(new(currx + voisins[i].X, curry + voisins[i].Y));
                    curr_front.Add(new(currx + voisins[i].X, curry + voisins[i].Y));
                    known[currx + voisins[i].X, curry + voisins[i].Y] = true;
                }
            }
            while (true)
            {
                List<Point> curr_prev = new(curr_front);
                curr_front = new();
                foreach (var el in curr_prev)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Point voisepoint = new(el.X + voisins[i].X, el.Y + voisins[i].Y);
                        if (pointavailiter(el.X, el.Y, el.X + voisins[i].X, el.Y + voisins[i].Y))
                        {
                            if (!curr_front.Contains(voisepoint) && !submedia[0].Contains(voisepoint))
                                curr_front.Add(voisepoint);
                            if (!submedia[0].Contains(voisepoint))
                            {
                                submedia[0].Add(voisepoint);
                                known[el.X + voisins[i].X, el.Y + voisins[i].Y] = true;
                            }
                        }
                    }
                }
                if (curr_front.Count == 0) break;
            }
            if (!if_all_known(known))
            {
                onemedium = false;
            }
            else
            {
                onemedium = true;
            }
        }
        private double pointdest(int xc1, int yc1, int xc2, int yc2) { return Math.Sqrt(Math.Pow(xc1 - xc2, 2) + Math.Pow(yc1 - yc2, 2)); }
        private bool pointavailiter(int x1, int y1, int x2, int y2)
        { //vieux, nouveau
            try
            {
                if (pointdest(x1, y1, x2, y2) == Math.Sqrt(2) || pointdest(x1, y1, x2, y2) == 1)
                    return availpoint(x2, y2);
                else if (pointdest(x1, y1, x2, y2) == Math.Sqrt(5))
                {
                    if (x1 - x2 == 2 && y1 - y2 == 1)//1
                        return availpoint(x2 + 1, y1) && availpoint(x2 + 1, y1 + 1);
                    if (x1 - x2 == 1 && y1 - y2 == 2)//2
                        return availpoint(x2 + 1, y2 + 1) && availpoint(x2, y2 + 1);
                    if (x1 - x2 == -1 && y1 - y2 == 2)//3
                        return availpoint(x2 - 1, y2 + 1) && availpoint(x2, y2 + 1);
                    if (x1 - x2 == -2 && y1 - y2 == 1) //4
                        return availpoint(x2 - 1, y2) && availpoint(x2 - 1, y2 + 1);
                    if (x1 - x2 == -2 && y1 - y2 == -1)//5
                        return availpoint(x2 - 1, y2) && availpoint(x2 - 1, y2 - 1);
                    if (x1 - x2 == -1 && y1 - y2 == -2)//6
                        return availpoint(x2 - 1, y2 - 1) && availpoint(x2, y2 - 1);
                    if (x1 - x2 == 1 && y1 - y2 == -2)//7
                        return availpoint(x2 + 1, y2 - 1) && availpoint(x2, y2 - 1);
                    if (x1 - x2 == 2 && y1 - y2 == -1)//8
                        return availpoint(x2 + 1, y2) && availpoint(x2 + 1, y2 - 1);
                }
                else if (pointdest(x2, y2) == Math.Sqrt(10))
                {
                    if (x1 - x2 == 3 && y1 - y2 == 1)//1
                        return availpoint(x2 + 1, y2) && availpoint(x2 + 2, y2) && availpoint(x2 + 1, y2 + 1) && availpoint(x2 + 2, y2 + 1);
                    if (x1 - x2 == 1 && y1 - y2 == 3)//2
                        return availpoint(x2 + 1, y2 + 1) && availpoint(x2 + 1, y2 + 2) && availpoint(x2, y2 + 1) && availpoint(x2, y2 + 2);
                    if (x1 - x2 == -1 && y1 - y2 == 3)//3
                        return availpoint(x2 - 1, y2 + 1) && availpoint(x2 - 1, y2 + 2) && availpoint(x2, y2 + 1) && availpoint(x2, y2 + 2);
                    if (x1 - x2 == -3 && y1 - y2 == 1) //4
                        return availpoint(x2 - 1, y2) && availpoint(x2 - 2, y2) && availpoint(x2 - 1, y2 + 1) && availpoint(x2 - 2, y2 + 1);
                    if (x1 - x2 == -3 && y1 - y2 == -1)//5
                        return availpoint(x2 - 1, y2) && availpoint(x2 - 2, y2) && availpoint(x2 - 1, y2 - 1) && availpoint(x2 - 1, y2 - 2);
                    if (x1 - x2 == -1 && y1 - y2 == -3)//6
                        return availpoint(x2 - 1, y2 - 1) && availpoint(x2 - 1, y2 - 2) && availpoint(x2, y2 - 1) && availpoint(x2, y2 - 2);
                    if (x1 - x2 == 1 && y1 - y2 == -3)//7
                        return availpoint(x2 + 1, y2 - 1) && availpoint(x2 + 1, y2 - 2) && availpoint(x2, y2 - 1) && availpoint(x2, y2 - 2);
                    if (x1 - x2 == 3 && y1 - y2 == -1)//8
                        return availpoint(x2 + 1, y2) && availpoint(x2 + 2, y2) && availpoint(x2 + 1, y2 - 1) && availpoint(x2 + 2, y2 - 1);
                }
                else if (pointdest(x2, y2) == Math.Sqrt(13))
                {
                    if (x1 - x2 == 3 && y1 - y2 == 2)//1
                        return availpoint(x2 + 1, y2) && availpoint(x2 + 1, y2 + 1) && availpoint(x2 + 2, y2 + 1) && availpoint(x2 + 2, y2 + 2);
                    if (x1 - x2 == 2 && y1 - y2 == 3)//2
                        return availpoint(x2, y2 + 1) && availpoint(x2 + 1, y2 + 1) && availpoint(x2 + 1, y2 + 2) && availpoint(x2 + 2, y2 + 2);
                    if (x1 - x2 == -2 && y1 - y2 == 3)//3
                        return availpoint(x2, y2 + 1) && availpoint(x2 - 1, y2 + 1) && availpoint(x2 - 1, y2 + 2) && availpoint(x2 - 2, y2 + 2);
                    if (x1 - x2 == -3 && y1 - y2 == 2) //4
                        return availpoint(x2 - 1, y2) && availpoint(x2 - 1, y2 + 1) && availpoint(x2 - 1, y2 + 2) && availpoint(x2 - 2, y2 + 2);
                    if (x1 - x2 == -3 && y1 - y2 == -2)//5
                        return availpoint(x2 - 1, y2) && availpoint(x2 - 1, y2 - 1) && availpoint(x2 - 1, y2 - 2) && availpoint(x2 - 2, y2 - 2);
                    if (x1 - x2 == -2 && y1 - y2 == -3)//6
                        return availpoint(x2, y2 - 1) && availpoint(x2 - 1, y2 - 1) && availpoint(x2 - 1, y2 - 2) && availpoint(x2 - 2, y2 - 2);
                    if (x1 - x2 == 2 && y1 - y2 == -3)//7
                        return availpoint(x2, y2 - 1) && availpoint(x2 + 1, y2 - 1) && availpoint(x2 + 1, y2 - 2) && availpoint(x2 + 2, y2 - 2);
                    if (x1 - x2 == 3 && y1 - y2 == -2)//8
                        return availpoint(x2 + 1, y2) && availpoint(x2 + 1, y2 - 1) && availpoint(x2 + 1, y2 - 2) && availpoint(x2 + 2, y2 - 2);
                }
                return false;
            }
            catch (Exception) { return false; }
        }
        private bool if_all_known(bool[,] known)
        {
            for (int i = 0; i < known.GetLength(0); i++)
                for (int j = 0; j < known.GetLength(1); j++)
                    if (!known[i, j]) return false;
            return true;
        }
        List<Point> voisins = new List<Point>() {
            new(- 1, - 1), new( 0, - 1), new(1, - 1), new(1, 0),   new(1, 1),   new(0, 1),   new(- 1, 1),   new(- 1, 0),
            new(- 1, - 2), new( 1, - 2), new(2, - 1), new(2, 1),   new(1, 2),   new(- 1, 2), new(- 2, 1),   new(- 2, - 1),
            new(- 2, - 3), new(- 1,- 3), new(1, - 3), new(2, - 3), new(3, - 2), new(3, - 1), new(3, 1),     new(3, 2),
            new(  2,   3), new( 1, 3  ), new(- 1, 3), new(- 2, 3), new(- 3, 2), new(- 3, 1), new(- 3, - 1), new(- 3, - 2)};
        private int size_rayon()
        {
            if (radioButton1.Checked) return 8;
            else if (radioButton2.Checked) return 16;
            else return 32;
        }
        private void calculcell(int i, int xc, int yc)
        {
            if (xc + 1 > 0 && yc + 1 > 0 && xc < own.dataGridView1.RowCount && yc < own.dataGridView1.ColumnCount && own.source[xc,yc] > 0 && pointavailiter(xc, yc) && destins[i][x1, y1] != -1)
            {//if exists
                int currstage = (int)Math.Floor(destins[i][x1, y1] / variants.Count) % variants.Count;
                decimal cur = (decimal)((Convert.ToDouble(variants[currstage][xc,yc]) + Convert.ToDouble(variants[currstage][x1,y1])) / 2 * Math.Sqrt(Math.Pow(xc - x1, 2) + Math.Pow(yc - y1, 2)) + Convert.ToDouble(destins[i][x1, y1]));
                if (currstage != Math.Floor(cur/own.numericUpDown11.Value) && variants.Count > 1)
                    cur = (decimal)((Convert.ToDouble(variants[(currstage+1) % variants.Count][xc, yc]) + Convert.ToDouble(variants[(currstage + 1) % variants.Count][x1, y1])) / 2 * Math.Sqrt(Math.Pow(xc - x1, 2) + Math.Pow(yc - y1, 2)) + Convert.ToDouble(destins[i][x1, y1]));
                if (destins[i][xc, yc] == -2)
                {//if not visited at all
                    destins[i][xc, yc] = cur;
                    previos[i][xc, yc] = new Point(x1, y1);
                }
                else if (cur < destins[i][xc, yc])
                {//if from this point is shorter than known path
                    destins[i][xc, yc] = cur;
                    previos[i][xc, yc] = new Point(x1, y1);
                }
                else if (own.source[xc,yc] == -1)
                {  //if point unpassable
                    destins[i][xc, yc] = -1;
                    previos[i][xc, yc] = new Point(-1, -1);
                }
            }
        }
        private void calculcell(List<Point> set, ref decimal[,] dest, ref Point[,] prev, int xc, int yc)
        {
            if (set.Contains(new Point(xc, yc)))
            {//if consists
                int currstage = (int)Math.Floor(dest[x1, y1] / variants.Count) % variants.Count;
                decimal cur = Convert.ToDecimal((Convert.ToDouble(variants[currstage][xc,yc]) + Convert.ToDouble(variants[currstage][x1,y1])) / 2 * Math.Sqrt(Math.Pow(xc - x1, 2) + Math.Pow(yc - y1, 2)) + Convert.ToDouble(dest[x1, y1]));
                if (currstage != Math.Floor(cur / variants.Count) && variants.Count > 1)
                    cur = Convert.ToDecimal((Convert.ToDouble(variants[(currstage + 1) % variants.Count][xc, yc]) + Convert.ToDouble(variants[(currstage + 1) % variants.Count][x1, y1])) / 2 * Math.Sqrt(Math.Pow(xc - x1, 2) + Math.Pow(yc - y1, 2)) + Convert.ToDouble(dest[x1, y1]));
                if (dest[xc, yc] == -2)
                {//if not visited at all
                    dest[xc, yc] = cur;
                    prev[xc, yc] = new Point(x1, y1);
                }
                else if (cur < dest[xc, yc])
                {//if from this point is shorter than known path
                    dest[xc, yc] = cur;
                    prev[xc, yc] = new Point(x1, y1);
                }
                else if (Convert.ToDecimal(own.source[xc,yc]) == -1)
                {  //if point unpassable
                    dest[xc, yc] = -1;
                    prev[xc, yc] = new Point(-1, -1);
                }
            }
        }
        private void calculcell(ref bool[,] access, ref decimal[,] dest, ref Point[,] prev, int xc, int yc, int xl, int yl)
        {
            if (xc >= 0 && yc >= 0 && xc < dataGridView2.RowCount && yc < dataGridView2.ColumnCount && access[xc, yc])
            {//if consists
                int currstage = (int)Math.Floor(dest[xl, yl] / variants.Count) % variants.Count;
                decimal cur = Convert.ToDecimal(Convert.ToDouble(variants[currstage][xc,yc]) + Convert.ToDouble(variants[currstage][xl,yl]) / 2 * Math.Sqrt(Math.Pow(xc - xl, 2) + Math.Pow(yc - yl, 2)) + Convert.ToDouble(dest[xl, yl]));
                if (currstage != Math.Floor(cur / variants.Count) && variants.Count > 1)
                    cur = Convert.ToDecimal((Convert.ToDouble(variants[(currstage + 1) % variants.Count][xc, yc]) + Convert.ToDouble(variants[(currstage + 1) % variants.Count][x1, y1])) / 2 * Math.Sqrt(Math.Pow(xc - x1, 2) + Math.Pow(yc - y1, 2)) + Convert.ToDouble(dest[xl, yl]));
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
                else if (Convert.ToDecimal(own.source[xc,yc]) == -1)
                {  //if point unpassable
                    dest[xc, yc] = -1;
                    prev[xc, yc] = new Point(-1, -1);
                }
            }
        }
        private bool checkallvis()
        {
            for (int i = 0; i < own.dataGridView1.RowCount; i++)
                for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                    if (!vis[i, j]) return false;
            return true;
        }
        private bool checkallvis(bool[,] set)
        {
            for (int i = 0; i < own.dataGridView1.RowCount; i++)
                for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                    if (!set[i, j]) return false;
            return true;
        }
        private bool pointavailiter(int i, int j)
        {
            try
            {
                if (pointdest(i, j) == Math.Sqrt(2) || pointdest(i, j) == 1)
                    return availpoint(i, j);
                else if (pointdest(i, j) == Math.Sqrt(5))
                {
                    if (x1 - i == 2 && y1 - j == 1)//1
                        return availpoint(i + 1, j) && availpoint(i + 1, j + 1);
                    if (x1 - i == 1 && y1 - j == 2)//2
                        return availpoint(i + 1, j + 1) && availpoint(i, j + 1);
                    if (x1 - i == -1 && y1 - j == 2)//3
                        return availpoint(i - 1, j + 1) && availpoint(i, j + 1);
                    if (x1 - i == -2 && y1 - j == 1) //4
                        return availpoint(i - 1, j) && availpoint(i - 1, j + 1);
                    if (x1 - i == -2 && y1 - j == -1)//5
                        return availpoint(i - 1, j) && availpoint(i - 1, j - 1);
                    if (x1 - i == -1 && y1 - j == -2)//6
                        return availpoint(i - 1, j - 1) && availpoint(i, j - 1);
                    if (x1 - i == 1 && y1 - j == -2)//7
                        return availpoint(i + 1, j - 1) && availpoint(i, j - 1);
                    if (x1 - i == 2 && y1 - j == -1)//8
                        return availpoint(i + 1, j) && availpoint(i + 1, j - 1);
                }
                else if (pointdest(i, j) == Math.Sqrt(10))
                {
                    if (x1 - i == 3 && y1 - j == 1)//1
                        return availpoint(i + 1, j) && availpoint(i + 2, j) && availpoint(i + 1, j + 1) && availpoint(i + 2, j + 1);
                    if (x1 - i == 1 && y1 - j == 3)//2
                        return availpoint(i + 1, j + 1) && availpoint(i + 1, j + 2) && availpoint(i, j + 1) && availpoint(i, j + 2);
                    if (x1 - i == -1 && y1 - j == 3)//3
                        return availpoint(i - 1, j + 1) && availpoint(i - 1, j + 2) && availpoint(i, j + 1) && availpoint(i, j + 2);
                    if (x1 - i == -3 && y1 - j == 1) //4
                        return availpoint(i - 1, j) && availpoint(i - 2, j) && availpoint(i - 1, j + 1) && availpoint(i - 2, j + 1);
                    if (x1 - i == -3 && y1 - j == -1)//5
                        return availpoint(i - 1, j) && availpoint(i - 2, j) && availpoint(i - 1, j - 1) && availpoint(i - 1, j - 2);
                    if (x1 - i == -1 && y1 - j == -3)//6
                        return availpoint(i - 1, j - 1) && availpoint(i - 1, j - 2) && availpoint(i, j - 1) && availpoint(i, j - 2);
                    if (x1 - i == 1 && y1 - j == -3)//7
                        return availpoint(i + 1, j - 1) && availpoint(i + 1, j - 2) && availpoint(i, j - 1) && availpoint(i, j - 2);
                    if (x1 - i == 3 && y1 - j == -1)//8
                        return availpoint(i + 1, j) && availpoint(i + 2, j) && availpoint(i + 1, j - 1) && availpoint(i + 2, j - 1);
                }
                else if (pointdest(i, j) == Math.Sqrt(13))
                {
                    if (x1 - i == 3 && y1 - j == 2)//1
                        return availpoint(i + 1, j) && availpoint(i + 1, j + 1) && availpoint(i + 2, j + 1) && availpoint(i + 2, j + 2);
                    if (x1 - i == 2 && y1 - j == 3)//2
                        return availpoint(i, j + 1) && availpoint(i + 1, j + 1) && availpoint(i + 1, j + 2) && availpoint(i + 2, j + 2);
                    if (x1 - i == -2 && y1 - j == 3)//3
                        return availpoint(i, j + 1) && availpoint(i - 1, j + 1) && availpoint(i - 1, j + 2) && availpoint(i - 2, j + 2);
                    if (x1 - i == -3 && y1 - j == 2) //4
                        return availpoint(i - 1, j) && availpoint(i - 1, j + 1) && availpoint(i - 1, j + 2) && availpoint(i - 2, j + 2);
                    if (x1 - i == -3 && y1 - j == -2)//5
                        return availpoint(i - 1, j) && availpoint(i - 1, j - 1) && availpoint(i - 1, j - 2) && availpoint(i - 2, j - 2);
                    if (x1 - i == -2 && y1 - j == -3)//6
                        return availpoint(i, j - 1) && availpoint(i - 1, j - 1) && availpoint(i - 1, j - 2) && availpoint(i - 2, j - 2);
                    if (x1 - i == 2 && y1 - j == -3)//7
                        return availpoint(i, j - 1) && availpoint(i + 1, j - 1) && availpoint(i + 1, j - 2) && availpoint(i + 2, j - 2);
                    if (x1 - i == 3 && y1 - j == -2)//8
                        return availpoint(i + 1, j) && availpoint(i + 1, j - 1) && availpoint(i + 1, j - 2) && availpoint(i + 2, j - 2);
                }
                return false;
            }
            catch (Exception) { return false; }
        }
        private double pointdest(int xc, int yc) { return Math.Sqrt(Math.Pow(x1 - xc, 2) + Math.Pow(y1 - yc, 2)); }
        private void button6_Click(object sender, EventArgs e)
        {
            bool t = true;
            int xl = Convert.ToInt32(numericUpDown3.Value);
            int yl = Convert.ToInt32(numericUpDown4.Value);
            if ( own.source[xl - 1,yl - 1] < 0)
                t = false;
            else
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == xl && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == yl)
                    {
                        t = false;
                        break;
                    }
                }
            }
            if (t)
            {
                dataGridView1.Rows.Add(new object[] { xl, yl });
                refr(false);
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            int xl = Convert.ToInt32(numericUpDown3.Value);
            int yl = Convert.ToInt32(numericUpDown4.Value);
            for (int i = 0; i < dataGridView1.RowCount; i++)
                if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == xl && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == yl)
                {
                    dataGridView1.Rows.RemoveAt(i);
                    break;
                }
            refr(false);
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e) { refr(false); }
        private void radioButton2_CheckedChanged(object sender, EventArgs e) { refr(false); }
        private void radioButton3_CheckedChanged(object sender, EventArgs e) { refr(false); }
        private void Vran_FormClosing(object sender, FormClosingEventArgs e) { own.vran = null; forfait?.Close(); }

        private void lesForfaitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (forfait == null)
            {
                forfait = new(this);
                if (own.lang == 1) forfait.toRusse();
                forfait.Show();
            }
            forfait.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var cers = (int)numericUpDown1.Value;
            var effs = (int)numericUpDown2.Value;
            dataGridView1.RowCount = 0;
            Random r = new Random();
            List<List<Point>> starts = new(), finis = new(), medieval;
            List<List<decimal>> minrads = new();
            reseachpoints(0, 0);
            for (int k = 0; k < effs; k++)
            {
                List<Point> genered = new();
                List<decimal> mins = new();
                for (int j = 0; j < cers; j++)
                { // generating new start points
                    Point cu;
                    do
                    {
                        cu = new(r.Next(own.dataGridView1.RowCount), r.Next(own.dataGridView1.ColumnCount));
                        if (own.source[cu.X,cu.Y] >= 0 && !genered.Contains(cu))
                            break;
                    } while (true);
                    genered.Add(cu);
                }
                starts.Add(genered);

                Cursor.Current = Cursors.WaitCursor;
                medieval = new() { genered };
                do
                {
                    curr_points = new();

                    destins = new();
                    previos = new();

                    for (int l = 0; l < cers; l++)
                    {

                        destins.Add(new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount]);
                        previos.Add(new Point[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount]);

                        x = Convert.ToInt32(medieval[^1][l].X); x1 = x;
                        y = Convert.ToInt32(medieval[^1][l].Y); y1 = y;

                        vis = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                        vis1 = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                        for (int i = 0; i < own.dataGridView1.RowCount; i++)
                            for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                            {
                                vis1[i, j] = false;
                            }
                        vis1[x, y] = true;
                        curr_med = determ_submed(new(x, y));
                        previos[l][x, y] = new(-2, -2);
                        for (int i = 0; i < own.dataGridView1.RowCount; i++)
                            for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                            {
                                if (availpoint(i, j) && availpoint(x1, y1) && submedia[curr_med].Contains(new(i, j)))
                                {
                                    destins[l][i, j] = -2;
                                    vis[i, j] = false;
                                }
                                else
                                {
                                    vis[i, j] = true;
                                    destins[l][i, j] = -1;
                                    previos[l][i, j] = new Point(-1, -1);
                                }
                            }
                        destins[l][x, y] = 0;

                        while (!checkallvis())
                        {
                            vis[x1, y1] = true;
                            for (int i = 0; i < size_rayon(); i++)
                                calculcell(l, x1 + voisins[i].X, y1 + voisins[i].Y);

                            //transmission le point actuel a le point le plus proche et minimum
                            var frontiers = new List<Points>();
                            for (int i = 0; i < own.dataGridView1.RowCount; i++)//searching frontier points
                                for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                                    if (destins[l][i, j] != -2 && !vis[i, j])
                                        frontiers.Add(new Points(i, j, destins[l][i, j]));
                            decimal mindest = Decimal.MaxValue;
                            foreach (var p in frontiers)//searching the minimum points
                                if (p.dest < mindest)
                                    mindest = p.dest;
                            var points = new List<Points>();
                            foreach (var p in frontiers)
                                if (p.dest == mindest)
                                    points.Add(new Points(p.xl, p.yl, (decimal)Math.Sqrt(Math.Pow(x1 - p.xl, 2) + Math.Pow(y1 - p.yl, 2))));

                            if (points.Count == 1) { x1 = points[0].xl; y1 = points[0].yl; }//if it is alone to choose, equal et continue
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
                                if (points1.Count == 1) { x1 = points1[0].xl; y1 = points1[0].yl; }//if it is alone to choose, equal et continue
                                else if (points1.Count > 1)
                                {//if not, choosing with least x
                                    int minx = own.dataGridView1.RowCount;
                                    foreach (var p in points1)
                                        if (p.xl < minx)
                                            minx = p.xl;
                                    foreach (var p in points1)
                                        if (p.xl == minx)
                                        {
                                            x1 = points1[0].xl;
                                            y1 = points1[0].yl;
                                            break;
                                        }
                                }
                            }
                        }
                    }

                    owingpoints = new List<List<Point>>(); //list of sets of chosen points
                    foreach (var po in medieval[^1])
                    {
                        Point p = new Point(po.X, po.Y);
                        List<Point> lp = new List<Point> { p };
                        owingpoints.Add(lp);
                    }
                    for (int i = 0; i < dataGridView2.RowCount; i++)
                        for (int j = 0; j < dataGridView2.ColumnCount; j++)
                        {
                            if (own.source[i,j] <= 0)
                            {

                            }
                            else if (!ifacentrepoint(i, j))
                            {
                                int h = mindest(i, j);
                                owingpoints[h].Add(new Point(i, j));
                            }
                        }
                    var opersets = new List<List<Point>>();
                    foreach (var sub in owingpoints)
                    {
                        List<Point> s = new();
                        foreach (var el in sub)
                        {
                            s.Add(el);
                        }
                        opersets.Add(s);
                    }

                    foreach (var sub in owingpoints)
                    {//chosing the centre for each
                        var minsum = decimal.MaxValue;
                        Point centre = new();
                        foreach (var el in sub)
                        {
                            var currl = calculater_sum_pour_point(sub, el);
                            if (currl < minsum)
                            {
                                minsum = currl;
                                centre = new(el.X, el.Y);
                            }
                        }
                        curr_points.Add(new(centre.X, centre.Y));
                    }
                    Cursor.Current = Cursors.Default;
                    medieval.Add(curr_points);
                    //waving from current centres to determine a min radius for each
                    minrads = new();
                    for (int t = 0; t < curr_points.Count; t++)
                    {
                        var currfront = new List<Point>(get_frontiers(owingpoints[t]));
                        decimal[,] curr_values = new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                        Point[,] previousl = new Point[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];

                        x = curr_points[t].X; x1 = x;
                        y = curr_points[t].Y; y1 = y;

                        vis1 = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                        for (int i = 0; i < own.dataGridView1.RowCount; i++)
                            for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                            {
                                vis1[i, j] = !owingpoints[t].Contains(new Point(i, j));
                                curr_values[i, j] = -2;
                            }
                        vis1[x, y] = true;
                        previousl[x, y] = new(-2, -2);
                        curr_values[x, y] = 0;

                        while (!checkallvis(vis1))
                        {
                            for (int i = 0; i < size_rayon(); i++)
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + voisins[i].X, y1 + voisins[i].Y);

                            //transmission le point actuel a le point le plus proche et minimum
                            var frontiers = new List<Points>();
                            for (int i = 0; i < own.dataGridView1.RowCount; i++)//searching frontier points
                                for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                                    if (curr_values[i, j] != -2 && !vis1[i, j])
                                        frontiers.Add(new Points(i, j, curr_values[i, j]));
                            decimal mindest = Decimal.MaxValue;
                            foreach (var p in frontiers)//searching the minimum points
                                if (p.dest < mindest)
                                    mindest = p.dest;
                            var points = new List<Points>();
                            foreach (var p in frontiers)
                                if (p.dest == mindest)
                                    points.Add(new Points(p.xl, p.yl, (decimal)Math.Sqrt(Math.Pow(x1 - p.xl, 2) + Math.Pow(y1 - p.yl, 2))));
                            if (points.Count == 1) { x1 = points[0].xl; y1 = points[0].yl; }//if it is alone to choose, equal et continue
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
                                if (points1.Count == 1) { x1 = points1[0].xl; y1 = points1[0].yl; }//if it is alone to choose, equal et continue
                                else if (points1.Count > 1)
                                {//if not, choosing with least x
                                    int minx = own.dataGridView1.RowCount;
                                    foreach (var p in points1)
                                        if (p.xl < minx)
                                            minx = p.xl;
                                    foreach (var p in points1)
                                        if (p.xl == minx)
                                        {
                                            x1 = points1[0].xl;
                                            y1 = points1[0].yl;
                                            break;
                                        }
                                }
                            }
                            vis1[x1, y1] = true;
                        }
                        wave_de_points.Add(curr_values);
                        decimal minv = curr_values[currfront[0].X, currfront[0].Y];
                        for (int u = 1; u < currfront.Count; u++)
                        {
                            if (minv > curr_values[currfront[u].X, currfront[u].Y])
                                minv = curr_values[currfront[u].X, currfront[u].Y];
                        }
                        mins.Add(minv);
                    }
                }
                while (!compararer_sets(medieval[^1], medieval[^2]));
                minrads.Add(mins);
                finis.Add(medieval[^1]);
            }
            decimal maxsum = 0;
            int indmax = 0;
            for (int i = 0; i < minrads.Count; i++)
            {
                decimal sum = 0;
                foreach (var p in minrads[i])
                {
                    sum += p;
                }
                if (sum > maxsum)
                {
                    maxsum = sum;
                    indmax = i;
                }
            }
            foreach (var el in finis[indmax])
                dataGridView1.Rows.Add(new object[] { (el.X + 1), (el.Y + 1) });
            refr(false);
            /*if (own.lang == 0) { 
                MessageBox.Show("Multistart s'est terminé avec le résultat suivant :\n" +
                    "Points de start "+starts.ToArray().ToString(), "Rapport");
            }
            else if (own.lang == 1) {



            }*/
        }
        private bool compararer_sets(List<Point> set1, List<Point> set2)
        {
            if (set1.Count != set2.Count) return false;
            else
            {
                foreach (var el in set1)
                {
                    if (!set2.Contains(el))
                        return false;
                }
                foreach (var el in set2)
                {
                    if (!set1.Contains(el))
                        return false;
                }
                return true;
            }
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            dataGridView2.DefaultCellStyle.Format = 'N' + numericUpDown6.Value.ToString();
            dataGridView2.AutoResizeColumns();
            dataGridView2.AutoResizeRows();
        }
        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            dataGridView2.DefaultCellStyle.Font = new System.Drawing.Font("Palatino Linotype", (float)numericUpDown5.Value);
            dataGridView2.AutoResizeColumns();
            dataGridView2.AutoResizeRows();
        }
        internal void ToFrancais()
        {
            groupBox1.Text = "Les points";
            groupBox3.Text = "Le point actuel";
            groupBox4.Text = "Reflexion de cellules";
            groupBox5.Text = "Multistart";
            label1.Text = "La quantité de cercles";
            label2.Text = "La quantité d'efforts";
            label7.Text = "La taille de fonte";
            label8.Text = "La quantité de signs";
            button1.Text = "Multistart";
            button2.Text = "Suppremer";
            button3.Text = "Finir les calculations";
            button4.Text = "Recomputer";
            button5.Text = "Suppremer";
            button6.Text = "Aujouter";
            groupBox2.Text = "Le destin de chercher de chemins";
            groupBox5.Text = "Multistart";

            radioButton3.Text = "III rayon (32 directions)";
            radioButton2.Text = "II rayon (16 directions)";
            radioButton1.Text = "I rayon (8 directions)";
            
            label4.Text = "La ligne";
            label3.Text = "La colonne";
            Text = "Voronoi";
            Column1.HeaderText = "La ligne";
            Column2.HeaderText = "La colonne";
            

            lesForfaitsToolStripMenuItem.Text = "Les forfaits";

            if (forfait != null) forfait.toFrancais();
        }
        internal void ToRusse()
        {
            groupBox1.Text = "Точки";
            groupBox2.Text = "Кол-во направлений поиска";
            groupBox3.Text = "Текущая точка";
            groupBox4.Text = "Отображение ячеек";
            groupBox5.Text = "Мультистарт";
            label1.Text = "Кол-во кругов";
            label2.Text = "Кол-во попыток";
            label3.Text = "Столбец";
            label4.Text = "Строка";
            label7.Text = "Размер шрифта";
            label8.Text = "Количество знаков";
            button1.Text = "Мультистарт";
            button3.Text = "Найти оптимальные центры";
            button4.Text = "Пересчитать";
            button5.Text = "Удалить";
            button6.Text = "Добавить";
            radioButton3.Text = "III радиус (32 направления)";
            radioButton2.Text = "II радиус (16 направления)";
            radioButton1.Text = "I радиус (8 направлений)";
            button2.Text = "Удалить";


            Text = "Диаграмма Вороного";

            Column1.HeaderText = "Строка";
            Column2.HeaderText = "Стоблец";
            lesForfaitsToolStripMenuItem.Text = "Упаковки";

            if (forfait != null) forfait.toRusse();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedCells[0].RowIndex < dataGridView1.RowCount)
            {
                numericUpDown3.Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                numericUpDown4.Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[1].Value);
            }
            else
            {
                numericUpDown3.Value = 1000;
                numericUpDown4.Value = 1000;
            }
        }

        private void button3_Click(object sender, EventArgs e){
            List<Point> genered = new();
            List<decimal> mins = new();
            List<List<Point>> starts = new(), finis = new(), medieval;
            List<List<decimal>> minrads = new();
            int cers = dataGridView1.RowCount;
            for (int j = 0; j < cers; j++) { // getting current points
                Point cu = new(Convert.ToInt32(dataGridView1.Rows[j].Cells[0].Value), Convert.ToInt32(dataGridView1.Rows[j].Cells[1].Value));
                
                genered.Add(cu);
            }
            starts.Add(genered);

            Cursor.Current = Cursors.WaitCursor;
            medieval = new() { genered };
            do
            {
                curr_points = new();

                destins = new();
                previos = new();

                for (int l = 0; l < cers; l++)
                {

                    destins.Add(new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount]);
                    previos.Add(new Point[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount]);

                    x = Convert.ToInt32(medieval[^1][l].X); x1 = x;
                    y = Convert.ToInt32(medieval[^1][l].Y); y1 = y;

                    vis = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    vis1 = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    for (int i = 0; i < own.dataGridView1.RowCount; i++)
                        for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                        {
                            vis1[i, j] = false;
                        }
                    vis1[x, y] = true;
                    curr_med = determ_submed(new(x, y));
                    previos[l][x, y] = new(-2, -2);
                    for (int i = 0; i < own.dataGridView1.RowCount; i++)
                        for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                        {
                            if (availpoint(i, j) && availpoint(x1, y1) && submedia[curr_med].Contains(new(i, j)))
                            {
                                destins[l][i, j] = -2;
                                vis[i, j] = false;
                            }
                            else
                            {
                                vis[i, j] = true;
                                destins[l][i, j] = -1;
                                previos[l][i, j] = new Point(-1, -1);
                            }
                        }
                    destins[l][x, y] = 0;

                    while (!checkallvis())
                    {
                        vis[x1, y1] = true;
                        for (int i = 0; i < size_rayon(); i++)
                            calculcell(l, x1 + voisins[i].X, y1 + voisins[i].Y);

                        //transmission le point actuel a le point le plus proche et minimum
                        var frontiers = new List<Points>();
                        for (int i = 0; i < own.dataGridView1.RowCount; i++)//searching frontier points
                            for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                                if (destins[l][i, j] != -2 && !vis[i, j])
                                    frontiers.Add(new Points(i, j, destins[l][i, j]));
                        decimal mindest = Decimal.MaxValue;
                        foreach (var p in frontiers)//searching the minimum points
                            if (p.dest < mindest)
                                mindest = p.dest;
                        var points = new List<Points>();
                        foreach (var p in frontiers)
                            if (p.dest == mindest)
                                points.Add(new Points(p.xl, p.yl, (decimal)Math.Sqrt(Math.Pow(x1 - p.xl, 2) + Math.Pow(y1 - p.yl, 2))));

                        if (points.Count == 1) { x1 = points[0].xl; y1 = points[0].yl; }//if it is alone to choose, equal et continue
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
                            if (points1.Count == 1) { x1 = points1[0].xl; y1 = points1[0].yl; }//if it is alone to choose, equal et continue
                            else if (points1.Count > 1)
                            {//if not, choosing with least x
                                int minx = own.dataGridView1.RowCount;
                                foreach (var p in points1)
                                    if (p.xl < minx)
                                        minx = p.xl;
                                foreach (var p in points1)
                                    if (p.xl == minx)
                                    {
                                        x1 = points1[0].xl;
                                        y1 = points1[0].yl;
                                        break;
                                    }
                            }
                        }
                    }
                }

                owingpoints = new List<List<Point>>(); //list of sets of chosen points
                foreach (var po in medieval[^1])
                {
                    Point p = new Point(po.X, po.Y);
                    List<Point> lp = new List<Point> { p };
                    owingpoints.Add(lp);
                }
                for (int i = 0; i < dataGridView2.RowCount; i++)
                    for (int j = 0; j < dataGridView2.ColumnCount; j++)
                    {
                        if (own.source[i,j] <= 0)
                        {

                        }
                        else if (!ifacentrepoint(i, j))
                        {
                            int h = mindest(i, j);
                            owingpoints[h].Add(new Point(i, j));
                        }
                    }
                var opersets = new List<List<Point>>();
                foreach (var sub in owingpoints)
                {
                    List<Point> s = new();
                    foreach (var el in sub)
                    {
                        s.Add(el);
                    }
                    opersets.Add(s);
                }

                foreach (var sub in owingpoints)
                {//chosing the centre for each
                    var minsum = decimal.MaxValue;
                    Point centre = new();
                    foreach (var el in sub)
                    {
                        var currl = calculater_sum_pour_point(sub, el);
                        if (currl < minsum)
                        {
                            minsum = currl;
                            centre = new(el.X, el.Y);
                        }
                    }
                    curr_points.Add(new(centre.X, centre.Y));
                }
                Cursor.Current = Cursors.Default;
                medieval.Add(curr_points);
                //waving from current centres to determine a min radius for each
                minrads = new();
                for (int t = 0; t < curr_points.Count; t++)
                {
                    var currfront = new List<Point>(get_frontiers(owingpoints[t]));
                    decimal[,] curr_values = new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    Point[,] previousl = new Point[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];

                    x = curr_points[t].X; x1 = x;
                    y = curr_points[t].Y; y1 = y;

                    vis1 = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    for (int i = 0; i < own.dataGridView1.RowCount; i++)
                        for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                        {
                            vis1[i, j] = !owingpoints[t].Contains(new Point(i, j));
                            curr_values[i, j] = -2;
                        }
                    vis1[x, y] = true;
                    previousl[x, y] = new(-2, -2);
                    curr_values[x, y] = 0;

                    while (!checkallvis(vis1))
                    {
                        for (int i = 0; i < size_rayon(); i++)
                            calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + voisins[i].X, y1 + voisins[i].Y);

                        //transmission le point actuel a le point le plus proche et minimum
                        var frontiers = new List<Points>();
                        for (int i = 0; i < own.dataGridView1.RowCount; i++)//searching frontier points
                            for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                                if (curr_values[i, j] != -2 && !vis1[i, j])
                                    frontiers.Add(new Points(i, j, curr_values[i, j]));
                        decimal mindest = Decimal.MaxValue;
                        foreach (var p in frontiers)//searching the minimum points
                            if (p.dest < mindest)
                                mindest = p.dest;
                        var points = new List<Points>();
                        foreach (var p in frontiers)
                            if (p.dest == mindest)
                                points.Add(new Points(p.xl, p.yl, (decimal)Math.Sqrt(Math.Pow(x1 - p.xl, 2) + Math.Pow(y1 - p.yl, 2))));
                        if (points.Count == 1) { x1 = points[0].xl; y1 = points[0].yl; }//if it is alone to choose, equal et continue
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
                            if (points1.Count == 1) { x1 = points1[0].xl; y1 = points1[0].yl; }//if it is alone to choose, equal et continue
                            else if (points1.Count > 1)
                            {//if not, choosing with least x
                                int minx = own.dataGridView1.RowCount;
                                foreach (var p in points1)
                                    if (p.xl < minx)
                                        minx = p.xl;
                                foreach (var p in points1)
                                    if (p.xl == minx)
                                    {
                                        x1 = points1[0].xl;
                                        y1 = points1[0].yl;
                                        break;
                                    }
                            }
                        }
                        vis1[x1, y1] = true;
                    }
                    wave_de_points.Add(curr_values);
                    decimal minv = curr_values[currfront[0].X, currfront[0].Y];
                    for (int u = 1; u < currfront.Count; u++)
                    {
                        if (minv > curr_values[currfront[u].X, currfront[u].Y])
                            minv = curr_values[currfront[u].X, currfront[u].Y];
                    }
                    mins.Add(minv);
                }
            }
            while (!compararer_sets(medieval[^1], medieval[^2]));
            minrads.Add(mins);
            finis.Add(medieval[^1]);
            string formater = "0.##", formate = "0"; ;
            if (numericUpDown6.Value > 2)
            {
                for (int n = 2; n < numericUpDown6.Value; n++)
                    formater += "#";
            }
            if (numericUpDown6.Value > 0)
            {
                formate += ".";
                for (int n = 2; n < numericUpDown6.Value; n++)
                    formate += "#";
            }
            for (int i = 0; i < cers; i++) {
                dataGridView1.Rows[i].Cells[0].Value = finis[0][i].X;
                dataGridView1.Rows[i].Cells[1].Value = finis[0][i].Y;
                dataGridView2.Rows[finis[0][i].X].Cells[finis[0][i].Y].Value = own.source[finis[0][i].X,finis[0][i].Y].ToString(formate) + "(" + minrads[0][i].ToString(formater) + ")";
            }
            refr(false);
        }
        
    }
}