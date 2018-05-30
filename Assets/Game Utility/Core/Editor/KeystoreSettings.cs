using UnityEditor;
using UnityEditor.Build;

public class KeystoreSettings : IPreprocessBuild
{
    string alias = "battle cars";
    string password = "springboard";

    public int callbackOrder
    {
        get
        {
            return 0;
        }
    }

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        if (target == BuildTarget.Android)
        {
            PlayerSettings.Android.keystorePass = password;
            PlayerSettings.Android.keyaliasName = alias;
            PlayerSettings.Android.keyaliasPass = password;
        }
    }
}
