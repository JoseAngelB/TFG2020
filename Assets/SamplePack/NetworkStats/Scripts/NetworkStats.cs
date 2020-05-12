using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NetworkStats : Bolt.GlobalEventListener
{
    public Text myText;
    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(transform.root);
        myText = GetComponent<Text>();
    }

    public override void Connected(BoltConnection connection)
    {
      //  connection.EnableMetrics = true;
    }

    public override void SceneLoadLocalDone(string map)
    {
        if (BoltNetwork.IsClient)
        {
       //    BoltNetwork.server.EnableMetrics = true;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (BoltNetwork.IsRunning)
            if (BoltNetwork.IsServer || BoltNetwork.IsClient)
                if (BoltNetwork.ServerFrame % 100 == 0)
                {
                    myText.text = null;
                    foreach (BoltConnection c in BoltNetwork.Connections)
                    {
                        //name from user data
                        myText.text += "Connection ID: " + c.ConnectionId + ", Bits per second in: " + c.BitsPerSecondIn + ", Bits per second out: " + c.BitsPerSecondOut + ", Ping: " + c.PingNetwork * 1000 + "\n";
                        //ping, ping aliased, in out for events states commands, upload download

                        myText.text += "\n States Total IN: " + c.StatesStats.TotalIn;
                        myText.text += " States Total OUT: " + c.StatesStats.TotalOut;
                        myText.text += " States IN: " + c.StatesStats.In;
                        myText.text += " States OUT: " + c.StatesStats.Out;

                        myText.text += "\n Events Total IN: " + c.EventsStats.TotalIn;
                        myText.text += " Events Total OUT: " + c.EventsStats.TotalOut;
                        myText.text += " Events IN: " + c.EventsStats.In;
                        myText.text += " Events OUT: " + c.EventsStats.Out;

                        myText.text += "\n Commands Total IN: " + c.CommandsStats.TotalIn;
                        myText.text += " Commands Total OUT: " + c.CommandsStats.TotalOut;
                        myText.text += " Commands IN: " + c.CommandsStats.In;
                        myText.text += " Commands OUT: " + c.CommandsStats.Out;


                    }
                }
    }
}
