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
    public Axis ringAxis;
    public Axis rotationAxis;
    public float offset = 0;

    public enum Direction { Clockwise, CounterClock, None };
    public enum Axis { x, y, z };

    Vector3 rotationVec = new Vector3();
    GameObject newPlate;

    // Use this for initialization
    void Start() {
        Vector3 pos = transform.position;

        Vector3[] points = GenerateCirclePts(pos, radius, plateCount, ringAxis);
        Vector3 forward = transform.forward;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
      
        foreach (Vector3 pt in points)
        {
            newPlate = Instantiate(plate);
            newPlate.transform.parent = this.transform;
            newPlate.transform.position = pt;
            if(lookAtCenter) newPlate.transform.LookAt(pos);
        }

        transform.forward = forward;
    }

    // Update is called once per frame
    void Update() {
        int direction;
        if (rotation == Direction.Clockwise)
            direction = 1;
        else if (rotation == Direction.CounterClock)
            direction = -1;
        else
            direction = 0;
        float velocity = direction * rotationSpeed;

        switch (rotationAxis)
        {
            case Axis.x:
                transform.Rotate(velocity, 0, 0);
                break;
            case Axis.y:
                transform.Rotate(0, velocity, 0);
                break;
            case Axis.z:
                transform.Rotate(0, 0, velocity);
                break;
        }
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
            //Vector3.

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
