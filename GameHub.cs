using Microsoft.AspNetCore.SignalR;

namespace MyGameBackend
{
    public class GameHub : Hub
    {
        private GameBroadcaster _broadcaster;

        public GameHub(GameBroadcaster broadcaster)
        {
            _broadcaster = broadcaster;
        }

        public PlayerModel RegisterPlayer(PlayerModel playerModel)
        {
            playerModel.ConnectionId = Context.ConnectionId;
            _broadcaster.AddPlayerInGame(playerModel);

            return playerModel;
        }

        public SyncObjectModel MovePlayer(double x, double y)
        {
            SyncObjectModel player = new(x, y, 0, Context.ConnectionId);
            _broadcaster.UpdateModel(player);

            return player;
        }
    }
}
