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
    partial class ResizeTagDlg : Form
    {
        private IccTag tag;

        public ResizeTagDlg(IccTag tag)
        {
            InitializeComponent();
            this.tag = tag;
        }

        private void ResizeTagDlg_Load(object sender, EventArgs e)
        {
            selTag.Text = newSig.Text = tag.TagSignature;
            curSize.Text = tag.DataArray.Length.ToString();
            newSize.Value = tag.DataArray.Length;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if ((int)newSize.Value < tag.DataArray.Length) {
                int difference = tag.DataArray.Length - (int)newSize.Value;
                string msg = String.Format("You are resizing a {0}-byte tag to {1} bytes. By doing this, {2} byte{3} of data will be truncated from the end.\r\nAre you sure you want to do this?",
                    tag.DataArray.Length, (int)newSize.Value, difference, (difference == 1) ? "" : "s");

                if (MessageBox.Show(msg, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No) {
                    return;
                }
            }

            // If the user answered Yes, or if truncation wasn't an issue in the first place:
            tag.TagSignature = newSig.Text;
            tag.Resize((int)newSize.Value);
            DialogResult = DialogResult.OK;
        }

        private void newSig_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = (newSig.Text.Length == 4);
        }
    }
}
