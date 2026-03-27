using Godot;
using System;

public partial class Average : Label
{
	[Export] public string BaseText;

	public void Update()
    {
        Text = BaseText + (new StatisticsData()).getAverage().ToString("f2");
    }
}
