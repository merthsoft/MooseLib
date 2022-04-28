namespace RayMapEditor;
public partial class FrameEditor : Form
{
    Frame Frame;
    public FrameEditor(Frame frame, List<Bitmap> images)
    {
        InitializeComponent();
        
        Frame = frame;
        imageList1.Images.AddRange(images.ToArray());
        imageBox.Image = imageList1.Images[Frame.Index];

        minTimeBox.Value = frame.MinTime;
        maxTimeBox.Value = frame.MaxTime;

        for (int i = 0; i < images.Count; i++)
            listView1.Items.Add(i.ToString(), i);
    }

    private void listView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (listView1.SelectedItems.Count == 0)
        {
            imageBox.Image = null;
            return;
        }

        imageBox.Image = imageList1.Images[listView1.SelectedItems[0].ImageIndex];
    }

    private void saveButton_Click(object sender, EventArgs e)
    {
        Frame.Index = listView1.SelectedIndices[0];
        Frame.MinTime = (int)minTimeBox.Value;
        Frame.MaxTime = (int)maxTimeBox.Value;

        Close();
    }
}
