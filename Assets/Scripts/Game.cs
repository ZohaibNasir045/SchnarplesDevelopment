using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity;
using UnityEngine.UI;

namespace GoFish
{
    public class Game : MonoBehaviour
    {
        public Text MessageText;
        public Text LocalName;
        public Text CurPlayer;
        public Text NextPlayer;
        protected bool Multiplayer = false;
        protected CardAnimator cardAnimator;

        [SerializeField]
        protected GameDataManager gameDataManager;

        public List<Transform> PlayerPositions = new List<Transform>();
        public List<Transform> BookPositions = new List<Transform>();

        [SerializeField]
        protected Player localPlayer;
        [SerializeField]
        protected Player remotePlayer1;
        [SerializeField]
        protected Player remotePlayer2;
        [SerializeField]
        protected Player remotePlayer3;

        [SerializeField]
        protected Player currentTurnPlayer = null ;
        [SerializeField]
        protected List<Player> TurnSequence = null;
        [SerializeField]
        protected Player currentTurnTargetPlayer;

        [SerializeField]
        protected Card selectedCard;
        [SerializeField]
        protected Ranks selectedRank;

        public enum GameState
        {
            Idle,
            GameStarted,
            TurnStarted,
            TurnSelectingNumber,
            TurnConfirmedSelectedNumber,
            TurnWaitingForOpponentConfirmation,
            TurnOpponentConfirmed,
            TurnGoFish,
            GameFinished
        };

        [SerializeField]
        protected GameState gameState = GameState.Idle;

        protected void Awake()
        {
            Debug.Log("base awake");
            if (!Multiplayer)
            {
                localPlayer = new Player();
                localPlayer.PlayerId = "offline-player";
                localPlayer.PlayerName = "Player";
                localPlayer.Position = PlayerPositions[0].position;
                localPlayer.BookPosition = BookPositions[0].position;
                LocalName.text = localPlayer.PlayerName;
                TurnSequence.Add(localPlayer);

                remotePlayer1 = new Player();
                remotePlayer1.PlayerId = "offline-bot-1";
                remotePlayer1.PlayerName = "Bot1";
                remotePlayer1.Position = PlayerPositions[1].position;
                remotePlayer1.BookPosition = BookPositions[1].position;
                remotePlayer1.IsAI = true;
                TurnSequence.Add(remotePlayer1);

                remotePlayer2 = new Player();
                remotePlayer2.PlayerId = "offline-bot-2";
                remotePlayer2.PlayerName = "Bot2";
                remotePlayer2.Position = PlayerPositions[2].position;
                remotePlayer2.BookPosition = BookPositions[2].position;
                remotePlayer2.IsAI = true;
                TurnSequence.Add(remotePlayer2);

                remotePlayer3 = new Player();
                remotePlayer3.PlayerId = "offline-bot-3";
                remotePlayer3.PlayerName = "Bot3";
                remotePlayer3.Position = PlayerPositions[3].position;
                remotePlayer3.BookPosition = BookPositions[3].position;
                remotePlayer3.IsAI = true;
                TurnSequence.Add(remotePlayer3);
            }
            cardAnimator = FindObjectOfType<CardAnimator>();
        }

        protected void Start()
        {
            gameState = GameState.GameStarted;
            GameFlow();
        }

        //****************** Game Flow *********************//
        public virtual void GameFlow()
        {
            if (gameState > GameState.GameStarted)
            {
                CheckPlayersBooks();
                ShowAndHidePlayersDisplayingCards();

                if (gameDataManager.GameFinished())
                {
                    gameState = GameState.GameFinished;
                }
            }

            switch (gameState)
            {
                case GameState.Idle:
                    {
                        Debug.Log("IDLE");
                        break;
                    }
                case GameState.GameStarted:
                    {
                        Debug.Log("GameStarted");
                        OnGameStarted();
                        break;
                    }
                case GameState.TurnStarted:
                    {
                        Debug.Log("TurnStarted");
                        OnTurnStarted();
                        break;
                    }
                case GameState.TurnSelectingNumber:
                    {
                        Debug.Log("TurnSelectingNumber");
                        OnTurnSelectingNumber();
                        break;
                    }
                case GameState.TurnConfirmedSelectedNumber:
                    {
                        Debug.Log("TurnComfirmedSelectedNumber");
                        OnTurnConfirmedSelectedNumber();
                        break;
                    }
                case GameState.TurnWaitingForOpponentConfirmation:
                    {
                        Debug.Log("TurnWaitingForOpponentConfirmation");
                        OnTurnWaitingForOpponentConfirmation();
                        break;
                    }
                case GameState.TurnOpponentConfirmed:
                    {
                        Debug.Log("TurnOpponentConfirmed");
                        OnTurnOpponentConfirmed();
                        break;
                    }
                case GameState.TurnGoFish:
                    {
                        Debug.Log("TurnGoFish");
                        OnTurnGoFish();
                        break;
                    }
                case GameState.GameFinished:
                    {
                        Debug.Log("GameFinished");
                        OnGameFinished();
                        break;
                    }
            }
        }

