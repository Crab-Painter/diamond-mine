using Godot;

public partial class UI : Control
{
	[Export] public Button UndoButton {get;set;}
	[Export] public Button RedoButton {get;set;}
	[Export] public Button NewGameButton {get;set;}
	[Export] public Label PlayerMessage {get;set;}
	[Export] public PointsCounter PointsCounter {get;set;}
	[Export] public ChangeArrow ChangeArrow {get;set;}
	[Export] public CurrentChange CurrentChange {get;set;}
	[Export] public Average Average {get;set;}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdatePoints(0);
	}

	public void UpdatePoints(uint points)
    {
        PointsCounter.Update(points);
        CurrentChange.Update(points);
		ChangeArrow.Update(points);
        Average.Update();
    }


}
