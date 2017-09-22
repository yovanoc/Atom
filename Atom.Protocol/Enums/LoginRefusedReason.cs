namespace Atom.Protocol.Enums
{
    public enum LoginRefusedReason
    {
        Unauthorized = 0,
        NotFound = 1,
        Banned = 2,
        AwaitingActivation = 3,
        WrongPassword = 4
    }
}