        protected virtual void OnGameStarted()
        {
            gameDataManager = new GameDataManager(localPlayer, remotePlayer1, remotePlayer2, remotePlayer3);
            gameDataManager.Shuffle();
            gameDataManager.DealCardValuesToPlayer(localPlayer, Constants.PLAYER_INITIAL_CARDS);
            gameDataManager.DealCardValuesToPlayer(remotePlayer1, Constants.PLAYER_INITIAL_CARDS);
            gameDataManager.DealCardValuesToPlayer(remotePlayer2, Constants.PLAYER_INITIAL_CARDS);
            gameDataManager.DealCardValuesToPlayer(remotePlayer3, Constants.PLAYER_INITIAL_CARDS);

            cardAnimator.DealDisplayingCards(localPlayer, Constants.PLAYER_INITIAL_CARDS);
            cardAnimator.DealDisplayingCards(remotePlayer1, Constants.PLAYER_INITIAL_CARDS);
            cardAnimator.DealDisplayingCards(remotePlayer2, Constants.PLAYER_INITIAL_CARDS);
            cardAnimator.DealDisplayingCards(remotePlayer3, Constants.PLAYER_INITIAL_CARDS);

            gameState = GameState.TurnStarted;
        }

        protected virtual void OnTurnStarted()
        {
            SwitchTurn();
            
            gameState = GameState.TurnSelectingNumber;
            GameFlow();
        }

        public void OnTurnSelectingNumber()
        {
            ResetSelectedCard();

            if (currentTurnPlayer == localPlayer)
            {
                SetMessage($"Your turn. Pick a card from your hand.");
            }
            else if (currentTurnTargetPlayer == localPlayer)
            {
                SetMessage($"Next turn is yours.");
            }
            else
            {
                SetMessage($"{currentTurnPlayer.PlayerName}'s turn");
            }

            if (currentTurnPlayer.IsAI)
            {
                selectedRank = gameDataManager.SelectRandomRanksFromPlayersCardValues(currentTurnPlayer);
                gameState = GameState.TurnConfirmedSelectedNumber;
                GameFlow();
            }
        }

        protected virtual void OnTurnConfirmedSelectedNumber()
        {
            if (currentTurnPlayer == localPlayer)
            {
                SetMessage($"Asking {currentTurnTargetPlayer.PlayerName} for {selectedRank}s...");
            }
            else
            {
                SetMessage($"{currentTurnPlayer.PlayerName} is asking for {selectedRank}s...");
            }

            gameState = GameState.TurnWaitingForOpponentConfirmation;
            GameFlow();
        }

        public void OnTurnWaitingForOpponentConfirmation()
        {
            if (currentTurnTargetPlayer.IsAI)
            {
                gameState = GameState.TurnOpponentConfirmed;
                GameFlow();
            }
        }

        protected virtual void OnTurnOpponentConfirmed()
        {
            List<byte> cardValuesFromTargetPlayer = gameDataManager.TakeCardValuesWithRankFromPlayer(currentTurnTargetPlayer, selectedRank);

            if (cardValuesFromTargetPlayer.Count > 0)
            {
                gameDataManager.AddCardValuesToPlayer(currentTurnPlayer, cardValuesFromTargetPlayer);

                bool senderIsLocalPlayer = currentTurnTargetPlayer == localPlayer;
                currentTurnTargetPlayer.SendDisplayingCardToPlayer(currentTurnPlayer, cardAnimator, cardValuesFromTargetPlayer, senderIsLocalPlayer);
                gameState = GameState.TurnSelectingNumber;
            }
            else
            {
                gameState = GameState.TurnGoFish;
                GameFlow();
            }
        }

        protected virtual void OnTurnGoFish()
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
                gameState = GameState.TurnStarted;
            }

