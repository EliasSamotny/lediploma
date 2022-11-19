using static l_application_pour_diploma.Classes;
namespace l_application_pour_diploma{
    
    public partial class Trouvation : Form{
        internal Commencement own;
        internal Routes? r;
        internal int x = 1, y = 1, x1 = 1, y1 = 1;
        internal bool[,] vis, vis1, accessible;
        internal Point[,] previos;
        public Trouvation(Commencement o){
            InitializeComponent();
            own = o;
        }
        private bool checkallvis(){
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    if (!vis[i, j]) return false;
            return true;
        }
        private void calculcell(int xc, int yc){
            if (xc + 1 > 0 && yc + 1 > 0 && xc < dataGridView1.RowCount && yc < dataGridView1.ColumnCount && own.dataGridView1.Rows[xc].Cells[yc].Value != null && pointavailiter(xc, yc))
            {//if exists
                decimal cur = (decimal)((Convert.ToDouble(own.dataGridView1.Rows[xc].Cells[yc].Value.ToString()) + Convert.ToDouble(own.dataGridView1.Rows[x1].Cells[y1].Value.ToString())) / 2 * Math.Sqrt(Math.Pow(xc - x1, 2) + Math.Pow(yc - y1, 2)) + Convert.ToDouble(dataGridView1.Rows[x1].Cells[y1].Value.ToString()));
                if (dataGridView1.Rows[xc].Cells[yc].Value == null)
                {//if not visited at all
                    dataGridView1.Rows[xc].Cells[yc].Value = cur;
                    previos[xc, yc] = new Point(x1, y1);
                }
                else if (cur < Convert.ToDecimal(dataGridView1.Rows[xc].Cells[yc].Value.ToString()))
                {//if from this point is shorter than known path
                    dataGridView1.Rows[xc].Cells[yc].Value = cur;
                    previos[xc, yc] = new Point(x1, y1);
                }
                else if (Convert.ToDecimal(own.dataGridView1.Rows[xc].Cells[yc].Value.ToString()) == -1){  //if point unpassable
                    dataGridView1.Rows[xc].Cells[yc].Value = -1;
                    //previos[xc, yc] = new Point(-1, -1);
                }
            }
        }
        internal void refresh(){
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
            accessible = new bool[dataGridView1.RowCount, dataGridView1.ColumnCount];
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++) {
                    accessible[i, j] = false;
                    vis1[i, j] = false;
                }
            accessible[x, y] = true;
            vis1[x, y] = true;
            reseachpoints(x, y);
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++) {
                    if (availpoint(i, j) && availpoint(x1, y1) && accessible[i, j]) {
                        dataGridView1.Rows[i].Cells[j].Value = null;
                        vis[i, j] = false;
                    }
                    else{
                        vis[i, j] = true;
                        dataGridView1.Rows[i].Cells[j].Value = -1;
                        previos[i, j] = new Point(-1,-1);
                    }
                }
            dataGridView1.Rows[x].Cells[y].Value = 0;
            while (!checkallvis()){
                vis[x1, y1] = true;
                calculcell(x1 - 1, y1 - 1);
                calculcell(x1, y1 - 1);
                calculcell(x1 + 1, y1 - 1);
                calculcell(x1 + 1, y1);
                calculcell(x1 + 1, y1 + 1);
                calculcell(x1, y1 + 1);
                calculcell(x1 - 1, y1 + 1);
                calculcell(x1 - 1, y1);
                if (!radioButton1.Checked){
                    calculcell(x1 - 1, y1 - 2);
                    calculcell(x1 + 1, y1 - 2);
                    calculcell(x1 + 2, y1 - 1);
                    calculcell(x1 + 2, y1 + 1);
                    calculcell(x1 + 1, y1 + 2);
                    calculcell(x1 - 1, y1 + 2);
                    calculcell(x1 - 2, y1 + 1);
                    calculcell(x1 - 2, y1 - 1);
                    if (!radioButton2.Checked){
                        calculcell(x1 - 2, y1 - 3);
                        calculcell(x1 - 1, y1 - 3);
                        calculcell(x1 + 1, y1 - 3);
                        calculcell(x1 + 2, y1 - 3);
                        calculcell(x1 + 3, y1 - 2);
                        calculcell(x1 + 3, y1 - 1);
                        calculcell(x1 + 3, y1 + 1);
                        calculcell(x1 + 3, y1 + 2);
                        calculcell(x1 + 2, y1 + 3);
                        calculcell(x1 + 1, y1 + 3);
                        calculcell(x1 - 1, y1 + 3);
                        calculcell(x1 - 2, y1 + 3);
                        calculcell(x1 - 3, y1 + 2);
                        calculcell(x1 - 3, y1 + 1);
                        calculcell(x1 - 3, y1 - 1);
                        calculcell(x1 - 3, y1 - 2);
                    }
                }
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
                else if (points.Count > 1){ //if not, search nearest to (0,0) 
                    mindest = Decimal.MaxValue;
                    foreach (var p in points)
                        if (p.dest < mindest)
                            mindest = p.dest;
                    var points1 = new List<Points>();
                    foreach (var p in points)
                        if (p.dest == mindest)
                            points1.Add(new Points(p.xl, p.yl, (decimal)Math.Sqrt(Math.Pow(p.xl, 2) + Math.Pow(p.yl, 2))));
                    if (points1.Count == 1) { x1 = points1[0].xl; y1 = points1[0].yl; }//if it is alone to choose, equal et continue
                    else if (points1.Count > 1) {//if not, choosing with least x
                        int minx = dataGridView1.RowCount;
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
            Cursor.Current = Cursors.Default;
            own.dataGridView1.ClearSelection();
            own.dataGridView1.CurrentCell = own.dataGridView1.Rows[x].Cells[y];
            own.textBox1.Text = Convert.ToString(x + 1);
            own.textBox2.Text = Convert.ToString(y + 1);
            if (checkBox1.Checked) fillcolors();
            else clearcolors();
            dataGridView1.AutoResizeColumns();
            if (r != null) r.refr();
        }
        private void reseachpoints(int u,int v) { 
            if (!chckrespoints() && Convert.ToDecimal(own.dataGridView1.Rows[u].Cells[v].Value) >= 0){
                for (byte i = 0; i < 8; i++){
                    if (!visiteda(u, v, i)){
                        switch (i){
                            case 0: { 
                                    vis1[u - 1, v - 1] = true; 
                                    accessible[u - 1, v - 1] = true; 
                                    reseachpoints(u - 1, v - 1); break; }
                            case 1: { vis1[u    , v - 1] = true; accessible[u, v -1] = true; reseachpoints(u    , v - 1); break; }
                            case 2: { vis1[u + 1, v - 1] = true; accessible[u +1, v - 1] = true; reseachpoints(u + 1, v - 1); break; }
                            case 3: { vis1[u + 1, v    ] = true; accessible[u +1, v    ] = true; reseachpoints(u + 1, v    ); break; }
                            case 4: { vis1[u + 1, v + 1] = true; accessible[u +1, v + 1] = true; reseachpoints(u + 1, v + 1); break; }
                            case 5: { vis1[u    , v + 1] = true; accessible[u, v +1] = true; reseachpoints(u    , v + 1); break; }
                            case 6: { vis1[u - 1, v + 1] = true; accessible[u -1, v + 1] = true; reseachpoints(u - 1, v + 1); break; }
                            case 7: { vis1[u - 1, v    ] = true; accessible[u -1, v    ] = true; reseachpoints(u - 1, v    ); break; }
                        }
                    }
                }
            }
        }
        private bool chckrespoints(){
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    if (!vis1[i, j]) return false;
            return true;
        }
        private bool pointavailiter(int i, int j) {
            try {
                if (pointdest(i, j) == Math.Sqrt(2) || pointdest(i, j) == 1)
                    return availpoint(i, j);
                else if (pointdest(i, j) == Math.Sqrt(5)){
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
                        return availpoint(i - 1, j) && availpoint(i - 2, j) && availpoint(i - 1, j - 1) && availpoint(i - 1, j - 2) ;
                    if (x1 - i == -1 && y1 - j == -3)//6
                        return  availpoint(i - 1, j - 1) && availpoint(i - 1, j - 2) && availpoint(i, j - 1) && availpoint(i, j - 2);
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
        private bool availpoint(int u, int v){ return Convert.ToDecimal(own.dataGridView1.Rows[u].Cells[v].Value) >= 0;}
        public void clearcells(){
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    dataGridView1.Rows[i].Cells[j].Value = null;
                }
        private double pointdest(int xc,int yc){return Math.Sqrt( Math.Pow(x1 - xc,2) + Math.Pow(y1 - yc, 2)); }
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
        private void Trouvation_Load(object sender, EventArgs e){refresh();}
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e){
            if (x!= dataGridView1.SelectedCells[0].RowIndex || y!= dataGridView1.SelectedCells[0].ColumnIndex)
                refresh();
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e){
            if (x != dataGridView1.SelectedCells[0].RowIndex || y != dataGridView1.SelectedCells[0].ColumnIndex)
                refresh();
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e) { refresh(); }
        private void radioButton2_CheckedChanged(object sender, EventArgs e) { refresh(); }
        private void radioButton3_CheckedChanged(object sender, EventArgs e) { refresh(); }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e){ }
        private void Trouvation_FormClosing(object sender, FormClosingEventArgs e){own.trouv = null;if (r != null) r.Close(); }
        private void checkBox1_CheckedChanged(object sender, EventArgs e){
            if (checkBox1.Checked) fillcolors();
            else clearcolors();
        }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e){
            dataGridView1.DefaultCellStyle.Format = 'N' + numericUpDown3.Value.ToString();
            dataGridView1.AutoResizeColumns();
        }
        private void numericUpDown4_ValueChanged(object sender, EventArgs e){
            dataGridView1.DefaultCellStyle.Font = new Font("Palatino Linotype", (float)numericUpDown4.Value);
            dataGridView1.AutoResizeColumns();
        }
        private void desCalculationsDeCheminsToolStripMenuItem_Click(object sender, EventArgs e){
            if (r == null && checkBox1.Checked) { 
                r = new Routes(this);
                if (own.lang == 1) r.toRusse();
                r.Show(); }
            else if (checkBox1.Checked) r.Focus();
            if (!checkBox1.Checked) MessageBox.Show("Il n'y a pas de carte de chaleur!");
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e){ }
        internal void toRusse() {
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
            Text = "Поиск короткого пути";
            groupBox4.Text = "Тепловая карта расстояний";
            checkBox1.Text = "Показать карту";
            desCalculationsDeCheminsToolStripMenuItem.Text = "Вычисление наикратчайших путей";
            desFenêtresToolStripMenuItem.Text = "Окна";
            sauvegarderToolStripMenuItem.Text = "Сохранить";
            chargerToolStripMenuItem.Text = "Загрузить";
            toolStripMenuItem1.Text = "Файл";
            if (r != null) r.toRusse();
        }
        internal void toFrancais(){
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
            if (r != null) r.toFrancais();
        }
        internal void clearcolors(){
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
        }
        private void fillcolors(){
            int r = own.dataGridView1.RowCount;
            int c = own.dataGridView1.ColumnCount;
            dataGridView1.RowCount = r;
            dataGridView1.ColumnCount = c;
            decimal maxval = 0, minval = Decimal.MaxValue;
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++){
                    var d = Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value);
                    if (maxval < d)
                        maxval = d;
                    if (minval > d && d > -1)
                        minval = d;
                }
            if (minval != maxval)
                for (int i = 0; i < r; i++)
                    for (int j = 0; j < c; j++){
                        if (Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) != -1){
                            var d = Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) / maxval;
                            if (d  < (decimal)0.2)
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
                            else if (d  >= (decimal)0.8 && d! < (decimal)0.9)
                                dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Orange;
                            else if (d  >= (decimal)0.9)
                                dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Red;
                        }
                        else dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;
                    }
            else for (int i = 0; i < r; i++)
                    for (int j = 0; j < c; j++){
                        if (Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) != -1){
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Blue;
                        } else dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;
                    }
            dataGridView1.Rows[x].Cells[y].Style.BackColor = Color.Violet;
            dataGridView1.AutoResizeColumns();
        }
    }
}
