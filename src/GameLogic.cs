using SwinGameSDK;

static class GameLogic
{
	public static void Main()
	{
		//Opens a new Graphics Window
		SwinGame.OpenGraphicsWindow("Battle Ships", 800, 600);

		//Load Resources
		var resources = new GameResources();
		resources.LoadResources();

		//Game Loop
		var controller = new GameController(resources: resources,
											screenController: new ScreenController(),
											menuController: new MenuController(),
											deploymentController: new DeploymentController(),
											discoveryController: new DiscoveryController(),
											endingGameController: new EndingGameController(),
											highScoreController: new HighScoreController());
		do
		{
			controller.HandleUserInput();
			controller.DrawScreen();
		} while (!(SwinGame.WindowCloseRequested() == true || controller.CurrentState == GameState.Quitting));

		SwinGame.StopMusic();

		//Free Resources and Close Audio, to end the program.
		GameResources.FreeResources();
	}
}
