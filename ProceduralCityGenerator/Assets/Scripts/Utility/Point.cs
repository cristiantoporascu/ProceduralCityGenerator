using Assets.Scripts.Roads;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public class Point
    {
        public Vector2 Position;
        public Road Parent;

        public Point() {}

        public Point(Vector2 position, Road parent = null)
        {
            Position = new Vector2(position.x, position.y);
            Parent = parent;
        }

        public Vector3 GetVector3Pos()
        {
            return new Vector3(Position.x, 0, Position.y);
        }

        public override bool Equals(object obj)
        {
            return (Vector2.Distance((obj as Point).Position, Position) < 0.01f);
        }
    }
}
