namespace GameFramework.AI.GOAP
{
    public sealed class GoapWorld
    {
        private static readonly GoapWorldState _world = new GoapWorldState();

        public GoapWorldState GetWorld => _world;
    }
}
