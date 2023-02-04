using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode
{
    private int origCost;
    private int cost;
    private Unit occupant = null;

    public Unit Occupant
    {
        get;
    }
    public int Cost
    {
        get { return cost; }
        set { cost = value; origCost = value; }
    }

    public MapNode(int c)
    {
        origCost = c;
        cost = origCost;
    }

    public bool Occupy(Unit u)
    {
        if (cost >= 0)
        {
            cost = -1;
            occupant = u;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Leave(Unit u)
    {
        if (cost < 0 && u == occupant)
        {
            cost = origCost;
            occupant = null;
            return true;
        }
        else
        {
            return false;
        }
    }



}

public class Map2D
{
    private MapNode[,] map;
    private Vector2Int size;

    public Map2D(int sx, int sy)
    {
        init(sx, sy);
    }

    public Map2D(Vector3Int v3)
    {
        init(v3.x, v3.y);
    }

    public void init(int sx, int sy)
    {
        map = new MapNode[sx, sy];
        size.x = sx; size.y = sy;


        for (int i = 0; i < sx; i++)
        {
            for (int j = 0; j < sy; j++)
            {
                map[i, j] = new MapNode(0);
            }
        }
    }

    public Vector2Int getSize()
    {
        return size;
    }

    public MapNode getNode(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < size.x && y < size.y)
        {
            return map[x, y];
        }

        else return null;
    }

    public MapNode getNode(Vector3Int v)
    {
        return getNode(v.x, v.y);
    }

    public MapNode getNode(Vector2Int v)
    {
        return getNode(v.x, v.y);
    }
}
