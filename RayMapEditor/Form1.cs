using System.Text.Json;

namespace RayMapEditor;

public partial class Form1 : Form
{
    Definitions Definitions;
    Bitmap[] Objects;

    Dictionary<ObjectType, ListViewGroup> ObjectListViewGroups = new();

    string? fileName = null;
    SaveMap currentMap = new SaveMap(48, 48);

    ToolStripButton? currentToolButton = null;
    Tool currentTool;

    float zoom => (float)numericUpDown1.Value;

    bool drawing;
    bool deleting;
    int? currentWallNumber;
    ItemType? currentItemType;
    string? currentItemName;

    IntVec2? previousMouse;
    List<IntVec2> phantomCells = new();
    IntVec2 startCell;

    List<SaveMap> UndoStack = new();
    int UndoPointer = -1;

    public Form1()
    {
        InitializeComponent();

        if (File.Exists("Definitions.json"))
            Definitions = JsonSerializer.Deserialize<Definitions>(File.ReadAllText("Definitions.json")) ?? new();
        else
            Definitions = new();

        Objects = LoadImageList("Objects");
        objectImageList.Images.AddRange(Objects);
        wallsImageList.Images.AddRange(LoadImageList("Walls"));
        doorsImageList.Images.AddRange(LoadImageList("Doors"));
        specialImageList.Images.AddRange(LoadImageList("Special"));

        foreach (var item in Enum.GetValues<ObjectType>())
        {
            ObjectListViewGroups[item] = new(item.ToString());
            objectListView.Groups.Add(ObjectListViewGroups[item]);
        }

        RebuildWalls();
        RebuildDoors();

        RebuildObjects();
        RebuildSpecial();
        RebuildActors();

        foreach (var tool in Enum.GetValues<Tool>())
        {
            var button = new ToolStripButton(tool.GetIcon());
            button.Click += (s, e) =>
            {
                if (currentToolButton != null)
                    currentToolButton.Checked = false;
                currentTool = tool;
                currentToolButton = s as ToolStripButton;
                currentToolButton!.Checked = true;
            };
            toolStrip1.Items.Add(button);

            if (tool == Tool.Pen)
            {
                currentToolButton = button;
                button.Checked = true;
                currentTool = tool;
            }
        }

        mapPicture.Size = new((int)(16 * currentMap.Width * zoom), (int)(16 * currentMap.Height * zoom));
        ResetHistory();
        PushHistory();
    }

    private void RebuildDoors()
    {
        doorsListView.Items.Clear();
        foreach (var doorDef in Definitions.Doors)
            doorsListView.Items.Add(doorDef.Name, doorDef.Index);
    }

    private void RebuildWalls()
    {
        wallListView.Items.Clear();
        for (var i = 0; i < wallsImageList.Images.Count; i++)
            wallListView.Items.Add($"Wall {i}", i);
    }

    private void RebuildSpecial()
    {
        foreach (var special in Definitions.Special)
            specialListView.Items.Add(special.Name, special.Index);
    }

    private void RebuildActors()
    {
        for (var i = 0; i < Definitions.Actors.Count; i++)
        {
            var actor = Definitions.Actors[i];
            var actorSlices = LoadImageList($"Actors/{actor.Name}");
            actorsImageList.Images.AddRange(new[] { actorSlices[2], actorSlices[6], actorSlices[4], actorSlices[0] });
            actorsListView.Items.Add($"{actor.Name} (W)", i * 4);
            actorsListView.Items.Add($"{actor.Name} (E)", i * 4 + 1);
            actorsListView.Items.Add($"{actor.Name} (N)", i * 4 + 2);
            actorsListView.Items.Add($"{actor.Name} (S)", i * 4 + 3);
        }
    }

    private void RebuildObjects()
    {
        objectListView.Items.Clear();
        foreach (var o in Definitions.Objects.OrderBy(o => o.Name))
            objectListView.Items.Add(
                new ListViewItem(o.ToString(), o.FirstFrameIndex, ObjectListViewGroups[o.Type]) { Tag = o });
    }

    private Bitmap[] LoadImageList(string assetName)
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

