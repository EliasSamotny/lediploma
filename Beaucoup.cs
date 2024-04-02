using static l_application_pour_diploma.Classes;
namespace l_application_pour_diploma{
    public partial class Beaucoup : Form{
        Commencement own;
        List<decimal[,]> destins;
        internal List<Point[,]> previos;
        int d;
        internal int x = 1, y = 1, x1 = 1, y1 = 1,x2,y2;
        List<Point> curr;
        internal bool[,] vis, vis1, accessible;
        public Beaucoup(Commencement o){
            InitializeComponent();
            own = o;
        }
        private void Beaucoup_FormClosing(object sender, FormClosingEventArgs e) { own.beaucoup = null; }
        private void Beaucoup_Load(object sender, EventArgs e){ 
            fillpict();
            dataGridView1.ColumnCount = 2;
            dataGridView1.Rows.Clear();
        }
        private void fillpict() {
            Bitmap bmp = new(pictureBox1.Width, pictureBox1.Height);
            Graphics carte = Graphics.FromImage(bmp);
            d = Math.Min(pictureBox1.Width, pictureBox1.Height) / Math.Max(own.dataGridView1.RowCount, own.dataGridView1.ColumnCount);
            for (int i = 0; i < own.dataGridView1.RowCount; i++){
                for (int j = 0; j < own.dataGridView1.Columns.Count; j++) {
                    carte.DrawRectangle(new Pen(Color.White), j * d, i * d, d, d);
                    carte.FillRectangle(new SolidBrush(own.dataGridView1.Rows[i].Cells[j].Style.BackColor), j * d + 1, i * d + 1, d - 1, d - 1);
                }

            }
            pictureBox1.Image = bmp;
        }
        internal void refr(){

            if (dataGridView1.Rows.Count > 1){
                own.insert_log("Refreshing the nearest point...", this);
                destins = new ();
                previos = new ();

                Cursor.Current = Cursors.WaitCursor;
                for (int l = 0; l < dataGridView1.RowCount; l++){
                    //own.insert_log("Checking " + l.ToString() + " point...");
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
                        calculcell(l,x1 - 1, y1 - 1);
                        calculcell(l, x1, y1 - 1);
                        calculcell(l, x1 + 1, y1 - 1);
                        calculcell(l, x1 + 1, y1);
                        calculcell(l, x1 + 1, y1 + 1);
                        calculcell(l, x1, y1 + 1);
                        calculcell(l, x1 - 1, y1 + 1);
                        calculcell(l, x1 - 1, y1);
                        if (!radioButton1.Checked){
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
                            else if (points1.Count > 1)
                            {//if not, choosing with least x
                                int minx = own.dataGridView1.RowCount;
                                foreach (var p in points1)
                                    if (p.xl < minx)
                                        minx = p.xl;
                                foreach (var p in points1)
                                    if (p.xl == minx){
                                        x1 = points1[0].xl;
                                        y1 = points1[0].yl;
                                        break;
                                    }
                            }
                        }
                    }
                }
                decimal[,] maxdim = new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount]; 
                int x3 = -1, y3 = -1; //meeting
                for (int i = 0; i < own.dataGridView1.RowCount;i++)
                    for (int j = 0; j < own.dataGridView1.ColumnCount; j++) {
                        maxdim[i, j] = maxval(i, j);
                    }
                decimal min = Decimal.MaxValue;
                for (int i = 0; i < own.dataGridView1.RowCount; i++)
                    for (int j = 0; j < own.dataGridView1.ColumnCount; j++) 
                        if (maxdim[i, j] < min){
                            min = maxdim[i, j];
                            x3 = i;
                            y3 = j;
                    }
                //own.insert_log("The nearest point calculated. Refreshing the image...");
                curr = new List<Point>();
                for (int i = 0; i < dataGridView1.RowCount; i++)
                    curr.Add(new(Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) - 1, Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) - 1));
                fillpict();
                Bitmap bmp = new(pictureBox1.Image);
                Graphics carte;
                for (int l = 0; l < destins.Count; l++) { //constr routes
                    x2 = curr[l].X;//point of destin
                    y2 = curr[l].Y;                    
                    if (previos[l][x3, y3].X != -1){
                        List<Point> route = new(){ new Point(x3, y3) };
                        int x4 = x3, x5 = x4, y4 = y3, y5 = y4; //for searching
                        while (x2 != x5 || y2 != y5) {
                            x4 = x5; y4 = y5; //current point
                            x5 = previos[l][x4, y4].X;
                            y5 = previos[l][x4, y4].Y;
                            route.Add(new Point(x5, y5));
                            if (x2 < 0)  break;
                        }
                        
                        bmp = new(pictureBox1.Image);
                        carte = Graphics.FromImage(bmp);
                        for (int i = route.Count - 1; i > 0; i--)  {
                            Point[] p1i = new Point[] {
                                new (route[i].Y * d + d / 2 - 1, route[i].X * d + d / 2 - 1),
                                new (route[i].Y * d + d / 2 - 1, route[i].X * d + d / 2),
                                new (route[i].Y * d + d / 2 - 1, route[i].X * d + d / 2 + 1),
                                new (route[i].Y * d + d / 2, route[i].X * d + d / 2 - 1),
                                new (route[i].Y * d + d / 2, route[i].X * d + d / 2),
                                new (route[i].Y * d + d / 2, route[i].X * d + d / 2 + 1),
                                new (route[i].Y * d + d / 2 + 1, route[i].X * d + d / 2 - 1),
                                new (route[i].Y * d + d / 2 + 1, route[i].X * d + d / 2),
                                new (route[i].Y * d + d / 2 + 1, route[i].X * d + d / 2 + 1)
                                    };
                            Point[] p2i = new Point[] {
                                new(route[i - 1].Y * d + d / 2 - 1, route[i - 1].X * d + d / 2 - 1),
                                new(route[i - 1].Y * d + d / 2 - 1, route[i - 1].X * d + d / 2),
                                new(route[i - 1].Y * d + d / 2 - 1, route[i - 1].X * d + d / 2 + 1),
                                new(route[i - 1].Y * d + d / 2, route[i - 1].X * d + d / 2 - 1),
                                new(route[i - 1].Y * d + d / 2, route[i - 1].X * d + d / 2),
                                new(route[i - 1].Y * d + d / 2, route[i - 1].X * d + d / 2 + 1),
                                new(route[i - 1].Y * d + d / 2 + 1, route[i - 1].X * d + d / 2 - 1),
                                new(route[i - 1].Y * d + d / 2 + 1, route[i - 1].X * d + d / 2),
                                new(route[i - 1].Y * d + d / 2 + 1, route[i - 1].X * d + d / 2 + 1),
                                    };
                            for (int j = 0; j < p1i.Length; j++)
                                carte.DrawLine(new Pen(Color.Black), p1i[j], p2i[j]);
                        }
                        pictureBox1.Image = bmp;
                        carte.FillEllipse(new SolidBrush(Color.Maroon), new Rectangle(y3 * d + d / 5, x3 * d + d / 5, 3 * d / 5, 3 * d / 5));
                    }
                    else{
                        MessageBox.Show("Le point final est inaccessible!");
                    }
                }


                //else MessageBox.Show("Tous les points ne sont pas dans la même zone de portée!");
                Cursor.Current = Cursors.Default;

            }
            own.insert_log("The nearest point refreshed.",this);
        }
        private decimal maxval(int xk,int yk){
            decimal s = 0;
            for (int i = 0; i < destins.Count; i++)
                if (destins[i][xk, yk] > 0) {
                    if (destins[i][xk, yk] > s)
                        s = destins[i][xk, yk];
                }                    
                else return Decimal.MaxValue;
            return s;
        }
        private bool chckpoints(){
            curr = new List <Point>();
            for (int i = 0; i < dataGridView1.RowCount; i++)
                curr.Add(new(Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value)-1,Convert.ToInt32(dataGridView1.CurrentRow.Cells[1].Value)-1));
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
        private void reseachpoints(int u, int v){
            if (!chckrespoints() && Convert.ToDecimal(own.dataGridView1.Rows[u].Cells[v].Value) >= 0) {
                for (byte i = 0; i < 8; i++) {
                    if (!visiteda(u, v, i)){
                        switch (i){
                            case 0:{
                                    vis1[u - 1, v - 1] = true;
                                    accessible[u - 1, v - 1] = true;
                                    reseachpoints(u - 1, v - 1); break;
                                }
                            case 1: { vis1[u, v - 1] = true; accessible[u, v - 1] = true; reseachpoints(u, v - 1); break; }
                            case 2: { vis1[u + 1, v - 1] = true; accessible[u + 1, v - 1] = true; reseachpoints(u + 1, v - 1); break; }
                            case 3: { vis1[u + 1, v] = true; accessible[u + 1, v] = true; reseachpoints(u + 1, v); break; }
                            case 4: { vis1[u + 1, v + 1] = true; accessible[u + 1, v + 1] = true; reseachpoints(u + 1, v + 1); break; }
                            case 5: { vis1[u, v + 1] = true; accessible[u, v + 1] = true; reseachpoints(u, v + 1); break; }
                            case 6: { vis1[u - 1, v + 1] = true; accessible[u - 1, v + 1] = true; reseachpoints(u - 1, v + 1); break; }
                            case 7: { vis1[u - 1, v] = true; accessible[u - 1, v] = true; reseachpoints(u - 1, v); break; }
                        }
                    }
                }
            }
        }
        private bool visiteda(int xc, int yc, byte i){
            try{
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
                if (destins[i][xc, yc] == -2){//if not visited at all
                    destins[i][xc, yc] = cur;
                    previos[i][xc, yc] = new Point(x1, y1);
                }             
                else if (cur < destins[i][xc, yc])
                {//if from this point is shorter than known path
                    destins[i][xc, yc] = cur;
                    previos[i][xc, yc] = new Point(x1, y1);
                }
                else if (Convert.ToDecimal(own.dataGridView1.Rows[xc].Cells[yc].Value) == -1)
                {  //if point unpassable
                    destins[i][xc, yc] = -1;
                    previos[i][xc, yc] = new Point(-1, -1);
                }
            }
        }
        private bool checkallvis(){
            for (int i = 0; i < own.dataGridView1.RowCount; i++)
                for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                    if (!vis[i, j]) return false;
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
        private void button3_Click(object sender, EventArgs e) {
            refr();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e){
            if (Convert.ToInt32(numericUpDown1.Value) > own.dataGridView1.RowCount)
                numericUpDown1.Value = own.dataGridView1.RowCount;
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e) {
            if (Convert.ToInt32(numericUpDown2.Value) > own.dataGridView1.ColumnCount)
                numericUpDown2.Value = own.dataGridView1.ColumnCount;
        }
        private void pictureBox1_Resize(object sender, EventArgs e) { refr(); }
        private bool availpoint(int u, int v) { return Convert.ToDecimal(own.dataGridView1.Rows[u].Cells[v].Value) >= 0; }
        private void radioButton1_CheckedChanged(object sender, EventArgs e){refr();}
        private void radioButton2_CheckedChanged(object sender, EventArgs e) {  }
        private void radioButton3_CheckedChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e){
            if (dataGridView1.SelectedCells[0].RowIndex < dataGridView1.RowCount) {
                numericUpDown3.Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                numericUpDown4.Value = Convert.ToDecimal(dataGridView1.Rows[e.RowIndex].Cells[1].Value);
            }else{
                numericUpDown3.Value = 1000;
                numericUpDown4.Value = 1000;
            }
        }
        private void button1_Click(object sender, EventArgs e){
            bool t = true;
            int xl = Convert.ToInt32(numericUpDown1.Value);
            int yl = Convert.ToInt32(numericUpDown2.Value);
            for (int i = 0; i < dataGridView1.RowCount; i++){
                    if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == xl && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == yl){
                        t = false;
                        break;
                    }
            }
            if (t) {                
                dataGridView1.Rows.Add(new Object[] { xl,yl });
            }
        }
        private void button2_Click(object sender, EventArgs e) {
            int xl = Convert.ToInt32(numericUpDown3.Value);
            int yl = Convert.ToInt32(numericUpDown4.Value);
            for (int i = 0; i < dataGridView1.RowCount; i++) {
               if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == xl && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == yl)
                        dataGridView1.Rows.RemoveAt(i);
                   
            }
        }
        internal void toFrancais(){
            Text = "Beaucoup";
            Column1.HeaderText = "La ligne";
            Column2.HeaderText = "La colonne";
            groupBox1.Text = "Les points";
            button3.Text = "Computer";
            groupBox2.Text = "Le destin de chercher de chemins";
            radioButton3.Text = "III rayon (32 directions)";
            radioButton2.Text = "II rayon (16 directions)";
            radioButton1.Text = "I rayon (8 directions)";
            button2.Text = "Suppremer";
            button1.Text = "Aujouter";
            label4.Text = "La ligne";
            label3.Text = "La colonne";
        }
        internal void toRusse() {
            Text = "Поиск точки наименьшего пути для множества точек";
            Column1.HeaderText = "Строка";
            Column2.HeaderText = "Стоблец";
            groupBox1.Text = "Точки";
            button3.Text = "Вычислить";
            groupBox2.Text = "Кол-во направлений поиска";
            radioButton3.Text = "III радиус (32 направления)";
            radioButton2.Text = "II радиус (16 направлений)";
            radioButton1.Text = "I радиус (8 направлений)";
            button2.Text = "Удалить";
            button1.Text = "Добавить";
            label4.Text = "Строка";
            label3.Text = "Столбец";
        }
    }
}
