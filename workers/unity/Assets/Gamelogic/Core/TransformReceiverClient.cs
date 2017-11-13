using Assets.Gamelogic.Utils;
using Improbable;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using UnityEngine;

[WorkerType(WorkerPlatform.UnityClient)]
public class TransformReceiverClient : MonoBehaviour
{
	[Require] private Position.Reader positionComponent;
	[Require] private Rotation.Reader rotationComponent;

	private bool isRemote;

	[SerializeField] private Rigidbody myRigidbody;

	private void Awake()
	{
		myRigidbody = gameObject.GetComponent<Rigidbody>();
	}

	void OnEnable()
	{
		transform.position = positionComponent.Data.coords.ToUnityVector();
		transform.rotation = rotationComponent.Data.rotation.ToUnityQuaternion();

		positionComponent.ComponentUpdated.Add(OnPositionUpdated);
		rotationComponent.ComponentUpdated.Add(OnRotationUpdated);

		if (IsNotAnAuthoritativePlayer())
		{
			SetUpRemoteTransform();
		}
	}

	void OnDisable()
	{
		positionComponent.ComponentUpdated.Remove(OnPositionUpdated);
		rotationComponent.ComponentUpdated.Remove(OnRotationUpdated);
	}

	private void Update()
	{
		if (IsNotAnAuthoritativePlayer())
		{
			myRigidbody.MovePosition(Vector3.Lerp(myRigidbody.position, positionComponent.Data.coords.ToVector3(), 0.2f));
			myRigidbody.MoveRotation(rotationComponent.Data.rotation.ToUnityQuaternion());
		}
		else if (isRemote)
		{
			TearDownRemoveTransform();
		}
	}

	private bool IsNotAnAuthoritativePlayer()
	{
		return gameObject.GetAuthority(ClientAuthorityCheck.ComponentId) == Authority.NotAuthoritative;
	}


	void OnPositionUpdated(Position.Update update)
	{
		if (IsNotAnAuthoritativePlayer())
		{
			if (update.coords.HasValue)
			{
				transform.position = update.coords.Value.ToUnityVector();
			}
		}
	}

	void OnRotationUpdated(Rotation.Update update)
	{
		if (IsNotAnAuthoritativePlayer())
		{
			if (update.rotation.HasValue)
			{
				transform.rotation = update.rotation.Value.ToUnityQuaternion();
			}
		}
	}
	private void SetUpRemoteTransform()
	{
		isRemote = true;
		myRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		myRigidbody.isKinematic = true;
	}

	private void TearDownRemoveTransform()
	{
		isRemote = false;
		myRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		myRigidbody.isKinematic = false;
	}

}
