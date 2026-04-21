using Godot;

namespace Diamondmine.scripts;

public partial class Foundation : Area2D, IHighlightable
{
	[Export] public string pathToHighlighter;
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

	public void HighlightOn()
	{
		var highlighter = (Sprite2D)GetNode(pathToHighlighter);
		highlighter.Visible = true;
	}
	public void HighlightOff()
	{
		var highlighter = (Sprite2D)GetNode(pathToHighlighter);
		highlighter.Visible = false;
	}
}
