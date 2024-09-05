using Oxide.Core.Plugins;
using Oxide.Core;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("Coordinate Bots", "David Ibáñez", "1.0.4")]
    [Description("Añade bots en múltiples zonas con patrullas que respetan las colisiones")]

    public class CoordinateBots : RustPlugin
    {
        private List<BotSpawnConfig> botSpawnConfigs = new List<BotSpawnConfig>
        {
            new BotSpawnConfig 
            { 
                Position = new Vector3(-858.87f, 14.65f, 195.37f), 
                BotCount = 2, 
                Waypoints = new List<Vector3>
                {
                    new Vector3(-836.71f, 14.65f, 148.19f),
                    new Vector3(-866.71f, 14.65f, 188.19f)
                }
            },
            new BotSpawnConfig 
            { 
                Position = new Vector3(-863.42f, 21.54f, 266.93f), 
                BotCount = 1, 
                Waypoints = new List<Vector3>
                {
                    new Vector3(-872.24f, 21.54f, 264.63f),
                    new Vector3(-860.48f, 21.54f, 266.66f)
                }
            },
            new BotSpawnConfig // Base de Piru 
            { 
                Position = new Vector3(-198.52f, 15.81f, 774.19f), 
                BotCount = 1, 
                Waypoints = new List<Vector3>
                {
                    new Vector3(-168.52f, 15.81f, 734.19f),
                    new Vector3(-200.52f, 15.81f, 784.19f)
                }
            }
        };

        private List<BaseEntity> spawnedBots = new List<BaseEntity>();

        private class BotSpawnConfig
        {
            public Vector3 Position { get; set; }
            public int BotCount { get; set; }
            public List<Vector3> Waypoints { get; set; }
        }

        private void OnServerInitialized()
        {
            foreach (var config in botSpawnConfigs)
            {
                SpawnBotsAtPosition(config);
            }
        }

        private void SpawnBotsAtPosition(BotSpawnConfig config)
        {
            for (int i = 0; i < config.BotCount; i++)
            {
                Vector3 spawnPosition = config.Position + new Vector3(UnityEngine.Random.Range(-10f, 10f), 0f, UnityEngine.Random.Range(-10f, 10f));
                BaseEntity bot = GameManager.server.CreateEntity("assets/rust.ai/agents/npcplayer/humannpc/scientist/scientistnpc_excavator.prefab", spawnPosition, Quaternion.identity);

                if (bot == null)
                {
                    Puts("Failed to create bot entity.");
                    continue;
                }

                bot.Spawn();
                spawnedBots.Add(bot);

                // Agregar el componente de AI de patrullaje con NavMesh
                PatrolAI patrolAI = bot.gameObject.AddComponent<PatrolAI>();
                if (patrolAI != null)
                {
                    patrolAI.SetWaypoints(config.Waypoints);
                }
                else
                {
                    Puts("Failed to add PatrolAI component.");
                }
            }
            Puts($"{config.BotCount} bots spawned at coordinates {config.Position}");
        }

        [ConsoleCommand("coordinatebots.respawn")]
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

            foreach (var config in botSpawnConfigs)
            {
                SpawnBotsAtPosition(config);
            }

            arg.ReplyWith("Bots respawned at specified coordinates.");
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

        public class PatrolAI : MonoBehaviour
        {
            private List<Vector3> waypoints;
            private int currentWaypoint = 0;
            private NavMeshAgent agent;
            private BaseEntity bot;
            private float moveSpeed = 3f;

            public void SetWaypoints(List<Vector3> waypoints)
            {
                this.waypoints = waypoints;
                bot = GetComponent<BaseEntity>();

                // Agregar y configurar NavMeshAgent
                agent = bot.gameObject.AddComponent<NavMeshAgent>();
                if (agent == null)
                {
                    Debug.LogError("NavMeshAgent could not be added to the bot.");
                    return;
                }

                agent.speed = moveSpeed;
                agent.angularSpeed = 120f;
                agent.acceleration = 8f;

                MoveToNextWaypoint();
            }

            private void MoveToNextWaypoint()
            {
                if (waypoints == null || waypoints.Count == 0 || agent == null)
                    return;

                agent.SetDestination(waypoints[currentWaypoint]);

                if (Vector3.Distance(bot.transform.position, waypoints[currentWaypoint]) < 1f)
                {
                    currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
                    MoveToNextWaypoint();
                }
            }

            private void Update()
            {
                if (agent != null && agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    currentWaypoint = (currentWaypoint + 1) % waypoints.Count;
                    MoveToNextWaypoint();
                }
            }
        }
    }
}
