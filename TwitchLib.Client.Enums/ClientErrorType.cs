using System;
using System.Collections.Generic;
using System.Text;

namespace TwitchLib.Client.Enums
{
    public enum ClientErrorType
    {
        UnaccountedFor,
        InvalidLogin,
        FailureToReceiveJoinConfirmation,
        WhisperRestricted,
        WhisperRestrictedClient
    }
}
