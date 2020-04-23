using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Media;
using System.IO;

namespace Snake
{
    //stores the position of objects on the console
    struct Position
    {
        public int row;
        public int col;
        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }

    class Program
    {
        static void PlayMusic()
        {
            System.Media.SoundPlayer bgm = new System.Media.SoundPlayer();
            bgm.SoundLocation = "../../bgm.wav";
            bgm.PlayLooping();
        }

        static void Main(string[] args)
        {
            PlayMusic();
            byte right = 0;
            byte left = 1;
            byte down = 2;
            byte up = 3;
            //means the last time the snakeElement ate
            int lastFoodTime = 0;
            //the time till the food spawns again
            int foodDissapearTime = 16000;
            int userPoints = 0;
            int negativePoints = 0;

            //array storing the direction of snake movement
            Position[] directions = new Position[]
            {
                new Position(0, 1), // right
                new Position(0, -1), // left
                new Position(1, 0), // down
                new Position(-1, 0), // up
            };

            double sleepTime = 100;
            int direction = right;
            //random number generator
            Random randomNumbersGenerator = new Random();
            //makes the number of rows that can be accessed on a console equal to the height of the window
            Console.BufferHeight = Console.WindowHeight;
            //store the last time the snake ate to time since the console started
            lastFoodTime = Environment.TickCount;

            //create Position objects and stores them in obstacles
            //These represent the obstacles
            List<Position> obstacles = new List<Position>()
            {
                //-1 prevents the obstacle from spawaning on the userpoints display
                new Position(randomNumbersGenerator.Next(0, Console.WindowHeight - 1),
                            randomNumbersGenerator.Next(0, Console.WindowWidth)),
                new Position(randomNumbersGenerator.Next(0, Console.WindowHeight - 1),
                            randomNumbersGenerator.Next(0, Console.WindowWidth)),
                new Position(randomNumbersGenerator.Next(0, Console.WindowHeight - 1),
                            randomNumbersGenerator.Next(0, Console.WindowWidth)),
                new Position(randomNumbersGenerator.Next(0, Console.WindowHeight - 1),
                            randomNumbersGenerator.Next(0, Console.WindowWidth)),
                new Position(randomNumbersGenerator.Next(0, Console.WindowHeight - 1),
                            randomNumbersGenerator.Next(0, Console.WindowWidth)),
            };
            //This draws the obstacles on the screen
            foreach (Position obstacle in obstacles)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.SetCursorPosition(obstacle.col, obstacle.row);
                Console.Write("=");
            }

