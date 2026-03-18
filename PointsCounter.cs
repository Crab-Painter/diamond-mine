using Godot;
using System;

public partial class PointsCounter : Label
{
    
    [Export] public string BaseText;
    public override void _Ready()
	{
		UpdatePoints(0);
		
	}
    public void UpdatePoints(int points)
    {
        Text = BaseText + points;
    }
}
