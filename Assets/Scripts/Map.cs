using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPos2D
{
    public Vector2Int pos2D;

    public override bool Equals(object obj) => this.Equals(obj as MapPos2D);

    public bool Equals(MapPos2D p)
    {
        if (p is null)
        {
            return false;
        }

        // Optimization for a common success case.
        if (Object.ReferenceEquals(this, p))
        {
            return true;
        }

        // If run-time types are not exactly the same, return false.
        if (this.GetType() != p.GetType())
        {
            return false;
        }

        return (pos2D.x == p.pos2D.x) && (pos2D.y == p.pos2D.y);
    }

    public override int GetHashCode()
    {
        return pos2D.GetHashCode();
    }

    public static bool operator ==(MapPos2D lhs, MapPos2D rhs)
    {
        if (lhs is null)
        {
            if (rhs is null)
            {
                return true;
            }

            // Only the left side is null.
            return false;
        }
        // Equals handles case of null on right side.
        return lhs.Equals(rhs);
    }

    public static bool operator !=(MapPos2D lhs, MapPos2D rhs) => !(lhs == rhs);
}

public class MapNode
{
    private int origCost;
    private int cost;
    private Unit occupant = null;

    public Unit Occupant
    {
        get { return occupant; }
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
    private Vector2Int offset;

    public Map2D(int sx, int sy, int ox, int oy)
    {
        init(sx, sy, ox, oy);
    }

    public Map2D(Vector3Int v3, int ox, int oy)
    {
        init(v3.x, v3.y, ox, oy);
    }

    public bool makeImpassable(Vector2Int p)
    {
        MapNode n = getNode(p);

        if (n.Occupant == null)
        {
            n.Cost = -1;
            return true;
        }

        return false;
    }

    public void init(int sx, int sy, int ox, int oy)
    {
        offset = new Vector2Int(ox, oy);
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

    public MapNode getNode(int px, int py)
    {
        int x = px + offset.x;
        int y = py + offset.y;

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

    public List<PathFinding.PathNode> generatePathNodeList()
    {
        List<PathFinding.PathNode> pathList = new List<PathFinding.PathNode>();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (map[i,j].Cost >= 0)
                {
                    pathList.Add(new PathFinding.PathNode( new Vector2Int(i - offset.x, j - offset.y)));
                }
            }
        }

        return pathList;
    }
}
