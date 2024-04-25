using System.Runtime.ConstrainedExecution;
using System.Linq;
using static l_application_pour_diploma.Classes;
using Color = System.Drawing.Color;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.EMMA;
using System.Diagnostics;
using Microsoft.VisualBasic.ApplicationServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Bibliography;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.CustomUI;

namespace l_application_pour_diploma{
    public partial class Vran : Form
    {
        internal Commencement own;
        private List<Color> ColeurList = new List<Color> {
            Color.Red, Color.Orange, Color.Yellow, Color.YellowGreen, Color.GreenYellow,
            Color.Green, Color.DarkGreen, Color.SkyBlue, Color.Cyan, Color.BlueViolet,
            Color.OrangeRed, Color.MediumBlue, Color.DarkBlue,
            Color.Violet, Color.Silver, Color.Gold, Color.SeaGreen, Color.AliceBlue,
            Color.Azure, Color.DarkViolet, Color.DeepSkyBlue, Color.DeepPink, Color.DarkOrchid};

        internal List<decimal> minrads;
        internal List<Point> curr_points;
        internal Packs? forfait;
        internal List<decimal[,]> wave_de_points;
        internal List<List<Point>> owingpoints;
        internal List<List<Point>> submedia;
        bool onemedium = true;
        int curr_med = 0;
        int size_rayon = 32;
        internal List<decimal[,]> variants;
        private static readonly object locker = new object();
        public Vran(Commencement o)
        {
            InitializeComponent();
            own = o;
            curr_points = new();
        }
        private bool availpoint(int u, int v) { return own.source[0][u, v] >= 0; }
        private void Vran_Load(object sender, EventArgs e)
        {
            refr(true);
            reseachpoints(0, 0);
            comboBox1.SelectedIndex = 2;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.SelectedIndex = 0;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
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
        private decimal[,] waving_in_domain_from_point(List<Point> medium, Point commenc)
        {
            decimal[,] destinl = new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
            Point[,] previosl = new Point[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
            int xlc = commenc.X; int xlc1 = xlc;
            int ylc = commenc.Y; int ylc1 = ylc;

            bool[,] vis = new bool[dataGridView2.RowCount, dataGridView2.ColumnCount];
            bool[,] accessl = new bool[dataGridView2.RowCount, dataGridView2.ColumnCount];

            Parallel.For(0, own.dataGridView1.RowCount, i =>
            {
                for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                {
                    if (medium.Contains(new(i, j)))
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
                for (int i = 0; i < own.dataGridView1.RowCount; i++)//searching frontier points
                    for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
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
                        int minx = own.dataGridView1.RowCount;
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
            var fr = get_frontiers(medium);
            var minim = decimal.MaxValue;
            foreach (var el in fr)
            {
                if (destinl[el.X, el.Y] < minim)
                {
                    minim = destinl[el.X, el.Y];
                }
            }
            return destinl;
        }
        private decimal trouv_radius(List<Point> medium, Point centrel)
        {
            var wave = waving_in_domain_from_point(medium, centrel);
            var frontl = get_frontiers(medium);
            //Point min_point = new();
            decimal minim = frontl
                .Select(point => wave[point.X, point.Y]) // Select the corresponding wave value for each frontier point
                .Where(value => value > 0) // Filter out negative values
                .DefaultIfEmpty(decimal.MaxValue) // If there are no positive values, set the default value to decimal.MaxValue
                .Min(); // Find the minimum value
            return minim;
        }
        private int determ_submed(Point p)
        {
            for (int i = 0; i < submedia.Count; i++)
                if (submedia[i].Contains(p))
                    return i;
            return 0;
        }
        internal void refr(bool changed_med)
        {
            own.insert_log("Refreshing the diagram...", this);
            wave_de_points = new();
            own.insert_log("Determining the pages of the media...", this);
            variants = new() { own.source[0] };
            List<decimal[,]> destins;
            List<Point[,]> previos;
            owingpoints = new();
            if (own.radioButton1.Checked && own.checkBox4.Checked)
            { //if inversion
                decimal[,] inverseMatrix = new decimal[own.source[0].GetLength(0), own.source[0].GetLength(1)];
                for (int i = 0; i < own.source[0].GetLength(0); i++)
                    for (int j = 0; j < own.source[0].GetLength(1); j++)
                        if (!own.checkBox2.Checked && own.source[0][i, j] > 0 //if domains not used and the first value is greater than 0
                            || own.checkBox2.Checked && own.domains.Any(l => l.Contains(new(i, j)))) // or  domains used and the point is in
                            inverseMatrix[i, j] = 1 / own.source[0][i, j];

                variants.Add(inverseMatrix);
            }
            else if (own.radioButton2.Checked && own.checkBox4.Checked) { //if regular changing set by main form
                decimal multi = (100 + own.numericUpDown9.Value) / 100;
                for (int t = 0; t < (int)own.numericUpDown8.Value; t++) {
                    decimal[,] next = new decimal[own.source[0].GetLength(0), own.source[0].GetLength(1)];
                    for (int i = 0; i < own.source[0].GetLength(0); i++)
                        for (int j = 0; j < own.source[0].GetLength(1); j++)
                            if (!own.checkBox2.Checked && own.source[0][i, j] > 0 //if domains not used and the first value is greater than 0
                                || own.checkBox2.Checked && own.domains.Any(l => l.Contains(new(i, j)))) //or domains used and the point is in
                                next[i, j] = variants[^1][i, j] * multi;
                            else next[i, j] = variants[^1][i, j];
                    variants.Add(next);
                }
            }
            own.insert_log("The pages of the media determined...", this);
            dataGridView2.RowCount = own.dataGridView1.RowCount;
            dataGridView2.ColumnCount = own.dataGridView1.ColumnCount;
            for (int i = 0; i < dataGridView2.RowCount; i++)
                for (int j = 0; j < dataGridView2.ColumnCount; j++) {
                    dataGridView2.Rows[i].Cells[j].Value = own.source[0][i, j];
                    dataGridView2.Rows[i].Cells[j].Style.BackColor = Color.White;
                }
            dataGridView2.AutoResizeColumns();
            if (changed_med) {
                own.insert_log("The media changed! Checking if it is still unified...", this);
                reseachpoints(0, 0);
            }
            own.insert_log("Proceeding current points...", this);
            if (dataGridView1.RowCount > 0) {
                if (curr_points.Count > 0 && curr_points.Count == dataGridView1.RowCount) {
                    dataGridView1.RowCount = 0;
                    foreach (var el in curr_points)
                        dataGridView1.Rows.Add(new object[] { (el.X + 1), (el.Y + 1) });
                }
                curr_points = new();
                List<int> randcol = new List<int>();
                own.insert_log("Choosing colours...", this);
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

                for (int l = 0; l < dataGridView1.RowCount; l++) { //l - for point

                    destins.Add(new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount]);
                    previos.Add(new Point[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount]);

                    int xlc = Convert.ToInt32(dataGridView1.Rows[l].Cells[0].Value) - 1, xlc1 = xlc;
                    int ylc = Convert.ToInt32(dataGridView1.Rows[l].Cells[1].Value) - 1, ylc1 = ylc;
                    own.insert_log($"Launching wave from {l + 1}/{dataGridView1.RowCount} ({xlc},{ylc} (raw)) point...", this);
                    bool[,] vis = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    bool[,] vis1 = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    Parallel.For(0, own.dataGridView1.RowCount, i =>
                    {
                        //for (int i = 0; i < own.dataGridView1.RowCount; i++)
                        for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                        {
                            vis1[i, j] = false;
                        }
                    });
                    vis1[xlc, ylc] = true;
                    curr_med = determ_submed(new(xlc, ylc));
                    previos[l][xlc, ylc] = new(-2, -2);
                    Parallel.For(0, own.dataGridView1.RowCount, i => {
                        //for (int i = 0; i < own.dataGridView1.RowCount; i++)
                        for (int j = 0; j < own.dataGridView1.ColumnCount; j++) {
                            if (availpoint(i, j) && availpoint(xlc1, ylc1) && submedia[curr_med].Contains(new(i, j))) {
                                destins[l][i, j] = -2;
                                vis[i, j] = false;
                            }
                            else {
                                vis[i, j] = true;
                                destins[l][i, j] = -1;
                                previos[l][i, j] = new Point(-1, -1);
                            }
                        }
                    });
                    destins[l][xlc, ylc] = 0;
                    while (!checkallvis(vis)){
                        vis[xlc1, ylc1] = true;
                        Parallel.For(0, size_rayon, i => { calculcell(l, xlc1 + voisins[i].X, ylc1 + voisins[i].Y, ref destins, ref previos, xlc1, ylc1); });

                        //transmission le point actuel a le point le plus proche et minimum
                        var frontiers = new List<Points>();
                        for (int i = 0; i < own.dataGridView1.RowCount; i++)//searching frontier points
                            for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                                if (destins[l][i, j] != -2 && !vis[i, j])
                                    frontiers.Add(new Points(i, j, destins[l][i, j]));
                        decimal mindest = Decimal.MaxValue;
                        object lock_mindest = new();
                        foreach (var p in frontiers)//searching the minimum points
                            if (p.dest < mindest)
                                lock (lock_mindest) {
                                    mindest = p.dest;
                                }
                        var points = new List<Points>();
                        object lock_frontiers = new();
                        Parallel.ForEach(frontiers, p => {
                            if (p.dest == mindest)
                                lock (frontiers)
                                    points.Add(new Points(p.xl, p.yl, (decimal)Math.Sqrt(Math.Pow(xlc1 - p.xl, 2) + Math.Pow(ylc1 - p.yl, 2))));
                        });
                        if (points.Count == 1) { xlc1 = points[0].xl; ylc1 = points[0].yl; }//if it is alone to choose, equal et continue
                        else if (points.Count > 1) { //if not, search nearest to (0,0) 
                            mindest = decimal.MaxValue;
                            foreach (var p in points)
                                if (p.dest < mindest)
                                    mindest = p.dest;
                            var points1 = new List<Points>();
                            foreach (var p in points)
                                if (p.dest == mindest)
                                    points1.Add(new Points(p.xl, p.yl, (decimal)Math.Sqrt(Math.Pow(p.xl, 2) + Math.Pow(p.yl, 2))));
                            if (points1.Count == 1) { xlc1 = points1[0].xl; ylc1 = points1[0].yl; }//if it is alone to choose, equal et continue
                            else if (points1.Count > 1) {//if not, choosing with least x
                                int minx = own.dataGridView1.RowCount;
                                foreach (var p in points1)
                                    if (p.xl < minx)
                                        minx = p.xl;
                                foreach (var p in points1)
                                    if (p.xl == minx) {
                                        xlc1 = points1[0].xl;
                                        ylc1 = points1[0].yl;
                                        break;
                                    }
                            }
                        }
                    }
                    own.insert_log("Wave launched.", this);
                }
                own.insert_log("Claiming points to domains...", this);
                owingpoints = new(); //list of sets of chosen points
                for (int i = 0; i < dataGridView1.RowCount; i++){
                    Point p = new(Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) - 1,
                            Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) - 1);
                    List<Point> lp = new() { p };
                    owingpoints.Add(lp);
                }
                //filling with a coleurs
                Parallel.For(0, dataGridView2.RowCount, i =>{
                    //for (int i = 0; i < dataGridView2.RowCount; i++)
                    for (int j = 0; j < dataGridView2.ColumnCount; j++){
                        if (own.source[0][i, j] <= 0)
                            dataGridView2.Rows[i].Cells[j].Style.BackColor = Color.Black;
                        else if (!ifacentrepoint(i, j)){
                            int h = mindest(i, j, destins);
                            dataGridView2.Rows[i].Cells[j].Style.BackColor = ColeurList[randcol[h % randcol.Count]];
                            owingpoints[h].Add(new Point(i, j));
                        }
                    }
                });
                //var opersets = new List<List<Point>> (owingpoints);
                var opersets = new List<List<Point>>();
                //foreach (var sub in owingpoints)
                Parallel.ForEach(owingpoints, sub =>{
                    List<Point> s = new();
                    foreach (var el in sub) {
                        s.Add(el);
                    }
                    opersets.Add(s);
                });
                minrads = new();
                own.insert_log($"        Domains established. Determining centres...", this);
                object locker_waves = new();
                foreach (var subset in owingpoints) {//chosing the centre for each
                    Point centre = new();
                    if (comboBox2.SelectedIndex == 0) {
                                                //decimal[,] currwave = new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                        decimal[,] waves = new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                        var subset_front = get_frontiers(subset);
                        //launching waves from frontiers
                        Parallel.ForEach(subset_front, el => {
                            decimal[,] wavesl = waving_in_domain_from_point(subset, el);
                            lock (locker_waves){
                                for (int i = 0; i < wavesl.GetLength(0); i++) {
                                    for (int j = 0; j < wavesl.GetLength(1); j++){
                                        waves[i, j] += wavesl[i, j];
                                    }
                                }
                            }
                        });
                        decimal minl = decimal.MaxValue;
                        for (int i = 0; i < waves.GetLength(0); i++) {
                            for (int j = 0; j < waves.GetLength(1); j++)  {
                                if (minl > waves[i, j] && waves[i, j] > 0) {
                                    lock (waves) {
                                        minl = waves[i, j];
                                        centre = new(i, j);
                                    }
                                }
                            }
                        }
                    }
                    else if (comboBox2.SelectedIndex == 1){
                        List<Points2> poiEtRadii = new();
                        Parallel.ForEach(subset, el => {
                            poiEtRadii.Add(new(el, trouv_radius(subset, el)));
                        });
                        centre = poiEtRadii.OrderByDescending(poi => poi.dest).First().point;

                    }

                    wave_de_points.Add(waving_in_domain_from_point(subset, centre));
                    dataGridView2.Rows[centre.X].Cells[centre.Y].Style.BackColor = Color.DarkKhaki;
                    curr_points.Add(new(centre.X, centre.Y));

                    string formater = "0.##", formate = "0"; ;
                    if (numericUpDown6.Value > 2) {
                        for (int n = 2; n < numericUpDown6.Value; n++)
                            formater += "#";
                    }
                    if (numericUpDown6.Value > 0) {
                        formate += ".";
                        for (int n = 2; n < numericUpDown6.Value; n++)
                            formate += "#";
                    }
                    minrads.Add(trouv_radius(subset, centre));
                    dataGridView2.Rows[centre.X].Cells[centre.Y].Value = own.source[0][centre.X, centre.Y].ToString(formate) + "(" + minrads[^1].ToString(formater) + ")";
                    label5.Text = $"Sum de radii = {minrads.Sum()}";
                }

                Cursor.Current = Cursors.Default;

                //waving from current centres to determine a min radius for each


            }
            dataGridView2.AutoResizeColumns();
            dataGridView2.AutoResizeRows();
            Focus();
            own.insert_log("Points claimed to domains. Diagram refreshed.", this);
            forfait?.renew();
        }
        internal List<Point> get_frontiers(List<Point> set){
            List<Point> frontl = new();
            object lock_front = new();
            Parallel.ForEach(set, el =>{
                if (if_front(set, el)){
                    lock(lock_front)
                        frontl.Add(el);
                }
            });
            return frontl;
        }
        private bool if_front(List<Point> set, Point point){
            for (int i = 0; i < 8; i++)
                if (!set.Contains(new(point.X + voisins[i].X, point.Y + voisins[i].Y)))
                    return true;
            return false;
        }
        private bool ifacentrepoint(int x, int y){
            for (int i = 0; i < dataGridView1.RowCount; i++){
                if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == x + 1 && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == y + 1)
                    return true;
            }
            return false;
        }
        private int mindest(int x, int y, List<decimal[,]> curr_destins){
            decimal minv = curr_destins[0][x, y];
            int ind = 0;
            for (int i = 1; i < curr_destins.Count; i++){
                if (minv > curr_destins[i][x, y]){
                    minv = curr_destins[i][x, y];
                    ind = i;
                }
            }
            return ind;
        }
        private void reseachpoints(int u, int v){
            submedia = new() { new() };
            submedia[0].Add(new(u, v));
            bool[,] known = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
            for (int i = 0; i < own.dataGridView1.RowCount; i++)
                for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                {
                    if (own.source[0][i, j] > -1)
                        known[i, j] = false;
                    else known[i, j] = true;
                }
            known[u, v] = true;
            int currx = u, curry = v;

            List<Point> curr_front = new();
            for (int i = 0; i < 8; i++){
                if (pointavailiter(currx, curry, currx + voisins[i].X, curry + voisins[i].Y)) {
                    submedia[0].Add(new(currx + voisins[i].X, curry + voisins[i].Y));
                    curr_front.Add(new(currx + voisins[i].X, curry + voisins[i].Y));
                    known[currx + voisins[i].X, curry + voisins[i].Y] = true;
                }
            }
            while (true){
                List<Point> curr_prev = new(curr_front);
                curr_front = new();
                foreach (var el in curr_prev){
                    for (int i = 0; i < 8; i++){
                        Point voisepoint = new(el.X + voisins[i].X, el.Y + voisins[i].Y);
                        if (pointavailiter(el.X, el.Y, el.X + voisins[i].X, el.Y + voisins[i].Y)) {
                            if (!curr_front.Contains(voisepoint) && !submedia[0].Contains(voisepoint))
                                curr_front.Add(voisepoint);
                            if (!submedia[0].Contains(voisepoint)) {
                                submedia[0].Add(voisepoint);
                                known[el.X + voisins[i].X, el.Y + voisins[i].Y] = true;
                            }
                        }
                    }
                }
                if (curr_front.Count == 0) break;
            }
            if (!if_all_known(known)){
                onemedium = false;
                own.insert_log("    The media checked. It is not unified.", this);
            }
            else
            {
                onemedium = true;
                own.insert_log("    The media checked. It is unified.", this);
            }
        }
        //private double pointdest(int xc1, int yc1, int xc2, int yc2) { return Math.Sqrt(Math.Pow(xc1 - xc2, 2) + Math.Pow(yc1 - yc2, 2)); }
        private bool pointavailiter(int x1, int y1, int x2, int y2) { //vieux, nouveau
            try{
                if (pointdest(x1, y1, x2, y2) == Math.Sqrt(2) || pointdest(x1, y1, x2, y2) == 1)
                    return availpoint(x2, y2);
                else if (pointdest(x1, y1, x2, y2) == Math.Sqrt(5)) {
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
                else if (pointdest(x1, y1, x2, y2) == Math.Sqrt(10)){
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
                else if (pointdest(x1, y1, x2, y2) == Math.Sqrt(13)) {
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

        private void calculcell(int i, int xc, int yc, ref List<decimal[,]> curr_destins, ref List<Point[,]> curr_previos, int xl1, int yl1)
        {
            if (xc + 1 > 0 && yc + 1 > 0 && xc < own.dataGridView1.RowCount && yc < own.dataGridView1.ColumnCount && own.source[0][xc, yc] > 0 && pointavailiter(xc, yc, xl1, yl1) && curr_destins[i][xl1, yl1] != -1)
            {//if exists
                decimal chaque = own.numericUpDown11.Value;
                int currstage = currst(curr_destins[i][xl1, yl1], chaque);
                decimal cur = (decimal)((Convert.ToDouble(variants[currstage][xc, yc]) + Convert.ToDouble(variants[currstage][xl1, yl1])) / 2 * Math.Sqrt(Math.Pow(xc - xl1, 2) + Math.Pow(yc - yl1, 2)) + Convert.ToDouble(curr_destins[i][xl1, yl1]));
                if (currstage != currst(cur, chaque) && variants.Count > 1)
                {
                    int diff = (currstage + (int)Math.Abs(currstage - Math.Floor(cur / chaque))) % variants.Count;
                    cur = (decimal)((Convert.ToDouble(variants[diff][xc, yc]) + Convert.ToDouble(variants[diff][xl1, yl1])) / 2 * Math.Sqrt(Math.Pow(xc - xl1, 2) + Math.Pow(yc - yl1, 2)) + Convert.ToDouble(curr_destins[i][xl1, yl1]));
                }
                if (curr_destins[i][xc, yc] == -2)
                {//if not visited at all
                    curr_destins[i][xc, yc] = cur;
                    curr_previos[i][xc, yc] = new Point(xl1, yl1);
                }
                else if (cur < curr_destins[i][xc, yc])
                {//if from this point is shorter than known path
                    curr_destins[i][xc, yc] = cur;
                    curr_previos[i][xc, yc] = new Point(xl1, yl1);
                }
                else if (own.source[0][xc, yc] == -1)
                {  //if point unpassable
                    curr_destins[i][xc, yc] = -1;
                    curr_previos[i][xc, yc] = new Point(-1, -1);
                }
            }
        }
        private int currst(decimal dest, decimal chaque)
        {
            int shift = (int)own.numericUpDown3.Value;
            int currstage = (int)Math.Floor(dest / chaque) % variants.Count;
            if (currstage > 0 && dest % chaque == 0)
                currstage--;
            return (currstage + shift) % variants.Count;
        }
        private void calculcell(ref bool[,] access, ref decimal[,] dest, ref Point[,] prev, int xc, int yc, int xl, int yl)
        {
            if (xc >= 0 && yc >= 0 && xc < dataGridView2.RowCount && yc < dataGridView2.ColumnCount && access[xc, yc])
            {//if consists
                decimal chaque = own.numericUpDown11.Value;
                int currstage = currst(dest[xl, yl], chaque);
                decimal cur = Convert.ToDecimal((Convert.ToDouble(variants[currstage][xc, yc]) + Convert.ToDouble(variants[currstage][xl, yl])) / 2 * Math.Sqrt(Math.Pow(xc - xl, 2) + Math.Pow(yc - yl, 2)) + Convert.ToDouble(dest[xl, yl]));
                if (currstage != currst(cur, chaque) && variants.Count > 1)
                {
                    int diff = (currstage + (int)Math.Abs(currstage - Math.Floor(cur / chaque))) % variants.Count;
                    cur = Convert.ToDecimal((Convert.ToDouble(variants[diff][xc, yc]) + Convert.ToDouble(variants[diff][xl, yl])) / 2 * Math.Sqrt(Math.Pow(xc - xl, 2) + Math.Pow(yc - yl, 2)) + Convert.ToDouble(dest[xl, yl]));
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
        private bool checkallvis(bool[,] set)
        {
            for (int i = 0; i < own.dataGridView1.RowCount; i++)
                for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                    if (!set[i, j]) return false;
            return true;
        }

        private double pointdest(int xc, int yc, int x1, int y1) { return Math.Sqrt(Math.Pow(x1 - xc, 2) + Math.Pow(y1 - yc, 2)); }
        private void button6_Click(object sender, EventArgs e)
        {//adding point
            foreach (DataGridViewCell sel_cell in dataGridView2.SelectedCells)
            {
                bool toadd = true;
                int xl = sel_cell.RowIndex;
                int yl = sel_cell.ColumnIndex;
                if (own.source[0][xl, yl] < 0)
                    toadd = false;
                else
                {
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == xl && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == yl)
                        {
                            toadd = false;
                            break;
                        }
                    }
                    if (toadd)
                        dataGridView1.Rows.Add(new object[] { sel_cell.RowIndex + 1, sel_cell.ColumnIndex + 1 });
                }

            }
            //dataGridView2.SelectedCells.Clear();
            refr(false);
        }
        private void button5_Click(object sender, EventArgs e){
            var poiForDel = new List<Point>();
            foreach (DataGridViewCell sel_cell in dataGridView2.SelectedCells){
                poiForDel.Add(new(sel_cell.RowIndex, sel_cell.ColumnIndex));
            }
            for (int i = 0; i < dataGridView1.RowCount; i++){
                if (poiForDel.Count > 0){
                    for (int j = 0; j < poiForDel.Count; j++)
                        if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == poiForDel[j].X + 1 && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == poiForDel[j].Y + 1){
                            poiForDel.RemoveAt(j);
                            j--;
                            dataGridView1.Rows.RemoveAt(i);
                    }
                }
                else break;
            }
            //dataGridView2.SelectedCells.Clear();            
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
        private void button1_Click(object sender, EventArgs e){
            var cers = (int)numericUpDown1.Value;
            var effs = (int)numericUpDown2.Value;
            DateTime start_stamp = DateTime.Now;
            own.insert_log($"Starting multistart with {effs} efforts for {cers} circles...", this);
            dataGridView1.RowCount = 0;
            Random r = new();
            List<List<Point>> finis = new();//locker
            List<List<decimal>> minrads = new();
            //own.insert_log("    Checking if media unified...", this);
            //reseachpoints(0, 0);
            for (int k = 0; k < effs; k++){
                DateTime start_effort_stamp = DateTime.Now;
                List<List<Point>> starts = new(), medieval;
                List<decimal> medieval_sums = new() { 0 };
                List<decimal[,]> destinsl;
                List<Point[,]> previosl;
                List<List<Point>> owingpointsl;
                List<Point> genered = new();
                List<decimal> mins = new();
                own.insert_log($"    {k + 1} effort: Generating starting points...", this);
                for (int j = 0; j < cers; j++)
                { // generating new start points
                    Point cu;
                    do
                    {
                        cu = new(r.Next(own.dataGridView1.RowCount), r.Next(own.dataGridView1.ColumnCount));
                    } while (own.source[0][cu.X, cu.Y] < 0 || genered.Contains(cu));
                    genered.Add(cu);
                }
                starts.Add(genered);
                own.insert_log($"    {k + 1} effort: Starting points: {{{string.Join(", ", genered.Select(p => $"({p.X}, {p.Y})"))}}}", this);
                Cursor.Current = Cursors.WaitCursor;
                medieval = new() { genered };
                own.insert_log($"    {k + 1} effort: Searching the optimum...", this);
                int do_cnt = 0;
                do
                {
                    do_cnt++;
                    curr_points = new();
                    owingpoints = new();
                    destinsl = new();
                    previosl = new();

                    for (int l = 0; l < cers; l++)
                    {
                        destinsl.Add(new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount]);
                        previosl.Add(new Point[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount]);

                        int xlc = Convert.ToInt32(medieval[^1][l].X), xlc1 = xlc;
                        int ylc = Convert.ToInt32(medieval[^1][l].Y), ylc1 = ylc;
                        own.insert_log($"        {k + 1} effort, {do_cnt} iteration: Launching wave from {l + 1}/{cers} point ({xlc},{ylc} (raw))...", this);
                        bool[,] vis = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                        bool[,] vis1 = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                        Parallel.For(0, own.dataGridView1.RowCount, i =>
                        {
                            for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                            {
                                vis1[i, j] = false;
                            }
                        });
                        vis1[xlc, ylc] = true;
                        curr_med = determ_submed(new(xlc, ylc));
                        previosl[l][xlc, ylc] = new(-2, -2);
                        Parallel.For(0, own.dataGridView1.RowCount, i =>
                        {
                            //for (int i = 0; i < own.dataGridView1.RowCount; i++)
                            for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                            {
                                if (availpoint(i, j) && availpoint(xlc1, ylc1) && submedia[curr_med].Contains(new(i, j)))
                                {
                                    destinsl[l][i, j] = -2;
                                    vis[i, j] = false;
                                }
                                else
                                {
                                    vis[i, j] = true;
                                    destinsl[l][i, j] = -1;
                                    previosl[l][i, j] = new Point(-1, -1);
                                }
                            }
                        });
                        destinsl[l][xlc, ylc] = 0;

                        while (!checkallvis(vis))
                        {
                            vis[xlc1, ylc1] = true;
                            Parallel.For(0, size_rayon, i => { calculcell(l, xlc1 + voisins[i].X, ylc1 + voisins[i].Y, ref destinsl, ref previosl, xlc1, ylc1); });

                            //transmission le point actuel a le point le plus proche et minimum
                            var frontiers = new List<Points>();
                            object locker_front = new();
                            Parallel.For(0, own.dataGridView1.RowCount, i =>
                            {
                                //for (int i = 0; i < own.dataGridView1.RowCount; i++)//searching frontier points
                                for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                                    if (destinsl[l][i, j] != -2 && !vis[i, j])
                                        lock (locker_front)
                                            frontiers.Add(new Points(i, j, destinsl[l][i, j]));
                            });
                            decimal mindest = decimal.MaxValue;
                            object locker_mindest = new();
                            Parallel.ForEach(frontiers, p =>
                            { //searching the minimum points
                                if (p.dest < mindest)
                                    lock (locker_mindest)
                                        mindest = p.dest;
                            });
                            var points = new List<Points>();
                            object locker_frontiers = new();
                            Parallel.ForEach(frontiers, p =>
                            {
                                if (p.dest == mindest)
                                    lock (locker_frontiers)
                                        points.Add(new Points(p.xl, p.yl, (decimal)Math.Sqrt(Math.Pow(xlc1 - p.xl, 2) + Math.Pow(ylc1 - p.yl, 2))));
                            });
                            if (points.Count == 1) { xlc1 = points[0].xl; ylc1 = points[0].yl; }//if it is alone to choose, equal et continue
                            else if (points.Count > 1)
                            { //if not, search nearest to (0,0) 
                                mindest = decimal.MaxValue;
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
                                    int minx = own.dataGridView1.RowCount;
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

                    }
                    own.insert_log($"        {k + 1} effort, {do_cnt} iteration: Waves from points launched. Claiming the domains...", this);
                    owingpointsl = new(); //list of sets of chosen points to keep
                    List<List<Point>> opersets = new(); //list of sets of chosen points to operate
                    Parallel.ForEach(medieval[^1], po =>
                    {
                        List<Point> lp = new() { po };
                        owingpointsl.Add(lp);
                        opersets.Add(lp);
                    });
                    object[] locker_owing_add = new object[owingpointsl.Count];
                    Array.Fill(locker_owing_add, new());

                    Parallel.For(0, dataGridView2.RowCount, i =>
                    {
                        for (int j = 0; j < dataGridView2.ColumnCount; j++)
                        {
                            if (own.source[0][i, j] <= 0) { }
                            else if (!ifacentrepoint(i, j))
                            {
                                int h = mindest(i, j, destinsl);
                                lock (locker_owing_add[h])
                                {
                                    owingpointsl[h].Add(new Point(i, j));
                                    opersets[h].Add(new Point(i, j));
                                }
                            }
                        }
                    });
                    List<Point> curr_pointsl = new();
                    mins = new();
                    own.insert_log($"        {k + 1} effort, {do_cnt} iteration: Domains established. Determining centres...", this);
                    foreach (var subset in owingpointsl)
                    {//chosing the centre for each
                        Point centre = new();
                        if (comboBox2.SelectedIndex == 0)
                        {
                            decimal[,] waves = new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                            var subset_front = get_frontiers(subset);
                            //launching waves from frontiers
                            //foreach (var el in subset_front){
                            object locker_wave = new();
                            Parallel.ForEach(subset_front, el =>
                            {
                                decimal[,] wavesl = waving_in_domain_from_point(subset, el);
                                for (int i = 0; i < wavesl.GetLength(0); i++)
                                {
                                    for (int j = 0; j < wavesl.GetLength(1); j++)
                                    {
                                        lock (locker_wave)
                                        {
                                            waves[i, j] += wavesl[i, j];
                                        }
                                    }
                                }
                            });
                            decimal minl = decimal.MaxValue;
                            for (int i = 0; i < waves.GetLength(0); i++)
                            {
                                for (int j = 0; j < waves.GetLength(1); j++)
                                {
                                    if (minl > waves[i, j])
                                    {
                                        if (waves[i, j] > 0)
                                            lock (locker_wave)
                                            {
                                                minl = waves[i, j];
                                                centre = new(i, j);
                                            }
                                    }
                                }
                            }
                        }
                        else if (comboBox2.SelectedIndex == 1)
                        {
                            List<Points2> poiEtRadii = new();
                            Parallel.ForEach(subset, el =>
                            {
                                poiEtRadii.Add(new(el, trouv_radius(subset, el)));
                            });
                            centre = poiEtRadii.OrderByDescending(poi => poi.dest).First().point;

                        }
                        mins.Add(trouv_radius(subset, centre));
                        // wave_de_pointsl.Add(waves);
                        curr_pointsl.Add(centre);
                        own.insert_log($"        {k + 1} effort, {do_cnt} iteration: The centre determined. The point: ({centre.X}, {centre.Y}).The radius = {mins[^1]}", this);
                    }
                    medieval_sums.Add(mins.Sum());
                    own.insert_log($"        {k + 1} effort, {do_cnt} iteration: Centres determined. The points: {{{string.Join(",", curr_pointsl.Select(p => $"({p.X}, {p.Y})"))}}}.", this);
                    own.insert_log($"        {k + 1} effort, {do_cnt} iteration: The sum of radii = {medieval_sums[^1]}", this);
                    medieval.Add(curr_pointsl);
                    //if (medival_sums[^1] <= medival_sums[^2]) break;
                    if (checkIfHasNonUnique(medieval))
                        break;
                }
                while (true);
                minrads.Add(mins);
                finis.Add(medieval[^1]);
                own.insert_log($"    {k + 1} effort: Optimum found in {do_cnt} iterations and {DateTime.Now - start_effort_stamp}.", this);
            }//);
            own.insert_log("Efforts ceased.", this);
            decimal maxsum = 0;
            int indmax = 0;
            for (int i = 0; i < minrads.Count; i++)
            {
                decimal sum = minrads[i].Sum();
                //foreach (var p in minrads[i]){ sum += p; }
                if (sum > maxsum)
                {
                    maxsum = sum;
                    indmax = i;
                }
            }

            //owingpoints and  to determine
            foreach (var el in finis[indmax])
                dataGridView1.Rows.Add(new object[] { (el.X + 1), (el.Y + 1) });
            own.insert_log($"The best effort is the {indmax + 1}. Its points: {{{string.Join(",", finis[indmax].Select(p => $"({p.X}, {p.Y})"))}}}.The sum of radii = {maxsum}. Taken time: {DateTime.Now - start_stamp}", this);
            own.insert_log($"Taken time: {DateTime.Now - start_stamp}", this);
            refr(false);
        }

        private bool checkIfHasNonUnique(List<List<Point>> listl)
        {
            for (int i = 0; i < listl.Count; i++)
                for (int j = i + 1; j < listl.Count; j++)
                    if (AreListsDePointsEqual(listl[i], listl[j])) return true;
            return false;
        }
        static bool AreListsDePointsEqual(List<Point> list1, List<Point> list2)
        {
            if (list1.Count != list2.Count) return false;
            else
            {
                for (int i = 0; i < list1.Count; i++)
                    if (list1[i].X != list2[i].X || list1[i].Y != list2[i].Y)
                        return false;
            }
            return true;
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

            comboBox1.Items[0] = "III rayon (32 directions)";
            comboBox1.Items[1] = "II rayon (16 directions)";
            comboBox1.Items[2] = "I rayon (8 directions)";

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
            label7.Text = "Размер шрифта";
            label8.Text = "Количество знаков";
            button1.Text = "Мультистарт";
            button3.Text = "Найти оптимальные центры";
            button4.Text = "Пересчитать";
            button5.Text = "Удалить";
            button6.Text = "Добавить";
            comboBox1.Items[2] = "III радиус (32 направления)";
            comboBox1.Items[1] = "II радиус (16 направления)";
            comboBox1.Items[0] = "I радиус (8 направлений)";
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

        private void button3_Click(object sender, EventArgs e)
        {
            List<decimal[,]> destins = new();
            List<Point[,]> previos;
            List<Point> genered = new();
            List<decimal> mins = new();
            List<List<Point>> starts = new(), finis = new(), medieval;
            List<decimal> medival_sums = new();
            List<List<decimal>> minrads = new();
            List<List<Point>> owingpoints;
            int cers = dataGridView1.RowCount;
            for (int j = 0; j < cers; j++)
            { // getting current points
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

                    int x = Convert.ToInt32(medieval[^1][l].X), x1 = x;
                    int y = Convert.ToInt32(medieval[^1][l].Y), y1 = y;

                    bool[,] vis = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    bool[,] vis1 = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
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

                    while (!checkallvis(vis))
                    {
                        vis[x1, y1] = true;
                        for (int i = 0; i < size_rayon; i++)
                            calculcell(l, x1 + voisins[i].X, y1 + voisins[i].Y, ref destins, ref previos, x1, y1);

                        //transmission le point actuel a le point le plus proche et minimum
                        var frontiers = new List<Points>();
                        for (int i = 0; i < own.dataGridView1.RowCount; i++)//searching frontier points
                            for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                                if (destins[l][i, j] != -2 && !vis[i, j])
                                    frontiers.Add(new Points(i, j, destins[l][i, j]));
                        decimal mindest = decimal.MaxValue;
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
                            mindest = decimal.MaxValue;
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
                    Point p = new(po.X, po.Y);
                    List<Point> lp = new() { p };
                    owingpoints.Add(lp);
                }
                for (int i = 0; i < dataGridView2.RowCount; i++)
                    for (int j = 0; j < dataGridView2.ColumnCount; j++)
                    {
                        if (own.source[0][i, j] <= 0) { }
                        else if (!ifacentrepoint(i, j))
                        {
                            int h = mindest(i, j, destins);
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
                minrads = new() { new() };
                foreach (var subset in owingpoints)
                {//chosing the centre for each
                    Point centre = new();
                    decimal[,] waves = new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    var subset_front = get_frontiers(subset);
                    Parallel.ForEach(subset_front, el =>
                    {
                        decimal[,] wavesl = waving_in_domain_from_point(subset, el);
                        lock (locker)
                        {
                            for (int i = 0; i < wavesl.GetLength(0); i++)
                            {
                                for (int j = 0; j < wavesl.GetLength(1); j++)
                                {
                                    waves[i, j] += wavesl[i, j];
                                }
                            }
                        }
                    });
                    wave_de_points.Add(waves);
                    decimal minl = decimal.MaxValue;
                    for (int i = 0; i < waves.GetLength(0); i++)
                    {
                        for (int j = 0; j < waves.GetLength(1); j++)
                        {
                            if (minl > waves[i, j])
                            {
                                lock (waves)
                                {
                                    minl = waves[i, j];
                                    centre = new(i, j);
                                }
                            }
                        }
                    }
                    mins.Add(trouv_radius(subset, centre));
                    // wave_de_pointsl.Add(waves);
                    curr_points.Add(new(centre.X, centre.Y));

                    dataGridView2.Rows[centre.X].Cells[centre.Y].Style.BackColor = Color.DarkKhaki;
                }

                medieval.Add(curr_points);
                medival_sums.Add(mins.Sum());
                //if (medival_sums[^1]! > medival_sums[^2]) break;
                if (checkIfHasNonUnique(medieval))
                    break;
            }
            while (true);
            finis.Add(medieval[^2]);
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
            for (int i = 0; i < cers; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = finis[0][i].X;
                dataGridView1.Rows[i].Cells[1].Value = finis[0][i].Y;
                dataGridView2.Rows[finis[0][i].X].Cells[finis[0][i].Y].Value = own.source[0][finis[0][i].X, finis[0][i].Y].ToString(formate) + "(" + minrads[0][i].ToString(formater) + ")";
            }
            refr(false);
            Cursor.Current = Cursors.Default;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0) size_rayon = 8;
            else if (comboBox1.SelectedIndex == 1) size_rayon = 16;
            else size_rayon = 32;
        }
    }
}