using Godot;

namespace Diamondmine.scripts.UI;
public partial class Average : Label
{
	[Export] public string BaseText;

	public void Update()
    {
        Text = BaseText + (new StatisticsData()).GetAverage().ToString("f2");
    }
}
