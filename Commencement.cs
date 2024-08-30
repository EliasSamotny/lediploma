using ex = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Diagnostics;
using System;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Bibliography;

namespace l_application_pour_diploma {
    public partial class Commencement : Form
    {
        private ex.Application? excelapp;
        private ex.Workbooks? excelappworkbooks;
        private ex.Workbook? excelappworkbook;
        private ex.Sheets? excelsheets;
        private ex.Worksheet? excelworksheet;
        private ex.Range? excelcells;
        public string? Filename;
        internal byte lang; // 0 - francais, 1 - russe
        internal List<decimal[,]> source;
        private bool global_log_allowed = true;
        private static readonly object log_locker = new object();
        string logFilePath = "C:\\Users\\Elias\\Desktop\\lediploma\\log.log";
        internal int stateShift;
        private bool loaded;
        public void insert_log(string log_mess, Form caller)
        {
            if (global_log_allowed)
            {
                lock (log_locker)
                {
                    using (StreamWriter sw = new StreamWriter(logFilePath, true))
                    {
                        // Get the current timestamp
                        string timestamp = DateTime.Now.ToString("HH:mm:ss:ff");

                        // Add the log string with the timestamp to the file
                        string logString = $"{timestamp}: {caller.Text}: {log_mess}";
                        sw.WriteLine(logString);
                    }
                }
            }
        }
        public Commencement()
        {
            InitializeComponent();
            trouv = null;
            lang = 0;

            //refreshdata();
            //domains = new();
        }
        internal Trouvation? trouv;
        internal Beaucoup? beaucoup;
        internal Vran? vran;
        internal Spann? span;
        internal List<decimal> transitions;
        private void Form1_Load(object sender, EventArgs e)
        {
            insert_log("Loading form...", this);
            russeToolStripMenuItem_Click(sender, e);
            dataGridView1.RowCount = (int)numericUpDown1.Value;
            dataGridView1.ColumnCount = (int)numericUpDown2.Value;
            int r = dataGridView1.RowCount, c = dataGridView1.ColumnCount;
            source = new() { new decimal[r, c] };
            set_unites();
            //set_mount();

            transitions = new List<decimal> { 10 };
            dataGridView2.Rows.Add(new object[] { transitions[0], 1 });
            insert_log("Form loaded.", this);
            dataGridView1.AutoResizeColumns();
        }
        private void set_unites(){
            int r = dataGridView1.RowCount, c = dataGridView1.ColumnCount;
            source[(int)numericUpDown8.Value - 1] = new decimal[r, c];
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    if (Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) < 0 && checkBox1.Checked)
                        dataGridView1.Rows[i].Cells[j].Value = -1;
                    else dataGridView1.Rows[i].Cells[j].Value = 1;
                    source[(int)numericUpDown8.Value - 1][i, j] = Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value);
                }
            //try { dataGridView1.Rows[0].Cells[0].Value = (decimal)dataGridView1.Rows[0].Cells[0].Value; }
            //catch (Exception) { dataGridView1.Rows[0].Cells[0].Value = (int)dataGridView1.Rows[0].Cells[0].Value; }
            refreshdata();

            if (checkBox3.Checked) fillcolors();
            else clearcolors();
        }
        private void set_mount(){
            int r = (int)numericUpDown1.Value, c = (int)numericUpDown2.Value;
            source = new() { new decimal[r, c] };
            for (int i = 0; i < r; i++){
                for (int j = 0; j < c; j++) {
                    try{
                        source[0][i, j] = mountagne(i, j, r, c);
                    }
                    catch (Exception){
                        source[0][i, j] = -1;
                    }
                }
            }
        }
        private decimal mountagne(int i, int j, int r, int c){
            double icoef = 0.5;
            double jcoef = 0.5;
            return 1 / (decimal)(Math.Pow((i - r / 2) * icoef, 2) / 2 + Math.Pow((j - c / 2) * jcoef, 2) / 2);
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            if (source[0].GetLength(0) < (int)numericUpDown1.Value && !loaded)
                for (int k = 0; k < source.Count; k++){
                    decimal[,] newArray = new decimal[(int)numericUpDown1.Value, source[0].GetLength(1)];

                    // Copy the values from the original array to the new array
                    //Array.Copy(source[k], 0, newArray, 0, source[k].Length);

                    for (int i = 0; i < source[k].GetLength(0); i++){
                        for (int j = 0; j < source[k].GetLength(1); j++){
                            newArray[i, j] = source[k][i, j];
                        }
                    }
                    for (int i = source[k].GetLength(0); i < (int)numericUpDown1.Value; i++){
                        for (int j = 0; j < source[k].GetLength(1); j++){
                            newArray[i, j] = source[k][source[k].GetLength(0) - 1, j];
                        }
                    }
                    //Array.Copy(newArray, 0, source[k], 0, newArray.Length);
                    source[k] = newArray;
                    refreshdata();
                }
            else if (!loaded){
                for (int k = 0; k < source.Count; k++){
                    decimal[,] newArray = new decimal[(int)numericUpDown1.Value, source[0].GetLength(1)];

                    // Copy the values from the original array to the new array
                    //Array.Copy(source[k], 0, newArray, 0, source[k].Length);
                    for (int i = 0; i < newArray.GetLength(0); i++){
                        for (int j = 0; j < source[k].GetLength(1); j++){
                            newArray[i, j] = source[k][i, j];
                        }
                    }
                    /*for (int i = source[k].GetLength(0); i < (int)numericUpDown1.Value; i++){
                        for (int j = 0; j < source[k].GetLength(1); j++)
                        {
                            newArray[i, j] = source[k][i, j];
                        }
                    }*/

                    //Array.Copy(newArray, 0, source[k], 0, newArray.Length);
                    source[k] = newArray;
                }
                refreshdata();
            }
            
        }
        internal void refreshdata(){
            dataGridView1.RowCount = (int)numericUpDown1.Value;
            dataGridView1.ColumnCount = (int)numericUpDown2.Value;
            int r = dataGridView1.RowCount, c = dataGridView1.ColumnCount;

            insert_log("Refreshing forms...", this);
            for (int i = 0; i < r; i++){
                for (int j = 0; j < c; j++){
                    dataGridView1.Rows[i].Cells[j].Value = source[Convert.ToInt32(numericUpDown8.Value) - 1][i, j];
                }
            }
            trouv?.refresh(true);
            vran?.refr(true);
            span?.refr(true);
            insert_log("Forms refreshed.", this);
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e) {
            if (source[0].GetLength(1) < (int)numericUpDown2.Value && !loaded){
                for (int k = 0; k < source.Count; k++){
                    decimal[,] newArray = new decimal[source[0].GetLength(0), (int)numericUpDown2.Value];

                    // Copy the values from the original array to the new array
                    //Array.Copy(source[k], 0, newArray, 0, source[k].Length);
                    for (int i = 0; i < source[k].GetLength(0); i++){
                        for (int j = 0; j < source[k].GetLength(1); j++)
                        {
                            newArray[i, j] = source[k][i, j];
                        }
                    }
                    for (int j = source[k].GetLength(1); j < (int)numericUpDown2.Value; j++){
                        for (int i = 0; i < source[k].GetLength(0); i++){
                            newArray[i, j] = source[k][i, source[k].GetLength(1) - 1];
                        }
                    }
                    //Array.Copy(newArray, 0, source[k], 0, newArray.Length);
                    source[k] = newArray;
                }
                refreshdata();
            }
            else if (!loaded){
                for (int k = 0; k < source.Count; k++){
                    decimal[,] newArray = new decimal[source[0].GetLength(0), (int)numericUpDown2.Value];

                    // Copy the values from the original array to the new array
                    //Array.Copy(source[k], 0, newArray, 0, source[k].Length);
                    /*for (int i = source[k].GetLength(0); i < (int)numericUpDown1.Value; i++){
                        for (int j = 0; j < source[k].GetLength(1); j++){
                            newArray[i, j] = source[k][i, j];
                        }
                    }*/
                    for (int i = 0; i < source[k].GetLength(0); i++)
                    {
                        for (int j = 0; j < newArray.GetLength(1); j++)
                        {
                            newArray[i, j] = source[k][i, j];
                        }
                    }
                    //Array.Copy(newArray, 0, source[k], 0, newArray.Length);
                    source[k] = newArray;
                }
                refreshdata();
            }
        }
        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e) { }
        private void numericUpDown1_KeyUp(object sender, KeyEventArgs e){ }
        private void numericUpDown2_KeyPress(object sender, KeyPressEventArgs e) { }
        private void numericUpDown2_KeyUp(object sender, KeyEventArgs e) { }
        private void desCalculationsDeCheminsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trouv == null)
            {
                trouv = new Trouvation(this);
                trouv.Show();
                if (lang == 1)
                    trouv.toRusse();
            }
            else
            {
                if (lang == 0)
                    MessageBox.Show("La fenêtre avec les calculations est déjà ouvertée.");
                else if (lang == 1)
                    MessageBox.Show("Окно с вычислениями минимальных расстояний уже открыто.");
            }
            trouv.Focus();
        }
        private void sauvegarderToolStripMenuItem_Click(object sender, EventArgs e){
            insert_log("Choosing file for saving media...", this);
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Tableur | *.xlsx| Tableur | *.xls| All files | *.*";
            saveFileDialog1.Title = "Sauvegarder la table";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                insert_log("Saving media...", this);
                Filename = saveFileDialog1.FileName;


                excelapp = new ex.Application();
                excelapp.SheetsInNewWorkbook = source.Count + 2;
                excelapp.Workbooks.Add(Type.Missing);
                excelappworkbooks = excelapp.Workbooks;
                excelappworkbook = excelappworkbooks[1];
                excelsheets = excelappworkbook.Sheets;

                for (int k = 0; k < source.Count; k++)
                {
                    excelworksheet = excelsheets[k + 1];
                    excelworksheet.Name = $"Etat {k + 1}";
                    for (int i = 0; i < (int)numericUpDown1.Value; i++)
                    {
                        for (int j = 0; j < (int)numericUpDown2.Value; j++)
                        {
                            excelcells = excelworksheet.get_Range(numColu(j + 2) + Convert.ToString(i + 2), Type.Missing);
                            excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString(source[k][i, j])));
                        }
                    }
                }
                excelworksheet = excelsheets[excelsheets.Count - 1];
                excelworksheet.Name = "Transitions";
                for (int j = 0; j < transitions.Count; j++)
                {
                    excelcells = excelworksheet.get_Range(numColu(0) + Convert.ToString(j + 2), Type.Missing);
                    excelcells.set_Value(Type.Missing, String.Format("{0}", Convert.ToString(transitions[j])));
                }

                excelapp.DisplayAlerts = true;
                excelappworkbook.SaveAs(Filename, Type.Missing, Type.Missing,
               Type.Missing, Type.Missing, Type.Missing, ex.XlSaveAsAccessMode.xlNoChange,
               Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                excelappworkbook.Close();
                MessageBox.Show("Le tableur est sauvegardé");
                insert_log("Media saved.", this);
            }
        }
        private string numColu(int f)
        {
            int div = f;
            string colLetter = String.Empty;
            int mod;
            while (div > 0)
            {
                mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = ((div - mod) / 26);
            }
            return colLetter;
        }
        private void chargerToolStripMenuItem_Click(object sender, EventArgs e){
            loaded = true;
            insert_log("Choosing file for loading media...", this);
            OpenFileDialog file = new OpenFileDialog(); //open dialog to choose file
            file.Filter = "Tableur | *.xlsx| Tableur | *.xls| All files | *.*";
            file.Title = "Charger la table";
            string filePath = string.Empty;
            string fileExt = string.Empty;
            if (file.ShowDialog() == DialogResult.OK){
                insert_log("Loading media...", this);
                
                filePath = file.FileName; //get the path of the file
                fileExt = Path.GetExtension(filePath); //get the file extension
                if (fileExt.CompareTo(".xls") == 0 || fileExt.CompareTo(".xlsx") == 0) {
                    //try{
                    source = new List<decimal[,]>();
                    transitions = new List<decimal>();
                    Cursor.Current = Cursors.WaitCursor;
                    ex.Application xlApp = new();
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

                    for (int k = 1; k < xlWorkbook.Sheets.Count; k++){
                        xlWorksheet = xlWorkbook.Sheets[k];
                        xlRange = xlWorksheet.UsedRange;
                        source.Add(new decimal[a - 1, b - 1]);
                        for (int i = 0; i < a - 1; i++)
                            for (int j = 0; j < b - 1; j++){
                                //xlRange = xlWorksheet.UsedRange;
                                var currval = Convert.ToDecimal(xlWorksheet.Cells[i + 2, j + 2].Value);
                                source[k - 1][i, j] = currval;

                                //cleanup
                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                                //release com objects to fully kill excel process from running in the background

                            }
                    }
                    xlWorksheet = xlWorkbook.Sheets[xlWorkbook.Sheets.Count];
                    xlRange = xlWorksheet.UsedRange;
                    transitions = new();
                    dataGridView2.RowCount = 0;
                    for (int k = 0; k < xlWorkbook.Sheets.Count - 2; k++) {                        
                        var valueloc = Convert.ToDecimal(xlWorksheet.Cells[k + 2, 2].Value);
                        transitions.Add(valueloc);
                        dataGridView2.Rows.Add(new object[] { transitions[k], k + 1 });

                    }
                    numericUpDown1.Value = a - 1;
                    numericUpDown2.Value = b - 1;
                    dataGridView1.RowCount = a - 2;
                    dataGridView1.ColumnCount = b - 2;
                    //numericUpDown8.Value = 1;
                    numericUpDown4.Value = source.Count;
                    numericUpDown3.Value = 1;

                    refreshdata();

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
                    //}catch (Exception ex){ MessageBox.Show(ex.Message.ToString());}
                    trouv?.refresh(true);
                    beaucoup?.refr();
                    vran?.refr(true);
                    insert_log("Media loaded.", this);
                }
                else
                {
                    MessageBox.Show("Please choose .xls or .xlsx file only.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            loaded = false;
        }
        private void sûrLauteurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lang == 0)
                MessageBox.Show("Cette application est faite par Ilia Madayëv.");
            else if (lang == 1)
                MessageBox.Show("Приложение было разработано Ильёй Мадаевым.");
        }
        private void calculationsDeChemanPourBeaucoupPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (beaucoup == null)
            {
                beaucoup = new Beaucoup(this);
                if (lang == 1) beaucoup.toRusse();
                beaucoup.Show();
            }
            beaucoup.Focus();
        }
        private void diagrammeDeVoronoїToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (vran == null)
            {
                vran = new Vran(this);
                if (lang == 1) vran.ToRusse();
                vran.Show();
            }
            vran.Focus();
        }

        private List<Color> ColeurList = new List<Color> { Color.Red, Color.Orange, Color.Yellow, Color.YellowGreen,
            Color.GreenYellow, Color.Green, Color.DarkGreen, Color.SkyBlue, Color.Cyan, Color.BlueViolet };

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            vran?.refr(true);
            trouv?.refresh(false);
        }
        private void françaisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lang != 0)
            {
                label1.Text = "Quantitè de lignes";
                label2.Text = "Quantitè de colonnes";
                //button1.Text = "Générater";
                toolStripMenuItem1.Text = "Ficher";
                chargerToolStripMenuItem.Text = "Charger";
                sauvegarderToolStripMenuItem.Text = "Sauvegarder";
                desFenêtresToolStripMenuItem.Text = "Fenêtres";
                desCalculationsDeCheminsToolStripMenuItem.Text = "Calculations de chemins pour deux points";
                langueToolStripMenuItem.Text = "Langue";
                sûrLauteurToolStripMenuItem.Text = "Sûr l\'auteur";
                groupBox1.Text = "Dimensions";
                //button2.Text = "Remplir toutes les unités";

                this.Text = "L\'application pour le diploma: donnes pour commencement";
                calculationsDeChemanPourBeaucoupPointsToolStripMenuItem.Text = "Calculations de cheman pour beaucoup points";
                diagrammeDeVoronoїToolStripMenuItem.Text = "Diagramme de Voronoї";

                groupBox2.Text = "Paramètres de base";
                checkBox5.Text = "Mercredi est changeable";
                label10.Text = "États total:";
                label3.Text = "État actuel:";
                label4.Text = "État initial:";

                checkBox1.Text = "Sauvegarder les unités neg.";
                button2.Text = "Generer les unités";
                groupBox5.Text = "Autres";

                groupBox4.Text = "Reflexion de cellules";
                label7.Text = "La taille de fonte";
                label8.Text = "La quantité de signs";

                arbreCouvrantToolStripMenuItem.Text = "Un arbre couvrant";
                trouv?.toFrancais();
                beaucoup?.toFrancais();
                vran?.ToFrancais();
                mediaset?.ToFrancais();
                lang = 0;
            }
        }
        private void russeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lang != 1)
            {
                label1.Text = "Кол-во строк";
                label2.Text = "Кол-во стоблцов";

                toolStripMenuItem1.Text = "Файл";
                chargerToolStripMenuItem.Text = "Загрузить";
                sauvegarderToolStripMenuItem.Text = "Сохранить";
                desFenêtresToolStripMenuItem.Text = "Окна";
                desCalculationsDeCheminsToolStripMenuItem.Text = "Запуск волны из точки";
                langueToolStripMenuItem.Text = "Язык";
                sûrLauteurToolStripMenuItem.Text = "Об авторе";
                groupBox1.Text = "Измерения";

                this.Text = "Среда для исследования";
                calculationsDeChemanPourBeaucoupPointsToolStripMenuItem.Text = "Вычисление оптимальной точки встречи";
                diagrammeDeVoronoїToolStripMenuItem.Text = "Диаграмма Вороного";
                arbreCouvrantToolStripMenuItem.Text = "Остовное дерево";
                groupBox2.Text = "Параметры среды";
                checkBox5.Text = "Изменяема";
                label10.Text = "Кол-во сост.:";
                label3.Text = "Тек. сост.:";
                label4.Text = "Нач. сост.:";
                environnementAvancésToolStripMenuItem.Text = "Параметры среды";

                button2.Text = "Сгенерировать единицы";
                checkBox1.Text = "Сохранить препятствия";
                groupBox5.Text = "Другое";

                groupBox4.Text = "Отображение ячеек";
                label7.Text = "Размер шрифта";
                label8.Text = "Количество знаков";

                checkBox3.Text = "Показать тепловую карту";

                trouv?.toRusse();
                beaucoup?.toRusse();
                vran?.ToRusse();
                lang = 1;
            }
        }

        private void arbreCouvrantToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (span == null)
            {
                span = new Spann(this);
                if (lang == 1) span.ToRusse();
                span.Show();
            }
            span.Focus();
        }
        internal MediaSettings? mediaset;


        private void environnementAvancésToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mediaset == null)
            {
                mediaset = new MediaSettings(this);
                if (lang == 1) mediaset.ToRusse();
                mediaset.Show();
            }
            mediaset.Focus();
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.DefaultCellStyle.Format = 'N' + numericUpDown6.Value.ToString();
            dataGridView1.AutoResizeColumns();
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.DefaultCellStyle.Font = new System.Drawing.Font("Palatino Linotype", (float)numericUpDown5.Value);
            dataGridView1.AutoResizeColumns();
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0) { transitions[e.RowIndex] = Convert.ToDecimal(dataGridView2.SelectedCells[0].Value); }
            //else if (e.ColumnIndex == 1) { }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked) fillcolors();
            else clearcolors();
        }
        internal void clearcolors()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
        }
        internal void fillcolors()
        {
            decimal maxval = 0, minval = decimal.MaxValue;
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    var d = Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value);
                    if (maxval < d) maxval = d;
                    if (minval > d && d > -1) minval = d;
                }
            if (minval != maxval)
                for (int i = 0; i < dataGridView1.RowCount; i++)
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
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
            else for (int i = 0; i < dataGridView1.RowCount; i++)
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    {
                        if (Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) != -1)
                        {
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Blue;
                        }
                        else dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;
                    }
        }

        private void button2_Click(object sender, EventArgs e) { set_unites(); }
        private void cascade_update(bool changed){
            vran?.refr(changed);
            trouv?.refresh(changed);
        }
        private void numericUpDown8_ValueChanged(object sender, EventArgs e){
            if (numericUpDown4.Value >= numericUpDown8.Value){
                for (int i = 0; i < dataGridView1.RowCount; i++)
                    for (int j = 0; j < dataGridView1.ColumnCount; j++){
                        if (Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) < 0)
                            dataGridView1.Rows[i].Cells[j].Value = source[(int)numericUpDown8.Value - 1][i, j];
                        else dataGridView1.Rows[i].Cells[j].Value = source[(int)numericUpDown8.Value - 1][i, j];
                    }
            }
            else numericUpDown8.Value = numericUpDown4.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e){
            if (source.Count > (int)numericUpDown4.Value){
                source.RemoveAt(source.Count - 1);
                dataGridView2.Rows.RemoveAt(source.Count - 1);
                transitions.RemoveAt(transitions.Count - 1);
                numericUpDown8.Maximum = transitions.Count;
                numericUpDown3.Maximum = transitions.Count;
                numericUpDown8.Value = transitions.Count;
            }
            else{
                int r = (int)numericUpDown1.Value, c = (int)numericUpDown2.Value;
                decimal[,] newsource = new decimal[r, c];
                for (int i = 0; i < r; i++){
                    for (int j = 0; j < c; j++) {
                        newsource[i, j] = source[^1][i, j] * 1.1m;
                    }

                }
                source.Add(newsource);
                dataGridView2.Rows.Add(new object[] { 10, source.Count });
                transitions.Add(10);
                numericUpDown8.Maximum = source.Count;
                numericUpDown3.Maximum = source.Count;
                numericUpDown8.Value = transitions.Count;
            }
            if (checkBox5.Checked) refreshdata();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e){
            bool curr = false;
            int currstate = (int)numericUpDown8.Value - 1;
            if (dataGridView1.SelectedCells[0].Value != null){
                string str = dataGridView1.SelectedCells[0].Value.ToString();
                if ((str[0] >= '0' && str[0] <= '9' || str[0] == '-') && str.Length > 0){
                    curr = true;
                    for (int i = 1; i < str.Length; i++)
                        if ((str[i] < '0' || str[i] > '9') && str[i] != ',')
                        {
                            curr = false;
                            break;
                        }
                }
                if (curr){
                    source[currstate][e.RowIndex, e.ColumnIndex] = Convert.ToDecimal(dataGridView1.SelectedCells[0].Value);
                    if (checkBox3.Checked) fillcolors();
                    trouv?.refresh(true);
                    vran?.refr(true);
                    span?.refr(true);

                }
                else{
                    MessageBox.Show("Invalid number");
                    //if (own.trouv != null) own.trouv.clearcells();
                    dataGridView1.SelectedCells[0].Value = -1;
                }
            }
            else{
                MessageBox.Show("Invalid number");
                //if (own.trouv != null) own.trouv.clearcells();
                dataGridView1.SelectedCells[0].Value = -1;
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e){
            if (numericUpDown4.Value >= numericUpDown8.Value){
                for (int i = 0; i < dataGridView1.RowCount; i++)
                    for (int j = 0; j < dataGridView1.ColumnCount; j++){
                        if (Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) < 0)
                            dataGridView1.Rows[i].Cells[j].Value = source[(int)numericUpDown8.Value - 1][i, j];
                        else dataGridView1.Rows[i].Cells[j].Value = source[(int)numericUpDown8.Value - 1][i, j];
                    }

            }
            else numericUpDown3.Value = numericUpDown1.Value;
            stateShift = (int)numericUpDown3.Value - 1;
            if (checkBox5.Checked) refreshdata();
        }

        private void numericUpDown4_KeyUp(object sender, KeyEventArgs e){ }
    }
}