using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWNetwork;
using UnityEngine.SceneManagement;

namespace GoFish
{
    public class MultiplayerGame : Game
    {
        NetCode netCode;
        public int remoteplayercount=0;
        protected new void Awake()
        {
            Multiplayer = true;
            base.Awake();
            

            netCode = FindObjectOfType<NetCode>();

            NetworkClient.Lobby.GetPlayersInRoom((successful, reply, error) =>
            {
                if (successful)
                {
                    foreach(SWPlayer swPlayer in reply.players)
                    {
                        string playerName = swPlayer.GetCustomDataString();
                        string playerId = swPlayer.id;

                        if (playerId.Equals(NetworkClient.Instance.PlayerId))
                        {
                            localPlayer.PlayerId = playerId;
                            localPlayer.PlayerName = playerName;
                            localPlayer.Position = PlayerPositions[0].position;
                            localPlayer.BookPosition = BookPositions[0].position;
                            LocalName.text = localPlayer.PlayerName;
                            TurnSequence.Add(localPlayer);
                        }
                        else
                        {
                            if (remoteplayercount == 0)
                            {
                                remotePlayer1.PlayerId = playerId;
                                remotePlayer1.PlayerName = playerName;
                                remotePlayer1.Position = PlayerPositions[1].position;
                                remotePlayer1.BookPosition = BookPositions[1].position;
                                remotePlayer1.IsAI = false;
                                TurnSequence.Add(remotePlayer1);
                                remoteplayercount++;
                            }
                            else if (remoteplayercount == 1)
                            {
                                remotePlayer2.PlayerId = playerId;
                                remotePlayer2.PlayerName = playerName;
                                remotePlayer2.Position = PlayerPositions[2].position;
                                remotePlayer2.BookPosition = BookPositions[2].position;
                                remotePlayer2.IsAI = false;
                                TurnSequence.Add(remotePlayer2);
                                remoteplayercount++;
                            }
                            else if (remoteplayercount == 2)
                            {
                                remotePlayer3.PlayerId = playerId;
                                remotePlayer3.PlayerName = playerName;
                                remotePlayer3.Position = PlayerPositions[3].position;
                                remotePlayer3.BookPosition = BookPositions[3].position;
                                remotePlayer3.IsAI = false;
                                TurnSequence.Add(remotePlayer3);
                                remoteplayercount++;
                            }
                        }
                    }

                    gameDataManager = new GameDataManager(localPlayer, remotePlayer1, remotePlayer2, remotePlayer3, NetworkClient.Lobby.RoomId);
                    netCode.EnableRoomPropertyAgent();
                }
                else
                {
                    Debug.Log("Failed to get players in room.");
                }

            });
        }

        protected new void Start()
        {
            Debug.Log("Multiplayer Game Start");
        }

        //****************** Game Flow *********************//
        public override void GameFlow()
        {
            Debug.LogError("Should never be here");
        }

        protected override void OnGameStarted()
        {
            if (NetworkClient.Instance.IsHost)
            {
                gameDataManager.Shuffle();
                gameDataManager.DealCardValuesToPlayer(localPlayer, Constants.PLAYER_INITIAL_CARDS);
                gameDataManager.DealCardValuesToPlayer(remotePlayer1, Constants.PLAYER_INITIAL_CARDS);
                gameDataManager.DealCardValuesToPlayer(remotePlayer2, Constants.PLAYER_INITIAL_CARDS);
                gameDataManager.DealCardValuesToPlayer(remotePlayer3, Constants.PLAYER_INITIAL_CARDS);

                gameState = GameState.TurnStarted;

                gameDataManager.SetGameState(gameState);
                netCode.ModifyGameData(gameDataManager.EncryptedData());
            }

            cardAnimator.DealDisplayingCards(localPlayer, Constants.PLAYER_INITIAL_CARDS);
            cardAnimator.DealDisplayingCards(remotePlayer1, Constants.PLAYER_INITIAL_CARDS);
            cardAnimator.DealDisplayingCards(remotePlayer2, Constants.PLAYER_INITIAL_CARDS);
            cardAnimator.DealDisplayingCards(remotePlayer3, Constants.PLAYER_INITIAL_CARDS);
        }

