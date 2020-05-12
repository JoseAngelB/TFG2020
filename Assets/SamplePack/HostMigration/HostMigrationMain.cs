using UnityEngine;
using System.Collections;
using System;
using UdpKit;
using UnityEngine.SceneManagement;
using Bolt;
using System.Linq;
using System.Collections.Generic;
using udpkit.platform.photon;

public class PlayerData
{
    public string myName;
    public Vector3 myPosition;

}

public class HostMigrationMain : Bolt.GlobalEventListener
{
    bool searching;

    string id;

    public List<PlayerData> PDS = new List<PlayerData>();

    public List<HostMigrationPlayer> TPCS = new List<HostMigrationPlayer>();

    string myName = "name";


    bool firstInit;
    public GameObject Cube;
    public BoltConnection backup;

    bool backupSelf;


    public class RoomProtocolToken : Bolt.IProtocolToken
    {
        public String ArbitraryData;

        public void Read(UdpPacket packet)
        {
            ArbitraryData = packet.ReadString();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteString(ArbitraryData);
        }
    }

    enum State
    {
        SelectMode,
        SelectMap,
        SelectRoom,
        StartServer,
        StartClient,
        Started,
    }

    State state;

    string map;
    string serverAddress = "127.0.0.1";

    int serverPort = 25000;



    void Awake()
    {
        //BoltNetwork.globalObject.GetComponent<BoltPoll>().AllowImmediateShutdown = false;

        
        DontDestroyOnLoad(gameObject);
        serverPort = BoltRuntimeSettings.instance.debugStartPort;
    }

    void OnGUI()
    {
        Rect tex = new Rect(10, 10, 140, 75);
        Rect area = new Rect(10, 90, Screen.width - 20, Screen.height - 100);

        GUI.Box(tex, Resources.Load("BoltLogo") as Texture2D);
        GUILayout.BeginArea(area);

        switch (state)
        {
            case State.SelectMode: State_SelectMode(); break;
            case State.SelectMap: State_SelectMap(); break;
            case State.SelectRoom: State_SelectRoom(); break;
            case State.StartClient: State_StartClient(); break;
            case State.StartServer: State_StartServer(); break;
        }

        GUILayout.EndArea();
    }

