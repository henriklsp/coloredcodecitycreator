using UnityEngine;
using System.Collections;
using Components;

namespace UI 
{
public class Panorama : MonoBehaviour 
{
	private ClassComponent[] allClasses;
	private PackageComponent[] allPackages;
	private float maxHeight;
	private Transform currentFocus;
	private Vector3 targetPosition;
	private Vector3 targetFacing;
	private float timeToNextPan;
	private const int PAN_TIME = 5;
	private const float PAN_SPEED = 1f;
	private const float ROTATE_SPEED = 0.1f;

	void Update () 
	{
		if (allClasses == null)
		{
			if (Creator.isComplete)
			{
				init();
			}
			return;
		}
		else
		{
			pan();
		}
	}

	private void pan()
	{
		timeToNextPan -= Time.deltaTime;
		if (timeToNextPan <= 0)
		{
			timeToNextPan = PAN_TIME;
			if (Random.Range(0,1) == 0)
			{
				currentFocus = allPackages[Random.Range(0,allPackages.Length-1)].transform;
			}
			else
			{
				currentFocus = allClasses[Random.Range(0,allClasses.Length-1)].transform;
			}
			Vector3 camDir = new Vector3(Random.Range(-30,-10), maxHeight+4, Random.Range(-25,25));
			targetPosition = currentFocus.position + camDir;
			targetFacing = currentFocus.position - targetPosition;
		}
		float distToMove = (targetPosition - transform.position).magnitude;
		float maxDelta = Time.deltaTime * PAN_SPEED * Mathf.Max(distToMove,1);
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, maxDelta);
		transform.forward = Vector3.RotateTowards(transform.forward, targetFacing, Time.deltaTime * ROTATE_SPEED, 0);
	}

	private void init()
	{
		allClasses = Object.FindObjectsOfType<ClassComponent>();
		NamesView.setAllClasses(allClasses);
		allPackages = Object.FindObjectsOfType<PackageComponent>();
		NamesView.setAllPackages(allPackages);
		currentFocus = allPackages[0].transform;
		targetPosition = transform.position;
		timeToNextPan = PAN_TIME * 2;
		foreach (ClassComponent classc in allClasses)
			if (classc.height() > maxHeight)
				maxHeight = classc.height();
	}
}
}