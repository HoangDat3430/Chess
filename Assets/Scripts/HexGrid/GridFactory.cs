using System;
using System.Collections.Generic;

public class GridFactory : IGridFactory
{
    private Dictionary<GridType, Func<IGrid>> _gridRegistry = new();
    public GridFactory()
    {
        _gridRegistry.Add(GridType.Square, () => new SquareGrid());
        _gridRegistry.Add(GridType.Hexagon, () => new HexGrid());
    }
    public IGrid CreateGrid(GridType gridType)
    {
        if (_gridRegistry.TryGetValue(gridType, out var value))
        {
            return value();
        }
        throw new Exception($"Unknown grid type: {gridType}");
    }
}
