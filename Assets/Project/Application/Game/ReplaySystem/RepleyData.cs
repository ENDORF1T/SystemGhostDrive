using UnityEngine;

public class RepleyData
{
    public Vector3 Position { get; private set; }

    public Quaternion Rotation { get; private set; }

    public Vector2 MovementInput { get; private set; }

    public float IsBrakeInput { get; private set; }

    public RepleyData(Vector3 position, Quaternion rotation, Vector2 movementInput, float isBrakeInput)
    {
        Position = position;
        Rotation = rotation;
        MovementInput = movementInput; 
        IsBrakeInput = isBrakeInput;
    }
}
