using UnityEngine;
using System;
using System.IO;
using System.Collections;
using Components;

public class Creator : MonoBehaviour 
{
	public ClassComponent classComponent;
	public PackageComponent packageComponent;
	public static bool isComplete = false;
    private static Creator singleton;

    public static void create (DirectoryInfo root)
    {
        singleton.createCity(root);
    }

	internal void Awake()
	{
		PackageComponent.prefab = packageComponent;
        singleton = this;
	}

    internal void createCity (DirectoryInfo root) 
	{
//		//set root folder
//		string rootString = System.IO.Directory.GetCurrentDirectory();
//		if (Application.isEditor)
//			rootString = "D:/Projekt/RTS/Assets/Scripts";
//		DirectoryInfo root = new DirectoryInfo(rootString);
		try {
			WalkDirectoryTree(root);
			OrganizeStreets();
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message + " "+e.StackTrace);
		}
		isComplete = true;
	}
	
    private void WalkDirectoryTree(DirectoryInfo root)
    {
		Debug.Log("Read directory "+root.Name);
        System.IO.FileInfo[] files = null;
        System.IO.DirectoryInfo[] subDirs = null;

        try
        {
			subDirs = root.GetDirectories();
            files = root.GetFiles("*.*");
        }
        // This is thrown if even one of the files requires permissions greater 
        // than the application provides. 
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        if (files != null)
        {
            foreach (System.IO.FileInfo file in files)
            {
				if (file.Extension.Equals(".cs") ||
				    file.Extension.Equals(".as") ||
				    file.Extension.Equals(".mxml") ||
				    file.Extension.Equals(".java"))
				{
					ParseFile(file);
				}
            }
		}
		if (subDirs != null)
		{
            foreach (System.IO.DirectoryInfo dirInfo in subDirs)
            {
                WalkDirectoryTree(dirInfo);
            }
        }            
    }

	private void ParseFile(FileInfo file)
	{
		string fileName = file.Name;
		int s = fileName.IndexOf('.');
		fileName = fileName.Substring(0,s);
		ClassComponent c = UnityEngine.Object.Instantiate(classComponent) as ClassComponent;
		c.setName(fileName);
		c.readFromFile(file);
	}

	private void OrganizeStreets()
	{
		PackageComponent.getRoot().organize();
	}
}
