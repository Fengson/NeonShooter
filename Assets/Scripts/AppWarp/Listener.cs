using UnityEngine;

using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using com.shephertz.app42.gaming.multiplayer.client.listener;
using com.shephertz.app42.gaming.multiplayer.client.command;
using com.shephertz.app42.gaming.multiplayer.client.message;
using com.shephertz.app42.gaming.multiplayer.client.transformer;

using System;
using System.Collections.Generic;
using System.Linq;
using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;

using NeonShooter.Utils;

namespace NeonShooter.AppWarp
{
	public class Listener : ConnectionRequestListener, LobbyRequestListener, ZoneRequestListener, RoomRequestListener, ChatRequestListener, UpdateRequestListener, NotifyListener, TurnBasedRoomListener
    {
        const int maxMessageLength = 1000;
        const string partWrapper1 = "{ Type : Parts, Id : ";
        const string partWrapper2 = ", Count : ";
        const string partWrapper3 = ", Index : ";
        const string partWrapper4 = ", Contents : \"";
        const string partWrapper5 = "\" }";
        const int maxCountLength = 10;
        const int maxIndexLength = 10;

		int state = 0;
		string debug = "";
		public appwarp appwarp;

        public bool CanSendMessages { get { return state == 1; } }

		public void Log(string msg)
		{
			debug = msg + "\n" + debug;
		}

		public string getDebug()
		{
			return debug;
		}

		public void onLog(String message){
			Log (message);
		}

		public bool sendMsg(string msg, string username = null)
        {
            Debug.Log(msg);

            if (!CanSendMessages) return false;

            Stack<string> parts = new Stack<string>();
            parts.Push(msg);
            if (msg.Length > maxMessageLength)
            {
                string partWrapper12 = partWrapper1 + DateTime.UtcNow.Ticks + partWrapper2;
                int wrapperLength = partWrapper12.Length + maxCountLength + partWrapper3.Length + maxIndexLength + partWrapper4.Length + partWrapper5.Length;
                int maxContentsLength = maxMessageLength - wrapperLength;

                int count = msg.Length / maxContentsLength + 1;
                string partWrapper123 = partWrapper12 + count + partWrapper3;
                
                for (int i = 0; i < count; i++)
                {
                    string partWrapper1234 = partWrapper123 + i + partWrapper4;

                    string whole = parts.Pop();
                    string part = whole.Substring(0, Math.Min(whole.Length, maxContentsLength));
                    string partWrapper12345 = partWrapper1234 + part + partWrapper5;
                    string rest = whole.Substring(part.Length);

                    parts.Push(partWrapper12345);
                    if (rest.Length > 0)
                        parts.Push(rest);
                }
            }

            foreach (var part in parts.Reverse())
            {
                if (username == null) WarpClient.GetInstance().SendChat(part);
                else WarpClient.GetInstance().sendPrivateChat(username, part);
            }
            return true;
		}

		//ConnectionRequestListener
		#region ConnectionRequestListener
		public void onConnectDone(ConnectEvent eventObj)
		{
			Log ("onConnectDone : " + eventObj.getResult());
			if(eventObj.getResult() == 0)
			{
				WarpClient.GetInstance().SubscribeRoom(appwarp.roomid);
			}
		}

		public void onInitUDPDone(byte res)
		{
		}
		
		public void onDisconnectDone(ConnectEvent eventObj)
		{
			Log("onDisconnectDone : " + eventObj.getResult());
		}
		#endregion
		
		//LobbyRequestListener
		#region LobbyRequestListener
		public void onJoinLobbyDone (LobbyEvent eventObj)
		{
			Log ("onJoinLobbyDone : " + eventObj.getResult());
			if(eventObj.getResult() == 0)
			{
				state = 1;
			}
		}
		
		public void onLeaveLobbyDone (LobbyEvent eventObj)
		{
			Log ("onLeaveLobbyDone : " + eventObj.getResult());
		}
		
		public void onSubscribeLobbyDone (LobbyEvent eventObj)
		{
			Log ("onSubscribeLobbyDone : " + eventObj.getResult());
			if(eventObj.getResult() == 0)
			{
				WarpClient.GetInstance().JoinLobby();
			}
		}
		
		public void onUnSubscribeLobbyDone (LobbyEvent eventObj)
		{
			Log ("onUnSubscribeLobbyDone : " + eventObj.getResult());
		}
		
		public void onGetLiveLobbyInfoDone (LiveRoomInfoEvent eventObj)
		{
			Log ("onGetLiveLobbyInfoDone : " + eventObj.getResult());
		}
		#endregion
		
		//ZoneRequestListener
		#region ZoneRequestListener
		public void onDeleteRoomDone (RoomEvent eventObj)
		{
			Log ("onDeleteRoomDone : " + eventObj.getResult());
		}
		
		public void onGetAllRoomsDone (AllRoomsEvent eventObj)
		{
			Log ("onGetAllRoomsDone : " + eventObj.getResult());
			for(int i=0; i< eventObj.getRoomIds().Length; ++i)
			{
				Log ("Room ID : " + eventObj.getRoomIds()[i]);
			}
		}
		
