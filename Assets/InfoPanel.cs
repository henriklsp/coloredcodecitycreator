using UnityEngine;
using UnityEngine.UI;
using Components;

namespace UI 
{
public class InfoPanel: MonoBehaviour
{
    public GameObject panel;
    public Text nameLabel;
    public Text packageLabel;
    public Text locField;
    public Text functionsField;
    public Text complexityField;
    public Text dependenciesField;
    public Text dependantsField;

	internal void Awake ()
	{
        panel.SetActive(false);
	}

	internal void Update ()
	{
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000) && hit.transform != null)
            {
                ClassComponent component = hit.transform.GetComponent<ClassComponent>();
                if (component != null)
                    show(component);
            }
        }
	}

    private void show (ClassComponent component)
    {
        panel.SetActive(true);
        nameLabel.text = component.getName();
        packageLabel.text = "in "+component.getPackageName();
        if (component.getPackageName().Length == 0)
            packageLabel.text = "in root";
        locField.text = "" + component.getLinesOfCode();
        complexityField.text = "" + component.getComplexity();
        functionsField.text = "" + component.getNumberOfFunctions();
        dependenciesField.text = "" + component.getDependencies();
        dependantsField.text = "" + component.getDependants();
    }
}
}