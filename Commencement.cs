using ex = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace l_application_pour_diploma{
    public partial class Commencement : Form{
        private ex.Application? excelapp;
        private ex.Workbooks? excelappworkbooks;
        private ex.Workbook? excelappworkbook;
        private ex.Sheets? excelsheets;
        private ex.Worksheet? excelworksheet;
        private ex.Range? excelcells;
        public string? Filename;
        internal byte lang; // 0 - francais, 1 - russe
        public Commencement() { 
            InitializeComponent();
            trouv = null;
            dataGridView1.RowCount = (int)numericUpDown1.Value;
            dataGridView1.ColumnCount = (int)numericUpDown2.Value;
            lang = 0;
            refreshdata(true);
            dataGridView1.AutoResizeColumns();
        }
        internal Trouvation? trouv;
        internal Beaucoup? beaucoup;
        internal Vran? vran;
        private void Form1_Load(object sender, EventArgs e) {
            russeToolStripMenuItem_Click(sender,e);
            dataGridView1.AutoResizeColumns();
            set_mount();
        }
        private void set_mount(){
            int r = dataGridView1.RowCount, c = dataGridView1.ColumnCount;
            
            for (int i = 0; i < r; i++){
                for (int j = 0; j < c; j++){
                    try{
                        dataGridView1.Rows[i].Cells[j].Value = mountagne(i, j, r, c);
                    }
                    catch (Exception){
                        dataGridView1.Rows[i].Cells[j].Value = -1;
                    }
                }
            }
            /*dataGridView1.Rows[r / 2].Cells[c / 2].Value = -1;
            if (r % 2 == 0) {
                dataGridView1.Rows[r / 2 + 1].Cells[c / 2].Value = -1;
                if (c % 2 == 0){
                    dataGridView1.Rows[r / 2].Cells[c / 2 + 1].Value = -1;
                    dataGridView1.Rows[r / 2 + 1].Cells[c / 2].Value = -1;
                }
            }
            else {
                if (c % 2 == 0)
                    dataGridView1.Rows[r / 2].Cells[c / 2 + 1].Value = -1;
            }*/
            if (checkBox3.Checked) fillcolors();
            else clearcolors();
        }
        private decimal mountagne(int i, int j,int r, int c){
            double icoef = 0.5*2;
            double jcoef = 0.5*2;
            return 40 / (decimal) (Math.Pow((i - r / 2) * icoef, 2) + Math.Pow((j - c / 2) * jcoef, 2));
        }
        private void button1_Click(object sender, EventArgs e){ refreshdata(true); }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e){ refreshdata(checkBox2.Checked); }
        private void refreshdata(bool t){            
            if (t) {
                dataGridView1.RowCount = (int) numericUpDown1.Value;
                dataGridView1.ColumnCount = (int)numericUpDown2.Value;
                Random r = new ();
                for (int i = 0; i < dataGridView1.RowCount; i++) 
                    for (int j = 0; j < dataGridView1.ColumnCount; j++){
                        if (checkBox1.Checked && Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) < 0)
                            dataGridView1.Rows[i].Cells[j].Value = -1;
                        else
                            dataGridView1.Rows[i].Cells[j].Value = r.Next((int)numericUpDown3.Value, (int)numericUpDown4.Value + 1);
                    }            
            }
            else {                 
                decimal[,] data1 = new decimal[dataGridView1.RowCount, dataGridView1.ColumnCount];
                for (int i = 0; i < dataGridView1.RowCount; i++)
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                        try {
                            data1[i, j] = Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value); }
                        catch (Exception) { data1[i, j] = 0; }

                int i1 = Math.Min((int)numericUpDown1.Value, dataGridView1.RowCount);
                int j1 = Math.Min((int)numericUpDown2.Value, dataGridView1.ColumnCount); 
                dataGridView1.RowCount = (int)numericUpDown1.Value;
                dataGridView1.ColumnCount = (int)numericUpDown2.Value;
                for (int i = 0; i < i1; i++) 
                    for (int j = 0; j < j1; j++)
                        dataGridView1.Rows[i].Cells[j].Value = data1[i, j];                   
            }
            if (trouv != null) {
                trouv.refresh();
            }
            if (checkBox3.Checked) fillcolors();
            else clearcolors();
            if (vran != null) vran.refr();
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoResizeRows();
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e) { refreshdata(checkBox2.Checked); }
        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e) {  }
        private void numericUpDown1_KeyUp(object sender, KeyEventArgs e) {  }
        private void numericUpDown2_KeyPress(object sender, KeyPressEventArgs e) {  }
        private void numericUpDown2_KeyUp(object sender, KeyEventArgs e) { }
        private void desCalculationsDeCheminsToolStripMenuItem_Click(object sender, EventArgs e){
            if (trouv == null){
                trouv = new Trouvation(this);
                trouv.Show();
                if (lang == 1)
                    trouv.toRusse();
            }
            else {
                if (lang == 0)
                    MessageBox.Show("La fenêtre avec les calculations est déjà ouvertée.");
                else if (lang == 1)
                    MessageBox.Show("Окно с вычислениями минимальных расстояний уже открыто.");
                 }
            trouv.Focus();
        }
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            bool curr = false;
            if (dataGridView1.SelectedCells[0].Value != null) {
                string str = dataGridView1.SelectedCells[0].Value.ToString();
                if ((str[0] >= '0' && str[0] <= '9' || str[0] =='-') && str.Length > 0) {
                    curr = true;
                    for (int i = 1;i < str.Length;i++)
                        if ((str[i] < '0' || str[i] > '9') && str[i] != ','){
                            curr = false;
                            break;
                        }
                }
                if (curr) { refreshdata(false); }
                else
                {
                    MessageBox.Show("Invalid number");
                    if (trouv != null) trouv.clearcells();
                    dataGridView1.SelectedCells[0].Value = -1;
                }
            }
            else{
                MessageBox.Show("Invalid number");
                if (trouv != null) trouv.clearcells();
                dataGridView1.SelectedCells[0].Value = -1;
            }
        }
        private void sauvegarderToolStripMenuItem_Click(object sender, EventArgs e){
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Tableur | *.xlsx| Tableur | *.xls| All files | *.*";
            saveFileDialog1.Title = "Sauvegarder la table";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != ""){
                Filename = saveFileDialog1.FileName;
                decimal[,] data = new decimal[dataGridView1.RowCount, dataGridView1.ColumnCount];
                for (int i = 0; i < dataGridView1.RowCount; i++)
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                        if (dataGridView1.Rows[i].Cells[j].Value != null)
                            data[i, j] = Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value.ToString());
                        else data[i, j] = 0;
                excelapp = new ex.Application();
                excelapp.SheetsInNewWorkbook = 2;
                excelapp.Workbooks.Add(Type.Missing);
                excelappworkbooks = excelapp.Workbooks;
                excelappworkbook = excelappworkbooks[1];
                excelsheets = excelappworkbook.Sheets;
                excelworksheet = excelsheets[1];
                excelworksheet.Name = "Les donnes";
                for (int i = 0; i < dataGridView1.RowCount; i++){
                    for (int j = 0; j < dataGridView1.ColumnCount; j++){
                        excelcells = excelworksheet.get_Range(numColu(j + 2) + Convert.ToString(i + 2), Type.Missing);
                        excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString(data[i, j])));
                    }
                }
                excelapp.DisplayAlerts = true;
                excelappworkbook.SaveAs(Filename, Type.Missing, Type.Missing,
               Type.Missing, Type.Missing, Type.Missing, ex.XlSaveAsAccessMode.xlNoChange,
               Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                excelappworkbook.Close();
                MessageBox.Show("Le tableur est sauvegardé");
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
        private void chargerToolStripMenuItem_Click(object sender, EventArgs e){
            OpenFileDialog file = new OpenFileDialog(); //open dialog to choose file
            file.Filter = "Tableur | *.xlsx| Tableur | *.xls| All files | *.*";
            file.Title = "Charger la table";
            string filePath = string.Empty;
            string fileExt = string.Empty;
            if (file.ShowDialog() == DialogResult.OK){
                filePath = file.FileName; //get the path of the file
                fileExt = Path.GetExtension(filePath); //get the file extension
                if (fileExt.CompareTo(".xls") == 0 || fileExt.CompareTo(".xlsx") == 0){
                    try{
                        Cursor.Current = Cursors.WaitCursor;
                        ex.Application xlApp = new ex.Application();
                        ex.Workbook xlWorkbook = xlApp.Workbooks.Open(filePath);
                        ex._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
                        ex.Range xlRange = xlWorksheet.UsedRange;
                        int a = 1, b = 1;
                        while (true){
                            if (xlWorksheet.Cells[a + 1, b + 1].Value != null) { a++; b++; }
                            else if (xlWorksheet.Cells[a + 1, b].Value != null) a++;
                            else if (xlWorksheet.Cells[a, b + 1].Value != null) b++;
                            else break;
                        }
                        dataGridView1.RowCount = a - 1;
                        dataGridView1.ColumnCount = b - 1;
                        numericUpDown1.Value = a - 1;
                        numericUpDown2.Value = b - 1;
                        for (int i = 0; i < a - 1; i++) 
                            for (int j = 0; j < b - 1; j++){
                                dataGridView1.Rows[i].Cells[j].Value = Convert.ToDecimal(xlWorksheet.Cells[i + 2, j + 2].Value);
                            }
                                                                          
                        //cleanup
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        //release com objects to fully kill excel process from running in the background
                        Marshal.ReleaseComObject(xlRange);
                        Marshal.ReleaseComObject(xlWorksheet);
                        //close and release
                        xlWorkbook.Close();
                        Marshal.ReleaseComObject(xlWorkbook);
                        //quit and release
                        xlApp.Quit();
                        Marshal.ReleaseComObject(xlApp);
                        // Set cursor as default arrow
                        Cursor.Current = Cursors.Default;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }                    
                    dataGridView1.AutoResizeColumns();
                    if (checkBox3.Checked) fillcolors();
                    else clearcolors();
                    if (trouv != null) trouv.refresh();
                    if (beaucoup != null) beaucoup.refr();
                    if (vran != null) vran.refr();
                }
                else{
                    MessageBox.Show("Please choose .xls or .xlsx file only.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error); //custom messageBox to show error
                }
            }
            dataGridView1.AutoResizeColumns();
        }
        private void showCurrCoord(){
            textBox1.Text = (dataGridView1.SelectedCells[0].RowIndex + 1).ToString();
            textBox2.Text = (dataGridView1.SelectedCells[0].ColumnIndex + 1).ToString();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e){ showCurrCoord(); }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) { showCurrCoord(); }
        private void françaisToolStripMenuItem_Click(object sender, EventArgs e){
            if (lang != 0) {
                label1.Text = "Quantitè de lignes";
                label2.Text = "Quantitè de colonnes";
                button1.Text = "Générater";
                toolStripMenuItem1.Text = "Ficher";
                chargerToolStripMenuItem.Text = "Charger";
                sauvegarderToolStripMenuItem.Text = "Sauvegarder";
                checkBox2.Text = "Remplir de nombres aléatoires après avoir changé les dimensions";
                desFenêtresToolStripMenuItem.Text = "Fenêtres";
                desCalculationsDeCheminsToolStripMenuItem.Text = "Calculations de chemins pour deux points";
                langueToolStripMenuItem.Text = "Langue";
                sûrLauteurToolStripMenuItem.Text = "Sûr l\'auteur";
                groupBox1.Text = "Dimensions";
                groupBox2.Text = "Le point actuel";
                label4.Text = "La ligne";
                label3.Text = "La colonne";
                button2.Text = "Remplir toutes les unités";
                groupBox3.Text = "Génération de nombres ";
                checkBox1.Text = "Sauvegarder les unités neg.";
                this.Name = "Commencement";
                this.Text = "L\'application pour le diploma: donnes pour commencement";
                groupBox4.Text = "Reflexion de cellules";
                label7.Text = "La taille de fonte";
                label8.Text = "La quantité de signs";
                groupBox5.Text = "La carte de chaleur de paysage";
                checkBox3.Text = "Afficher la carte";
                calculationsDeChemanPourBeaucoupPointsToolStripMenuItem.Text = "Calculations de cheman pour beaucoup points";
                diagrammeDeVoronoїToolStripMenuItem.Text = "Diagramme de Voronoї";
                if (trouv != null) trouv.toFrancais();
                if (beaucoup != null) beaucoup.toFrancais();
                if (vran != null) vran.toFrancais();
                lang = 0;
            }
        }
        private void russeToolStripMenuItem_Click(object sender, EventArgs e){
            if (lang != 1) {
                label1.Text = "Кол-во строк";
                label2.Text = "Кол-во стоблцов";
                button1.Text = "Сгенерировать";
                toolStripMenuItem1.Text = "Файл";
                chargerToolStripMenuItem.Text = "Загрузить";
                sauvegarderToolStripMenuItem.Text = "Сохранить";
                checkBox2.Text = "Генерировать случайные числа при изменении измерений";
                desFenêtresToolStripMenuItem.Text = "Окна";
                desCalculationsDeCheminsToolStripMenuItem.Text = "Вычисление расстояний для двух точек";
                langueToolStripMenuItem.Text = "Язык";
                sûrLauteurToolStripMenuItem.Text = "Об авторе";
                groupBox1.Text = "Измерения";
                groupBox2.Text = "Исследуемая точка";
                label4.Text = "Строка";
                label3.Text = "Столбец";
                button2.Text = "Сгенерировать единицы";
                groupBox3.Text = "Генерация чисел";
                checkBox1.Text = "Сохранить препятствия";
                this.Text = "L\'application pour le diploma: данные для исследования";
                groupBox4.Text = "Отображение ячеек";
                label7.Text = "Размер шрифта";
                label8.Text = "Количество знаков";
                groupBox5.Text = "Тепловая карта ландшафта";
                checkBox3.Text = "Показать карту";
                calculationsDeChemanPourBeaucoupPointsToolStripMenuItem.Text = "Вычисление оптимальной точки встречи";
                diagrammeDeVoronoїToolStripMenuItem.Text = "Диаграмма Вороного";
                if (trouv != null) trouv.toRusse();
                if (beaucoup != null) beaucoup.toRusse();
                if (vran != null) vran.toRusse();
                lang = 1;
            }
        }
        private void button2_Click(object sender, EventArgs e){
            set_unites();
        }
        private void set_unites() {
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    if (checkBox1.Checked && Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) < 0)
                        dataGridView1.Rows[i].Cells[j].Value = -1;
                    else dataGridView1.Rows[i].Cells[j].Value = 1;
            try { dataGridView1.Rows[0].Cells[0].Value = (decimal)dataGridView1.Rows[0].Cells[0].Value; }
            catch (Exception) { dataGridView1.Rows[0].Cells[0].Value = (int)dataGridView1.Rows[0].Cells[0].Value; }
            if (trouv != null) trouv.refresh();
            if (vran != null) vran.refr();
            if (checkBox3.Checked) fillcolors();
            else clearcolors();
        }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e){
            if (numericUpDown3.Value > numericUpDown4.Value)
                numericUpDown3.Value = numericUpDown4.Value - 1;
        }
        private void numericUpDown4_ValueChanged(object sender, EventArgs e){
            if (numericUpDown4.Value < numericUpDown3.Value)
                numericUpDown4.Value = numericUpDown3.Value + 1;
        }
        private void sûrLauteurToolStripMenuItem_Click(object sender, EventArgs e){ 
            if (lang == 0)
                MessageBox.Show("Cette application est faite par Ilia Madayëv.");
            else if (lang == 1)
                MessageBox.Show("Приложение было разработано Ильёй Мадаевым.");
        }
        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e){ }
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e){ }
        private void dataGridView1_KeyUp(object sender, KeyEventArgs e) { }
        internal void clearcolors(){
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
        }
        internal void fillcolors(){
            decimal maxval = 0, minval = Decimal.MaxValue;
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++){
                    var d = Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value);
                    if (maxval < d) maxval = d;
                    if (minval > d && d > -1) minval = d;
                }
            if (minval != maxval)
                for (int i = 0; i < dataGridView1.RowCount; i++)
                    for (int j = 0; j < dataGridView1.ColumnCount; j++) {
                        if (Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) != -1){
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
            else for (int i = 0; i < dataGridView1.RowCount; i++)
                    for (int j = 0; j < dataGridView1.ColumnCount; j++){
                        if (Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) != -1){
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Blue;
                        }
                        else dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;
                    }
        }
        private void numericUpDown6_ValueChanged(object sender, EventArgs e){
            dataGridView1.DefaultCellStyle.Format = 'N' + numericUpDown6.Value.ToString();
            dataGridView1.AutoResizeColumns();
        }
        private void numericUpDown5_ValueChanged(object sender, EventArgs e){
            dataGridView1.DefaultCellStyle.Font = new Font("Palatino Linotype", (float)numericUpDown5.Value);
            dataGridView1.AutoResizeColumns();
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e){
            if (checkBox3.Checked) fillcolors();
            else clearcolors();
        }
        private void calculationsDeChemanPourBeaucoupPointsToolStripMenuItem_Click(object sender, EventArgs e){
            if (beaucoup == null){
                beaucoup = new Beaucoup(this);
                if (lang == 1) beaucoup.toRusse();
                beaucoup.Show();
            }
            beaucoup.Focus();
        }
        private void diagrammeDeVoronoїToolStripMenuItem_Click(object sender, EventArgs e) {
            if (vran == null) {
                vran = new Vran(this);
                if (lang == 1) vran.toRusse();
                vran.Show();
            }
            vran.Focus();
        }

        private void dataGridView1_ColumnAdded(object sender, DataGridViewColumnEventArgs e){
            int col = dataGridView1.ColumnCount - 1;
            for (int i = 0; i < dataGridView1.RowCount; i++){
                dataGridView1.Rows[i].Cells[col].Value = dataGridView1.Rows[i].Cells[col - 1].Value;
            }
        }
    }
}