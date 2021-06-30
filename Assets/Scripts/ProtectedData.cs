using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SWNetwork;
using UnityEngine;

namespace GoFish
{
    /// <summary>
    /// Stores the important data of the game
    /// We will encypt the fields in a multiplayer game.
    /// </summary>
    [Serializable]
    public class ProtectedData
    {
        [SerializeField]
        List<byte> poolOfCards = new List<byte>();
        [SerializeField]
        List<byte> player1Cards = new List<byte>();
        [SerializeField]
        List<byte> player2Cards = new List<byte>();
        [SerializeField]
        List<byte> player3Cards = new List<byte>();
        [SerializeField]
        List<byte> player4Cards = new List<byte>();
        [SerializeField]
        List<byte> booksForPlayer1 = new List<byte>();
        [SerializeField]
        List<byte> booksForPlayer2 = new List<byte>();
        [SerializeField]
        List<byte> booksForPlayer3 = new List<byte>();
        [SerializeField]
        List<byte> booksForPlayer4 = new List<byte>();
        [SerializeField]
        string player1Id;
        [SerializeField]
        string player2Id;
        [SerializeField]
        string player3Id;
        [SerializeField]
        string player4Id;
        [SerializeField]
        string currentTurnPlayerId;
        [SerializeField]
        int currentGameState;
        [SerializeField]
        int selectedRank;

        byte[] encryptionKey;
        byte[] safeData;

        public ProtectedData(string p1Id, string p2Id, string p3Id, string p4Id, string roomId)
        {
            player1Id = p1Id;
            player2Id = p2Id;
            player3Id = p3Id;
            player4Id = p4Id;
            currentTurnPlayerId = "";
            selectedRank = (int)Ranks.NoRanks;
            CalculateKey(roomId);
            Encrypt();
        }

        public void SetPoolOfCards(List<byte> cardValues)
        {
            Decrypt();
            poolOfCards = cardValues;
            Encrypt();
        }

        public List<byte> GetPoolOfCards()
        {
            List<byte> result;
            Decrypt();
            result = poolOfCards;
            Encrypt();
            return result;
        }

        public List<byte> PlayerCards(Player player)
        {
            List<byte> result;
            Decrypt();
            if (player.PlayerId.Equals(player1Id))
            {
                result = player1Cards;
            }
            else if (player.PlayerId.Equals(player2Id))
            {
                result = player2Cards;
            }
            else if (player.PlayerId.Equals(player3Id))
            {
                result = player3Cards;
            }
            else
            {
                result = player4Cards;
            }
            Encrypt();
            return result;
        }

        public List<byte> PlayerBooks(Player player)
        {
            List<byte> result;
            Decrypt();
            if (player.PlayerId.Equals(player1Id))
            {
                result = booksForPlayer1;
            }
            else
            {
                result = booksForPlayer2;
            }
            Encrypt();
            return result;
        }

        public void AddCardValuesToPlayer(Player player, List<byte> cardValues)
        {
            Decrypt();
            if (player.PlayerId.Equals(player1Id))
            {
                player1Cards.AddRange(cardValues);
                player1Cards.Sort();
            }
            else if (player.PlayerId.Equals(player2Id))
            {
                player2Cards.AddRange(cardValues);
                player2Cards.Sort();
            }
            else if (player.PlayerId.Equals(player3Id))
            {
                player3Cards.AddRange(cardValues);
                player3Cards.Sort();
            }
            else
            {
                player4Cards.AddRange(cardValues);
                player4Cards.Sort();
            }
            Encrypt();
        }

        public void AddCardValueToPlayer(Player player, byte cardValue)
        {
            Decrypt();
            if (player.PlayerId.Equals(player1Id))
            {
                player1Cards.Add(cardValue);
                player1Cards.Sort();
            }
            else if (player.PlayerId.Equals(player2Id))
            {
                player2Cards.Add(cardValue);
                player2Cards.Sort();
            }
            else if (player.PlayerId.Equals(player3Id))
            {
                player3Cards.Add(cardValue);
                player3Cards.Sort();
            }
            else
            {
                player4Cards.Add(cardValue);
                player4Cards.Sort();
            }
            Encrypt();
        }

        public void RemoveCardValuesFromPlayer(Player player, List<byte> cardValuesToRemove)
        {
            Decrypt();
            if (player.PlayerId.Equals(player1Id))
            {
                player1Cards.RemoveAll(cv => cardValuesToRemove.Contains(cv));
            }
            else if (player.PlayerId.Equals(player2Id))
            {
                player2Cards.RemoveAll(cv => cardValuesToRemove.Contains(cv));
            }
            else if (player.PlayerId.Equals(player3Id))
            {
                player3Cards.RemoveAll(cv => cardValuesToRemove.Contains(cv));
            }
            else
            {
                player4Cards.RemoveAll(cv => cardValuesToRemove.Contains(cv));
            }
            Encrypt();
        }

        public void AddBooksForPlayer(Player player, Ranks ranks)
        {
            Decrypt();
            if (player.PlayerId.Equals(player1Id))
            {
                booksForPlayer1.Add((byte)ranks);
            }
            else if (player.PlayerId.Equals(player2Id))
            {
                booksForPlayer2.Add((byte)ranks);
            }
            else if (player.PlayerId.Equals(player3Id))
            {
                booksForPlayer3.Add((byte)ranks);
            }
            else
            {
                booksForPlayer4.Add((byte)ranks);
            }
            Encrypt();
        }

        public bool GameFinished()
        {
            bool result = false;
            Decrypt();
            if (poolOfCards.Count == 0)
            {
                result = true;
            }

            if (player1Cards.Count == 0)
            {
                result = true;
            }

            if (player2Cards.Count == 0)
            {
                result = true;
            }
            if (player3Cards.Count == 0)
            {
                result = true;
            }
            if (player4Cards.Count == 0)
            {
                result = true;
            }
            Encrypt();

            return result;
        }

