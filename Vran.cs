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
            Color.GreenYellow, Color.Green, Color.DarkGreen, Color.SkyBlue, Color.Blue, Color.DarkBlue, Color.Cyan, Color.BlueViolet, 
            Color.Violet, Color.DarkViolet};
        List<decimal[,]> destins;
        internal List<Point[,]> previos;
        internal int x = 1, y = 1, x1 = 1, y1 = 1;
        List<Point> curr;
        internal bool[,] vis, vis1, accessible;
        public Vran(Commencement o) {
            InitializeComponent();
            own = o;
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
            if (dataGridView1.RowCount > 1) {
                List<int> randcol = new List<int>();
                for (int i = 0; i < dataGridView1.RowCount; i++) { //choosing les coleurs
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
                for (int l = 0; l < dataGridView1.RowCount; l++) {
                    destins.Add(new decimal[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount]);
                    previos.Add(new Point[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount]);

                    x = Convert.ToInt32(dataGridView1.Rows[l].Cells[0].Value) - 1; x1 = x;
                    y = Convert.ToInt32(dataGridView1.Rows[l].Cells[1].Value) - 1; y1 = y;

                    vis = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    vis1 = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    accessible = new bool[own.dataGridView1.RowCount, own.dataGridView1.ColumnCount];
                    for (int i = 0; i < own.dataGridView1.RowCount; i++)
                        for (int j = 0; j < own.dataGridView1.ColumnCount; j++)
                        {
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
                //filling with a coleurs
                for (int i = 0; i < dataGridView2.RowCount; i++)
                    for (int j = 0; j < dataGridView2.ColumnCount; j++) {
                        if (!ifacentrepoint(i, j))
                            dataGridView2.Rows[i].Cells[j].Style.BackColor = ColeurList[randcol[mindest(i, j)]];
                    }

                Cursor.Current = Cursors.Default;
            }
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
        private bool chckrespoints()
        {
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
        private bool checkallvis() {
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
            for (int i = 0; i < dataGridView1.RowCount; i++){
                if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == xl && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == yl){
                    t = false;
                    break;
                }
            }
            if (t)
                dataGridView1.Rows.Add(new Object[] { xl, yl });
            refr();
        }
        private void button5_Click(object sender, EventArgs e){
            int xl = Convert.ToInt32(numericUpDown3.Value);
            int yl = Convert.ToInt32(numericUpDown4.Value);
            for (int i = 0; i < dataGridView1.RowCount; i++) 
                if (Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value) == xl && Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value) == yl){
                    dataGridView1.Rows.RemoveAt(i);
                    break;
                }
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e) { refr(); }
        private void radioButton2_CheckedChanged(object sender, EventArgs e) { refr(); }
        private void radioButton3_CheckedChanged(object sender, EventArgs e) { refr(); }
        private void numericUpDown6_ValueChanged(object sender, EventArgs e) {
            dataGridView2.DefaultCellStyle.Format = 'N' + numericUpDown6.Value.ToString();
            dataGridView2.AutoResizeColumns();
            dataGridView1.DefaultCellStyle.Format = 'N' + numericUpDown6.Value.ToString();
            dataGridView1.AutoResizeColumns();
        }
        private void numericUpDown5_ValueChanged(object sender, EventArgs e){
            dataGridView2.DefaultCellStyle.Font = new Font("Palatino Linotype", (float)numericUpDown5.Value);
            dataGridView2.AutoResizeColumns();
            dataGridView1.DefaultCellStyle.Font = new Font("Palatino Linotype", (float)numericUpDown5.Value);
            dataGridView1.AutoResizeColumns();
        }
        internal void toFrancais() {
            MessageBox.Show("Pas fait");
        }
        internal void toRusse(){
            MessageBox.Show("Pas fait");
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
