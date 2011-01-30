using System;
using UnityEngine;

namespace Irrelevant.Assets.Scripts.AI
{
	public class DevGuy
	{
		public Vector2 Position;

		// AI data:
		public bool waitingOthers = false;
		public int lastVisitedBlockIndex = -1;
		public Track currentTrack;

		public int lastHousePointInd = -1;

		public IndieDevBehavior indieDevBehaviour;

		public void SetLastHouse(int houseTileInd)
		{
			lastHousePointInd = houseTileInd;
		}

		public void ResetAIData() // TODO:m call?
		{
			lastVisitedBlockIndex = -1;
			currentTrack = null;
		}

	}

}
