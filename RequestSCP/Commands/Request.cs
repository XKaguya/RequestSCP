using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using PlayerRoles;
using System.Linq;
using Exiled.API.Enums;

namespace RequestSCP.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Request : ICommand
    {
        private static bool hasRequestedThisRound = false;
        private static DateTime roundStartTime;

        public string Command => "request";
        public string Description => "申请成为指定的SCP / Request to be specific SCP.";
        public string[] Aliases => Array.Empty<string>();

        public bool HandleScpRoleChange(Player player, string roleCode, RoleTypeId targetRole, int requiredScps, out string response)
        {
            response = "";

            var scpPlayers = Player.List.Where(p => p.Role.Side == Side.Scp && p.Role.Type == targetRole).ToList();

            if (scpPlayers.Count > 0)
            {
                response = $"本局游戏中已经存在一个{roleCode}了。 / There's already a {roleCode} in the game.";
                hasRequestedThisRound = false;
                return false;
            }

            var allScpPlayers = Player.List.Where(p => p.Role.Side == Side.Scp).ToList();

            if (allScpPlayers.Count >= requiredScps)
            {
                player.Role.Set(targetRole);
                response = $"成功将玩家{player.Nickname}的角色更换为{roleCode}。 / Successfully changed {player.Nickname}'s role to {roleCode}.";
                return true;
            }
            else
            {
                response = "条件未满足。 / Condition not fit.";
                hasRequestedThisRound = false;
                return false;
            }
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender))
            {
                response = "Error. Only players can use this command.";
                return false;
            }

            Player player = Player.Get(sender);

            string[] argsArray = arguments.ToArray();

            if (argsArray.Length != 1)
            {
                response = "参数错误。 / Argument error.";
                return false;
            }

            if (player.Role.Side != Side.Scp)
            {
                response = "只有SCP可以执行本命令。 / Only SCP player can execute this command.";
                return false;
            }

            if (hasRequestedThisRound || (DateTime.Now - roundStartTime).TotalSeconds < 180)
            {
                response = "游戏内只能申请一次，并且在游戏开始后的3分钟内禁止申请。 / You can only request once in a round.And you cannot request after round start in 3 minutes.";
                return false;
            }

            string roleName = argsArray[0];
            hasRequestedThisRound = true;
            return HandleScpRoleChange(player, roleName, GetTargetRole(roleName), GetRequiredScps(roleName), out response);
        }

        private RoleTypeId GetTargetRole(string roleName)
        {
            switch (roleName)
            {
                case "106":
                    return RoleTypeId.Scp106;
                case "173":
                    return RoleTypeId.Scp173;
                case "096":
                    return RoleTypeId.Scp096;
                case "079":
                    return RoleTypeId.Scp079;
                case "939":
                    return RoleTypeId.Scp939;
                case "049":
                    return RoleTypeId.Scp049;
                default:
                    return RoleTypeId.None;
            }
        }

        private int GetRequiredScps(string roleName)
        {
            if (roleName == "096")
            {
                return 2;
            }
            else if (roleName == "079")
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
    }
}
