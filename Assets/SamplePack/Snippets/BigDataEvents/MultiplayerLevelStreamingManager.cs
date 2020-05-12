#if false
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdpKit;
using Newtonsoft.Json;

public class MultiplayerLevelStreamingManager : BoltGlobalEventListenerSingleton<MultiplayerLevelStreamingManager> 
{

	private Dictionary<ulong, byte[]> _workshopIDToLevelDataDictionary;
	private System.Action _downloadCompleteCallback;

	private bool _isSendingLevelToServer;
	private bool _isWaitingForServerToSendLevel;
	private ByteChunkifier _levelDataChunkifier;


	public override void Awake ()
	{
		base.Awake ();

		_workshopIDToLevelDataDictionary = new Dictionary<ulong, byte[]>();
	}
		
	public void SendLevelToServer(string levelJSON)
	{
		_isSendingLevelToServer = true;

		byte[] data = StringUtils.Zip(levelJSON);

		sendBytes (data, null);
	}

	private void sendBytes(byte[] data, BoltConnection connection)
	{
		int chunkSize = 600;
		ByteChunkifier chunkifier = new ByteChunkifier(data, chunkSize);

		SendLevelDataEvent sendEvent;
		byte[] sendBuffer;
		int index = 0;
		while (chunkifier.ReadChunk(out sendBuffer))
		{
			if (connection != null)
			{
				sendEvent = SendLevelDataEvent.Create (connection, Bolt.ReliabilityModes.ReliableOrdered);
			}
			else
			{
				sendEvent = SendLevelDataEvent.Create (Bolt.GlobalTargets.OnlyServer, Bolt.ReliabilityModes.ReliableOrdered);
			}
			sendEvent.ChunkSize = chunkSize;
			sendEvent.ChunkIndex = index;
			sendEvent.TotalSize = data.Length;
			sendEvent.Complete = !chunkifier.HasBytesLeft();
			sendEvent.BinaryData = new ByteArrayProtocolToken(sendBuffer);
			sendEvent.Send();

			Debug.Log("Sending Level Data: "+index+", "+((index+1)*chunkSize)+" / "+data.Length);

			index++;
		}
	}

	public bool IsSendingLevelToServer ()
	{
		return _isSendingLevelToServer;
	}

	public override void OnEvent (ServerReceivedLevelFromClient evnt)
	{
		_isSendingLevelToServer = false;
	}

	public override void OnEvent (SendLevelDataEvent sendEvent)
	{
		if (_levelDataChunkifier == null)
		{
			_levelDataChunkifier = new ByteChunkifier(sendEvent.TotalSize, sendEvent.ChunkSize);
		}

		ByteArrayProtocolToken token = sendEvent.BinaryData as ByteArrayProtocolToken;
		_levelDataChunkifier.WriteChunk(token.ByteArray, sendEvent.ChunkIndex);

		if (sendEvent.Complete)
		{
			byte[] data = _levelDataChunkifier.FullArray;

			onReceivedLevelData(data, sendEvent.RaisedBy);

			_levelDataChunkifier = null;
		}
	}

	private void onReceivedLevelData (byte[] data, BoltConnection connection)
	{
		string levelJSON = StringUtils.Unzip (data);
		LevelEditorLevelData levelData = JsonConvert.DeserializeObject<LevelEditorLevelData>(levelJSON, DataRepository.Instance.GetSettings());

		_workshopIDToLevelDataDictionary [levelData.WorkshopUploadID] = data;

		LevelDescription levelDescription = new LevelDescription();
		levelDescription.LevelID = levelData.WorkshopUploadID.ToString();
		levelDescription.LevelJSONPath = null;
		levelDescription.IsStreamedMultiplayerLevel = true;
		levelDescription.LevelTags = new List<LevelTags>();
		levelDescription.LevelEditorDifficultyIndex = 0;

		if (levelData.LevelType == LevelType.LastBotStanding)
		{
			LevelManager.Instance.AddLevelToBattleRoyaleLevels (levelDescription);
		}
		else
		{
			throw new System.NotImplementedException ("Only LastBotStanding levels can be streamed right now!");
		}

		if (_downloadCompleteCallback != null)
		{
			_downloadCompleteCallback ();
			_downloadCompleteCallback = null;
		}

		if (BoltNetwork.IsServer)
		{
			ServerReceivedLevelFromClient receivedEvent = ServerReceivedLevelFromClient.Create (connection, Bolt.ReliabilityModes.ReliableOrdered);
			receivedEvent.Send ();
		}
		else
		{
			_isWaitingForServerToSendLevel = false;
		}
	}

	public bool HasLevelData (ulong levelID)
	{
		return _workshopIDToLevelDataDictionary.ContainsKey (levelID);
	}

	public string GetLevelJSON (string levelID)
	{
		ulong ulongLevelID;
		if (ulong.TryParse (levelID, out ulongLevelID))
		{
			if (HasLevelData (ulongLevelID))
			{
				return StringUtils.Unzip(_workshopIDToLevelDataDictionary [ulongLevelID]);
			}
		}
		throw new UnityException ("MultiplayerLevelStreamingManager can't find level: "+levelID);
	}

	public void RequestDownloadFromServer (string levelID, System.Action clientDownloadCompleteCallback)
	{
		_isWaitingForServerToSendLevel = true;

		_downloadCompleteCallback = clientDownloadCompleteCallback;

		ClientRequestStreamingLevelDownload requestEvent = ClientRequestStreamingLevelDownload.Create (Bolt.GlobalTargets.OnlyServer, Bolt.ReliabilityModes.ReliableOrdered);
		requestEvent.LevelID = levelID;
		requestEvent.Send();
	}

	public bool IsWaitingForServerToSendLevel()
	{
		return _isWaitingForServerToSendLevel;
	}

	public override void OnEvent (ClientRequestStreamingLevelDownload levelRequestEvent)
	{
		if (!BoltNetwork.IsServer)
		{
			return;
		}

		ulong levelID;
		if (ulong.TryParse (levelRequestEvent.LevelID, out levelID))
		{
			if (HasLevelData (levelID))
			{
				byte[] levelData = _workshopIDToLevelDataDictionary [levelID];

				sendBytes (levelData, levelRequestEvent.RaisedBy);
			}
		}
	}
}
#endif