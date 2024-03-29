﻿using System;
using UnityEngine;

namespace Irrelevant.Assets.Scripts.AI
{
	public class DevGuy
	{
		public Vector2 Position;

		// AI data:
		public int lastVisitedBlockIndex = -1;
		public Track currentTrack;
		public bool waiting = false;
		public float waited = 0f;
		public float dontWait = 0;
		public bool forcedNoWait = false;
		public const float MAX_WAIT = 6f;
		public const float DONT_WAIT = 2f;

		public int lastHousePointInd = -1;

		public IndieDevBehavior indieDevBehaviour;

		public void SetLastHouse(int houseTileInd)
		{
			lastHousePointInd = houseTileInd;
		}

	}

}
