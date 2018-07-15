using Snake.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace Snake.Renderer
{
    public static class ConsoleRenderer
    {
		public static void Render(World world)
		{
			Console.SetCursorPosition(0, 3);
			Console.WriteLine();

			if (!world.Snake.Alive)
				Console.WriteLine(world.Score + " GAME OVER  ");
			else
				Console.WriteLine(world.Score + "              ");

			for (int y = -1; y < world.Height + 2; y++)
			{
				Console.WriteLine();

				for (int x = -1; x < world.Width + 2; x++)
				{
					if (y == -1 || y == world.Height + 1)
					{
						Console.Write("+");
						continue;
					}

					if (x == -1 || x == world.Width + 1)
					{
						Console.Write('+');
						continue;
					}

					if (x == world.Food.Location.X && y == world.Food.Location.Y)
					{
						Console.Write('*');
						continue;
					}

					if (world.Bonus.Active && x == world.Bonus.Location.X && y == world.Bonus.Location.Y)
					{
						Console.Write('?');
						continue;
					}

					if (x == world.Snake.Location.X && y == world.Snake.Location.Y)
					{
						Console.Write('@');
						continue;
					}
					bool drawn = false;
					foreach(var tail in world.Snake.Tail.ToArray())
						if (x == tail.X && y == tail.Y)
						{
							Console.Write('█');
							drawn = true;
							break;
						}

					if (drawn)
						continue;

					Console.Write(' ');
				}
			}
		}
    }
}
