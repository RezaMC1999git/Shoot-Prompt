using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Rebellis : MonoBehaviour
{
    private string userName = "MythicVoyager"
        , password = "Wfr8mL4ASthbJaXy7QZNRH",
        baseURL = "https://auth.rebellis.ai",
        apiURL = "https://api.rebellis.ai",
        blenderURL = "https://blender.rebellis.ai",
        token, npyName, UUID4 = "4a2ecbd9-21f6-4195-a6b2-58e686f84985",
        savePath = "Assets/animation.fbx";

    public NPC npc;
    public Animator animator;
    public RuntimeAnimatorController animatorController;
    public AnimationClip animationClip;
    public Transform NpcTransform;
    public PromptManager promptManager;


    private void Start()
    {
        GameObject fbxObject = Resources.Load<GameObject>("animation");
        GameObject newNpc = null;
        if (fbxObject != null) 
        {
            newNpc = Instantiate(fbxObject, NpcTransform);
            newNpc.name = "newNPC";
            Vector3 size = new Vector3(0.3f, 0.3f, 0.3f);
            newNpc.transform.localScale = size;
            CapsuleCollider cc = newNpc.AddComponent<CapsuleCollider>();
            Rigidbody rb = newNpc.AddComponent<Rigidbody>();
            cc.radius = 5f;
            Vector3 newCenter = new Vector3(cc.center.x, 5f, cc.center.z);
            cc.center = newCenter;
            cc.isTrigger = false;
        }
        GenerateUUID4();
        StartCoroutine(GetToken());
    }

    IEnumerator GetToken()
    {
        var json = new JObject
        {
            { "username", userName },
            { "password", password }
        };
        var bytes = Encoding.UTF8.GetBytes(json.ToString());
        using (var webRequest = new UnityWebRequest(baseURL + "/token", "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bytes);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();
            TokenClass tokenClass = JsonConvert.DeserializeObject<TokenClass>(webRequest.downloadHandler.text);
            Debug.LogError($"GetToken Responce:{webRequest.downloadHandler.text}");
            token = tokenClass.access_token;
            StartCoroutine(CreateTransaction());
        }
    }

    IEnumerator CreateTransaction()
    {
        var json = new JObject
        {
            { "transaction_uuid", UUID4 },
            { "repeat", "1" }
        };
        var bytes = Encoding.UTF8.GetBytes(json.ToString());
        using (var webRequest = new UnityWebRequest(baseURL + "/trans", "POST"))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            webRequest.uploadHandler = new UploadHandlerRaw(bytes);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();
            //StartCoroutine(GenerateModel());
            Debug.LogError($"CreateTransaction Responce:{webRequest.downloadHandler.text}");
        }
    }

    public IEnumerator GenerateModel(string prompt)
    {
        var json = new JObject
        {
            { "prompt", prompt},
            { "repeat", 1 },
            { "transaction_id", UUID4 }
        };
        var bytes = Encoding.UTF8.GetBytes(json.ToString());
        using (var webRequest = new UnityWebRequest(apiURL + "/v1/generate", "POST"))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            webRequest.uploadHandler = new UploadHandlerRaw(bytes);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();
            Debug.LogError($"GenerateModel Responce:{webRequest.downloadHandler.text}");
            NPYClass npyClass = JsonConvert.DeserializeObject<NPYClass>(webRequest.downloadHandler.text);
            npyName = npyClass.npy[0];
            Debug.LogError(npyName);
            StartCoroutine(RetrievingResults());
            StartCoroutine(GenerateFBX());
        }
    }

    IEnumerator GenerateFBX()
    {
        var json = new JObject
        {
            { "transaction_id", UUID4 },
            { "character", "Rebellis" },
            { "solo_npy", UUID4 + "_0.npy" }
        };
        var bytes = Encoding.UTF8.GetBytes(json.ToString());
        using (var webRequest = new UnityWebRequest(blenderURL + "/v1/generate", "POST"))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            webRequest.uploadHandler = new UploadHandlerRaw(bytes);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();
            Debug.LogError($"GenerateFBX Responce:{webRequest.downloadHandler.text}");
            FileEntryList fileEntryList = JsonConvert.DeserializeObject<FileEntryList>("{\"entries\":" + webRequest.downloadHandler.text + "}");
            StartCoroutine(DownloadFBXFile(fileEntryList.entries[0].fbx));
            GenerateUUID4();
            promptManager.sendPromptButton.interactable = true;
            promptManager.generatingText.gameObject.SetActive(false);
        }
    }

    IEnumerator DownloadFBXFile(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            byte[] fbxData = www.downloadHandler.data;
            //SaveData(savePath, fbxData);
            File.WriteAllBytes(savePath, fbxData);
            yield return new WaitForSeconds(10f);
            //#if UNITY_EDITOR
            //            AssetDatabase.Refresh();
            //#endif            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            //Debug.Log("FBX file downloaded and saved at " + savePath);
            //byte[] fileData = LoadFileToByteArray(savePath);
            //GameObject fbxObject = null;
            //if (fileData != null)
            //{
            //    fbxObject = LoadModelFromBytes(fileData);
            //    if (fbxObject != null)
            //    {
            //        fbxObject.transform.position = Vector3.zero; // Position the object as needed
            //    }
            //}
            //GameObject fbxObject = Resources.Load<GameObject>("animation");
            ////GameObject fbxObject = LoadData<GameObject>(savePath);
            ////NPC newNpc = Instantiate(npc, NpcTransform);
            //GameObject newNpc = Instantiate(fbxObject, NpcTransform);
            //newNpc.name = "newNPC";
            //Vector3 size = new Vector3(0.3f, 0.3f, 0.3f);
            //newNpc.transform.localScale = size;
            //CapsuleCollider cc = newNpc.AddComponent<CapsuleCollider>();
            //Rigidbody rb = newNpc.AddComponent<Rigidbody>();
            //cc.radius = 5f;
            //Vector3 newCenter = new Vector3(cc.center.x, 5f, cc.center.z);
            //cc.center = newCenter;
            //cc.isTrigger = false;
            //newNpc.animator.runtimeAnimatorController = animatorController;

            // Create a new state in the Animator Controller with the animation clip
            //AnimatorOverrideController overrideController = new AnimatorOverrideController(newNpc.animator.runtimeAnimatorController);
            //overrideController["Default State"] = animationClip; // "Default State" should be replaced with the actual state name
            //animator.runtimeAnimatorController = overrideController;

            // Optionally, start playing the animation
            //newNpc.animator.Play("Default State");
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    public static void SaveData<T>(string myPath, T data)
    {
        string path = Application.persistentDataPath + myPath;
        bool fileExist = false;

        try
        {
            string directoryPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }

            fileExist = true;

        }
        catch (Exception error)
        {
            Debug.LogError("could not create file Due To: " + error.Message + "\nAnd with stack trace: " + error.StackTrace);
            throw error;
        }

        if (fileExist)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(data);
                File.WriteAllText(path, jsonData);
            }
            catch (Exception error)
            {
                Debug.LogError("could not write due to: " + error.Message + "\nAnd with stack trace: " + error.StackTrace);
                throw error;
            }
        }
    }

    private byte[] LoadFileToByteArray(string filePath)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, filePath);

        if (File.Exists(fullPath))
        {
            try
            {
                return File.ReadAllBytes(fullPath);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error reading file: " + ex.Message);
                return null;
            }
        }
        else
        {
            Debug.LogError("File not found: " + fullPath);
            return null;
        }
    }

    private GameObject LoadModelFromBytes(byte[] fileData)
    {
        try
        {
            using (MemoryStream stream = new MemoryStream(fileData))
            {
                //return Importer.LoadFromFile(stream);
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading model: " + ex.Message);
            return null;
        }
    }

    public static T LoadData<T>(string myPath)
    {
        string path = Application.persistentDataPath + myPath;

        if (!File.Exists(path))
        {
            return default(T);
        }
        try
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));

        }
        catch (Exception error)
        {
            Debug.LogError("could not read due to: " + error.Message + "\nAnd with stack trace: " + error.StackTrace);
            throw error;
        }
    }

    IEnumerator RetrievingResults()
    {
        using (var webRequest = new UnityWebRequest(baseURL + "/trans/verify?uuid=" + UUID4, "GET"))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();
            Debug.LogError($"RetrievingResults Responce:{webRequest.downloadHandler.text}");
            yield return new WaitForSeconds(1f);
            //StartCoroutine(RetrievingResults());
        }
    }


    void GenerateUUID4()
    {
        Guid uuid = Guid.NewGuid();
        UUID4 = uuid.ToString();
    }

    IEnumerator RecieveCredit()
    {
        using (var webRequest = new UnityWebRequest(baseURL + "/user", "GET"))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            yield return webRequest.SendWebRequest();
            Debug.LogError($"RecieveCredit Responce:{webRequest.downloadHandler.text}");
        }
    }
}

public class TokenClass
{
    public string access_token { set; get; }
}

public class NPYClass
{
    public string[] npy { set; get; }
}

public class FileEntryList
{
    public List<FbxClass> entries;
}

public class FbxClass
{
    public string fbx { set; get; }
}

