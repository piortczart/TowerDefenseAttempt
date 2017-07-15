namespace TowerDefenseColab.GameBusHere
{
    class MessageMouseDragged : IGameMessage
    {
        public int ChangeX { get; set; }
        public int ChangeY { get; set; }
    }
}
