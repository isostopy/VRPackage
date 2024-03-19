using UnityEngine;

// Fuerza el tick del sistema de fisicas a ser igual al frame rate del juego
// para evitar el flickering cuando se mueven los objetos con fisicas.
// Extraido de la muestra DistanceGrab del paquete Oculus Integration.
public class TickAligner : MonoBehaviour
{
	private void Start()
	{
		float freq = OVRManager.display.displayFrequency;
		if (freq > 0.1f)
		{
			Time.fixedDeltaTime = 1.0f / freq;
		}
	}
}
