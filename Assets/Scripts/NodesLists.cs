using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodesLists : MonoBehaviour
{
    //list of of each type of piece that can be attached
    public List<GameObject> connectors = new List<GameObject>();
    public List<GameObject> straights = new List<GameObject>();
    public List<GameObject> spirals = new List<GameObject>();

    // dictionary that maps transform positions to game objects that have snapped to those positions
    public Dictionary<Vector3, GameObject> snappedPositions = new Dictionary<Vector3, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
