using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Components;

namespace UI 
{
public class NamesView : MonoBehaviour 
{
	public GUIStyle style;
//	public static IList<ClassComponent> classesNotShowing;
	public static IList<PackageComponent> packagesNotShowing;
//	private static IList<ClassComponent> classesToShow;
	private static IList<PackageComponent> packagesToShow;
	private static int[] linesOccupied;
	private double timeToNextUpdate = 0;
	private const int LABEL_WIDTH = 200;
	private const int PACKAGES_TO_SHOW = 10;
	private const int CLASSES_TO_SHOW = 5;
	private const int IDEAL_DIST_FROM_CAM = 40;
	private const int LINES = 15;

	public static void setAllClasses(ClassComponent[] classes)
	{
//		classesNotShowing = new List<ClassComponent>(classes);
		linesOccupied = new int[LINES];
	}

	public static void setAllPackages(PackageComponent[] packages)
	{
		packagesNotShowing = new List<PackageComponent>(packages);
	}

	void Start () 
	{
//		classesToShow = new List<ClassComponent>();
		packagesToShow = new List<PackageComponent>();
	}

	void Update () 
	{
        if (packagesToShow == null || packagesNotShowing == null || linesOccupied == null)
			return;
		timeToNextUpdate -= Time.deltaTime;
		if (timeToNextUpdate > 0)
			return;
		timeToNextUpdate = 0.5;
		updateLines();

        PackageComponent mostSuitableNotShowing = getMostSuitableNotShowingPackage();
        if (packagesToShow.Count < PACKAGES_TO_SHOW && mostSuitableNotShowing != null)
        {
            packagesToShow.Add(mostSuitableNotShowing);
            packagesNotShowing.Remove(mostSuitableNotShowing);
        }
        else if (mostSuitableNotShowing != null)
        {
            PackageComponent leastSuitableShowing = getLeastSuitableShowingPackage();
            if (suitability(leastSuitableShowing) > suitability(mostSuitableNotShowing) + 1000)
            {
                packagesToShow.Remove(leastSuitableShowing);
                packagesNotShowing.Add(leastSuitableShowing);
                packagesToShow.Add(mostSuitableNotShowing);
                packagesNotShowing.Remove(mostSuitableNotShowing);
            }
        }
	}

	void OnGUI()
	{
		if (packagesToShow == null)
			return;

		GUI.color = Color.white;
		foreach (PackageComponent package in packagesToShow)
		{
			if (isInFrontOfCamera(package.transform.position))
			{
				Vector3 screenPoint = Camera.main.WorldToScreenPoint(package.transform.position);
				if (screenPoint.x > 0 
				    && screenPoint.x < Screen.width 
				    && screenPoint.y > 0 
				    && screenPoint.y < Screen.height)
				{
					Rect rect = new Rect(screenPoint.x, Screen.height-screenPoint.y, LABEL_WIDTH, style.fontSize+10);
					string name = package.getFullName();
					GUI.Label(rect, name, style);
				}
			}
		}
	}

	private PackageComponent getLeastSuitableShowingPackage ()
	{
		PackageComponent leastSuitableShowing = null;
		int lowestSuitability = 0;
        for (int i=0; i<packagesToShow.Count; i++)
		{
			int s = suitability(packagesToShow[i]);
			if (s<lowestSuitability)
			{
				lowestSuitability = s;
				leastSuitableShowing = packagesToShow[i];
			}
		}
		return leastSuitableShowing;
	}
	
	private PackageComponent getMostSuitableNotShowingPackage ()
	{
        PackageComponent mostSuitable = null;
        int highestSuitability = 0;
        for (int i=0; i<packagesNotShowing.Count; i++)
        {
            int s = suitability(packagesNotShowing[i]);
            if (s > highestSuitability)
            {
                highestSuitability = s;
                mostSuitable = packagesNotShowing[i];
            }
        }
        return mostSuitable;
	}

	private int suitability(MonoBehaviour element)
	{
        if (element == null)
            return 0;
        if (linesOccupied[getLineNum(element.transform)] > 0)
            return 0;
		int suitability = (int)((element as IComponent).length() * (element as IComponent).depth());
		Transform cam = Camera.main.transform;
		if (isInFrontOfCamera(element.transform.position))
			suitability += 100000;
		Vector3 inFrontOfCamera = cam.position + cam.forward * IDEAL_DIST_FROM_CAM;
		Vector3 diff = element.transform.position - inFrontOfCamera;
		suitability -= (int)((diff.x * diff.x) + (diff.y * diff.y));
		return suitability;
	}

	private bool isInFrontOfCamera(Vector3 point)
	{
		Vector3 camToPoint = point - Camera.main.transform.position;
		return (Vector3.Dot(camToPoint, Camera.main.transform.forward) > 0);
	}
	
	private void updateLines()
	{
		for (int i=0; i<LINES; i++)
			linesOccupied[i] = 0;
		foreach (PackageComponent p in packagesToShow)
		{
			linesOccupied[getLineNum(p.transform)]++;
		}
	}

	private int getLineNum(Transform transform)
	{
		Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
		int lineNum = (int)(screenPoint.y / Screen.height * LINES);
		if (lineNum < 0)
			lineNum = 0;
		if (lineNum >= LINES)
			lineNum = LINES-1;
		return lineNum;
	}
}
}