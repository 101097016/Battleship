using System;
using System.Collections.Generic;

// The AIMediumPlayer is a type of AIPlayer where it will try and destroy a ship
// if it has found a ship.
public class AIMediumPlayer : AIPlayer
{
	// Private enumarator for AI states. currently there are two states,
	// the AI can be searching for a ship, or if it has found a ship it will
	// target the same ship
	private enum AIStates
	{
		Searching,
		TargetingShip
	}

	// Declaring private fields for the Medium AI, it knows that it needs to Search (SearchCoords)
	// and it knows its stack of targets that it has shot at.
	private AIStates _CurrentState = AIStates.Searching;
	private Stack<Location> _Targets = new Stack<Location>();

	// Using class BattleShipGame for base code of shooting and swapping between players,
	// it also checks if ships are destroyed.
	public AIMediumPlayer(BattleShipsGame controller) : base(controller)
	{
	}

	// GenerateCoordinates should generate random shooting coordinates
	// only when it has not found a ship, or has destroyed a ship and 
	// needs new shooting coordinates
	// row = the generated row.
	// column = the generated column.
	protected override void GenerateCoords(ref int row, ref int column)
	{
		do
		{
			// check which state the AI is in and uppon that choose which coordinate generation
			// method will be used.
			switch (_CurrentState)
			{
				case AIStates.Searching:
					SearchCoords(ref row, ref column);
					break;
				case AIStates.TargetingShip:
					TargetCoords(ref row, ref column);
					break;
				default:
					throw new ApplicationException("AI has gone in an invalid state");
			}
		// While inside the grid and not a sea tile, do the search.
		} while (row < 0 || column < 0 || row >= EnemyGrid.Height || column >= EnemyGrid.Width || EnemyGrid.Item(row, column) != TileView.Sea); 
	}

	// TargetCoords is used when a ship has been hit and it will try and destroy
	// this ship.
	// row = row generated around the hit tile.
	// column = column generated around the hit tile.
	private void TargetCoords(ref int row, ref int column)
	{
		Location l = _Targets.Pop();

		if (_Targets.Count == 0)
		{
			_CurrentState = AIStates.Searching;
		}
		row = l.Row;
		column = l.Column;
	}

	// SearchCoords will randomly generate shots within the grid as long as its not hit that tile already.
	// row = the generated row.
	// column = the generated column.
	private void SearchCoords(ref int row, ref int column)
	{
		row = _Random.Next(0, EnemyGrid.Height);
		column = _Random.Next(0, EnemyGrid.Width);
	}

	// ProcessShot will be called uppon when a ship is found.
	// It will create a stack with targets it will try to hit. These targets
	// will be around the tile that has been hit.
	// row = the row it needs to process.
	// col = the column it needs to process.
	// result = the result og the last shot (should be hit).
	protected override void ProcessShot(int row, int col, AttackResult result)
	{

		if (result.Value == ResultOfAttack.Hit)
		{
			_CurrentState = AIStates.TargetingShip;
			AddTarget(row - 1, col);
			AddTarget(row, col - 1);
			AddTarget(row + 1, col);
			AddTarget(row, col + 1);
		}
		else if (result.Value == ResultOfAttack.ShotAlready)
		{
			throw new ApplicationException("Error in AI");
		}
	}

	// AddTarget will add the targets it will shoot onto a stack
	// row = the row of the targets location.
	// column = the column of the targets location.
	private void AddTarget(int row, int column)
	{
		if (row >= 0 && column >= 0 && row < EnemyGrid.Height && column < EnemyGrid.Width && EnemyGrid.Item(row, column) == TileView.Sea)
		{

			_Targets.Push(new Location(row, column));
		}
	}
}
