using DocumentFormat.OpenXml.Office2019.Excel.RichData2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static l_application_pour_diploma.Classes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace l_application_pour_diploma{
    public partial class Vran : Form{
        private Commencement own;
        private List<Color> ColeurList = new List<Color> { Color.Red, Color.OrangeRed, Color.Orange, Color.Yellow, Color.YellowGreen, 
            Color.GreenYellow, Color.Green, Color.DarkGreen, Color.SkyBlue, Color.MediumBlue, Color.DarkBlue, Color.Cyan, Color.BlueViolet, 
            Color.Violet, Color.Silver, Color.Gold, Color.SeaGreen};
        List<decimal[,]> destins;
        internal List<Point[,]> previos;
        internal int x = 1, y = 1, x1 = 1, y1 = 1;
        List<Point> curr;
        internal bool[,] vis, vis1, accessible;
        List<Point> curr_points;
        internal Packs? forfait;
        public Vran(Commencement o) {
            InitializeComponent();
            own = o;
            curr_points = new();
        }
        private bool availpoint(int u, int v) { return Convert.ToDecimal(own.dataGridView1.Rows[u].Cells[v].Value) >= 0; }
        private void Vran_Load(object sender, EventArgs e) {
            refr();
        }
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e){
            numericUpDown3.Value = Convert.ToDecimal(dataGridView2.SelectedCells[0].RowIndex) + 1;
            numericUpDown4.Value = Convert.ToDecimal(dataGridView2.SelectedCells[0].ColumnIndex) + 1;
        }
        private void button4_Click(object sender, EventArgs e){
            refr();
        }
        internal void refr() {
            dataGridView2.RowCount = own.dataGridView1.RowCount;
            dataGridView2.ColumnCount = own.dataGridView1.ColumnCount;
            for (int i = 0; i < dataGridView2.RowCount; i++)
                for (int j = 0; j < dataGridView2.ColumnCount; j++){
                    dataGridView2.Rows[i].Cells[j].Value = own.dataGridView1.Rows[i].Cells[j].Value;
                    dataGridView2.Rows[i].Cells[j].Style.BackColor = Color.White;
                }
            dataGridView2.AutoResizeColumns();
            if (dataGridView1.RowCount > 0) {
                if (curr_points.Count > 0 && curr_points.Count == dataGridView1.RowCount){
                    dataGridView1.RowCount = 0;
                    foreach(var el in curr_points)
                        dataGridView1.Rows.Add(new object[] { (el.X + 1), (el.Y + 1) });
                }
                curr_points = new();
                List<int> randcol = new List<int>();
                for (int i = 0; i < dataGridView1.RowCount; i++) { //choosing les coleurs
                    Random r = new Random();
                    int cur = r.Next(ColeurList.Count);
                    while (randcol.Contains(cur)) {
                        cur = r.Next(ColeurList.Count);
                    }
                    randcol.Add(cur);
                }
                destins = new();
                previos = new();

                Cursor.Current = Cursors.WaitCursor;
                for (int l = 0; l < dataGridView1.RowCount; l++) {
                    destins.Add(new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount]);
                    previos.Add(new Point[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount]);

                    x = Convert.ToInt32(dataGridView1.Rows[l].Cells[0].Value) - 1; x1 = x;
                    y = Convert.ToInt32(dataGridView1.Rows[l].Cells[1].Value) - 1; y1 = y;

                    vis = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    vis1 = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    accessible = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    for (int i = 0; i < own.dataGridView1.RowCount; i++)
                        for (int j = 0; j < own.dataGridView1.ColumnCount; j++){
                            accessible[i, j] = false;
                            vis1[i, j] = false;
                        }
                    accessible[x, y] = true;
                    vis1[x, y] = true;
                    reseachpoints(x, y);
                    if (!chckpoints()) break;

                    previos[l][x, y] = new(-2, -2);
                    for (int i = 0; i < own.dataGridView1.RowCount; i++)
                        for (int j = 0; j < own.dataGridView1.ColumnCount; j++){
                            if (availpoint(i, j) && availpoint(x1, y1) && accessible[i, j]){
                                destins[l][i, j] = -2;
                                vis[i, j] = false;
                            }
                            else{
                                vis[i, j] = true;
                                destins[l][i, j] = -1;
                                previos[l][i, j] = new Point(-1, -1);
                            }
                        }
                    destins[l][x, y] = 0;
                    while (!checkallvis()){
                        vis[x1, y1] = true;
                        calculcell(l, x1 - 1, y1 - 1);
                        calculcell(l, x1, y1 - 1);
                        calculcell(l, x1 + 1, y1 - 1);
                        calculcell(l, x1 + 1, y1);
                        calculcell(l, x1 + 1, y1 + 1);
                        calculcell(l, x1, y1 + 1);
                        calculcell(l, x1 - 1, y1 + 1);
                        calculcell(l, x1 - 1, y1);
                        if (!radioButton1.Checked) {
                            calculcell(l, x1 - 1, y1 - 2);
                            calculcell(l, x1 + 1, y1 - 2);
                            calculcell(l, x1 + 2, y1 - 1);
                            calculcell(l, x1 + 2, y1 + 1);
                            calculcell(l, x1 + 1, y1 + 2);
                            calculcell(l, x1 - 1, y1 + 2);
                            calculcell(l, x1 - 2, y1 + 1);
                            calculcell(l, x1 - 2, y1 - 1);
                            if (!radioButton2.Checked){
                                calculcell(l, x1 - 2, y1 - 3);
                                calculcell(l, x1 - 1, y1 - 3);
                                calculcell(l, x1 + 1, y1 - 3);
                                calculcell(l, x1 + 2, y1 - 3);
                                calculcell(l, x1 + 3, y1 - 2);
                                calculcell(l, x1 + 3, y1 - 1);
                                calculcell(l, x1 + 3, y1 + 1);
                                calculcell(l, x1 + 3, y1 + 2);
                                calculcell(l, x1 + 2, y1 + 3);
                                calculcell(l, x1 + 1, y1 + 3);
                                calculcell(l, x1 - 1, y1 + 3);
                                calculcell(l, x1 - 2, y1 + 3);
                                calculcell(l, x1 - 3, y1 + 2);
                                calculcell(l, x1 - 3, y1 + 1);
                                calculcell(l, x1 - 3, y1 - 1);
                                calculcell(l, x1 - 3, y1 - 2);
                            }
                        }
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
                        else if (points.Count > 1) { //if not, search nearest to (0,0) 
                            mindest = Decimal.MaxValue;
                            foreach (var p in points)
                                if (p.dest < mindest)
                                    mindest = p.dest;
                            var points1 = new List<Points>();
                            foreach (var p in points)
                                if (p.dest == mindest)
                                    points1.Add(new Points(p.xl, p.yl, (decimal)Math.Sqrt(Math.Pow(p.xl, 2) + Math.Pow(p.yl, 2))));
                            if (points1.Count == 1) { x1 = points1[0].xl; y1 = points1[0].yl; }//if it is alone to choose, equal et continue
                            else if (points1.Count > 1)  {//if not, choosing with least x
                                int minx = own.dataGridView1.RowCount;
                                foreach (var p in points1)
                                    if (p.xl < minx)
                                        minx = p.xl;
                                foreach (var p in points1)
                                    if (p.xl == minx) {
                                        x1 = points1[0].xl;
                                        y1 = points1[0].yl;
                                        break;
                                    }
                            }
                        }
                    }
                }
                List<List<Point>> owingpoints = new List<List<Point>> (); //list of sets of chosen points
                for (int i = 0; i < dataGridView1.RowCount; i++) {
                    Point p = new Point(Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) - 1,
                            Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) - 1);
                    List<Point> lp = new List<Point> { p };
                    owingpoints.Add(lp);
                }
                //filling with a coleurs
                for (int i = 0; i < dataGridView2.RowCount; i++)
                    for (int j = 0; j < dataGridView2.ColumnCount; j++) {
                        if (Convert.ToDecimal(own.dataGridView1.Rows[i].Cells[j].Value) <= 0)
                            dataGridView2.Rows[i].Cells[j].Style.BackColor = Color.Black;
                        else if (!ifacentrepoint(i, j)){
                            int h = mindest(i, j);
                            dataGridView2.Rows[i].Cells[j].Style.BackColor = ColeurList[randcol[h % (randcol.Count)]];
                            owingpoints[h].Add(new Point(i, j));
                        }
                    }
                //var opersets = new List<List<Point>> (owingpoints);
                var opersets = new List<List<Point>>();
                foreach (var sub in owingpoints) {
                    List<Point> s = new();
                    foreach (var el in sub) {
                        s.Add(el);
                    }
                    opersets.Add(s);
                }
                
                for (int i = 0; i < opersets.Count; i++){ //compress sets
                    while (opersets[i].Count > 1) { 
                        if (!if_not_a_ligne(opersets[i])){
                            opersets[i] = sort(opersets[i]);
                            opersets[i] = new List<Point> { opersets[i][opersets[i].Count / 2] };
                            break;
                        }
                        else {
                            List<Point> currfrontiers = new List<Point>(get_frontiers(opersets[i]));
                            if (currfrontiers.Count < opersets[i].Count) {
                                foreach (var el in currfrontiers) {
                                    opersets[i].Remove(el);
                                }
                            }
                            else {
                                List<int> rank = new ();// 0 - id, 1 - rank
                                foreach (var el in currfrontiers) {
                                    rank.Add(calcrank(currfrontiers, el));
                                }
                                int maxind = 0; int maxrank = rank[maxind]; 
                                for (int j = 1; j < currfrontiers.Count; j++) {
                                    if (rank[j] > maxrank) {
                                        maxind = j;
                                        maxrank = rank[j];
                                    }
                                }
                                opersets[i] = new List<Point> { new Point(currfrontiers[maxind].X, currfrontiers[maxind].Y) };
                            }
                        }
                    }
                    dataGridView2.Rows[opersets[i][0].X].Cells[opersets[i][0].Y].Style.BackColor = Color.DarkKhaki;
                    curr_points.Add(new(opersets[i][0].X, opersets[i][0].Y));
                }                
                Cursor.Current = Cursors.Default;

                //waving from current centres to determine a min radius for each
                for (int t = 0; t < curr_points.Count; t++) {
                    var currfront = new List<Point> (get_frontiers(owingpoints[t]));
                    decimal[,] curr_values = new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    Point[,] previousl = new Point[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];

                    x = curr_points[t].X; x1 = x;
                    y = curr_points[t].Y; y1 = y;

                    vis1 = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    for (int i = 0; i < own.dataGridView1.RowCount; i++)
                        for (int j = 0; j < own.dataGridView1.ColumnCount; j++){
                            vis1[i, j] = !owingpoints[t].Contains(new Point(i,j));
                            curr_values[i, j] = -2;
                        }
                    vis1[x, y] = true;
                    previousl[x, y] = new(-2, -2);
                    curr_values[x, y] = 0;

                    while (!checkallvisl(vis1)){                        
                        calculcell(owingpoints[t], ref curr_values, ref previousl, x1 - 1, y1 - 1);
                        calculcell(owingpoints[t], ref curr_values, ref previousl, x1, y1 - 1);
                        calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + 1, y1 - 1);
                        calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + 1, y1);
                        calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + 1, y1 + 1);
                        calculcell(owingpoints[t], ref curr_values, ref previousl, x1, y1 + 1);
                        calculcell(owingpoints[t], ref curr_values, ref previousl, x1 - 1, y1 + 1);
                        calculcell(owingpoints[t], ref curr_values, ref previousl, x1 - 1, y1);
                        if (!radioButton1.Checked) {
                            calculcell(owingpoints[t], ref curr_values, ref previousl, x1 - 1, y1 - 2);
                            calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + 1, y1 - 2);
                            calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + 2, y1 - 1);
                            calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + 2, y1 + 1);
                            calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + 1, y1 + 2);
                            calculcell(owingpoints[t], ref curr_values, ref previousl, x1 - 1, y1 + 2);
                            calculcell(owingpoints[t], ref curr_values, ref previousl, x1 - 2, y1 + 1);
                            calculcell(owingpoints[t], ref curr_values, ref previousl, x1 - 2, y1 - 1);
                            if (!radioButton2.Checked) {
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 - 2, y1 - 3);
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 - 1, y1 - 3);
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + 1, y1 - 3);
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + 2, y1 - 3);
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + 3, y1 - 2);
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + 3, y1 - 1);
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + 3, y1 + 1);
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + 3, y1 + 2);
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + 2, y1 + 3);
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 + 1, y1 + 3);
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 - 1, y1 + 3);
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 - 2, y1 + 3);
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 - 3, y1 + 2);
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 - 3, y1 + 1);
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 - 3, y1 - 1);
                                calculcell(owingpoints[t], ref curr_values, ref previousl, x1 - 3, y1 - 2);
                            }
                        }
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
                        else if (points.Count > 1) { //if not, search nearest to (0,0) 
                            mindest = Decimal.MaxValue;
                            foreach (var p in points)
                                if (p.dest < mindest)
                                    mindest = p.dest;
                            var points1 = new List<Points>();
                            foreach (var p in points)
                                if (p.dest == mindest)
                                    points1.Add(new Points(p.xl, p.yl, (decimal)Math.Sqrt(Math.Pow(p.xl, 2) + Math.Pow(p.yl, 2))));
                            if (points1.Count == 1) { x1 = points1[0].xl; y1 = points1[0].yl; }//if it is alone to choose, equal et continue
                            else if (points1.Count > 1)  {//if not, choosing with least x
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
                    decimal minv = curr_values[currfront[0].X, currfront[0].Y] ;
                    for (int u = 1; u < currfront.Count; u++){
                        if (minv > curr_values[currfront[u].X, currfront[u].Y])
                            minv = curr_values[currfront[u].X, currfront[u].Y];
                    }
                    string formater = "0.##", formate = "0"; ;
                    if (numericUpDown6.Value > 2){
                        for (int n = 2; n < numericUpDown6.Value; n++)
                            formater += "#";
                    }
                    if (numericUpDown6.Value > 0) {
                        formate += ".";
                        for (int n = 2; n < numericUpDown6.Value; n++)
                            formate += "#";
                    }
                    dataGridView2.Rows[x].Cells[y].Value = Convert.ToDecimal(own.dataGridView1.Rows[x].Cells[y].Value).ToString(formate) +"("+ minv.ToString(formater)+")";
                    dataGridView2.AutoResizeColumns();
                    dataGridView2.AutoResizeRows();
                }

            }
            if (forfait != null) forfait.renew();
        }
        private List<Point> get_frontiers (List<Point> set) {
            List<Point> frontl = new(); 
            foreach (var el in set) {
                if (if_front(set, el)) {
                    frontl.Add(el);
                }
            }
            return frontl;
        }
        private int calcrank(List<Point> set, Point point){
            int count = 0;
            if (set.Contains(new Point(point.X - 1, point.Y - 1))) count++;
            if (set.Contains(new Point(point.X - 1, point.Y    ))) count += 2;
            if (set.Contains(new Point(point.X - 1, point.Y + 1))) count++;
            if (set.Contains(new Point(point.X    , point.Y + 1))) count += 2;
            if (set.Contains(new Point(point.X + 1, point.Y + 1))) count++;
            if (set.Contains(new Point(point.X + 1, point.Y    ))) count += 2;
            if (set.Contains(new Point(point.X + 1, point.Y - 1))) count++;
            if (set.Contains(new Point(point.X    , point.Y - 1))) count += 2;
            return count;
        }
        private bool if_front(List<Point> set, Point point) {
            if (!set.Contains(new Point(point.X - 1, point.Y - 1))) return true;
            if (!set.Contains(new Point(point.X - 1, point.Y    ))) return true;
            if (!set.Contains(new Point(point.X - 1, point.Y + 1))) return true;
            if (!set.Contains(new Point(point.X    , point.Y + 1))) return true;
            if (!set.Contains(new Point(point.X + 1, point.Y + 1))) return true;
            if (!set.Contains(new Point(point.X + 1, point.Y    ))) return true;
            if (!set.Contains(new Point(point.X + 1, point.Y - 1))) return true;
            if (!set.Contains(new Point(point.X    , point.Y - 1))) return true;
            return false;
        }
        private List<Point> sort (List<Point> set){
            List<Point> sorted = set;
            if (set[0].X == set[1].X) {
                List<List<int>> igrecs = new();
                for (int i = 0; i < set.Count; i++) {
                    igrecs.Add( new List<int> { i, set[i].Y} );
                }
                igrecs = QSort(igrecs);
                sorted = new();
                foreach (var el in igrecs) {
                    sorted.Add(set[el[0]]);
                }
            }
            else if (set[0].Y == set[1].Y) {
                List<List<int>> exs = new();
                for (int i = 0; i < set.Count; i++)
                {
                    exs.Add(new List<int> { i, set[i].X });
                }
                exs = QSort(exs);
                sorted = new();
                foreach (var el in exs)
                {
                    sorted.Add(set[el[0]]);
                }
            }
            return sorted;
        }
        private List<List<int>> QSort(List<List<int>> set) {
            int count = 1;
            while (count > 0) {
                count = 0;
                for (int i = 0; i < set.Count - 1; i++)
                    if (set[i][1] < set[i + 1][1]){
                        (set[i][0], set[i][1], set[i + 1][0], set[i + 1][1]) = (set[i + 1][0], set[i + 1][1], set[i][0], set[i][1]);
                        count++;
                    }
            }
            return set;
        }
        private bool if_not_a_ligne(List<Point> set){
            int currx0 = set[0].X, curry0 = set[0].Y, currx1 = set[1].X, curry1 = set[1].Y;
            if (currx0 == currx1 || curry0 == curry1) {
                if (currx0 == currx1) {
                    for (int i = 2; i < set.Count; i++) {
                        if (set[i].X != currx0) return true;
                    }
                    return false;
                }
                else if (curry0 == curry1) {
                    for (int i = 2; i < set.Count; i++) {
                        if (set[i].Y != curry0) return true;
                    }
                    return false;
                }
                return false;
            }
            else return true;
        }
        private bool ifacentrepoint(int x, int y) {
            for (int i = 0; i < dataGridView1.RowCount; i++) {
                if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == x + 1 && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == y + 1)
                    return true;
            }
            return false;
        }
        private int mindest(int x, int y) {
            decimal minv = destins[0][x,y];
            int ind = 0;
            for (int i = 1; i < destins.Count; i++){
                if (minv > destins[i][x, y]){
                    minv = destins[i][x, y];
                    ind = i;
                }
            }
            return ind;
        }
        private bool chckpoints(){
            curr = new List<Point>();
            for (int i = 0; i < dataGridView1.RowCount; i++)
                curr.Add(new(Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value) - 1, Convert.ToInt32(dataGridView1.CurrentRow.Cells[1].Value) - 1));
            for (int i = 0; i < dataGridView1.RowCount; i++)
                if (!accessible[curr[i].X, curr[i].Y])
                    return false;
            return true;
        }
        private bool chckrespoints(){
            for (int i = 0; i < own.dataGridView1.RowCount; i++)
                for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                    if (!vis1[i, j]) return false;
            return true;
        }
        private void reseachpoints(int u, int v) {
            if (!chckrespoints() && Convert.ToDecimal(own.dataGridView1.Rows[u].Cells[v].Value) >= 0) {
                for (byte i = 0; i < 8; i++) {
                    if (!visiteda(u, v, i)){
                        switch (i) {
                            case 0: { vis1[u - 1, v - 1] = true; accessible[u - 1, v - 1] = true; reseachpoints(u - 1, v - 1); break; }
                            case 1: { vis1[u    , v - 1] = true; accessible[u    , v - 1] = true; reseachpoints(u    , v - 1); break; }
                            case 2: { vis1[u + 1, v - 1] = true; accessible[u + 1, v - 1] = true; reseachpoints(u + 1, v - 1); break; }
                            case 3: { vis1[u + 1, v    ] = true; accessible[u + 1, v    ] = true; reseachpoints(u + 1, v    ); break; }
                            case 4: { vis1[u + 1, v + 1] = true; accessible[u + 1, v + 1] = true; reseachpoints(u + 1, v + 1); break; }
                            case 5: { vis1[u    , v + 1] = true; accessible[u    , v + 1] = true; reseachpoints(u    , v + 1); break; }
                            case 6: { vis1[u - 1, v + 1] = true; accessible[u - 1, v + 1] = true; reseachpoints(u - 1, v + 1); break; }
                            case 7: { vis1[u - 1, v    ] = true; accessible[u - 1, v    ] = true; reseachpoints(u - 1, v    ); break; }
                        }
                    }
                }
            }
        }
        private bool visiteda(int xc, int yc, byte i){
            try {
                switch (i){
                    case 0: return vis1[xc - 1, yc - 1];
                    case 1: return vis1[xc, yc - 1];
                    case 2: return vis1[xc + 1, yc - 1];
                    case 3: return vis1[xc + 1, yc];
                    case 4: return vis1[xc + 1, yc + 1];
                    case 5: return vis1[xc, yc + 1];
                    case 6: return vis1[xc - 1, yc + 1];
                    case 7: return vis1[xc - 1, yc];
                }
                return false;
            }
            catch (Exception) { return true; }
        }
        private void calculcell(int i, int xc, int yc){
            if (xc + 1 > 0 && yc + 1 > 0 && xc < own.dataGridView1.RowCount && yc < own.dataGridView1.ColumnCount && own.dataGridView1.Rows[xc].Cells[yc].Value != null && pointavailiter(xc, yc) && destins[i][x1, y1] != -1)
            {//if exists
                decimal cur = (decimal)((Convert.ToDouble(own.dataGridView1.Rows[xc].Cells[yc].Value) + Convert.ToDouble(own.dataGridView1.Rows[x1].Cells[y1].Value)) / 2 * Math.Sqrt(Math.Pow(xc - x1, 2) + Math.Pow(yc - y1, 2)) + Convert.ToDouble(destins[i][x1, y1]));
                if (destins[i][xc, yc] == -2) {//if not visited at all
                    destins[i][xc, yc] = cur;
                    previos[i][xc, yc] = new Point(x1, y1);
                }
                else if (cur < destins[i][xc, yc]) {//if from this point is shorter than known path
                    destins[i][xc, yc] = cur;
                    previos[i][xc, yc] = new Point(x1, y1);
                }
                else if (Convert.ToDecimal(own.dataGridView1.Rows[xc].Cells[yc].Value) == -1) {  //if point unpassable
                    destins[i][xc, yc] = -1;
                    previos[i][xc, yc] = new Point(-1, -1);
                }
            }
        }
        private void calculcell(List<Point> set,ref decimal[,] dest, ref Point[,] prev, int xc, int yc) {
            if (set.Contains(new Point(xc,yc))) {//if consists
                decimal cur = Convert.ToDecimal(((Convert.ToDouble(own.dataGridView1.Rows[xc].Cells[yc].Value) + Convert.ToDouble(own.dataGridView1.Rows[x1].Cells[y1].Value)) / 2 * Math.Sqrt(Math.Pow(xc - x1, 2) + Math.Pow(yc - y1, 2)) + Convert.ToDouble(dest[x1, y1])));
                if (dest[xc, yc] == -2)  {//if not visited at all
                    dest[xc, yc] = cur;
                    prev[xc, yc] = new Point(x1, y1);
                }
                else if (cur < dest[xc, yc]) {//if from this point is shorter than known path
                    dest[xc, yc] = cur;
                    prev[xc, yc] = new Point(x1, y1);
                }
                else if (Convert.ToDecimal(own.dataGridView1.Rows[xc].Cells[yc].Value) == -1){  //if point unpassable
                    dest[xc, yc] = -1;
                    prev[xc, yc] = new Point(-1, -1);
                }
            }
        }
        private bool checkallvis() {
            for (int i = 0; i < own.dataGridView1.RowCount; i++)
                for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                    if (!vis[i, j]) return false;
            return true;
        }
        private bool checkallvisl(bool[,] set){
            for (int i = 0; i < own.dataGridView1.RowCount; i++)
                for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                    if (!set[i, j]) return false;
            return true;
        }
        private bool pointavailiter(int i, int j){
            try{
                if (pointdest(i, j) == Math.Sqrt(2) || pointdest(i, j) == 1)
                    return availpoint(i, j);
                else if (pointdest(i, j) == Math.Sqrt(5)) {
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
                else if (pointdest(i, j) == Math.Sqrt(10)){
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
                else if (pointdest(i, j) == Math.Sqrt(13)){
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
        private void button6_Click(object sender, EventArgs e){
            bool t = true;
            int xl = Convert.ToInt32(numericUpDown3.Value);
            int yl = Convert.ToInt32(numericUpDown4.Value);
            if (Convert.ToDecimal(own.dataGridView1.Rows[xl-1].Cells[yl-1].Value) < 0) t = false;
            else{
                for (int i = 0; i < dataGridView1.RowCount; i++){
                    if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == xl && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == yl){
                        t = false;
                        break;
                    }
                }
            }
            if (t) {
                dataGridView1.Rows.Add(new object[] { xl, yl });
                refr();
            }
        }
        private void button5_Click(object sender, EventArgs e){
            int xl = Convert.ToInt32(numericUpDown3.Value);
            int yl = Convert.ToInt32(numericUpDown4.Value);
            for (int i = 0; i < dataGridView1.RowCount; i++) 
                if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == xl && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == yl){
                    dataGridView1.Rows.RemoveAt(i);
                    break;
                }
            refr();
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e) { refr(); }
        private void radioButton2_CheckedChanged(object sender, EventArgs e) { refr(); }
        private void radioButton3_CheckedChanged(object sender, EventArgs e) { refr(); }
        private void Vran_FormClosing(object sender, FormClosingEventArgs e){ own.vran = null; }

        private void lesForfaitsToolStripMenuItem_Click(object sender, EventArgs e){
            if (forfait == null){
                forfait = new(this);
                if (own.lang == 1) forfait.toRusse();
                forfait.Show();                
            }
            forfait.Focus();
        }
        private void numericUpDown6_ValueChanged(object sender, EventArgs e) {
            dataGridView2.DefaultCellStyle.Format = 'N' + numericUpDown6.Value.ToString();
            //dataGridView1.DefaultCellStyle.Format = 'N' + numericUpDown6.Value.ToString();
            dataGridView2.AutoResizeColumns();
            dataGridView2.AutoResizeRows();
        }
        private void numericUpDown5_ValueChanged(object sender, EventArgs e){
            dataGridView2.DefaultCellStyle.Font = new Font("Palatino Linotype", (float)numericUpDown5.Value);
            //dataGridView1.DefaultCellStyle.Font = new Font("Palatino Linotype", (float)numericUpDown5.Value);
            dataGridView2.AutoResizeColumns();
            dataGridView2.AutoResizeRows();
        }
        internal void toFrancais() {
            groupBox1.Text = "Les points";
            groupBox4.Text = "Reflexion de cellules";
            label7.Text = "La taille de fonte";
            label8.Text = "La quantité de signs";
            button4.Text = "Computer";
            button5.Text = "Suppremer";
            button6.Text = "Aujouter";
            groupBox2.Text = "Le destin de chercher de chemins";
            radioButton3.Text = "III rayon (32 directions)";
            radioButton2.Text = "II rayon (16 directions)";
            radioButton1.Text = "I rayon (8 directions)";
            button2.Text = "Suppremer";
            label4.Text = "La ligne";
            label3.Text = "La colonne";
            Text = "Voronoi";
            Column1.HeaderText = "La ligne";
            Column2.HeaderText = "La colonne";

            lesForfaitsToolStripMenuItem.Text = "Les forfaits";

            if (forfait != null) forfait.toFrancais();
        }
        internal void toRusse(){
            groupBox1.Text = "Точки";
            groupBox4.Text = "Отображение ячеек";
            label7.Text = "Размер шрифта";
            label8.Text = "Количество знаков";
            button4.Text = "Вычислить";
            button5.Text = "Удалить";
            button6.Text = "Добавить";
            groupBox2.Text = "Кол-во направлений поиска";
            radioButton3.Text = "III радиус (32 направления)";
            radioButton2.Text = "II радиус (16 направления)";
            radioButton1.Text = "I радиус (8 направлений)";
            button2.Text = "Удалить";
            label4.Text = "Строка";
            label3.Text = "Столбец";
            Text = "Диаграмма Вороного";

            Column1.HeaderText = "Строка";
            Column2.HeaderText = "Стоблец";
            lesForfaitsToolStripMenuItem.Text = "Упаковки";

            if (forfait != null) forfait.toRusse();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e){
            if (dataGridView1.SelectedCells[0].RowIndex < dataGridView1.RowCount) {
                numericUpDown3.Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                numericUpDown4.Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[1].Value);
            }
            else {
                numericUpDown3.Value = 1000;
                numericUpDown4.Value = 1000;
            }
        }
    }
}
