using Oxide.Core.Plugins;
using Oxide.Core;
using UnityEngine;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("Monument Bots", "David Ibáñez", "1.0.1")]
    [Description("Añade bots en monumentos específicos")]

    public class MonumentBots : RustPlugin
    {
        // Configuración de monumentos y bots
        private Dictionary<string, int> monumentBotsConfig = new Dictionary<string, int>
        {
            //{"Oil Rig", 28}
        };

        private List<BaseEntity> spawnedBots = new List<BaseEntity>();

        private void OnServerInitialized()
        {
            foreach (var monument in monumentBotsConfig)
            {
                SpawnBotsAtMonument(monument.Key, monument.Value);
            }
        }

        private void SpawnBotsAtMonument(string monumentName, int botCount)
        {
            var monuments = TerrainMeta.Path.Monuments;
            foreach (var monument in monuments)
            {
                if (monument.displayPhrase.english.Contains(monumentName))
                {
                    for (int i = 0; i < botCount; i++)
                    {
                        Vector3 spawnPosition = monument.transform.position + new Vector3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10));
                        BaseEntity bot = GameManager.server.CreateEntity("assets/rust.ai/agents/npcplayer/humannpc/scientist/scientistnpc_oilrig.prefab", spawnPosition, Quaternion.identity);
                        if (bot != null)
                        {
                            bot.Spawn();
                            spawnedBots.Add(bot);
                        }
                    }
                    Puts($"{botCount} bots spawned at {monumentName}");
                }
            }
        }

        [ConsoleCommand("monumentbots.respawn")]
        private void RespawnBotsCommand(ConsoleSystem.Arg arg)
        {
            if (arg.Player() != null && !arg.Player().IsAdmin)
            {
                arg.ReplyWith("You don't have permission to use this command.");
                return;
            }

            foreach (var bot in spawnedBots)
            {
                if (!bot.IsDestroyed)
                {
                    bot.Kill();
                }
            }
            spawnedBots.Clear();

            foreach (var monument in monumentBotsConfig)
            {
                SpawnBotsAtMonument(monument.Key, monument.Value);
            }

            arg.ReplyWith("Bots respawned at configured monuments.");
        }

        private void Unload()
        {
            foreach (var bot in spawnedBots)
            {
                if (!bot.IsDestroyed)
                {
                    bot.Kill();
                }
            }
            spawnedBots.Clear();
        }
    }
}
