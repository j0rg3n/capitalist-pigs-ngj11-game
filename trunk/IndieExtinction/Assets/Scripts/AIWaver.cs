using UnityEngine;
using System.Collections.Generic;

namespace Irrelevant.Assets.Scripts.AI
{
	public class Waver
	{
		public GlobalDevAIBehaviour AI;


		public const int STAY_AWAY_FROM_SP_DIST_DEFENSIVE = 3;
		public static int CompareTracks(Track a, Track b)
		{
			return b.grade - a.grade;
		}

		System.Comparison<Track> trackComparison = new System.Comparison<Track>(CompareTracks);

		List<Track> tracks = new List<Track>();
		List<Track> completeTracks = new List<Track>();


		public int GetBlockDistance(int indA, int indB)
		{
			int iA, jA;
			BlockIndexToCoords(indA, out iA, out jA);

			int iB, jB;
			BlockIndexToCoords(indB, out iB, out jB);

			// horizontal dist
			int distHLeft = iB - iA;
			if (distHLeft < 0)
				distHLeft = -distHLeft; //distHLeft += MapWidth;

			int distHRight = iA - iB;
			if (distHRight < 0)
				distHRight = -distHRight; //distHRight += MapWidth;

			int distH = System.Math.Min(distHLeft, distHRight);

			// vertical dist
			int distVUp = jB - jA;
			if (distVUp < 0)
				distVUp = -distVUp; // distVUp += MapHeight;

			int distVDown = jA - jB;
			if (distVDown < 0)
				distVDown = -distVDown; // distVDown += MapHeight;

			int distV = System.Math.Min(distVUp, distVDown);

			// dist:
			//System.Diagnostics.Debug.Assert((distV + distH) <= (MapSize / 4));
			return distV + distH;

		}

		public int BlockCoordsToIndex(int i, int j)
		{
			return AI.MapWidth * j + i;
		}

		public void BlockIndexToCoords(int ind, out int i, out int j)
		{
			i = ind % AI.MapWidth;
			j = ind / AI.MapWidth;
		}

		public Vector2 BlockCoordsToPosition(int i, int j)
		{
			return new Vector2(((float)i + 0.5f) / (float)AI.MapWidth, 	((float)j + 0.5f) / (float)AI.MapHeight);
		}

		public Vector2 BlockIndexToPosition(int ind)
		{
			int i, j;
			BlockIndexToCoords(ind, out i, out j);
			return BlockCoordsToPosition(i, j);
		}

		public int PositionToBlockIndex(Vector2 pos)
		{
			//int i = (int)(pos.x / AI.BlockSize);
			//int j = (int)(pos.y / AI.BlockSize);
			int i = (int)(pos.x * AI.MapWidth);
			int j = (int)(pos.y * AI.MapHeight);
			i = Mathf.Clamp(i, 0, AI.MapWidth-1);
			j = Mathf.Clamp(j, 0, AI.MapHeight-1);
			//UnityEngine.Debug.Log(string.Format("coords {0} {1}", i,j));
			System.Diagnostics.Debug.Assert(0 <= i && i < AI.MapWidth);
			System.Diagnostics.Debug.Assert(0 <= j && j < AI.MapHeight);
			return BlockCoordsToIndex(i, j);
		}

		public bool PassableBlock(int[,] map, int blockInd)
		{
			int i, j;
			BlockIndexToCoords(blockInd, out i, out j);
			//Debug.Log(string.Format("block {0} i {1} j {2} map {3}", blockInd, i, j, map[i, j]));
			return map[i, j] > 0;
		}

		int FindNeightbourBlocks(int block, ref int[] neighbours)
		{
			int cnt = 0;
			//Debug.Log(string.Format("block {0}", block));
			// up
			neighbours[cnt] = block - AI.MapWidth;
			//Debug.Log(string.Format("n {0}", neighbours[cnt]));
			if (neighbours[cnt] >= 0)
			{
				cnt++;
			}
			//Debug.Log(string.Format("cnt {0}", cnt));

			// down
			neighbours[cnt] = block + AI.MapWidth;
			//Debug.Log(string.Format("n {0}", neighbours[cnt]));
			if (neighbours[cnt] < AI.MapSize)
			{
				cnt++;
			}
			//Debug.Log(string.Format("cnt {0}", cnt));

			// left
			neighbours[cnt] = block - 1;
			//Debug.Log(string.Format("n {0}", neighbours[cnt]));
			if (neighbours[cnt] >= 0 && (block) % AI.MapWidth != 0)
			{
				cnt++;
			}
			//Debug.Log(string.Format("cnt {0}", cnt));

			// right
			neighbours[cnt] = block + 1;
			//Debug.Log(string.Format("n {0}", neighbours[cnt]));
			if (neighbours[cnt] < AI.MapSize && (neighbours[cnt]) % AI.MapWidth != 0)  // block is in right screen border
			{
				cnt++;
			}
			//Debug.Log(string.Format("cnt {0}", cnt));
			return cnt;
		}


