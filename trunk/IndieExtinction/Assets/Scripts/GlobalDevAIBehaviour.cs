using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;
using Irrelevant.Assets.Scripts.AI;
using Irrelevant.Assets.Scripts;

public class GlobalDevAIBehaviour : MonoBehaviour
{
	public float BlockSize = 3f; // units width of a block
	public int MapWidth = 60;
	public int MapHeight = 40;
	public float calmSpeed = 0.03f;
	public float hasteSpeedMultiplier = 1.5f;

	public int MapSize;
	public float MapWidthUnits;
	public float MapHeightUnits;

	float fElapsedSeconds;

	public Texture2D mapFieldTexture;
	public Transform buildingPrefab;
	public Transform startBuildingPrefab;

	public Transform InstantiateBuilding()
	{
		return (Transform)Instantiate(buildingPrefab);
	}


	public Transform debugPrefab;

	// TODO:3 consider keeping this in a single-dimention array called map
	int[,] levelMatrix; // 0 - not passable, 1 - passable
	int startSpawnPointInd = -1;
	public struct HouseIndices
	{
		public HouseIndices(int ind1, int ind2)
		{
			houseTileInd = ind1;
			houseInd = ind2;
		}

		public int houseTileInd;
		public int houseInd;
	}
	public List<HouseIndices> destHouseInds = new List<HouseIndices>();

	Waver waver = new Waver();

    // ****************************************************************
    // *********** AI: main procedure *********************************
    // ****************************************************************

	Color colorPass = new Color(1f, 0f, 0f);
	Color colorIndieHouse = new Color(1f, 1f, 0f);
	Color colorStartPoint = new Color(1f, 0f, 1f);
	

	public int studiosCreated = 0;

	// Use this for initialization
	void Start () 
	{
		waver.AI = this;

        GetComponent<MeshRenderer>().material.mainTexture = mapFieldTexture;

		MapWidth = mapFieldTexture.width;
		MapHeight = mapFieldTexture.height;

        // Create Level Matrix
        MapSize = MapWidth * MapHeight;
		MapWidthUnits = MapWidth * BlockSize;
		MapHeightUnits = MapHeight * BlockSize;
        levelMatrix = new int[MapWidth, MapHeight];
		bool bFoundStartingPoint = false;
		GlobalObjects.indieHouseLocations = new List<IndieHouseLocation>();
        for (int i = 0; i < MapWidth; ++i)
        {
            for (int jj = 0; jj < MapHeight; ++jj)
            {
				Color pixel = mapFieldTexture.GetPixel(i, jj);
				int j = MapHeight - jj - 1;

				if (pixel == colorPass || pixel == colorIndieHouse || pixel == colorStartPoint)
				{
					levelMatrix[i, j] = 1;
				}
				else
				{
					levelMatrix[i, j] = 0;
				}

				if (pixel == colorStartPoint)
				{
					System.Diagnostics.Debug.Assert(!bFoundStartingPoint);
					bFoundStartingPoint = true;
					startSpawnPointInd = waver.BlockCoordsToIndex(i, j);

					Vector3 baseWorldPos = MathUtil.GetWorldPositionFromGridCoordinate(GetComponent<MeshFilter>(), i + .5f, j + .5f, MapWidth, MapHeight);
					Transform startBuilding = (Transform)Instantiate(startBuildingPrefab);

					var offset = startBuilding.GetComponent<MeshFilter>().mesh.bounds.extents;
					offset.Scale(Vector3.up);
					startBuilding.position = baseWorldPos + offset;
				}
				if (pixel == colorIndieHouse)
				{
					IndieHouseLocation location = new IndieHouseLocation();
					location.houseTileInd = waver.BlockCoordsToIndex(i, j);
					location.locationInd = GlobalObjects.indieHouseLocations.Count;
					GlobalObjects.indieHouseLocations.Add(location);
					++studiosCreated;

					location.baseWorldPos = MathUtil.GetWorldPositionFromGridCoordinate(GetComponent<MeshFilter>(), i + .5f, j + .5f, MapWidth, MapHeight);

                    // TODO:m remove
                    //location.waitingCount = 5;
                    //location.CreateHouse();
                }
			}
        }

		/*
		for (int i = 25; i < 28; ++i)
		{
			for (int j = 1; j < 4; ++j)
			{
				if (levelMatrix[i, MapHeight - j - 1] > 0)
				{
					Vector3 baseWorldPos = MathUtil.GetWorldPositionFromGridCoordinate(GetComponent<MeshFilter>(), i + .5f, j + .5f, MapWidth, MapHeight);
					Transform newDev = (Transform)Instantiate(debugPrefab, baseWorldPos, Quaternion.identity);
					newDev.position = baseWorldPos;
				}
			}
		}*/

		System.Diagnostics.Debug.Assert(bFoundStartingPoint);
	}

