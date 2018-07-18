using System.Linq;
using Snake.AI;
using Snake.Game;
using Snake.Renderer;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using System.Diagnostics;

namespace Snake
{
	class Program
	{
		static object _lockObjectConsole = new object();
		static object _lockObjectFile = new object();
		const string TrainingFileName = @"AI\TrainingAi";
		private static int trainWorldWidth = 20;
		private static int trainWorldHeigh = 10;
		private static int worldWidth = 20;
		private static int worldHeigh = 10;
		private static int foodPoints = 100;
		private static long learningIteration = 10000000000;
		private static int learningPeers = 100;
		private static int learningPeersMutants = 300;
		//private static int numberOfLearningPaths = 5;
		private static int parallelNetworks = 40;

		private static NeuralNetwork SuperNetwork = SnakeNeuralAI.CreateNetwork();

		public static void TrainNeuralAI()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			

			//Continue training AI
			//SuperNetwork = (ActivationNetwork)ActivationNetwork.Load(@"AI\TrainedNetwork");

			int bestScore = 0;

			IList<SnakeNeuralAiTrainer> students = new List<SnakeNeuralAiTrainer>();
			IList<SnakeNeuralAiTrainer> bestSnakeAIs = new List<SnakeNeuralAiTrainer>();

			for (long i = 0; i < learningIteration / (learningPeers + learningPeersMutants); i++)
			{
				students.Clear();

				for (int j = 0; j < learningPeers + learningPeersMutants; j++)
				{
					bool learnMode = j < learningPeersMutants;
					SnakeNeuralAI snakeAI;
					if (bestSnakeAIs.Count == parallelNetworks)
						snakeAI = SnakeNeuralAI.Clone(bestSnakeAIs[j % parallelNetworks].SnakeNeuralAI.Network, learnMode);
					else
						snakeAI = SnakeNeuralAI.Clone(SuperNetwork, learnMode);

					students.Add(new SnakeNeuralAiTrainer(snakeAI));

				}

				Parallel.ForEach(students, new ParallelOptions() { MaxDegreeOfParallelism = 6 }, (trainer) =>
				{
					World world = new World(trainWorldWidth, trainWorldHeigh, foodPoints);
					trainer.Train(world);
				});

				bestSnakeAIs = students.OrderByDescending(x => x.Score).Take(parallelNetworks).ToList();

				lock (_lockObjectFile)
				{
					if (bestSnakeAIs.First().Score > 0)// && bestOfTheBest.Score > bestScore * 0.1)
						SuperNetwork = bestSnakeAIs.First().SnakeNeuralAI.Network;
				}

				bestScore = Math.Max(bestSnakeAIs.Max(x => x.Score), bestScore);

				SuperNetwork.Save(TrainingFileName);

				lock (_lockObjectConsole)
				{
					Console.SetCursorPosition(0, 0);
					Console.WriteLine("Current average score: " + Math.Round(bestSnakeAIs.Average(x => x.Score)) + "    ");
					Console.WriteLine("Current best score: " + bestSnakeAIs.Max(x => x.Score) + "     ");
					Console.WriteLine("Learning progress: " + Math.Round((100m / (learningIteration / (learningPeers + learningPeersMutants))) * i) + "% (" + i * (learningPeers + learningPeersMutants) + " games played in " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss") + ")");
				}
			}
		}

		static void Main(string[] args)
		{
			Console.CursorVisible = false;
			Console.WriteLine("Press 1 to train new neural network");
			Console.WriteLine("Press 2 start snake with hardcoded AI");
			Console.WriteLine("Press 3 to load trained network");

			var key = Console.ReadKey();

			bool isTraining = false;

			SnakeAI snakeAI = null;
			if (key.KeyChar == '1')
			{
				isTraining = true;

				new Thread(() =>
				{
					Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
					TrainNeuralAI();
				}).Start();
			}
			else
			if (key.KeyChar == '2')
			{
				snakeAI = new SnakeFakeAI();
			}
			else
			{
				snakeAI = new SnakeNeuralAI()
				{
					Network = NeuralNetwork.Load(@"AI\TrainedNetwork")
				};
			}

			Console.Clear();


			while (true)
			{
				if (isTraining)
				{
					lock (_lockObjectFile)
					{
						snakeAI = SnakeNeuralAI.Clone(SuperNetwork, false);
					}
				}

				World world = new World(worldWidth, worldHeigh, foodPoints);

				Stopwatch stopwatch = new Stopwatch();
				while (world.Snake.Alive)
				{
					stopwatch.Restart();
					snakeAI.MakeDecision(world);

					lock (_lockObjectConsole)
					{
						ConsoleRenderer.Render(world);
					}
					Thread.Sleep(Math.Max(0, 50 - (int)stopwatch.ElapsedMilliseconds));


					if (Console.KeyAvailable)
					{
						Console.ReadKey();
						break;
					}
				}

				lock (_lockObjectConsole)
				{
					ConsoleRenderer.Render(world);
				}
				Thread.Sleep(2000);
			}
		}
	}
}
