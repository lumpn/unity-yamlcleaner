using UnityEngine;
using UnityEngine.Events;

public class DemoScript : MonoBehaviour
{
    [SerializeField] private float value;
    [SerializeField] private UnityEvent onValueChanged;

    public string text;

    [field:SerializeField] public bool isVisible { get; set; }
}
