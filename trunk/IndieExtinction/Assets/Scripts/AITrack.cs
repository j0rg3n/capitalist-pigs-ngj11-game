using System;
using System.Collections.Generic;

namespace Irrelevant.Assets.Scripts.AI
{
	public  class Track
	{
		public Waver waver;

		public const int MAX_LENGTH = 14 + 1; // 1 for the starting tile

		public List<int> track = new List<int>(); // furthest blocks come last
		public int grade = 0;
		public bool makeHaste = false;

		public Track(Waver waver)
		{
			this.waver = waver;
		}

		public Track(int startBlock, Waver waver)
		{
			this.waver = waver;
			track.Add(startBlock);
		}

		public bool CanExtend()
		{
			return track.Count < MAX_LENGTH - UnityEngine.Random.Range(0, 3);
		}

		public Track Extend(int newInd)
		{
			Track extendedTrack = new Track(waver);
			extendedTrack.track.AddRange(track);
			extendedTrack.track.Add(newInd);
			extendedTrack.waver = waver;
			return extendedTrack;
		}

		public int PeekNext()
		{
			return track[0];
		}

		public int Peek(int i)
		{
			return track[i];
		}

		public int PopNext()
		{
			int ind = track[0];
			track.RemoveAt(0);
			return ind;
		}

		public bool HasBlock(int blockInd)
		{
			return track.Contains(blockInd);
		}

		public int GetLast()
		{
			return track[track.Count-1];
		}

		public int Length()
		{
			return track.Count;
		}

		string BlockToString(int block)
		{
			int x, y;
			waver.BlockIndexToCoords(block, out x, out y);
			return String.Format("{0}({1},{2})", block, x, y);
			//return String.Format("{0}", block);
		}

		public string ToSrting()
		{
			string res = String.Empty;
			switch (track.Count)
			{
				case 0:
					res = String.Format("Empty");
					break;
				case 1:
					res = String.Format("{0}", BlockToString(track[0]));
					break;
				case 2:
					res = String.Format("{0} {1}", BlockToString(track[0]), BlockToString(track[1]));
					break;
				case 3:
					res = String.Format("{0} {1} {2}", BlockToString(track[0]), BlockToString(track[1]), BlockToString(track[2]));
					break;
				case 4:
					res = String.Format("{0} {1} {2} {3}", BlockToString(track[0]), BlockToString(track[1]), BlockToString(track[2]), BlockToString(track[3]));
					break;
				case 5:
					res = String.Format("{0} {1} {2} {3} {4}", BlockToString(track[0]), BlockToString(track[1]), BlockToString(track[2]), BlockToString(track[3]), BlockToString(track[4]));
					break;
			}
			return res;
		}

	}
}
