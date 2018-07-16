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

			StringBuilder render = new StringBuilder();
			render.AppendLine();

			if (!world.Snake.Alive)
				render.AppendLine(world.Score + " GAME OVER  ");
			else
				render.AppendLine(world.Score + "              ");

			for (int y = -1; y < world.Height + 2; y++)
			{
				render.AppendLine();

				for (int x = -1; x < world.Width + 2; x++)
				{
					if (y == -1 || y == world.Height + 1)
					{
						render.Append("+");
						continue;
					}

					if (x == -1 || x == world.Width + 1)
					{
						render.Append('+');
						continue;
					}

					if (x == world.Food.Location.X && y == world.Food.Location.Y)
					{
						render.Append('*');
						continue;
					}

					if (world.Bonus.Active && x == world.Bonus.Location.X && y == world.Bonus.Location.Y)
					{
						render.Append('?');
						continue;
					}

					if (x == world.Snake.Location.X && y == world.Snake.Location.Y)
					{
						render.Append('@');
						continue;
					}
					bool drawn = false;
					foreach(var tail in world.Snake.Tail.ToArray())
						if (x == tail.X && y == tail.Y)
						{
							render.Append('█');
							drawn = true;
							break;
						}

					if (drawn)
						continue;

					render.Append(' ');
				}

			}
			Console.Write(render.ToString());
		}
	}
}
