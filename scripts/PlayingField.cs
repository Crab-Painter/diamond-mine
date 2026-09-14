using Godot;
using System;

namespace Diamondmine.scripts;
public partial class PlayingField : Node2D
{
	[Export] public Foundation DiamondFoundation {get;set;}
	[Export] public Timer DoubleClickTimer {get;set;}
	[Export] public Node2D FoundationsNode {get;set;}

	public Foundation GetFoundationNode(int id)
	{
		return FoundationsNode.GetNode<Foundation>("Foundation"+id);
	}
}
