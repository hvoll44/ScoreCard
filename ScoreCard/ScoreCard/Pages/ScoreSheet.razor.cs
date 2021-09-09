using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ScoreCard.Models;
using System.Linq;

namespace ScoreCard.Pages
{
    public partial class ScoreSheet
    {
        private List<Player> _players = new List<Player>();
        private int _rounds;

        [Parameter]
        public int Players { get; set; } = 3;

        [Parameter]
        public int Rounds { get; set; } = 7;

        protected override async Task OnParametersSetAsync()
        {
            _rounds = Rounds;
            await Task.Run(SetPlayers);
            await Read();
            _rounds = _players.OrderByDescending(p => p.Scores.Count).First().Scores.Count;
        }

        private void SetPlayers()
        {
            for (int i = 0; i < Players; i++)
            {
                _players.Add(new Player($"Player{i + 1}", _rounds));
            }
        }

        private async Task AddPlayerAsync()
        {
            _players.Add(new Player($"Player{_players.Count + 1}", _rounds));
            await Save();
        }

        private async Task AddRoundAsync()
        {
            _rounds++;
            foreach (var player in _players)
            {
                while (player.Scores.Count < _rounds)
                {
                    player.Scores.Add(null);
                }
            }
            StateHasChanged();
            await Save();
        }

        private async Task ClearAsync()
        {
            _rounds = Rounds;
            _players = new List<Player>();
            SetPlayers();
            await Delete();
        }

        private async Task Delete()
        {
            await JSRuntime.InvokeAsync<string>("localStorage.removeItem", "players");
        }

        private async Task DeletePlayer(int playerIndex)
        {
            if (_players.Count > 1)
            {
                _players.RemoveAt(playerIndex);
            }
        }

        private async Task OnChangedAsync()
        {
            await Save();
        }

        private async Task OnPlayerBoxClicked(string id)
        {
            await Focus(id);
        }

        private async Task Focus(string elementId)
        {
            await JSRuntime.InvokeVoidAsync("jsfunction.focusElement", elementId);
        }

        private async Task Read()
        {
            var json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "players");

            if (json != null)
            {
                _players = JsonSerializer.Deserialize<List<Player>>(json);
            }
        }

        private async Task Save()
        {
            var json = JsonSerializer.Serialize(_players);
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", "players", json);
        }
    }
}