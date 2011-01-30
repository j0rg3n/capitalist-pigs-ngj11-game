using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irrelevant.Assets.Scripts.AI;

namespace Irrelevant.Assets.Scripts
{
	public class IndieHouseLocation
	{
		public const int MAX_DEVS_IN_INDIE_HOUSE = 20;
		public const int MIN_DEVS_TO_CREATE_A_HOUSE = 10;
		public const float CURSE_DURATION = 4f;

		public int houseTileInd = -1;
		public Vector3 baseWorldPos;
		public bool isPresent = false;
		public int devsCount = 0;
		public bool cursed = false;
		public float cursedDuration = 0f;

		IndieStudioBehavior studio;
		public int locationInd;

		public Waver waver;

		public bool IsFull()
		{
			return studio != null ? studio.IndieDevCount >= MAX_DEVS_IN_INDIE_HOUSE : false;
		}

		public bool CanCreate()
		{
			return !cursed;
		}


		public bool Overlaps(DevGuy devGuy)
		{
			Vector3 worldPos = devGuy.indieDevBehaviour.GetAIWorldTransform();
			if (isPresent)
			{
				MeshFilter studioMesh = studio.GetComponent<MeshFilter>(); // TODO:m this has been deleted :(
				Transform transform = studio.GetComponent<Transform>();

				var bounds = studioMesh.mesh.bounds;
				Vector3 localPos = transform.InverseTransformPoint(worldPos);
				bool inside = localPos.x <= bounds.center.x + bounds.extents.x;
				inside &= localPos.x >= bounds.center.x - bounds.extents.x;
				inside &= localPos.z <= bounds.center.z + bounds.extents.z;
				inside &= localPos.z >= bounds.center.z - bounds.extents.z;
				return inside;
			}
			return false;
		}


		public void AddDev(DevGuy devGuy)
		{
			studio.IndieDevCount++;
			IndieStudioBehavior.Destroy(devGuy.indieDevBehaviour.gameObject);
		}

		public void CreateHouse(List<IndieDevBehavior> waitingDevs)
		{
			System.Diagnostics.Debug.Assert(!isPresent);
			isPresent = true;

			Transform buildingTransform = GlobalObjects.GetDevAIBehaviour().InstantiateBuilding();

			var offset = buildingTransform.GetComponent<MeshFilter>().mesh.bounds.extents;
			offset.Scale(Vector3.up);

            buildingTransform.position = baseWorldPos + offset;
            GlobalObjects.GetGlobbalGameState().ScaleInstance(buildingTransform);

			studio = buildingTransform.GetComponent<IndieStudioBehavior>();
			studio.location = this;
			studio.IndieDevCount = waitingDevs.Count;

			foreach (IndieDevBehavior indieDev in waitingDevs)
			{	
				IndieStudioBehavior.Destroy(indieDev.gameObject);
			}
			waitingDevs.Clear();

			waver.AI = GlobalObjects.GetDevAIBehaviour();
			waver.StartWave(houseTileInd, GlobalObjects.GetDevAIBehaviour().levelMatrix);
		}

		public void HouseDestroyed()
		{
			if (!isPresent)
				return;
			
			isPresent = false;
			studio = null;
			//return;
			
			cursed = true;
			cursedDuration = 0f;
			/*
			IndieDevBehavior[] indieDevs = GlobalObjects.GetIndieDevs();
			foreach (IndieDevBehavior indieDev in indieDevs)
			{
				if (!indieDev.alive)
					continue;
				DevGuy devGuy = indieDev.aiDevGuy;
				// update the track if it needs to
				if (devGuy.currentTrack != null && devGuy.currentTrack.track.Contains(houseTileInd))
				{
					devGuy.currentTrack = null;
				}
			}
			 */
		}

		public void UpdateCurseState(float fElapsedSeconds)
		{
			if (cursed)
			{
				cursedDuration += fElapsedSeconds;
				if (cursedDuration >= CURSE_DURATION)
				{
					cursed = false;
					cursedDuration = 0f;
				}
			}
		}

	}
}
