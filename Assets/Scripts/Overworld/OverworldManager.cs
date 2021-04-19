using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldManager : Manager<OverworldManager>
{
    [SerializeField] private GameObject map;
    private GameObject mapContainer;
    private Map mapRef;

    public void createOverworldMap()
    {
        mapContainer = Instantiate(map, transform);
        mapRef = mapContainer.transform.GetChild(0).GetChild(0).GetComponent<Map>();
    }

    public void setShowMap(bool b)
    {
        mapContainer.SetActive(b);
    }

    public void advanceLayer(MapNode fromNode)
    {
        mapRef.advanceNode(fromNode);
    }
}
