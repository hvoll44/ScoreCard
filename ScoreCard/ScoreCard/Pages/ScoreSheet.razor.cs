using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;
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
            _players.Add(new Player($"Newb", _rounds));
            await Save();
        }

        private async Task AddRoundsAsync()
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

        public async Task Delete()
        {
            await JSRuntime.InvokeAsync<string>("localStorage.removeItem", "name");
        }

        private async Task OnChangedAsync()
        {
            await Save();
        }


        public async Task Read()
        {
            var json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "name");

            if (json != null)
            {
                _players = JsonSerializer.Deserialize<List<Player>>(json);
            }
        }

        public async Task Save()
        {
            var json = JsonSerializer.Serialize(_players);
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", "name", json);
        }
    }
}