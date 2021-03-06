﻿using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Roads
{
    public class Road
    {
        public int Lanes;
        public Point StartPoint;
        public Point EndPoint;

        public Road(Point start, Point end, int lanes = 1)
        {
            StartPoint = new Point(start.Position, this);
            EndPoint = new Point(end.Position, this);
            Lanes = lanes;
        }

        public Point GetOther(Point current)
        {
            return StartPoint.Equals(current) ? EndPoint : StartPoint;
        }

        public float Length()
        {
            return Vector2.Distance(StartPoint.Position, EndPoint.Position);
        }

        public override bool Equals(object obj)
        {
            Road roadObj = obj as Road;
            return StartPoint.Equals(roadObj.StartPoint) && EndPoint.Equals(roadObj.EndPoint) ||
                   EndPoint.Equals(roadObj.StartPoint) && StartPoint.Equals(roadObj.EndPoint);
        }
    }
}

