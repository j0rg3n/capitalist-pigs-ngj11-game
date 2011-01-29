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
		public const int MAX_DEVS_IN_INDIE_HOUSE = 25;
		public const int MIN_DEVS_TO_CREATE_A_HOUSE = 5;

		public int houseTileInd = -1;
		public Vector3 baseWorldPos;
		public bool isPresent = false;
		public int devsCount = 0;
		public int waitingCount = 0;
		List<DevGuy> waitingDevs = new List<DevGuy>();

		IndieStudioBehavior studio;

		public bool IsFull()
		{
			return studio != null ? studio.indieDevCount >= MAX_DEVS_IN_INDIE_HOUSE : false;
		}

		public bool CanCreate()
		{
			return waitingCount >= MIN_DEVS_TO_CREATE_A_HOUSE;
		}


		public bool Overlaps(DevGuy devGuy)
		{
			// TODO:m 
			return false;
		}

		public void AddWaiting(DevGuy devGuy)
		{
			waitingCount++;
			waitingDevs.Add(devGuy);
		}

		public void AddDev(DevGuy devGuy)
		{
			studio.indieDevCount++;
			// TODO:m destroy the dev
		}

		public void CreateHouse()
		{
			System.Diagnostics.Debug.Assert(!isPresent);
			isPresent = true;

			Transform buildingTransform = GlobalObjects.GetDevAIBehaviour().InstantiateBuilding();

			var offset = buildingTransform.GetComponent<MeshFilter>().mesh.bounds.extents;
			offset.Scale(Vector3.up);

            buildingTransform.position = baseWorldPos + offset;

			studio = buildingTransform.GetComponent<IndieStudioBehavior>();
			studio.location = this;
			studio.indieDevCount = waitingCount;
			studio.devTimeSeconds = 40 / studio.indieDevCount;
			waitingCount = 0;
			
			// TODO:m destroy the devs
			waitingDevs.Clear();
		}

		public void HouseDestroyed()
		{
			System.Diagnostics.Debug.Assert(isPresent);
			isPresent = false;
			studio = null;
		}


	}
}
