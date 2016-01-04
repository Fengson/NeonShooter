using NeonShooter.Players;
using NeonShooter.Utils;
using System.Collections;
using System.Linq;
using UnityEngine;

using com.shephertz.app42.gaming.multiplayer.client;
using System.Collections.Generic;
using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using NeonShooter.AppWarp.Json;
using NeonShooter.AppWarp.States;
using NeonShooter.AppWarp.Events;
using System.Text;
using System.IO;
using NeonShooter.AppWarp.Serializing.Json;
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
        PlayerEvents playerEvents;
        PlayerStateJsonSerializer playerSerializer;

        public static Vector3 newPos = new Vector3(0, 0, 0);

        void Start()
        {
            listener.appwarp = GetComponent<appwarp>();
            InitializeWarpClient();

            username = System.DateTime.UtcNow.Ticks.ToString();
            warpClient.Connect(username);

            player = GetComponent<Player>();
            playerState = new PlayerState(player);
            playerEvents = new PlayerEvents(this, player);

            playerSerializer = new PlayerStateJsonSerializer(new JsonSerializationDict());
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

            enemy.Player = player;

            lock (playerState)
            {
                if (!playerState.IsNewcomer)
                {
                    //var ms = new MemoryStream();
                    //var bw = new BinaryWriter(ms);
                    //bw.Write((byte)0); // type of msg
                    lock (playerState)
                    {
                        var json = playerSerializer.SerializeAbsolute(playerState)
                            //playerState.AbsoluteJson
                            as JsonObject;
                        json.Append(new JsonPair("Type", "PlayerState")); // TODO: this leads to json with doubled Type : PlayerState. Is there any purpose?
                        SendPlayerState(json, playerName);

                        //bw.WriteAbsolute(playerState);
                        //playerState.ClearChanges();
                    }
                    //bw.Flush();
                    //ms.Position = 0;

                    //SendMessage(ms, playerName);
                }
            }
        }

        public void removePlayer(string playerName)
        {
            enemies[playerName].GetComponent<EnemyPlayer>().SetLeftGame();
            enemies.Remove(playerName);
        }

        void Update()
        {
            timer -= Time.deltaTime;
            if (listener.CanSendMessages && timer < 0)
            {
                //var ms = new MemoryStream();
                //var bw = new BinaryWriter(ms);
                //bw.Write((byte)0); // type of msg
                lock (playerState)
                {
                    var json = playerSerializer.SerializeRelative(playerState)
                        //playerState.RelativeJson
                        as JsonObject;
                    playerState.ClearChanges();
                    SendPlayerState(json);

                    //bw.WriteRelative(playerState);
                    //playerState.ClearChanges();
                }
                //bw.Flush();
                //ms.Position = 0;

                //SendMessage(ms);

                timer = interval;
            }            

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            WarpClient.GetInstance().Update();
        }

        #region Sending

        public void SendPlayerState(JsonObject json)
        {
            SendPlayerState(json, (string)null);
        }

        public void SendPlayerState(JsonObject json, EnemyPlayer receiver)
        {
            SendPlayerState(json, receiver == null ? null : receiver.NetworkName);
        }

        public void SendPlayerState(JsonObject json, string receiver)
        {
            SendMessage(json, "PlayerState", receiver);
        }

        public void SendPlayerEvent(JsonObject json)
        {
            SendPlayerEvent(json, (string)null);
        }

        public void SendPlayerEvent(JsonObject json, EnemyPlayer receiver)
        {
            SendPlayerEvent(json, receiver == null ? null : receiver.NetworkName);
        }

        public void SendPlayerEvent(JsonObject json, string receiver)
        {
            SendMessage(json, "PlayerEvent", receiver);
        }

        public void SendMessage(JsonObject json, string type, string receiver = null)
        {
            json.Append(new JsonPair("Type", type));

            string message = json.BuildString();
            listener.sendJsonMsg(message, receiver);
        }

        public void SendMessage(MemoryStream binary, string receiver = null)
        {
            listener.sendBinaryMsg(binary, receiver);
        }

        #endregion

        public void InterpretMessage(Listener.SingleMessage message)
        {
            //Debug.Log(message);

            switch (message.Type)
            {
                case Listener.MessageType.Json:
                    InterpretJsonMessage(message.Sender, message.Contents);
                    break;
                case Listener.MessageType.Binary:
                    InterpretBinaryMessage(message.Sender, message.Contents);
                    break;
            }
        }

        private void InterpretJsonMessage(string sender, string message)
        {
            var enemy = enemies[sender].GetComponent<EnemyPlayer>();

            var json = JSON.Parse(message);
            var type = json["Type"];

            if (type.Value == "PlayerState")
            {
                var enemyState = new PlayerState(json, enemy);
                enemyState.ApplyTo(enemy);
            }
            else if (type.Value == "PlayerEvent")
            {
                if (json.Childs.Count() != 2)
                    throw new System.FormatException("Invalid format of JSON object with Type : PlayerEvent; " +
                        "JSON object should have exactly two values (including Type).");
                foreach (var kv in json.AsObject.getDictionary())
                {
                    if (kv.Key == "Type") continue;
                    playerEvents[kv.Key].OnActionReceived(enemy, kv.Value);
                }
            }
        }

        private void InterpretBinaryMessage(string sender, string message)
        {
            //var enemy = enemies[sender].GetComponent<EnemyPlayer>();

            //byte[] bytes = System.Convert.FromBase64String(message);
            //Debug.Log(System.BitConverter.ToString(bytes).Replace('-', ' '));
            //var ms = new MemoryStream(bytes);
            //var br = new BinaryReader(ms);

            //int type = br.ReadByte();

            //// PlayerState
            //if (type == 0)
            //{
            //    var enemyState = new PlayerState(br, enemy);
            //    enemyState.ApplyTo(enemy);
            //}
            // NOT IMPLEMENTED
            //else if (type.Value == "PlayerEvent")
            //{
            //    if (json.Childs.Count() != 2)
            //        throw new System.FormatException("Invalid format of JSON object with Type : PlayerEvent; " +
            //            "JSON object should have exactly two values (including Type).");
            //    foreach (var kv in json.AsObject.getDictionary())
            //    {
            //        if (kv.Key == "Type") continue;
            //        playerEvents[kv.Key].OnActionReceived(enemy, kv.Value);
            //    }
            //}
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