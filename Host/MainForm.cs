using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimplyHost
{
    public partial class MainForm : Form
    {
        private readonly HostController _hostController;

        public MainForm()
        {
            InitializeComponent();

            _hostController = new HostController(this);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _hostController.Start();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _hostController.Finish();
        }

        public Size GetSizeFromClientSize(Size clientSize)
        {
            return SizeFromClientSize(clientSize);
        }
        
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			_hostController.KeyDown(e);
		}

		private void MainForm_KeyUp(object sender, KeyEventArgs e)
		{
			_hostController.KeyUp(e);
		}

        private Color? _bypassOffColor;
        private bool _bypass;

        private void bypassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_bypassOffColor.HasValue)
                _bypassOffColor = bypassToolStripMenuItem.BackColor;

            _bypass = !_bypass;

            bypassToolStripMenuItem.BackColor = _bypass ? Color.Salmon : _bypassOffColor.Value;

            _hostController.SetBypass(_bypass);
        }

		private void printParametersToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_hostController.CopyParameters();
		}
	}
}