        protected override void OnTurnStarted()
        {
            if (NetworkClient.Instance.IsHost)
            {
                SwitchTurn();
                gameState = GameState.TurnSelectingNumber;

                gameDataManager.SetCurrentTurnPlayer(currentTurnPlayer);
                gameDataManager.SetGameState(gameState);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
                netCode.NotifyOtherPlayersGameStateChanged();
            }
        }

        protected override void OnTurnConfirmedSelectedNumber()
        {
            if (currentTurnPlayer == localPlayer)
            {
                SetMessage($"Asking {currentTurnTargetPlayer.PlayerName} for {selectedRank}s...");
            }
            else
            {
                SetMessage($"{currentTurnPlayer.PlayerName} is asking for {selectedRank}s...");
            }

            /*if (NetworkClient.Instance.IsHost)
            {*/
                gameState = GameState.TurnWaitingForOpponentConfirmation;
                gameDataManager.SetGameState(gameState);

                netCode.ModifyGameData(gameDataManager.EncryptedData());
                netCode.NotifyOtherPlayersGameStateChanged();
            //}
        }

        protected override void OnTurnOpponentConfirmed()
        {
            List<byte> cardValuesFromTargetPlayer = gameDataManager.TakeCardValuesWithRankFromPlayer(currentTurnTargetPlayer, selectedRank);

            if (cardValuesFromTargetPlayer.Count > 0)
            {
                gameDataManager.AddCardValuesToPlayer(currentTurnPlayer, cardValuesFromTargetPlayer);

                bool senderIsLocalPlayer = currentTurnTargetPlayer == localPlayer;
                currentTurnTargetPlayer.SendDisplayingCardToPlayer(currentTurnPlayer, cardAnimator, cardValuesFromTargetPlayer, senderIsLocalPlayer);

                /*if (NetworkClient.Instance.IsHost)
                {*/
                    gameState = GameState.TurnSelectingNumber;

                    gameDataManager.SetGameState(gameState);
                    netCode.ModifyGameData(gameDataManager.EncryptedData());
                //}

            }
            else
            {
                /*if (NetworkClient.Instance.IsHost)
                {*/
                    gameState = GameState.TurnGoFish;

                    gameDataManager.SetGameState(gameState);
                    netCode.ModifyGameData(gameDataManager.EncryptedData());
                    netCode.NotifyOtherPlayersGameStateChanged();
                //}
            }
        }

        protected override void OnTurnGoFish()
        {
            SetMessage($"Go fish!");

            byte cardValue = gameDataManager.DrawCardValue();

            if (cardValue == Constants.POOL_IS_EMPTY)
            {
                Debug.LogError("Pool is empty");
                return;
            }

            if (Card.GetRank(cardValue) == selectedRank)
            {
                cardAnimator.DrawDisplayingCard(currentTurnPlayer, cardValue);
            }
            else
            {
                cardAnimator.DrawDisplayingCard(currentTurnPlayer);

                /*if (NetworkClient.Instance.IsHost)
                {*/
                    gameState = GameState.TurnStarted;
                //}
            }

            gameDataManager.AddCardValueToPlayer(currentTurnPlayer, cardValue);

            //if (NetworkClient.Instance.IsHost)
            //{
                gameDataManager.SetGameState(gameState);
                netCode.ModifyGameData(gameDataManager.EncryptedData());
            //}
        }

        public override void AllAnimationsFinished()
        {
            //if (NetworkClient.Instance.IsHost)
            //{
                netCode.NotifyOtherPlayersGameStateChanged();
            //}
        }

        //****************** User Interaction *********************//
        public override void OnOkSelected()
        {
            if (gameState == GameState.TurnSelectingNumber && localPlayer == currentTurnPlayer)
            {
                if (selectedCard != null)
                {
                    netCode.NotifyHostPlayerRankSelected((int)selectedCard.Rank);
                }
            }
            else if (gameState == GameState.TurnWaitingForOpponentConfirmation && localPlayer == currentTurnTargetPlayer)
            {
                netCode.NotifyHostPlayerOpponentConfirmed();
            }
            else if (gameState == GameState.GameFinished)
            {
                netCode.LeaveRoom();
            }
        }

