using Microsoft.AspNetCore.SignalR;
using System.Numerics;

namespace MyGameBackend
{
    public class GameHub : Hub
    {
        private GameBroadcaster _broadcaster;

        public GameHub(GameBroadcaster broadcaster)
        {
            _broadcaster = broadcaster;
        }

        public void RegisterPlayer(SyncObjectModel player)
        {
            player.Authority = Context.ConnectionId;
            _broadcaster.AddPlayerInGame(player);
        }

        public void MovePlayer(SyncObjectModel player)
        {
            player.Authority = Context.ConnectionId;
            _broadcaster.UpdateModel(player);
        }

        public void RefreshPlayer(SyncObjectModel player)
        {
            player.Authority = Context.ConnectionId;
            _broadcaster.RefreshPlayer(player);
        }

        public void GeneratePlayerId()
        {
            _broadcaster.SendId(Context.ConnectionId);
        }

        public void AddProjectile(ProjectileModel projectile)
        {
            _broadcaster.AddProjectile(projectile);
        }
    }
}
