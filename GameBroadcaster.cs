using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace MyGameBackend
{
    public class GameBroadcaster
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly TimeSpan BroadcastInterval =
            TimeSpan.FromMilliseconds(100);
        private Timer _broadcastLoop;
        private bool _modelUpdated;

        ConcurrentQueue<SyncObjectModel> _oneFrameSyncModels = new ConcurrentQueue<SyncObjectModel>();
        ConcurrentQueue<PlayerModel> _players = new ConcurrentQueue<PlayerModel>();

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
                        _hubContext.Clients.AllExcept(model.Authority).SendAsync("Receive", model);
                    //else удалить его из _players
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
            // Добавить проверку
            int playersCount = _players.Count;
            player.PlayerIndex = playersCount;

            _hubContext.Clients.Client(player.ConnectionId).SendAsync("AddPlayers", _players.Select(o => new SyncObjectModel(400, 400, o.PlayerIndex, o.ConnectionId)).ToList());
            _hubContext.Clients.AllExcept(player.ConnectionId).SendAsync("AddPlayers", new List<SyncObjectModel>() { new SyncObjectModel(400, 400, player.PlayerIndex, player.ConnectionId) });
            _players.Enqueue(player);

        }
    }
}
