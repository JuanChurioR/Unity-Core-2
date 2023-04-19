using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


public class Saving : MonoBehaviour
{
    // Creates a new menu item 'Examples > Create Prefab' in the main menu.
    GameObject nivel;
    public string localPath;
    public string npr;
    private const string BucketName = "sokoban-3d";
    private const string AuthToken = "264420253787-7o8np4te8dinjuj449fsk6qoosuk751f.apps.googleusercontent.com";


    public void Update()
    {
        if (Input.GetKeyDown("z"))
        {
            save();

        }

        if (Input.GetKeyDown("x"))
        {
            up();

        }

    }

    public void save()
    {
        nivel = GameObject.FindGameObjectWithTag("placeHolder");
        GameObject[] calderos = GameObject.FindGameObjectsWithTag("box");
        GameObject[] puntos = GameObject.FindGameObjectsWithTag("finish_point");
        GameObject[] escenario = GameObject.FindGameObjectsWithTag("scenery");
        if (calderos.Length > 0)
        {
            looping(calderos);
        }
        if (puntos.Length > 0)
        {
            looping(puntos);
        }
        if (escenario.Length > 0)
        {
            looping(escenario);
        }
    }

    private void looping(GameObject[] ObjectArray)
    {
        foreach (GameObject gameObject in ObjectArray)
        {
            gameObject.transform.parent = nivel.transform;

        }

        if (!Directory.Exists("Assets/Niveles/"))
            AssetDatabase.CreateFolder("Assets", "Niveles");

        localPath = "Assets/Niveles/" + "nivel" + ".prefab";


        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);


        bool prefabSuccess;

        PrefabUtility.SaveAsPrefabAssetAndConnect(nivel, localPath, InteractionMode.UserAction, out prefabSuccess);

        if (prefabSuccess == true)
            Debug.Log("Prefab was saved successfully");
        else
            Debug.Log("Prefab failed to save" + prefabSuccess);
    }

    public void up()
    {

        localPath = "Assets/pared_piedra.prefab";
        npr = "pared_piedra.prefab";
        //Load the prefab from local storage
        GameObject prefab = Resources.Load(localPath) as GameObject;
        //Upload the prefab to the cloud

        public static async Task<string> UploadPrefabToGoogleCloudStorage()
        {

            // Build the upload URL for your bucket
            string url = string.Format("https://www.googleapis.com/upload/storage/v1/b/{0}/o?uploadType=media&name={1}", BucketName, npr);

            // Create the web request and set the necessary headers
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Headers.Add("Authorization", "Bearer " + AuthToken);
            request.ContentType = "application/octet-stream";

            // Read the prefab from the local file path
            byte[] fileBytes = File.ReadAllBytes(localPath);

            // Set the content length of the request to match the prefab size
            request.ContentLength = fileBytes.Length;

            // Write the prefab bytes to the request stream
            using (Stream requestStream = request.GetRequestStream())
            {
                await requestStream.WriteAsync(fileBytes, 0, fileBytes.Length);
            }

            // Send the web request and get the response
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // The prefab was successfully uploaded to Google Cloud Storage
                    // Return the URL to access the prefab in the bucket
                    return string.Format("https://storage.googleapis.com/{0}/{1}", BucketName, npr);
                }
                else
                {
                    // There was an error uploading the prefab
                    // Log or handle the error appropriately
                    throw new Exception("Error uploading prefab to Google Cloud Storage");
                }
            }
        }

        Debug.Log("Prefab uploaded to cloud.");
    }



}