using Godot;
using System;

public partial class PointsCounter : Label
{
    
    [Export] public string BaseText;
    public void Update(uint points)
    {
        Text = BaseText + points;
    }
}
