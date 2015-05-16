using AssemblyCSharp;
using NeonShooter.Players;
using System.Collections;
using UnityEngine;

using com.shephertz.app42.gaming.multiplayer.client;
//using com.shephertz.app42.gaming.multiplayer.client.events;
//using com.shephertz.app42.gaming.multiplayer.client.listener;
//using com.shephertz.app42.gaming.multiplayer.client.command;
//using com.shephertz.app42.gaming.multiplayer.client.message;
//using com.shephertz.app42.gaming.multiplayer.client.transformer;

public class appwarp : MonoBehaviour
{
    public float interval = 0.1f;
    public GameObject enemyPrefab;
    float timer = 0;


    //please update with values you get after signing up
    public static string apiKey = "a01d3f81260e3915d082aa969051eb562be504f9c5bed8b4870f9046da9d4625";
    public static string secretKey = "76facfb38f9dfda935055487c4a1c7396113ecb8ddcb46b2cb075ea78197c9a8";
    public static string roomid = "160047117";
    public static string username;
    Listener listen = new Listener();
    public static Vector3 newPos = new Vector3(0, 0, 0);
    void Start()
    {
        listen.appwarp = this.GetComponent<appwarp>();
        WarpClient.initialize(apiKey, secretKey);
        WarpClient.GetInstance().AddConnectionRequestListener(listen);
        WarpClient.GetInstance().AddChatRequestListener(listen);
        WarpClient.GetInstance().AddUpdateRequestListener(listen);
        WarpClient.GetInstance().AddLobbyRequestListener(listen);
        WarpClient.GetInstance().AddNotificationListener(listen);
        WarpClient.GetInstance().AddRoomRequestListener(listen);
        WarpClient.GetInstance().AddZoneRequestListener(listen);
        WarpClient.GetInstance().AddTurnBasedRoomRequestListener(listen);
        // join with a unique name (current time stamp)
        username = System.DateTime.UtcNow.Ticks.ToString();
        WarpClient.GetInstance().Connect(username);
        //For weapon tests
        addPlayer("TestPlayer");
    }


    public static ArrayList playerNames = new ArrayList();
    public static ArrayList enemies = new ArrayList();
    public void addPlayer(string playerName)
    {
        playerNames.Add(playerName);
        GameObject enemy = (GameObject)GameObject.Instantiate(enemyPrefab, new Vector3(0.65f, 0.98f, 1f), Quaternion.identity);
        enemy.GetComponent<EnemyPlayer>().NetworkName = playerName;
        enemies.Add(enemy);
        listen.Log("dodalem gracza; " + enemy.GetComponent<EnemyPlayer>().NetworkName);
    }

    public void movePlayer(float x, float y, float z, string playerName)
    {
        GameObject enemy = findEnemy(playerName);
        enemy.GetComponent<EnemyPlayer>().Position.Value = new Vector3(x, y, z);
    }

    private GameObject findEnemy(string enemyName)
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyPlayer>().NetworkName == enemyName)
                return enemy;
        }
        return null;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            string json = "{\"x\":\"" + transform.position.x + "\",\"y\":\"" + transform.position.y + "\",\"z\":\"" + transform.position.z + "\"}";

            listen.sendMsg(json);

            timer = interval;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        WarpClient.GetInstance().Update();
    }

    void OnGUI()
    {
        GUI.contentColor = Color.black;
        GUI.Label(new Rect(10, 10, 500, 200), listen.getDebug());
    }

    void OnApplicationQuit()
    {
        WarpClient.GetInstance().Disconnect();
    }

}