	void CalculateInitialRoutes() // TODO:m call
	{
		Update();
	}
	
	// ****************************************************************
	// *********** Level Matrix stuff *********************************
	// ****************************************************************

	int ClampX(int x)
	{
		if (x < 0)
			x += MapWidth;
		if (x >= MapWidth)
			x -= MapWidth;
		return x;
	}

	int ClampY(int y)
	{
		if (y < 0)
			y += MapHeight;
		if (y >= MapHeight)
			y -= MapHeight;
		return y;
	}

	bool IsPassable(int i, int j)
	{
		return levelMatrix[ClampX(i), ClampY(j)] > 0;
	}

	bool CheckImmediateProximity(DevGuy devGuy, int devBlockInd, HouseIndices houseInds)
	{
		IndieHouseLocation location = GlobalObjects.indieHouseLocations[houseInds.houseInd];
		bool overlaps = false;
		if (!location.isPresent)
		{
			overlaps = location.houseTileInd == devBlockInd;
		}
		else
		{
			overlaps = location.Overlaps(devGuy);
		}


		if (overlaps)
		{
			if (location.isPresent)
			{
				if (!location.IsFull())
				{
					location.AddDev(devGuy);
				}
			}
			else
			{
				devGuy.waitingOthers = true;
				location.AddWaiting(devGuy);
				if (location.CanCreate())
				{
					location.CreateHouse();
				}
			}
			return true;
		}
		return false;
	}

    // ****************************************************************	
    // *********** DevGuy updating *************************************
    // ****************************************************************

	class DevToProcess
	{
		public DevGuy devGuy;
		public bool hasTrack;
		public int currentBlock;
		public int p1SPInd;
		public int p2SPInd;
	}

	static int CompareDevsToProcess(DevToProcess a, DevToProcess b)
	{
		int ai = a.hasTrack ? -1 : a.currentBlock;
		int bi = b.hasTrack ? -1 : b.currentBlock;
		return bi - ai;
	}

	System.Comparison<DevToProcess> DevToProcessComparison = new System.Comparison<DevToProcess>(CompareDevsToProcess);

