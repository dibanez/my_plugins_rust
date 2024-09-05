using Oxide.Core.Plugins;
using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Event Controller", "David Ibáñez", "1.0.2")]
    [Description("Permite a los administradores lanzar eventos como el barco, helicóptero y aviones de suministro")]

    public class EventController : RustPlugin
    {
        [ChatCommand("event")]
        private void EventCommand(BasePlayer player, string command, string[] args)
        {
            if (!player.IsAdmin)
            {
                player.ChatMessage("No tienes permiso para usar este comando.");
                return;
            }

            if (args.Length == 0)
            {
                player.ChatMessage("Uso: /event <cargo|heli|drop>");
                return;
            }

            switch (args[0].ToLower())
            {
                case "cargo":
                    CallCargoShip(player);
                    break;
                case "heli":
                    CallHelicopter(player);
                    break;
                case "drop":
                    CallSupplyDrop(player);
                    break;
                default:
                    player.ChatMessage("Evento no reconocido. Usa: /event <cargo|heli|drop>");
                    break;
            }
        }

        private void CallCargoShip(BasePlayer player)
        {
            var cargoShips = UnityEngine.Object.FindObjectsOfType<CargoShip>();

            if (cargoShips.Length > 0)
            {
                player.ChatMessage("El barco de carga ya está activo.");
                return;
            }

            CargoShip cargoShip = GameManager.server.CreateEntity("assets/content/vehicles/boats/cargoship/cargoshiptest.prefab", new Vector3(), new Quaternion(), true) as CargoShip;

            if (cargoShip != null)
            {
                cargoShip.Spawn();
                player.ChatMessage("El barco de carga ha sido lanzado.");
            }
            else
            {
                player.ChatMessage("No se pudo iniciar el barco de carga.");
            }
        }

        private void CallHelicopter(BasePlayer player)
        {
            BaseHelicopter helicopter = (BaseHelicopter)GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", Vector3.zero, Quaternion.identity, true);

            if (helicopter != null)
            {
                helicopter.Spawn();
                player.ChatMessage("El helicóptero ha sido lanzado.");
            }
            else
            {
                player.ChatMessage("No se pudo lanzar el helicóptero.");
            }
        }

        private void CallSupplyDrop(BasePlayer player)
        {
            Vector3 dropPosition = player.transform.position + new Vector3(0, 10, 0); // Posición sobre el jugador
            CargoPlane cargoPlane = (CargoPlane)GameManager.server.CreateEntity("assets/prefabs/npc/cargo plane/cargo_plane.prefab", new Vector3(), new Quaternion(), true);

            if (cargoPlane != null)
            {
                cargoPlane.transform.position = dropPosition + new Vector3(0, 1000, 0); // Posición muy arriba para que caiga
                cargoPlane.Spawn();
                player.ChatMessage("El avión de suministro ha sido lanzado.");
            }
            else
            {
                player.ChatMessage("No se pudo lanzar el avión de suministro.");
            }
        }
    }
}
