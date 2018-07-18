using Snake.Game;
using System;
using System.Collections.Generic;
using System.IO;

namespace Snake.AI
{
	public class SnakeNeuralAI : SnakeAI
	{
		public NeuralNetwork Network;
		public bool isLearning;

		public SnakeNeuralAI Clone(bool learn)
		{
			return Clone(this.Network, learn);
		}


		public static SnakeNeuralAI Clone(NeuralNetwork network, bool learn)
		{
			SnakeNeuralAI clone = new SnakeNeuralAI(learn, new NeuralNetwork(network));

			if (learn)
				clone.Network.Mutate();
			return clone;
		}

		public SnakeNeuralAI(bool learn = false, NeuralNetwork exisingNetwork = null)
		{
			this.isLearning = learn;

			if (exisingNetwork != null)
				Network = exisingNetwork;
			else
				Network = CreateNetwork();
		}

		public static NeuralNetwork CreateNetwork()
		{
			return new NeuralNetwork(new int[] { 13, 11, 3 });
		}

		public override void MakeDecision(World world)
		{
			double[] input = new double[]
			{
				world.IsClearAhead() ? 0 : 1,
				world.IsClearAheadAhead() ? 0 : 1,
				world.IsClearLeft() ? 0 : 1,
				world.IsClearLeftThenAhead() ? 0 : 1,
				world.IsClearRight() ? 0 : 1,
				world.IsClearRightThenAhead() ? 0 : 1,
				world.IsFoodStraightAhead() ? 0 : 1,
				world.IsFoodToTheLeft() ? 0 : 1,
				world.IsFoodToTheRight() ? 0 : 1,
				world.IsBonusStraightAhead() ? 0 : 1,
				world.IsBonusToTheLeft() ? 0 : 1,
				world.IsBonusToTheRight() ? 0 : 1,
				//world.Snake.Tail.Count > 10 ? 0 : 1,
				((double)world.Snake.Direction) / 4
			};

			if (isLearning)
			{
				//Network.Mutate();
			}

			double[] output = Network.FeedForward(input);

			Decision finalDecision = ConvertOutputToDecision(output);				

			ApplyDecision(world, finalDecision);
			world.Update();
		}

		public static Decision ConvertOutputToDecision(double[] output)
		{
			double continueScore = output[0];
			double turnLeftScore = output[1];
			double turnRightScore = output[2];

			Decision finalDecision = Decision.Continue;
			if (turnLeftScore > continueScore)
				finalDecision = Decision.TurnLeft;
			if (turnRightScore > turnLeftScore && turnRightScore > continueScore)
				finalDecision = Decision.TurnRight;
			return finalDecision;
		}
	}
}