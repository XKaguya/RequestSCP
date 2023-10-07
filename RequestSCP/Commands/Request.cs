using System;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using PlayerRoles;
using System.Linq;
using Exiled.API.Enums;
using System.Collections.Generic;

namespace RequestSCP.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Request : ICommand
    {
        public string Command => "request";
        public string Description => "�����Ϊָ����SCP / Request to be specific SCP.";
        public string[] Aliases => Array.Empty<string>();

        private static HashSet<Player> requestedPlayers = new HashSet<Player>();
        private static DateTime roundStartTime;

        public static void ResetRequestStatus()
        {
            requestedPlayers.Clear();
            roundStartTime = DateTime.Now;
        }

        public bool HandleScpRoleChange(Player player, string roleCode, RoleTypeId targetRole, int requiredScps, out string response)
        {
            response = "";

            var scpPlayers = Player.List.Where(p => p.Role.Side == Side.Scp && p.Role.Type == targetRole).ToList();

            if ((targetRole == RoleTypeId.Scp096 || targetRole == RoleTypeId.Scp173) && scpPlayers.Count > 0 && Player.List.Count() < 12)
            {
                response = $"��ǰ��Ϸ���Ѿ�����{roleCode}��������Ϸ����С��12�ˣ��޷������Ϊ{roleCode}�� / There's already a {roleCode} in the game, and the player count is less than 12, cannot request {roleCode}.";
                Request.ResetRequestStatus();
                return false;
            }

            if (scpPlayers.Count > 0)
            {
                response = $"������Ϸ���Ѿ�����һ��{roleCode}�ˡ� / There's already a {roleCode} in the game.";
                Request.ResetRequestStatus();
                return false;
            }

            var allScpPlayers = Player.List.Where(p => p.Role.Side == Side.Scp).ToList();

            if (allScpPlayers.Count >= requiredScps)
            {
                player.Role.Set(targetRole);
                response = $"�ɹ������{player.Nickname}�Ľ�ɫ����Ϊ{roleCode}�� / Successfully changed {player.Nickname}'s role to {roleCode}.";
                return true;
            }
            else
            {
                response = "����δ���㡣 / Condition not fit.";
                Request.ResetRequestStatus();
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
                response = "�������� / Argument error.";
                return false;
            }

            if (player.Role.Side != Side.Scp)
            {
                response = "ֻ��SCP����ִ�б���� / Only SCP player can execute this command.";
                return false;
            }

            if (DateTime.Now - roundStartTime > TimeSpan.FromSeconds(180))
            {
                response = "��Ϸ�Ѿ���ʼ�˳���3���ӣ��޷��ٽ������� / The game has been running for more than 3 minutes, you cannot request anymore.";
                return false;
            }

            if (!requestedPlayers.Contains(player))
            {
                requestedPlayers.Add(player);
                string roleName = argsArray[0];
                return HandleScpRoleChange(player, roleName, GetTargetRole(roleName), GetRequiredScps(roleName), out response);
            }
            else
            {
                response = "��Ϸ��ֻ������һ�Σ���������Ϸ��ʼ���3�����ڽ�ֹ���롣 / You can only request once in a round.And you cannot request after round start in 3 minutes.";
                return false;
            }
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
                return 3;
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