        //****************** NetCode Events *********************//
        public void OnGameDataReady(EncryptedData encryptedData)
        {
            if(encryptedData == null)
            {
                Debug.Log("New game");
                //if (NetworkClient.Instance.IsHost)
                //{
                    gameState = GameState.GameStarted;
                    gameDataManager.SetGameState(gameState);

                    netCode.ModifyGameData(gameDataManager.EncryptedData());

                    netCode.NotifyOtherPlayersGameStateChanged();
                //}
            }
            else
            {
                gameDataManager.ApplyEncrptedData(encryptedData);
                gameState = gameDataManager.GetGameState();
                currentTurnPlayer = gameDataManager.GetCurrentTurnPlayer();
                currentTurnTargetPlayer = gameDataManager.GetCurrentTurnTargetPlayer();
                selectedRank = gameDataManager.GetSelectedRank();

                if(gameState > GameState.GameStarted)
                {
                    Debug.Log("Restore the game state");

                    //restore player's cards
                    cardAnimator.DealDisplayingCards(localPlayer, gameDataManager.PlayerCards(localPlayer).Count, false);
                    cardAnimator.DealDisplayingCards(remotePlayer1, gameDataManager.PlayerCards(remotePlayer1).Count, false);
                    cardAnimator.DealDisplayingCards(remotePlayer2, gameDataManager.PlayerCards(remotePlayer2).Count, false);
                    cardAnimator.DealDisplayingCards(remotePlayer3, gameDataManager.PlayerCards(remotePlayer3).Count, false);

                    //restore player's books
                    List<byte> booksForLocalPlayer = gameDataManager.PlayerBooks(localPlayer);
                    foreach(byte rank in booksForLocalPlayer)
                    {
                        localPlayer.RestoreBook((Ranks)rank, cardAnimator);
                    }

                    List<byte> booksForremotePlayer1 = gameDataManager.PlayerBooks(remotePlayer1);
                    foreach (byte rank in booksForremotePlayer1)
                    {
                        remotePlayer1.RestoreBook((Ranks)rank, cardAnimator);
                    }
                    List<byte> booksForremotePlayer2 = gameDataManager.PlayerBooks(remotePlayer2);
                    foreach (byte rank in booksForremotePlayer2)
                    {
                        remotePlayer2.RestoreBook((Ranks)rank, cardAnimator);
                    }
                    List<byte> booksForremotePlayer3 = gameDataManager.PlayerBooks(remotePlayer3);
                    foreach (byte rank in booksForremotePlayer3)
                    {
                        remotePlayer3.RestoreBook((Ranks)rank, cardAnimator);
                    }
                    base.GameFlow();
                }
            }
        }

        public void OnGameDataChanged(EncryptedData encryptedData)
        {
            gameDataManager.ApplyEncrptedData(encryptedData);
            gameState = gameDataManager.GetGameState();
            currentTurnPlayer = gameDataManager.GetCurrentTurnPlayer();
            currentTurnTargetPlayer = gameDataManager.GetCurrentTurnTargetPlayer();
            selectedRank = gameDataManager.GetSelectedRank();
        }

        public void OnGameStateChanged()
        {
            base.GameFlow();
        }

        public void OnRankSelected(Ranks rank)
        {
            selectedRank = rank;
            gameState = GameState.TurnConfirmedSelectedNumber;

            gameDataManager.SetSelectedRank(selectedRank);
            gameDataManager.SetGameState(gameState);

            netCode.ModifyGameData(gameDataManager.EncryptedData());
            netCode.NotifyOtherPlayersGameStateChanged();
        }

        public void OnOppoentConfirmed()
        {
            gameState = GameState.TurnOpponentConfirmed;

            gameDataManager.SetGameState(gameState);

            netCode.ModifyGameData(gameDataManager.EncryptedData());
            netCode.NotifyOtherPlayersGameStateChanged();
        }

        public void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }
    }
}
