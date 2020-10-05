using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject infoScreen;
    public GameObject chooseLevelScreen;
    string levelToLoad;
    public Transform[] levelsObjects;

    // Start is called before the first frame update
    void Start()
    {
        int levelsResolved = 0;
        for (int i = 0; i < levelsObjects.Length; i++)
        {
            if(!PlayerPrefs.HasKey(levelsObjects[i].name + "Resolved")){
                levelsObjects[i].Find("Button").gameObject.SetActive(false);
                levelsObjects[i].Find("Status").gameObject.SetActive(false);
            }
            else
            {
                levelsResolved ++;
            }
        }

        levelsObjects[levelsResolved].Find("Button").gameObject.SetActive(true);
    }

    public void StartLevel(string levelName){
        if(!PlayerPrefs.HasKey("username") || !PlayerPrefs.HasKey("userage")){
            levelToLoad = levelName;
            infoScreen.SetActive(true);
            chooseLevelScreen.SetActive(false);
            return;
        }
        SceneManager.LoadScene(levelName);
    }

    public void SaveUser(){

        string username = infoScreen.transform.Find("NameInputField").GetComponent<InputField>().text;
        int userage = 0;
        int.TryParse(infoScreen.transform.Find("AgeInputField").GetComponent<InputField>().text, out userage);
        if(userage > 0 && !string.IsNullOrEmpty(username))
        {
            PlayerPrefs.SetString("username",username);
            PlayerPrefs.SetInt("userage",userage);

            PlayerPrefs.Save();

            StartLevel(levelToLoad);
        }
        else
            Debug.Log("Sem nome ou idade");        
    }
}
