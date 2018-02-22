using System;
using TwitchLib.Client.Exceptions;

namespace TwitchLib.Client.Events
{
    public class OnFailureToReceiveJoinConfirmationArgs : EventArgs
    {
        public FailureToReceiveJoinConfirmationException Exception;
    }
}