	// Update is called once per frame
	void Update()
	{
		fElapsedSeconds = Time.deltaTime; // seconds
		destHouseInds.Clear();

		for (int h = 0; h < GlobalObjects.indieHouseLocations.Count; h++)
		{
			if (!GlobalObjects.indieHouseLocations[h].isPresent || !GlobalObjects.indieHouseLocations[h].IsFull())
			{
				destHouseInds.Add(new HouseIndices(GlobalObjects.indieHouseLocations[h].houseTileInd, h));
			}
		}

		IndieDevBehavior[] indieDevs = GlobalObjects.GetIndieDevs();
		List<DevToProcess> devsToProcess = new List<DevToProcess>();

		foreach (IndieDevBehavior indieDev in indieDevs)
		{
			if (!indieDev.alive)
				continue;

			DevGuy devGuy = indieDev.aiDevGuy;
			int currentBlock = waver.PositionToBlockIndex(devGuy.Position);
			for (int h = 0; h < destHouseInds.Count; h++)
			{
				bool bSkip = CheckImmediateProximity(devGuy, currentBlock, destHouseInds[h]);
				if (bSkip)
					continue;
			}
			
			// update the track if it needs to
			if (devGuy.currentTrack != null && devGuy.currentTrack.Length() == 0)
				devGuy.currentTrack = null;

			int p1SPInd = startSpawnPointInd;
			// TODO:m optimize by not doing stuff for the second spawn point if too slow
			int p2SPInd = devGuy.lastHousePointInd >= 0 ? devGuy.lastHousePointInd : startSpawnPointInd;

			if (devGuy.currentTrack != null)
			{
				System.Diagnostics.Debug.Assert(!devGuy.currentTrack.HasBlock(p1SPInd));
				System.Diagnostics.Debug.Assert(!devGuy.currentTrack.HasBlock(p2SPInd));

				if (waver.WaverIsSPIsTooClose(p1SPInd, p2SPInd, devGuy.currentTrack.PeekNext()))
					devGuy.currentTrack = null;
			}

			// probably not necessary - was needed since tiles could rotate ad block the path
			//if (devGuy.currentTrack != null)
			//{
			//    int icur, jcur;
			//    Waver.BlockIndexToCoords(devGuy.currentTrack.PeekNext(), out icur, out jcur);
			//    if (!Waver.PassableBlock(levelMatrix, devGuy.currentTrack.PeekNext()))
			//        devGuy.currentTrack = null;
			//}

			DevToProcess d = new DevToProcess();
			d.devGuy = devGuy;
			d.hasTrack = devGuy.currentTrack != null;
			//UnityEngine.Debug.Log(string.Format("pos {0} {1}", devGuy.Position.x, devGuy.Position.y));
			d.currentBlock = currentBlock;
			UnityEngine.Debug.Log(string.Format("current block ind {0}", d.currentBlock));
			d.p1SPInd = p1SPInd;
			d.p2SPInd = p2SPInd;
			
			devsToProcess.Add(d);
		}
		devsToProcess.Sort(DevToProcessComparison);
		int i=0;
		while (i<devsToProcess.Count && devsToProcess[i].hasTrack)
		{
			++i;
			continue;
		}
		while (i<devsToProcess.Count)
		{
			int j=i+1;
			while (j<devsToProcess.Count && devsToProcess[i].currentBlock == devsToProcess[j].currentBlock)
				++j;

			bool hasTracks = waver.StartWave(devsToProcess[i].currentBlock, levelMatrix);
			if (hasTracks)
			{
				for (int k = i; k < j; ++k)
				{
					devsToProcess[k].devGuy.currentTrack = waver.ChooseTrack(devsToProcess[k].p1SPInd, devsToProcess[k].p2SPInd, devsToProcess[k].devGuy.lastVisitedBlockIndex);
					System.Diagnostics.Debug.Assert(devsToProcess[k].devGuy.currentTrack == null || !devsToProcess[k].devGuy.currentTrack.HasBlock(devsToProcess[k].p1SPInd));
					System.Diagnostics.Debug.Assert(devsToProcess[k].devGuy.currentTrack == null || !devsToProcess[k].devGuy.currentTrack.HasBlock(devsToProcess[k].p2SPInd));
					UnityEngine.Debug.Log(string.Format("Gave track to {0}", k));
					/*
					for (int ii = 0; ii < devsToProcess[k].devGuy.currentTrack.track.Count; ++ii)
					{
						int ind1, ind2;
						waver.BlockIndexToCoords(devsToProcess[k].devGuy.currentTrack.track[ii], out ind1, out ind2);
						Vector3 baseWorldPos = MathUtil.GetWorldPositionFromGridCoordinate(GetComponent<MeshFilter>(), ind1 + .5f, ind2 + .5f, MapWidth, MapHeight);
						Transform newDev = (Transform)Instantiate(debugPrefab, baseWorldPos, Quaternion.identity);
						newDev.position = baseWorldPos;
					}
					UnityEngine.Debug.Log(string.Format("{0}", devsToProcess[k].devGuy.currentTrack.ToSrting()));
					 */
				}
			}
			i=j;
		}

		foreach (DevToProcess d in devsToProcess)
		{
			if (d.devGuy.currentTrack == null)
				continue;
			// do smooth movement
			int nextBlockInd = d.devGuy.currentTrack.PeekNext();
			UnityEngine.Debug.Log(string.Format("next block ind {0}", nextBlockInd));
			//Debug.Print(String.Format("{0}, cur {1}, next {2}, track: {3}", devGuy.type, currentBlock, nextBlockInd, devGuy.currentTrack.ToSrting()));
			bool areWeThereYet = nextBlockInd != d.currentBlock ? MoveTowards(d.devGuy, d.currentBlock, nextBlockInd, (d.devGuy.currentTrack.makeHaste ? hasteSpeedMultiplier * calmSpeed : calmSpeed)) : true;
			if (areWeThereYet)
			{
				if (nextBlockInd != d.currentBlock)
					d.devGuy.lastVisitedBlockIndex = d.currentBlock;
				d.currentBlock = d.devGuy.currentTrack.PopNext();
			}
		}
	}


