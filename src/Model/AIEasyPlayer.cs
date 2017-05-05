using System;
using System.Collections;
using System.Collections.Generic;
using SwinGameSDK;


/// <summary>
/// The AIEAsyPlayer is a kind of AIPlayer which hits just randomly.
/// </summary>

public class AIEasyPlayer : AIPlayer
{
	public AIEasyPlayer (BattleShipsGame controller, bool isExtendedMapTemp) : base (controller , isExtendedMapTemp ) { }

	protected override void GenerateCoords (ref int row, ref int column)
	{
		do {
			SearchCoords (ref row, ref column);
		} while ((row < 0 || column < 0 || row >= EnemyGrid.Height || column >= EnemyGrid.Width || EnemyGrid.Item (row, column) != TileView.Sea));
	}

	/// <summary>
	/// randomly shots within the grid as long as it does not hit that tile already
	/// </summary>
	/// <param name="row">generated row</param>
	/// <param name="column">generated column</param>


	private void SearchCoords (ref int row, ref int column)
	{
		row = _Random.Next (0, EnemyGrid.Height);
		column = _Random.Next (0, EnemyGrid.Width);
	}


	protected override void ProcessShot (int row, int col, AttackResult result)
	{
		if (result.Value == ResultOfAttack.Hit) {
			return;
		} else if (result.Value == ResultOfAttack.ShotAlready) {
			throw new ApplicationException ("Error in AI");
		}
	}
}

