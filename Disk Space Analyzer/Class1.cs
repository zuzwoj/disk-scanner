using System.IO;
using System.Windows.Forms.VisualStyles;

namespace Disk_Space_Analyzer
{
    public partial class popupForm : Form
    {
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private ListView listView1;
        private ColumnHeader name;
        private ColumnHeader total;
        private ColumnHeader free;
        private ColumnHeader usedtotal;
        private RadioButton radioButton3;
        private Button button1;
        private Button button2;
        private FolderBrowserDialog folderBrowserDialog1;
        private Button button3;
        System.Windows.Forms.DialogResult result = 0;
        private ColumnHeader progressbar;
        private ComboBox comboBox1;
        public List<string> list = new List<string>();

        public popupForm()
        {
            InitializeComponent();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                ListViewItem newItem = new ListViewItem();
                int taken = (int)(100 * (double)d.AvailableFreeSpace / (double)d.TotalSize);
                newItem.Text = d.Name;
                newItem.SubItems.Add((d.TotalSize / 1073741824).ToString() + "GB");
                newItem.SubItems.Add((d.AvailableFreeSpace / 1073741824).ToString() + "GB");
                newItem.SubItems.Add("etwas");
                newItem.SubItems.Add(taken.ToString() + "%");
                listView1.Items.Add(newItem);
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                ProgressBar pb = new ProgressBar();
                Rectangle r = listView1.GetItemRect(0);
                r.X = r.X + listView1.Columns[0].Width + listView1.Columns[1].Width + +listView1.Columns[2].Width;
                r.Width = listView1.Columns[3].Width;
                pb.SetBounds(r.X, r.Y, r.Width, r.Height);
                pb.Minimum = 0;
                pb.Maximum = 100;
                pb.Value = taken;
                pb.Name = d.Name;
                pb.ForeColor = Color.DarkBlue;
                listView1.Controls.Add(pb);
            }
            listView1.Refresh();
        }
        private void InitializeComponent()
        {
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            listView1 = new ListView();
            name = new ColumnHeader();
            total = new ColumnHeader();
            free = new ColumnHeader();
            progressbar = new ColumnHeader();
            usedtotal = new ColumnHeader();
            radioButton3 = new RadioButton();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            folderBrowserDialog1 = new FolderBrowserDialog();
            comboBox1 = new ComboBox();
            SuspendLayout();
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Checked = true;
            radioButton1.Location = new Point(12, 12);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(137, 19);
            radioButton1.TabIndex = 0;
            radioButton1.TabStop = true;
            radioButton1.Text = "All Local Drives";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            radioButton1.Click += radioButton1_CheckedChanged;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(12, 37);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(144, 19);
            radioButton2.TabIndex = 1;
            radioButton2.Text = "Individual Drives";
            radioButton2.UseVisualStyleBackColor = true;
            radioButton2.CheckedChanged += radioButton2_CheckedChanged;
            radioButton2.Click += radioButton2_CheckedChanged;
            // 
            // listView1
            // 
            listView1.Columns.AddRange(new ColumnHeader[] { name, total, free, progressbar, usedtotal });
            listView1.Enabled = false;
            listView1.Location = new Point(12, 62);
            listView1.Name = "listView1";
            listView1.Size = new Size(460, 104);
            listView1.TabIndex = 2;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            // 
            // name
            // 
            name.Text = "Name";
            // 
            // total
            // 
            total.Text = "Total";
            // 
            // free
            // 
            free.Text = "Free";
            // 
            // progressbar
            // 
            progressbar.Tag = "progressbar";
            progressbar.Text = "Used/Total";
            // 
            // usedtotal
            // 
            usedtotal.Text = "Used/Total";
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new Point(12, 172);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(81, 19);
            radioButton3.TabIndex = 3;
            radioButton3.Text = "A Folder";
            radioButton3.UseVisualStyleBackColor = true;
            radioButton3.CheckedChanged += radioButton3_CheckedChanged;
            radioButton3.Click += radioButton3_CheckedChanged;
            // 
            // button1
            // 
            button1.Location = new Point(397, 197);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 5;
            button1.Text = "...";
            button1.UseVisualStyleBackColor = true;
            button1.Click += browseButton_Click;
            // 
            // button2
            // 
            button2.Location = new Point(397, 226);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 6;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = true;
            button2.Click += cancelButton_Click;
            // 
            // button3
            // 
            button3.Location = new Point(316, 226);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 7;
            button3.Text = "OK";
            button3.UseVisualStyleBackColor = true;
            button3.Click += okButton_Click;
            // 
            // comboBox1
            // 
            comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox1.AutoCompleteSource = AutoCompleteSource.FileSystem;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(12, 197);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(379, 23);
            comboBox1.TabIndex = 8;
            comboBox1.TextUpdate += textBox1_TextChanged;
            // 
            // popupForm
            // 
            ClientSize = new Size(484, 261);
            Controls.Add(comboBox1);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(radioButton3);
            Controls.Add(listView1);
            Controls.Add(radioButton2);
            Controls.Add(radioButton1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            KeyPreview = true;
            MinimumSize = new Size(500, 300);
            Name = "popupForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select Disc or Folder";
            KeyDown += popupForm_KeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            // https://stackoverflow.com/questions/11624298/how-do-i-use-openfiledialog-to-select-a-folder
            result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog1.SelectedPath))
            {
                comboBox1.Text = folderBrowserDialog1.SelectedPath;
                comboBox1.ForeColor = Color.Black;
                comboBox1.Refresh();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e) { this.Close(); }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (result == DialogResult.OK || radioButton1.Checked || radioButton2.Checked)
            {
                if (radioButton1.Checked)
                {
                    DriveInfo[] allDrives = DriveInfo.GetDrives();
                    foreach (DriveInfo d in allDrives) list.Add(d.Name);
                }
                else if (radioButton2.Checked) { foreach (var item in listView1.Items) list.Add(((ListViewItem)item).Text); }
                else { list.Add(comboBox1.Text); }
                this.Close();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            listView1.Enabled = false;
            comboBox1.Enabled = false;
            button1.Enabled = false;
            comboBox1.Refresh();
            listView1.Refresh();
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            listView1.Enabled = true;
            comboBox1.Enabled = false;
            button1.Enabled = false;
            comboBox1.Refresh();
            listView1.Refresh();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            listView1.Enabled = false;
            comboBox1.Enabled = true;
            button1.Enabled = true;
            comboBox1.Refresh();
            listView1.Refresh();
        }

        private void popupForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.A) { radioButton1.PerformClick(); }
            else if (e.Alt && e.KeyCode == Keys.I) { radioButton2.PerformClick(); }
            else if (e.Alt && e.KeyCode == Keys.F) { radioButton3.PerformClick(); }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(comboBox1.Text)) { comboBox1.ForeColor = Color.Black; }
            else { comboBox1.ForeColor = Color.Red; }
            Refresh();
        }
    }
}