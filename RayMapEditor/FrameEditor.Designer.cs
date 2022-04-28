namespace RayMapEditor;

partial class FrameEditor
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
            this.imageBox = new RayMapEditor.InterpolatedPictureBox();
            this.minTimeBox = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.maxTimeBox = new System.Windows.Forms.NumericUpDown();
            this.saveButton = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minTimeBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxTimeBox)).BeginInit();
            this.SuspendLayout();
            // 
            // imageBox
            // 
            this.imageBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.imageBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.imageBox.Location = new System.Drawing.Point(12, 12);
            this.imageBox.Name = "imageBox";
            this.imageBox.Size = new System.Drawing.Size(160, 160);
            this.imageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imageBox.TabIndex = 6;
            this.imageBox.TabStop = false;
            // 
            // minTimeBox
            // 
            this.minTimeBox.Location = new System.Drawing.Point(178, 37);
            this.minTimeBox.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.minTimeBox.Name = "minTimeBox";
            this.minTimeBox.Size = new System.Drawing.Size(94, 27);
            this.minTimeBox.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(178, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Min time";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(178, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 20);
            this.label2.TabIndex = 10;
            this.label2.Text = "Max time";
            // 
            // maxTimeBox
            // 
            this.maxTimeBox.Location = new System.Drawing.Point(178, 102);
            this.maxTimeBox.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.maxTimeBox.Name = "maxTimeBox";
            this.maxTimeBox.Size = new System.Drawing.Size(94, 27);
            this.maxTimeBox.TabIndex = 9;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(178, 143);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(94, 29);
            this.saveButton.TabIndex = 11;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // listView1
            // 
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.Location = new System.Drawing.Point(12, 178);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(289, 290);
            this.listView1.TabIndex = 12;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // FrameEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 480);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.maxTimeBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.minTimeBox);
            this.Controls.Add(this.imageBox);
            this.Name = "FrameEditor";
            this.Text = "FrameEditor";
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minTimeBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxTimeBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private InterpolatedPictureBox imageBox;
    private NumericUpDown minTimeBox;
    private Label label1;
    private Label label2;
    private NumericUpDown maxTimeBox;
    private Button saveButton;
    private ListView listView1;
    private ImageList imageList1;
}