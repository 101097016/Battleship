using SwinGameSDK;

/// <summary>
/// The EndingGameController is responsible for managing the interactions at the end
/// of a game.
/// </summary>

public class EndingGameController
{
	internal GameController _controller;

	/// <summary>
	/// Draw the end of the game screen, shows the win/lose state
	/// </summary>
	public virtual void DrawEndOfGame()
	{
		_controller.screenController.DrawField(_controller.ComputerPlayer.PlayerGrid, _controller.ComputerPlayer, true);
		_controller.screenController.DrawSmallField(_controller.HumanPlayer.PlayerGrid, _controller.HumanPlayer);

		if (_controller.HumanPlayer.IsDestroyed)
		{
			SwinGame.DrawTextLines("YOU LOSE!", Color.White, Color.Transparent, _controller.Resources.GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
		}
		else
		{
			SwinGame.DrawTextLines("-- WINNER --", Color.White, Color.Transparent, _controller.Resources.GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
		}
	}

	/// <summary>
	/// Handle the input during the end of the game. Any interaction
	/// will result in it reading in the highscore.
	/// </summary>
	public virtual void HandleEndOfGameInput()
	{
		if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.vk_RETURN) || SwinGame.KeyTyped(KeyCode.vk_ESCAPE))
		{
			_controller.highScoreController.ReadHighScore(_controller.HumanPlayer.Score);
			_controller.EndCurrentState();
		}
	}

}