        public string WinnerPlayerId()
        {
            string result;
            Decrypt();
            int higest = booksForPlayer1.Count;
            if (booksForPlayer2.Count > higest)
            {
                result = player2Id;
            }
            if (booksForPlayer3.Count > higest)
            {
                result = player3Id;
            }
            if (booksForPlayer4.Count > higest)
            {
                result = player4Id;
            }
            else
            {
                result = player1Id;
            }
            Encrypt();
            return result;
        }

        public void SetCurrentTurnPlayerId(string playerId)
        {
            Decrypt();
            currentTurnPlayerId = playerId;
            Encrypt();
        }

        public string GetCurrentTurnPlayerId()
        {
            string result;
            Decrypt();
            result = currentTurnPlayerId;
            Encrypt();
            return result;
        }

        public void SetGameState(int gameState)
        {
            Decrypt();
            currentGameState = gameState;
            Encrypt();
        }
        public int GetGameState()
        {
            int result;
            Decrypt();
            result = currentGameState;
            Encrypt();
            return result;
        }

        public void SetSelectedRank(int rank)
        {
            Decrypt();
            selectedRank = rank;
            Encrypt();
        }

        public int GetSelectedRank()
        {
            int result;
            Decrypt();
            result = selectedRank;
            Encrypt();
            return result;
        }

        public Byte[] ToArray()
        {
            return safeData;
        }

        public void ApplyByteArray(Byte[] byteArray)
        {
            safeData = byteArray;
        }

        void CalculateKey(string roomId)
        {
            string roomIdSubString = roomId.Substring(0, 16);
            encryptionKey = Encoding.UTF8.GetBytes(roomIdSubString);
        }

        void Encrypt()
        {
            SWNetworkMessage message = new SWNetworkMessage();
            message.Push((Byte)poolOfCards.Count);
            message.PushByteArray(poolOfCards.ToArray());

            message.Push((Byte)player1Cards.Count);
            message.PushByteArray(player1Cards.ToArray());

            message.Push((Byte)player2Cards.Count);
            message.PushByteArray(player2Cards.ToArray());

            message.Push((Byte)player3Cards.Count);
            message.PushByteArray(player3Cards.ToArray());

            message.Push((Byte)player4Cards.Count);
            message.PushByteArray(player4Cards.ToArray());

            message.Push((Byte)booksForPlayer1.Count);
            message.PushByteArray(booksForPlayer1.ToArray());

            message.Push((Byte)booksForPlayer2.Count);
            message.PushByteArray(booksForPlayer2.ToArray());

            message.Push((Byte)booksForPlayer3.Count);
            message.PushByteArray(booksForPlayer3.ToArray());

            message.Push((Byte)booksForPlayer4.Count);
            message.PushByteArray(booksForPlayer4.ToArray());

            message.PushUTF8ShortString(player1Id);
            message.PushUTF8ShortString(player2Id);
            message.PushUTF8ShortString(player3Id);
            message.PushUTF8ShortString(player4Id);

            message.PushUTF8ShortString(currentTurnPlayerId);
            message.Push(currentGameState);

            message.Push(selectedRank);

            safeData = AES.EncryptAES128(message.ToArray(), encryptionKey);

            poolOfCards = new List<byte>();
            player1Cards = new List<byte>();
            player2Cards = new List<byte>();
            player3Cards = new List<byte>();
            player4Cards = new List<byte>();
            booksForPlayer1 = new List<byte>();
            booksForPlayer2 = new List<byte>();
            booksForPlayer3 = new List<byte>();
            booksForPlayer4 = new List<byte>();
            player1Id = null;
            player2Id = null;
            player3Id = null;
            player4Id = null;
            currentTurnPlayerId = null;
            currentGameState = 0;
            selectedRank = 0;
        }

        void Decrypt()
        {
            byte[] byteArray = AES.DecryptAES128(safeData, encryptionKey);

            SWNetworkMessage message = new SWNetworkMessage(byteArray);
            byte poolOfCardsCount = message.PopByte();
            poolOfCards = message.PopByteArray(poolOfCardsCount).ToList();

            byte player1CardsCount = message.PopByte();
            player1Cards = message.PopByteArray(player1CardsCount).ToList();

            byte player2CardsCount = message.PopByte();
            player2Cards = message.PopByteArray(player2CardsCount).ToList();

            byte player3CardsCount = message.PopByte();
            player3Cards = message.PopByteArray(player3CardsCount).ToList();

            byte player4CardsCount = message.PopByte();
            player4Cards = message.PopByteArray(player4CardsCount).ToList();

            byte booksForPlayer1Count = message.PopByte();
            booksForPlayer1 = message.PopByteArray(booksForPlayer1Count).ToList();

            byte booksForPlayer2Count = message.PopByte();
            booksForPlayer2 = message.PopByteArray(booksForPlayer2Count).ToList();

            byte booksForPlayer3Count = message.PopByte();
            booksForPlayer3 = message.PopByteArray(booksForPlayer3Count).ToList();

            byte booksForPlayer4Count = message.PopByte();
            booksForPlayer4 = message.PopByteArray(booksForPlayer4Count).ToList();

            player1Id = message.PopUTF8ShortString();
            player2Id = message.PopUTF8ShortString();
            player3Id = message.PopUTF8ShortString();
            player4Id = message.PopUTF8ShortString();

            currentTurnPlayerId = message.PopUTF8ShortString();
            currentGameState = message.PopInt32();

            selectedRank = message.PopInt32();
        }
    }
}