            //Creates the snake using a queue data structure of length 3
            //Queue operates as a first-in first-out array
            Queue<Position> snakeElements = new Queue<Position>();
            //sets the length of snake equal to 3
            for (int i = 0; i <= 3; i++)
            {
                snakeElements.Enqueue(new Position(0, i));
            }
            //creates the food position that is randomly generated as long as the snake has not eaten the food
            //or the food was generated in place of an obstacle
            Position food;
            do
            {
                food = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight - 1),
                    randomNumbersGenerator.Next(0, Console.WindowWidth));
            }
            while (snakeElements.Contains(food) || obstacles.Contains(food));

            //Draws the food on the console
            Console.SetCursorPosition(food.col, food.row);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("@");

            //Draws the snake on the console
            foreach (Position position in snakeElements)
            {
                Console.SetCursorPosition(position.col, position.row);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("*");
            }

            //main game loop
            while (true)
            {
                negativePoints++;
                //checks if user can input values through keyboard     
                if (Console.KeyAvailable)
                {
                    //The controls of the snake
                    ConsoleKeyInfo userInput = Console.ReadKey();
                    if (userInput.Key == ConsoleKey.LeftArrow)
                    {
                        if (direction != right) direction = left;
                    }
                    if (userInput.Key == ConsoleKey.RightArrow)
                    {
                        if (direction != left) direction = right;
                    }
                    if (userInput.Key == ConsoleKey.UpArrow)
                    {
                        if (direction != down) direction = up;
                    }
                    if (userInput.Key == ConsoleKey.DownArrow)
                    {
                        if (direction != up) direction = down;
                    }
                }
                //return the last element in the snakebody
                Position snakeHead = snakeElements.Last();
                //sets the direction the snake will move
                Position nextDirection = directions[direction];

                //changes the direction of the snakehead when the snake direction changes
                Position snakeNewHead = new Position(snakeHead.row + nextDirection.row,
                    snakeHead.col + nextDirection.col);

                //allows the snake to exit the window and enter at the opposite side
                if (snakeNewHead.col < 0) snakeNewHead.col = Console.WindowWidth - 1;
                if (snakeNewHead.row < 0) snakeNewHead.row = Console.WindowHeight - 1;
                if (snakeNewHead.row >= Console.WindowHeight) snakeNewHead.row = 0;
                if (snakeNewHead.col >= Console.WindowWidth) snakeNewHead.col = 0;

                //user points calculation
                userPoints = (snakeElements.Count - 6) * 100 - negativePoints;
                if (userPoints < 0) userPoints = 0;
                userPoints = Math.Max(userPoints, 0);
                //displays points while playing game
                string displaypoints = $" Points:{userPoints}";
                Console.SetCursorPosition(Console.WindowWidth - displaypoints.Length, 0);
                Console.WriteLine(displaypoints);
                //checks snake collision with obstacles and ends the game
                if (snakeElements.Contains(snakeNewHead) || obstacles.Contains(snakeNewHead))
                {
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.Red;
					Console.SetCursorPosition((Console.WindowWidth - 1) / 2, (Console.WindowHeight - 1) / 2);
                    Console.WriteLine("Game over!");
                    //6 and not 5 because we enqueue snakeNewHead and dont dequeue snakeHead
                    string points = $"Your points are: {userPoints}";
                    Console.WriteLine(points);
					Console.ReadLine();
                    using (StreamWriter sw = File.CreateText("..\\..\\user.txt"))
                    {
                        sw.WriteLine(points);
                    }

                    return;
                }
                //winning game logic
                if (userPoints >= 500)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Congratulations!!! You Win !!!");
                    string points = $"Your points are: {userPoints}";
                    Console.WriteLine(points);
                    using (StreamWriter sw = File.CreateText("..\\.."))
                    {
                        sw.WriteLine(points);
                    }
                    return;
                }

                //sets the last element in the queue to be *
                Console.SetCursorPosition(snakeHead.col, snakeHead.row);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("*");

                //sets the snake head by adding < and changing its direction depending on the direction the snake is moving 
                //inside the queue as first element
                snakeElements.Enqueue(snakeNewHead);
                Console.SetCursorPosition(snakeNewHead.col, snakeNewHead.row);
                Console.ForegroundColor = ConsoleColor.Gray;
                if (direction == right) Console.Write(">");
                if (direction == left) Console.Write("<");
                if (direction == up) Console.Write("^");
                if (direction == down) Console.Write("v");

                //game main logic//
                //Snake eating on the food @ or is moving
                if (snakeNewHead.col == food.col && snakeNewHead.row == food.row)
                {
                    Console.Beep();
                    // spawns new food at a random position if the snake ate the food
                    do
                    {
                        //prevents the food spawaning on the user points text
                        food = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight - 1),
                            randomNumbersGenerator.Next(0, Console.WindowWidth));
                    }
                    while (snakeElements.Contains(food) || obstacles.Contains(food));
                    lastFoodTime = Environment.TickCount;
                    Console.SetCursorPosition(food.col, food.row);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("@");
                    sleepTime--;

                    //spawns obstacles and ensures the obstacle do not spawn on food


                    Position obstacle;
                    do
                    {
                        obstacle = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight - 1),
                            randomNumbersGenerator.Next(0, Console.WindowWidth));
                    }
                    while (snakeElements.Contains(obstacle) ||
                        obstacles.Contains(obstacle) ||
                        (food.row != obstacle.row && food.col != obstacle.row));
                    //adds obstacle in the list of obstacles and draw the obstacle
                    obstacles.Add(obstacle);
                    Console.SetCursorPosition(obstacle.col, obstacle.row);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("=");
                }
                else
                {
                    //dequeue removes the first element added in the queue and returns it
                    //
                    // moving...
                    Position last = snakeElements.Dequeue();
                    Console.SetCursorPosition(last.col, last.row);
                    Console.Write(" ");
                }

                //dispawns the food and spawns it on another place
                //if the snake does not eat food before it despawns the user points are penalised by increasing the negative points
                if (Environment.TickCount - lastFoodTime >= foodDissapearTime)
                {
                    negativePoints = negativePoints + 10;
                    Console.SetCursorPosition(food.col, food.row);
                    Console.Write(" ");
                    do
                    {
                        food = new Position(randomNumbersGenerator.Next(0, Console.WindowHeight - 1),
                            randomNumbersGenerator.Next(0, Console.WindowWidth));
                    }
                    while (snakeElements.Contains(food) || obstacles.Contains(food));
                    lastFoodTime = Environment.TickCount;
                }
                Console.SetCursorPosition(food.col, food.row);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("@");

                sleepTime -= 0.01;

                Thread.Sleep((int)sleepTime);
            }
        }
    }
}
