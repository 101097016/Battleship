﻿using System;

// The BattleShipsGame controls a big part of the game. It will add the two players
// to the game and make sure that both players ships are all deployed before starting the game.
// It also allows players to shoot and swap turns between player. It will also check if players 
// are destroyed.
public class BattleShipsGame
{

	// The attack delegate type is used to send notifications of the end of an
	// attack by a player or the AI.
	// sender = the game sending the notification
	// result = the result of the attack
	public delegate void AttackCompletedHandler(object sender, AttackResult result);

	// The AttackCompleted event is raised when an attack has completed.
	// This is used by the UI to play sound effects etc.
	public event AttackCompletedHandler AttackCompleted;

	private Player[] _players = new Player[3];
	private int _playerIndex = 0;

	// The current player.
	// This value will switch between the two players as they have their attacks
	public Player Player
	{
		get
		{
			return _players[_playerIndex];
		}
	}

	// AddDeployedPlayer adds both players and will make sure
	// that the AI player deploys all ships
	// p = 
	public void AddDeployedPlayer(Player p)
	{
		if (_players[0] == null)
		{
			_players[0] = p;
		}
		else if (_players[1] == null)
		{
			_players[1] = p;
			CompleteDeployment();
		}
		else
		{
			throw new ApplicationException("You cannot add another player, the game already has two players.");
		}
	}

	// Assigns each player the other's grid as the enemy grid. This allows each player
	// to examine the details visible on the other's sea grid.
	private void CompleteDeployment()
	{
		_players[0].Enemy = new SeaGridAdapter(_players[1].PlayerGrid);
		_players[1].Enemy = new SeaGridAdapter(_players[0].PlayerGrid);
	}

	// Shoot will swap between players and check if a player has been killed.
	// It also allows the current player to hit on the enemygrid.
	// row = the row fired upon
	// col = the column fired upon
	// returns the result of the attack
	public AttackResult Shoot(int row, int col)
	{
		AttackResult newAttack = null;
		int otherPlayer = (_playerIndex + 1) % 2;

		newAttack = Player.Shoot(row, col);

		// Will exit the game when all players ships are destroyed
		if (_players[otherPlayer].IsDestroyed)
		{
			newAttack = new AttackResult(ResultOfAttack.GameOver, newAttack.Ship, newAttack.Text, row, col);
		}

		if (AttackCompleted != null)
			AttackCompleted(this, newAttack);

		// Change player if the last hit was a miss
		if (newAttack.Value == ResultOfAttack.Miss)
		{
			_playerIndex = otherPlayer;
		}

		return newAttack;
	}
}
