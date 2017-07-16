namespace TowerDefenseColab.GameBusHere.Messages
{
    class MessageMouseDragged : IGameMessage
    {
        public int ChangeX { get; set; }
        public int ChangeY { get; set; }
    }
}
