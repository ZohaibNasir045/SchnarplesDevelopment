﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CurrentRoomIsVisibleProperty.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.UI;


    /// <summary>
    /// PhotonNetwork.CurrentRoom.IsVisible UI property
    /// </summary>
    public class CurrentRoomIsVisibleProperty : PropertyListenerBase
    {

        public Text Text;

        int _cache = -1;

        void Update()
        {

            if (PhotonNetwork.CurrentRoom != null)
            {
                if ((PhotonNetwork.CurrentRoom.IsVisible && _cache != 1) || (!PhotonNetwork.CurrentRoom.IsVisible && _cache != 0))
                {
                    _cache = PhotonNetwork.CurrentRoom.IsVisible ? 1 : 0;
                    Text.text = PhotonNetwork.CurrentRoom.IsVisible ? "true" : "false";
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
