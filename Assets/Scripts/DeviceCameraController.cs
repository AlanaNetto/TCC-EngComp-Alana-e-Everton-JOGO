using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class DeviceCameraController : MonoBehaviour
{
    private WebCamTexture webcamTexture;
    public RawImage cameraPreview;
    public RawImage imageCaptured;

    public GameObject LoadingScreen;

    void Start () {
        LoadingScreen.SetActive(false);
        cameraPreview.gameObject.SetActive(false);
        imageCaptured.gameObject.SetActive(false);
        webcamTexture = new WebCamTexture();
    }

    public void OpenCamera(){
        cameraPreview.gameObject.SetActive(true);
        cameraPreview.texture = webcamTexture;
        cameraPreview.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    public void CloseCamera(){
        cameraPreview.gameObject.SetActive(false);
        webcamTexture.Stop();
    }

    public void TakePicture(){
        webcamTexture.Pause();
        imageCaptured.texture = webcamTexture;
        imageCaptured.material.mainTexture = webcamTexture;
        imageCaptured.gameObject.SetActive(true);
    }

    public void Cancel(){
        imageCaptured.gameObject.SetActive(false);
        imageCaptured.texture = null;
        imageCaptured.material.mainTexture = null;
        webcamTexture.Play();
    }

    public void SendSolution(){
        //Captura a imagem da textura e converte para um objeto possivel de gerar o PNG
        Texture2D tex = new Texture2D(webcamTexture.width, webcamTexture.height);
        tex.SetPixels(webcamTexture.GetPixels());
        tex.Apply();

        //Desabilita a interface da Camera
        imageCaptured.gameObject.SetActive(false);
        imageCaptured.texture = null;
        imageCaptured.material.mainTexture = null;
        cameraPreview.gameObject.SetActive(false);
        webcamTexture.Stop();

        //Ativa a tela de Loading
        LoadingScreen.SetActive(true);

        File.WriteAllBytes(Application.persistentDataPath + "/image.png", tex.EncodeToPNG());
        

        //Gera o PNG da imagem capturada e envia para o servidor
        GameController.gameController.SendSolutionToServer(tex.EncodeToPNG());
    }
}


