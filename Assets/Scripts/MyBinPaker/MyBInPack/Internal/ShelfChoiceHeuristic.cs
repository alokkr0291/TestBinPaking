

namespace MobackPacker.Internal
{
    public enum ShelfChoiceHeuristic
    {
        ShelfNextFit, // We always put the new cuboid to the last open shelf.
        ShelfFirstFit, // We test each cuboid against each shelf in turn and
                       // pack it to the first where it fits.
    }
    
    
    public enum FreeRectChoiceHeuristic
    {
        RectBestAreaFit,
        RectBestShortSideFit,
    }
    
    public enum GuillotineSplitHeuristic
    {
        SplitShorterLeftoverAxis,
        SplitLongerLeftoverAxis,
        SplitShorterAxis,
        SplitLongerAxis
    }
    
    public enum FreeCuboidChoiceHeuristic
    {
        CuboidMinHeight,
        CuboidMinWidth
    }
}
