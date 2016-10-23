using System;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace XNADemo
{
    class NetworkHelper
    {
        
        private NetworkSession session = null;  //  The game session 
        private int maximumGamers = 2;  // Only 2 will play here, but a game can have up to 31 gamers
        private int maximumLocalPlayers = 1;  //  no split-screen, only remote players
        IAsyncResult AsyncSessionFind = null; // Used for asynchronous session finding
        PacketWriter packetWriter = new PacketWriter();  //  packet writer used to send data
        PacketReader packetReader = new PacketReader();  //  packet reader used to receive data

        public NetworkSessionState SessionState
        {
            get
            {
                if (session == null)
                    return NetworkSessionState.Ended;
                else
                    return session.SessionState; 
            }
        }

        //  Message regarding the session current state 
        private String message = "Waiting for user command...";
        public String Message
        {
            get { return message; }
        }

        public void SignInGamer()
        {
            if (!Guide.IsVisible)
            {
                Guide.ShowSignIn(1, false);
            }
        }

        public void Update()
        {
            // Pump the underlying session object.
            if (session != null)
                session.Update(); 
        }

        public void CreateSession()
        {
            if (session == null)
            {
                // Create the session
                session = NetworkSession.Create(
                    NetworkSessionType.SystemLink,
                    maximumLocalPlayers, maximumGamers);

                //--  Configure session behaviour
                //  If the host goes out, another machine will assume as a new host
                session.AllowHostMigration = true;
                //  Allow players to join a game in progress
                session.AllowJoinInProgress = true;

                //--  Configure the session events
                session.GamerJoined += new EventHandler<GamerJoinedEventArgs>(session_GamerJoined);
                session.GamerLeft += new EventHandler<GamerLeftEventArgs>(session_GamerLeft);
                session.GameStarted += new EventHandler<GameStartedEventArgs>(session_GameStarted);
                session.GameEnded += new EventHandler<GameEndedEventArgs>(session_GameEnded);
                session.SessionEnded += new EventHandler<NetworkSessionEndedEventArgs>(session_SessionEnded);
                session.HostChanged += new EventHandler<HostChangedEventArgs>(session_HostChanged);
            }
        }

        public void FindSession()
        {
            AvailableNetworkSessionCollection availableSessions;  //  all sessions found
            AvailableNetworkSession availableSession = null;  //  the session we´ll join

            availableSessions = NetworkSession.Find(NetworkSessionType.SystemLink,
                maximumLocalPlayers, null);

            //  Look for a session with availiable gamer slots
            foreach (AvailableNetworkSession curSession in availableSessions)
            {
                int TotalSessionSlots = curSession.OpenPublicGamerSlots + curSession.OpenPrivateGamerSlots;
                if (TotalSessionSlots > curSession.CurrentGamerCount)
                    availableSession = curSession;
            }

            // if a session was found, connect to it 
            if (availableSession != null)
            {
                message = "Found an avaliable session at host" + availableSession.HostGamertag;
                session = NetworkSession.Join(availableSession);
            }
            else
                message = "No sessions found!";

        }

        public void AsyncFindSession()
        {
            message = "Asynchronous search started!";
            if (AsyncSessionFind == null)
            {
                AsyncSessionFind = NetworkSession.BeginFind(NetworkSessionType.SystemLink,
                    maximumLocalPlayers, null,
                    new AsyncCallback(session_SessionFound), null);
            }
        }

        public void session_SessionFound(IAsyncResult result)
        {
            AvailableNetworkSessionCollection availableSessions;  //  all sessions found
            AvailableNetworkSession availableSession = null;     // the session we will join

            if (AsyncSessionFind.IsCompleted)
            {
                availableSessions = NetworkSession.EndFind(result);

                 //  Look for a session with availiable gamer slots
                foreach (AvailableNetworkSession curSession in availableSessions)
                {
                    int TotalSessionSlots = curSession.OpenPublicGamerSlots + 
                                            curSession.OpenPrivateGamerSlots;
                    if (TotalSessionSlots > curSession.CurrentGamerCount)
                        availableSession = curSession;
                }

                // if a session was found, connect to it 
                if (availableSession != null)
                {
                    message = "Found an avaliable session at host " + availableSession.HostGamertag;
                    session = NetworkSession.Join(availableSession);
                }
                else
                    message = "No sessions found!";

                //  Reset the session finding result
                AsyncSessionFind = null;
            }
        }



        #region session events
        void session_GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            if (e.Gamer.IsHost)
            {
                message = "The Host started the session!";
            }
            else
                message = "Gamer " + e.Gamer.Tag + " joined the session!";

            //  If the local player is the host, check if the game needs to start
            if (session.IsHost)
            {
                //  If two players joined, and are ready, we start the game
                if (session.IsEveryoneReady && Gamer.SignedInGamers.Count >= 1)
                    session.StartGame();
            }
        }

        void session_GamerLeft(object sender, GamerLeftEventArgs e)
        {
            message = "Gamer " + e.Gamer.Tag + " left the session!";
        }

        void session_GameStarted(object sender, GameStartedEventArgs e)
        {
            message = "Game Started";
         }

        void session_HostChanged(object sender, HostChangedEventArgs e)
        {
            message = "Host changed from " + e.OldHost.Tag + " to " + e.NewHost.Tag;
        }

         void session_SessionEnded(object sender, NetworkSessionEndedEventArgs e)
        {
             switch (e.EndReason)
             { 
                 case NetworkSessionEndReason.ClientSignedOut:
                     message = "This client player has signed out of session";
                     break;
                 case NetworkSessionEndReason.Disconnected:
                     message = "Network connectivity problems ended the session";
                     break;
                 case NetworkSessionEndReason.RemovedByHost:
                     message = "The host removed this client player from the session";
                     break;
                 case NetworkSessionEndReason.HostEndedSession:
                     message = "The host left the session, removing all active players";
                     break;
                 default:
                     message = "Session ended due to an unknown reason";
                     break;
             }
        }       

        void session_GameEnded(object sender, GameEndedEventArgs e)
        {
            message = "Game Ended";
        }

        #endregion
        
        //  Set all local players (one, in this case) to "ready" state
        public void SetPlayerReady ()
        {
            foreach (LocalNetworkGamer gamer in session.LocalGamers)
                gamer.IsReady = true;
            message = "Local players are ready to go!";
        }

        public void SendMessage(string key)
        {
            foreach (LocalNetworkGamer localPlayer in session.LocalGamers)
            {
                packetWriter.Write(key);
                localPlayer.SendData(packetWriter, SendDataOptions.None);
                message = "Sending message: " + key;
            }
        }

        public void ReceiveMessage()
        {
            NetworkGamer remotePlayer;  // The sender of the message
            
            foreach (LocalNetworkGamer localPlayer in session.LocalGamers)
            {
                //  While there is data available for us, keep reading
                while (localPlayer.IsDataAvailable)
                {
                    localPlayer.ReceiveData(packetReader, out remotePlayer);
                    //  Ignore input from local players
                    if (!remotePlayer.IsLocal)
                        message = "Received message: " + packetReader.ReadString();
                }
            }
        }

    }
}
