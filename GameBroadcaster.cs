using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace MyGameBackend
{
    public class GameBroadcaster
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly TimeSpan BroadcastInterval =
            TimeSpan.FromMilliseconds(40);
        private Timer _broadcastLoop;
        private bool _modelUpdated;

        //ConcurrentQueue<SyncObjectModel> _oneFrameSyncModels = new();
        //ConcurrentQueue<PlayerModel> _players = new();

        List<PlayerModel> _players = new();
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
        }
        public void Broadcast(object state)
        {
            // No need to send anything if our model hasn't changed
            if (_modelUpdated)
            {
                foreach (var model in _oneFrameSyncModels)
                {
                    if (_players.Any(p => p.ConnectionId == model.Authority))
                    {
                        _hubContext.Clients.Client(model.Authority).SendAsync("Receive", new SyncObjectModel(model.X, model.Y, model.Id, null));
                        _hubContext.Clients.AllExcept(model.Authority).SendAsync("Receive", model);
                    }
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
        public void AddPlayerInGame(PlayerModel player)
        {
            if (!_players.Any(p => p.ConnectionId == player.ConnectionId))
            {
                int playersCount = _players.Count;
                player.PlayerIndex = playersCount;

                // Это нужно правильно переписать!!!
                _hubContext.Clients.Client(player.ConnectionId).SendAsync("AddPlayers", _players.Select(o => new SyncObjectModel(400, 400, o.PlayerIndex, o.ConnectionId)).ToList());
                _hubContext.Clients.AllExcept(player.ConnectionId).SendAsync("AddPlayers", new List<SyncObjectModel>() { new SyncObjectModel(400, 400, player.PlayerIndex, player.ConnectionId) });
                _players.Add(player);
            }
        }
    }
}