        return ret.ToArray();
    }

    private void editObjectDefinitionsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var ode = new ObjectDefinitionEditor(Definitions, Objects);
        ode.ShowDialog();
        RebuildObjects();
    }

    private void mapPicture_Paint(object sender, PaintEventArgs e)
    {
        var g = e.Graphics;

        for (var x = 0; x < currentMap.Width; x++)
            for (var y = 0; y < currentMap.Height; y++)
            {
                var wall = currentMap.Walls[x][y];
                if (wall > 0)
                {
                    var wallImage = wallsImageList.Images[wall];
                    g.DrawImage(wallImage, x * 16 * zoom, y * 16 * zoom, 16 * zoom, 16 * zoom);
                }
            }

        foreach (var item in currentMap.Items)
        {
            var image = GetImage(item);
            var x = item.X;
            var y = item.Y;

            g.DrawImage(image, x * 16 * zoom, y * 16 * zoom, 16 * zoom, 16 * zoom);
        }

        foreach (var cell in phantomCells)
            g.FillRectangle(Brushes.Blue, cell.x * 16 * zoom, cell.y * 16 * zoom, 16 * zoom, 16 * zoom);
    }

    private Image GetImage(SaveItem item) 
        => item.ItypeType switch
        {
            ItemType.Door => doorsImageList.Images[GetDoorDefinition(item.Name).Index],
            ItemType.Actor => actorsImageList.Images[GetActorImageIndex(item.Name)],
            ItemType.Special => specialImageList.Images[GetSpecialDefinition(item.Name).Index],
            _ => objectImageList.Images[GetObjectDefinition(item.Name).FirstFrameIndex],
        };

    private int GetActorImageIndex(string name)
        => Definitions.Actors.FindIndex(a => name.StartsWith(a.Name)) * 4 + (name.Split(' ')[1][1] switch
        {
            'W' => 0,
            'E' => 1,
            'N' => 2,
            _ => 3
        });

    private ActorDefinition GetActorDefinition(string actorName)
        => Definitions.Actors.First(a => actorName.StartsWith(a.Name));

    private ObjectDefinition GetObjectDefinition(string objectName)
        => Definitions.Objects.First(o => o.Name == objectName);

    public DoorDefinition GetDoorDefinition(string doorName)
        => Definitions.Doors.First(d => d.Name == doorName);
    
    private SpecialDefinition GetSpecialDefinition(string name)
        => Definitions.Special.First(s => s.Name == name);

    private void startDraw(MouseEventArgs e)
    {
        if (currentWallNumber == null && currentItemName == null)
            return;

        drawing = true;

        var cellX = (int)(e.X / (16 * zoom));
        var cellY = (int)(e.Y / (16 * zoom));

        startCell = new(cellX, cellY);

        Draw(e);
    }

    private void ResetHistory()
    {
        UndoStack.Clear();
        UndoPointer = -1;
        undoToolStripMenuItem.Enabled = undoButton.Enabled = false;
        redoToolStripMenuItem.Enabled = redoButton.Enabled = false;
    }

    private void PushHistory()
    {
        if (UndoPointer != UndoStack.Count - 1)
        {
            UndoStack = UndoStack.Take(UndoPointer + 1).ToList();
            redoToolStripMenuItem.Enabled = redoButton.Enabled = false;
            UndoPointer = UndoStack.Count - 1;
        }

        UndoStack.Add(currentMap.DeepCopy());
        UndoPointer++;
        if (UndoPointer > 0)
            undoToolStripMenuItem.Enabled = undoButton.Enabled = true;
    }

    private void endDraw()
    {
        if (!drawing)
            return;

        foreach (var cell in phantomCells)
            PlaceItem(cell.x, cell.y);

        drawing = false;
        deleting = false;
        startCell = IntVec2.Zero;
        phantomCells.Clear();

        PushHistory();
    }

    private void mapPicture_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
            startDraw(e);
        else if (e.Button == MouseButtons.Right)
        {
            deleting = true;
            startDraw(e);
        }
    }

    private void mapPicture_MouseUp(object sender, MouseEventArgs e)
    {
        if (drawing && (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right))
            endDraw();
    }

    private void mapPicture_MouseMove(object sender, MouseEventArgs e)
    {
        if (drawing)
            Draw(e);
        else
            previousMouse = null;
    }

    private void Draw(MouseEventArgs e)
    {
        var cellX = (int)(e.X / (16 * zoom));
        var cellY = (int)(e.Y / (16 * zoom));

        switch (currentTool)
        {
            case Tool.Pen:
                PenDraw(cellX, cellY);
                break;
            case Tool.Line:
                LineDraw(cellX, cellY);
                break;
            case Tool.Rectangle:
                RectDraw(cellX, cellY);
                break;
            case Tool.RectangleFill:
                FilledRectDraw(cellX, cellY);
                break;
            case Tool.Ellipse:
                EllipseDraw(cellX, cellY);
                break;
            case Tool.EllipseFill:
                FilledEllipseDraw(cellX, cellY);
                break;
            case Tool.Circle:
                CircleDraw(cellX, cellY);
                break;
            case Tool.CircleFill:
                FilledCircleDraw(cellX, cellY);
                break;
        }

        previousMouse = new(cellX, cellY);
        mapPicture.Invalidate();
    }

    private void PlaceItem(int cellX, int cellY)
    {
        if (cellX < 0 || cellY < 0 || cellX >= currentMap.Width || cellY >= currentMap.Height)
            return;

        if (deleting)
        {
            Delete(cellX, cellY);
            return;
        }

        currentMap.Items.RemoveAll(r => r.X == cellX && r.Y == cellY);
        if (currentWallNumber.HasValue)
            currentMap.Walls[cellX][cellY] = currentWallNumber.Value;
        else if (currentItemName != null && currentItemType.HasValue)
            currentMap.Items.Add(new(currentItemType.Value, currentItemName, cellX, cellY));
        mapPicture.Invalidate();
    }

    private void Delete(int cellX, int cellY)
    {
        currentMap.Items.RemoveAll(r => r.X == cellX && r.Y == cellY);
        currentMap.Walls[cellX][cellY] = -1;
        mapPicture.Invalidate();
    }

    private void PenDraw(int cellX, int cellY)
    {
        if (previousMouse == null)
            PlaceItem(cellX, cellY);
        else
        {
            var prevX = previousMouse.Value.x;
            var prevY = previousMouse.Value.y;
            foreach (var vec in Primitives.Line(prevX, prevY, cellX, cellY, 1, true))
                PlaceItem(vec.x, vec.y);
        }
    }

    private void LineDraw(int cellX, int cellY)
    {
        phantomCells.Clear();
        phantomCells.AddRange(Primitives.Line(startCell, new(cellX, cellY), 1, true));
    }

    private void RectDraw(int cellX, int cellY)
    {
        phantomCells.Clear();
        phantomCells.AddRange(Primitives.Rectangle(startCell.x, startCell.y, cellX, cellY, false, 0, 1));
    }

    private void FilledRectDraw(int cellX, int cellY)
    {
        phantomCells.Clear();
        phantomCells.AddRange(Primitives.Rectangle(startCell.x, startCell.y, cellX, cellY, true, 0, 1));
    }

    private void EllipseDraw(int cellX, int cellY)
    {
        phantomCells.Clear();
        phantomCells.AddRange(Primitives.Ellipse(startCell.x, startCell.y, cellX, cellY, false, 1, true));
    }

    private void FilledEllipseDraw(int cellX, int cellY)
    {
        phantomCells.Clear();
        phantomCells.AddRange(Primitives.Ellipse(startCell.x, startCell.y, cellX, cellY, true, 1, true));
    }

    private void CircleDraw(int cellX, int cellY)
    {
        phantomCells.Clear();
        phantomCells.AddRange(Primitives.Circle(startCell, new IntVec2(cellX, cellY), false, 1, true));
    }

    private void FilledCircleDraw(int cellX, int cellY)
    {
        phantomCells.Clear();
        phantomCells.AddRange(Primitives.Circle(startCell, new IntVec2(cellX, cellY), true, 1, true));
    }

    private void wallListView_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (wallListView.SelectedIndices.Count == 0)
        {
            currentWallNumber = null;
            return;
        }

        objectListView.SelectedIndices.Clear();
        doorsListView.SelectedIndices.Clear();
        actorsListView.SelectedIndices.Clear();
        specialListView.SelectedIndices.Clear();

        currentItemName = null;
        currentItemType = null;
        currentWallNumber = wallListView.SelectedIndices[0];

        currentPicture.Image = wallsImageList.Images[currentWallNumber.Value];
    }

    private void numericUpDown1_ValueChanged(object sender, EventArgs e)
    {
        mapPicture.Size = new((int)(16 * currentMap.Width * zoom), (int)(16 * currentMap.Height * zoom));
        mapPicture.Invalidate();
    }

    private void doorsListView_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (doorsListView.SelectedIndices.Count == 0)
        {
            return;
        }

        objectListView.SelectedIndices.Clear();
        wallListView.SelectedIndices.Clear();
        actorsListView.SelectedIndices.Clear();
        specialListView.SelectedIndices.Clear();

        currentWallNumber = null;
        currentItemType = ItemType.Door;
        currentItemName = doorsListView.SelectedItems[0].Text;
        currentPicture.Image = doorsImageList.Images[GetDoorDefinition(currentItemName).Index];
    }

    private void objectListView_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (objectListView.SelectedIndices.Count == 0)
        {
            return;
        }

        wallListView.SelectedIndices.Clear();
        doorsListView.SelectedIndices.Clear();
        actorsListView.SelectedIndices.Clear();
        specialListView.SelectedIndices.Clear();

        currentWallNumber = null;
        currentItemType = ItemType.Object;
        currentItemName = (objectListView.SelectedItems[0].Tag as ObjectDefinition)!.Name;
        currentPicture.Image = objectImageList.Images[GetObjectDefinition(currentItemName).FirstFrameIndex];
    }

    private void actorsListView_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (actorsListView.SelectedIndices.Count == 0)
        {
            return;
        }

        wallListView.SelectedIndices.Clear();
        doorsListView.SelectedIndices.Clear();
        objectListView.SelectedIndices.Clear();
        specialListView.SelectedIndices.Clear();

        currentWallNumber = null;
        currentItemType = ItemType.Actor;
        currentItemName = actorsListView.SelectedItems[0].Text;
        currentPicture.Image = actorsImageList.Images[actorsListView.SelectedItems[0].ImageIndex];
    }

    private void specialListView_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (specialListView.SelectedIndices.Count == 0)
        {
            return;
        }

        wallListView.SelectedIndices.Clear();
        doorsListView.SelectedIndices.Clear();
        objectListView.SelectedIndices.Clear();
        actorsListView.SelectedIndices.Clear();

        currentWallNumber = null;
        currentItemType = ItemType.Special;
        currentItemName = specialListView.SelectedItems[0].Text;
        currentPicture.Image = specialImageList.Images[specialListView.SelectedItems[0].ImageIndex];
    }

    private void save(SaveMap saveMap, string fileName)
        => File.WriteAllText(fileName, JsonSerializer.Serialize(saveMap));

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (fileName == null)
        {
            saveAsToolStripMenuItem_Click(sender, e);
            return;
        }
        save(currentMap, fileName);
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog();
        if (sfd.ShowDialog() != DialogResult.OK)
            return;
        fileName = sfd.FileName;
        save(currentMap, fileName);
    }

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog();
        if (ofd.ShowDialog() != DialogResult.OK)
            return;
        fileName = ofd.FileName;
        currentMap = open(fileName) ?? new(48, 48);
        mapPicture.Invalidate();
    }

    private SaveMap? open(string fileName)
    {
        try
        {
            var ret = JsonSerializer.Deserialize<SaveMap>(File.ReadAllText(fileName));
            ResetHistory();
            PushHistory();
            return ret;
        } catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Could not open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }

    private void Undo()
    {
        if (!UndoStack.Any() || UndoPointer == -1)
            return;

        UndoPointer--;
        currentMap = UndoStack[UndoPointer];
        redoToolStripMenuItem.Enabled = redoButton.Enabled = true;

        if (UndoPointer == 0)
            undoToolStripMenuItem.Enabled = undoButton.Enabled = false;

        mapPicture.Invalidate();
    }

    private void Redo()
    {
        if (!UndoStack.Any() || UndoPointer == UndoStack.Count)
            return;

        UndoPointer++;
        currentMap = UndoStack[UndoPointer];

        if (UndoPointer == UndoStack.Count - 1)
            redoToolStripMenuItem.Enabled = redoButton.Enabled = false;

        mapPicture.Invalidate();
    }

    private void undoButton_Click(object sender, EventArgs e)
        => Undo();

    private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        => Undo();

    private void redoButton_Click(object sender, EventArgs e)
        => Redo();

    private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        => Redo();
}
