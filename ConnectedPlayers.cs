using Oxide.Core.Plugins;
using System.Collections.Generic;
using Oxide.Core;
using System;

namespace Oxide.Plugins
{
    [Info("Connected Players", "David Ibáñez", "1.0.0")]
    [Description("Muestra a los administradores la lista de jugadores conectados")]

    public class ConnectedPlayers : RustPlugin
    {
        [ChatCommand("listplayers")]
        private void ListPlayersCommand(BasePlayer player, string command, string[] args)
        {
            if (!player.IsAdmin)
            {
                player.ChatMessage("No tienes permiso para usar este comando.");
                return;
            }

            List<string> connectedPlayers = new List<string>();
            foreach (var connectedPlayer in BasePlayer.activePlayerList)
            {
                connectedPlayers.Add($"{connectedPlayer.displayName} (SteamID: {connectedPlayer.UserIDString})");
            }

            if (connectedPlayers.Count == 0)
            {
                player.ChatMessage("No hay jugadores conectados.");
            }
            else
            {
                player.ChatMessage("Jugadores conectados:\n" + string.Join("\n", connectedPlayers));
            }
        }
    }
}
