using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using BiotonicFrontiers.Core;
using BiotonicFrontiers.Net;
using BiotonicFrontiers.Data.DTO;

namespace BiotonicFrontiers.UI.Map
{
    /// <summary>Full-screen land grid, now wired to real ownership + claiming.</summary>
    public class LandMapUI : MonoBehaviour
    {
        [SerializeField] private RectTransform gridRoot;
        [SerializeField] private LandTile      tilePrefab;
        [SerializeField] private Color         ownedColor = new(0.15f, 0.65f, 1f, 0.85f);

        private readonly Dictionary<(int,int), LandTile> _tiles = new();

        // ------------------------------------------------------------------ lifecycle
        private async void Start()
        {
            // TODO: width/height from server config once exposed
            BuildGrid(64, 64);
            await RefreshOwnedAsync();
        }

        // ------------------------------------------------------------------ public
        public void HandleTileClick(int x, int y) =>
            NetworkManager.Instance.ClaimLand(x, y);   // will echo back via WS → EventBus

        // ------------------------------------------------------------------ helpers
        private void BuildGrid(int w, int h)
        {
            for (int y = 0; y < h; ++y)
            for (int x = 0; x < w; ++x)
            {
                var t = Instantiate(tilePrefab, gridRoot);
                t.Init(x, y, Color.white);
                _tiles[(x,y)] = t;
            }
        }

        private async UniTask RefreshOwnedAsync()
        {
            var pid     = System.Guid.Parse(GameManager.Instance.PlayerId);
            var parcels = await LandService.GetOwnedParcelsAsync(pid);
            HighlightOwned(parcels.ConvertAll(p => (p.x, p.y)));
        }

        private void HighlightOwned(IEnumerable<(int,int)> coords)
        {
            foreach (var (x,y) in coords)
                if (_tiles.TryGetValue((x,y), out var tile))
                    tile.GetComponent<Image>().color = ownedColor;
        }
    }
}
