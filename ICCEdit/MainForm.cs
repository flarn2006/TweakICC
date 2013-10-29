using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Be.Windows.Forms;

namespace ICCEdit
{
    public partial class MainForm : Form
    {
        private IccProfile profile;
        private IccTag currentTag;
        private bool dirty = false;

        public MainForm()
        {
            InitializeComponent();
            profile = new IccProfile();
        }

        private class IccTagListViewItem : ListViewItem
        {
            private IccTag tag;
            private uint id;

            public IccTagListViewItem(IccTag tag, uint id)
                : base()
            {
                this.tag = tag;
                this.id = id;
                Text = tag.TagSignature;
                SubItems.Add(id.ToString());
                SubItems.Add(tag.DataArray.Length.ToString());
            }

            public IccTag IccTag
            {
                get { return tag; }
            }

            public uint TagID
            {
                get { return id; }
            }
        }

        private void UpdateListView()
        {
            tagList.Items.Clear();
            for (int i = 0; i < profile.Tags.Count; i++) {
                tagList.Items.Add(new IccTagListViewItem(profile.Tags[i], (uint)i));
            }
            deleteToolStripMenuItem.Enabled = (tagList.Items.Count > 0);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openDlg.ShowDialog() == DialogResult.OK) {
                FileStream fs = null;
                try {
                    fs = File.OpenRead(openDlg.FileName);
                    IccProfileReader pr = new IccProfileReader(fs);
                    try {
                        if (ConfirmDiscardChanges()) {
                            profile = pr.ReadProfile();
                            UpdateListView();
                            EditHeader();
                            dirty = false;
                        }
                    } catch (InvalidDataException) {
                        MessageBox.Show("The selected file is not an ICC profile.", "Error opening", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } catch (IOException ex) {
                    MessageBox.Show(ex.Message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } finally {
                    if (fs != null) fs.Close();
                }
            }
        }

        private void tagList_ItemActivate(object sender, EventArgs e)
        {
            currentTag = (tagList.SelectedItems[0] as IccTagListViewItem).IccTag;
            hexEdit.ByteProvider = new ArrayByteProvider(currentTag.DataArray);
            currentlyEditing.Text = String.Format("Tag with signature '{0}'", currentTag.TagSignature);
        }

        private void iCCProfileSpecificationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool tryAgain = true;
            while (tryAgain) {
                if (File.Exists(openPDFDlg.FileName)) {
                    System.Diagnostics.Process.Start(openPDFDlg.FileName);
                    tryAgain = false;
                } else {
                    openPDFDlg.InitialDirectory = Environment.CurrentDirectory;
                    tryAgain = (openPDFDlg.ShowDialog() == DialogResult.OK);
                }
            }
        }

        private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewTagDlg dlg = new NewTagDlg();
            if (dlg.ShowDialog() == DialogResult.OK) {
                IccTag newTag = new IccTag(dlg.TagSignature, dlg.DataLength);
                profile.Tags.Insert(SelectedIndex ?? profile.Tags.Count, newTag);
                UpdateListView();
                dirty = true;
            }
        }

        private IccTag SelectedTag
        {
            get
            {
                if (tagList.SelectedItems.Count == 0) return null;
                else return (tagList.SelectedItems[0] as IccTagListViewItem).IccTag;
            }
        }

        private int? SelectedIndex
        {
            get
            {
                if (tagList.SelectedIndices.Count == 0) return null;
                else return tagList.SelectedIndices[0];
            }
            set
            {
                tagList.SelectedIndices.Clear();
                if (value != null) {
                    if (value < 0) value = 0;
                    else if (value >= tagList.Items.Count) value = tagList.Items.Count - 1;
                    tagList.SelectedIndices.Add((int)value);
                }
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedTag != null) {
                string msg = String.Format("Really delete this tag containing {0} bytes of data? This cannot be undone!", SelectedTag.DataArray.Length);

                if (MessageBox.Show(msg, "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    profile.Tags.Remove(SelectedTag);
                    UpdateListView();
                }

                dirty = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void tagList_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool enable = (tagList.SelectedItems.Count > 0);
            deleteToolStripMenuItem.Enabled = enable;
            moveUpToolStripMenuItem.Enabled = enable;
            moveDownToolStripMenuItem.Enabled = enable;
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((SelectedIndex ?? 0) > 0) {
                int pos = (int)SelectedIndex - 1;
                profile.Tags.Remove(SelectedTag);
                profile.Tags.Insert(pos, SelectedTag);
                UpdateListView();
                SelectedIndex = pos;
                dirty = true;
            }
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((SelectedIndex ?? profile.Tags.Count) < profile.Tags.Count) {
                int pos = (int)SelectedIndex + 1;
                profile.Tags.Remove(SelectedTag);
                if (pos < profile.Tags.Count) {
                    profile.Tags.Insert(pos, SelectedTag);
                } else {
                    profile.Tags.Add(SelectedTag);
                }
                UpdateListView();
                SelectedIndex = pos;
                dirty = true;
            }
        }

        private void EditHeader()
        {
            currentTag = null;
            currentlyEditing.Text = "Profile Header";
            hexEdit.ByteProvider = new ArrayByteProvider(profile.Header.Bytes);
        }

        private void editHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditHeader();
        }

        private void NewProfile()
        {
            if (ConfirmDiscardChanges()) {
                profile = new IccProfile();
                UpdateListView();
                EditHeader();
                dirty = false;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            NewProfile();
        }

        private bool ConfirmDiscardChanges()
        {
            if (dirty) {
                DialogResult dr = MessageBox.Show("You have unsaved changes. Would you like to save first?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes) {
                    return SaveAs();
                } else if (dr == DialogResult.No) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return true;
            }
        }

        private bool SaveAs()
        {
            if (saveDlg.ShowDialog() == DialogResult.OK) {
                FileStream fs = null;
                try {
                    fs = File.OpenWrite(saveDlg.FileName);
                    IccProfileWriter pw = new IccProfileWriter(fs);
                    pw.WriteProfile(profile);
                    dirty = false;
                    return true;
                } catch (IOException ex) {
                    MessageBox.Show(ex.Message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                } finally {
                    if (fs != null) fs.Close();
                }
            } else {
                return false;
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProfile();
        }
    }
}
