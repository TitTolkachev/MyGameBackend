namespace MyGameBackend
{
    public class PlayerModel
    {
        public string ConnectionId { get; set; }
        public int PlayerIndex { get; set; }

        public PlayerModel(string connectionId, int playerIndex)
        {
            ConnectionId = connectionId;
            PlayerIndex = playerIndex;
        }
    }
}
