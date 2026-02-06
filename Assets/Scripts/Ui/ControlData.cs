using UnityEngine;


[CreateAssetMenu(fileName = "ControlData", menuName = "Data/PlayerControl")]
public class ControlData : ScriptableObject
{
    public KeyCode moveUpKey = KeyCode.W;
    public KeyCode moveDownKey = KeyCode.S;
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;
    public KeyCode flashlightKey = KeyCode.F;
    public KeyCode runKey = KeyCode.LeftShift;

    public float mouseSensitivity = 1.0f;
}
