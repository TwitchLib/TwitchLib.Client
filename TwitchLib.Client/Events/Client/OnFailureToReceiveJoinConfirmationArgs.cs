using System;

namespace TwitchLib.Client.Events.Client
{
    public class OnFailureToReceiveJoinConfirmationArgs : EventArgs
    {
        public FailureToReceiveJoinConfirmationException Exception;
    }
}
