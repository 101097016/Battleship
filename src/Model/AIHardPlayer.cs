using System;
using System.Collections.Generic;


// AIHardPlayer is a type of player. This AI will know directions of ships
// when it has found 2 ship tiles and will try to destroy that ship. If that ship
// is not destroyed it will shoot the other way. Ship still not destroyed, then
// the AI knows it has hit multiple ships. Then will try to destoy all around tiles
// that have been hit.
public class AIHardPlayer : AIPlayer
{
	// Target allows the AI to know more things, for example the source of a
	// shot target. It's using the protected class Location from AIPlayer.cs to store
	// the rows and columns of the place where shot at.
	protected class Target
	{
		private readonly Location _ShotAt;
		private readonly Location _Source;

		// The target shot at.
		public Location ShotAt
		{
			get
			{
				return _ShotAt;
			}
		}
	
		// The source that added this location as a target.
		public Location Source
		{
			get
			{
				return _Source;
			}
		}

		// Takes in the location of the aim and stores them in the _ShotAt and _Source variables.
		internal Target(Location shootat, Location source)
		{
			_ShotAt = shootat;
			_Source = source;
		}

		// If source shot and shootat shot are on the same row then.
		// give a boolean true.
		public bool SameRow
		{
			get
			{
				return _ShotAt.Row == _Source.Row;
			}
		}

		// If source shot and shootat shot are on the same column then.
		// give a boolean true.
		public bool SameColumn
		{
			get
			{
				return _ShotAt.Column == _Source.Column;
			}
		}
	}

	// Private enumerator for AI states. currently there are three states,
	// the AI can be searching for a ship, it can target a ship and it can hit a locked on ship.
	private enum AIStates
	{
		// The AI is searching for its next target.
		Searching,


		// The AI is trying to target a ship.
		TargetingShip,


		// The AI is locked onto a ship.
		HittingShip
	}

	// Assigning enumerator AIStates .Searching to a variable and collecting stacks and lists of Targets.
	private AIStates _CurrentState = AIStates.Searching;
	private Stack<Target> _Targets = new Stack<Target>();
	private List<Target> _LastHit = new List<Target>();
	private Target _CurrentTarget;

	// Using class BattleShipGame for base code of shooting and swapping between players,
	// it also checks if ships are destroyed.
	public AIHardPlayer(BattleShipsGame game) : base(game)
	{
	}


	// GenerateCoords will call upon the right methods to generate the appropriate shooting
	// coordinates.
	// row = the row that will be shot at.
	// column = the column that will be shot at.
	protected override void GenerateCoords(ref int row, ref int column)
	{
		do
		{
			_CurrentTarget = null;

			// check which state the AI is in and upon that choose which coordinate generation
			// method will be used.
			switch (_CurrentState)
			{
				case AIStates.Searching:
					SearchCoords(ref row, ref column);
					break;
				case AIStates.TargetingShip:
				case AIStates.HittingShip:
					TargetCoords(ref row, ref column);
					break;
				default:
					throw new ApplicationException("AI has gone in an invalid state");
			}

		// while inside the grid and not a sea tile do the search.
		} while (row < 0 || column < 0 || row >= EnemyGrid.Height || column >= EnemyGrid.Width || EnemyGrid.Item(row, column) != TileView.Sea); 
	}

	// TargetCoords is used when a ship has been hit and it will try and destroy this ship.
	// row = row generated around the hit tile.
	// column = column generated around the hit tile.
	private void TargetCoords(ref int row, ref int column)
	{
		Target t = _Targets.Pop();

		row = t.ShotAt.Row;
		column = t.ShotAt.Column;
		_CurrentTarget = t;
	}

	// SearchCoords will randomly generate shots within the grid as long as its not hit that tile already.
	// row = the generated row.
	// column = the generated column.
	private void SearchCoords(ref int row, ref int column)
	{
		row = _Random.Next(0, EnemyGrid.Height);
		column = _Random.Next(0, EnemyGrid.Width);
		_CurrentTarget = new Target(new Location(row, column), null);
	}

	// ProcessShot is able to process each shot that is made and call the right methods belonging
	// to that shot. For example, if its a miss = do nothing, if it's a hit = process that hit location
	// row = the row that was shot at.
	// col = the column that was shot at.
	// result = the result from that hit.
	protected override void ProcessShot(int row, int col, AttackResult result)
	{
		switch (result.Value)
		{
			case ResultOfAttack.Miss:
				_CurrentTarget = null;
				break;
			case ResultOfAttack.Hit:
				ProcessHit(row, col);
				break;
			case ResultOfAttack.Destroyed:
				ProcessDestroy(row, col, result.Ship);
				break;
			case ResultOfAttack.ShotAlready:
				throw new ApplicationException("Error in AI");
		}

		// If there are no current targets the AI searches for targets.
		if (_Targets.Count == 0)
		{
			_CurrentState = AIStates.Searching;
		}
	}

