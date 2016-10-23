using Microsoft.Xna.Framework.Net;

namespace RockRainLive
{
    /// <summary>
    /// Helper for network services
    /// </summary>
    class NetworkHelper
    {
        // NetworkStuff
        private NetworkSession networkSession;
        private readonly PacketWriter serverPacketWriter = new PacketWriter();
        private readonly PacketReader serverPacketReader = new PacketReader();
        private readonly PacketWriter clientPacketWriter = new PacketWriter();
        private readonly PacketReader clientPacketReader = new PacketReader();

        /// <summary>
        /// The active network session
        /// </summary>
        public NetworkSession NetworkGameSession
        {
            get { return networkSession; }
            set { networkSession = value; }
        }

        /// <summary>
        /// Writer for the server data
        /// </summary>
        public PacketWriter ServerPacketWriter
        {
            get { return serverPacketWriter; }
        }

        /// <summary>
        /// Writer for the client data
        /// </summary>
        public PacketWriter ClientPacketWriter
        {
            get { return clientPacketWriter; }
        }

        /// <summary>
        /// Reader for the client data
        /// </summary>
        public PacketReader ClientPacketReader
        {
            get { return clientPacketReader; }
        }

        /// <summary>
        /// Reader for the server data
        /// </summary>
        public PacketReader ServerPacketReader
        {
            get { return serverPacketReader; }
        }

        /// <summary>
        /// Send all server data
        /// </summary>
        public void SendServerData()
        {
            if (ServerPacketWriter.Length > 0)
            {
                // Send the combined data to everyone in the session.
                LocalNetworkGamer server = (LocalNetworkGamer) networkSession.Host;

                server.SendData(ServerPacketWriter, SendDataOptions.InOrder);
            }
        }

        /// <summary>
        /// Read server data
        /// </summary>
        public NetworkGamer ReadServerData(LocalNetworkGamer gamer)
        {
            NetworkGamer sender;

            // Read a single packet from the network.
            gamer.ReceiveData(ServerPacketReader, out sender);
            return sender;
        }

        /// <summary>
        /// Send all client data
        /// </summary>
        public void SendClientData()
        {
            if (ClientPacketWriter.Length > 0)
            {
                // The first player is always running in the server...
                networkSession.LocalGamers[0].SendData(clientPacketWriter,
                                                       SendDataOptions.InOrder,
                                                       networkSession.Host);
            }
        }

        /// <summary>
        /// Read the Client Data
        /// </summary>
        public NetworkGamer ReadClientData(LocalNetworkGamer gamer)
        {
            NetworkGamer sender;

            // Read a single packet from the network.
            gamer.ReceiveData(ClientPacketReader, out sender);
            return sender;
        }
    }
}