	/*
	bool UpdateDevGuy(DevGuy devGuy)
    {
        //int i, j;
        //SetMatrixValue(devGuy.Position, -1, out i, out j); // TODO:m what's this for?

        if (devGuy.currentTrack == null)
            return false; // don't skip

        // do smooth movement
        int nextBlockInd = devGuy.currentTrack.PeekNext();
        //Debug.Print(String.Format("{0}, cur {1}, next {2}, track: {3}", devGuy.type, currentBlock, nextBlockInd, devGuy.currentTrack.ToSrting()));
		bool areWeThereYet = nextBlockInd != d.currentBlock ? MoveTowards(devGuy, currentBlock, nextBlockInd, (devGuy.currentTrack.makeHaste ? hasteSpeed : calmSpeed)) : true;
        if (areWeThereYet)
        {
            if (nextBlockInd != currentBlock)
                devGuy.lastVisitedBlockIndex = currentBlock;
            currentBlock = devGuy.currentTrack.PopNext();
        }

        return false; // don't skip
    }*/

	public Vector2 ValidatePosition(Vector2 Position)
	{
		if (Position.x >= MapWidthUnits)
			Position.x = MapWidthUnits; // TODO:m good enough or less?
		if (Position.x < 0f)
			Position.x = 0f;
		if (Position.y >= MapHeightUnits)
			Position.y = MapHeightUnits;
		if (Position.y < 0f)
			Position.y = 0f;

		return Position;

		// TODO:2 This crashed, but now maybe it's fixed
		// Per: this asserted sometimes when player 2 took a banana (Position.Y == 800.0f && j == 28)
		//int i = (int)(Position.X / BlockSize);
		//int j = (int)(Position.Y / BlockSize);
		//if (i == MapWidth)
		//{
		//    Position.X -= Game1.GetSingleton().graphics.PreferredBackBufferWidth;
		//    i = (int)(Position.X / BlockSize);
		//}
		//if (j < MapHeight)
		//{
		//    Position.Y -= Game1.GetSingleton().graphics.PreferredBackBufferHeight;
		//    j = (int)(Position.Y / BlockSize);
		//}
		//Debug.Assert(0 <= i && i < MapWidth);
		//Debug.Assert(0 <= j && j < MapHeight);
	}


    // smooth movement
    bool MoveTowards(DevGuy devGuy, int fromBlockInd, int toBlockInd, float speed)
    {
        bool destinationReached = false;
        float trajectory = fElapsedSeconds * speed;

        int toX, toY, fromX, fromY;
        waver.BlockIndexToCoords(toBlockInd, out toX, out toY);
		waver.BlockIndexToCoords(fromBlockInd, out fromX, out fromY);

        // these are here to account  for the borders-of-the-map teleportation 
        if ((fromX - toX) == (MapWidth - 1))
            toX += MapWidth;
        else if ((toX - fromX) == (MapWidth - 1))
            toX -= MapWidth;

        if ((fromY - toY) == (MapHeight - 1))
            toY += MapHeight;
        else if ((toY - fromY) == (MapHeight - 1))
            toY -= MapHeight;

        Vector2 destination = waver.BlockCoordsToPosition(toX, toY);
        Vector2 direction = destination - devGuy.Position;

        Vector2 directionN = direction;
		directionN.Normalize();
        float dist = direction.magnitude;
        destinationReached = dist <= trajectory;
        if (destinationReached)
        {
            devGuy.Position = destination;
        }
        else
        {
            devGuy.Position += directionN * trajectory;
        }

		devGuy.Position = ValidatePosition(devGuy.Position);
        return destinationReached;
    }

}

