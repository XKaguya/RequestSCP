using System;
using Exiled.API.Features;
using RequestSCP.Commands;

namespace RequestSCP
{
    public class RequestSCP : Plugin<Config>
    {
        public override string Author => "RedLeaves";
        public override string Name => "RequestSCP";

        public override Version Version { get; } = new(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new(8, 0, 1);

        public static RequestSCP Instance;

        public override void OnEnabled()
        {
            Instance = this;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            base.OnDisabled();
        }

        private void OnRoundStarted()
        {
            Request.ResetRequestStatus();
        }
    }
}
