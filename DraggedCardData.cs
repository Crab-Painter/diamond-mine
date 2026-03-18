using Godot;

public class DraggedCardData(
    Card cardNode,
    Area2D cardParentNode,
    Vector2 relativeDragVector,
    int cardZIndexGlobal,
    bool wasParentClosed
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

	private readonly Area2D _cardParentNode = cardParentNode;
    public Area2D CrardParentNode
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

    private readonly bool _wasParentClosed = wasParentClosed;
    public bool WasParentClosed
    {
        get
        {
            return _wasParentClosed;
        }
    }
}
