using UnityEngine;

public static class ExtensionsWheelCollider
{
	public static void Reset(this WheelCollider wheel)
	{
		wheel.motorTorque = 0;
		wheel.steerAngle = 0;
		wheel.brakeTorque = 0;
	}
}
