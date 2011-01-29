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
		public const int NoTracksDelay = 60; // frames
		public int noTracksDuring = 0;

		public int lastHousePointInd = -1;
		public bool updateHousePointInd = false;
		public bool hasHousePoint = false;
		public Vector2 lastHouseCoords;

		public void SetLastHouse(Vector2 coords) // TODO:m call
		{
			lastHousePointInd = -1;
			updateHousePointInd = true;
			hasHousePoint = true;
			lastHouseCoords = coords;
		}

		public void ResetLastHouse() // TODO:m call
		{
			lastHousePointInd = -1;
			updateHousePointInd = false;
			hasHousePoint = false;
		}


		public void ResetAIData() // TODO:m call
		{
			lastVisitedBlockIndex = -1;
			currentTrack = null;
			noTracksDuring = 0;
		}

	}

}
