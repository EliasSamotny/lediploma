namespace l_application_pour_diploma
{
    partial class Spann
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
            pictureBox1 = new PictureBox();
            groupBox3 = new GroupBox();
            numericUpDown3 = new NumericUpDown();
            numericUpDown4 = new NumericUpDown();
            dataGridView1 = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            button5 = new Button();
            button6 = new Button();
            groupBox2 = new GroupBox();
            comboBox1 = new ComboBox();
            button3 = new Button();
            button4 = new Button();
            button1 = new Button();
            groupBox1 = new GroupBox();
            label1 = new Label();
            groupBox4 = new GroupBox();
            groupBox5 = new GroupBox();
            comboBox3 = new ComboBox();
            comboBox2 = new ComboBox();
            groupBox6 = new GroupBox();
            groupBox7 = new GroupBox();
            comboBox4 = new ComboBox();
            comboBox5 = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox6.SuspendLayout();
            groupBox7.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Location = new Point(298, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(740, 665);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.MouseClick += pictureBox1_MouseClick;
            pictureBox1.MouseDoubleClick += pictureBox1_MouseDoubleClick;
            pictureBox1.Resize += pictureBox1_Resize;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(numericUpDown3);
            groupBox3.Controls.Add(numericUpDown4);
            groupBox3.Location = new Point(164, 215);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(128, 56);
            groupBox3.TabIndex = 17;
            groupBox3.TabStop = false;
            groupBox3.Text = "Le point actuel";
            // 
            // numericUpDown3
            // 
            numericUpDown3.Location = new Point(10, 21);
            numericUpDown3.Margin = new Padding(8, 13, 8, 13);
            numericUpDown3.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown3.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown3.Name = "numericUpDown3";
            numericUpDown3.Size = new Size(38, 29);
            numericUpDown3.TabIndex = 1;
            numericUpDown3.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numericUpDown4
            // 
            numericUpDown4.Location = new Point(77, 21);
            numericUpDown4.Margin = new Padding(8, 13, 8, 13);
            numericUpDown4.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown4.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown4.Name = "numericUpDown4";
            numericUpDown4.Size = new Size(40, 29);
            numericUpDown4.TabIndex = 1;
            numericUpDown4.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = true;
            dataGridView1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2 });
            dataGridView1.Location = new Point(12, 381);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.RowTemplate.ReadOnly = true;
            dataGridView1.ShowEditingIcon = false;
            dataGridView1.Size = new Size(280, 296);
            dataGridView1.TabIndex = 19;
            // 
            // Column1
            // 
            Column1.HeaderText = "La ligne";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            // 
            // Column2
            // 
            Column2.HeaderText = "La colonne";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            Column2.Width = 120;
            // 
            // button5
            // 
            button5.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button5.Location = new Point(152, 311);
            button5.Name = "button5";
            button5.Size = new Size(140, 29);
            button5.TabIndex = 21;
            button5.Text = "Suppremer";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button6.Location = new Point(12, 311);
            button6.Name = "button6";
            button6.Size = new Size(134, 29);
            button6.TabIndex = 20;
            button6.Text = "Aujouter";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(comboBox1);
            groupBox2.Location = new Point(12, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(280, 56);
            groupBox2.TabIndex = 22;
            groupBox2.TabStop = false;
            groupBox2.Text = "Le destin de chercher de chemins";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "I Rayon (8 directions)", "II Rayon (16 directions)", "III Rayon (32 directions)" });
            comboBox1.Location = new Point(6, 21);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(268, 30);
            comboBox1.TabIndex = 0;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button3.Location = new Point(130, 276);
            button3.Name = "button3";
            button3.Size = new Size(162, 29);
            button3.TabIndex = 23;
            button3.Text = "Finir les calculations";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(12, 276);
            button4.Name = "button4";
            button4.Size = new Size(112, 29);
            button4.TabIndex = 24;
            button4.Text = "Un pas";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button1.Location = new Point(12, 346);
            button1.Name = "button1";
            button1.Size = new Size(280, 29);
            button1.TabIndex = 24;
            button1.Text = "Définir par défaut";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 215);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(146, 55);
            groupBox1.TabIndex = 25;
            groupBox1.TabStop = false;
            groupBox1.Text = "Longueur d'arbre";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 25);
            label1.Name = "label1";
            label1.Size = new Size(18, 22);
            label1.TabIndex = 0;
            label1.Text = "0";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(groupBox5);
            groupBox4.Controls.Add(comboBox2);
            groupBox4.Location = new Point(12, 74);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(280, 56);
            groupBox4.TabIndex = 22;
            groupBox4.TabStop = false;
            groupBox4.Text = "L'algorythme";
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(comboBox3);
            groupBox5.Location = new Point(158, 57);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(280, 56);
            groupBox5.TabIndex = 26;
            groupBox5.TabStop = false;
            groupBox5.Text = "L'algorythme";
            // 
            // comboBox3
            // 
            comboBox3.FormattingEnabled = true;
            comboBox3.Items.AddRange(new object[] { "Fermat", "Kruskal" });
            comboBox3.Location = new Point(6, 21);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new Size(268, 30);
            comboBox3.TabIndex = 0;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Items.AddRange(new object[] { "Fermat", "Kruskal" });
            comboBox2.Location = new Point(6, 21);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(268, 30);
            comboBox2.TabIndex = 0;
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(groupBox7);
            groupBox6.Controls.Add(comboBox5);
            groupBox6.Location = new Point(12, 136);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(280, 56);
            groupBox6.TabIndex = 26;
            groupBox6.TabStop = false;
            groupBox6.Text = "Politique de routage (pour Krouskal)";
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(comboBox4);
            groupBox7.Location = new Point(158, 57);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new Size(280, 56);
            groupBox7.TabIndex = 26;
            groupBox7.TabStop = false;
            groupBox7.Text = "L'algorythme";
            // 
            // comboBox4
            // 
            comboBox4.FormattingEnabled = true;
            comboBox4.Items.AddRange(new object[] { "Fermat", "Kruskal" });
            comboBox4.Location = new Point(6, 21);
            comboBox4.Name = "comboBox4";
            comboBox4.Size = new Size(268, 30);
            comboBox4.TabIndex = 0;
            // 
            // comboBox5
            // 
            comboBox5.FormattingEnabled = true;
            comboBox5.Items.AddRange(new object[] { "Pas de point principal", "Premier point est principal" });
            comboBox5.Location = new Point(6, 21);
            comboBox5.Name = "comboBox5";
            comboBox5.Size = new Size(268, 30);
            comboBox5.TabIndex = 0;
            comboBox5.SelectedIndexChanged += comboBox5_SelectedIndexChanged;
            // 
            // Spann
            // 
            AutoScaleDimensions = new SizeF(9F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1050, 689);
            Controls.Add(groupBox6);
            Controls.Add(groupBox1);
            Controls.Add(button3);
            Controls.Add(button1);
            Controls.Add(button4);
            Controls.Add(button5);
            Controls.Add(button6);
            Controls.Add(dataGridView1);
            Controls.Add(groupBox3);
            Controls.Add(groupBox4);
            Controls.Add(groupBox2);
            Controls.Add(pictureBox1);
            Font = new Font("Palatino Linotype", 12F, FontStyle.Regular, GraphicsUnit.Point);
            Margin = new Padding(4);
            Name = "Spann";
            Text = "Spanning tree";
            FormClosing += Spann_FormClosing;
            Load += Spann_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            groupBox6.ResumeLayout(false);
            groupBox7.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private GroupBox groupBox3;
        private NumericUpDown numericUpDown3;
        private NumericUpDown numericUpDown4;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private Button button5;
        private Button button6;
        private GroupBox groupBox2;
        private ComboBox comboBox1;
        private Button button3;
        private Button button4;
        private Button button1;
        private GroupBox groupBox1;
        private Label label1;
        private GroupBox groupBox4;
        private ComboBox comboBox2;
        private GroupBox groupBox5;
        private ComboBox comboBox3;
        private GroupBox groupBox6;
        private GroupBox groupBox7;
        private ComboBox comboBox4;
        private ComboBox comboBox5;
    }
}