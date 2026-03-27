using Godot;
using System;

public partial class ChangeArrow : TextureRect
{
	[Export] public string arrowUpPath;
	[Export] public string arrowDownPath;
	public override void _Ready()
	{
		Update(0);
	}
    public void Update(uint points)
    {
		int sign = Math.Sign((new StatisticsData()).getChange(points));
		switch (sign)
		{
			case 1:
				Texture = (Texture2D)ResourceLoader.Load(arrowUpPath);
				break;
			case 0:
				Texture = null;
				break;
			case -1:
				Texture = (Texture2D)ResourceLoader.Load(arrowDownPath);
				break;
		}
    }
}
