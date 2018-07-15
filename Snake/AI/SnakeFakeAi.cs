using Snake.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snake.AI
{
	public class SnakeFakeAI : SnakeAI
	{
		public override void MakeDecision(World world)
		{
			ApplyDecision(world, GetDecision(world));
			world.Update();
		}

		public Decision GetDecision(World world)
		{
			if (!world.IsClearAhead())
			{
				if (world.IsFoodToTheLeft() && world.IsClearLeft())
				{
					return Decision.TurnLeft;
				}
				if (world.IsFoodToTheRight() && world.IsClearRight())
				{
					return Decision.TurnRight;
				}
				if (world.IsClearLeft())
				{
					return Decision.TurnLeft;
				}
				if (world.IsClearRight())
				{
					return Decision.TurnRight;
				}
			}
			else
			{
				if (world.IsFoodStraightAhead())
				{
					return Decision.Continue;
				}
				else
				{
					if (world.IsFoodToTheLeft() && world.IsClearLeft())
					{
						return Decision.TurnLeft;
					}
					if (world.IsFoodToTheRight() && world.IsClearRight())
					{
						return Decision.TurnRight;
					}
				}
			}

			for (int i = 0; i < 100; i++)
			{
				Decision randomDecision = MakeRandomDecision();
				if (randomDecision == Decision.TurnLeft && world.IsClear(world.Snake.GetPredictedLocation(world.Snake.GetTurnLeftDirection())))
					return randomDecision;
				if (randomDecision == Decision.TurnRight && world.IsClear(world.Snake.GetPredictedLocation(world.Snake.GetTurnRightDirection())))
					return randomDecision;
				if (randomDecision == Decision.Continue && world.IsClear(world.Snake.GetPredictedLocation()))
					return randomDecision;
			}

			//Give up...
			return MakeRandomDecision();
		}
	}
}
