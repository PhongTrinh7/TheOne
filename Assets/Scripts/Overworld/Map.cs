using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public int stepAmount;
    public MapNode[] availableNodes; 
    public List<MapNode> instantiatedNodes;
    public GameObject mapNodeLayer;

    public void Start()
    {
        populateMap();
    }

    public void populateMap()
    {
        MapNode mapNode = availableNodes[Random.Range(0, availableNodes.Length)];

        MapNode nodeInstance = Instantiate(mapNode, transform);

        List<MapNode> mapNodes = new List<MapNode>();

        mapNodes.Add(nodeInstance);

        nodeInstance.addNextNodes(createNextLayer(0, mapNodes));
    }

    protected List<MapNode> createNextLayer(int currentStep, List<MapNode> nodes)
    {
        List<MapNode> mapNodes = new List<MapNode>();

        if (currentStep < stepAmount)
        {
            GameObject nodeLayer = Instantiate(mapNodeLayer, transform);

            for (int i = 0; i < Random.Range(1, 4); i++)
            {
                MapNode mapNode = availableNodes[Random.Range(0, availableNodes.Length)];

                MapNode nodeInstance = Instantiate(mapNode, nodeLayer.transform);

                nodeInstance.addLastNodes(nodes);

                mapNodes.Add(nodeInstance);
            }

            List<MapNode> nextNodes = createNextLayer(currentStep + 1, mapNodes);

            foreach (MapNode n in mapNodes)
            {
                n.addNextNodes(nextNodes);
            }
        }
        else
        {
            MapNode mapNode = availableNodes[Random.Range(0, availableNodes.Length)];

            MapNode nodeInstance = Instantiate(mapNode, transform);

            nodeInstance.addLastNodes(nodes);

            mapNodes.Add(nodeInstance);
        }

        return mapNodes;
    }
}
