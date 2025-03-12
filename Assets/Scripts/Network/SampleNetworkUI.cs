using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SampleNetworkUI : MonoBehaviour
{
    [SerializeField] Button _host;
    [SerializeField] Button _client;

	private void Start()
	{
		_host.onClick.AddListener(
			() => NetworkManager.Singleton.StartHost()
		);
		_client.onClick.AddListener(
			() => NetworkManager.Singleton.StartClient()
		);
	}
}
