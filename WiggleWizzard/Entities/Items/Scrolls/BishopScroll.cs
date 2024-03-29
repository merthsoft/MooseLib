﻿using Merthsoft.Moose.Dungeon.Tiles;

namespace Merthsoft.Moose.Dungeon.Entities.Items.Scrolls;
public class BishopScroll : Scroll
{
    public BishopScroll(ScrollDef def, int x, int y) : base(def, x, y)
    {

    }

    protected override IEnumerable<Point> AllowedCells()
    {
        var (playerX, playerY) = player.Cell;
        var deltaX = -1;
        var deltaY = -1;
        while (!game.GetDungeonTile(playerX + deltaX, playerY + deltaY).BlocksSight())
        {
            yield return new(playerX + deltaX, playerY + deltaY);
            deltaX -= 1;
            deltaY -= 1;
        }

        deltaX = -1;
        deltaY = 1;
        while (!game.GetDungeonTile(playerX + deltaX, playerY + deltaY).BlocksSight())
        {
            yield return new(playerX + deltaX, playerY + deltaY);
            deltaX -= 1;
            deltaY += 1;
        }

        deltaX = 1;
        deltaY = -1;
        while (!game.GetDungeonTile(playerX + deltaX, playerY + deltaY).BlocksSight())
        {
            yield return new(playerX + deltaX, playerY + deltaY);
            deltaX += 1;
            deltaY -= 1;
        }

        deltaX = 1;
        deltaY = 1;
        while (!game.GetDungeonTile(playerX + deltaX, playerY + deltaY).BlocksSight())
        {
            yield return new(playerX + deltaX, playerY + deltaY);
            deltaX += 1;
            deltaY += 1;
        }
    }
}
