namespace RayMapEditor;

partial class ObjectDefinitionEditor
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.objectListView = new System.Windows.Forms.ListView();
            this.objectImageList = new System.Windows.Forms.ImageList(this.components);
            this.saveButton = new System.Windows.Forms.Button();
            this.clearFramesButton = new System.Windows.Forms.Button();
            this.removeFrameButton = new System.Windows.Forms.Button();
            this.addFrameButton = new System.Windows.Forms.Button();
            this.framesBox = new System.Windows.Forms.ListBox();
            this.image = new RayMapEditor.InterpolatedPictureBox();
            this.blockingCheckBox = new System.Windows.Forms.CheckBox();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.newObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newObjectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateImageTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.image)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.objectListView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.saveButton);
            this.splitContainer1.Panel2.Controls.Add(this.clearFramesButton);
            this.splitContainer1.Panel2.Controls.Add(this.removeFrameButton);
            this.splitContainer1.Panel2.Controls.Add(this.addFrameButton);
            this.splitContainer1.Panel2.Controls.Add(this.framesBox);
            this.splitContainer1.Panel2.Controls.Add(this.image);
            this.splitContainer1.Panel2.Controls.Add(this.blockingCheckBox);
            this.splitContainer1.Panel2.Controls.Add(this.typeComboBox);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.nameTextBox);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(473, 428);
            this.splitContainer1.SplitterDistance = 199;
            this.splitContainer1.TabIndex = 0;
            // 
            // objectListView
            // 
            this.objectListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView.LargeImageList = this.objectImageList;
            this.objectListView.Location = new System.Drawing.Point(0, 0);
            this.objectListView.MultiSelect = false;
            this.objectListView.Name = "objectListView";
            this.objectListView.Size = new System.Drawing.Size(199, 428);
            this.objectListView.TabIndex = 0;
            this.objectListView.UseCompatibleStateImageBehavior = false;
            this.objectListView.SelectedIndexChanged += new System.EventHandler(this.objectListView_SelectedIndexChanged);
            // 
            // objectImageList
            // 
            this.objectImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this.objectImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.objectImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(3, 119);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(94, 29);
            this.saveButton.TabIndex = 10;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // clearFramesButton
            // 
            this.clearFramesButton.Location = new System.Drawing.Point(168, 385);
            this.clearFramesButton.Name = "clearFramesButton";
            this.clearFramesButton.Size = new System.Drawing.Size(94, 29);
            this.clearFramesButton.TabIndex = 9;
            this.clearFramesButton.Text = "Clear";
            this.clearFramesButton.UseVisualStyleBackColor = true;
            // 
            // removeFrameButton
            // 
            this.removeFrameButton.Location = new System.Drawing.Point(168, 350);
            this.removeFrameButton.Name = "removeFrameButton";
            this.removeFrameButton.Size = new System.Drawing.Size(94, 29);
            this.removeFrameButton.TabIndex = 8;
            this.removeFrameButton.Text = "Remove";
            this.removeFrameButton.UseVisualStyleBackColor = true;
            // 
            // addFrameButton
            // 
            this.addFrameButton.Location = new System.Drawing.Point(168, 315);
            this.addFrameButton.Name = "addFrameButton";
            this.addFrameButton.Size = new System.Drawing.Size(94, 29);
            this.addFrameButton.TabIndex = 7;
            this.addFrameButton.Text = "Add";
            this.addFrameButton.UseVisualStyleBackColor = true;
            this.addFrameButton.Click += new System.EventHandler(this.addFrameButton_Click);
            // 
            // framesBox
            // 
            this.framesBox.FormattingEnabled = true;
            this.framesBox.ItemHeight = 20;
            this.framesBox.Location = new System.Drawing.Point(2, 315);
            this.framesBox.Name = "framesBox";
            this.framesBox.Size = new System.Drawing.Size(160, 104);
            this.framesBox.TabIndex = 6;
            this.framesBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.framesBox_MouseDoubleClick);
            // 
            // image
            // 
            this.image.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.image.Location = new System.Drawing.Point(2, 149);
            this.image.Name = "image";
            this.image.Size = new System.Drawing.Size(160, 160);
            this.image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.image.TabIndex = 5;
            this.image.TabStop = false;
            // 
            // blockingCheckBox
            // 
            this.blockingCheckBox.AutoSize = true;
            this.blockingCheckBox.Location = new System.Drawing.Point(174, 85);
            this.blockingCheckBox.Name = "blockingCheckBox";
            this.blockingCheckBox.Size = new System.Drawing.Size(88, 24);
            this.blockingCheckBox.TabIndex = 4;
            this.blockingCheckBox.Text = "Blocking";
            this.blockingCheckBox.UseVisualStyleBackColor = true;
            // 
            // typeComboBox
            // 
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Location = new System.Drawing.Point(1, 85);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(167, 28);
            this.typeComboBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Type";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(2, 32);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(260, 27);
            this.nameTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newObjectToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(473, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // newObjectToolStripMenuItem
            // 
            this.newObjectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newObjectToolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.deleteToolStripMenuItem});
            this.newObjectToolStripMenuItem.Name = "newObjectToolStripMenuItem";
            this.newObjectToolStripMenuItem.Size = new System.Drawing.Size(46, 24);
            this.newObjectToolStripMenuItem.Text = "File";
            // 
            // newObjectToolStripMenuItem1
            // 
            this.newObjectToolStripMenuItem1.Name = "newObjectToolStripMenuItem1";
            this.newObjectToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newObjectToolStripMenuItem1.Size = new System.Drawing.Size(223, 26);
            this.newObjectToolStripMenuItem1.Text = "New Object";
            this.newObjectToolStripMenuItem1.Click += new System.EventHandler(this.newObjectToolStripMenuItem1_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(223, 26);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(223, 26);
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // updateImageTimer
            // 
            this.updateImageTimer.Enabled = true;
            this.updateImageTimer.Interval = 250;
            this.updateImageTimer.Tick += new System.EventHandler(this.updateImageTimer_Tick);
            // 
            // ObjectDefinitionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 456);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ObjectDefinitionEditor";
            this.Text = "ObjectDefinitionEditor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.image)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private SplitContainer splitContainer1;
    private ListView objectListView;
    private ImageList objectImageList;
    private Button clearFramesButton;
    private Button removeFrameButton;
    private Button addFrameButton;
    private ListBox framesBox;
    private InterpolatedPictureBox image;
    private CheckBox blockingCheckBox;
    private ComboBox typeComboBox;
    private Label label2;
    private TextBox nameTextBox;
    private Label label1;
    private Button saveButton;
    private MenuStrip menuStrip1;
    private ToolStripMenuItem newObjectToolStripMenuItem;
    private ToolStripMenuItem newObjectToolStripMenuItem1;
    private ToolStripMenuItem saveToolStripMenuItem;
    private System.Windows.Forms.Timer updateImageTimer;
    private ToolStripMenuItem deleteToolStripMenuItem;
}