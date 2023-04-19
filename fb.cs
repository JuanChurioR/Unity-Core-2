using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

using Firebase;
using Firebase.Database;
using Firebase.Storage;
using Newtonsoft.Json;

public class fb : MonoBehaviour
{
    private const string storageUrl = "https://firebasestorage.googleapis.com/v0/b/sokito-8cf30.appspot.com/o/Niveles%2Fsokito.txt?alt=media";
    public GameObject gameObjectToUpload;

    void Start() //el objetivo es subir gameObjectToUpload al bucket de Firebase
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;

        
        StorageReference storageRef = storage.RootReference.Child("Niveles/sokito.prefab"); //dirección a la que iria

        
        string prefabJson = JsonConvert.SerializeObject(gameObjectToUpload); //conversion a Json

        
        byte[] prefabBytes = System.Text.Encoding.UTF8.GetBytes(prefabJson); //codificación

        
        storageRef.PutBytesAsync(prefabBytes).ContinueWith(task => { //subir bytes codificados
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to upload prefab: " + task.Exception);
            }
            else
            {
                Debug.Log("Prefab uploaded successfully!");
            }
        });
        StartCoroutine(Courutine()); //llamar a leer txt

    }

    IEnumerator Courutine() //leer txt en Firebase
    {
        using (UnityWebRequest www = UnityWebRequest.Get(storageUrl))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("UnityWebRequest failed: " + www.error);
            }
            else
            {
                Debug.Log("File content: " + www.downloadHandler.text);
            }
        }
    }
}
