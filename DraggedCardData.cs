using Godot;

public class DraggedCardData(
    Card cardNode,
    Node2D cardParentNode,
    Vector2 relativeDragVector,
    int cardZIndexGlobal
    )
{
    private readonly Card _cardNode = cardNode;
    public Card CardNode
    {
        get
        {
            return _cardNode;
        }
    }

	private readonly Node2D _cardParentNode = cardParentNode;
    public Node2D CrardParentNode
    {
        get
        {
            return _cardParentNode;
        }
    }

	private readonly Vector2 _relativeDragVector = relativeDragVector;
    public Vector2 RelativeDragVector
    {
        get
        {
            return _relativeDragVector;
        }
    }

	private readonly int _cardZIndexGlobal = cardZIndexGlobal;
    public int CardZIndexGlobal
    {
        get
        {
            return _cardZIndexGlobal;
        }
    }
}
