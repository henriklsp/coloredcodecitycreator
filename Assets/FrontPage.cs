using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;

namespace UI 
{
public class FrontPage: MonoBehaviour
{
    public InputField input;
    public Text output;

    public void go ()
    {
        DirectoryInfo root = new DirectoryInfo(input.text);
        string error = getError(root);
        if (error == null)
        {
            output.text = "Processing...";
            StartCoroutine(process(root));
        }
        else
        {
            output.text = error;
        }
    }

	internal void Awake ()
	{
        input.text = System.IO.Directory.GetCurrentDirectory();
	}

    private string getError (DirectoryInfo root)
    {
        System.IO.FileInfo[] files = null;
        System.IO.DirectoryInfo[] subDirs = null;

        try
        {
            subDirs = root.GetDirectories();
            files = root.GetFiles("*.*");
        }
        catch (Exception e)
        {
            return "Cannot access " + root.FullName + ":  " + e.Message;
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
                    return null;
                }
            }
        }
        string subError = null;
        if (subDirs != null)
        {
            foreach (System.IO.DirectoryInfo dirInfo in subDirs)
            {
                string e = getError(dirInfo);
                if (e == null)
                    return null;
                else if (e.Equals("No source code found") == false)
                    subError = e;
            }
        }
        if (subError != null)
            return subError;
        return "No source code found";
    }

    IEnumerator process (DirectoryInfo root)
    {
        yield return null;
        Creator.create(root);
        gameObject.SetActive(false);
    }
}
}