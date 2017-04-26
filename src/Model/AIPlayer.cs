using SwinGameSDK;

/// <summary>
/// The AIPlayer is a type of player. It can readomly deploy ships, it also has the
/// functionality to generate coordinates and shoot at tiles
/// </summary> 
public abstract class AIPlayer : Player
{
	/// <summary>
	/// Location can store the location of the last hit made by an
	/// AI Player. The use of which determines the difficulty.
	/// </summary>
	protected class Location
	{
		private int _Row;
		private int _Column;

		/// <summary>
		/// Gets or sets the row.
		/// </summary>
		/// <value>The row.</value>
		public int Row
		{
			get
			{
				return _Row;
			}
			set
			{
				_Row = value;
			}
		}

		/// <summary>
		/// Gets or sets the column.
		/// </summary>
		/// <value>The column.</value>
		public int Column
		{
			get
			{
				return _Column;
			}
			set
			{
				_Column = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:AIPlayer.Location"/> class.
		/// Sets the last hit made to the local variables.
		/// </summary>
		/// <param name="row">the row of the location.</param>
		/// <param name="column">the column of the location.</param>
		public Location(int row, int column)
		{
			_Column = column;
			_Row = row;
		}

		/// <summary>
		/// Determines whether a specified instance of <see cref="AIPlayer.Location"/> is equal to another specified <see cref="AIPlayer.Location"/>.
		/// </summary>
		/// <param name="this">The first <see cref="AIPlayer.Location"/> to compare.</param>
		/// <param name="other">The second <see cref="AIPlayer.Location"/> to compare.</param>
		/// <returns><c>true</c> if <c>this</c> and <c>other</c> are equal; otherwise, <c>false</c>.</returns>
		public static bool operator ==(Location @this, Location other)
		{
			return @this != (object)null && other != (object)null && @this.Row == other.Row && @this.Column == other.Column;
		}

		/// <summary>
		/// Determines whether the specified <see cref="object"/> is equal to the current <see cref="T:AIPlayer.Location"/>.
		/// </summary>
		/// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:AIPlayer.Location"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="T:AIPlayer.Location"/>;
		/// otherwise, <c>false</c>.</returns>
		public override bool Equals(object obj)
		{
			return this == (Location)obj;
		}

		/// <summary>
		/// Determines whether a specified instance of <see cref="AIPlayer.Location"/> is not equal to another specified <see cref="AIPlayer.Location"/>.
		/// </summary>
		/// <param name="this">The first <see cref="AIPlayer.Location"/> to compare.</param>
		/// <param name="other">The second <see cref="AIPlayer.Location"/> to compare.</param>
		/// <returns><c>true</c> if <c>this</c> and <c>other</c> are not equal; otherwise, <c>false</c>.</returns>
		public static bool operator !=(Location @this, Location other)
		{
			return !(@this == other);
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:AIPlayer"/> class.
	/// </summary>
	/// <param name="game">Game.</param>
	protected AIPlayer(BattleShipsGame game) : base(game)
	{
	}

	/// <summary>
	/// Generates the coords.
	/// Generate a valid row, column to shoot at
	/// </summary>
	/// <param name="row">output the row for the next shot</param>
	/// <param name="column">output the column for the next show</param>
	protected abstract void GenerateCoords(ref int row, ref int column);

	/// <summary>
	/// Processes the shot.
	/// The last shot had the following result. Child classes can use this
	/// to prepare for the next shot.
	/// // result = 
	/// </summary>
	/// <param name="row">the row shot</param>
	/// <param name="col">the column shot</param>
	/// <param name="result">The result of the shot</param>
	protected abstract void ProcessShot(int row, int col, AttackResult result);

	/// <summary>
	/// Attack this instance.
	/// The AI takes its attacks until its go is over.
	/// </summary>
	/// <returns>The attack.</returns>
	public override AttackResult Attack()
	{
		AttackResult result = null;
		int row = 0;
		int column = 0;

		// keep hitting until there's a miss
		do 
		{
			Delay();

			// generate coordinates for shot
			GenerateCoords(ref row, ref column);

			// take shot
			result = _game.Shoot(row, column); 
			ProcessShot(row, column, result);
		} while (result.Value != ResultOfAttack.Miss && result.Value != ResultOfAttack.GameOver && !SwinGame.WindowCloseRequested());

		return result;
	}

	/// <summary>
	/// Wait a short period to simulate the think time
	/// </summary>
	private void Delay()
	{
		int i = 0;
		for (i = 0; i <= 150; i++)
		{
			// Don't delay if window is closed
			if (SwinGame.WindowCloseRequested())
			{
				return;
			}

			SwinGame.Delay(5);
			SwinGame.ProcessEvents();
			SwinGame.RefreshScreen();
		}
	}
}