		public void onCreateRoomDone (RoomEvent eventObj)
		{
			Log ("onCreateRoomDone : " + eventObj.getResult());
		}
		
		public void onGetOnlineUsersDone (AllUsersEvent eventObj)
		{
			Log ("onGetOnlineUsersDone : " + eventObj.getResult());
		}
		
		public void onGetLiveUserInfoDone (LiveUserInfoEvent eventObj)
		{
			Log ("onGetLiveUserInfoDone : " + eventObj.getResult());
		}
		
		public void onSetCustomUserDataDone (LiveUserInfoEvent eventObj)
		{
			Log ("onSetCustomUserDataDone : " + eventObj.getResult());
		}
		
        public void onGetMatchedRoomsDone(MatchedRoomsEvent eventObj)
		{
			if (eventObj.getResult() == WarpResponseResultCode.SUCCESS)
            {
                Log ("GetMatchedRooms event received with success status");
                foreach (var roomData in eventObj.getRoomsData())
                {
                    Log("Room ID:" + roomData.getId());
                }
            }
		}		
		#endregion

		//RoomRequestListener
		#region RoomRequestListener
		public void onSubscribeRoomDone (RoomEvent eventObj)
		{
			if(eventObj.getResult() == 0)
			{
				/*string json = "{\"start\":\""+id+"\"}";
				WarpClient.GetInstance().SendChat(json);
				state = 1;*/
				WarpClient.GetInstance().JoinRoom(appwarp.roomid);
			}
			
			Log ("onSubscribeRoomDone : " + eventObj.getResult());
		}
		
		public void onUnSubscribeRoomDone (RoomEvent eventObj)
		{
			Log ("onUnSubscribeRoomDone : " + eventObj.getResult());
		}
		
		public void onJoinRoomDone (RoomEvent eventObj)
		{
			if(eventObj.getResult() == 0)
			{
				state = 1;
			}
			Log ("onJoinRoomDone : " + eventObj.getResult());
			
		}
		
		public void onLockPropertiesDone(byte result)
		{
			Log ("onLockPropertiesDone : " + result);
		}
		
		public void onUnlockPropertiesDone(byte result)
		{
			Log ("onUnlockPropertiesDone : " + result);
		}
		
		public void onLeaveRoomDone (RoomEvent eventObj)
		{
			Log ("onLeaveRoomDone : " + eventObj.getResult());
		}
		
		public void onGetLiveRoomInfoDone (LiveRoomInfoEvent eventObj)
		{
			Log ("onGetLiveRoomInfoDone : " + eventObj.getResult());
		}
		
		public void onSetCustomRoomDataDone (LiveRoomInfoEvent eventObj)
		{
			Log ("onSetCustomRoomDataDone : " + eventObj.getResult());
		}
		
		public void onUpdatePropertyDone(LiveRoomInfoEvent eventObj)
        {
            if (WarpResponseResultCode.SUCCESS == eventObj.getResult())
            {
                Log ("UpdateProperty event received with success status");
            }
            else
            {
                Log ("Update Propert event received with fail status. Status is :" + eventObj.getResult().ToString());
            }
        }
		#endregion
		
		//ChatRequestListener
		#region ChatRequestListener
		public void onSendChatDone (byte result)
		{
			//Log ("onSendChatDone result : " + result);
			
		}
		
		public void onSendPrivateChatDone(byte result)
		{
			Log ("onSendPrivateChatDone : " + result);
		}
		#endregion
		
		//UpdateRequestListener
		#region UpdateRequestListener
		public void onSendUpdateDone (byte result)
		{
		}
		public void onSendPrivateUpdateDone (byte result)
		{
			Log ("onSendPrivateUpdateDone : " + result);
		}
		#endregion

		//NotifyListener
		#region NotifyListener
		public void onRoomCreated (RoomData eventObj)
		{
			Log ("onRoomCreated");
		}
		public void onPrivateUpdateReceived (string sender, byte[] update, bool fromUdp)
		{
			Log ("onPrivateUpdate");
		}
		public void onRoomDestroyed (RoomData eventObj)
		{
			Log ("onRoomDestroyed");
		}
		
		public void onUserLeftRoom (RoomData eventObj, string username)
		{
			Log ("onUserLeftRoom : " + username);
		}
		
		public void onUserJoinedRoom (RoomData eventObj, string username)
		{
			Log ("onUserJoinedRoom : " + username);
			Log ("Player joined: " + eventObj.getName() + " " + username);

			if(username != appwarp.username)
				appwarp.addPlayer (username);
		}
		
		public void onUserLeftLobby (LobbyData eventObj, string username)
		{
			Log ("onUserLeftLobby : " + username);
		}
		
		public void onUserJoinedLobby (LobbyData eventObj, string username)
		{
			Log ("onUserJoinedLobby : " + username);
		}
		
		public void onUserChangeRoomProperty(RoomData roomData, string sender, Dictionary<string, object> properties, Dictionary<string, string> lockedPropertiesTable)
		{
			Log ("onUserChangeRoomProperty : " + sender);
		}
			
