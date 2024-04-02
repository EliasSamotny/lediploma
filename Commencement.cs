using ex = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Diagnostics;

namespace l_application_pour_diploma
{
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
        internal decimal[,] source;
        private bool global_log_allowed = true;
        private static readonly object log_locker = new object();
        string logFilePath = "C:\\Users\\Elias\\Desktop\\lediploma\\log.log";
        public void insert_log(string log_mess, Form caller) {
            if (global_log_allowed){
                lock (log_locker){
                    using (StreamWriter sw = new StreamWriter(logFilePath, true)){
                        // Get the current timestamp
                        string timestamp = DateTime.Now.ToString();

                        // Add the log string with the timestamp to the file
                        string logString = timestamp + $", {caller.Text}: {log_mess}";
                        sw.WriteLine(logString);
                    }
                }
            }
        }
        public Commencement()
        {
            InitializeComponent();
            trouv = null;
            dataGridView1.RowCount = (int)numericUpDown1.Value;
            dataGridView1.ColumnCount = (int)numericUpDown2.Value;
            lang = 0;

            refreshdata();
            dataGridView1.AutoResizeColumns();
            domains = new();
        }
        internal Trouvation? trouv;
        internal Beaucoup? beaucoup;
        internal Vran? vran;
        private void Form1_Load(object sender, EventArgs e)
        {
            insert_log("Loading form...", this);
            russeToolStripMenuItem_Click(sender, e);
            dataGridView1.AutoResizeColumns();
            set_mount();
            int r = dataGridView1.RowCount, c = dataGridView1.ColumnCount;
            source = new decimal[r, c];
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    source[i, j] = Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value);
                }
            }
            insert_log("Form loaded.", this);
        }
        private void set_mount()
        {
            int r = dataGridView1.RowCount, c = dataGridView1.ColumnCount;

            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    try
                    {
                        dataGridView1.Rows[i].Cells[j].Value = mountagne(i, j, r, c);
                    }
                    catch (Exception)
                    {
                        dataGridView1.Rows[i].Cells[j].Value = -1;
                    }
                }
            }
            if (checkBox3.Checked) fillcolors();
            else clearcolors();
        }
        private decimal mountagne(int i, int j, int r, int c)
        {
            double icoef = 0.5;
            double jcoef = 0.5;
            return 1 / (decimal)(Math.Pow((i - r / 2) * icoef, 2) / 2 + Math.Pow((j - c / 2) * jcoef, 2) / 2);
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e) { refreshdata(); }
        private void refreshdata()
        {
            insert_log("Refreshing forms...", this);
            decimal[,] data1 = new decimal[dataGridView1.RowCount, dataGridView1.ColumnCount];
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    try
                    {
                        data1[i, j] = Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value);
                    }
                    catch (Exception) { data1[i, j] = 0; }

            int i1 = Math.Min((int)numericUpDown1.Value, dataGridView1.RowCount);
            int j1 = Math.Min((int)numericUpDown2.Value, dataGridView1.ColumnCount);
            dataGridView1.RowCount = (int)numericUpDown1.Value;
            dataGridView1.ColumnCount = (int)numericUpDown2.Value;
            int r = dataGridView1.RowCount, c = dataGridView1.ColumnCount;
            source = new decimal[r, c];
            for (int i = 0; i < i1; i++)
                for (int j = 0; j < j1; j++)
                {
                    source[i, j] = data1[i, j];
                    dataGridView1.Rows[i].Cells[j].Value = data1[i, j];
                }

            trouv?.refresh(true);
            if (checkBox3.Checked) fillcolors();
            else clearcolors();
            vran?.refr(true);
            dataGridView1.AutoResizeColumns();
            dataGridView1.AutoResizeRows();
            insert_log("Forms refreshed...", this);
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e) { refreshdata(); }
        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e) { }
        private void numericUpDown1_KeyUp(object sender, KeyEventArgs e) { }
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
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            bool curr = false;
            if (dataGridView1.SelectedCells[0].Value != null)
            {
                string str = dataGridView1.SelectedCells[0].Value.ToString();
                if ((str[0] >= '0' && str[0] <= '9' || str[0] == '-') && str.Length > 0)
                {
                    curr = true;
                    for (int i = 1; i < str.Length; i++)
                        if ((str[i] < '0' || str[i] > '9') && str[i] != ',')
                        {
                            curr = false;
                            break;
                        }
                }
                if (curr) { refreshdata(); }
                else
                {
                    MessageBox.Show("Invalid number");
                    if (trouv != null) trouv.clearcells();
                    dataGridView1.SelectedCells[0].Value = -1;
                }
            }
            else
            {
                MessageBox.Show("Invalid number");
                if (trouv != null) trouv.clearcells();
                dataGridView1.SelectedCells[0].Value = -1;
            }
        }
        private void sauvegarderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insert_log("Choosing file for saving media...", this);
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Tableur | *.xlsx| Tableur | *.xls| All files | *.*";
            saveFileDialog1.Title = "Sauvegarder la table";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                insert_log("Saving media...", this);
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
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    {
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
        private void chargerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            insert_log("Choosing file for loading media...", this);
            OpenFileDialog file = new OpenFileDialog(); //open dialog to choose file
            file.Filter = "Tableur | *.xlsx| Tableur | *.xls| All files | *.*";
            file.Title = "Charger la table";
            string filePath = string.Empty;
            string fileExt = string.Empty;
            if (file.ShowDialog() == DialogResult.OK)
            {
                insert_log("Loading media...", this);
                filePath = file.FileName; //get the path of the file
                fileExt = Path.GetExtension(filePath); //get the file extension
                if (fileExt.CompareTo(".xls") == 0 || fileExt.CompareTo(".xlsx") == 0)
                {
                    try
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        ex.Application xlApp = new();
                        ex.Workbook xlWorkbook = xlApp.Workbooks.Open(filePath);
                        ex._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
                        ex.Range xlRange = xlWorksheet.UsedRange;
                        int a = 1, b = 1;
                        while (true)
                        {
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
                            for (int j = 0; j < b - 1; j++)
                            {
                                var currval = Convert.ToDecimal(xlWorksheet.Cells[i + 2, j + 2].Value);
                                source[i, j] = currval;
                                dataGridView1.Rows[i].Cells[j].Value = currval;
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
            dataGridView1.AutoResizeColumns();
                     
        }
        private void showCurrCoord()
        {
            textBox1.Text = (dataGridView1.SelectedCells[0].RowIndex + 1).ToString();
            textBox2.Text = (dataGridView1.SelectedCells[0].ColumnIndex + 1).ToString();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) { showCurrCoord(); }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e) { showCurrCoord(); }

        private void button2_Click(object sender, EventArgs e)
        {
            set_unites();
        }
        private void set_unites()
        {
            int r = dataGridView1.RowCount, c = dataGridView1.ColumnCount;
            source = new decimal[r, c];
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    if (checkBox1.Checked && Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) < 0)
                        dataGridView1.Rows[i].Cells[j].Value = -1;
                    else dataGridView1.Rows[i].Cells[j].Value = 1;
                    source[i, j] = Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value);
                }
            try { dataGridView1.Rows[0].Cells[0].Value = (decimal)dataGridView1.Rows[0].Cells[0].Value; }
            catch (Exception) { dataGridView1.Rows[0].Cells[0].Value = (int)dataGridView1.Rows[0].Cells[0].Value; }
            trouv?.refresh(true);
            vran?.refr(true);
            if (checkBox3.Checked) fillcolors();
            else clearcolors();
        }
        private void sûrLauteurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lang == 0)
                MessageBox.Show("Cette application est faite par Ilia Madayëv.");
            else if (lang == 1)
                MessageBox.Show("Приложение было разработано Ильёй Мадаевым.");
        }
        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e) { }
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e) { }
        private void dataGridView1_KeyUp(object sender, KeyEventArgs e) { }
        internal void clearcolors()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
        }
        internal void fillcolors()
        {
            decimal maxval = 0, minval = Decimal.MaxValue;
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
        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.DefaultCellStyle.Format = 'N' + numericUpDown6.Value.ToString();
            dataGridView1.AutoResizeColumns();
        }
        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.DefaultCellStyle.Font = new Font("Palatino Linotype", (float)numericUpDown5.Value);
            dataGridView1.AutoResizeColumns();
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked) fillcolors();
            else clearcolors();
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
        int tick = 0;
        private void dataGridView1_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            int col = dataGridView1.ColumnCount - 1;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Rows[i].Cells[col].Value = dataGridView1.Rows[i].Cells[col - 1].Value;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (tick < (int)numericUpDown8.Value + 1)
                tick++;
            else
                tick = 0;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    if (!checkBox2.Checked)
                    {
                        if (Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value) > -1)
                        {
                            if (radioButton1.Checked)
                                dataGridView1.Rows[i].Cells[j].Value = 1 / Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value);
                            if (radioButton2.Checked)
                            {
                                if (tick < (int)numericUpDown8.Value)
                                    dataGridView1.Rows[i].Cells[j].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value) * (100 + (double)numericUpDown9.Value) / 100;
                                else
                                    dataGridView1.Rows[i].Cells[j].Value = source[i, j];
                            }

                        }
                    }
                    else if (domains.Any(list => list.Contains(new(i, j))))
                    {
                        if (radioButton1.Checked)
                            dataGridView1.Rows[i].Cells[j].Value = 1 / Convert.ToDecimal(dataGridView1.Rows[i].Cells[j].Value);
                        if (radioButton2.Checked)
                        {
                            if (tick < (int)numericUpDown8.Value)
                                dataGridView1.Rows[i].Cells[j].Value = Convert.ToDouble(dataGridView1.Rows[i].Cells[j].Value) * (100 + (double)numericUpDown9.Value) / 100;
                            else
                                dataGridView1.Rows[i].Cells[j].Value = source[i, j];
                        }

                    }


                }
            }
            trouv?.refresh(true);
            vran?.refr(true);
            if (checkBox3.Checked) fillcolors();
            else clearcolors();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked && checkBox5.Checked)
            {
                timer1.Enabled = true;
                timer1.Start();
            }
            else
            {
                timer1.Enabled = false;
                for (int i = 0; i < dataGridView1.RowCount; i++)
                    for (int j = 0; j < dataGridView1.ColumnCount; j++)
                        dataGridView1.Rows[i].Cells[j].Value = source[i, j];
            }
            vran?.refr(false);
            trouv?.refresh(false);
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = (int)numericUpDown7.Value;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            tick = 0;
            trouv?.refresh(true);
            vran?.refr(false);
        }
        public List<List<Point>> domains;
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked && domains.Count == 0)
            {
                checkBox2.Checked = false;
                if (lang == 0) MessageBox.Show("Vous n'avez chosis les domains.");
                if (lang == 1) MessageBox.Show("Области изменения не выбраны.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                List<Point> newd = new();
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    var el = dataGridView1.SelectedCells[i];
                    if (!domains.Any(list => list.Contains(new(el.RowIndex, el.ColumnIndex))) && Convert.ToDecimal(el.Value) > 0)
                    { //check if already included
                        newd.Add(new(el.RowIndex, el.ColumnIndex));
                    }
                }
                domains.Add(newd);
                textBox3.Text = Convert.ToString(domains.Count);
                textBox4.Text = Convert.ToString(domains.Sum(list => list.Count));
                dataGridView1.ClearSelection();
                affichdom();
            }
            else
            {
                if (lang == 0)
                {
                    MessageBox.Show("Choisir le domain pour à ajouter");
                }
                else if (lang == 1)
                    MessageBox.Show("Выберите область для добавления");
            }
        }
        private List<Color> ColeurList = new List<Color> { Color.Red, Color.Orange, Color.Yellow, Color.YellowGreen,
            Color.GreenYellow, Color.Green, Color.DarkGreen, Color.SkyBlue, Color.Cyan, Color.BlueViolet };
        private void button4_Click(object sender, EventArgs e) { affichdom(); }
        private void affichdom()
        {
            checkBox3.Checked = false;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    if (domains.Any(list => list.Contains(new(i, j))))
                    {
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = ColeurList[domains.FindIndex(list => list.Contains(new(i, j))) % ColeurList.Count];
                    }
                    else
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                }

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            domains = new();
            textBox3.Text = "0";
            textBox4.Text = "0";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                for (int i = 0; i < dataGridView1.SelectedCells.Count; i++)
                {
                    var el = dataGridView1.SelectedCells[i];
                    if (domains.Any(list => list.Contains(new(el.RowIndex, el.ColumnIndex))))
                    { //check if already included
                        int index = domains.FindIndex(l => l.Contains(new(el.RowIndex, el.ColumnIndex)));
                        domains[index].Remove(new(el.RowIndex, el.ColumnIndex));
                    }
                }
                domains.RemoveAll(l => l.Count == 0);
                textBox3.Text = Convert.ToString(domains.Count);
                textBox4.Text = Convert.ToString(domains.Sum(list => list.Count));
                dataGridView1.ClearSelection();
                affichdom();
            }
            else
            {
                if (lang == 0)
                {
                    MessageBox.Show("Choisir le domain pour à suppremer");
                }
                else if (lang == 1)
                    MessageBox.Show("Выберите область для удаления");
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked && checkBox5.Checked)
            {
                timer1.Enabled = true;
                timer1.Start();
            }
            else
            {
                timer1.Enabled = false;
            }
        }

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            vran?.refr(true);
            trouv?.refresh(false);
        }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                trouv?.refresh(true);
                vran?.refr(false);
            }
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            trouv?.refresh(true);
            vran?.refr(false);
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown3.Maximum = numericUpDown8.Value;
            numericUpDown3.Value = 0;
            trouv?.refresh(true);
            vran?.refr(false);

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
                this.Text = "L\'application pour le diploma: donnes pour commencement";
                groupBox4.Text = "Reflexion de cellules";
                label7.Text = "La taille de fonte";
                label8.Text = "La quantité de signs";
                groupBox5.Text = "Autres";
                checkBox3.Text = "Afficher la carte de chaleur";
                calculationsDeChemanPourBeaucoupPointsToolStripMenuItem.Text = "Calculations de cheman pour beaucoup points";
                diagrammeDeVoronoїToolStripMenuItem.Text = "Diagramme de Voronoї";
                groupBox6.Text = "Changement de milieu";
                checkBox4.Text = "Changeable";
                label9.Text = "Temps de changement (ms)";

                radioButton1.Text = "Inverser";
                radioButton2.Text = "Changer N fois par M % et revenir";
                label5.Text = "Etat initial";

                groupBox6.Text = "Changement de milieu";
                checkBox4.Text = "Changeable";
                checkBox5.Text = "Reflexer les changements";
                label9.Text = "Temps de changement (ms)";

                groupBox7.Text = "Conformité de changement";
                label13.Text = "Unités";

                button1.Text = "Aujouter";
                button3.Text = "Suppremer";
                button4.Text = "Afficher";
                button5.Text = "Nettoyer";

                groupBox8.Text = "Quantité";
                label14.Text = "Domains";
                label6.Text = "Cellules";

                trouv?.toFrancais();
                beaucoup?.toFrancais();
                vran?.ToFrancais();
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
                checkBox2.Text = "Использовать";
                desFenêtresToolStripMenuItem.Text = "Окна";
                desCalculationsDeCheminsToolStripMenuItem.Text = "Запуск волны из точки";
                langueToolStripMenuItem.Text = "Язык";
                sûrLauteurToolStripMenuItem.Text = "Об авторе";
                groupBox1.Text = "Измерения";
                groupBox2.Text = "Исследуемая точка";
                label4.Text = "Строка";
                label3.Text = "Столбец";
                button2.Text = "Сгенерировать единицы";
                groupBox3.Text = "Области изменения";
                checkBox1.Text = "Сохранить препятствия";
                this.Text = "L\'application pour le diploma: данные для исследования";
                groupBox4.Text = "Отображение ячеек";
                label7.Text = "Размер шрифта";
                label8.Text = "Количество знаков";
                groupBox5.Text = "Другое";
                checkBox3.Text = "Показать тепловую карту";
                calculationsDeChemanPourBeaucoupPointsToolStripMenuItem.Text = "Вычисление оптимальной точки встречи";
                diagrammeDeVoronoїToolStripMenuItem.Text = "Диаграмма Вороного";

                groupBox6.Text = "Изменяемость среды";
                checkBox4.Text = "Изменяема";
                checkBox5.Text = "Отображать изменения";
                label9.Text = "Время (мс)";

                radioButton1.Text = "Замена обратными числами";
                radioButton2.Text = "Изменить N раз на M % и вернуть исходные значения";
                label5.Text = "Начальное состояние";

                groupBox7.Text = "Настройки изменений";
                label13.Text = "Единицы";

                button1.Text = "Добавить";
                button3.Text = "Удалить";
                button4.Text = "Показать";
                button5.Text = "Очистить";

                groupBox8.Text = "Кол-во";
                label14.Text = "Областей";
                label6.Text = "Клеток";

                trouv?.toRusse();
                beaucoup?.toRusse();
                vran?.ToRusse();
                lang = 1;
            }
        }
    }
}