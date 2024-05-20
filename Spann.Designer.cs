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
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Location = new Point(298, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(740, 624);
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
            groupBox3.Location = new Point(18, 74);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(153, 56);
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
            numericUpDown3.ReadOnly = true;
            numericUpDown3.Size = new Size(61, 29);
            numericUpDown3.TabIndex = 1;
            numericUpDown3.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numericUpDown4
            // 
            numericUpDown4.Location = new Point(81, 22);
            numericUpDown4.Margin = new Padding(8, 13, 8, 13);
            numericUpDown4.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDown4.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown4.Name = "numericUpDown4";
            numericUpDown4.ReadOnly = true;
            numericUpDown4.Size = new Size(61, 29);
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
            dataGridView1.Location = new Point(12, 340);
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
            button5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button5.Location = new Point(152, 305);
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
            button6.Location = new Point(12, 305);
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
            button3.Anchor = AnchorStyles.Left;
            button3.Location = new Point(130, 270);
            button3.Name = "button3";
            button3.Size = new Size(162, 29);
            button3.TabIndex = 23;
            button3.Text = "Finir les calculations";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(12, 270);
            button4.Name = "button4";
            button4.Size = new Size(112, 29);
            button4.TabIndex = 24;
            button4.Text = "Recomputer";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // Spann
            // 
            AutoScaleDimensions = new SizeF(9F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1050, 648);
            Controls.Add(button3);
            Controls.Add(button4);
            Controls.Add(groupBox2);
            Controls.Add(button5);
            Controls.Add(button6);
            Controls.Add(dataGridView1);
            Controls.Add(groupBox3);
            Controls.Add(pictureBox1);
            Font = new Font("Palatino Linotype", 12F, FontStyle.Regular, GraphicsUnit.Point);
            Margin = new Padding(4);
            Name = "Spann";
            Text = "Spanning tree";
            FormClosing += Spann_FormClosing;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            groupBox2.ResumeLayout(false);
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
    }
}