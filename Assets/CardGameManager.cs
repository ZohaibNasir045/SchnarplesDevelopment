using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CardGameManager : MonoBehaviourPunCallbacks
    {
        public static CardGameManager Instance = null;

        public Text InfoText;

        public GameObject PlayerPrefab;

        #region UNITY

        public void Awake()
        {
            Instance = this;
        }
        // Start is called before the first frame update
        public void Start()
        {
            Hashtable props = new Hashtable
            {
                {CardGame.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion
    }
