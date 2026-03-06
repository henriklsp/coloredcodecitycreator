using UnityEngine;
using System.Collections.Generic;

namespace Components 
{
public class PackageComponent : MonoBehaviour, IComponent 
{
	internal static PackageComponent prefab;
	private static PackageComponent root;
	private float _length;
	private float _depth;
	private float leftSideLength;
	private string fullName = "";

	private List<IComponent> components = new List<IComponent>();
	private const float STREET_WIDTH = 1.5f;
	private const float MIN_STREET_SIZE = 3f;
	private const float SPACE = 0.5f;

	public static PackageComponent getRoot()
	{
		if (root == null)
		{
			root = Object.Instantiate(prefab) as PackageComponent;
			(root as MonoBehaviour).gameObject.name = "root";
		}

		return root;
	}

	public PackageComponent getSubPackage(string name)
	{
		int dot = name.IndexOf('.');
		if (dot > 0) //recurse through hierarchy of subpackages
		{
			string firstName = name.Substring(0,dot);
			string lastName = name.Substring(dot+1);			
			PackageComponent first = getSubPackageOrNull(firstName);
			if (first == null)
			{
				first = create(firstName);
			}
			return first.getSubPackage(lastName);
		}
		else //return the package
		{
			PackageComponent p = getSubPackageOrNull(name);
			if (p==null)
			{
				p = create(name);
			}
			return p;
		}
	}

	public string getName()
	{

		return gameObject.name;
	}

	public string getFullName()
	{
		return fullName;
	}

	public float length()
	{
		return _length;
	}
	
	public float depth()
	{
		return _depth;
	}

	public void addComponent(IComponent component)
	{
		components.Add(component);
	}

	public void organize()
	{
		float leftTotalLength = 0;
		float leftMaxDepth = 0;
		float rightTotalLength = 0;
		float rightMaxDepth = 0;
		bool leftSide;
		float positionAlong=0;
		float positionAcross=0;
		float lastPosition=MIN_STREET_SIZE;
		foreach (IComponent component in components)
		{
			if (component is PackageComponent)
				(component as PackageComponent).organize();
			else if (component is ClassComponent)
				(component as ClassComponent).createVisuals();
			if (leftTotalLength < rightTotalLength)
			{
				leftSide = true;
				leftTotalLength += SPACE;
				if (component is PackageComponent)
					positionAlong = leftTotalLength + (component as PackageComponent).leftSideLength + STREET_WIDTH/2;
				else
					positionAlong = leftTotalLength + component.length() / 2;
				leftTotalLength += component.length();
				positionAcross = STREET_WIDTH/2;
				if (component is ClassComponent)
					positionAcross += component.depth() / 2;
				if (component.depth() > leftMaxDepth)
					leftMaxDepth = component.depth();
			}
			else
			{
				leftSide = false;
				rightTotalLength += SPACE;
				if (component is PackageComponent)
					positionAlong = rightTotalLength + component.length() - (component as PackageComponent).leftSideLength + STREET_WIDTH/2;
				else
					positionAlong = rightTotalLength + component.length() / 2;
				rightTotalLength += component.length();
				positionAcross = -STREET_WIDTH/2;
				if (component is ClassComponent)
					positionAcross -= component.depth() / 2;
				if (component.depth() > rightMaxDepth)
					rightMaxDepth = component.depth();
			}
			Transform t = (component as MonoBehaviour).transform;
			t.parent = this.transform;
			Vector3 position = new Vector3(positionAlong, t.position.y, positionAcross);
			t.position = position;

			if (leftSide)
				t.localEulerAngles = new Vector3(0,-90,0);
			else
				t.localEulerAngles = new Vector3(0,90,0);
                lastPosition = Mathf.Max(lastPosition, positionAlong+STREET_WIDTH/2);
		}

		//rotated 90 degrees in relation to parent
		//from parents point of view depth = length of this street
		_depth = Mathf.Max(leftTotalLength, rightTotalLength);
		_depth = Mathf.Max(_depth, MIN_STREET_SIZE) + SPACE;
		_length = leftMaxDepth + rightMaxDepth + STREET_WIDTH;
		leftSideLength = leftMaxDepth;

		//set size and position of street itself
		Transform streetTrans = transform.GetChild(0).transform;
		streetTrans.position = new Vector3(lastPosition/2, 0, 0);
		streetTrans.localScale = new Vector3(lastPosition, 0.01f, STREET_WIDTH);
	}

	private PackageComponent getSubPackageOrNull(string name)
	{
		for (int i=0; i<components.Count; i++)
		{
			if (components[i].getName() == name
			    && components[i] is PackageComponent)
			{
				return components[i] as PackageComponent;
			}
		}
		return null;
	}

	private PackageComponent create (string name)
	{
		PackageComponent package = Object.Instantiate(prefab) as PackageComponent;
		package.gameObject.name = name;
		if (this == root)
			package.fullName = name;
		else
			package.fullName = this.fullName + "." + name;
		components.Add(package);
		return package;
	}
}
}