﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bomberman_client.GameClasses;
using System.Drawing;
using Bomberman_client;

public class MessageAnalyzerServer
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
        foreach (Player searchedPlayer in gameCoreServer.objectsList.players)
        {
            if (searchedPlayer.id == id)
            {
                return searchedPlayer;
            }
        }
        return null;
    }
    public void AnalyzeMessage(byte[] message, ref bool isDisconnected)
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

                                if (searchedPlayer.IsDead)
                                {
                                    int nextPos = gameCoreServer.randomGen.Next(gameCoreServer.spawnPoints.Count - 1);
                                    while (searchedPlayer.prevLocation == (nextPos = gameCoreServer.randomGen.Next(gameCoreServer.spawnPoints.Count - 1)))
                                    {
                                    }
                                    searchedPlayer.prevLocation = nextPos;
                                    Point newLocation = gameCoreServer.spawnPoints[nextPos];

                                    searchedPlayer.X = newLocation.X;
                                    searchedPlayer.Y = newLocation.Y;
                                    searchedPlayer.IsDying = false;
                                    searchedPlayer.IsDead = false;
                                }
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
                                    gameCoreServer.objectsList.bombs.Add(searchedPlayer.bombFactory.GetBomb(searchedPlayer.bombLevel, new Point(searchedPlayer.X, searchedPlayer.Y)));
                                    searchedPlayer.CurrCountBombs++;
                                }
                            }
                            break;
                        case (int)KindMessages.KindPlayerMessages.Connect:
                            {
                                //gameCoreServer.objectsList.players.Add(new Player(new Point(20, 20), gameCoreServer.playerSize, "", gameCoreServer.DeletePlayerFromField, gameCoreServer.bombTexture, gameCoreServer.bombSize, gameCoreServer.ExplosionBomb, idClient));
                            }
                            break;
                        case (int)KindMessages.KindPlayerMessages.Disconnect:
                            {

                                gameCoreServer.objectsList.players.Remove(FindPlayer(idClient));
                                isDisconnected = true;
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
    public MessageAnalyzerServer(GameCoreServer gameCoreServer)
    {
        this.gameCoreServer = gameCoreServer;
    }
}
