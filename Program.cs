Console.Title = "The Long Game";

Console.Write("Welcome to The Long Game, please enter your username to get started: ");
string? username = Console.ReadLine()!;

//Lets make sure the user enters a name - otherwise we can't give them a score.
while(username is null || username == "")
{
	Console.WriteLine("You must supply a username to play");
	Console.Write("What is your username?: ");
	username = Console.ReadLine()!;
}

//Create the player and the game.
LongGame game = new(new User(username));

//Let's play!
if (game.Player.NewPlayer)
{	
	Console.WriteLine($"""
			Welcome {game.Player.Username},
			As this is your first time with us your score starts with {game.Player.Score}.
			The rules are simple. Keep typing to increase your score, every key press is worth 1 point
			To exit and save your progress press "Enter".
			""");
}
else
{
	if (game.Player.ScoreReset)
	{
		Console.WriteLine($"Welcome back {game.Player.Username}, " +
			$"your previous score has been reset and you are starting again at {game.Player.Score}.");
	}
	else
	{
		Console.WriteLine($"Welcome back {game.Player.Username}, last time you scored {game.Player.Score} - You know the rules, begin...");
	}
}
game.Run();

//Game & User Classes
public class LongGame
{
	public User Player { get; init; }
	public bool GameOver { get; private set; }

	public LongGame(User username)
	{
		Player = username;
	}

	public void Run()
	{			
		ConsoleKey userKey;
		while (!GameOver)
		{
			userKey = Console.ReadKey().Key;
			if (userKey == ConsoleKey.Enter)
			{
				Player.SavePlayerScore();
				GameOver = true;
			}
			else
			{
				Player.IncreaseScore();
			}
			Console.Clear();
			Console.WriteLine($"Score: {Player.Score}");
		}
	}
	public bool EndGame() => GameOver = true;
}

public class User
{
	public string Username { get; init; }
	public int Score { get; private set; }
	public bool NewPlayer { get; private set; }
	public bool ScoreReset { get; private set; }

	public User(string username)
	{
		Username = FormatUsername(username);
		Score = LoadPlayerScore();
	}

	private string FormatUsername(string username)
	{
		username = username.ToLower();
		return char.ToUpper(username[0]) + username.Remove(0, 1);
	}

	public int IncreaseScore() => Score++;

	//Checks to see if the user has played before and loads their previous score.
	private int LoadPlayerScore()
	{
		if (File.Exists($"{Username}.txt"))
		{
			string[] fileContents = File.ReadAllLines($"{Username}.txt");
			bool parsed = int.TryParse(fileContents[0], out int score);

			if (!parsed)
			{
				ScoreReset = true;
				return 0;
			}
			return score;
		}
		else
		{
			NewPlayer = true;
			return 0;
		}
	}

	//Save the players current score to a file of their username.
	public void SavePlayerScore()
	{
		File.WriteAllText($"{Username}.txt", Score.ToString());
	}
}

