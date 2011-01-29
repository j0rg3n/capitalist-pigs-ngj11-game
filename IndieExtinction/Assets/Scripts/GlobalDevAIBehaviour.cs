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
	const float calmSpeed = 60f;
	const float hasteSpeed = calmSpeed * 1.5f;

	public int MapSize;
	public float MapWidthUnits;
	public float MapHeightUnits;

	float fElapsedSeconds;

	public Texture2D mapFieldTexture;
	public Transform buildingPrefab;

	public Transform InstantiateBuilding()
	{
		return (Transform)Instantiate(buildingPrefab);
	}

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
	public List<HouseIndices> destHouseInds;

	Waver waver = new Waver();

    // ****************************************************************
    // *********** AI: main procedure *********************************
    // ****************************************************************

	Color colorPass = new Color(1f, 0f, 0f);
	Color colorIndieHouse = new Color(1f, 1f, 0f);
	Color colorStartPoint = new Color(1f, 0f, 1f);
	

	public int studiosCreated = 0;

	public Color debugCol;
	public Color debugCol2;
	public Color debugCol3;
	public Color debugCol4;

	public float debugFloat1;
	public float debugFloat2;
	public float debugFloat3;
	public int debugInt1 = -1;
	public int debugInt2 = -1;
	public int debugInt3 = -1;

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
            for (int j = 0; j < MapHeight; ++j)
            {
				Color pixel = mapFieldTexture.GetPixel(i, j);

				if (pixel == colorPass || pixel == colorIndieHouse || pixel == colorStartPoint)
				{
					levelMatrix[i, j] = 1;
					debugCol = pixel;
				}
				else
				{
					levelMatrix[i, j] = 0;
				}

				if (pixel == colorStartPoint)
				{
					debugCol2 = pixel;
					System.Diagnostics.Debug.Assert(!bFoundStartingPoint);
					bFoundStartingPoint = true;
					startSpawnPointInd = waver.BlockCoordsToIndex(i, j);
					debugInt2 = startSpawnPointInd;
				}
				if (pixel == colorIndieHouse)
				{
					debugCol3 = pixel;
					IndieHouseLocation location = new IndieHouseLocation();
					location.houseTileInd = waver.BlockCoordsToIndex(i, j);
					if (debugInt1 < 0)
					{
						debugInt1 = location.houseTileInd;
					}
					GlobalObjects.indieHouseLocations.Add(location);
					++studiosCreated;

					location.baseWorldPos = MathUtil.GetWorldPositionFromGridCoordinate(GetComponent<MeshFilter>(), i + .5f, j + .5f, MapWidth, MapHeight);

                    // TODO:m remove
                    location.waitingCount = 5;
                    location.CreateHouse();
                }
			}
        }
		System.Diagnostics.Debug.Assert(bFoundStartingPoint);
	}

	void CalculateInitialRoutes() // TODO:m call
	{
		Update();
	}
	
	// Update is called once per frame
	void Update () 
	{
		return;

		fElapsedSeconds = (float)Time.deltaTime; // seconds
        destHouseInds.Clear();

		for (int h = 0; h < GlobalObjects.indieHouseLocations.Count; h++)
        {
			if (!GlobalObjects.indieHouseLocations[h].isPresent || !GlobalObjects.indieHouseLocations[h].IsFull())
            {
				destHouseInds.Add(new HouseIndices(GlobalObjects.indieHouseLocations[h].houseTileInd, h));
            }
        }

		foreach (IndieDevBehavior indieDev in GlobalObjects.GetIndieDevs())
		{
			UpdateDevGuy(indieDev.aiDevGuy);
		}
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

	bool CheckImmediateProximity(DevGuy devGuy, HouseIndices houseInds)
	{
		IndieHouseLocation location = GlobalObjects.indieHouseLocations[houseInds.houseInd];
		if (location.Overlaps(devGuy))
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

	bool UpdateDevGuy(DevGuy devGuy)
    {
        //int i, j;
        //SetMatrixValue(devGuy.Position, -1, out i, out j); // TODO:m what's this for?

		for (int h = 0; h < destHouseInds.Count; h++)
        {
            bool bSkip = CheckImmediateProximity(devGuy, destHouseInds[h]);
            if (bSkip)
                return bSkip;
        }


        // update the track if it needs to
        if (devGuy.currentTrack != null && devGuy.currentTrack.Length() == 0)
            devGuy.currentTrack = null;

        if (devGuy.updateHousePointInd)
        {
            devGuy.updateHousePointInd = false;
            devGuy.lastHousePointInd = -1;
            if (devGuy.hasHousePoint)
            {
                devGuy.lastHousePointInd = waver.PositionToBlockIndex(devGuy.lastHouseCoords);
            }
        }

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

        if (devGuy.noTracksDuring != 0) // TODO:m what's this for?
            return false;

        int currentBlock = waver.PositionToBlockIndex(devGuy.Position);
        if (devGuy.currentTrack == null)
        {
            bool hasTracks = waver.StartWave(currentBlock, levelMatrix);
            if (hasTracks)
            {
                devGuy.currentTrack = waver.ChooseTrack(p1SPInd, p2SPInd, devGuy.lastVisitedBlockIndex);
				System.Diagnostics.Debug.Assert(devGuy.currentTrack == null || !devGuy.currentTrack.HasBlock(p1SPInd));
				System.Diagnostics.Debug.Assert(devGuy.currentTrack == null || !devGuy.currentTrack.HasBlock(p2SPInd));
            }
            else
            {
                devGuy.currentTrack = null;
                ++devGuy.noTracksDuring;
                if (devGuy.noTracksDuring == DevGuy.NoTracksDelay)
                {
                    // TODO:3 test this
                    devGuy.noTracksDuring = 0;
                }
            }
        }

        if (devGuy.currentTrack == null)
            return false; // don't skip

        // do smooth movement
        int nextBlockInd = devGuy.currentTrack.PeekNext();
        //Debug.Print(String.Format("{0}, cur {1}, next {2}, track: {3}", devGuy.type, currentBlock, nextBlockInd, devGuy.currentTrack.ToSrting()));
        bool areWeThereYet = nextBlockInd != currentBlock ? MoveTowards(devGuy, currentBlock, nextBlockInd, (devGuy.currentTrack.makeHaste ? hasteSpeed : calmSpeed)) : true;
        if (areWeThereYet)
        {
            if (nextBlockInd != currentBlock)
                devGuy.lastVisitedBlockIndex = currentBlock;
            currentBlock = devGuy.currentTrack.PopNext();
        }

        return false; // don't skip
    }

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
        float dist = direction.magnitude; // TODO:m is this the length
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

