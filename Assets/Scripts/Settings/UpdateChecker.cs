using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace Gameplay
{
    public class UpdateChecker : MonoBehaviour
    {
        public GameObject obj;
        public ApiCall log;
        public UpdateLogHolder newLog;
        private string currentURL;
        private string changelogURL;
        void Start()
        {
            // A correct website page.
            StartCoroutine(GetRequest("https://api.github.com/repos/SpinningSpectre/GameVersionHolder/contents/Permafrost.txt?ref=main"));

            // A non-existing page.
            StartCoroutine(GetRequest("https://error.html"));
        }

        IEnumerator GetRequest(string uri)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        JsonUtility.FromJsonOverwrite(webRequest.downloadHandler.text,log);
                        byte[] decodedBytes = Convert.FromBase64String(log.content);
                        string decodedText = Encoding.UTF8.GetString(decodedBytes);
                        JsonUtility.FromJsonOverwrite(decodedText, newLog);
                        if (versionCompare(newLog.version,Application.version) <= 0) { print("You on newer man");  yield break; }
                        print("Version out of date! Your version: " + Application.version + ", New version: " + newLog.version);
                        currentURL = newLog.url;
                        changelogURL = newLog.changelogurl;
                        GiveUpdatePopup();
                        break;
                }
            }
        }

        public void GiveUpdatePopup()
        {
            obj.SetActive(true);
            EventSystem.current.SetSelectedGameObject(obj);
            Transform parent = obj.transform.Find("RawImage");
            print(newLog.maintext);
            parent.Find("MainText").GetComponent<TMP_Text>().text = newLog.maintext;
            parent.Find("SecondaryText").GetComponent<TMP_Text>().text = newLog.secondarytext;
            parent.Find("Version").GetComponent<TMP_Text>().text = newLog.version;
        }

        public void OpenLink()
        {
            Application.OpenURL(currentURL);
        }
        public void OpenChangelog()
        {
            Application.OpenURL(changelogURL);
        }

        //not copied from online
        static int versionCompare(string v1, string v2)
        {
            // vnum stores each numeric 
            // part of version 
            int vnum1 = 0, vnum2 = 0;

            // loop until both string are 
            // processed 
            for (int i = 0, j = 0; (i < v1.Length || j < v2.Length);)
            {

                // storing numeric part of 
                // version 1 in vnum1 
                while (i < v1.Length && v1[i] != '.')
                {
                    vnum1 = vnum1 * 10 + (v1[i] - '0');
                    i++;
                }

                // storing numeric part of 
                // version 2 in vnum2 
                while (j < v2.Length && v2[j] != '.')
                {
                    vnum2 = vnum2 * 10 + (v2[j] - '0');
                    j++;
                }

                if (vnum1 > vnum2)
                    return 1;
                if (vnum2 > vnum1)
                    return -1;

                // if equal, reset variables and 
                // go for next numeric part 
                vnum1 = vnum2 = 0;
                i++;
                j++;
            }
            return 0;
        }
    }


    [System.Serializable]
    public class UpdateLogHolder
    {
        public string version;
        public string maintext;
        public string secondarytext;
        public string url;
        public string changelogurl;
    }

    [System.Serializable]
    public class ApiCall
    {
        public string content;
    }
}
