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

        public SyncObjectModel RegisterPlayer(SyncObjectModel player)
        {
            player.Authority = Context.ConnectionId;
            _broadcaster.AddPlayerInGame(player);

            return player;
        }

        public SyncObjectModel MovePlayer(double x, double y)
        {
            SyncObjectModel player = new(x, y, 0, Context.ConnectionId);
            _broadcaster.UpdateModel(player);

            return player;
        }

        public void RefreshPlayer(SyncObjectModel player)
        {
            player.Authority = Context.ConnectionId;
            _broadcaster.RefreshPlayer(player);
        }
    }
}
