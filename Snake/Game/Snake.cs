using System;
using System.Collections.Generic;
using System.Text;

namespace Snake.Game
{
    public class Snake
    {
		const int StartTailSize = 4;
		public Location Location { get; set; }
		public Queue<Location> Tail { get; set; }
		private int TailSize;

		public Direction Direction { get; set; }
		public bool Alive { get; set; }

		public Snake(Location location, Direction direction)
		{
			this.Location = location;
			this.Direction = direction;
			this.Alive = true;
			this.TailSize = StartTailSize;
			this.Tail = new Queue<Location>();
		}

		public void Grow()
		{
			this.TailSize++;
		}

		public void Shrink()
		{
			this.TailSize = StartTailSize;
		}

		public void Update()
		{
			Tail.Enqueue(this.Location);

			this.Location = GetPredictedLocation();

			while (Tail.Count > TailSize)
				Tail.Dequeue();
		}

		public void TurnLeft()
		{
			this.Direction = GetTurnLeftDirection();
		}

		public void TurnRight()
		{
			this.Direction = GetTurnRightDirection();
		}

		public bool IsHit(Location location)
		{
			foreach(Location tailLocation in Tail.ToArray())
			{
				if (tailLocation == location)
					return true;
			}

			return false;
		}

		public Location GetPredictedLocation()
		{
			return GetPredictedLocation(this.Location, this.Direction);
		}

		public Location GetPredictedLocation(Direction direction)
		{
			return GetPredictedLocation(this.Location, direction);
		}

		public Location GetPredictedLocation(Location location)
		{
			return GetPredictedLocation(location, this.Direction);
		}

		public static Location GetPredictedLocation(Location location, Direction direction)
		{
			switch(direction)
			{
				case Direction.Down:
					return new Location(location.X, location.Y+1);
				case Direction.Up:
					return new Location(location.X, location.Y-1);
				case Direction.Left:
					return new Location(location.X-1, location.Y);
				case Direction.Right:
					return new Location(location.X+1, location.Y);
				default:
					throw new Exception("Unknown direction");
			}
		}

		public Direction GetTurnLeftDirection()
		{
			switch (this.Direction)
			{
				case Direction.Down:
					return Direction.Right;
				case Direction.Up:
					return Direction.Left;
				case Direction.Left:
					return Direction.Down;
				case Direction.Right:
					return Direction.Up;
				default:
					throw new Exception("Unknown direction");
			}
		}

		public Direction GetTurnRightDirection()
		{
			switch (this.Direction)
			{
				case Direction.Down:
					return Direction.Left;
				case Direction.Up:
					return Direction.Right;
				case Direction.Left:
					return Direction.Up;
				case Direction.Right:
					return Direction.Down;
				default:
					throw new Exception("Unknown direction");
			}
		}
	}
}
