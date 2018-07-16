using System;
using System.Collections.Generic;
using System.Text;

namespace Snake.Game
{
	public class World
	{
		private Random random = new Random(Guid.NewGuid().GetHashCode());

		public int Width { get; set; }
		public int Height { get; set; }

		public Snake Snake { get; set; }
		public Food Food { get; set; }
		public Bonus Bonus { get; set; }

		public int Score { get; set; }

		public World(int width, int height, int foodPoints)
		{
			this.Width = width;
			this.Height = height;
			this.Snake = new Snake(new Location(width / 2, height / 2), Direction.Right);
			this.Food = new Food(foodPoints);
			this.Bonus = new Bonus();
			PlaceFood();
			PlaceBonus();

		}

		public void Update()
		{
			Snake.Update();
			if (!IsClear(Snake.Location))
				Snake.Alive = false;

			if (Snake.Location == Food.Location)
			{
				Score += Food.Points;
				Snake.Grow();
				PlaceFood();
			}
			else
			if (Bonus.Active && Snake.Location == Bonus.Location)
			{
				Bonus.ActiveTick = 0;
				Bonus.Active = false;
				Snake.Shrink();
				PlaceBonus();
			}

			Bonus.Update();

			if (Food.Location == Bonus.Location)
			{
				PlaceBonus();
			}
		}

		private void PlaceFood()
		{
			Food.Location = new Location(random.Next(Width - 2) + 1, random.Next(Height - 2) + 1);
		}

		private void PlaceBonus()
		{
			Bonus.Location = new Location(random.Next(Width - 2) + 1, random.Next(Height - 2) + 1);
		}

		public bool HitWall(Location location)
		{
			if (location.X < 0) //Wall left
				return true;
			if (location.X > Width) //Wall right
				return true;
			if (location.Y < 0) //Wall up
				return true;
			if (location.Y > Height) //Wall down
				return true;

			return false;
		}

		public bool IsClear(Location location)
		{
			if (HitWall(location)) //Wall left
				return false;
			if (Snake.IsHit(location)) //Hit tail
				return false;

			return true;
		}

		public bool IsClearAhead()
		{
			return IsClear(Snake.GetPredictedLocation());
		}

		public bool IsClearAheadAhead()
		{
			return IsClear(Snake.GetPredictedLocation(Snake.GetPredictedLocation()));
		}

		public bool IsClearRight()
		{
			bool isClear;
			Snake.TurnRight();
			isClear = IsClearAhead();
			Snake.TurnLeft();
			return isClear;
		}

		public bool IsClearRightThenAhead()
		{
			bool isClear;
			Snake.TurnRight();
			isClear = IsClearAheadAhead();
			Snake.TurnLeft();
			return isClear;
		}

		public bool IsClearLeft()
		{
			bool isClear;
			Snake.TurnLeft();
			isClear = IsClearAhead();
			Snake.TurnRight();
			return isClear;
		}

		public bool IsClearLeftThenAhead()
		{
			bool isClear;
			Snake.TurnLeft();
			isClear = IsClearAheadAhead();
			Snake.TurnRight();
			return isClear;
		}

		/*public bool IsFoodStraightAhead()
		{
			Location newLocation = Snake.Location;

			while (!HitWall(newLocation))
			{
				newLocation = Snake.GetPredictedLocation(newLocation);
				if (newLocation == Food.Location)
					return true;
			}

			return false;
		}*/

		public bool IsFoodStraightAhead()
		{
			if (Food.Location.Y < Snake.Location.Y && Snake.Direction == Direction.Up)
				return true;
			if (Food.Location.Y > Snake.Location.Y && Snake.Direction == Direction.Down)
				return true;
			if (Food.Location.X < Snake.Location.X && Snake.Direction == Direction.Left)
				return true;
			if (Food.Location.X > Snake.Location.X && Snake.Direction == Direction.Right)
				return true;

			return false;
		}


		public bool IsFoodToTheRight()
		{
			if (Food.Location.X > Snake.Location.X && Snake.Direction == Direction.Up)
				return true;
			if (Food.Location.X < Snake.Location.X && Snake.Direction == Direction.Down)
				return true;
			if (Food.Location.Y < Snake.Location.Y && Snake.Direction == Direction.Left)
				return true;
			if (Food.Location.Y > Snake.Location.Y && Snake.Direction == Direction.Right)
				return true;

			return false;
		}

		public bool IsFoodToTheLeft()
		{
			if (Food.Location.X < Snake.Location.X && Snake.Direction == Direction.Up)
				return true;
			if (Food.Location.X > Snake.Location.X && Snake.Direction == Direction.Down)
				return true;
			if (Food.Location.Y > Snake.Location.Y && Snake.Direction == Direction.Left)
				return true;
			if (Food.Location.Y < Snake.Location.Y && Snake.Direction == Direction.Right)
				return true;

			return false;
		}

		public bool IsFoodBehind()
		{
			if (Food.Location.Y < Snake.Location.Y && Snake.Direction == Direction.Up)
				return true;
			if (Food.Location.Y > Snake.Location.Y && Snake.Direction == Direction.Down)
				return true;
			if (Food.Location.X > Snake.Location.X && Snake.Direction == Direction.Left)
				return true;
			if (Food.Location.X < Snake.Location.X && Snake.Direction == Direction.Right)
				return true;

			return false;
		}

		/*public bool IsBonusStraightAhead()
		{
			if (!Bonus.Active)
				return false;

			Location newLocation = Snake.Location;

			while (!HitWall(newLocation))
			{
				newLocation = Snake.GetPredictedLocation(newLocation);
				if (newLocation == Bonus.Location)
					return true;
			}

			return false;
		}*/

		public bool IsBonusStraightAhead()
		{
			if (!Bonus.Active)
				return false;

			if (Bonus.Location.Y < Snake.Location.Y && Snake.Direction == Direction.Up)
				return true;
			if (Bonus.Location.Y > Snake.Location.Y && Snake.Direction == Direction.Down)
				return true;
			if (Bonus.Location.X < Snake.Location.X && Snake.Direction == Direction.Left)
				return true;
			if (Bonus.Location.X > Snake.Location.X && Snake.Direction == Direction.Right)
				return true;

			return false;
		}

		public bool IsBonusToTheRight()
		{
			if (!Bonus.Active)
				return false;

			if (Bonus.Location.X > Snake.Location.X && Snake.Direction == Direction.Up)
				return true;
			if (Bonus.Location.X < Snake.Location.X && Snake.Direction == Direction.Down)
				return true;
			if (Bonus.Location.Y < Snake.Location.Y && Snake.Direction == Direction.Left)
				return true;
			if (Bonus.Location.Y > Snake.Location.Y && Snake.Direction == Direction.Right)
				return true;

			return false;
		}

		public bool IsBonusToTheLeft()
		{
			if (!Bonus.Active)
				return false;

			if (Bonus.Location.X < Snake.Location.X && Snake.Direction == Direction.Up)
				return true;
			if (Bonus.Location.X > Snake.Location.X && Snake.Direction == Direction.Down)
				return true;
			if (Bonus.Location.Y > Snake.Location.Y && Snake.Direction == Direction.Left)
				return true;
			if (Bonus.Location.Y < Snake.Location.Y && Snake.Direction == Direction.Right)
				return true;

			return false;
		}

	}
}