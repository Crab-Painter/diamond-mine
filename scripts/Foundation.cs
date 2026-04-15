using Godot;

namespace Diamondmine.scripts;

public partial class Foundation : Area2D
{
	public Area2D furtestCard;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		furtestCard = this;
	}
	    
	public override string ToString()
    {
		string result = Name + " Collision layer is " + CollisionLayer.ToString();
		result += " furtestCard is ";
		result += furtestCard == this ? "this" : furtestCard.ToString();
        return base.ToString() + result;
    }
}
