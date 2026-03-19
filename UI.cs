using Godot;

public partial class UI : Control
{
	[Export] public PointsCounter PointsCounter {get;set;}
	[Export] public Button UndoButton {get;set;}
	[Export] public Button RedoButton {get;set;}
	[Export] public Button NewGameButton {get;set;}
	[Export] public Label PlayerMessage {get;set;}




	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


}