    void State_SelectRoom()
    {
        try
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Looking for rooms:");
            myName = GUILayout.TextField(myName);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();

            foreach (var session in BoltNetwork.SessionList)
            {
                var photonSession = session.Value as PhotonSession;

                if (photonSession.Source == UdpSessionSource.Photon)
                {
                    var matchName = photonSession.HostName;
                    var label = string.Format("Join: {0} | {1}/{2}", matchName, photonSession.ConnectionsCurrent, photonSession.ConnectionsMax);

                    if (ExpandButton(label))
                    {
                        RoomProtocolToken token = new RoomProtocolToken();
                        token.ArbitraryData = myName;

                        BoltNetwork.Connect(photonSession, token);
                        state = State.Started;
                    }
                }
            }

            GUILayout.EndVertical();
        }
        catch
        {
            Debug.Log("ee");
        }
    }


    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<RoomProtocolToken>();
    }

    /*
    private void State_EnterServerIp()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("Server IP: ");
        serverAddress = GUILayout.TextField(serverAddress);


        GUILayout.Label("Name: ");
        myName = GUILayout.TextField(myName);

        if (GUILayout.Button("Connect"))
        {
            state = State.StartClient;
        }

        GUILayout.EndHorizontal();
    }
    */

    void State_SelectMode()
    {
        if (ExpandButton("Server"))
        {
            state = State.SelectMap;
        }
        if (ExpandButton("Client"))
        {
            state = State.StartClient;
        }
    }

    void State_SelectMap()
    {
        foreach (string value in BoltScenes.AllScenes)
        {
            if (SceneManager.GetActiveScene().name != value)
            {
                if (ExpandButton(value))
                {
                    map = value;
                    state = State.StartServer;
                }
            }
        }
    }

    void State_StartServer()
    {
        id = Guid.NewGuid().ToString();
        BoltLauncher.StartServer(new UdpEndPoint(UdpIPv4Address.Any, (ushort)serverPort));
        state = State.Started;
    }

    void State_StartClient()
    {
        BoltLauncher.StartClient(UdpEndPoint.Any);
        state = State.SelectRoom;
    }

    bool ExpandButton(string text)
    {
        return GUILayout.Button(text, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
    }
    public override void BoltStartDone()
    {
        BoltNetwork.AddGlobalEventListener(this);

        if (BoltNetwork.IsServer)
        {

            // var matchName = string.Format("{0} - {1}", id, map);
            Debug.Log(id);
            BoltNetwork.SetServerInfo(id, null);
            BoltNetwork.LoadScene("HostMigLevel");

        }
        else
        {
            searching = true;
        }

    }

    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        if (BoltNetwork.IsServer)
        {

            RoomProtocolToken token = (RoomProtocolToken)connection.ConnectToken;
            bool found = false;

            foreach (BoltEntity BE in BoltNetwork.Entities)
            {
                if (BE.GetComponent<HostMigrationPlayer>().state.name == token.ArbitraryData)
                {
                    BE.AssignControl(connection);
                    found = true;
                    break;
                }
            }

            if (found == false)
            {
                BoltEntity BE = BoltNetwork.Instantiate(BoltPrefabs.HostMigrationPlayer);
                TPCS.Add(BE.GetComponent<HostMigrationPlayer>());
                BE.GetComponent<HostMigrationPlayer>().state.name = token.ArbitraryData;
                BE.AssignControl(connection);
            }


        }
    }

    public override void SceneLoadLocalDone(string map)
    {
        if (firstInit == false)
        {
            firstInit = true;
            GameObject.Instantiate(Cube);

        }

        if (BoltNetwork.IsServer)
        {
            if (backupSelf == false)
            {

                BoltEntity BE = BoltNetwork.Instantiate(BoltPrefabs.HostMigrationPlayer);
                TPCS.Add(BE.GetComponent<HostMigrationPlayer>());
                BE.GetComponent<HostMigrationPlayer>().state.name = "host";
                BE.TakeControl();
            }
            else
            {
                foreach (PlayerData PD in PDS)
                {
                    BoltEntity BE = BoltNetwork.Instantiate(BoltPrefabs.HostMigrationPlayer, PD.myPosition, Quaternion.identity);
                    BE.GetComponent<HostMigrationPlayer>().state.name = PD.myName;

                    if (PD.myName == myName)
                        BE.TakeControl();


                }

            }
        }
    }


    public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
    {
        RoomProtocolToken RPT = new RoomProtocolToken();
        RPT.ArbitraryData = id.ToString();

        BoltNetwork.Accept(endpoint, RPT);
    }



    public override void Connected(BoltConnection connection)
    {
        if (BoltNetwork.IsServer)
        {



            if (backup == null)
            {
                backup = connection;

                var evnt = BackupChosen.Create(connection);
                evnt.Send();
            }
            else
            {
                if (backup.DisconnectReason != UdpConnectionDisconnectReason.Unknown)
                {
                    backup = connection;

                    var evnt = BackupChosen.Create(connection);
                    evnt.Send();

                }
            }




        }
        else
        {
            RoomProtocolToken RPT = (RoomProtocolToken)connection.AcceptToken;
            id = RPT.ArbitraryData;
        }
    }
    

    public override void Disconnected(BoltConnection connection)
    {

        Debug.Log("Disconnected");
        if (BoltNetwork.IsServer)
        {

            if (backup == connection)
            {
                foreach (BoltConnection BC in BoltNetwork.Connections)
                {
                    if (BC.DisconnectReason == UdpConnectionDisconnectReason.Unknown)
                    {
                        backup = BC;

                        var evnt = BackupChosen.Create(backup);
                        evnt.Send();
                        break;
                    }
                }
            }

            // backup = BoltNetwork.clients.First();

        }
        else
        {
            if (backupSelf)
                foreach (BoltEntity BE in BoltNetwork.Entities)
                {
                    PlayerData PD = new PlayerData();
                    PD.myName = BE.GetComponent<HostMigrationPlayer>().state.name;
                    PD.myPosition = BE.transform.position;
                    PDS.Add(PD);

                    //PD.Add(new PlayerData)

                }


        }

        //BoltNetwork.connections.GetEnumerator().Current.fir
        //  if(backup == connection)
        //     backup = null;

        //state = State.SelectMode;
    }

    public override void BoltShutdownBegin(AddCallback registerDoneCallback)
    {
        Debug.Log("BoltShutdownBegin");

        registerDoneCallback(test0);
    }

    void test0()
    {
        if (backupSelf)
        {           
            BoltLauncher.StartServer((ushort)serverPort);
        }
        else
            BoltLauncher.StartClient();

    }

    public override void OnEvent(BackupChosen evnt)
    {
        backupSelf = true;
        Debug.Log("w");
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        if (searching == true)
            foreach (var session in BoltNetwork.SessionList)
            {
                var photonSession = session.Value as PhotonSession;

                if (photonSession.Source == UdpSessionSource.Photon)
                {
                    var matchName = photonSession.HostName;
                    //var label = string.Format("Join: {0} | {1}/{2}", matchName, photonSession.ConnectionsCurrent, photonSession.ConnectionsMax);

                    if (matchName == id)
                    {
                        searching = false;

                        RoomProtocolToken token = new RoomProtocolToken();
                        token.ArbitraryData = myName;

                        BoltNetwork.Connect(photonSession, token);
                        state = State.Started;

                        break;
                    }
                }
            }
    }

}
