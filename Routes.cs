namespace l_application_pour_diploma{
    public partial class Routes : Form{
        internal Trouvation own;
        int d;
        int x, y,x2,y2,u,v;
        public Routes(Trouvation t){
            own = t;
            InitializeComponent();
        }
        private void Routes_Load(object sender, EventArgs e){refr();}
        private void findroute(){
            textBox1.Text = "";
            x = own.dataGridView1.RowCount - 1;
            y = own.dataGridView1.ColumnCount - 1;
            u = own.dataGridView1.SelectedCells[0].RowIndex;
            v = own.dataGridView1.SelectedCells[0].ColumnIndex;
            
            x2 = (int)numericUpDown1.Value - 1;
            y2 = (int)numericUpDown2.Value - 1;
            if (own.previos[x2,y2].X!=-1) {
                List<Point> route = new();
                route.Add(new Point(x2, y2));
                while (x2 != u || y2 != v){
                    int x1 = x2, y1 = y2;
                    route.Add(new Point(own.previos[x1,y1].X, own.previos[x1, y1].Y));
                    x2 = own.previos[x1, y1].X; 
                    y2 = own.previos[x1, y1].Y;
                }
                refr();
                Bitmap bmp = new(pictureBox1.Image);
                Graphics carte = Graphics.FromImage(bmp);

                for (int i = route.Count - 1; i > 0 ; i--) {
                    textBox1.AppendText("(" + (route[i].X+1)+" , " + (route[i].Y+1) + ") --> (" + (route[i-1].X+1) + " , " + (route[i-1].Y+1) + ")\r\n") ;
                    Point[] p1i = new Point[] { 
                        new (route[i].Y * d + d / 2, route[i].X * d + d / 2), 
                        new (route[i].Y * d + d / 2, route[i].X * d + 1 + d / 2),
                        new (route[i].Y * d + d / 2, route[i].X * d + 2 + d / 2)
                    };
                    Point[] p2i = new Point[] { 
                        new(route[i - 1].Y * d + d/2, route[i - 1].X * d + d / 2), 
                        new(route[i - 1].Y * d + d/2, route[i - 1].X * d + 1 + d / 2),
                        new(route[i - 1].Y * d + d/2, route[i - 1].X * d + 2 + d / 2)
                    };
                    for (int j = 0; j < p1i.Length; j++){
                        carte.DrawLine(new Pen(Color.Black), p1i[j], p2i[j]);
                    }
                }
                pictureBox1.Image = bmp;
            }
            else {
                textBox1.Text = "Il n'y a une route du point ("+ textBox2.Text+" , "+ textBox3.Text+") au point (" + (x+1)+" , " + (y+1)+")!";
                MessageBox.Show("Le point final est inaccessible!"); }
        }
        internal void refr(){
            Bitmap bmp = new (pictureBox1.Width, pictureBox1.Height);
            Graphics carte = Graphics.FromImage(bmp);
            d = Math.Min(pictureBox1.Width, pictureBox1.Height)/Math.Max(own.dataGridView1.RowCount, own.dataGridView1.ColumnCount);
            for (int i = 0; i < own.dataGridView1.RowCount; i++) {
                for (int j = 0; j < own.dataGridView1.Columns.Count; j++) {
                    carte.DrawRectangle(new Pen(Color.White), j * d, i * d, d, d);
                    carte.FillRectangle(new SolidBrush(own.dataGridView1.Rows[i].Cells[j].Style.BackColor), j * d + 1, i * d + 1, d - 1, d - 1);
                }
            
            }
            pictureBox1.Image = bmp;
            textBox2.Text = own.dataGridView1.SelectedCells[0].RowIndex + 1+"";
            textBox3.Text = own.dataGridView1.SelectedCells[0].ColumnIndex + 1 + "";
        }
        internal void toRusse() {
            groupBox1.Text = "Построение наикратчайших маршрутов";
            label1.Text = "Маршрут";
            groupBox2.Text = "Начальная точка";
            label4.Text = "Строка";
            label3.Text = "Стоблец";
            groupBox3.Text = "Коненчая точка";
            label2.Text = "Строка";
            label5.Text = "Стоблец";
            Text = "Построение маршрутов";
        }
        internal void toFrancais() {
            groupBox1.Text = "Les routes plus courts";
            label1.Text = "L\'itinéraire";
            groupBox2.Text = "Le point de départ";
            label4.Text = "La ligne";
            label3.Text = "La colonne";
            groupBox3.Text = "Le point final";
            label2.Text = "La ligne";
            label5.Text = "La colonne";
            Text = "Routes";
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e){
            if (Convert.ToUInt32(numericUpDown1.Value) > own.dataGridView1.RowCount){
                numericUpDown1.Value = own.dataGridView1.RowCount;
            }
            findroute();
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e){
            if (Convert.ToUInt32(numericUpDown2.Value) > own.dataGridView1.ColumnCount){
                numericUpDown2.Value = own.dataGridView1.ColumnCount;
            }
            findroute();
        }
        private void Routes_FormClosing(object sender, FormClosingEventArgs e){own.r = null;}
        private void Routes_ResizeBegin(object sender, EventArgs e) {  refr(); }
        private void Routes_ResizeEnd(object sender, EventArgs e) { refr(); }
        private void pictureBox1_Resize(object sender, EventArgs e) { refr(); }
        private void Routes_Resize(object sender, EventArgs e) {  refr(); }
    }
}
