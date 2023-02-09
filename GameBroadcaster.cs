using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace MyGameBackend
{
    public class GameBroadcaster
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly TimeSpan BroadcastInterval =
            TimeSpan.FromMilliseconds(40);
        private readonly TimeSpan RefreshClientsPlayersListInterval =
            TimeSpan.FromMilliseconds(1500);
        private Timer _broadcastLoop;
        private Timer _refreshClientsPlayersListLoop;
        private bool _modelUpdated;
        private int _playerId = 1;

        //ConcurrentQueue<SyncObjectModel> _oneFrameSyncModels = new();
        //ConcurrentQueue<PlayerModel> _players = new();

        ConcurrentQueue<SyncObjectModel> _players = new();
        ConcurrentQueue<SyncObjectModel> _refreshedPlayers = new();
        ConcurrentQueue<SyncObjectModel> _oneFrameSyncModels = new();

        public GameBroadcaster(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
            _modelUpdated = false;
            // Start the broadcast loop
            _broadcastLoop = new Timer(
                Broadcast,
                null,
                BroadcastInterval,
                BroadcastInterval);
            _refreshClientsPlayersListLoop = new Timer(
                RefreshClientsPlayersList,
                null,
                RefreshClientsPlayersListInterval,
                RefreshClientsPlayersListInterval
                );
        }
        public void Broadcast(object state)
        {
            // No need to send anything if our model hasn't changed
            if (_modelUpdated)
            {
                foreach (var model in _oneFrameSyncModels)
                {
                    //if (_players.Any(p => p.ConnectionId == model.Authority))
                    //{
                    _hubContext.Clients.Client(model.Authority).SendAsync("Receive", new SyncObjectModel(model.X, model.Y, model.Id, null));
                    _hubContext.Clients.AllExcept(model.Authority).SendAsync("Receive", model);
                    //}
                }
                _modelUpdated = false;
                _oneFrameSyncModels.Clear();
            }
        }
        public void UpdateModel(SyncObjectModel clientModel)
        {
            _oneFrameSyncModels.Enqueue(clientModel);

            _modelUpdated = true;
        }
        public void AddPlayerInGame(SyncObjectModel player)
        {
            //if (!_players.Any(p => p.ConnectionId == player.ConnectionId))
            //{
            _hubContext.Clients.Client(player.Authority).SendAsync("AddPlayers", _players);
            _players.Enqueue(player);
            _refreshedPlayers.Enqueue(player);
            _hubContext.Clients.AllExcept(player.Authority).SendAsync("AddPlayers", new List<SyncObjectModel>() { player });
            //}
        }
        public void RefreshPlayer(SyncObjectModel player)
        {
            _refreshedPlayers.Enqueue(player);
        }
        public void RefreshClientsPlayersList(object state)
        {
            if (!_refreshedPlayers.IsEmpty)
            {
                _players = _refreshedPlayers;
                _refreshedPlayers = new();
            }
            _hubContext.Clients.All.SendAsync("RefreshPlayersList", _players);
        }

        public void SendId(string authority)
        {
            _hubContext.Clients.Client(authority).SendAsync("PlayerId", _playerId++);
        }
    }
}
