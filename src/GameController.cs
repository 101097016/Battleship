using System;
using System.Collections.Generic;
using SwinGameSDK;

/// <summary>
/// The GameController is responsible for controlling the game,
/// managing user input, and displaying the current state of the
/// game.
/// </summary>
public sealed class GameController
{
	private BattleShipsGame _theGame;
	private Player _human;
	private AIPlayer _ai;

	private Stack<GameState> _state = new Stack<GameState>();

	private AIOption _aiSetting;

	private GameResources _resources;
	private ScreenController _screenController;
	private MenuController _menuController;
	private DeploymentController _deploymentController;
	private DiscoveryController _discoveryController;
	private EndingGameController _endingGameController;
	private HighScoreController _highScoreController;

	internal ScreenController screenController
	{
		get { return _screenController; }	}
	internal HighScoreController highScoreController
	{
		get { return _highScoreController; }
	}

	/// <summary>
	/// Returns or sets the current game resources.
	/// </summary>
	/// <value>The current game resources</value>
	/// <returns>The current game resources</returns>
	public GameResources Resources
	{
		get
		{
			return _resources;
		}
		set
		{
			_resources = value;
			if (SwinGame.MusicPlaying()) SwinGame.StopMusic();
			SwinGame.PlayMusic(value.GameMusic("Background"));
		}
	}

	/// <summary>
	/// Returns the current state of the game, indicating which screen is
	/// currently being used.
	/// </summary>
	/// <value>The current state</value>
	/// <returns>The current state</returns>
	public GameState CurrentState
	{
		get
		{
			return _state.Peek();
		}
	}

	/// <summary>
	/// Returns the human player.
	/// </summary>
	/// <value>the human player</value>
	/// <returns>the human player</returns>
	public Player HumanPlayer
	{
		get
		{
			return _human;
		}
	}

	/// <summary>
	/// Returns the computer player.
	/// </summary>
	/// <value>the computer player</value>
	/// <returns>the conputer player</returns>
	public Player ComputerPlayer
	{
		get
		{
			return _ai;
		}
	}

	public GameController(GameResources resources,
						  ScreenController screenController,
						  MenuController menuController,
						  DeploymentController deploymentController,
						  DiscoveryController discoveryController,
						  EndingGameController endingGameController,
						  HighScoreController highScoreController)
	{
		//install the controllers and resources
		Resources = resources;

		_screenController = screenController;
		_screenController._controller = this;

		_menuController = menuController;
		_menuController._controller = this;

		_deploymentController = deploymentController;
		_deploymentController._controller = this;

		_discoveryController = discoveryController;
		_discoveryController._controller = this;

		_endingGameController = endingGameController;
		_endingGameController._controller = this;

		_highScoreController = highScoreController;
		_highScoreController._controller = this;

		//bottom state will be quitting. If player exits main menu then the game is over
		_state.Push(GameState.Quitting);

		//at the start the player is viewing the main menu
		_state.Push(GameState.ViewingMainMenu);
	}

	/// <summary>
	/// Starts a new game.
	/// </summary>
	/// <remarks>
	/// Creates an AI player based upon the _aiSetting.
	/// </remarks>
	public void StartGame()
	{
		if (_theGame != null)
		{
			EndGame();
		}

		//Create the game
		_theGame = new BattleShipsGame();

		switch (_aiSetting)
		{
			case AIOption.Medium:
				_ai = new AIMediumPlayer(_theGame);
				break;
			case AIOption.Hard:
				_ai = new AIHardPlayer(_theGame);
				break;
			default:
				_ai = new AIHardPlayer(_theGame);
				break;
		}

		_human = new Player(_theGame);

		//AddHandler _human.PlayerGrid.Changed, AddressOf GridChanged
		_ai.PlayerGrid.Changed += GridChanged;
		_theGame.AttackCompleted += AttackCompleted;

		AddNewState(GameState.Deploying);
	}

	/// <summary>
	/// Stops listening to the old game once a new game is started
	/// </summary>

