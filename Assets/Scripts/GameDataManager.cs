using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GoFish
{
    [Serializable]
    public class EncryptedData
    {
        public byte[] data;
    }

    [Serializable]
    public class GameDataManager
    {
        Player localPlayer;
        Player remotePlayer1;
        Player remotePlayer2;
        Player remotePlayer3;

        [SerializeField]
        ProtectedData protectedData;

        public GameDataManager(Player local, Player remote1, Player remote2, Player remote3, string roomId = "1234567890123456")
        {
            localPlayer = local;
            remotePlayer1 = remote1;
            remotePlayer2 = remote2;
            remotePlayer3 = remote3;
            protectedData = new ProtectedData(localPlayer.PlayerId, remotePlayer1.PlayerId, remotePlayer2.PlayerId, remotePlayer3.PlayerId, roomId);
            
        }

        public void Shuffle()
        {
            List<byte> cardValues = new List<byte>();

            for (byte value = 0; value < 52; value++)
            {
                cardValues.Add(value);
            }

            List<byte> poolOfCards = new List<byte>();

            for (int index = 0; index < 52; index++)
            {
                int valueIndexToAdd = UnityEngine.Random.Range(0, cardValues.Count);

                byte valueToAdd = cardValues[valueIndexToAdd];
                poolOfCards.Add(valueToAdd);
                cardValues.Remove(valueToAdd);
            }

            protectedData.SetPoolOfCards(poolOfCards);
        }

        public void DealCardValuesToPlayer(Player player, int numberOfCards)
        {
            List<byte> poolOfCards = protectedData.GetPoolOfCards();

            int numberOfCardsInThePool = poolOfCards.Count;
            int start = numberOfCardsInThePool - 1 - numberOfCards;

            List<byte> cardValues = poolOfCards.GetRange(start, numberOfCards);
            poolOfCards.RemoveRange(start, numberOfCards);

            protectedData.AddCardValuesToPlayer(player, cardValues);
            protectedData.SetPoolOfCards(poolOfCards);
        }

        public byte DrawCardValue()
        {
            List<byte> poolOfCards = protectedData.GetPoolOfCards();

            int numberOfCardsInThePool = poolOfCards.Count;

            if (numberOfCardsInThePool > 0)
            {
                byte cardValue = poolOfCards[numberOfCardsInThePool - 1];
                poolOfCards.Remove(cardValue);
                protectedData.SetPoolOfCards(poolOfCards);
                return cardValue;
            }

            return Constants.POOL_IS_EMPTY;
        }

        public List<byte> PlayerCards(Player player)
        {
            return protectedData.PlayerCards(player);
        }

        public List<byte> PlayerBooks(Player player)
        {
            return protectedData.PlayerBooks(player);
        }

        public void AddCardValuesToPlayer(Player player, List<byte> cardValues)
        {
            protectedData.AddCardValuesToPlayer(player, cardValues);
        }

        public void AddCardValueToPlayer(Player player, byte cardValue)
        {
            protectedData.AddCardValueToPlayer(player, cardValue);
        }

        public void RemoveCardValuesFromPlayer(Player player, List<byte> cardValuesToRemove)
        {
            protectedData.RemoveCardValuesFromPlayer(player, cardValuesToRemove);
        }

        public void AddBooksForPlayer(Player player, Ranks ranks)
        {
            protectedData.AddBooksForPlayer(player, ranks);
        }

        public Player Winner()
        {
            string winnerPlayerId = protectedData.WinnerPlayerId();
            if (winnerPlayerId.Equals(localPlayer.PlayerId))
            {
                return localPlayer;
            }
            else if(winnerPlayerId.Equals(remotePlayer1.PlayerId))
            {
                return remotePlayer1;
            }
            else if (winnerPlayerId.Equals(remotePlayer2.PlayerId))
            {
                return remotePlayer2;
            }
            else
            {
                return remotePlayer3;
            }
        }

        public bool GameFinished()
        {
            return protectedData.GameFinished();
        }

        public List<byte> TakeCardValuesWithRankFromPlayer(Player player, Ranks ranks)
        {
            List<byte> playerCards = protectedData.PlayerCards(player);

            List<byte> result = new List<byte>();

            foreach (byte cv in playerCards)
            {
                if (Card.GetRank(cv) == ranks)
                {
                    result.Add(cv);
                }
            }

            protectedData.RemoveCardValuesFromPlayer(player, result);

            return result;
        }

        public Dictionary<Ranks, List<byte>> GetBooks(Player player)
        {
            List<byte> playerCards = protectedData.PlayerCards(player);

            var groups = playerCards.GroupBy(Card.GetRank).Where(g => g.Count() == 4);

            if (groups.Count() > 0)
            {
                Dictionary<Ranks, List<byte>> setOfFourDictionary = new Dictionary<Ranks, List<byte>>();

                foreach (var group in groups)
                {
                    List<byte> cardValues = new List<byte>();

                    foreach (var value in group)
                    {
                        cardValues.Add(value);
                    }

                    setOfFourDictionary[group.Key] = cardValues;
                }

                return setOfFourDictionary;
            }

            return null;
        }

        public Ranks SelectRandomRanksFromPlayersCardValues(Player player)
        {
            List<byte> playerCards = protectedData.PlayerCards(player);
            int index = UnityEngine.Random.Range(0, playerCards.Count);

            return Card.GetRank(playerCards[index]);
        }

        public void SetCurrentTurnPlayer(Player player)
        {
            protectedData.SetCurrentTurnPlayerId(player.PlayerId);
        }

        public Player GetCurrentTurnPlayer()
        {
            string playerId = protectedData.GetCurrentTurnPlayerId();
            if (localPlayer.PlayerId.Equals(playerId))
            {
                return localPlayer;
            }
            else if (playerId.Equals(remotePlayer1.PlayerId))
            {
                return remotePlayer1;
            }
            else if (playerId.Equals(remotePlayer2.PlayerId))
            {
                return remotePlayer2;
            }
            else
            {
                return remotePlayer3;
            }
        }

        public Player GetCurrentTurnTargetPlayer()
        {
            string playerId = protectedData.GetCurrentTurnPlayerId();
            if (localPlayer.PlayerId.Equals(playerId))
            {
                return remotePlayer1;
            }
            else if (remotePlayer1.PlayerId.Equals(playerId))
            {
                return remotePlayer2;
            }
            else if (remotePlayer2.PlayerId.Equals(playerId))
            {
                return remotePlayer3;
            }
            else
            {
                return localPlayer;
            }
        }

        public void SetGameState(Game.GameState gameState)
        {
            protectedData.SetGameState((int)gameState);
        }

        public Game.GameState GetGameState()
        {
            return (Game.GameState)protectedData.GetGameState();
        }

        public void SetSelectedRank(Ranks rank)
        {
            protectedData.SetSelectedRank((int)rank);
        }

        public Ranks GetSelectedRank()
        {
            return (Ranks)protectedData.GetSelectedRank();
        }

        public EncryptedData EncryptedData()
        {
            Byte[] data = protectedData.ToArray();

            EncryptedData encryptedData = new EncryptedData();
            encryptedData.data = data;

            return encryptedData;
        }

        public void ApplyEncrptedData(EncryptedData encryptedData)
        {
            if(encryptedData == null)
            {
                return;
            }

            protectedData.ApplyByteArray(encryptedData.data);
        }
    }
}
