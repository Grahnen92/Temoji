using UnityEngine;
using System.Collections;
using System;

public class MakePlateRings : MonoBehaviour {

    public GameObject plate;
    public float radius;
    public Direction rotation;
    public int plateCount;
    public bool lookAtCenter;
    public float rotationSpeed;

    public enum Direction { Clockwise, CounterClock };
    public enum Axis { x, y, z };


    // Use this for initialization
    void Start() {
        Vector3 pos = transform.position;

        Vector3[] points = GenerateCirclePts(pos, radius, plateCount, Axis.z);
        foreach (Vector3 pt in points)
        {
            plate = Instantiate(plate);
            plate.transform.parent = this.transform;
            plate.transform.position = pt;
            if(lookAtCenter) plate.transform.LookAt(pos);
        }
    }

    // Update is called once per frame
    void Update() {
        if (rotation == Direction.Clockwise)
            transform.Rotate(0, 0, rotationSpeed);
        else
            transform.Rotate(0, 0, -rotationSpeed);
    }

    // Axis: (x,0), (y,1), (z,2)
    Vector3[] GenerateCirclePts(Vector3 pos, float radius, int numPoints, Axis axis)
    {
        Vector3[] ctrlPts = new Vector3[numPoints];
        double theta = 2 * Math.PI / numPoints;
        for (int i = 0; i < numPoints; i++)
        {
            float first = (float)(radius * Math.Cos(theta * i));
            float sec = (float)(radius * Math.Sin(theta * i));

            switch (axis)
            {
                case Axis.x:
                    ctrlPts[i] = new Vector3(0, first, sec) + pos;
                    break;
                case Axis.y:
                    ctrlPts[i] = new Vector3(first, 0, sec) + pos;
                    break;
                case Axis.z:
                    ctrlPts[i] = new Vector3(first, sec, 0) + pos;
                    break;
            }
        }

        return ctrlPts;
    }
}