	private void EndGame()
	{
		//RemoveHandler _human.PlayerGrid.Changed, AddressOf GridChanged
		_ai.PlayerGrid.Changed -= GridChanged;
		_theGame.AttackCompleted -= AttackCompleted;
	}

	/// <summary>
	/// Listens to the game grids for any changes and redraws the screen
	/// when the grids change
	/// </summary>
	/// <param name="sender">the grid that changed</param>
	/// <param name="args">not used</param>
	private void GridChanged(object sender, EventArgs args)
	{
		DrawScreen();
		SwinGame.RefreshScreen();
	}

	private void PlayHitSequence(int row, int column, bool showAnimation)
	{
		if (showAnimation)
		{
			screenController.AddExplosion(row, column);
		}

		Audio.PlaySoundEffect(Resources.GameSound("Hit"));

		screenController.DrawAnimationSequence();
	}

	private void PlayMissSequence(int row, int column, bool showAnimation)
	{
		if (showAnimation)
		{
			screenController.AddSplash(row, column);
		}

		Audio.PlaySoundEffect(Resources.GameSound("Miss"));

		screenController.DrawAnimationSequence();
	}

	/// <summary>
	/// Listens for attacks to be completed.
	/// </summary>
	/// <param name="sender">the game</param>
	/// <param name="result">the result of the attack</param>
	/// <remarks>
	/// Displays a message, plays sound and redraws the screen
	/// </remarks>
	private void AttackCompleted(object sender, AttackResult result)
	{
		bool isHuman = _theGame.Player == HumanPlayer;

		if (isHuman)
		{
			screenController.Message = "You " + result;
		}
		else
		{
			screenController.Message = "The AI " + result;
		}

		switch (result.Value)
		{
			case ResultOfAttack.Destroyed:
				PlayHitSequence(result.Row, result.Column, isHuman);
				Audio.PlaySoundEffect(Resources.GameSound("Sink"));
				break;
			case ResultOfAttack.GameOver:
				PlayHitSequence(result.Row, result.Column, isHuman);
				Audio.PlaySoundEffect(Resources.GameSound("Sink"));

				//JED, removed this loop, was causeing hanging during the end game. With the screech of a background music this
				//sound overlap is inaudible.
				/*
				while (Audio.SoundEffectPlaying(Resources.GameSound("Sink")))
				{
					SwinGame.Delay(10);
					SwinGame.RefreshScreen();
				}
				*/

				if (HumanPlayer.IsDestroyed)
				{
					Audio.PlaySoundEffect(Resources.GameSound("Lose"));
				}
				else
				{
					Audio.PlaySoundEffect(Resources.GameSound("Winner"));
				}
				break;
			case ResultOfAttack.Hit:
				PlayHitSequence(result.Row, result.Column, isHuman);
				break;
			case ResultOfAttack.Miss:
				PlayMissSequence(result.Row, result.Column, isHuman);
				break;
			case ResultOfAttack.ShotAlready:
				Audio.PlaySoundEffect(Resources.GameSound("Error"));
				break;
		}
	}

	/// <summary>
	/// Completes the deployment phase of the game and
	/// switches to the battle mode (Discovering state)
	/// </summary>
	/// <remarks>
	/// This adds the players to the game before switching
	/// state.
	/// </remarks>
	public void EndDeployment()
	{
		//deploy the players
		_theGame.AddDeployedPlayer(_human);
		_theGame.AddDeployedPlayer(_ai);

		SwitchState(GameState.Discovering);
	}

	/// <summary>
	/// Gets the player to attack the indicated row and column.
	/// </summary>
	/// <param name="row">the row to attack</param>
	/// <param name="col">the column to attack</param>
	/// <remarks>
	/// Checks the attack result once the attack is complete
	/// </remarks>
	public void Attack(int row, int col)
	{
		AttackResult result = _theGame.Shoot(row, col);
		CheckAttackResult(result);
	}

