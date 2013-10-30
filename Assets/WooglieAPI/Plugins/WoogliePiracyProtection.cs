using UnityEngine;
using System.Collections;

using System.Text.RegularExpressions;

public class WoogliePiracyProtection : MonoBehaviour
{

    void Awake()
    {
        TestPiracy();

        Debug.Log("Wooglie");
        Application.ExternalEval("HelloWooglie();");
    }

    void Start()
    {
        StartCoroutine(MustHaveValidated());
    }
    void OnLevelWasLoaded(int level)
    {
        StartCoroutine(MustHaveValidated());
    }

    IEnumerator MustHaveValidated()
    {
        yield return new WaitForSeconds(5);
        if (!validated)
        {
            Application.OpenURL("http://www.wooglie.com/");
        }
    }

    static bool validated = false;
    public void WoogliePong(string reply)
    {
        validated = true;
    }

    /// <summary>
    /// Check if the game is running on Wooglie servers
    /// </summary>
    public static void TestPiracy()
    {
        if (!Application.isWebPlayer)
        {
            return;
        }

        //First test: Unity values    
        if (!ValidWooglieURL(Application.absoluteURL))
        {
            Application.OpenURL("http://www.wooglie.com/?InvalidURL=" + Application.absoluteURL + "invalidType=abs");
            return;
        }

        if (!ValidWooglieURLSRC(Application.srcValue))
        {
            Application.OpenURL("http://www.wooglie.com/?InvalidURL=" + Application.srcValue + "invalidType=src");
            return;
        }

        string script = "var ind =  document.location.host.indexOf(\"wooglie.com\"); if(ind==-1){ document.location='http://www.wooglie.com/?InvalidURL='+document.location+'invalidType=js'; }" +
            "var desiredIndex = document.location.host.length - 11;\n" +
            "if(ind==-1 || ind!=desiredIndex ){ document.location='http://www.wooglie.com/?InvalidURL='+document.location+'invalidType=js'; } ";

        //Second test: Run some javascript to double check the URL; If it's not wooglie, visit wooglie!
        Application.ExternalEval(script);

        //if(   document.location.host != 'www.wooglie.com' && document.location.host != 'contentmirror.wooglie.com' && document.location.host != 'http://www.wooglie.com' && document.location.host != 'wooglie.com' && document.location.host != 'localhost'){ document.location='http://www.wooglie.com/?invalidUrl2'; }");


    }

    static bool ValidWooglieURL(string URL)
    {
        URL = URL.ToLower();
        Regex objNotNaturalPattern = new Regex("http://[a-z]*.wooglie.com");
        Match ma = objNotNaturalPattern.Match(URL);
        if (ma.Success)
        {
            if (ma.Index == 0)
            {
                int woogIn = URL.IndexOf("wooglie.com");
                int slashPos = woogIn + 11;
                if (slashPos >= URL.Length || URL[slashPos] == '/')
                    return true;
            }
        }
        if (URL.IndexOf("http://wooglie.com/") == 0) return true;
        return false;
    }

    static bool ValidWooglieURLSRC(string URL)
    {
        if (ValidWooglieURL(URL))
            return true;

        //Is this just a relative path?
        if (URL.Contains("http://"))
            return false;
        if (URL.Contains("https://"))
            return false;
        if (URL.Contains("ftp://"))
            return false;
        if (URL.Contains("www."))
            return false;

        return true;
    }

}
