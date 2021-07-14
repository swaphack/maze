using System.Collections.Generic;
using UnityEngine;

namespace DelaunayVoronoi
{
    public class Point
    {
        /// <summary>
        /// Used only for generating a unique ID for each instance of this class that gets generated
        /// </summary>
        private static int _counter;

        /// <summary>
        /// Used for identifying an instance of a class; can be useful in troubleshooting when geometry goes weird
        /// (e.g. when trying to identify when Triangle objects are being created with the same Point object twice)
        /// </summary>
        private readonly int _instanceId = _counter++;

        public float X => Position.x;
        public float Y => Position.y;

        public Vector3 Position { get; set; }

        public HashSet<Triangle> AdjacentTriangles { get; } = new HashSet<Triangle>();

        public Point(Vector3 pos)
        {
            this.Position = pos;
        }
        public Point(float x, float y)
        {
            Position = new Vector3(x, y);
        }

        public override string ToString()
        {
            // Simple way of seeing what's going on in the debugger when investigating weirdness
            return $"{nameof(Point)} ({X:0.##},{Y:0.##})";
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            var point = obj as Point;

            return this.Position == point.Position;
        }

        public override int GetHashCode()
        {
            int hCode = (int)X ^ (int)Y;
            return hCode.GetHashCode();
        }
    }
}