using System;
using System.Collections.Generic;
using System.Linq;

namespace Snake
{
	internal class Program
	{
		private const int SLEEP_TIME = 150;
		private const int COLS_NUMBER = 40;
		private const int ROWS_NUMBER = 20;

		private enum GAME_STATE { WIN, LOSE, IN_PROGRESS }

		private class SnakeNode
		{
			public static SnakeNode head;
			public int x;
			public int y;
		}

		private struct Vector2
		{
			public int x;
			public int y;

			public Vector2(int x, int y) { this.x = x; this.y = y; }
		}

		static void Main(string[] args)
		{
			Console.CursorVisible = false;

			Program program = new Program();
			while (program.Run()) 
			{
				var key = Console.ReadKey().Key;
				if (key == ConsoleKey.Escape)
				{
					Console.SetCursorPosition(0, ROWS_NUMBER + 1);
					return;
				}
				if (key == ConsoleKey.R) 
				{
					continue; 
				}
			};
		}

		private bool Run()
		{
			char[][] board = new char[ROWS_NUMBER][];
			for (int i = 0; i < board.Length; i++)
			{
				board[i] = new char[COLS_NUMBER];
				for (int j = 0; j < COLS_NUMBER; j++)
				{
					board[i][j] = ' ';
				}
			}

			board[0][0] = '┌';
			board[0][COLS_NUMBER - 1] = '┐';
			board[ROWS_NUMBER - 1][0] = '└';
			board[ROWS_NUMBER - 1][COLS_NUMBER - 1] = '┘';

			DrawHorizontalWalls(board, 0);
			DrawHorizontalWalls(board, ROWS_NUMBER - 1);
			DrawVerticalWalls(board, 0);
			DrawVerticalWalls(board, COLS_NUMBER - 1);

			List<SnakeNode> snake = new List<SnakeNode>() { new SnakeNode() { x = 4, y = 5 }, new SnakeNode() { x = 5, y = 5 }, new SnakeNode() { x = 6, y = 5 } };
			SnakeNode.head = snake.Last();

			int maxSnakeSize = (COLS_NUMBER - 2) * (ROWS_NUMBER - 2);
			GAME_STATE state = GAME_STATE.IN_PROGRESS;

			Vector2 direction = new Vector2(1, 0);
			Vector2 fruitPosition = SpawnFruit();
			int fruitsPicked = 0;

			while (true)
			{
				if (Console.KeyAvailable) { direction = GetDirection(Console.ReadKey().Key, direction); }

				MoveSnake(snake, direction);

				if (!IntersectsWithBoard(snake) || IntersectsWithSelf(snake))
				{
					state = GAME_STATE.LOSE;
					break;
				}

				if (IntersectsWithFruit(snake, fruitPosition))
				{
					fruitsPicked++;

					SnakeNode.head = new SnakeNode() { x = SnakeNode.head.x + direction.x, y = SnakeNode.head.y + direction.y };
					snake.Add(SnakeNode.head);

					if (snake.Count == maxSnakeSize) { state = GAME_STATE.WIN; break; }

					fruitPosition = SpawnFruit();
				}

				DrawBoard(board, fruitsPicked);
				DrawFruit(fruitPosition);
				DrawSnake(snake);

				System.Threading.Thread.Sleep(SLEEP_TIME);
			}

			DrawEndMessage(state);

			return true;
		}

		private void DrawEndMessage(GAME_STATE state)
		{
			string message = state == GAME_STATE.LOSE ? "Game over" : "Game won";
			int gameOverPos = (COLS_NUMBER - message.Length) / 2;
			Console.SetCursorPosition(gameOverPos, 9);
			Console.WriteLine(message);

			string message1 = "Press 'r' to retry or 'esc' to exit";
			int coordX = (COLS_NUMBER - message1.Length) / 2;
			Console.SetCursorPosition(coordX, 10);
			Console.WriteLine(message1);
		}

		private void DrawHorizontalWalls(char[][] board, int index)
		{
			for (int i = 1; i < board[0].Length - 1; i++)
			{
				board[index][i] = '─';
			}
		}

		private void DrawVerticalWalls(char[][] board, int index)
		{
			for (int i = 1; i < board.Length - 1; i++)
			{
				board[i][index] = '│';
			}
		}

		private bool IntersectsWithSelf(List<SnakeNode> snake)
		{
			for (int i = 0; i < snake.Count - 2; i++)
			{
				if (snake[i].x == SnakeNode.head.x && snake[i].y == SnakeNode.head.y)
				{
					return true;
				}
			}

			return false;
		}

		private bool IntersectsWithBoard(List<SnakeNode> snake)
		{
			return (SnakeNode.head.x < COLS_NUMBER - 1 && SnakeNode.head.x > 0) && (SnakeNode.head.y > 0 && SnakeNode.head.y < ROWS_NUMBER - 1);
		}

		private bool IntersectsWithFruit(List<SnakeNode> snake, Vector2 fruitPosition)
		{
			return SnakeNode.head.x == fruitPosition.x && SnakeNode.head.y == fruitPosition.y;
		}

		private void DrawFruit(Vector2 position)
		{
			Console.SetCursorPosition(position.x, position.y);
			Console.Write("@");
		}

		private void DrawBoard(char[][] board, int counter)
		{
			Console.SetCursorPosition(0, 0);
			foreach (var boardItem in board)
			{
				Console.WriteLine(boardItem);
			}

			Console.WriteLine($"Score: {counter}");
		}

		private void DrawSnake(List<SnakeNode> snake)
		{
			foreach (var snakeNode in snake)
			{
				Console.SetCursorPosition(snakeNode.x, snakeNode.y);
				Console.Write('#');
			}
		}

		private void MoveSnake(List<SnakeNode> snake, Vector2 direction)
		{
			var oldX = SnakeNode.head.x;
			var oldY = SnakeNode.head.y;

			SnakeNode.head.x += 1 * direction.x;
			SnakeNode.head.y += 1 * direction.y;

			for (int i = snake.Count - 2; i >= 0; i--)
			{
				var tempX = snake[i].x;
				var tempY = snake[i].y;

				snake[i].x = oldX;
				snake[i].y = oldY;

				oldX = tempX;
				oldY = tempY;
			}
		}

		private Vector2 GetDirection(ConsoleKey key, Vector2 currentDirection)
		{
			if (key == ConsoleKey.UpArrow && currentDirection.y != 1)
			{
				currentDirection = new Vector2(0, -1);
			}
			else if (key == ConsoleKey.RightArrow && currentDirection.x != -1)
			{
				currentDirection = new Vector2(1, 0);
			}
			else if (key == ConsoleKey.DownArrow && currentDirection.y != -1)
			{
				currentDirection = new Vector2(0, 1);
			}
			else if (key == ConsoleKey.LeftArrow && currentDirection.x != 1)
			{
				currentDirection = new Vector2(-1, 0);
			}

			return currentDirection;
		}

		private Vector2 SpawnFruit()
		{
			Random random = new Random();
			int xPos = random.Next(1, 38);
			int yPos = random.Next(1, 19);

			return new Vector2() { x = xPos, y = yPos };
		}
	}
}
