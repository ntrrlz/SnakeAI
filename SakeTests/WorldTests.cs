using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snake.Game;

namespace SakeTests
{
	[TestClass]
	public class WorldTests
	{
		[TestMethod]
		public void FoodPositionLeftTest()
		{
			World world = new World(20, 20, 100);
			world.Food.Location = new Location(10, 10);
			world.Snake.Location = new Location(20, 10);
			world.Snake.Direction = Direction.Up;

			Assert.IsTrue(world.IsFoodToTheLeft());
			Assert.IsFalse(world.IsFoodStraightAhead());
			Assert.IsFalse(world.IsFoodToTheRight());
		}

		[TestMethod]
		public void FoodPositionRightTest()
		{
			World world = new World(20, 20, 100);
			world.Food.Location = new Location(20, 10);
			world.Snake.Location = new Location(10, 10);
			world.Snake.Direction = Direction.Up;

			Assert.IsTrue(world.IsFoodToTheRight());
			Assert.IsFalse(world.IsFoodStraightAhead());
			Assert.IsFalse(world.IsFoodToTheLeft());
		}

		[TestMethod]
		public void FoodPositionLeftWhenDirectionLeftTest()
		{
			World world = new World(20, 20, 100);
			world.Food.Location = new Location(10, 10);
			world.Snake.Location = new Location(20, 10);
			world.Snake.Direction = Direction.Left;

			Assert.IsFalse(world.IsFoodToTheLeft());
			Assert.IsTrue(world.IsFoodStraightAhead());
			Assert.IsFalse(world.IsFoodToTheRight());
		}

		[TestMethod]
		public void SnakedangerAheadTest()
		{
			World world = new World(20, 20, 100);
			world.Update();
			world.Snake.TurnLeft();
			world.Update();
			world.Snake.TurnLeft();
			world.Update();
			world.Snake.TurnLeft();
			Assert.IsFalse(world.IsClearAhead());
			world.Update();


			Assert.IsFalse(world.Snake.Alive);
		}

		[TestMethod]
		public void SnakedangerAheadAheadTest()
		{
			World world = new World(20, 20, 100);
			world.Snake.Grow();
			world.Snake.Grow();
			world.Snake.Grow();
			world.Update();
			world.Snake.TurnLeft();
			world.Update();
			world.Update();
			world.Snake.TurnLeft();
			world.Update();
			world.Snake.TurnLeft();
			Assert.IsTrue(world.IsClearAhead());
			Assert.IsFalse(world.IsClearAheadAhead());
			world.Update();
			Assert.IsTrue(world.Snake.Alive);
			world.Update();
			Assert.IsFalse(world.Snake.Alive);
		}
	}
}
