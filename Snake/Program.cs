using System.Linq;
using Snake.AI;
using Snake.Game;
using Snake.Renderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AForge.Neuro;
using AForge.Neuro.Learning;
using System.Diagnostics;

namespace Snake
{
	class Program
	{
		static object _lockObjectConsole = new object();
		static object _lockObjectFile = new object();
		const string TrainingFileName = @"AI\TrainingAi";
		private static int trainWorldWidth = 40;
		private static int trainWorldHeigh = 20;
		private static int worldWidth = 40;
		private static int worldHeigh = 20;
		private static int foodPoints = 100;
		private static long learningIteration = 10000000000;
		private static int learningPeers = 350;
		private static int learningPeersRandom = 150;
		//private static int numberOfLearningPaths = 5;
		private static int learningPeersToUseForSuper = 20;


		public static SnakeNeuralAI TrainNeuralAI()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			var SuperNetwork = SnakeNeuralAI.CreateNetwork();

			//Continue training AI
			//SuperNetwork = (ActivationNetwork)ActivationNetwork.Load(@"AI\TrainedNetwork");

			var SuperTeacher = new PerceptronLearning(SuperNetwork) { LearningRate = 0.1 };
			int bestScore = 0;

			IList<SnakeNeuralAiTrainer> bestSnakeAI = new List<SnakeNeuralAiTrainer>();

			for (long i = 0; i < learningIteration / (learningPeers + learningPeersRandom); i++)
			{
				bestSnakeAI.Clear();

				using (MemoryStream stream = new MemoryStream())
				{
					SuperNetwork.Save(stream);

					for (int j = 0; j < learningPeers; j++)
					{
						stream.Seek(0, SeekOrigin.Begin);
						SnakeNeuralAI snakeAI = new SnakeNeuralAI(true, stream);

						if (snakeAI.random.Next(100) < 2)
						{
							snakeAI.ReSeed();
							snakeAI.isMutant = true;
						}

						bestSnakeAI.Add(new SnakeNeuralAiTrainer(snakeAI));

					}

					for (int j = 0; j < learningPeersRandom; j++)
					{
						stream.Seek(0, SeekOrigin.Begin);
						SnakeNeuralAI snakeAI = new SnakeNeuralAI(true, stream);
						snakeAI.Network.Randomize();

						bestSnakeAI.Add(new SnakeNeuralAiTrainer(snakeAI));
					}
				}

				Parallel.ForEach(bestSnakeAI, new ParallelOptions() { MaxDegreeOfParallelism = 6 }, (trainer) =>
				{
					World world = new World(trainWorldWidth, trainWorldHeigh, foodPoints);
					trainer.Train(world);
				});

				bestSnakeAI = bestSnakeAI.OrderByDescending(x => x.Score).Take(learningPeersToUseForSuper).ToList();

				foreach (var bestOfTheBest in bestSnakeAI)
				{
					if (bestOfTheBest.Score > 0)// && bestOfTheBest.Score > bestScore * 0.1)
						bestOfTheBest.AddResults(SuperTeacher);
				}


				bestScore = Math.Max(bestSnakeAI.Max(x => x.Score), bestScore);

				lock (_lockObjectFile)
				{
					SuperNetwork.Save(TrainingFileName);
				}
				lock (_lockObjectConsole)
				{
					Console.SetCursorPosition(0, 0);
					Console.WriteLine("Current average score: " + Math.Round(bestSnakeAI.Average(x => x.Score)) + "    ");
					Console.WriteLine("Current best score: " + bestSnakeAI.Max(x => x.Score) + "     ");
					Console.WriteLine("Learning progress: " + Math.Round((100m / (learningIteration / (learningPeers + learningPeersRandom))) * i) + "% (" + i * (learningPeers + learningPeersRandom) + " games played in " + stopwatch.Elapsed.ToString(@"hh\:mm\:ss") + ")");
				}
			}
			SuperNetwork.Save(@"c:\temp\Network" + bestScore);
			return SnakeNeuralAI.Clone(SuperNetwork, false);
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

				if (File.Exists(TrainingFileName))
					File.Delete(TrainingFileName);
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
					Network = ActivationNetwork.Load(@"AI\TrainedNetwork")
				};
			}

			Console.Clear();


			while (true)
			{
				if (isTraining)
				{
					lock (_lockObjectFile)
					{
						if (!File.Exists(TrainingFileName))
							continue;

						snakeAI = new SnakeNeuralAI()
						{
							Network = ActivationNetwork.Load(TrainingFileName)
						};
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
					Thread.Sleep(Math.Max(0, 0 - (int)stopwatch.ElapsedMilliseconds));


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
