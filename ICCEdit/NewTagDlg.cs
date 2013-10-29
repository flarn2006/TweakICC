using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ICCEdit
{
    public partial class NewTagDlg : Form
    {
        public NewTagDlg()
        {
            InitializeComponent();
        }

        private void tagSig_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = (tagSig.Text.Length == 4);
        }

        public string TagSignature
        {
            get { return tagSig.Text; }
            set { tagSig.Text = value; }
        }

        public uint DataLength
        {
            get { return (uint)dataLength.Value; }
            set { dataLength.Value = value; }
        }
    }
}
