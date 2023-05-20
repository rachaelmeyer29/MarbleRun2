using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapConnection : MonoBehaviour
{
    public NodesLists NodesLists;

    //offset transforms for each type of piece that can be attached
    public Transform connectorPosition;
    public Transform straightPosition;
    public Transform spiralPosition;
    public GameObject xrOrigin;

    private Rigidbody rb;

    //float to hold the closest position the piece is to the other game pieces's nodes
    public float snapDistancePieces = 0.04f;
    //transform that is populated when there is a successful snap made
    public Transform connectedNodePieceOffset = null;

    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.tag == "Connector")
        {
            NodesLists.connectors.Add(gameObject);
        }
        if (gameObject.tag == "Straight")
        {
            NodesLists.straights.Add(gameObject);
        }
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        float smallestDistance = snapDistancePieces;

        //if object is a connector, scan other pieces for positioning
        if (gameObject.tag == "Connector")
        {
            //scanning other connector pieces
            foreach (GameObject node in NodesLists.connectors)
            {
                Transform nodeOffset = node.transform.Find("ConnectorPieceOffset").GetComponent<Transform>();
                if ((Vector3.Distance(nodeOffset.position, transform.position) < smallestDistance) && (nodeOffset.position != connectorPosition.position) && (!NodesLists.snappedPositions.ContainsKey(nodeOffset.position)))
                {
                    //snapping piece to node location
                    transform.position = nodeOffset.position;
                    transform.rotation = nodeOffset.rotation;

                    //turning on kinematic for the piece that snapped (game object) because it needs to stick to the one below it no matter what way you turn it
                    rb.isKinematic = true;
                    //populating the transform with the node the piece snapped to so I can use it later to see if there's been a successful snap
                    connectedNodePieceOffset = node.transform.Find("ConnectorPieceOffset").GetComponent<Transform>();

                    //turn off mesh colliders on both objects
                    GetComponent<MeshCollider>().enabled = false;
                    if (node.transform.GetComponent<MeshCollider>().enabled == true)
                    {
                        node.transform.GetComponent<MeshCollider>().enabled = false;
                    }

                    //turn on box colliders in snapped object (this is so a marble can pass through the game pieces)
                    var childTransforms = transform.GetComponentsInChildren<Transform>();
                    if (childTransforms.Length > 0)
                    {
                        foreach (var childTransform in childTransforms)
                        {
                            if ((childTransform.tag == "Colliders") && (childTransform.gameObject.GetComponent<BoxCollider>().enabled == false))
                            {
                                childTransform.gameObject.GetComponent<BoxCollider>().enabled = true;
                            }
                        }
                    }
                    //turn on box colliders in snap-recieving (node) object
                    var nodeChildTransforms = node.transform.transform.GetComponentsInChildren<Transform>();
                    if (nodeChildTransforms.Length > 0)
                    {
                        foreach (var childTransform in nodeChildTransforms)
                        {
                            if ((childTransform.tag == "Colliders") && (childTransform.gameObject.GetComponent<BoxCollider>().enabled == false))
                            {
                                childTransform.gameObject.GetComponent<BoxCollider>().enabled = true;
                            }
                        }
                    }

                    //update smallest distance
                    smallestDistance = Vector3.Distance(nodeOffset.position, transform.position);


                    // add the snapped position to the dictionary
                    NodesLists.snappedPositions[nodeOffset.position] = gameObject;

                }
                //if there has been a successful snap, this will be populated. If it's populated, then testing to see how far my game object is from the node
                if (connectedNodePieceOffset is not null)
                {
                    //testing to see how far my game object is from the node it successfully snapped to. If it's been removed, the kinematic goes back to being turned off
                    if ((Vector3.Distance(connectedNodePieceOffset.position, transform.position) > .05f))
                    {
                        rb.isKinematic = false;
                        rb.useGravity = true;
                        connectedNodePieceOffset = null;
                    }


                //commented out for now because I need to figure out and fix the issue before it matters to add this back in and make sure its working
                    /*     var childTransforms = transform.GetComponentsInChildren<Transform>();
                         if (childTransforms.Length > 0)
                         {
                             foreach (var childTransform in childTransforms)
                             {
                                 if ((childTransform.tag == "Colliders") && (childTransform.gameObject.GetComponent<BoxCollider>().enabled == true))
                                 {
                                     childTransform.gameObject.GetComponent<BoxCollider>().enabled = false;
                                 }
                             }
                         }
                         GetComponent<MeshCollider>().enabled = true;*/
                }

                //testing to see wtf is going on by changing the color of one of the pieces if the main object kinematic has been turned back on or not...
                //(I hav a mac so I have to build and run to my oculus every time I test it, and can't see the inspector)
                //based on this test, it seems like kinematic has NOT been turned back on, which doesn't make sense then why it's freezing!!
                if (rb.isKinematic == false)
                {
                    GameObject.Find("ConnectorPiece1 (1)").GetComponent<Renderer>().material.color = new Color(0, 0, 102);
                }

                if (rb.isKinematic == true)
                {
                    GameObject.Find("ConnectorPiece1 (1)").GetComponent<Renderer>().material.color = new Color(0, 0, 0);
                }
            }

            //HI ADAM! IGNORE EVERYTHING BELOW THIS LINE BC NOT IMPORTANT TO THE ISSUE... JUST TESTING OTHER PIECES THAT ARE NOT IN THE SCENE RIGHT NOW

            //scanning straight pieces
          /*  foreach (GameObject node in NodesLists.straights)
            {
                Transform nodeOffset = node.transform.Find("ConnectorPieceOffset").GetComponent<Transform>();
                if ((Vector3.Distance(nodeOffset.position, transform.position) < smallestDistance) && (nodeOffset.position != connectorPosition.position) && !NodesLists.snappedPositions.ContainsKey(nodeOffset.position))
                {
                    transform.position = nodeOffset.position;
                    transform.rotation = nodeOffset.rotation;
                    rb.isKinematic = true;
                    smallestDistance = Vector3.Distance(nodeOffset.position, transform.position);

                    // add the snapped position to the dictionary
                    NodesLists.snappedPositions[nodeOffset.position] = gameObject;
                }
                
            }
        }

        //if object is a straight, scan other pieces for positioning
        if (gameObject.tag == "Straight")
        {
            //scanning connector pieces
            foreach (GameObject node in NodesLists.connectors)
            {
                Transform nodeOffset = node.transform.Find("StraightPieceOffset").GetComponent<Transform>();
                if ((Vector3.Distance(nodeOffset.position, transform.position) < smallestDistance) && (nodeOffset.position != connectorPosition.position) && !NodesLists.snappedPositions.ContainsKey(nodeOffset.position))
                {
                    transform.position = nodeOffset.position;
                    transform.rotation = nodeOffset.rotation;
                    rb.isKinematic = true;
                    smallestDistance = Vector3.Distance(nodeOffset.position, transform.position);

                    // add the snapped position to the dictionary
                    NodesLists.snappedPositions[nodeOffset.position] = gameObject;
                }
                
            }

            //scanning other straight pieces
            foreach (GameObject node in NodesLists.straights)
            {
                Transform nodeOffset = node.transform.Find("StraightPieceOffset").GetComponent<Transform>();
                if ((Vector3.Distance(nodeOffset.position, transform.position) < smallestDistance) && (nodeOffset.position != connectorPosition.position) && !NodesLists.snappedPositions.ContainsKey(nodeOffset.position))
                {
                    transform.position = nodeOffset.position;
                    transform.rotation = nodeOffset.rotation;
                    rb.isKinematic = true;
                    smallestDistance = Vector3.Distance(nodeOffset.position, transform.position);

                    // add the snapped position to the dictionary
                    NodesLists.snappedPositions[nodeOffset.position] = gameObject;
                }
                
            }*/
        }
    }
}