		// returns true if atleas one track exists
		public bool StartWave(int start, int[,] map) // 0 - not passable, 1 - rotating, 2 - empty (passable))
		{
			//Debug.Log(string.Format("start {0}", start));
			tracks.Clear();
			completeTracks.Clear();

			Track stratTrack = new Track(start, this);
			tracks.Add(stratTrack);

			int[] neighbours = new int[4] { 0, 0, 0, 0 };

			while (tracks.Count > 0 && tracks[0].CanExtend())
			{
				Track nextTrack = tracks[0];
				tracks.RemoveAt(0);
				int endBlock = nextTrack.GetLast();
				int neighbourCnt = FindNeightbourBlocks(endBlock, ref neighbours);
				bool foundNeighbour = false;
				for (int i = 0; i < neighbourCnt; ++i)
				{
					System.Diagnostics.Debug.Assert(neighbours[i] >= 0 && neighbours[i] < AI.MapSize);
					bool passable = PassableBlock(map, neighbours[i]);
					bool notIn = !nextTrack.HasBlock(neighbours[i]);
					//UnityEngine.Debug.Log(string.Format("n {0} {1} {2} {3}", i, neighbours[i], passable, notIn));
					if (passable && notIn)
					{
						foundNeighbour = true;
						tracks.Add(nextTrack.Extend(neighbours[i]));
					}
				}
				if (!foundNeighbour && nextTrack.Length() > 1)
				{
					completeTracks.Add(nextTrack);
				}
			}

			completeTracks.AddRange(tracks);

			// pop away the start block
			foreach (Track track in completeTracks)
			{
				track.PopNext();
			}

			return completeTracks.Count > 0;
		}


		public bool WaverIsSPIsTooClose(int p1SPInd, int p2SPInd, int posInd)
		{
			int distToSP1 = GetBlockDistance(p1SPInd, posInd);
			int distToSP2 = GetBlockDistance(p2SPInd, posInd);
			int distLimit = STAY_AWAY_FROM_SP_DIST_DEFENSIVE;
			bool SP1IsClose = distToSP1 < distLimit;
			bool SP2IsClose = distToSP2 < distLimit;
			return SP1IsClose || SP2IsClose;
		}

		public int WaverIsSPIsTooClose(int p1SPInd, int p2SPInd, Track track)
		{
			int minDistToSP1 = GetBlockDistance(p1SPInd, track.PeekNext());
			int minDistToSP2 = GetBlockDistance(p2SPInd, track.PeekNext());
			for (int i = 1; i < track.Length(); ++i)
			{
				minDistToSP1 = System.Math.Min(minDistToSP1, GetBlockDistance(p1SPInd, track.Peek(i)));
				minDistToSP2 = System.Math.Min(minDistToSP2, GetBlockDistance(p2SPInd, track.Peek(i)));
			}

			int distLimit = STAY_AWAY_FROM_SP_DIST_DEFENSIVE;
			bool SP1IsClose = minDistToSP1 < distLimit;
			bool SP2IsClose = minDistToSP1 < distLimit;
			int val1 = SP1IsClose ? -distLimit + minDistToSP1 : 0;
			int val2 = SP2IsClose ? -distLimit + minDistToSP2 : 0;
			return System.Math.Min(val1, val2);
		}

		const int TrackLengthWeight = 1;
		const int HasLastVisitedWeight = 1;
		const int HasHouseWeight = 10;
		const int IsTowardsSPWeight = 5;

		public Track ChooseTrack(int p1SPInd, int p2SPInd, int lastVisitedBlock)
		{
			if (completeTracks.Count == 0)
			{
				return null;
			}

			// evaluate tracks
			foreach (Track track in completeTracks)
			{
				int houseOnTheTrack = 0;
				for (int h = 0; h < AI.destHouseInds.Count; h++)
				{
					if (track.HasBlock(AI.destHouseInds[h].houseTileInd))
					{
						houseOnTheTrack = 1;
						break;
					}
				}
				bool lastVisitedOnTheTrack = track.HasBlock(lastVisitedBlock);
				int isTowardsSP = WaverIsSPIsTooClose(p1SPInd, p2SPInd, track);

				// the higher the grade, the better
				// this grades according to players on the track. If still there's player outside the track but close - no avoidance is done. TODO: improve this...
				track.grade = TrackLengthWeight * track.Length() +
								HasLastVisitedWeight * (lastVisitedOnTheTrack ? -1 : 0) +
								HasHouseWeight * houseOnTheTrack +
								IsTowardsSPWeight * isTowardsSP;
				track.makeHaste = houseOnTheTrack > 0;
			}

			completeTracks.Sort(trackComparison);
			int bestGrade = completeTracks[0].grade;

			int bestGradeCnt = 1;
			for (int i = 1; i < completeTracks.Count; ++i)
			{
				if (completeTracks[i].grade == bestGrade)
					bestGradeCnt++;
				else
					break;
			}

			int chosenTrackInd = Random.Range(0, bestGradeCnt - 1);
			Track chosen = completeTracks[chosenTrackInd];

			// make sure we don't choose a track leading to a spawning point
			if (chosen.HasBlock(p1SPInd) || chosen.HasBlock(p2SPInd))
			{
				chosen = null;
				for (int i = 0; i < completeTracks.Count; ++i)
				{
					if (!completeTracks[i].HasBlock(p1SPInd) && !completeTracks[i].HasBlock(p2SPInd))
					{
						chosen = completeTracks[i];
						break;
					}
				}
			}
			return chosen;
		}
	}
}