		public void onPrivateChatReceived(string sender, string message)
		{
            Log("onPrivateChatReceived : " + sender);

            receiveMessage(sender, message);
		}
		
		public void onMoveCompleted(MoveEvent move)
		{
			Log ("onMoveCompleted by : " + move.getSender());
		}
		
		public void onChatReceived (ChatEvent eventObj)
		{
            receiveMessage(eventObj.getSender(), eventObj.getMessage());
		}

        private abstract class GeneralMessage
        {
            public string Sender { get; set; }

            public abstract SingleMessage CompleteMessage { get; }
        }

        private class SingleMessage : GeneralMessage
        {
            public string Contents { get; set; }
            public override SingleMessage CompleteMessage { get { return this; } }
        }

        private class MessageSequence : GeneralMessage
        {
            public string Id { get; set; }
            public int Count { get; set; }
            public int Received { get; set; }
            public string[] Messages { get; set; }

            public override SingleMessage CompleteMessage
            {
                get 
                {
                    if (Received < Count) return null;
                    return new SingleMessage
                    {
                        Sender = Sender,
                        Contents = String.Concat(Messages)
                    };
                }
            }
        }

        Queue<GeneralMessage> messageQueue = new Queue<GeneralMessage>();
        Dictionary<string, Dictionary<string, MessageSequence>> messageSequences = new Dictionary<string, Dictionary<string, MessageSequence>>();

        private void receiveMessage(string sender, string message)
        {
            if (sender == appwarp.username) return;

            EnqueueMessage(sender, message);

            while (true)
            {
                var singleMessage = TryDequeueMessage();
                if (singleMessage == null) break;

                ProcessMessage(singleMessage.Sender, singleMessage.Contents);
            }
        }

        private void EnqueueMessage(string sender, string message)
        {
            var json = JSON.Parse(message);
            var type = json["Type"];
            if (type.Value == "Parts")
            {
                string id = json["Id"].Value;
                int count = json["Count"].AsInt;
                int index = json["Index"].AsInt;
                string contents = json["Contents"];

                bool isNew = false;

                var innerDict = messageSequences.TryGet(sender);
                if (innerDict == null)
                {
                    innerDict = new Dictionary<string, MessageSequence>();
                    messageSequences[sender] = innerDict;
                    isNew = true;
                }
                
                MessageSequence seq = null;
                if (!isNew) seq = innerDict.TryGet(id);
                if (seq == null)
                {
                    seq = new MessageSequence
                    {
                        Sender = sender,
                        Id = id,
                        Count = count,
                        Received = 0,
                        Messages = new string[count]
                    };
                    innerDict[id] = seq;
                    messageQueue.Enqueue(seq);
                }

                seq.Messages[index] = contents;
                seq.Received++;
            }
            else
            {
                messageQueue.Enqueue(new SingleMessage
                {
                    Sender = sender,
                    Contents = message
                });
            }
        }

        private SingleMessage TryDequeueMessage()
        {
            if (messageQueue.Count == 0) return null;
            SingleMessage message = messageQueue.Peek().CompleteMessage;
            if (message == null) return null;
            messageQueue.Dequeue();
            return message;
        }

        private void ProcessMessage(string sender, string message)
        {
            if (!appwarp.playerNames.Contains(sender))
            {
                appwarp.addPlayer(sender);
            }

            appwarp.InterpretMessage(message, sender);
        }
		
		public void onUpdatePeersReceived (UpdateEvent eventObj)
		{
			Log ("onUpdatePeersReceived");
		}
		
		public void onUserChangeRoomProperty(RoomData roomData, string sender, Dictionary<String, System.Object> properties)
        {
            Log("Notification for User Changed Room Propert received");
            Log(roomData.getId());
            Log(sender);
            foreach (KeyValuePair<String, System.Object> entry in properties)
            {
                Log("KEY:" + entry.Key);
                Log("VALUE:" + entry.Value.ToString());
            }
        }

		
		public void onUserPaused(String locid, Boolean isLobby, String username)
		{
			Log("onUserPaused");
		}
		
		public void onUserResumed(String locid, Boolean isLobby, String username)
		{
			Log("onUserResumed");
		}
		
		public void onGameStarted(string sender, string roomId, string nextTurn)
		{
			Log("onGameStarted");
		}
		
		public void onGameStopped(string sender, string roomId)
		{
			Log("onGameStopped");
		}

		public void onNextTurnRequest (string lastTurn)
		{
			Log("onNextTurnRequest");
		}
		#endregion

		//TurnBasedRoomListener
		#region TurnBasedRoomListener
		public void onSendMoveDone(byte result)
		{
		}
		
		public void onStartGameDone(byte result)
		{
		}
		
		public void onStopGameDone(byte result)
		{
		}
		
		public void onSetNextTurnDone(byte result)
		{
		}
		
		public void onGetMoveHistoryDone(byte result, MoveEvent[] moves)
		{
		}
		#endregion
	}
}

