using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{
    public static GameController gameController;

    private string APIUrl = "http://tcc-alana-everton.us-south.cf.appdomain.cloud/solution";
    public List<Trash> trashList;
    public List<GameObject> screenList;
    CharacterController character;

    private string solutionID;

    void Start () {
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        GameController.gameController = this;
    }

    public void ColectTrash(Trash trash){
        Trash temp = trashList.Find(t => t.GetHashCode() == trash.GetHashCode());
        temp.collected = true;
    }

    public void DiscardTrash(string trashType){
        List<Trash> tempList = trashList.FindAll(t => t.trashType == trashType);
        if(tempList.Count > 0) {
            foreach (var t in tempList)
            {
                t.discarted = t.discartedCorrectly = true;
            }
        }
        else {
            foreach (var t in trashList)
            {
                t.discarted = true;
            }
        }
    }

    public void TryAgain(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadScene(string LevelName){
        SceneManager.LoadScene(LevelName);
    }
    async public void SendSolutionToServer(byte[] image){
        
        string uploadResult = null;
        
        try{
            uploadResult = await UploadImage(image);
            screenList.Find(s => s.name == "Loading Screen").SetActive(false);
            await Task.Delay(TimeSpan.FromSeconds(1));
            await ExecuteSolution(ConvertSolution(uploadResult));

            // Verifica se todos os lixos foram coletados
            int trashCount = trashList.FindAll(t => !t.collected).Count;
            if(trashCount > 0){
                screenList.Find(s => s.name == "Failure Screen").SetActive(true);
                Debug.Log(trashCount + "lixo(s) não foram coletados");
                return;
            }
            
            // Verifica se todos os lixos foram descartados
            trashCount = trashList.FindAll(t => !t.discarted).Count;
            if(trashCount > 0){
                screenList.Find(s => s.name == "Failure Screen").SetActive(true);
                Debug.Log(trashCount + " lixo(s) não foram descartados");
                return;
            }

            // Verifica se todos os lixos foram descartados corretamente
            trashCount = trashList.FindAll(t => !t.discartedCorrectly).Count;
            if(trashCount > 0){
                screenList.Find(s => s.name == "Failure Screen").SetActive(true);
                Debug.Log(trashCount + " lixo(s) não foram descartados corretamente");
                return;
            }

            await UpdateCorrectSolution(solutionID);
            solutionID = "";
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name+"Resolved",1);
            screenList.Find(s => s.name == "Success Screen").SetActive(true);
            Debug.Log("Fase Completada com sucesso!");

        }
        catch(Exception e){
            var compliationErrorScreen = screenList.Find(s => s.name == "Compilation Error Screen");
            var errorObj = compliationErrorScreen.transform.Find("Modal/Specification Error Text");
            errorObj.gameObject.GetComponent<Text>().text = "Erro: " + e.Message;
            compliationErrorScreen.SetActive(true);
            Debug.LogError(e);
        }
    }

    async Task<string> UploadImage(byte[] image)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", image, "solution.png", "image/png");
        form.AddField("childInfo", "{" + $"\"name\":\"{PlayerPrefs.GetString("username")}\", \"age\":\"{PlayerPrefs.GetString("userage")}\"" + "}");
        form.AddField("deviceID", SystemInfo.deviceUniqueIdentifier);
        var postRequest = UnityWebRequest.Post(APIUrl,form);
        await postRequest.SendWebRequest();
        if(postRequest.isNetworkError || postRequest.isHttpError)
            throw new Exception("Erro ao enviar imagem");
        return postRequest.downloadHandler.text;
    }

    async Task<string> UpdateCorrectSolution(string solutionID){

        WWWForm form = new WWWForm();
        form.AddField("childInfo", "{" + $"\"name\":\"{PlayerPrefs.GetString("username")}\", \"age\":\"{PlayerPrefs.GetString("userage")}\"" + "}");
        form.AddField("solutionID",solutionID);
        form.AddField("correctSolution","true");

        var putRequest = UnityWebRequest.Put(APIUrl,form.data);
        putRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        await putRequest.SendWebRequest();
        if(putRequest.isNetworkError || putRequest.isHttpError)
            throw new Exception("Erro ao atualizar solução");
        return putRequest.downloadHandler.text;
    }

    async Task ExecuteSolution(List<Block> blocks){
        foreach (var block in blocks)
        {
            await ExecuteAction(block);
        }
    }

    async Task ExecuteAction(Block b){
        

        switch (b.name)
        {
            case "Walk": 
                character.Walk(); 
                await Task.Delay(TimeSpan.FromSeconds(1));
            break;
            
            case "Turn": 
                character.Turn90(); 
                await Task.Delay(TimeSpan.FromSeconds(1));
            break;
            
            case "Loop": 
                await ExecuteLoop((Loop)b); 
            break;

            default: 
                Debug.Log("Block without action"); 
            break;
        }
    }

    async Task ExecuteLoop(Loop p){
        for (int i = 0; i < p.repeatTimes; i++)
        {
            foreach (var block in p.blocks)
            {
                await ExecuteAction(block);
            }
        }
    }

    List<Block> ConvertSolution(string solution){
        try
        {
            APIResult APIResult = JsonUtility.FromJson<APIResult>(solution);

            int qtdLoopBlocks = APIResult.blocks.FindAll(x => x.name == "Loop").Count;
            int qtdNumberBlocks = APIResult.blocks.FindAll(x => x.name.StartsWith("Number")).Count;

            //Verifica se todos os loops tem dois blocos (inicio e fim)
            if( qtdLoopBlocks % 2 != 0){
                throw new Exception("Quantidade de loops inválida");
            }
            // Verifica se existe loops sem blocos numéricos
            else if( qtdLoopBlocks / 2  > qtdNumberBlocks ){
                throw new Exception("Bloco numérico faltando");
            }
            // Verifica se existe mais blocos númericos do que pares de loop
            else if( qtdLoopBlocks / 2  < qtdNumberBlocks ){
                throw new Exception("Bloco numérico em excesso");
            }

            List<Block> blocks = new List<Block>();

            bool isLoopBlock = false;
            Loop loopBlock = null;
            for (int i = 0; i < APIResult.blocks.Count; i++)
            {
                if(APIResult.blocks[i].name == "Loop") { 
                    if(loopBlock == null) {
                        loopBlock = new Loop();
                        loopBlock.blocks = new List<Block>();
                        isLoopBlock = true; 
                    }
                    else{
                        blocks.Add(loopBlock);
                        isLoopBlock = false; 
                    }
                }
                else if(APIResult.blocks[i].name.StartsWith("Number")) {  
                    string number = APIResult.blocks[i].name.Replace("Number","");
                    int repeatTimes = int.Parse(number);
                    loopBlock.repeatTimes = repeatTimes;
                }
                else if(isLoopBlock){
                    loopBlock.blocks.Add(APIResult.blocks[i]);
                }
                else {
                    blocks.Add(APIResult.blocks[i]);
                }
            }
            solutionID = APIResult.solutionID;
            return blocks;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

}
