namespace ReadFrxRes1
{
    partial class Form1
    {
        private System.Windows.Forms.ToolTip toolTip1 = null;
        public System.Windows.Forms.Button cmdOpenFile = null;
        public System.Windows.Forms.Button cmdSavePicture = null;
        public System.Windows.Forms.ListBox List1 = null;
        public System.Windows.Forms.PictureBox Picture1 = null;
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing);
        }

#region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmdOpenFile = new System.Windows.Forms.Button();
            this.cmdSavePicture = new System.Windows.Forms.Button();
            this.List1 = new System.Windows.Forms.ListBox();
            this.Picture1 = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.Picture1)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdOpenFile
            // 
            this.cmdOpenFile.Location = new System.Drawing.Point(8, 498);
            this.cmdOpenFile.Name = "cmdOpenFile";
            this.cmdOpenFile.Size = new System.Drawing.Size(81, 33);
            this.cmdOpenFile.TabIndex = 3;
            this.cmdOpenFile.Text = "打开文件";
            this.cmdOpenFile.Click += new System.EventHandler(this.cmdOpenFile_Click);
            // 
            // cmdSavePicture
            // 
            this.cmdSavePicture.Location = new System.Drawing.Point(95, 498);
            this.cmdSavePicture.Name = "cmdSavePicture";
            this.cmdSavePicture.Size = new System.Drawing.Size(81, 33);
            this.cmdSavePicture.TabIndex = 2;
            this.cmdSavePicture.Text = "保存图片";
            this.cmdSavePicture.Click += new System.EventHandler(this.cmdSavePicture_Click);
            // 
            // List1
            // 
            this.List1.ItemHeight = 14;
            this.List1.Location = new System.Drawing.Point(8, 8);
            this.List1.Name = "List1";
            this.List1.Size = new System.Drawing.Size(195, 368);
            this.List1.TabIndex = 1;
            this.List1.Click += new System.EventHandler(this.List1_Click);
            // 
            // Picture1
            // 
            this.Picture1.Location = new System.Drawing.Point(209, 1);
            this.Picture1.Name = "Picture1";
            this.Picture1.Size = new System.Drawing.Size(467, 375);
            this.Picture1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Picture1.TabIndex = 0;
            this.Picture1.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(8, 382);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(668, 110);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(182, 498);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(494, 32);
            this.textBox1.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(676, 533);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.cmdOpenFile);
            this.Controls.Add(this.cmdSavePicture);
            this.Controls.Add(this.List1);
            this.Controls.Add(this.Picture1);
            this.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Location = new System.Drawing.Point(4, 30);
            this.Name = "Form1";
            this.Text = "读取Frx Ctx文件";
            this.Resize += new System.EventHandler(this.Form_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.Picture1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TextBox textBox1;
    }
}
