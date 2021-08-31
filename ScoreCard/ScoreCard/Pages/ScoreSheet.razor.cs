using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace ScoreCard.Pages
{
    public partial class ScoreSheet
    {
        private List<Player> _players = new List<Player>();

        [Parameter]
        public int Players { get; set; } = 3;

        [Parameter]
        public int Rounds { get; set; } = 7;

        protected override async Task OnParametersSetAsync()
        {
            await Task.Run(SetPlayers);
            await Read();
        }

        private void SetPlayers()
        {
            for (int i = 0; i < Players; i++)
            {
                _players.Add(new Player($"Player{i + 1}", Rounds));
            }
        }

        private async Task ClearAsync()
        {
            _players = new List<Player>();
            SetPlayers();
            await Delete();
        }

        private async Task OnScoreChangedAsync()
        {
            await Save();
        }

        public async Task Delete()
        {
            await JSRuntime.InvokeAsync<string>("localStorage.removeItem", "name");
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