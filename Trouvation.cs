using static l_application_pour_diploma.Classes;
namespace l_application_pour_diploma
{
    public partial class Trouvation : Form
    {
        internal Commencement own;
        internal Routes? r;
        internal int x = 1, y = 1, x1 = 1, y1 = 1;
        internal bool[,] vis, vis1;
        internal Point[,] previos;
        internal List<List<Point>> submedia;
        bool onemedium = true;
        internal List<decimal[,]> variants;
        int curr_med = 0;
        public Trouvation(Commencement o)
        {
            InitializeComponent();
            own = o;
        }
        private bool checkallvis()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    if (!vis[i, j] && submedia[0].Contains(new(x, y)))
                        return false;
            return true;
        }
        private int currst(decimal dest, decimal chaque)
        {
            int shift = (int)own.numericUpDown3.Value;
            int currstage = ((int)Math.Floor(dest / chaque)) % variants.Count;
            if (currstage > 0 && dest % chaque == 0)
                currstage--;
            return ((currstage + shift) % variants.Count);
        }
        private void calculcell(int xc, int yc)
        {
            if (xc + 1 > 0 && yc + 1 > 0 && xc < dataGridView1.RowCount && yc < dataGridView1.ColumnCount && own.dataGridView1.Rows[xc].Cells[yc].Value != null && pointavailiter(xc, yc))
            {//if exists
                decimal chaque = own.numericUpDown11.Value;
                int currstage = currst(Convert.ToDecimal(dataGridView1.Rows[x1].Cells[y1].Value), chaque);

                decimal cur = (decimal)((Convert.ToDouble(variants[currstage][xc, yc]) + Convert.ToDouble(variants[currstage][x1, y1])) / 2 * Math.Sqrt(Math.Pow(xc - x1, 2) + Math.Pow(yc - y1, 2)) + Convert.ToDouble(dataGridView1.Rows[x1].Cells[y1].Value));

                if (currstage != (currst(cur, chaque)) && variants.Count > 1)
                {
                    int diff = (currstage + (int)Math.Abs(currstage - Math.Floor(cur / chaque))) % variants.Count;
                    cur = (decimal)((Convert.ToDouble(variants[diff][xc, yc]) + Convert.ToDouble(variants[diff][x1, y1])) / 2 * Math.Sqrt(Math.Pow(xc - x1, 2) + Math.Pow(yc - y1, 2)) + Convert.ToDouble(dataGridView1.Rows[x1].Cells[y1].Value));
                }

                if (dataGridView1.Rows[xc].Cells[yc].Value == null)
                {//if not visited at all
                    dataGridView1.Rows[xc].Cells[yc].Value = cur;
                    previos[xc, yc] = new Point(x1, y1);
                }
                else if (cur < Convert.ToDecimal(dataGridView1.Rows[xc].Cells[yc].Value))
                {//if from this point is shorter than known path
                    dataGridView1.Rows[xc].Cells[yc].Value = cur;
                    previos[xc, yc] = new Point(x1, y1);
                }
                else if (Convert.ToDecimal(own.dataGridView1.Rows[xc].Cells[yc].Value) == -1)
                {  //if point unpassable
                    dataGridView1.Rows[xc].Cells[yc].Value = -1;
                }
            }
        }
        private int determ_submed(Point p)
        {
            for (int i = 0; i < submedia.Count; i++)
                if (submedia[i].Contains(p)) return i;
            return 0;
        }
        internal void refresh(bool changed_med){
            own.insert_log("Refreshing the shortest path...", this);
            variants = new() { own.source[0] };
            if (own.radioButton1.Checked && own.checkBox4.Checked)
            {
                decimal[,] inverseMatrix = new decimal[own.source[0].GetLength(0), own.source[0].GetLength(1)];
                for (int i = 0; i < own.source[0].GetLength(0); i++)
                    for (int j = 0; j < own.source[0].GetLength(1); j++)
                        if (!own.checkBox2.Checked && own.source[0][i, j] > 0 || own.checkBox2.Checked && own.domains.Any(l => l.Contains(new(i, j))))
                            inverseMatrix[i, j] = 1 / own.source[0][i, j];

                variants.Add(inverseMatrix);
            }
            else if ((own.radioButton2.Checked) && own.checkBox4.Checked)
            {
                decimal multi = (100 + own.numericUpDown9.Value) / 100;
                for (int t = 0; t < (int)own.numericUpDown8.Value; t++)
                {

                    decimal[,] next = new decimal[own.source[0].GetLength(0), own.source[0].GetLength(1)];
                    for (int i = 0; i < own.source[0].GetLength(0); i++)
                        for (int j = 0; j < own.source[0].GetLength(1); j++)
                            if (!own.checkBox2.Checked && own.source[0][i, j] > 0 || own.checkBox2.Checked && own.domains.Any(l => l.Contains(new(i, j))))
                                next[i, j] = variants[^1][i, j] * multi;
                            else next[i, j] = variants[^1][i, j];
                    variants.Add(next);
                }

            }

            Cursor.Current = Cursors.WaitCursor;
            dataGridView1.RowCount = own.dataGridView1.RowCount;
            dataGridView1.ColumnCount = own.dataGridView1.ColumnCount;
            x = dataGridView1.SelectedCells[0].RowIndex; x1 = x;
            y = dataGridView1.SelectedCells[0].ColumnIndex; y1 = y;

            numericUpDown1.Value = x + 1;
            numericUpDown2.Value = y + 1;
            previos = new Point[dataGridView1.RowCount, dataGridView1.ColumnCount];
            vis = new bool[dataGridView1.RowCount, dataGridView1.ColumnCount];
            vis1 = new bool[dataGridView1.RowCount, dataGridView1.ColumnCount];
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    vis1[i, j] = false;
                }
            vis1[x, y] = true;
            if (changed_med)
                reseachpoints(x, y);
            curr_med = determ_submed(new(x, y));
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    if (availpoint(i, j) && availpoint(x1, y1) && submedia[curr_med].Contains(new(i, j)))
                    {
                        dataGridView1.Rows[i].Cells[j].Value = null;
                        vis[i, j] = false;
                    }
                    else
                    {
                        vis[i, j] = true;
                        dataGridView1.Rows[i].Cells[j].Value = -1;
                        previos[i, j] = new Point(-1, -1);
                    }
                }
            dataGridView1.Rows[x].Cells[y].Value = 0;
            while (!checkallvis())
            {
                vis[x1, y1] = true;
                for (int v = 0; v < size_rayon(); v++)
                    calculcell(x1 + voisins[v].X, y1 + voisins[v].Y);
                //transmission le point actuel a le point le plus proche et minimum
                var frontiers = new List<Points>();
                for (int i = 0; i < dataGridView1.RowCount; i++)//searching frontier points
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                        if (dataGridView1.Rows[i].Cells[j].Value != null && !vis[i, j])
                            frontiers.Add(new Points(i, j, Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value)));
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
                        int minx = dataGridView1.RowCount;
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
            Cursor.Current = Cursors.Default;
            own.dataGridView1.ClearSelection();
            own.dataGridView1.CurrentCell = own.dataGridView1.Rows[x].Cells[y];
            own.textBox1.Text = Convert.ToString(x + 1);
            own.textBox2.Text = Convert.ToString(y + 1);
            if (checkBox1.Checked) fillcolors();
            else clearcolors();
            dataGridView1.AutoResizeColumns();
            r?.findroute();
            decimal sumtotal = 0;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    decimal currv = Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value);
                    if (currv > 0) sumtotal += currv;
                }

            }
            textBox1.Text = sumtotal.ToString("0.##");
            own.insert_log("The shortest path refreshed.", this);
        }
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
        private void reseachpoints(int u, int v)
        {
            submedia = new() { new() };
            submedia[0].Add(new(u, v));
            bool[,] known = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
            for (int i = 0; i < own.dataGridView1.RowCount; i++)
                for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                {
                    if (Convert.ToDecimal(own.dataGridView1.Rows[i].Cells[j].Value) > -1)
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

        private bool if_all_known(bool[,] known)
        {
            for (int i = 0; i < known.GetLength(0); i++)
                for (int j = 0; j < known.GetLength(1); j++)
                    if (!known[i, j]) return false;
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
        private bool availpoint(int u, int v) { return Convert.ToDecimal(own.dataGridView1.Rows[u].Cells[v].Value) >= 0; }
        public void clearcells()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    dataGridView1.Rows[i].Cells[j].Value = null;
        }
        private double pointdest(int xc, int yc) { return Math.Sqrt(Math.Pow(x1 - xc, 2) + Math.Pow(y1 - yc, 2)); }
        private double pointdest(int xc1, int yc1, int xc2, int yc2) { return Math.Sqrt(Math.Pow(xc1 - xc2, 2) + Math.Pow(yc1 - yc2, 2)); }
        private void Trouvation_Load(object sender, EventArgs e) { refresh(true); }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (x != dataGridView1.SelectedCells[0].RowIndex || y != dataGridView1.SelectedCells[0].ColumnIndex)
            //    refresh(false);
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (x != dataGridView1.SelectedCells[0].RowIndex || y != dataGridView1.SelectedCells[0].ColumnIndex)
                refresh(false);
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e) { refresh(false); }
        private void radioButton2_CheckedChanged(object sender, EventArgs e) { refresh(false); }
        private void radioButton3_CheckedChanged(object sender, EventArgs e) { refresh(false); }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e) { }
        private void Trouvation_FormClosing(object sender, FormClosingEventArgs e)
        {
            own.trouv = null;
            r?.Close();
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) fillcolors();
            else clearcolors();
        }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.DefaultCellStyle.Format = 'N' + numericUpDown3.Value.ToString();
            dataGridView1.AutoResizeColumns();
        }
        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.DefaultCellStyle.Font = new Font("Palatino Linotype", (float)numericUpDown4.Value);
            dataGridView1.AutoResizeColumns();
        }
        private void desCalculationsDeCheminsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (r == null && checkBox1.Checked)
            {
                r = new Routes(this);
                if (own.lang == 1) r.toRusse();
                r.Show();
            }
            else if (checkBox1.Checked) r.Focus();
            if (!checkBox1.Checked) MessageBox.Show("Il n'y a pas de carte de chaleur!");
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e) { }
        internal void toRusse()
        {
            groupBox1.Text = "Количество направлений поиска";
            radioButton3.Text = "III радиус (32 направления)";
            radioButton2.Text = "II радиус (16 направлений)";
            radioButton1.Text = "I радиус (8 направлений)";
            label2.Text = "Столбец";
            label3.Text = "Строка";
            groupBox2.Text = "Выбранная ячейка";
            groupBox3.Text = "Отображение ячеек";
            label1.Text = "Размер шрифта";
            label4.Text = "Количество знаков";
            Text = "Запуск волны из точки";
            groupBox4.Text = "Тепловая карта расстояний";
            checkBox1.Text = "Показать карту";
            desCalculationsDeCheminsToolStripMenuItem.Text = "Вычисление наикратчайших путей";
            desFenêtresToolStripMenuItem.Text = "Окна";
            sauvegarderToolStripMenuItem.Text = "Сохранить";
            chargerToolStripMenuItem.Text = "Загрузить";
            toolStripMenuItem1.Text = "Файл";
            r?.toRusse();
        }
        internal void toFrancais()
        {
            groupBox1.Text = "Le destin de chercher de chemins";
            radioButton3.Text = "III rayon (32 directions)";
            radioButton2.Text = "II rayon (16 directions)";
            radioButton1.Text = "I rayon (8 directions)";
            label2.Text = "La colonne";
            label3.Text = "La ligne";
            groupBox2.Text = "Cellule selectée";
            groupBox3.Text = "Reflexion de cellules";
            label1.Text = "La taille de fonte";
            label4.Text = "La quantité de signs";
            Text = "Trouvation un chemin le plus court";
            groupBox4.Text = "La carte de chaleur des destances";
            checkBox1.Text = "Afficher la carte";
            desCalculationsDeCheminsToolStripMenuItem.Text = "Calculations de routes la plus courtes";
            desFenêtresToolStripMenuItem.Text = "Fenêtres";
            sauvegarderToolStripMenuItem.Text = "Sauvegarder";
            chargerToolStripMenuItem.Text = "Charger";
            toolStripMenuItem1.Text = "Ficher";
            r?.toFrancais();
        }
        internal void clearcolors()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
        }
        private void fillcolors()
        {
            int r = own.dataGridView1.RowCount;
            int c = own.dataGridView1.ColumnCount;
            dataGridView1.RowCount = r;
            dataGridView1.ColumnCount = c;
            decimal maxval = 0, minval = Decimal.MaxValue;
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                {
                    var d = Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value);
                    if (maxval < d)
                        maxval = d;
                    if (minval > d && d > -1)
                        minval = d;
                }
            if (minval != maxval)
                for (int i = 0; i < r; i++)
                    for (int j = 0; j < c; j++)
                    {
                        if (Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) != -1)
                        {
                            var d = Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) / maxval;
                            if (d < (decimal)0.2)
                                dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.BlueViolet;
                            else if (d >= (decimal)0.2 && d! < (decimal)0.3)
                                dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Blue;
                            else if (d >= (decimal)0.3 && d! < (decimal)0.4)
                                dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.SkyBlue;
                            else if (d >= (decimal)0.4 && d! < (decimal)0.5)
                                dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.SeaGreen;
                            else if (d >= (decimal)0.5 && d! < (decimal)0.6)
                                dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Green;
                            else if (d >= (decimal)0.6 && d! < (decimal)0.7)
                                dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.GreenYellow;
                            else if (d >= (decimal)0.7 && d! < (decimal)0.8)
                                dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Yellow;
                            else if (d >= (decimal)0.8 && d! < (decimal)0.9)
                                dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Orange;
                            else if (d >= (decimal)0.9)
                                dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Red;
                        }
                        else dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;
                    }
            else for (int i = 0; i < r; i++)
                    for (int j = 0; j < c; j++)
                    {
                        if (Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) != -1)
                        {
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Blue;
                        }
                        else dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;
                    }
            dataGridView1.Rows[x].Cells[y].Style.BackColor = Color.Violet;
            dataGridView1.AutoResizeColumns();
        }
    }
}
