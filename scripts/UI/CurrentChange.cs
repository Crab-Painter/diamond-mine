using Godot;

namespace Diamondmine.scripts.UI;

public partial class CurrentChange : Label
{
	public void Update(uint points)
    {
        Text = StatisticsData.GetAverageChange(points).ToString("f2");
    }
}