	// ProcessDestroy is able to process the destroyed ships targets and remove _LastHit targets.
	// It will also call RemoveShotsAround to remove targets that it was going to shoot at.
	// row = the row that was shot at and destroyed.
	// col = the row that was shot at and destroyed.
	// ship = the row that was shot at and destroyed.
	private void ProcessDestroy(int row, int col, Ship ship)
	{
		bool foundOriginal = false;
		Location source = null;
		Target current = _CurrentTarget;

		foundOriginal = false;

		// i = 1, as we dont have targets from the current hit...
		for (int i = 1; i < ship.Hits; i++)
		{

			if (!foundOriginal)
			{
				source = current.Source;
				// Source is nothing if the ship was originally hit in
				// the middle. This then searched forward, rather than
				// backward through the list of targets.
				if (source == null)
				{
					source = current.ShotAt;
					foundOriginal = true;
				}
			}
			else
			{
				source = current.ShotAt;
			}

			// find the source in _LastHit.
			foreach (Target t in _LastHit)
			{
				if ((!foundOriginal && t.ShotAt == source) || (foundOriginal && t.Source == source))
				{
					current = t;
					_LastHit.Remove(t);
					break;
				}
			}

			RemoveShotsAround(current.ShotAt);
		}
	}

	// RemoveShotsAround will remove targets that belong to the destroyed ship by checking if 
	// the source of the targets belong to the destroyed ship. If they don't put them on a new stack.
	// Then clear the targets stack and move all the targets that still need to be shot at back 
	// onto the targets stack.
	// toRemove = 
	private void RemoveShotsAround(Location toRemove)
	{
		// Create a new stack.
		Stack<Target> newStack = new Stack<Target>();

		// check all targets in the _Targets stack.
		foreach (Target t in _Targets)
		{

			// if the source of the target does not belong to the destroyed ship put them on the newStack
			if (t.Source != toRemove)
			{
				newStack.Push(t);
			}
		}

		// Clear the _Targets stack.
		_Targets.Clear();

		// For all the targets in the newStack, move them back onto the _Targets stack.
		foreach (Target t in newStack)
		{
			_Targets.Push(t);
		}

		// if the _Targets stack is 0 then change the AI's state back to searching.
		if (_Targets.Count == 0)
		{
			_CurrentState = AIStates.Searching;
		}
	}

	// ProcessHit gets the last hit location coordinates and will ask AddTarget to
	// create targets around that location by calling the method four times each time with
	// a new location around the last hit location.
	// It will then set the state of the AI and if it's not Searching or targetingShip then 
	// start ReOrderTargets.
	// row = 
	// col = 
	private void ProcessHit(int row, int col)
	{
		_LastHit.Add(_CurrentTarget);

		// Uses _CurrentTarget as the source.
		AddTarget(row - 1, col);
		AddTarget(row, col - 1);
		AddTarget(row + 1, col);
		AddTarget(row, col + 1);

		if (_CurrentState == AIStates.Searching)
		{
			_CurrentState = AIStates.TargetingShip;
		}
		else
		{
			// Either targeting or hitting... both are the same here.
			_CurrentState = AIStates.HittingShip;

			ReOrderTargets();
		}
	}

	// ReOrderTargets will optimise the targeting by re-orderin the stack that the targets are in.
	// By putting the most important targets at the top they are the ones that will be shot at first.
	private void ReOrderTargets()
	{

		// if the ship is lying on the same row, call MoveToTopOfStack to optimise on the row.
		if (_CurrentTarget.SameRow)
		{
			MoveToTopOfStack(_CurrentTarget.ShotAt.Row, -1);
		}
		else if (_CurrentTarget.SameColumn)
		{
			// else if the ship is lying on the same column, call MoveToTopOfStack to optimise on the column.
			MoveToTopOfStack(-1, _CurrentTarget.ShotAt.Column);
		}
	}

	// MoveToTopOfStack will re-order the stack by checking the coordinates of each target
	// If they have the right column or row values it will be moved to the _Match stack else 
	// put it on the _NoMatch stack. Then move all the targets from the _NoMatch stack back on the 
	// _Targets stack, these will be at the bottom making them less important. The move all the
	// targets from the _Match stack on the _Targets stack, these will be on the top and will therefore
	// be shot at first.
	// row = the row of the optimisation.
	// column = the column of the optimisation.
	private void MoveToTopOfStack(int row, int column)
	{
		Stack<Target> _NoMatch = new Stack<Target>();
		Stack<Target> _Match = new Stack<Target>();

		Target current = null;

		while (_Targets.Count > 0)
		{
			current = _Targets.Pop();
			if (current.ShotAt.Row == row || current.ShotAt.Column == column)
			{
				_Match.Push(current);
			}
			else
			{
				_NoMatch.Push(current);
			}
		}

		foreach (Target t in _NoMatch)
		{
			_Targets.Push(t);
		}
		foreach (Target t in _Match)
		{
			_Targets.Push(t);
		}
	}

	// AddTarget will add the targets it will shoot onto a stack.
	// row = the row of the targets location.
	// column = the column of the targets location.
	private void AddTarget(int row, int column)
	{

		if (row >= 0 && column >= 0 && row < EnemyGrid.Height && column < EnemyGrid.Width && EnemyGrid.Item(row, column) == TileView.Sea)
		{

			_Targets.Push(new Target(new Location(row, column), _CurrentTarget.ShotAt));
		}
	}
}
