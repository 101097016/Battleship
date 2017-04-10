/// <summary>
/// AttackResult gives the result after a shot has been made.
/// </summary>
public class AttackResult
{
	private ResultOfAttack _Value;
	private Ship _Ship;
	private string _Text;
	private int _Row;
	private int _Column;

	/// <summary>
	/// The result of the attack
	/// </summary>
	public ResultOfAttack Value
	{
		get
		{
			return _Value;
		}
	}

	/// <summary>
	/// The ship, if any, involved in this result
	/// </summary>
	/// <value>The ship.</value> 
	public Ship Ship
	{
		get
		{
			return _Ship;
		}
	}

	/// <summary>
	/// A textual description of the result.
	/// </summary>
	/// <value>The text.</value>
	public string Text
	{
		get
		{
			return _Text;
		}
	}

	/// <summary>
	/// The row where the attack occurred
	/// </summary>
	/// <value>The row.</value>
	public int Row
	{
		get
		{
			return _Row;
		}
	}

	/// <summary>
	/// The column where the attack occurred
	/// </summary>
	/// <value>The column.</value>
	public int Column
	{
		get
		{
			return _Column;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:AttackResult"/> class.
	/// Set the _Value to the PossibleAttack value
	/// </summary>
	/// <param name="value">either hit, miss, destroyed, shotalready</param>
	/// <param name="text">Text.</param>
	/// <param name="row">Row.</param>
	/// <param name="column">Column.</param>
	public AttackResult(ResultOfAttack value, string text, int row, int column)
	{
		_Value = value;
		_Text = text;
		_Ship = null;
		_Row = row;
		_Column = column;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="T:AttackResult"/> class.
	/// Set the _Value to the PossibleAttack value, and the _Ship to the ship
	/// </summary>
	/// <param name="value">either hit, miss, destroyed, shotalready</param>
	/// <param name="ship">the ship information</param>
	/// <param name="text">Text.</param>
	/// <param name="row">Row.</param>
	/// <param name="column">Column.</param>
	public AttackResult(ResultOfAttack value, Ship ship, string text, int row, int column) : this(value, text, row, column)
	{
		_Ship = ship;
	}

	/// <summary>
	/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:AttackResult"/>.
	/// Displays the textual information about the attack
	/// </summary>
	/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:AttackResult"/>.</returns>
	public override string ToString()
	{
		if (_Ship == null)
		{
			return Text;
		}

		return Text + " " + _Ship.Name;
	}
}