	/// <summary>
	/// Gets the AI to attack.
	/// </summary>
	/// <remarks>
	/// Checks the attack result once the attack is complete.
	/// </remarks>
	private void AIAttack()
	{
		AttackResult result = _theGame.Player.Attack();
		CheckAttackResult(result);
	}

	/// <summary>
	/// Checks the results of the attack and switches to
	/// Ending the Game if the result was game over.
	/// </summary>
	/// <param name="result">the result of the last
	/// attack</param>
	/// <remarks>Gets the AI to attack if the result switched
	/// to the AI player.</remarks>
	private void CheckAttackResult(AttackResult result)
	{
		if (result.Value == ResultOfAttack.Miss)
		{
			if (_theGame.Player == ComputerPlayer)
			{
				AIAttack();
			}
		}
		else if (result.Value == ResultOfAttack.GameOver)
		{
			SwitchState(GameState.EndingGame);
		}
	}

	/// <summary>
	/// Handles the user SwinGame.
	/// </summary>
	/// <remarks>
	/// Reads key and mouse input and converts these into
	/// actions for the game to perform. The actions
	/// performed depend upon the state of the game.
	/// </remarks>
	public void HandleUserInput()
	{
		//Read incoming input events
		SwinGame.ProcessEvents();

		switch (CurrentState)
		{
			case GameState.ViewingMainMenu:
				_menuController.HandleMainMenuInput();
				break;
			case GameState.ViewingGameMenu:
				_menuController.HandleGameMenuInput();
				break;
			case GameState.AlteringSettings:
				_menuController.HandleSetupMenuInput();
				break;
			case GameState.AlteringTheme:
				_menuController.HandleThemeMenuInput();
				break;
			case GameState.Deploying:
				_deploymentController.HandleDeploymentInput();
				break;
			case GameState.Discovering:
				_discoveryController.HandleDiscoveryInput();
				break;
			case GameState.EndingGame:
				_endingGameController.HandleEndOfGameInput();
				break;
			case GameState.ViewingHighScores:
				highScoreController.HandleHighScoreInput();
				break;
		}

		screenController.UpdateAnimations();
	}

	/// <summary>
	/// Draws the current state of the game to the screen.
	/// </summary>
	/// <remarks>
	/// What is drawn depends upon the state of the game.
	/// </remarks>
	public void DrawScreen()
	{
		screenController.DrawBackground();

		switch (CurrentState)
		{
			case GameState.ViewingMainMenu:
				_menuController.DrawMainMenu();
				break;
			case GameState.ViewingGameMenu:
				_menuController.DrawGameMenu();
				break;
			case GameState.AlteringSettings:
				_menuController.DrawSettings();
				break;
			case GameState.AlteringTheme:
				_menuController.DrawThemesMenu();
				break;
			case GameState.Deploying:
				_deploymentController.DrawDeployment();
				break;
			case GameState.Discovering:
				_discoveryController.DrawDiscovery();
				break;
			case GameState.EndingGame:
				_endingGameController.DrawEndOfGame();
				break;
			case GameState.ViewingHighScores:
				highScoreController.DrawHighScores();
				break;
		}

		screenController.DrawAnimations();

		SwinGame.RefreshScreen();
	}

	/// <summary>
	/// Move the game to a new state. The current state is maintained
	/// so that it can be returned to.
	/// </summary>
	/// <param name="state">the new game state</param>
	public void AddNewState(GameState state)
	{
		_state.Push(state);
		screenController.Message = "";
	}

	/// <summary>
	/// End the current state and add in the new state.
	/// </summary>
	/// <param name="newState">the new state of the game</param>
	public void SwitchState(GameState newState)
	{
		EndCurrentState();
		AddNewState(newState);
	}

	/// <summary>
	/// Ends the current state, returning to the prior state
	/// </summary>
	public void EndCurrentState()
	{
		_state.Pop();
	}

	/// <summary>
	/// Sets the difficulty for the next level of the game.
	/// </summary>
	/// <param name="setting">the new difficulty level</param>
	public void SetDifficulty(AIOption setting)
	{
		_aiSetting = setting;
	}
}
