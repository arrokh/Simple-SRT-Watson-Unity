using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New SSRTConfig", menuName = "IBM - Watson")]
public class SSRTConfiguration : ScriptableObject 
{
    public string username;
    public string password;
    public string url;

}
