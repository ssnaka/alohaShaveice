using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Line : MonoBehaviour
{
    public Material material;
    private Mesh mesh;
    public Vector3[] vertices;
    public Vector3 start;
    public Vector3 end;
    public int lineWidth;

    public List<LineRenderer> lines = new List<LineRenderer>();
    public Vector3[] points = new Vector3[200];

    // Use this for initialization
    void Start()
    {

        foreach (Transform item in transform)
        {
            lines.Add(item.GetComponent<LineRenderer>());
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetVertexCount(int count)
    {
        int i = 0;
        if (lines.Count < count)
            AddLine();
        foreach (LineRenderer item in lines)
        {
            if (i < count)
            {
                item.enabled = true;
                SetSorting(item);
            }
            else
                item.enabled = false;
            i++;
        }
    }

    public void AddPoint(Vector3 position, int index)
    {
        points[index] = position;
        if (index > 0)
        {
            lines[index].SetPosition(0, points[index - 1]);
            lines[index].SetPosition(1, points[index]);
        }
    }

    void AddLine()
    {
        GameObject newLine = Instantiate(transform.GetChild(0).gameObject) as GameObject;
        newLine.transform.SetParent(transform);
        lines.Add(newLine.GetComponent<LineRenderer>());

    }

    void SetSorting(LineRenderer lr)
    {
        lr.sortingLayerID = 0;
        lr.sortingOrder = 1;

    }
}
