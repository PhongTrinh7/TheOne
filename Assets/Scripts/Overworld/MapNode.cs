using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MapNode : MonoBehaviour
{
    public List<MapNode> next, last;
    public string nodeType;
    public Image icon;
    public bool activated, cleared;

    public virtual void addNextNode(MapNode node)
    {
        next.Add(node);
    }

    public virtual void addLastNode(MapNode node)
    {
        last.Add(node);
    }

    public virtual void addNextNodes(List<MapNode> nodes)
    {
        foreach (MapNode node in nodes)
        {
            next.Add(node);
        }
    }

    public virtual void addLastNodes(List<MapNode> nodes)
    {
        foreach (MapNode node in nodes)
        {
            last.Add(node);
        }
    }

    public virtual void removeNextNode()
    {

    }

    public virtual void removeLastNode()
    {

    }

    public virtual void activateNode()
    {
        activated = true;
    }

    public virtual void deactivateNode()
    {
        activated = false;
    }

    public virtual void enterNode()
    {
        Debug.Log("Entering " + nodeType);
    }
}
