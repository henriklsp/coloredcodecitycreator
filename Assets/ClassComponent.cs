using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Components 
{
public class ClassComponent : MonoBehaviour, IComponent 
{
	private float size;
	private string packageName = "";
	private int numberOfDependencies = 0;
	private int numberOfDependants = 0;
	private int complexity = 0;
	private int maxComplexity = 0;
	private int numberOfFunctions = 0;
	private int logicalLinesOfCode = 0;
	private int offScale = 0;
	private const float HEIGHT_PER_FUNCTION = 0.4f;
	private const float HEIGHT_MIN = 0.2f;
	private const float HEIGHT_MAX = 40f;
	private const float VOL_PER_LOC = 0.1f;
	private const float SIZE_MIN = 0.1f;
	private const float SIZE_MAX = 20f;
	private const float R_PER_DEP = 0.05f;
	private const float G_PER_DEP = 0.05f;
	private const float B_PER_COMP = 0.05f;
	private static Dictionary<string,int> allDependencies
		= new Dictionary<string,int>();
	private static Dictionary<string,ClassComponent> allClasses
		= new Dictionary<string,ClassComponent>();

	public void readFromFile(FileInfo file)
	{
		using (StreamReader sr = file.OpenText())
		{
			string line = "";
			while ((line = sr.ReadLine()) != null) 
			{
				line = line.Trim();
				if (line.Length > 1)
				{
					readLine(line);
				}
			}
		}
		if (packageName == "")
			PackageComponent.getRoot().addComponent(this);
/*		Debug.Log(name+" package="+packageName+" dependencies="+numberOfDependencies
		          +" maxComplexity="+maxComplexity+" functions="+numberOfFunctions
		          +" lloc="+logicalLinesOfCode);*/
		allClasses.Add((packageName+"."+name).ToLower(), this);
	}
	
	public void setName(string name)
	{
		this.name = name;
		gameObject.name = name;
	}

	public string getName()
	{
		return gameObject.name;
	}

	public float length()
	{
		return size;
	}

	public float depth()
	{
		return size;
	}

	public float height()
	{
		return transform.localScale.y;
	}

    public string getPackageName()
    {
        return packageName;
    }

    public int getNumberOfFunctions()
    {
        return numberOfFunctions;
    }

    public int getLinesOfCode()
    {
        return logicalLinesOfCode;
    }

    public int getComplexity()
    {
        return maxComplexity;
    }

    public int getDependencies()
    {
        return numberOfDependencies;
    }

    public int getDependants()
    {
        return numberOfDependants;
    }

	private void readLine(string line)
	{
		if (line.StartsWith("//") || 
		    line.StartsWith("/*") || 
		    line.StartsWith("*") ||
		    line.StartsWith("<"))
		{
		}
		else if (line.StartsWith("package ") || line.StartsWith("namespace "))
		{
			parsePackage (line);
		}
		else if ((line.StartsWith("import ") || line.StartsWith("using "))
		         && line.EndsWith(";"))
		{
			parseImport (line);
		}
		else if (line.StartsWith("if") ||
		    line.StartsWith("while") ||
		    line.StartsWith("for") ||
		    line.StartsWith("switch") ||
		    line.StartsWith("else if"))
		{
			complexity ++;
			if (complexity > maxComplexity)
				maxComplexity = complexity;
		}
		else if (line.EndsWith(";"))
		{
			logicalLinesOfCode ++;
		}
		else if (isFunction(line))
		{
			numberOfFunctions++;
			complexity = 1;
		}
	}

	private void parsePackage (string line)
	{
		if (packageName == "") 
		{
			int s = line.IndexOf (' ');
			line = line.Substring (s + 1);
			char[] charsToTrim = {' ', '{', '\t'};
			line = line.Trim(charsToTrim);
			if (line.Length == 0)
				return;
			packageName = line;
			string fullname = (packageName + "." + name).ToLower();
			if (allDependencies.ContainsKey (fullname))
				numberOfDependants = allDependencies [fullname];
			PackageComponent package = PackageComponent.getRoot ().getSubPackage (packageName);
			package.addComponent (this);
			transform.parent = package.transform;
		}
	}

	private void parseImport (string line)
	{
		int s = line.IndexOf (' ');
		string importName = line.Substring (s + 1);
		bool isSystem = (importName.StartsWith ("System") || importName.StartsWith ("java") || importName.StartsWith ("flash") || importName.StartsWith ("mx"));
		if (isSystem == false) {
			char[] charsToTrim = {' ', ';', '\t'};
			importName = importName.Trim(charsToTrim).ToLower();
			numberOfDependencies++;
			//TODO C# using statement do not include class names
			// keep track of packages imported
			// when we encounter public xxx statements
			// check if the name after public/private etc. is a class
			// in imported packages
			if (allClasses.ContainsKey (importName))
			{
				allClasses [importName].addDependant ();
			}
			else
			{
				if (allDependencies.ContainsKey (importName) == false)
					allDependencies.Add (importName, 0);
				allDependencies [importName]++;
			}
		}
	}

	private void addDependant()
	{
		numberOfDependants++;
	}

	private bool isFunction(string line)
	{
		string[] split = line.Split(new char[]{' '});
		int tokens=0;
		int parantheses=0;
		for (int i=0; i<split.Length; i++)
		{
			if (split[i].StartsWith("function"))
				return true;
			else if (split[i].StartsWith("void"))
				return true;
			else if (split[i].StartsWith("catch"))
				return false;
			else if (split[i].StartsWith("=") && parantheses==0)
				return false;
			else if (split[i].StartsWith("&&"))
				return false;
			else if (split[i].StartsWith("||"))
				return false;
			else if (split[i].StartsWith("("))
				parantheses++;
			else if (split[i].Contains("("))
			{
				parantheses++;
				tokens++;
			}
			else if (split[i].Length > 1 && parantheses==0)
				tokens ++;
		}

		//functions must have at least return type and function name
		return (tokens >= 2 && parantheses == 1);
	}

	public void createVisuals()
	{
		//height relative to number of functions
		float height = numberOfFunctions * HEIGHT_PER_FUNCTION;
		if (height < HEIGHT_MIN)
		{
			height = HEIGHT_MIN;
		}
		if (height > HEIGHT_MAX)
		{
			height = HEIGHT_MAX;
			offScale++;
		}
		//volume relative to lloc
		float volume = logicalLinesOfCode * VOL_PER_LOC;
		size = volume / height;
		if (size < SIZE_MIN)
		{
			size = SIZE_MIN;
		}
		if (size > SIZE_MAX)
		{
			size = SIZE_MAX;
			offScale++;
		}
		transform.localScale = new Vector3(size,height,size);
		transform.position = new Vector3(0, height*0.5f, 0);
		setColours();
	}

	private void setColours()
	{
		float r = 0;
		float g = 0;
		float b = 0;
		if (numberOfDependencies > 0)
			r = 0.1f + numberOfDependencies * R_PER_DEP;
		if (r>1)
		{
			r=1;
			offScale++;
		}
		if (numberOfDependants > 0)
			g = 0.1f + numberOfDependants * G_PER_DEP;
		if (g > 1)
		{
			g=1;
			offScale++;
		}
		b = maxComplexity * B_PER_COMP;
		if (b>1)
		{
			b=1;
			offScale++;
		}
		gameObject.GetComponent<Renderer>().material.color = new Color(r,g,b);
		//if two are off chart, add smoke
		//if three are off chart, add fire
	}
}
}