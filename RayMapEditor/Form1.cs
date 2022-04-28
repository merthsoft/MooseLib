using System.Text.Json;

namespace RayMapEditor;

public partial class Form1 : Form
{
    Definitions Definitions;
    List<Bitmap> Objects;

    Dictionary<ObjectType, ListViewGroup> ObjectListViewGroups = new();

    int NumWalls = 0;

    public Form1()
    {
        InitializeComponent();

        if (File.Exists("Definitions.json"))
            Definitions = JsonSerializer.Deserialize<Definitions>(File.ReadAllText("Definitions.json")) ?? new();
        else
            Definitions = new();

        Objects = LoadImageList("Objects");
        objectImageList.Images.AddRange(Objects.ToArray());

        foreach (var item in Enum.GetValues<ObjectType>())
        {
            ObjectListViewGroups[item] = new(item.ToString());
            objectListView.Groups.Add(ObjectListViewGroups[item]);
        }

        RebuildObjects();
    }

    private void RebuildObjects()
    {
        objectListView.Items.Clear();
        foreach (var o in Definitions.Objects.OrderBy(o => o.Index))
            objectListView.Items.Add(
                new ListViewItem(o.ToString(), o.Frames[0].Index, ObjectListViewGroups[o.Type]));
    }

    private List<Bitmap> LoadImageList(string assetName)
    {
        using var image = (Bitmap.FromFile($"{assetName}.png") as Bitmap)!;
        List<Bitmap> ret = new();
        for (var y = 0; y < image.Height; y += 16)
            for (var x = 0; x < image.Width; x += 16)
            {
                var tile = new Bitmap(16, 16);
                for (var i = 0; i < 16; i++)
                    for (var j = 0; j < 16; j++)
                        tile.SetPixel(i, j, image.GetPixel(i + x, j + y));

                ret.Add(tile);
            }

        return ret;
    }

    private void editObjectDefinitionsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var ode = new ObjectDefinitionEditor(Definitions, Objects);
        ode.ShowDialog();
        RebuildObjects();
    }
}
