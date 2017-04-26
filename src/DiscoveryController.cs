using System;
using SwinGameSDK;

/// <summary>
/// The battle phase is handled by the DiscoveryController.
/// </summary>
public class DiscoveryController
{
	internal GameController _controller;

	/// <summary>
	/// Handles input during the discovery phase of the game.
	/// </summary>
	/// <remarks>
	/// Escape opens the game menu. Clicking the mouse will
	/// attack a location.
	/// </remarks>
	public virtual void HandleDiscoveryInput()
	{
		if (SwinGame.KeyTyped(KeyCode.vk_ESCAPE))
		{
			_controller.AddNewState(GameState.ViewingGameMenu);
		}

		if (SwinGame.MouseClicked(MouseButton.LeftButton))
		{
			DoAttack();
		}
	}

	/// <summary>
	/// Attack the location that the mouse if over.
	/// </summary>
	private void DoAttack()
	{
		Point2D mouse = SwinGame.MousePosition();


		//Calculate the row/col clicked
		int row = 0;
		int col = 0;
		row = Convert.ToInt32(Math.Floor((mouse.Y - ScreenController.FIELD_TOP) / (ScreenController.CELL_HEIGHT + ScreenController.CELL_GAP)));
		col = Convert.ToInt32(Math.Floor((mouse.X - ScreenController.FIELD_LEFT) / (ScreenController.CELL_WIDTH + ScreenController.CELL_GAP)));

		if (row >= 0 && row < _controller.HumanPlayer.EnemyGrid.Height)
		{
			if (col >= 0 && col < _controller.HumanPlayer.EnemyGrid.Width)
			{
				_controller.Attack(row, col);
			}
		}
	}

	/// <summary>
	/// Draws the game during the attack phase.
	/// </summary>s
	public virtual void DrawDiscovery()
	{
		const int SCORES_LEFT = 172;
		const int SHOTS_TOP = 157;
		const int HITS_TOP = 206;
		const int SPLASH_TOP = 256;
        
		if ((SwinGame.KeyDown(KeyCode.vk_LSHIFT) || SwinGame.KeyDown(KeyCode.vk_RSHIFT)) && SwinGame.KeyDown(KeyCode.vk_c))
		{
			_controller.screenController.DrawField(_controller.HumanPlayer.EnemyGrid, _controller.ComputerPlayer, true);
		}
		else
		{
			_controller.screenController.DrawField(_controller.HumanPlayer.EnemyGrid, _controller.ComputerPlayer, false);
		}

		_controller.screenController.DrawSmallField(_controller.HumanPlayer.PlayerGrid, _controller.HumanPlayer);
		_controller.screenController.DrawMessage();

		SwinGame.DrawText(_controller.HumanPlayer.Shots.ToString(), Color.White, _controller.Resources.GameFont("Menu"), SCORES_LEFT, SHOTS_TOP);
		SwinGame.DrawText(_controller.HumanPlayer.Hits.ToString(), Color.White, _controller.Resources.GameFont("Menu"), SCORES_LEFT, HITS_TOP);
		SwinGame.DrawText(_controller.HumanPlayer.Missed.ToString(), Color.White, _controller.Resources.GameFont("Menu"), SCORES_LEFT, SPLASH_TOP);
	}

}
