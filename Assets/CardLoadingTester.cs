using System;
using KompasCore.GameCore;
using KompasServer.Cards;
using KompasServer.GameCore;
using Newtonsoft.Json;
using UnityEngine;

public class CardLoadingTester : MonoBehaviour
{
	public ServerCardRepository cardRepository;
	public ServerPlayer dummyPlayer;

	void Start()
	{
		int i = 0;
		foreach (var c in CardRepository.CardNames)
		{
			try
			{
				cardRepository.InstantiateServerNonAvatar(c, dummyPlayer, i++);
			}
			catch(JsonSerializationException e)
			{
				Debug.LogError($"Loading {c}: {e}");
			}
			catch(NullReferenceException nre)
			{
				Debug.Log($"Null ref {nre}");
			}
			catch(ArgumentNullException ane)
			{
				Debug.Log($"Null ref {ane}");
			}
			
		}
	}
}
