using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Core;
using Improbable.Player;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

[WorkerType(WorkerPlatform.UnityWorker)]
public class TransformSenderWorker : MonoBehaviour
{
	[Require] private Position.Writer positionComponent;
	[Require] private Rotation.Writer rotationComponent;

	[Require] private PlayerTransform.Reader playerReader;

	private int fixedFramesSinceLastUpdate = 0;


	private void OnEnable()
	{
		transform.position = positionComponent.Data.coords.ToVector3();
		
	}

	private void FixedUpdate()
	{
		transform.position = playerReader.Data.position.ToUnityVector();
		transform.rotation = playerReader.Data.rotation.ToUnityQuaternion();

		var newPosition = transform.position.ToCoordinates();
		var newRotation = transform.rotation;

		fixedFramesSinceLastUpdate++;
		if ((PositionNeedsUpdate(newPosition)) && fixedFramesSinceLastUpdate > 5)
		{
			fixedFramesSinceLastUpdate = 0;
			positionComponent.Send(new Position.Update().SetCoords(newPosition));
			rotationComponent.Send(new Rotation.Update().SetRotation(newRotation.ToNativeQuaternion()));
		}
	}

	private bool PositionNeedsUpdate(Coordinates newPosition)
	{
		return !MathUtils.CompareEqualityEpsilon(newPosition, positionComponent.Data.coords);
	}

}
