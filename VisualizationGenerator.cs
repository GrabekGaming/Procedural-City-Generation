using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Local
{
    //parameters
    public Vector3 pos, dir;
    public int length;
    public GameObject parent;
}

public class VisualizationGenerator : MonoBehaviour
{
    /*********************************************/
    ///////////////Variables /////////////////////
    public ProceduralGenerator generator;
    List<Vector3> positions = new List<Vector3>();
    public Material roadMaterial;
    public GameObject currentLine;
    public Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
    public Camera mainCamera;
    public GameObject building;
    public GameObject colliderObject;
    public List<Vector3> intersections;
    public GameObject crossing;
    public List<Transform> roads;
    public float intersectionCap;
    public float crossingRange;
    public float extensionRange;

    //Variables to change randomly 
    public int length = 12; //length >= 1
    public float angle = 90; // angle >= 70 dgr

    public int Length
    {
        get
        {
            if (length > 0)
            {
                return length;
            }
            else
            {
                return 1;
            }
        }
        set => length = value;
    }
    /*********************************************/
    public void Start()
    {
        var sequence = generator.sequenceGenerator();
        VisualizeSequence(sequence);
    }
    public void Update()
    {
        //some cool features
    }

    public void DrawNewCity()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        currentLine = this.gameObject;
        roads.Clear();
        bounds = new Bounds(Vector3.zero, Vector3.zero);
        var sequence = generator.sequenceGenerator();
        VisualizeSequence(sequence);
    }

    private void VisualizeSequence(string sequence)// switch
    {
        Stack<Local> savePoints = new Stack<Local>();
        var currentPosition = Vector3.zero;
        Vector3 tempPosition = Vector3.zero;

        /// IMPORTANT - depends on if we use 2d or 3d the angles may differ 
        Vector3 direction = Vector3.forward;


        positions.Add(currentPosition); // add new positions to the list

        foreach (var let in sequence)
        {
            commands com = (commands)let;
            switch (com)
            {
                case commands.draw:
                    tempPosition = currentPosition;
                    currentPosition += direction * length;
                    currentPosition = draw(tempPosition, currentPosition, Color.red, currentLine, direction);
                    int newLength = UnityEngine.Random.Range(4, 12); // temporary random generator of the lenght of the line
                    Length = newLength;
                    //Length -= 1;
                    positions.Add(currentPosition);
                    break;

                case commands.turnRight:
                    direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                    //int newAngle = UnityEngine.Random.Range(70, 90); // temporary random generator of an angle
                    //angle = newAngle;
                    break;

                case commands.turnLeft:
                    direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                    //int newAngle1 = UnityEngine.Random.Range(70, 90); // temporary random generator of an angle
                    //angle = newAngle1;
                    break;

                case commands.save:
                    savePoints.Push(new Local
                    {
                        pos = currentPosition,
                        dir = direction,
                        length = Length,
                        parent = currentLine
                    });
                    break;

                case commands.load:
                    if (savePoints.Count > 0)
                    {
                        var local = savePoints.Pop();
                        currentPosition = local.pos;
                        direction = local.dir;
                        Length = local.length;
                        currentLine = local.parent;
                    }
                    else
                        throw new System.Exception("Error");
                    break;

                default:
                    break;
            }
        }

        CalculateViewport();
        AddBuildings();
    }

    private void CalculateViewport()
    {
        mainCamera.orthographicSize = bounds.size.x > bounds.size.z ? (bounds.size.x / 2) : (bounds.size.z / 2) * 1.1f;
        mainCamera.transform.position = new Vector3(bounds.center.x, 10, bounds.center.z);
    }

    private void UpdateBounds(Bounds branchBounds)
    {
        bounds.Encapsulate(branchBounds);
    }

    private Vector3 draw(Vector3 begin, Vector3 end, Color color, GameObject parent, Vector3 direction)
    {
        end = CheckEnd(end, direction);
        if(CheckCrossings(begin, end))
        {
            currentLine = new GameObject("line");
            currentLine.tag = "Line";
            currentLine.transform.position = begin;
            currentLine.transform.SetParent(parent.transform);


            var lineRenderer = currentLine.AddComponent<LineRenderer>();

            lineRenderer.material = roadMaterial;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.startWidth = 0.3f;
            lineRenderer.endWidth = 0.3f;
            lineRenderer.SetPosition(0, begin);
            lineRenderer.SetPosition(1, end);

            GameObject col = Instantiate(colliderObject);
            col.transform.SetParent(currentLine.transform);
            var capsuleCollider = col.GetComponent<CapsuleCollider>();
            col.transform.position = new Vector3((float)((begin.x + end.x) * 0.5), 0, (float)((begin.z + end.z) * 0.5));
            col.transform.LookAt(end);
            capsuleCollider.height = Vector3.Distance(begin, end);
            capsuleCollider.radius = 0.02f;
            capsuleCollider.direction = 2;

            roads.Add(lineRenderer.transform);

            UpdateBounds(currentLine.GetComponent<LineRenderer>().bounds);
            return end;
        }
        return begin;        
    }

    public Vector3 CheckEnd(Vector3 end, Vector3 direction)
    {
        Collider[] colliders = Physics.OverlapSphere(end, crossingRange);

        if(colliders.Length > 0)
        {
            end = colliders[0].gameObject.transform.position;
            return end;
        }
        if(Physics.Raycast(end, direction, out RaycastHit hitInfo, extensionRange))
        {
            end = hitInfo.point;
        }
        return end;
    }

    public bool CheckCrossings(Vector3 begin, Vector3 end)
    {
        RaycastHit[] hits = Physics.RaycastAll(begin, end, Vector3.Distance(begin, end));
        if (hits.Length < intersectionCap)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                intersections.Add(hits[i].point);
                Instantiate(crossing, hits[i].point, Quaternion.identity);
            }
            return true;
        }
        return false;
    }

    public void AddBuildings()
    {
        foreach(Transform road in roads)
        {
            GenerateBuildings(road.GetComponent<LineRenderer>());
        }
    }

    public void GenerateBuildings(LineRenderer lineRenderer)
    {
        Vector3 start = lineRenderer.GetPosition(0);
        Vector3 end = lineRenderer.GetPosition(1);
        float distance = Vector3.Distance(start, end);
        Quaternion rotation = lineRenderer.gameObject.GetComponentInChildren<CapsuleCollider>().transform.rotation;
        var vector = end - start;
        var inverseVector = new Vector3(vector.z, 0, -vector.x);
        inverseVector.Normalize();

        for (float d = 0; d < distance; d += 2.6f) 
        {
            Vector3 lerpSpot = new Vector3(Vector3.Lerp(start, end, d / distance).x, 0, Vector3.Lerp(start, end, d / distance).z);
            Vector3 position = (1f * inverseVector) + lerpSpot;
            
            GameObject currentBuilding = Instantiate(building, lineRenderer.transform);
            currentBuilding.transform.position = position;
            currentBuilding.transform.rotation = rotation;

            position = (-1f * inverseVector) + lerpSpot;
            GameObject leftBuilding = Instantiate(building, lineRenderer.transform);
            leftBuilding.transform.position = position;
            leftBuilding.transform.rotation = rotation;
        }
    }

    public enum commands //commands 
    {
        defaultt = '1',
        draw = 'F',
        turnRight = '+',
        turnLeft = '-',
        save = '[',
        load = ']'


    }
}
