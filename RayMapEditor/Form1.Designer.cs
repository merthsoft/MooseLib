namespace RayMapEditor;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        menuStrip1 = new MenuStrip();
        fileToolStripMenuItem = new ToolStripMenuItem();
        newToolStripMenuItem = new ToolStripMenuItem();
        openToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator = new ToolStripSeparator();
        saveToolStripMenuItem = new ToolStripMenuItem();
        saveAsToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator1 = new ToolStripSeparator();
        exitToolStripMenuItem = new ToolStripMenuItem();
        editToolStripMenuItem = new ToolStripMenuItem();
        undoToolStripMenuItem = new ToolStripMenuItem();
        redoToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator3 = new ToolStripSeparator();
        cutToolStripMenuItem = new ToolStripMenuItem();
        copyToolStripMenuItem = new ToolStripMenuItem();
        pasteToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator4 = new ToolStripSeparator();
        selectAllToolStripMenuItem = new ToolStripMenuItem();
        toolsToolStripMenuItem = new ToolStripMenuItem();
        editObjectDefinitionsToolStripMenuItem = new ToolStripMenuItem();
        helpToolStripMenuItem = new ToolStripMenuItem();
        contentsToolStripMenuItem = new ToolStripMenuItem();
        indexToolStripMenuItem = new ToolStripMenuItem();
        searchToolStripMenuItem = new ToolStripMenuItem();
        toolStripSeparator5 = new ToolStripSeparator();
        aboutToolStripMenuItem = new ToolStripMenuItem();
        statusStrip1 = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();
        splitContainer1 = new SplitContainer();
        splitContainer2 = new SplitContainer();
        tabControl3 = new TabControl();
        tabPage4 = new TabPage();
        wallListView = new ListView();
        wallsImageList = new ImageList(components);
        tabPage5 = new TabPage();
        doorsListView = new ListView();
        doorsImageList = new ImageList(components);
        tabControl1 = new TabControl();
        tabPage1 = new TabPage();
        objectListView = new ListView();
        objectImageList = new ImageList(components);
        tabPage2 = new TabPage();
        actorsListView = new ListView();
        actorsImageList = new ImageList(components);
        tabPage3 = new TabPage();
        specialListView = new ListView();
        specialImageList = new ImageList(components);
        mapPictureBox = new InterpolatedPictureBox();
        panel1 = new Panel();
        label4 = new Label();
        currentPicture = new InterpolatedPictureBox();
        numericUpDown1 = new NumericUpDown();
        label3 = new Label();
        label1 = new Label();
        heightBox = new NumericUpDown();
        widthBox = new NumericUpDown();
        label2 = new Label();
        toolStrip1 = new ToolStrip();
        undoButton = new ToolStripButton();
        redoButton = new ToolStripButton();
        toolStripSeparator6 = new ToolStripSeparator();
        menuStrip1.SuspendLayout();
        statusStrip1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
        splitContainer1.Panel1.SuspendLayout();
        splitContainer1.Panel2.SuspendLayout();
        splitContainer1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
        splitContainer2.Panel1.SuspendLayout();
        splitContainer2.Panel2.SuspendLayout();
        splitContainer2.SuspendLayout();
        tabControl3.SuspendLayout();
        tabPage4.SuspendLayout();
        tabPage5.SuspendLayout();
        tabControl1.SuspendLayout();
        tabPage1.SuspendLayout();
        tabPage2.SuspendLayout();
        tabPage3.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)mapPictureBox).BeginInit();
        panel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)currentPicture).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)heightBox).BeginInit();
        ((System.ComponentModel.ISupportInitialize)widthBox).BeginInit();
        toolStrip1.SuspendLayout();
        SuspendLayout();
        // 
        // menuStrip1
        // 
        menuStrip1.ImageScalingSize = new Size(20, 20);
        menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, toolsToolStripMenuItem, helpToolStripMenuItem });
        menuStrip1.Location = new Point(0, 0);
        menuStrip1.Name = "menuStrip1";
        menuStrip1.Size = new Size(800, 28);
        menuStrip1.TabIndex = 0;
        menuStrip1.Text = "menuStrip1";
        // 
        // fileToolStripMenuItem
        // 
        fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, toolStripSeparator, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
        fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        fileToolStripMenuItem.Size = new Size(46, 24);
        fileToolStripMenuItem.Text = "&File";
        // 
        // newToolStripMenuItem
        // 
        newToolStripMenuItem.Image = (Image)resources.GetObject("newToolStripMenuItem.Image");
        newToolStripMenuItem.ImageTransparentColor = Color.Magenta;
        newToolStripMenuItem.Name = "newToolStripMenuItem";
        newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
        newToolStripMenuItem.Size = new Size(224, 26);
        newToolStripMenuItem.Text = "&New";
        newToolStripMenuItem.Click += newToolStripMenuItem_Click;
        // 
        // openToolStripMenuItem
        // 
        openToolStripMenuItem.Image = (Image)resources.GetObject("openToolStripMenuItem.Image");
        openToolStripMenuItem.ImageTransparentColor = Color.Magenta;
        openToolStripMenuItem.Name = "openToolStripMenuItem";
        openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
        openToolStripMenuItem.Size = new Size(224, 26);
        openToolStripMenuItem.Text = "&Open";
        openToolStripMenuItem.Click += openToolStripMenuItem_Click;
        // 
        // toolStripSeparator
        // 
        toolStripSeparator.Name = "toolStripSeparator";
        toolStripSeparator.Size = new Size(221, 6);
        // 
        // saveToolStripMenuItem
        // 
        saveToolStripMenuItem.Image = (Image)resources.GetObject("saveToolStripMenuItem.Image");
        saveToolStripMenuItem.ImageTransparentColor = Color.Magenta;
        saveToolStripMenuItem.Name = "saveToolStripMenuItem";
        saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
        saveToolStripMenuItem.Size = new Size(224, 26);
        saveToolStripMenuItem.Text = "&Save";
        saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
        // 
        // saveAsToolStripMenuItem
        // 
        saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
        saveAsToolStripMenuItem.Size = new Size(224, 26);
        saveAsToolStripMenuItem.Text = "Save &As";
        saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
        // 
        // toolStripSeparator1
        // 
        toolStripSeparator1.Name = "toolStripSeparator1";
        toolStripSeparator1.Size = new Size(221, 6);
        // 
        // exitToolStripMenuItem
        // 
        exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        exitToolStripMenuItem.Size = new Size(224, 26);
        exitToolStripMenuItem.Text = "E&xit";
        // 
        // editToolStripMenuItem
        // 
        editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { undoToolStripMenuItem, redoToolStripMenuItem, toolStripSeparator3, cutToolStripMenuItem, copyToolStripMenuItem, pasteToolStripMenuItem, toolStripSeparator4, selectAllToolStripMenuItem });
        editToolStripMenuItem.Name = "editToolStripMenuItem";
        editToolStripMenuItem.Size = new Size(49, 24);
        editToolStripMenuItem.Text = "&Edit";
        // 
        // undoToolStripMenuItem
        // 
        undoToolStripMenuItem.Enabled = false;
        undoToolStripMenuItem.Name = "undoToolStripMenuItem";
        undoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
        undoToolStripMenuItem.Size = new Size(179, 26);
        undoToolStripMenuItem.Text = "&Undo";
        undoToolStripMenuItem.Click += undoToolStripMenuItem_Click;
        // 
        // redoToolStripMenuItem
        // 
        redoToolStripMenuItem.Enabled = false;
        redoToolStripMenuItem.Name = "redoToolStripMenuItem";
        redoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
        redoToolStripMenuItem.Size = new Size(179, 26);
        redoToolStripMenuItem.Text = "&Redo";
        redoToolStripMenuItem.Click += redoToolStripMenuItem_Click;
        // 
        // toolStripSeparator3
        // 
        toolStripSeparator3.Name = "toolStripSeparator3";
        toolStripSeparator3.Size = new Size(176, 6);
        // 
        // cutToolStripMenuItem
        // 
        cutToolStripMenuItem.Image = (Image)resources.GetObject("cutToolStripMenuItem.Image");
        cutToolStripMenuItem.ImageTransparentColor = Color.Magenta;
        cutToolStripMenuItem.Name = "cutToolStripMenuItem";
        cutToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.X;
        cutToolStripMenuItem.Size = new Size(179, 26);
        cutToolStripMenuItem.Text = "Cu&t";
        // 
        // copyToolStripMenuItem
        // 
        copyToolStripMenuItem.Image = (Image)resources.GetObject("copyToolStripMenuItem.Image");
        copyToolStripMenuItem.ImageTransparentColor = Color.Magenta;
        copyToolStripMenuItem.Name = "copyToolStripMenuItem";
        copyToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
        copyToolStripMenuItem.Size = new Size(179, 26);
        copyToolStripMenuItem.Text = "&Copy";
        // 
        // pasteToolStripMenuItem
        // 
        pasteToolStripMenuItem.Image = (Image)resources.GetObject("pasteToolStripMenuItem.Image");
        pasteToolStripMenuItem.ImageTransparentColor = Color.Magenta;
        pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
        pasteToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
        pasteToolStripMenuItem.Size = new Size(179, 26);
        pasteToolStripMenuItem.Text = "&Paste";
        // 
        // toolStripSeparator4
        // 
        toolStripSeparator4.Name = "toolStripSeparator4";
        toolStripSeparator4.Size = new Size(176, 6);
        // 
        // selectAllToolStripMenuItem
        // 
        selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
        selectAllToolStripMenuItem.Size = new Size(179, 26);
        selectAllToolStripMenuItem.Text = "Select &All";
        // 
        // toolsToolStripMenuItem
        // 
        toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { editObjectDefinitionsToolStripMenuItem });
        toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
        toolsToolStripMenuItem.Size = new Size(58, 24);
        toolsToolStripMenuItem.Text = "&Tools";
        // 
        // editObjectDefinitionsToolStripMenuItem
        // 
        editObjectDefinitionsToolStripMenuItem.Name = "editObjectDefinitionsToolStripMenuItem";
        editObjectDefinitionsToolStripMenuItem.Size = new Size(242, 26);
        editObjectDefinitionsToolStripMenuItem.Text = "Edit Object Definitions";
        editObjectDefinitionsToolStripMenuItem.Click += editObjectDefinitionsToolStripMenuItem_Click;
        // 
        // helpToolStripMenuItem
        // 
        helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { contentsToolStripMenuItem, indexToolStripMenuItem, searchToolStripMenuItem, toolStripSeparator5, aboutToolStripMenuItem });
        helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        helpToolStripMenuItem.Size = new Size(55, 24);
        helpToolStripMenuItem.Text = "&Help";
        // 
        // contentsToolStripMenuItem
        // 
        contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
        contentsToolStripMenuItem.Size = new Size(150, 26);
        contentsToolStripMenuItem.Text = "&Contents";
        // 
        // indexToolStripMenuItem
        // 
        indexToolStripMenuItem.Name = "indexToolStripMenuItem";
        indexToolStripMenuItem.Size = new Size(150, 26);
        indexToolStripMenuItem.Text = "&Index";
        // 
        // searchToolStripMenuItem
        // 
        searchToolStripMenuItem.Name = "searchToolStripMenuItem";
        searchToolStripMenuItem.Size = new Size(150, 26);
        searchToolStripMenuItem.Text = "&Search";
        // 
        // toolStripSeparator5
        // 
        toolStripSeparator5.Name = "toolStripSeparator5";
        toolStripSeparator5.Size = new Size(147, 6);
        // 
        // aboutToolStripMenuItem
        // 
        aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
        aboutToolStripMenuItem.Size = new Size(150, 26);
        aboutToolStripMenuItem.Text = "&About...";
        // 
        // statusStrip1
        // 
        statusStrip1.ImageScalingSize = new Size(20, 20);
        statusStrip1.Items.AddRange(new ToolStripItem[] { statusLabel });
        statusStrip1.Location = new Point(0, 702);
        statusStrip1.Name = "statusStrip1";
        statusStrip1.Size = new Size(800, 26);
        statusStrip1.TabIndex = 1;
        statusStrip1.Text = "statusStrip1";
        // 
        // statusLabel
        // 
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(50, 20);
        statusLabel.Text = "Ready";
        // 
        // splitContainer1
        // 
        splitContainer1.Dock = DockStyle.Fill;
        splitContainer1.Location = new Point(0, 55);
        splitContainer1.Name = "splitContainer1";
        // 
        // splitContainer1.Panel1
        // 
        splitContainer1.Panel1.Controls.Add(splitContainer2);
        // 
        // splitContainer1.Panel2
        // 
        splitContainer1.Panel2.AutoScroll = true;
        splitContainer1.Panel2.Controls.Add(mapPictureBox);
        splitContainer1.Panel2.Controls.Add(panel1);
        splitContainer1.Size = new Size(800, 647);
        splitContainer1.SplitterDistance = 266;
        splitContainer1.TabIndex = 2;
        // 
        // splitContainer2
        // 
        splitContainer2.Dock = DockStyle.Fill;
        splitContainer2.Location = new Point(0, 0);
        splitContainer2.Name = "splitContainer2";
        splitContainer2.Orientation = Orientation.Horizontal;
        // 
        // splitContainer2.Panel1
        // 
        splitContainer2.Panel1.Controls.Add(tabControl3);
        // 
        // splitContainer2.Panel2
        // 
        splitContainer2.Panel2.Controls.Add(tabControl1);
        splitContainer2.Size = new Size(266, 647);
        splitContainer2.SplitterDistance = 258;
        splitContainer2.TabIndex = 2;
        // 
        // tabControl3
        // 
        tabControl3.Controls.Add(tabPage4);
        tabControl3.Controls.Add(tabPage5);
        tabControl3.Dock = DockStyle.Fill;
        tabControl3.Location = new Point(0, 0);
        tabControl3.Name = "tabControl3";
        tabControl3.SelectedIndex = 0;
        tabControl3.Size = new Size(266, 258);
        tabControl3.TabIndex = 1;
        // 
        // tabPage4
        // 
        tabPage4.Controls.Add(wallListView);
        tabPage4.Location = new Point(4, 29);
        tabPage4.Name = "tabPage4";
        tabPage4.Padding = new Padding(3);
        tabPage4.Size = new Size(258, 225);
        tabPage4.TabIndex = 0;
        tabPage4.Text = "Walls";
        tabPage4.UseVisualStyleBackColor = true;
        // 
        // wallListView
        // 
        wallListView.Dock = DockStyle.Fill;
        wallListView.LargeImageList = wallsImageList;
        wallListView.Location = new Point(3, 3);
        wallListView.MultiSelect = false;
        wallListView.Name = "wallListView";
        wallListView.Size = new Size(252, 219);
        wallListView.TabIndex = 0;
        wallListView.UseCompatibleStateImageBehavior = false;
        wallListView.SelectedIndexChanged += wallListView_SelectedIndexChanged;
        // 
        // wallsImageList
        // 
        wallsImageList.ColorDepth = ColorDepth.Depth24Bit;
        wallsImageList.ImageSize = new Size(32, 32);
        wallsImageList.TransparentColor = Color.Transparent;
        // 
        // tabPage5
        // 
        tabPage5.Controls.Add(doorsListView);
        tabPage5.Location = new Point(4, 29);
        tabPage5.Name = "tabPage5";
        tabPage5.Padding = new Padding(3);
        tabPage5.Size = new Size(258, 225);
        tabPage5.TabIndex = 1;
        tabPage5.Text = "Doors";
        tabPage5.UseVisualStyleBackColor = true;
        // 
        // doorsListView
        // 
        doorsListView.Dock = DockStyle.Fill;
        doorsListView.LargeImageList = doorsImageList;
        doorsListView.Location = new Point(3, 3);
        doorsListView.MultiSelect = false;
        doorsListView.Name = "doorsListView";
        doorsListView.Size = new Size(252, 219);
        doorsListView.TabIndex = 1;
        doorsListView.UseCompatibleStateImageBehavior = false;
        doorsListView.SelectedIndexChanged += doorsListView_SelectedIndexChanged;
        // 
        // doorsImageList
        // 
        doorsImageList.ColorDepth = ColorDepth.Depth24Bit;
        doorsImageList.ImageSize = new Size(32, 32);
        doorsImageList.TransparentColor = Color.Transparent;
        // 
        // tabControl1
        // 
        tabControl1.Controls.Add(tabPage1);
        tabControl1.Controls.Add(tabPage2);
        tabControl1.Controls.Add(tabPage3);
        tabControl1.Dock = DockStyle.Fill;
        tabControl1.Location = new Point(0, 0);
        tabControl1.Name = "tabControl1";
        tabControl1.SelectedIndex = 0;
        tabControl1.Size = new Size(266, 385);
        tabControl1.TabIndex = 2;
        // 
        // tabPage1
        // 
        tabPage1.Controls.Add(objectListView);
        tabPage1.Location = new Point(4, 29);
        tabPage1.Name = "tabPage1";
        tabPage1.Padding = new Padding(3);
        tabPage1.Size = new Size(258, 352);
        tabPage1.TabIndex = 0;
        tabPage1.Text = "Objects";
        tabPage1.UseVisualStyleBackColor = true;
        // 
        // objectListView
        // 
        objectListView.Dock = DockStyle.Fill;
        objectListView.LargeImageList = objectImageList;
        objectListView.Location = new Point(3, 3);
        objectListView.MultiSelect = false;
        objectListView.Name = "objectListView";
        objectListView.Size = new Size(252, 346);
        objectListView.TabIndex = 1;
        objectListView.UseCompatibleStateImageBehavior = false;
        objectListView.SelectedIndexChanged += objectListView_SelectedIndexChanged;
        // 
        // objectImageList
        // 
        objectImageList.ColorDepth = ColorDepth.Depth24Bit;
        objectImageList.ImageSize = new Size(32, 32);
        objectImageList.TransparentColor = Color.Transparent;
        // 
        // tabPage2
        // 
        tabPage2.Controls.Add(actorsListView);
        tabPage2.Location = new Point(4, 29);
        tabPage2.Name = "tabPage2";
        tabPage2.Padding = new Padding(3);
        tabPage2.Size = new Size(258, 352);
        tabPage2.TabIndex = 1;
        tabPage2.Text = "Actors";
        tabPage2.UseVisualStyleBackColor = true;
        // 
        // actorsListView
        // 
        actorsListView.Dock = DockStyle.Fill;
        actorsListView.LargeImageList = actorsImageList;
        actorsListView.Location = new Point(3, 3);
        actorsListView.MultiSelect = false;
        actorsListView.Name = "actorsListView";
        actorsListView.Size = new Size(252, 346);
        actorsListView.TabIndex = 1;
        actorsListView.UseCompatibleStateImageBehavior = false;
        actorsListView.SelectedIndexChanged += actorsListView_SelectedIndexChanged;
        // 
        // actorsImageList
        // 
        actorsImageList.ColorDepth = ColorDepth.Depth24Bit;
        actorsImageList.ImageSize = new Size(32, 32);
        actorsImageList.TransparentColor = Color.Transparent;
        // 
        // tabPage3
        // 
        tabPage3.Controls.Add(specialListView);
        tabPage3.Location = new Point(4, 29);
        tabPage3.Name = "tabPage3";
        tabPage3.Size = new Size(258, 352);
        tabPage3.TabIndex = 2;
        tabPage3.Text = "Special";
        tabPage3.UseVisualStyleBackColor = true;
        // 
        // specialListView
        // 
        specialListView.Dock = DockStyle.Fill;
        specialListView.LargeImageList = specialImageList;
        specialListView.Location = new Point(0, 0);
        specialListView.MultiSelect = false;
        specialListView.Name = "specialListView";
        specialListView.Size = new Size(258, 352);
        specialListView.TabIndex = 1;
        specialListView.UseCompatibleStateImageBehavior = false;
        specialListView.SelectedIndexChanged += specialListView_SelectedIndexChanged;
        // 
        // specialImageList
        // 
        specialImageList.ColorDepth = ColorDepth.Depth24Bit;
        specialImageList.ImageSize = new Size(32, 32);
        specialImageList.TransparentColor = Color.Transparent;
        // 
        // mapPicture
        // 
        mapPictureBox.BackColor = Color.Black;
        mapPictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        mapPictureBox.Location = new Point(3, 61);
        mapPictureBox.Name = "mapPicture";
        mapPictureBox.Size = new Size(256, 256);
        mapPictureBox.TabIndex = 1;
        mapPictureBox.TabStop = false;
        mapPictureBox.Paint += mapPicture_Paint;
        mapPictureBox.MouseDown += mapPicture_MouseDown;
        mapPictureBox.MouseMove += mapPicture_MouseMove;
        mapPictureBox.MouseUp += mapPicture_MouseUp;
        // 
        // panel1
        // 
        panel1.Controls.Add(label4);
        panel1.Controls.Add(currentPicture);
        panel1.Controls.Add(numericUpDown1);
        panel1.Controls.Add(label3);
        panel1.Controls.Add(label1);
        panel1.Controls.Add(heightBox);
        panel1.Controls.Add(widthBox);
        panel1.Controls.Add(label2);
        panel1.Dock = DockStyle.Top;
        panel1.Location = new Point(0, 0);
        panel1.Name = "panel1";
        panel1.Size = new Size(530, 55);
        panel1.TabIndex = 6;
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(197, 0);
        label4.Name = "label4";
        label4.Size = new Size(57, 20);
        label4.TabIndex = 9;
        label4.Text = "Current";
        // 
        // currentPicture
        // 
        currentPicture.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        currentPicture.Location = new Point(197, 20);
        currentPicture.Name = "currentPicture";
        currentPicture.Size = new Size(32, 32);
        currentPicture.SizeMode = PictureBoxSizeMode.StretchImage;
        currentPicture.TabIndex = 8;
        currentPicture.TabStop = false;
        // 
        // numericUpDown1
        // 
        numericUpDown1.DecimalPlaces = 2;
        numericUpDown1.Increment = new decimal(new int[] { 25, 0, 0, 131072 });
        numericUpDown1.Location = new Point(115, 23);
        numericUpDown1.Maximum = new decimal(new int[] { 4, 0, 0, 0 });
        numericUpDown1.Minimum = new decimal(new int[] { 25, 0, 0, 131072 });
        numericUpDown1.Name = "numericUpDown1";
        numericUpDown1.Size = new Size(76, 27);
        numericUpDown1.TabIndex = 7;
        numericUpDown1.Value = new decimal(new int[] { 1, 0, 0, 0 });
        numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(115, 0);
        label3.Name = "label3";
        label3.Size = new Size(49, 20);
        label3.TabIndex = 6;
        label3.Text = "Zoom";
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(3, 0);
        label1.Name = "label1";
        label1.Size = new Size(49, 20);
        label1.TabIndex = 2;
        label1.Text = "Width";
        // 
        // heightBox
        // 
        heightBox.Location = new Point(59, 23);
        heightBox.Maximum = new decimal(new int[] { 128, 0, 0, 0 });
        heightBox.Minimum = new decimal(new int[] { 16, 0, 0, 0 });
        heightBox.Name = "heightBox";
        heightBox.Size = new Size(50, 27);
        heightBox.TabIndex = 5;
        heightBox.Value = new decimal(new int[] { 48, 0, 0, 0 });
        // 
        // widthBox
        // 
        widthBox.Location = new Point(3, 23);
        widthBox.Maximum = new decimal(new int[] { 128, 0, 0, 0 });
        widthBox.Minimum = new decimal(new int[] { 16, 0, 0, 0 });
        widthBox.Name = "widthBox";
        widthBox.Size = new Size(50, 27);
        widthBox.TabIndex = 3;
        widthBox.Value = new decimal(new int[] { 48, 0, 0, 0 });
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(59, 0);
        label2.Name = "label2";
        label2.Size = new Size(54, 20);
        label2.TabIndex = 4;
        label2.Text = "Height";
        // 
        // toolStrip1
        // 
        toolStrip1.ImageScalingSize = new Size(20, 20);
        toolStrip1.Items.AddRange(new ToolStripItem[] { undoButton, redoButton, toolStripSeparator6 });
        toolStrip1.Location = new Point(0, 28);
        toolStrip1.Name = "toolStrip1";
        toolStrip1.Size = new Size(800, 27);
        toolStrip1.TabIndex = 0;
        toolStrip1.Text = "toolStrip1";
        // 
        // undoButton
        // 
        undoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        undoButton.Enabled = false;
        undoButton.Image = Resources.icon_undo;
        undoButton.ImageTransparentColor = Color.Magenta;
        undoButton.Name = "undoButton";
        undoButton.Size = new Size(29, 24);
        undoButton.Text = "Undo";
        undoButton.Click += undoButton_Click;
        // 
        // redoButton
        // 
        redoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
        redoButton.Enabled = false;
        redoButton.Image = Resources.icon_redo;
        redoButton.ImageTransparentColor = Color.Magenta;
        redoButton.Name = "redoButton";
        redoButton.Size = new Size(29, 24);
        redoButton.Text = "Redo";
        redoButton.Click += redoButton_Click;
        // 
        // toolStripSeparator6
        // 
        toolStripSeparator6.Name = "toolStripSeparator6";
        toolStripSeparator6.Size = new Size(6, 27);
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 728);
        Controls.Add(splitContainer1);
        Controls.Add(statusStrip1);
        Controls.Add(toolStrip1);
        Controls.Add(menuStrip1);
        MainMenuStrip = menuStrip1;
        Name = "Form1";
        Text = "Form1";
        menuStrip1.ResumeLayout(false);
        menuStrip1.PerformLayout();
        statusStrip1.ResumeLayout(false);
        statusStrip1.PerformLayout();
        splitContainer1.Panel1.ResumeLayout(false);
        splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
        splitContainer1.ResumeLayout(false);
        splitContainer2.Panel1.ResumeLayout(false);
        splitContainer2.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
        splitContainer2.ResumeLayout(false);
        tabControl3.ResumeLayout(false);
        tabPage4.ResumeLayout(false);
        tabPage5.ResumeLayout(false);
        tabControl1.ResumeLayout(false);
        tabPage1.ResumeLayout(false);
        tabPage2.ResumeLayout(false);
        tabPage3.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)mapPictureBox).EndInit();
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)currentPicture).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
        ((System.ComponentModel.ISupportInitialize)heightBox).EndInit();
        ((System.ComponentModel.ISupportInitialize)widthBox).EndInit();
        toolStrip1.ResumeLayout(false);
        toolStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip menuStrip1;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem newToolStripMenuItem;
    private ToolStripMenuItem openToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator;
    private ToolStripMenuItem saveToolStripMenuItem;
    private ToolStripMenuItem saveAsToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem editToolStripMenuItem;
    private ToolStripMenuItem undoToolStripMenuItem;
    private ToolStripMenuItem redoToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator3;
    private ToolStripMenuItem cutToolStripMenuItem;
    private ToolStripMenuItem copyToolStripMenuItem;
    private ToolStripMenuItem pasteToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator4;
    private ToolStripMenuItem selectAllToolStripMenuItem;
    private ToolStripMenuItem toolsToolStripMenuItem;
    private ToolStripMenuItem helpToolStripMenuItem;
    private ToolStripMenuItem contentsToolStripMenuItem;
    private ToolStripMenuItem indexToolStripMenuItem;
    private ToolStripMenuItem searchToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator5;
    private ToolStripMenuItem aboutToolStripMenuItem;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel statusLabel;
    private SplitContainer splitContainer1;
    private ToolStrip toolStrip1;
    private ToolStripButton undoButton;
    private ToolStripButton redoButton;
    private ToolStripSeparator toolStripSeparator6;
    private SplitContainer splitContainer2;
    private ImageList objectImageList;
    private ToolStripMenuItem editObjectDefinitionsToolStripMenuItem;
    private ListView wallListView;
    private ListView objectListView;
    private TabControl tabControl3;
    private TabPage tabPage4;
    private TabPage tabPage5;
    private ListView doorsListView;
    private TabControl tabControl1;
    private TabPage tabPage1;
    private TabPage tabPage2;
    private ListView actorsListView;
    private TabPage tabPage3;
    private ListView specialListView;
    private ImageList doorsImageList;
    private ImageList wallsImageList;
    private ImageList actorsImageList;
    private ImageList specialImageList;
    private InterpolatedPictureBox mapPictureBox;
    private Panel panel1;
    private Label label1;
    private NumericUpDown heightBox;
    private NumericUpDown widthBox;
    private Label label2;
    private NumericUpDown numericUpDown1;
    private Label label3;
    private Label label4;
    private InterpolatedPictureBox currentPicture;
}
