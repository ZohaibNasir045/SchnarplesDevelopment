﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentRoomMaxPlayersProperty.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.UI;


    /// <summary>
    /// PhotonNetwork.CurrentRoom.MaxPlayers UI property.
    /// </summary>
    public class CurrentRoomMaxPlayersProperty : PropertyListenerBase
    {
        public Text Text;

        int _cache = -1;

        void Update()
        {
            if (PhotonNetwork.CurrentRoom != null)
            {
                if (PhotonNetwork.CurrentRoom.MaxPlayers != _cache)
                {
                    _cache = PhotonNetwork.CurrentRoom.MaxPlayers;
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
