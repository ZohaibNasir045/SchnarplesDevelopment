// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentRoomPlayerCountProperty.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.UI;


    /// <summary>
    /// PhotonNetwork.CurrentRoom.PlayerCount UI property.
    /// </summary>
    public class CurrentRoomPlayerCountProperty : PropertyListenerBase
    {
        public Text Text;

        int _cache = -1;

        void Update()
        {
            if (PhotonNetwork.CurrentRoom != null)
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount != _cache)
                {
                    _cache = PhotonNetwork.CurrentRoom.PlayerCount;
                    Text.text = _cache.ToString();
                    this.OnValueChanged();
                }
            }
            else
            {
                if (_cache != -1)
                {
                    _cache = -1;
                    Text.text = "n/a";
                }
            }
        }
    }
