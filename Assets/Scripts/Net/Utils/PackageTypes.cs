namespace Net.Utils
{
    public enum PackageType {
        ConnectPackage,  //client -> server
        DisconnectPackage, //client -> server
        AcceptPackage, //server -> client
        DeclinePackage, //server -> client
        EventPackage, //client -> server, server -> client?
        StatePackage //server -> client, multicast
    }
}
