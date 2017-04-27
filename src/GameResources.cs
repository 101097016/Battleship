using SwinGameSDK;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// The Resources Class stores all of the Games Media Resources, such as Images, Fonts
/// Sounds, Music.
/// </summary>
public class GameResources
{

	private void LoadFonts()
	{
		NewFont("ArialLarge", "arial.ttf", 80);
		NewFont("Courier", "cour.ttf", 14);
		NewFont("CourierSmall", "cour.ttf", 8);
		NewFont("Menu", "ffaccess.ttf", 8);
	}

	private void LoadImages()
	{
		//Backgrounds
		NewImage("Menu", "main_page.jpg");
		NewImage("Discovery", "discover.jpg");
		NewImage("Deploy", "deploy.jpg");

		//Deployment
		NewImage("LeftRightButton", "deploy_dir_button_horiz.png");
		NewImage("UpDownButton", "deploy_dir_button_vert.png");
		NewImage("SelectedShip", "deploy_button_hl.png");
		NewImage("PlayButton", "deploy_play_button.png");
		NewImage("RandomButton", "deploy_randomize_button.png");

		//Ships
		for (int i = 1; i <= 5; i++)
		{
			NewImage("ShipLR" + i, "ship_deploy_horiz_" + i + ".png");
			NewImage("ShipUD" + i, "ship_deploy_vert_" + i + ".png");
		}

		//Explosions
		NewImage("Explosion", "explosion.png");
		NewImage("Splash", "splash.png");

	}

	private void LoadSounds()
	{
		NewSound("Error", "error.wav");
		NewSound("Hit", "hit.wav");
		NewSound("Sink", "sink.wav");
		NewSound("Miss", "watershot.wav");
		NewSound("Winner", "winner.wav");
		NewSound("Lose", "lose.wav");
	}

	private void LoadMusic()
	{
		NewMusic("Background", "background.mp3");
	}

	/// <summary>
	/// Gets a Font Loaded in the Resources
	/// </summary>
	/// <param name="font">Name of Font</param>
	/// <returns>The Font Loaded with this Name</returns>

	public Font GameFont(string font)
	{
		return _Fonts[font];
	}

	/// <summary>
	/// Gets an Image loaded in the Resources
	/// </summary>
	/// <param name="image">Name of image</param>
	/// <returns>The image loaded with this name</returns>

	public Bitmap GameImage(string image)
	{
		return _Images[image];
	}

	/// <summary>
	/// Gets an sound loaded in the Resources
	/// </summary>
	/// <param name="sound">Name of sound</param>
	/// <returns>The sound with this name</returns>

	public SoundEffect GameSound(string sound)
	{
		return _Sounds[sound];
	}

	/// <summary>
	/// Gets the music loaded in the Resources
	/// </summary>
	/// <param name="music">Name of music</param>
	/// <returns>The music with this name</returns>

	public Music GameMusic(string music)
	{
		return _Music[music];
	}

	private Dictionary<string, Bitmap> _Images = new Dictionary<string, Bitmap>();
	private Dictionary<string, Font> _Fonts = new Dictionary<string, Font>();
	private Dictionary<string, SoundEffect> _Sounds = new Dictionary<string, SoundEffect>();
	private Dictionary<string, Music> _Music = new Dictionary<string, Music>();

	private string _ThemeDirName;


	public GameResources(string themeDirName, bool playIntro)
	{
		_ThemeDirName = themeDirName;
		LoadResources(playIntro);
	}

	private void LoadResources(bool playIntro)
	{
		var loadingScreen = new LoadingScreen();
		loadingScreen.Show(playIntro);

		loadingScreen.Message("Loading fonts...", 0);
		LoadFonts();
		SwinGame.Delay(100);

		loadingScreen.Message("Loading images...", 1);
		LoadImages();
		SwinGame.Delay(100);

		loadingScreen.Message("Loading sounds...", 2);
		LoadSounds();
		SwinGame.Delay(100);

		loadingScreen.Message("Loading music...", 3);
		LoadMusic();
		SwinGame.Delay(100);

		SwinGame.Delay(100);
		loadingScreen.Message("Game loaded...", 5);
		SwinGame.Delay(100);

		loadingScreen.Hide();
	}

	private void NewFont(string fontName, string filename, int size)
	{
		_Fonts.Add(fontName, SwinGame.LoadFont(SwinGame.PathToResource(filename, ResourceKind.FontResource), size));
	}

	private void NewImage(string imageName, string filename)
	{
		_Images.Add(imageName, SwinGame.LoadBitmap(SwinGame.PathToResource(filename, ResourceKind.BitmapResource, _ThemeDirName)));
	}

	private void NewSound(string soundName, string filename)
	{
		_Sounds.Add(soundName, Audio.LoadSoundEffect(SwinGame.PathToResource(filename, ResourceKind.SoundResource, _ThemeDirName)));
	}

	private void NewMusic(string musicName, string filename)
	{
		_Music.Add(musicName, Audio.LoadMusic(SwinGame.PathToResource(filename, ResourceKind.SoundResource, _ThemeDirName)));
	}

	private void FreeFonts()
	{
		foreach (Font obj in _Fonts.Values)
		{
			SwinGame.FreeFont(obj);
		}
	}

	private void FreeImages()
	{
		foreach (Bitmap obj in _Images.Values)
		{
			SwinGame.FreeBitmap(obj);
		}
	}

	private void FreeSounds()
	{
		foreach (SoundEffect obj in _Sounds.Values)
		{
			Audio.FreeSoundEffect(obj);
		}
	}

	private void FreeMusic()
	{
		foreach (Music obj in _Music.Values)
		{
			Audio.FreeMusic(obj);
		}
	}

	public void FreeResources()
	{
		FreeFonts();
		FreeImages();
		FreeMusic();
		FreeSounds();
		SwinGame.ProcessEvents();
	}
}
