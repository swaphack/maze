using System;
using UnityEngine;

namespace Geometry
{
	public struct Point
	{
		public int x;
		public int y;

		public Point(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public Point(Point point)
		{
			this.x = point.x;
			this.y = point.y;
		}

		public Point(Vector2 point)
		{
			this.x = (int)point.x;
			this.y = (int)point.y;
		}

		public override bool Equals (object other)
		{
			return base.Equals (other);
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override string ToString ()
		{
			return string.Format ("({0}:{1})", x, y);
		}

		public static bool operator==(Point a, Point b)
		{
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Point a, Point b)
		{
			return a.x != b.x || a.y != b.y;
		}

		public static Point operator+(Point a, Point b)
		{
			return new Point(a.x + b.x, a.y+b.y);
		}

		public static Point operator-(Point a, Point b)
		{
			return new Point(a.x - b.x, a.y-b.y);
		}

		public static Point operator*(Point a, int k)
		{
			return new Point(a.x * k, a.y * k);
		}

		public static Point operator*(int k, Point a)
		{
			return new Point(a.x * k, a.y * k);
		}

		public static implicit operator Vector2(Point a)
		{
			return new Vector2 (a.x, a.y);
		}

		public static implicit operator Point(Vector2 a)
		{
			return new Point ((int)a.x, (int)a.y);
		}
	}

	/// <summary>
	/// 线段
	/// </summary>
	public class Line
	{
		public Point src;
		public Point dest;

		public Line()
		{
			this.src = new Point ();
			this.dest = new Point();
		}

		public Line(Point src, Point dest)
		{
			this.src = src;
			this.dest  = dest;
		}

		public Line(Vector2 src, Vector2 dest)
		{
			this.src = src;
			this.dest  = dest;
		}

		public override string ToString ()
		{
			return string.Format ("Src : {0}, Dest :{1}", src, dest);
		}

		/// <summary>
		/// 点在线上
		/// </summary>
		/// <param name="point">Point.</param>
		public bool Contains(Point point)
		{
			if (src == point || dest == point) {
				return true;
			}

			Point vs = point - src;
			Point vd = point - dest;

			if (Vector2.Dot ((Vector2)vs, (Vector2)vd) > Constraints.Eps) {
				return false;
			}

			float eps = Constraints.Eps;
			if ((Mathf.Min (src.x, dest.x) - eps) <= point.x 
				&& (point.x - eps) <= Mathf.Max (src.x, dest.x)
				&& (Mathf.Min (src.y, dest.y) - eps) <= point.y 
				&& (point.y - eps) <= Mathf.Max (src.y, dest.y)) {
				return true;
			}

			return false;
		}

		/// <summary>
		/// 相交
		/// </summary>
		/// <param name="line">Line2d.</param>
		public bool Intersect(Line line)
		{
			Vector2 a = (Vector2)this.src;
			Vector2 b = (Vector2)this.dest;
			Vector2 c = (Vector2)line.src;
			Vector2 d = (Vector2)line.dest;

			if (!(Mathf.Min (a.x, b.x) <= Mathf.Max (c.x, d.x)
				&& Mathf.Min (c.y, d.y) <= Mathf.Max (a.y, b.y)
				&& Mathf.Min (c.x, d.x) <= Mathf.Max (a.x, b.x)
				&& Mathf.Min (a.y, b.y) <= Mathf.Max (c.y, d.y))) {
				return false;
			}

			float u, v, w, z;
			float eps = Constraints.Eps;
			u = (c.x - a.x) * (b.y - a.y) - (b.x - a.x) * (c.y - a.y);
			v = (d.x - a.x) * (b.y - a.y) - (b.x - a.x) * (d.y - a.y);
			w = (a.x - c.x) * (d.y - c.y) - (d.x - c.x) * (a.y - c.y);
			z = (b.x - c.x) * (d.y - c.y) - (d.x - c.x) * (b.y - c.y);
			return (u * v <= eps && w * z <= eps);
		}
	}
}

