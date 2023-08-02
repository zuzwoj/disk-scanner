using System.Collections.Immutable;
using System.Data;
using System.Xml.Linq;

namespace Disk_Space_Analyzer
{
    public partial class Form1 : Form
    {
        public string ReturnValue { get; set; }
        public List<string> Displayed = new List<string>();
        public static readonly string[] units = { "bytes", "KB", "MB", "GB", "TB", "PB" };
        public Form1()
        {
            InitializeComponent();
            this.treeView1.Nodes.Clear();
            ReturnValue = "";
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in allDrives)
            {
                string name = drive.Name;
                DirectoryInfo di = new DirectoryInfo(name);
                TreeNode tds = treeView1.Nodes.Add(di.Name);
                tds.Tag = di.FullName;
            }
        }

        private void Form1_Load(object sender, EventArgs e) { }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                var formPopup = new popupForm();
                formPopup.ShowDialog(this);
            }
            else if (e.Control && e.KeyCode == Keys.T) { backgroundWorker1.CancelAsync(); }
            else if (e.Alt && e.KeyCode == Keys.F4) { this.Close(); }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) { this.Close(); }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var formPopup = new popupForm())
            {
                formPopup.Text = "Select Disc or Folder";
                var result = formPopup.ShowDialog();
                if (formPopup.list.Count > 0)
                {
                    treeView1.Nodes.Clear();
                    foreach (string dir in formPopup.list)
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        TreeNode tds = treeView1.Nodes.Add(di.Name);
                        tds.Tag = di.FullName;
                    }
                    treeView1.Refresh();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var formPopup = new popupForm())
            {
                formPopup.Text = "Select Disc or Folder";
                var result = formPopup.ShowDialog();
                if (formPopup.list.Count > 0)
                {
                    treeView1.Nodes.Clear();
                    foreach (string dir in formPopup.list)
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        TreeNode tds = treeView1.Nodes.Add(di.Name);
                        tds.Tag = di.FullName;
                    }
                    treeView1.Refresh();
                }
                toolStripProgressBar1.Value = 0;
                statusStrip1.Refresh();
                if (backgroundWorker1.IsBusy) backgroundWorker1.CancelAsync();
                backgroundWorker1.RunWorkerAsync(argument: treeView1.Nodes[0].Tag.ToString());
            }
        }

        void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            DirectoryInfo nodeDirInfo;
            string? id = "<files>";
            if (e.Node.Tag != null)
            {
                id = e.Node.Tag.ToString();
                nodeDirInfo = new DirectoryInfo(id!);
            }
            else nodeDirInfo = new DirectoryInfo(e.Node.Parent.Tag.ToString()!);
            TreeNode? item = null;
            int subdirs = 0, files = 0;
            try
            {
                foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
                {
                    if (!Displayed.Contains(id!) && id != "<files>")
                    {
                        item = new TreeNode(dir.Name);
                        item.Tag = dir.FullName;
                        newSelected.Nodes.Add(item);
                    }
                    ++subdirs;
                }
            }
            catch { }
            try
            {
                if (!Displayed.Contains(id!) && id != "<files>" && nodeDirInfo.GetFiles().Length > 2)
                {
                    item = new TreeNode("<files>");
                    newSelected.Nodes.Add(item);
                    newSelected = item;
                }
                foreach (FileInfo file in nodeDirInfo.GetFiles())
                {
                    if (!Displayed.Contains(id!) && id != "<files>")
                    {
                        item = new TreeNode(file.Name);
                        item.Tag = file.FullName;
                        newSelected.Nodes.Add(item);
                    }
                    ++files;
                }
            }
            catch { }
            if (!Displayed.Contains(id!) && id != "<files>") Displayed.Add(id!);
            long size;
            int un = 0;
            try { size = nodeDirInfo.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length); } catch { size = 0; }
            while (size >= 1024)
            {
                size /= 1024;
                ++un;
            }
            fullPath.Clear();
            this.size.Clear();
            items.Clear();
            this.files.Clear();
            this.subdirs.Clear();
            lastChange.Clear();
            if (id != "<files>") try
                {
                    fullPath.AppendText(nodeDirInfo.FullName);
                    this.size.AppendText(size + " " + units[un]);
                    items.AppendText((subdirs + files).ToString());
                    this.files.AppendText(files.ToString());
                    this.subdirs.AppendText(subdirs.ToString());
                    lastChange.AppendText(nodeDirInfo.LastWriteTimeUtc.ToString());
                }
                catch { }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Dictionary<string, long> workVolume = new Dictionary<string, long>();
            Dictionary<string, int> workQuantity = new Dictionary<string, int>();
            DirectoryInfo nodeDirInfo = new DirectoryInfo((string)e.Argument!);
            int count = nodeDirInfo.EnumerateDirectories().Count();
            int I = 0;
            foreach (var dir in nodeDirInfo.GetDirectories())
            {
                if (backgroundWorker1.CancellationPending)
                {
                    workVolume = new Dictionary<string, long>();
                    workQuantity = new Dictionary<string, int>();
                    backgroundWorker1.ReportProgress(0);
                    return;
                }
                try
                {
                    foreach (var file in dir.GetFiles())
                    {
                        string extension = file.Extension;
                        FileInfo finfo = new FileInfo(file.FullName);
                        if (workVolume.ContainsKey(extension))
                        {
                            workVolume[extension] += finfo.Length;
                            ++workQuantity[extension];
                        }
                        else
                        {
                            workVolume.Add(extension, finfo.Length);
                            workQuantity.Add(extension, 1);
                        }
                    }
                }
                catch { }
                traverseDirectory(dir, workVolume, workQuantity);
                ++I;
                int percentage = I * 100 / count;
                backgroundWorker1.ReportProgress(percentage);
            }
            e.Result = (workVolume, workQuantity);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            userControl12.mode = comboBox1.SelectedIndex;
            userControl12.Refresh();
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.ProgressBar.Value = e.ProgressPercentage;
            toolStripProgressBar1.ProgressBar.Refresh();
        }

        public void traverseDirectory(DirectoryInfo dir, Dictionary<string, long> typesVolume, Dictionary<string, int> typesQuantity)
        {
            try
            {
                foreach (var type in dir.GetDirectories()) traverseDirectory(type, typesVolume, typesQuantity);
            }
            catch { }
            try
            {
                foreach (var file in dir.GetFiles())
                {
                    string extension = file.Extension;
                    FileInfo finfo = new FileInfo(file.FullName);
                    if (typesVolume.ContainsKey(extension))
                    {
                        typesVolume[extension] += finfo.Length;
                        ++typesQuantity[extension];
                    }
                    else
                    {
                        typesVolume.Add(extension, finfo.Length);
                        typesQuantity.Add(extension, 1);
                    }
                }
            }
            catch { }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            (Dictionary<string, long>, Dictionary<string, int>)? workResult = e.Result as (Dictionary<string, long>, Dictionary<string, int>)?;
            int times = workResult?.Item1.Count ?? 0;
            if (times > 10)
            {
                userControl12.typesVolume = new (string, long)[10];
                userControl12.typesQuantity = new (string, int)[10];
                for (int i = 0; i < 9; ++i)
                {
                    var elem1 = workResult?.Item1.MaxBy(x => x.Value);
                    workResult?.Item1.Remove(elem1?.Key!);
                    userControl12.typesVolume[i] = (elem1?.Key!, elem1?.Value ?? 0);
                    var elem2 = workResult?.Item2.MaxBy(x => x.Value);
                    workResult?.Item2.Remove(elem2?.Key!);
                    userControl12.typesQuantity[i] = (elem2?.Key!, elem2?.Value ?? 0);
                }
                long otherVolume = 0;
                int otherQuantity = 0;
                foreach (var elem in workResult?.Item1!) otherVolume += elem.Value;
                foreach (var elem in workResult?.Item2!) otherQuantity += elem.Value;
                userControl12.typesVolume[9] = ("Others", otherVolume);
                userControl12.typesQuantity[9] = ("Others", otherQuantity);
            }
            else
            {
                userControl12.typesVolume = new (string, long)[times];
                userControl12.typesQuantity = new (string, int)[times];
                for (int i = 0; i < times; ++i)
                {
                    var elem1 = workResult?.Item1.MaxBy(x => x.Value);
                    workResult?.Item1.Remove(elem1?.Key!);
                    userControl12.typesVolume[i] = (elem1?.Key!, elem1?.Value ?? 0);
                    var elem2 = workResult?.Item2.MaxBy(x => x.Value);
                    workResult?.Item2.Remove(elem2?.Key!);
                    userControl12.typesQuantity[i] = (elem2?.Key!, elem2?.Value ?? 0);
                }
            }
            userControl12.Refresh();
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e) { if (backgroundWorker1.IsBusy) backgroundWorker1.CancelAsync(); }
        private void Form1_Resize(object sender, EventArgs e) { userControl12.Refresh(); }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var popup = new AboutWindow();
            popup.ShowDialog(this);
        }
    }
}