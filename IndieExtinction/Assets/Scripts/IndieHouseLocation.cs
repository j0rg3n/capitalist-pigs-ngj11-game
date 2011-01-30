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
		public const int MAX_DEVS_IN_INDIE_HOUSE = 4;
		public const int MIN_DEVS_TO_CREATE_A_HOUSE = 3;

		public int houseTileInd = -1;
		public Vector3 baseWorldPos;
		public bool isPresent = false;
		public int devsCount = 0;
		public int waitingCount = 0;
		List<DevGuy> waitingDevs = new List<DevGuy>();

		IndieStudioBehavior studio;
		public int locationInd;

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
			Vector3 worldPos = devGuy.indieDevBehaviour.GetAIWorldTransform();
			if (isPresent)
			{
				MeshFilter studioMesh = studio.GetComponent<MeshFilter>();
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

		public void AddWaiting(DevGuy devGuy)
		{
			waitingCount++;
			waitingDevs.Add(devGuy);
		}

		public void AddDev(DevGuy devGuy)
		{
			studio.indieDevCount++;
			IndieStudioBehavior.Destroy(devGuy.indieDevBehaviour.gameObject);
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

			foreach (DevGuy devGuy in waitingDevs)
			{
				IndieStudioBehavior.Destroy(devGuy.indieDevBehaviour.gameObject);
			}
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
