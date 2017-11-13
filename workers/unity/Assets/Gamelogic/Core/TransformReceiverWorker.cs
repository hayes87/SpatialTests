using System;
using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using UnityEngine;

[WorkerType(WorkerPlatform.UnityWorker)]
public class TransformReceiverWorker : MonoBehaviour
{
	[Require] private Position.Reader positionComponent;
	[Require] private Rotation.Reader rotationComponent;

	[SerializeField] private Rigidbody myRigidbody;

	private void Awake()
	{
		myRigidbody = GetComponent<Rigidbody>();
	}

	private void OnEnable()
	{
		positionComponent.CoordsUpdated.AddAndInvoke(OnPositionUpdated);
		rotationComponent.RotationUpdated.AddAndInvoke(OnRotationUpdated);
	}

	private void OnDisable()
	{
		positionComponent.CoordsUpdated.Remove(OnPositionUpdated);
		rotationComponent.RotationUpdated.Remove(OnRotationUpdated);
	}

	private void OnPositionUpdated(Coordinates coords)
	{
		if (positionComponent.Authority == Authority.NotAuthoritative)
		{
			myRigidbody.MovePosition(coords.ToVector3());
		}
	}

	private void OnRotationUpdated(Improbable.Core.Quaternion rotation)
	{
		if (rotationComponent.Authority == Authority.NotAuthoritative)
		{
			myRigidbody.MoveRotation(rotation.ToUnityQuaternion());
		}
	}
}

