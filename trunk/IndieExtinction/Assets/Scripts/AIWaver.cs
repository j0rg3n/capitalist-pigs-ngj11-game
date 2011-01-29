using UnityEngine;
using System.Collections.Generic;

namespace Irrelevant.Assets.Scripts.AI
{
	public class Waver
	{
		public GlobalDevAIBehaviour AI;


		public const int STAY_AWAY_FROM_SP_DIST_DEFENSIVE = 1;
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
			return new Vector2(i * AI.BlockSize + AI.BlockSize * 0.5f, j * AI.BlockSize + AI.BlockSize * 0.5f);
		}

		public Vector2 BlockIndexToPosition(int ind)
		{
			int i, j;
			BlockIndexToCoords(ind, out i, out j);
			return BlockCoordsToPosition(i, j);
		}

		public int PositionToBlockIndex(Vector2 pos)
		{
			int i = (int)(pos.x / AI.BlockSize);
			int j = (int)(pos.y / AI.BlockSize);
			System.Diagnostics.Debug.Assert(0 <= i && i < AI.MapWidth);
			System.Diagnostics.Debug.Assert(0 <= j && j < AI.MapHeight);
			return BlockCoordsToIndex(i, j);
		}

		public bool PassableBlock(int[,] map, int blockInd)
		{
			int i, j;
			BlockIndexToCoords(blockInd, out i, out j);
			return map[i, j] > 1;
		}

		void FindNeightbourBlocks(int block, ref int[] neighbours)
		{
			// TODO:m should not wrap around
			// up
			neighbours[0] = block - AI.MapWidth;
			if (neighbours[0] < 0)
				neighbours[0] += AI.MapSize;

			// down
			neighbours[1] = block + AI.MapWidth;
			if (neighbours[1] >= AI.MapSize)
				neighbours[1] -= AI.MapSize;

			// left
			neighbours[2] = block - 1;
			if (neighbours[2] < 0)
				neighbours[2] += AI.MapSize;
			else if ((neighbours[2] + 1) % AI.MapWidth == 0)  // block is in left screen border
			{
				neighbours[2] += AI.MapWidth;
			}

			// right
			neighbours[3] = block + 1;
			if (neighbours[3] >= AI.MapSize)
				neighbours[3] -= AI.MapSize;
			else if ((neighbours[3]) % AI.MapWidth == 0)  // block is in right screen border
			{
				neighbours[3] -= AI.MapWidth;
			}
		}


		// returns true if atleas one track exists
		public bool StartWave(int start, int[,] map) // 0 - not passable, 1 - rotating, 2 - empty (passable))
		{
			tracks.Clear();
			completeTracks.Clear();

			Track stratTrack = new Track(start);
			tracks.Add(stratTrack);

			int[] neighbours = new int[4] { 0, 0, 0, 0 };

			while (tracks.Count > 0 && tracks[0].CanExtend())
			{
				Track nextTrack = tracks[0];
				tracks.RemoveAt(0);
				int endBlock = nextTrack.GetLast();
				FindNeightbourBlocks(endBlock, ref neighbours);
				bool foundNeighbour = false;
				for (int i = 0; i < 4; ++i)
				{
					if (PassableBlock(map, neighbours[i]) && !nextTrack.HasBlock(neighbours[i]))
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
