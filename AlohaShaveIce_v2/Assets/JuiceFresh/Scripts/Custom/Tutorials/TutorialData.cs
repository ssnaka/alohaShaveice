using System.Collections;
using System.Collections.Generic;

public class TutorialData 
{
	public List<ATutorial> tutorials {get; set;}
}

public class ATutorial
{
	public TutorialType type {get; set;}
	public string prefabName {get; set;}
	public string descriptionTextAsset{get; set;}
}
