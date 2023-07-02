using FMODUnity;
using UnityEngine;

public class ExamplePlayOneShotDemo02 : MonoBehaviour
{
    public FMODAudioSource AudioSource;
    public string ParameterName;
    public EventReference Clip;

    [Range(0, 2)] public int Value;

    public void TestPlay()
    {
        AudioSource.PlayOneShot(Clip, ParameterName, Value);
    }

    public void ChangeValue(float value)
    {
        Value = (int)value;
    }
}