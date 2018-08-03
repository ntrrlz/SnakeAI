using System;
using System.Collections.Generic;
using System.Text;

namespace Snake.Game
{
	public struct Location
	{
		public Location(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public int X { get; }
		public int Y { get; }

		public double Distance(Location location)
		{
			var dx = location.X - X;
			var dy = location.Y - Y;

			return Math.Sqrt(dx * dx + dy * dy);
		}

		public override bool Equals(object obj)
		{
			return this.Equals((Location)obj);
		}

		public bool Equals(Location p)
		{

			// Return true if the fields match.
			// Note that the base class is not invoked because it is
			// System.Object, which defines Equals as reference equality.
			return (X == p.X) && (Y == p.Y);
		}

		public override int GetHashCode()
		{
			return X * 0x00010000 + Y;
		}

		public static bool operator ==(Location lhs, Location rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Location lhs, Location rhs)
		{
			return !(lhs == rhs);
		}
	}
}
