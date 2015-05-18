using NeonShooter.Players;
using System.Collections;
using UnityEngine;

using com.shephertz.app42.gaming.multiplayer.client;
using System.Collections.Generic;
using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.AppWarp.States;
using NeonShooter.Utils;
//using com.shephertz.app42.gaming.multiplayer.client.events;
//using com.shephertz.app42.gaming.multiplayer.client.listener;
//using com.shephertz.app42.gaming.multiplayer.client.command;
//using com.shephertz.app42.gaming.multiplayer.client.message;
//using com.shephertz.app42.gaming.multiplayer.client.transformer;

namespace NeonShooter.AppWarp
{
    public class appwarp : MonoBehaviour
    {
        float timer = 0;
        WarpClient warpClient;

        public float interval = 0.1f;
        public GameObject enemyPrefab;

        //please update with values you get after signing up
        public static string apiKey = "a01d3f81260e3915d082aa969051eb562be504f9c5bed8b4870f9046da9d4625";
        public static string secretKey = "76facfb38f9dfda935055487c4a1c7396113ecb8ddcb46b2cb075ea78197c9a8";
        public static string roomid = "160047117";

        public static string username;

        Listener listener = new Listener();

        Player player;
        PlayerState playerState;

        public static Vector3 newPos = new Vector3(0, 0, 0);
        void Start()
        {
            listener.appwarp = GetComponent<appwarp>();
            InitializeWarpClient();

            username = System.DateTime.UtcNow.Ticks.ToString();
            warpClient.Connect(username);

            player = GetComponent<Player>();
            playerState = new PlayerState(player);

            //For weapon tests
            //addPlayer("TestPlayer");
        }

        public static ArrayList playerNames = new ArrayList();
        public static Dictionary<string, GameObject> enemies = new Dictionary<string, GameObject>();

        public void addPlayer(string playerName)
        {
            playerNames.Add(playerName);

            var enemyObject = (GameObject)Object.Instantiate(enemyPrefab, new Vector3(0.65f, 0.98f, 1), Quaternion.identity);
            var enemy = enemyObject.GetComponent<EnemyPlayer>();

            enemy.NetworkName = playerName;
            enemies[enemy.NetworkName] = enemyObject;
            
            //var json = playerState.AbsoluteJson as JsonObject;
            //json.Append(new JsonPair("Type", "PlayerState"));
            //listener.sendPrivateMsg(playerName, json.ToString());
        }

        public void removePlayer(string playerName)
        {
            // TODO: ...
        }

        void Update()
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                var json = playerState.RelativeJson as JsonObject;
                json.Append(new JsonPair("Type", "PlayerState"));
                playerState.ClearChanges();
                listener.sendMsg(json.ToString());

                Debug.Log(json.ToString());

                timer = interval;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            WarpClient.GetInstance().Update();
        }

        public void InterpretMessage(string message, string sender)
        {
            var enemy = enemies[sender].GetComponent<EnemyPlayer>();

            //Debug.Log(message);

            var json = JSON.Parse(message);
            var type = json["Type"];

            if (type.Value == "PlayerState")
            {
                var enemyState = PlayerState.FromJSONNode(json, enemy);
                enemyState.ApplyTo(enemy);
            }
        }

        void OnGUI()
        {
            GUI.contentColor = Color.black;
            GUI.Label(new Rect(10, 10, 500, 200), listener.getDebug());
        }

        void OnApplicationQuit()
        {
            WarpClient.GetInstance().Disconnect();
        }

        void InitializeWarpClient()
        {
            WarpClient.initialize(apiKey, secretKey);
            warpClient = WarpClient.GetInstance();
            warpClient.AddConnectionRequestListener(listener);
            warpClient.AddChatRequestListener(listener);
            warpClient.AddUpdateRequestListener(listener);
            warpClient.AddLobbyRequestListener(listener);
            warpClient.AddNotificationListener(listener);
            warpClient.AddRoomRequestListener(listener);
            warpClient.AddZoneRequestListener(listener);
            warpClient.AddTurnBasedRoomRequestListener(listener);
        }
    }

}