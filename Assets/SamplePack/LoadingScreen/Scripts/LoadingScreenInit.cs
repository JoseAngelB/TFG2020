using UnityEngine;
using System.Collections;
using System;
using UdpKit;
using UnityEngine.SceneManagement;


public class LoadingScreenInit : Bolt.GlobalEventListener
{
    public ImageController IC;

    enum State
    {
        SelectMode,
        SelectMap,
        EnterServerIp,
        StartServer,
        StartClient,
        Started,
    }

    State state;

    string map;
    string serverAddress = "127.0.0.1";
    ulong ID = 1234;

    int serverPort = 25000;


    private void FixedUpdate()
    {
        if (BoltNetwork.IsRunning)
            if (BoltNetwork.CurrentAsyncOperation != null)
            {
                Debug.Log(BoltNetwork.CurrentAsyncOperation.progress);
                if (BoltNetwork.CurrentAsyncOperation.progress >= 0.89f)
                {
                    IC.SceneLoadStart();
                }
            }
       
    }

    void Awake()
    {
        serverPort = BoltRuntimeSettings.instance.debugStartPort;

        // StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Level2");
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;


        while (!asyncOperation.isDone)
        {
            //Output the current progress
            //m_Text.text = "Loading progress: " + (asyncOperation.progress * 100) + "%";

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                //Change the Text to show the Scene is ready
                // m_Text.text = "Press the space bar to continue";
                //Wait to you press the space key to activate the Scene
                if (Input.GetKeyDown(KeyCode.Space))
                    //Activate the Scene
                    asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
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
            case State.EnterServerIp: State_EnterServerIp(); break;
            case State.StartClient: State_StartClient(); break;
            case State.StartServer: State_StartServer(); break;
        }

        GUILayout.EndArea();
    }

    private void State_EnterServerIp()
    {
        GUILayout.BeginHorizontal();

        GUILayout.Label("Server IP: ");
        serverAddress = GUILayout.TextField(serverAddress);


        ID = Convert.ToUInt64(GUILayout.TextField(ID.ToString()));
        if (GUILayout.Button("Connect"))
        {
            state = State.StartClient;
        }

        GUILayout.EndHorizontal();
    }


    void State_SelectMode()
    {
        if (ExpandButton("Server"))
        {
            state = State.SelectMap;
        }
        if (ExpandButton("Client"))
        {
            state = State.EnterServerIp;
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
        BoltLauncher.StartServer(new UdpEndPoint(UdpIPv4Address.Any, (ushort)serverPort));

        state = State.Started;
    }

    void State_StartClient()
    {
        BoltLauncher.StartClient(UdpEndPoint.Any);
        state = State.Started;
    }

    bool ExpandButton(string text)
    {
        return GUILayout.Button(text, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
    }
    public override void BoltStartDone()
    {
        if (BoltNetwork.IsClient)
        {

           // BoltNetwork.Connect(new UdpEndPoint(UdpIPv4Address.Parse(serverAddress), (ushort)serverPort));

        }
        else
        {
            BoltNetwork.LoadScene(map);

        }
    }

}