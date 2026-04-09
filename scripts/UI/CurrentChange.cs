using Godot;

namespace Diamondmine.scripts.UI;

public partial class CurrentChange : Label
{
	public void Update(uint points)
    {
        Text = (new StatisticsData()).GetChange(points).ToString("f2");
    }
}
