using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public int stepAmount;
    public int currentLayer;

    public MapNode[] availableNodes;
    public GameObject mapNodeLayer; //Prefab
    public List<MapNode> instantiatedNodes;
    public List<List<MapNode>> mapNodesLayers;

    public void Start()
    {
        currentLayer = 0;
        mapNodesLayers = new List<List<MapNode>>();
        populateMap();
    }

    public void populateMap()
    {
        GameObject nodeLayer = Instantiate(mapNodeLayer, transform);

        MapNode mapNode = availableNodes[Random.Range(0, availableNodes.Length)];

        MapNode nodeInstance = Instantiate(mapNode, nodeLayer.transform);

        List<MapNode> mapNodes = new List<MapNode>();

        mapNodes.Add(nodeInstance);

        mapNodesLayers.Add(mapNodes);

        nodeInstance.addNextNodes(createNextLayer(0, mapNodes));
    }

    protected List<MapNode> createNextLayer(int currentStep, List<MapNode> nodes)
    {
        List<MapNode> mapNodes = new List<MapNode>();

        mapNodesLayers.Add(mapNodes);

        if (currentStep < stepAmount)
        {
            GameObject nodeLayer = Instantiate(mapNodeLayer, transform);

            for (int i = 0; i < Random.Range(1, 4); i++)
            {
                MapNode mapNode = availableNodes[Random.Range(0, availableNodes.Length)];

                MapNode nodeInstance = Instantiate(mapNode, nodeLayer.transform);

                nodeInstance.activated = false;

                nodeInstance.GetComponent<Button>().interactable = false;

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
            GameObject nodeLayer = Instantiate(mapNodeLayer, transform);

            MapNode mapNode = availableNodes[Random.Range(0, availableNodes.Length)];

            MapNode nodeInstance = Instantiate(mapNode, nodeLayer.transform);

            nodeInstance.activated = false;

            nodeInstance.GetComponent<Button>().interactable = false;

            nodeInstance.addLastNodes(nodes);

            mapNodes.Add(nodeInstance);
        }

        return mapNodes;
    }

    public void advanceNode(MapNode fromNode)
    {
        fromNode.cleared = true;

        foreach (MapNode node in fromNode.next)
        {
            node.activated = true;

            node.GetComponent<Button>().interactable = true;
        }

        foreach (MapNode node in mapNodesLayers[currentLayer])
        {
            node.activated = false;

            node.GetComponent<Button>().interactable = false;
        }

        currentLayer++;

        //mapNodeLayers[currentLayer].transform.GetComponentInChildren<MapNode>();
    }
}
