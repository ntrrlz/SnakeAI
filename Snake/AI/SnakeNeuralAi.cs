using AForge.Neuro;
using AForge.Neuro.Learning;
using Snake.Game;
using System;
using System.Collections.Generic;
using System.IO;

namespace Snake.AI
{
	public class SnakeNeuralAI : SnakeAI
	{
		public IList<Tuple<double[], double[]>> Runs = new List<Tuple<double[], double[]>>();

		public Network Network;
		public bool isLearning;
		public bool isMutant;

		public SnakeNeuralAI Clone(bool learn)
		{
			return Clone((ActivationNetwork)this.Network, learn);
		}


		public static SnakeNeuralAI Clone(ActivationNetwork network, bool learn)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				network.Save(stream);
				stream.Seek(0, SeekOrigin.Begin);
				SnakeNeuralAI clone = new SnakeNeuralAI(learn, stream);

				return clone;
			}
		}

		public SnakeNeuralAI(bool learn = false, Stream loadFromStream = null)
		{
			this.isLearning = learn;

			if (loadFromStream != null)
				Network = (ActivationNetwork)ActivationNetwork.Load(loadFromStream);
			else
				Network = CreateNetwork();
		}

		public static ActivationNetwork CreateNetwork()
		{
			return new ActivationNetwork(new BipolarSigmoidFunction(), 13, 3);
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
				world.Snake.Tail.Count > 10 ? 0 : 1
			};


			double[] output = Network.Compute(input);

			if (isLearning && isMutant && random.Next(100) < 2)
			{
				ReSeed();
 				output = new double[]
				{
					random.NextDouble(),
					random.NextDouble(),
					random.NextDouble()
				};
			}

			Decision finalDecision = ConvertOutputToDecision(output);				

			ApplyDecision(world, finalDecision);
			world.Update();

			if (world.Snake.Alive)
				Runs.Add(new Tuple<double[], double[]>(input, output));
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