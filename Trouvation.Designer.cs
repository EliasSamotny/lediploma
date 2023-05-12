namespace l_application_pour_diploma
{
    partial class Trouvation
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            dataGridView1 = new DataGridView();
            groupBox1 = new GroupBox();
            radioButton3 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton1 = new RadioButton();
            label2 = new Label();
            label3 = new Label();
            groupBox2 = new GroupBox();
            numericUpDown2 = new NumericUpDown();
            numericUpDown1 = new NumericUpDown();
            groupBox3 = new GroupBox();
            numericUpDown4 = new NumericUpDown();
            numericUpDown3 = new NumericUpDown();
            label1 = new Label();
            label4 = new Label();
            groupBox4 = new GroupBox();
            checkBox1 = new CheckBox();
            menuStrip1 = new MenuStrip();
            toolStripMenuItem1 = new ToolStripMenuItem();
            chargerToolStripMenuItem = new ToolStripMenuItem();
            sauvegarderToolStripMenuItem = new ToolStripMenuItem();
            desFenêtresToolStripMenuItem = new ToolStripMenuItem();
            desCalculationsDeCheminsToolStripMenuItem = new ToolStripMenuItem();
            textBox1 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).BeginInit();
            groupBox4.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToOrderColumns = true;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.ColumnHeadersVisible = false;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Window;
            dataGridViewCellStyle1.Font = new Font("Palatino Linotype", 12F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle1.Format = "N2";
            dataGridViewCellStyle1.NullValue = null;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.False;
            dataGridView1.DefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.Location = new Point(12, 230);
            dataGridView1.Margin = new Padding(5, 6, 5, 6);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new Size(606, 310);
            dataGridView1.TabIndex = 7;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBox1.Controls.Add(radioButton3);
            groupBox1.Controls.Add(radioButton2);
            groupBox1.Controls.Add(radioButton1);
            groupBox1.Location = new Point(364, 33);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(254, 120);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Le destin de chercher de chemins";
            // 
            // radioButton3
            // 
            radioButton3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            radioButton3.AutoSize = true;
            radioButton3.Checked = true;
            radioButton3.Location = new Point(9, 88);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(186, 26);
            radioButton3.TabIndex = 0;
            radioButton3.TabStop = true;
            radioButton3.Text = "III rayon (32 directions)";
            radioButton3.UseVisualStyleBackColor = true;
            radioButton3.CheckedChanged += radioButton3_CheckedChanged;
            // 
            // radioButton2
            // 
            radioButton2.Anchor = AnchorStyles.Left;
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(9, 55);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(181, 26);
            radioButton2.TabIndex = 0;
            radioButton2.Text = "II rayon (16 directions)";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(9, 23);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(168, 26);
            radioButton1.TabIndex = 0;
            radioButton1.Text = "I rayon (8 directions)";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(7, 56);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(84, 22);
            label2.TabIndex = 10;
            label2.Text = "La colonne";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(7, 25);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(65, 22);
            label3.TabIndex = 11;
            label3.Text = "La ligne";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(numericUpDown2);
            groupBox2.Controls.Add(numericUpDown1);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(label3);
            groupBox2.Location = new Point(12, 134);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(238, 87);
            groupBox2.TabIndex = 12;
            groupBox2.TabStop = false;
            groupBox2.Text = "Cellule selectée";
            // 
            // numericUpDown2
            // 
            numericUpDown2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            numericUpDown2.InterceptArrowKeys = false;
            numericUpDown2.Location = new Point(162, 54);
            numericUpDown2.Margin = new Padding(5, 6, 5, 6);
            numericUpDown2.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown2.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.ReadOnly = true;
            numericUpDown2.Size = new Size(70, 29);
            numericUpDown2.TabIndex = 12;
            numericUpDown2.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown2.ValueChanged += numericUpDown2_ValueChanged;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numericUpDown1.InterceptArrowKeys = false;
            numericUpDown1.Location = new Point(162, 23);
            numericUpDown1.Margin = new Padding(4);
            numericUpDown1.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.ReadOnly = true;
            numericUpDown1.Size = new Size(70, 29);
            numericUpDown1.TabIndex = 12;
            numericUpDown1.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(numericUpDown4);
            groupBox3.Controls.Add(numericUpDown3);
            groupBox3.Controls.Add(label1);
            groupBox3.Controls.Add(label4);
            groupBox3.Location = new Point(12, 33);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(238, 95);
            groupBox3.TabIndex = 12;
            groupBox3.TabStop = false;
            groupBox3.Text = "Reflexion de cellules";
            // 
            // numericUpDown4
            // 
            numericUpDown4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numericUpDown4.Location = new Point(162, 58);
            numericUpDown4.Margin = new Padding(5, 6, 5, 6);
            numericUpDown4.Maximum = new decimal(new int[] { 25, 0, 0, 0 });
            numericUpDown4.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
            numericUpDown4.Name = "numericUpDown4";
            numericUpDown4.ReadOnly = true;
            numericUpDown4.Size = new Size(70, 29);
            numericUpDown4.TabIndex = 12;
            numericUpDown4.Value = new decimal(new int[] { 12, 0, 0, 0 });
            numericUpDown4.ValueChanged += numericUpDown4_ValueChanged;
            // 
            // numericUpDown3
            // 
            numericUpDown3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numericUpDown3.Location = new Point(162, 23);
            numericUpDown3.Margin = new Padding(5, 6, 5, 6);
            numericUpDown3.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            numericUpDown3.Name = "numericUpDown3";
            numericUpDown3.ReadOnly = true;
            numericUpDown3.Size = new Size(70, 29);
            numericUpDown3.TabIndex = 12;
            numericUpDown3.Value = new decimal(new int[] { 2, 0, 0, 0 });
            numericUpDown3.ValueChanged += numericUpDown3_ValueChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 60);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(122, 22);
            label1.TabIndex = 11;
            label1.Text = "La taille de fonte";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(7, 25);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(146, 22);
            label4.TabIndex = 11;
            label4.Text = "La quantité de signs";
            // 
            // groupBox4
            // 
            groupBox4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBox4.Controls.Add(checkBox1);
            groupBox4.Location = new Point(364, 159);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(254, 62);
            groupBox4.TabIndex = 15;
            groupBox4.TabStop = false;
            groupBox4.Text = "La carte de chaleur des distances";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Checked = true;
            checkBox1.CheckState = CheckState.Checked;
            checkBox1.Location = new Point(9, 26);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(136, 26);
            checkBox1.TabIndex = 5;
            checkBox1.Text = "Afficher la carte";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // menuStrip1
            // 
            menuStrip1.Font = new Font("Palatino Linotype", 12F, FontStyle.Regular, GraphicsUnit.Point);
            menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, desFenêtresToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(630, 30);
            menuStrip1.TabIndex = 16;
            menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { chargerToolStripMenuItem, sauvegarderToolStripMenuItem });
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(64, 26);
            toolStripMenuItem1.Text = "Ficher";
            // 
            // chargerToolStripMenuItem
            // 
            chargerToolStripMenuItem.Name = "chargerToolStripMenuItem";
            chargerToolStripMenuItem.Size = new Size(166, 26);
            chargerToolStripMenuItem.Text = "Charger";
            // 
            // sauvegarderToolStripMenuItem
            // 
            sauvegarderToolStripMenuItem.Name = "sauvegarderToolStripMenuItem";
            sauvegarderToolStripMenuItem.Size = new Size(166, 26);
            sauvegarderToolStripMenuItem.Text = "Sauvegarder";
            // 
            // desFenêtresToolStripMenuItem
            // 
            desFenêtresToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { desCalculationsDeCheminsToolStripMenuItem });
            desFenêtresToolStripMenuItem.Name = "desFenêtresToolStripMenuItem";
            desFenêtresToolStripMenuItem.Size = new Size(78, 26);
            desFenêtresToolStripMenuItem.Text = "Fenêtres";
            // 
            // desCalculationsDeCheminsToolStripMenuItem
            // 
            desCalculationsDeCheminsToolStripMenuItem.Name = "desCalculationsDeCheminsToolStripMenuItem";
            desCalculationsDeCheminsToolStripMenuItem.Size = new Size(314, 26);
            desCalculationsDeCheminsToolStripMenuItem.Text = "Calculations de routes le plus court";
            desCalculationsDeCheminsToolStripMenuItem.Click += desCalculationsDeCheminsToolStripMenuItem_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(256, 192);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(100, 29);
            textBox1.TabIndex = 17;
            // 
            // Trouvation
            // 
            AutoScaleDimensions = new SizeF(9F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(630, 555);
            Controls.Add(textBox1);
            Controls.Add(menuStrip1);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(dataGridView1);
            Font = new Font("Palatino Linotype", 12F, FontStyle.Regular, GraphicsUnit.Point);
            Margin = new Padding(4);
            Name = "Trouvation";
            Text = "Trouvation un chemin le plus court";
            FormClosing += Trouvation_FormClosing;
            Load += Trouvation_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).EndInit();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        internal DataGridView dataGridView1;
        private GroupBox groupBox1;
        internal RadioButton radioButton3;
        internal RadioButton radioButton2;
        internal RadioButton radioButton1;
        private Label label2;
        private Label label3;
        private GroupBox groupBox2;
        private NumericUpDown numericUpDown2;
        private NumericUpDown numericUpDown1;
        private GroupBox groupBox3;
        private NumericUpDown numericUpDown3;
        private Label label4;
        private NumericUpDown numericUpDown4;
        private Label label1;
        private GroupBox groupBox4;
        private CheckBox checkBox1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem chargerToolStripMenuItem;
        private ToolStripMenuItem sauvegarderToolStripMenuItem;
        private ToolStripMenuItem desFenêtresToolStripMenuItem;
        private ToolStripMenuItem desCalculationsDeCheminsToolStripMenuItem;
        private TextBox textBox1;
    }
}