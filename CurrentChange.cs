using Godot;
using System;

public partial class CurrentChange : Label
{
	public void Update(uint points)
    {
        Text = (new StatisticsData()).getChange(points).ToString("f2");
    }
}
