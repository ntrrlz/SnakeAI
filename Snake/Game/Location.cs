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
			// If parameter is null, return false.
			if (Object.ReferenceEquals(p, null))
			{
				return false;
			}

			// Optimization for a common success case.
			if (Object.ReferenceEquals(this, p))
			{
				return true;
			}

			// If run-time types are not exactly the same, return false.
			if (this.GetType() != p.GetType())
			{
				return false;
			}

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
			// Check for null on left side.
			if (Object.ReferenceEquals(lhs, null))
			{
				if (Object.ReferenceEquals(rhs, null))
				{
					// null == null = true.
					return true;
				}

				// Only the left side is null.
				return false;
			}
			// Equals handles case of null on right side.
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Location lhs, Location rhs)
		{
			return !(lhs == rhs);
		}
	}
}
