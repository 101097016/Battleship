using System;
using System.IO;
using System.Collections.Generic;
using SwinGameSDK;

/// <summary>
/// Controls displaying and collecting high score data.
/// </summary>
/// <remarks>
/// Data is saved to a file.
/// </remarks>
public class HighScoreController
{
	internal GameController _controller;

	protected const int NAME_WIDTH = 3;
	protected const int SCORES_LEFT = 490;

	/// <summary>
	/// The score structure is used to keep the name and
	/// score of the top players together.
	/// </summary>
	protected struct Score : IComparable
	{
		public string Name;
		public int Value;

		/// <summary>
		/// Allows scores to be compared to facilitate sorting
		/// </summary>
		/// <param name="obj">the object to compare to</param>
		/// <returns>a value that indicates the sort order</returns>
		public int CompareTo(object obj)
		{
			if (obj is Score)
			{
				return ((Score)obj).Value - Value;
			}
			else
			{
				return 0;
			}
		}
	}

	protected List<Score> _Scores = new List<Score>();

	/// <summary>
	/// Loads the scores from the highscores text file.
	/// </summary>
	/// <remarks>
	/// The format is
	/// # of scores
	/// NNNSSS
	/// 
	/// Where NNN is the name and SSS is the score
	/// </remarks>
	protected virtual void LoadScores()
	{
		string filename = SwinGame.PathToResource("highscores.txt");

		var input = new StreamReader(filename);

		//Read in the # of scores
		int numScores = Convert.ToInt32(input.ReadLine());

		_Scores.Clear();

		int i = 0;

		for (i = 1; i <= numScores; i++)
		{
			string line = input.ReadLine();
			_Scores.Add(new Score
			{
				Name = line.Substring(0, NAME_WIDTH),
				Value = Convert.ToInt32(line.Substring(NAME_WIDTH))
			});
		}
		input.Close();
	}

	/// <summary>
	/// Saves the scores back to the highscores text file.
	/// </summary>
	/// <remarks>
	/// The format is
	/// # of scores
	/// NNNSSS
	/// 
	/// Where NNN is the name and SSS is the score
	/// </remarks>
	protected virtual void SaveScores()
	{
		string filename = SwinGame.PathToResource("highscores.txt");

		var output = new StreamWriter(filename);

		output.WriteLine(_Scores.Count);

		foreach (Score s in _Scores)
		{
			output.WriteLine(s.Name + s.Value);
		}

		output.Close();
	}

	/// <summary>
	/// Draws the high scores to the screen.
	/// </summary>
	public virtual void DrawHighScores()
	{
		const int SCORES_HEADING = 40;
		const int SCORES_TOP = 80;
		const int SCORE_GAP = 30;

		if (_Scores.Count == 0)
		{
			LoadScores();
		}

		SwinGame.DrawText("   High Scores   ", Color.White, _controller.Resources.GameFont("Courier"), SCORES_LEFT, SCORES_HEADING);

		//For all of the scores
		int i = 0;
		for (i = 0; i < _Scores.Count; i++)
		{
			Score s = _Scores[i];


			//for scores 1 - 9 use 01 - 09
			if (i < 9)
			{
				SwinGame.DrawText(" " + (i + 1) + ":   " + s.Name + "   " + s.Value, Color.White, _controller.Resources.GameFont("Courier"), SCORES_LEFT, SCORES_TOP + i * SCORE_GAP);
			}
			else
			{
				SwinGame.DrawText((i + 1) + ":   " + s.Name + "   " + s.Value, Color.White, _controller.Resources.GameFont("Courier"), SCORES_LEFT, SCORES_TOP + i * SCORE_GAP);
			}
		}
	}

	/// <summary>
	/// Handles the user input during the top score screen.
	/// </summary>
	/// <remarks></remarks>
	public virtual void HandleHighScoreInput()
	{
		if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.vk_ESCAPE) || SwinGame.KeyTyped(KeyCode.vk_RETURN))
		{
			_controller.EndCurrentState();
		}
	}

	/// <summary>
	/// Read the user's name for their highscore.
	/// </summary>
	/// <param name="value">the player's score.</param>
	/// <remarks>
	/// This verifies if the score is a highscore.
	/// </remarks>
	public virtual void ReadHighScore(int value)
	{
		const int ENTRY_TOP = 500;

		if (_Scores.Count == 0)
		{
			LoadScores();
		}

		//is it a high score
		if (value > _Scores[_Scores.Count - 1].Value)
		{
			var s = new Score();
			s.Value = value;

			_controller.AddNewState(GameState.ViewingHighScores);

			int x = SCORES_LEFT + SwinGame.TextWidth(_controller.Resources.GameFont("Courier"), "Name: ");

			SwinGame.StartReadingText(Color.White, NAME_WIDTH, _controller.Resources.GameFont("Courier"), x, ENTRY_TOP);

			//Read the text from the user
			while (SwinGame.ReadingText() && !SwinGame.WindowCloseRequested())
			{
				SwinGame.ProcessEvents();

				_controller.screenController.DrawBackground();
				DrawHighScores();
				SwinGame.DrawText("Name: ", Color.White, _controller.Resources.GameFont("Courier"), SCORES_LEFT, ENTRY_TOP);
				SwinGame.RefreshScreen();
			}

			s.Name = SwinGame.TextReadAsASCII();

			if (s.Name.Length < 3)
			{
				s.Name = s.Name + new string(' ', 3 - s.Name.Length);
			}

			_Scores.RemoveAt(_Scores.Count - 1);
			_Scores.Add(s);
			_Scores.Sort();
			SaveScores();

			_controller.EndCurrentState();
		}
	}
}