            gameDataManager.AddCardValueToPlayer(currentTurnPlayer, cardValue);
        }

        public void OnGameFinished()
        {
            if (gameDataManager.Winner() == localPlayer)
            {
                SetMessage($"You WON!");
            }
            else
            {
                SetMessage($"You LOST!");
            }
        }

        //****************** Helper Methods *********************//
        public void ResetSelectedCard()
        {
            if (selectedCard != null)
            {
                selectedCard.OnSelected(false);
                selectedCard = null;
                selectedRank = 0;
            }
        }

        protected void SetMessage(string message)
        {
            MessageText.text = message;
        }

        public void SwitchTurn()
        {
            
            if (currentTurnPlayer.PlayerName == "")
            {
                currentTurnPlayer = TurnSequence[0];
                currentTurnTargetPlayer = TurnSequence[1];
                UpdateTurnUI();
                return;
            }
            else
            {
                if(currentTurnPlayer == TurnSequence[0])
                {
                    currentTurnPlayer = TurnSequence[1];
                    currentTurnTargetPlayer = TurnSequence[2];
                    UpdateTurnUI();
                    return;
                }
                else if (currentTurnPlayer == TurnSequence[1])
                {
                    currentTurnPlayer = TurnSequence[2];
                    currentTurnTargetPlayer = TurnSequence[3];
                    UpdateTurnUI();
                    return;
                }
                else if (currentTurnPlayer == TurnSequence[2])
                {
                    currentTurnPlayer = TurnSequence[3];
                    currentTurnTargetPlayer = TurnSequence[0];
                    UpdateTurnUI();
                    return;
                }
                else if (currentTurnPlayer == TurnSequence[3])
                {
                    currentTurnPlayer = TurnSequence[0];
                    currentTurnTargetPlayer = TurnSequence[1];
                    UpdateTurnUI();
                    return;
                }
            }
            
        }
        public void UpdateTurnUI()
        {
            CurPlayer.text = currentTurnPlayer.PlayerName;
            NextPlayer.text = currentTurnTargetPlayer.PlayerName;
        }
        public void PlayerShowBooksIfNecessary(Player player)
        {
            Dictionary<Ranks, List<byte>> books = gameDataManager.GetBooks(player);

            if (books != null)
            {
                foreach (var book in books)
                {
                    player.ReceiveBook(book.Key, cardAnimator);

                    gameDataManager.RemoveCardValuesFromPlayer(player, book.Value);
                    gameDataManager.AddBooksForPlayer(player, book.Key);
                }
            }
        }

        public void CheckPlayersBooks()
        {
            List<byte> playerCardValues = gameDataManager.PlayerCards(localPlayer);
            localPlayer.SetCardValues(playerCardValues);
            PlayerShowBooksIfNecessary(localPlayer);

            playerCardValues = gameDataManager.PlayerCards(remotePlayer1);
            remotePlayer1.SetCardValues(playerCardValues);
            playerCardValues = gameDataManager.PlayerCards(remotePlayer2);
            remotePlayer2.SetCardValues(playerCardValues);
            playerCardValues = gameDataManager.PlayerCards(remotePlayer3);
            remotePlayer3.SetCardValues(playerCardValues);
            PlayerShowBooksIfNecessary(remotePlayer1);
            PlayerShowBooksIfNecessary(remotePlayer2);
            PlayerShowBooksIfNecessary(remotePlayer3);
        }

        public void ShowAndHidePlayersDisplayingCards()
        {
            localPlayer.ShowCardValues();
            remotePlayer1.HideCardValues();
            remotePlayer2.HideCardValues();
            remotePlayer3.HideCardValues();
        }

        //****************** User Interaction *********************//
        public void OnCardSelected(Card card)
        {
            if (gameState == GameState.TurnSelectingNumber)
            {
                if (card.OwnerId == currentTurnPlayer.PlayerId)
                {
                    if (selectedCard != null)
                    {
                        selectedCard.OnSelected(false);
                        selectedRank = 0;
                    }

                    selectedCard = card;
                    selectedRank = selectedCard.Rank;
                    selectedCard.OnSelected(true);
                    SetMessage($"Ask {currentTurnTargetPlayer.PlayerName} for {selectedCard.Rank}s ?");
                }
            }
        }

        public virtual void OnOkSelected()
        {
            if (gameState == GameState.TurnSelectingNumber && localPlayer == currentTurnPlayer)
            {
                if (selectedCard != null)
                {
                    gameState = GameState.TurnConfirmedSelectedNumber;
                    GameFlow();
                }
            }
            else if (gameState == GameState.TurnWaitingForOpponentConfirmation && localPlayer == currentTurnTargetPlayer)
            {
                gameState = GameState.TurnOpponentConfirmed;
                GameFlow();
            }
        }

        //****************** Animator Event *********************//
        public virtual void AllAnimationsFinished()
        {
            GameFlow();
        }
    }
}
