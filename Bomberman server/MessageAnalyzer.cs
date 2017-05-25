using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bomberman_client.GameClasses;
using System.Drawing;

namespace Bomberman_client
{
    public class MessageAnalyzer
    {
        GameCoreServer gameCoreServer;
        public int GetNextValue(byte[] message, ref int i)
        {
            int result = BitConverter.ToInt32(message, i);
            i += sizeof(int);
            return (result);
        }
        private Player FindPlayer(int id)
        {
            foreach (Player searchedPlayer in gameCoreServer.players)
            {
                if (searchedPlayer.id == id)
                {
                    return searchedPlayer;
                }
            }
            return null;
        }
        public void AnalyzeMessage(byte[] message)
        {
            int i = 0;
            int idClient = GetNextValue(message, ref i);
            int kindMessage;
            kindMessage = GetNextValue(message, ref i);
            switch (kindMessage)
            {
                case (int)KindMessages.KindMessage.Player:
                    {

                        int kindPlayerAction = GetNextValue(message, ref i);
                        switch (kindPlayerAction)
                        {
                            case (int)KindMessages.KindPlayerMessages.NewDirection:
                                {
                                    Player searchedPlayer = FindPlayer(idClient);
                                    searchedPlayer.direction = (Player.Direction)GetNextValue(message, ref i);
                                    searchedPlayer.isMoved = true;
                                }
                                break;
                            case (int)KindMessages.KindPlayerMessages.Spawn:
                                {
                                    Player searchedPlayer = FindPlayer(idClient);

                                    searchedPlayer.X = 20;
                                    searchedPlayer.Y = 20;
                                    searchedPlayer.texture = gameCoreServer.playerTexture;
                                    searchedPlayer.IsDying = false;
                                    searchedPlayer.IsDead = false;
                                }
                                break;
                            case (int)KindMessages.KindPlayerMessages.Death:
                                {

                                }
                                break;
                            case (int)KindMessages.KindPlayerMessages.PlaceBomb:
                                {
                                    Player searchedPlayer = FindPlayer(idClient);
                                    if (searchedPlayer.CurrCountBombs != searchedPlayer.maxCountBombs)
                                    {
                                        gameCoreServer.bombs.Add(searchedPlayer.bombFactory.GetBomb(searchedPlayer.bombLevel, new Point(searchedPlayer.X, searchedPlayer.Y)));
                                        searchedPlayer.CurrCountBombs++;
                                    }
                                }
                                break;
                            case (int)KindMessages.KindPlayerMessages.Connect:
                                {
                                    gameCoreServer.players.Add(new Player(new Point(20, 20), gameCoreServer.playerTexture, gameCoreServer.playerSize, "", gameCoreServer.DeletePlayerFromField, gameCoreServer.bombTexture, gameCoreServer.bombSize, gameCoreServer.ExplosionBomb, idClient));
                                }
                                break;
                            case (int)KindMessages.KindPlayerMessages.Disconnect:
                                {
                                    gameCoreServer.players.Remove(FindPlayer(idClient));
                                }
                                break;
                            case (int)KindMessages.KindPlayerMessages.StopWalking:
                                {
                                    Player searchedPlayer = FindPlayer(idClient);
                                    searchedPlayer.isMoved = false;
                                }
                                break;
                        }

                    }
                    break;
                case (int)KindMessages.KindMessage.Wall:
                    {
                    }
                    break;
            }

        }
        public MessageAnalyzer(GameCoreServer gameCoreServer)
        {
            this.gameCoreServer = gameCoreServer;
        }
    }
}
