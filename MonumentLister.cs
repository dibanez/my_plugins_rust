using System.Collections.Generic;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("MonumentLister", "David Ibáñez", "1.0.0")]
    [Description("Lista todos los monumentos en el mapa")]

    public class MonumentLister : RustPlugin
    {
        [ConsoleCommand("monuments.list")]
        private void ListMonumentsCommand(ConsoleSystem.Arg arg)
        {
            if (arg.Player() != null && !arg.Player().IsAdmin)
            {
                arg.ReplyWith("You don't have permission to use this command.");
                return;
            }

            var monuments = TerrainMeta.Path.Monuments;
            foreach (var monument in monuments)
            {
                string monumentInfo = $"Monument: {monument.displayPhrase.english} | Position: {monument.transform.position}";
                Puts(monumentInfo);
                arg.ReplyWith(monumentInfo);
            }
        }
    }
}