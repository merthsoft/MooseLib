using System.Text.Json;

namespace RayMapEditor;
public partial class ObjectDefinitionEditor : Form
{
    Definitions Definitions;
    List<ObjectDefinition> ObjectDefinitions;
    private readonly Bitmap[] Images;
    Dictionary<ObjectType, ListViewGroup> ListViewGroups = [];

    ObjectDefinition? SelectedDefinition = null;
    public int FrameIndex = 0;

    public ObjectDefinitionEditor(Definitions definitions, Bitmap[] images)
    {
        InitializeComponent();
        Definitions = definitions;
        ObjectDefinitions = definitions.Objects;
        Images = images;
        objectImageList.Images.AddRange(images);

        foreach (var item in Enum.GetValues<ObjectType>())
        {
            ListViewGroups[item] = new(item.ToString());
            objectListView.Groups.Add(ListViewGroups[item]);
            typeComboBox.Items.Add(item);
        }

        RebuildObjects();
    }

    private void RebuildObjects()
    {
        objectListView.Items.Clear();
        foreach (var o in ObjectDefinitions.OrderBy(o => o.Name))
            objectListView.Items.Add(
                new ListViewItem(o.ToString(), o.FirstFrameIndex, ListViewGroups[o.Type]) { Tag = o });
    }

    private void newObjectToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        NewObject();
    }

    private void NewObject()
    {
        var frame = new Frame();
        var fe = new FrameEditor(frame, Images);
        if (fe.ShowDialog() != DialogResult.OK)
            return;

        var newDef = new ObjectDefinition
        {
            Name = "new object",
            Blocking = false,
            Type = ObjectType.Static,
            Frames = [frame]
        };
        ObjectDefinitions.Add(newDef);
        RebuildObjects();
        foreach (var item in objectListView.Items)
            if (item is ListViewItem li && li.Tag == newDef)
                li.Selected = true;
    }

    private void objectListView_SelectedIndexChanged(object sender, EventArgs e)
    {
        FrameIndex = 0;
        SelectedDefinition = null;

        nameTextBox.Text = null;
        typeComboBox.SelectedItem = null;
        blockingCheckBox.Checked = false;
        framesBox.Items.Clear();
        
        if (objectListView.SelectedItems.Count == 0)
            return;

        SelectedDefinition = (objectListView.SelectedItems[0].Tag as ObjectDefinition)!;
        nameTextBox.Text = SelectedDefinition.Name;
        typeComboBox.SelectedItem = SelectedDefinition.Type;
        blockingCheckBox.Checked = SelectedDefinition.Blocking;

        foreach (var frame in SelectedDefinition.Frames)
        {
            framesBox.Items.Add(frame);
        }
    }

    private void updateImageTimer_Tick(object sender, EventArgs e)
    {
        if (SelectedDefinition == null)
        {
            image.Image = null;
            return;
        }

        image.Image = objectImageList.Images[SelectedDefinition.Frames[FrameIndex].Index];
        FrameIndex++;
        if (FrameIndex >= SelectedDefinition.Frames.Count)
            FrameIndex = 0;
    }

    private void framesBox_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        var frame = (framesBox.SelectedItem as Frame)!;
        var fe = new FrameEditor(frame, Images);
        fe.ShowDialog();
    }

    private void saveButton_Click(object sender, EventArgs e)
    {
        SaveDefinition();
    }

    private void SaveDefinition()
    {
        if (SelectedDefinition == null)
            return;
        SelectedDefinition.Name = nameTextBox.Text;
        SelectedDefinition.Type = (ObjectType)typeComboBox.SelectedItem;
        SelectedDefinition.Blocking = blockingCheckBox.Checked;

        RebuildObjects();
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        SaveDefinition();
        File.WriteAllText("Definitions.json", JsonSerializer.Serialize(Definitions));
    }

    private void addFrameButton_Click(object sender, EventArgs e)
    {
        if (SelectedDefinition == null)
            return;

        var frame = new Frame();
        var fe = new FrameEditor(frame, Images);
        if (fe.ShowDialog() != DialogResult.OK)
            return;
        SelectedDefinition.Frames.Add(frame);

        framesBox.Items.Clear();
        foreach (var f in SelectedDefinition.Frames)
            framesBox.Items.Add(f);
    }
}
