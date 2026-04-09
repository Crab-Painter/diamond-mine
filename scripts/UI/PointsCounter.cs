using Godot;

namespace Diamondmine.scripts.UI;

public partial class PointsCounter : Label
{
    
    [Export] public string BaseText;
    public void Update(uint points)
    {
        Text = BaseText + points;
    }
}
