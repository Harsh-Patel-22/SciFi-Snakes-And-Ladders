using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SnakesAndLadders {
    public class Model{

        public event Action<int> OnTurnChange;

        private int maxPlayers;
        private Player[] players;
        private int turn;

        private bool[] dirtiedTiles;

        public class SnakeData {
            public int Head;
            public int Tail;

            public SnakeData(int head, int tail) {
                Head = head;
                Tail = tail;
            }
        }
        
        public class LadderData {
            public int Start;
            public int End;

            public LadderData(int start, int end) {
                Start = start;
                End = end;
            }
        }

        private List<SnakeData> snakeData;
        private List<LadderData> ladderData;

        public Model(int maxPlayers, SnakesSO[] snakeData, LaddersSO[] ladderData) {
            this.maxPlayers = maxPlayers;
            players = new Player[maxPlayers];
            dirtiedTiles = new bool[100];

            for (int i = 0; i < dirtiedTiles.Length; i++) {
                dirtiedTiles[i] = false;
            }

            int startPosition = 0;
           
            for (int i = 0; i < maxPlayers; i++) {
                players[i] = new Player(startPosition);
            }

            this.snakeData = new List<SnakeData>();
            this.ladderData = new List<LadderData>();

            for(int i = 0; i < snakeData.Length; i++) {
                int head = snakeData[i].Head;
                int tail = snakeData[i].Tail;
                this.snakeData.Add(new SnakeData(head, tail));
                dirtiedTiles[head] = true;
                dirtiedTiles[tail] = true;
            }
            for(int i = 0; i < ladderData.Length; i++) {
                int start = ladderData[i].Start;
                int end = ladderData[i].End;
                this.ladderData.Add(new LadderData(start, end));
                dirtiedTiles[start] = true;
                dirtiedTiles[end] = true;
            }
        }

        public int RollDie() {
            return UnityEngine.Random.Range(1, 7);
            //return 100;
        }

        // Code related to snakes & ladders
        private SnakeData GetSnake(int snakeHead) {
            for (int i = 0; i < snakeData.Count; i++) {
                if (snakeData[i].Head == snakeHead) {
                    return snakeData[i];
                }
            }
            return null;
        }

        public bool HasSnake(int tileNumber) {
            SnakeData snakesSO = GetSnake(tileNumber);
            if (snakesSO != null) {
                return true;
            }
            return false;
        }

        public int GetNewTargetTileAfterSnakeBite(int tileNumber) {
            SnakeData snakeSO = GetSnake(tileNumber);
            return snakeSO.Tail;
        }
        
        private LadderData GetLadder(int ladderStart) {
            for (int i = 0; i < ladderData.Count; i++) {
                if (ladderData[i].Start == ladderStart) {
                    return ladderData[i];
                }
            }
            return null;
        }

        public bool HasLadder(int tileNumber) {
            LadderData ladderSO = GetLadder(tileNumber);
            if (ladderSO != null) {
                return true;
            }
            return false;
        }

        public int GetNewTargetTileAfterLadderClimb(int tileNumber) {
            LadderData ladderSO = GetLadder(tileNumber);
            return ladderSO.End;
        }

        public List<int> GetAllSnakePositions() {
            // cache this as a field
            List<int> positions = new List<int>();
            foreach (SnakeData snake in snakeData) {
                positions.Add(snake.Head);
            }
            return positions;
        }
        public List<int> GetAllLadderPositions() {
            // cache this as a field
            List<int> positions = new List<int>();
            
            foreach (LadderData ladder in ladderData) {
                positions.Add(ladder.Start);
            }
            return positions;
        }

        private void MoveSnakes() {
            for (int i = 0; i < snakeData.Count; i++) {
                int currentHead = snakeData[i].Head;
                int currentTail = snakeData[i].Tail;
                int targetHead = currentHead + UnityEngine.Random.Range(-1, 10);

                
                if (targetHead >= 100) {
                    targetHead = UnityEngine.Random.Range(currentTail, 100);
                }
                
                
                while (dirtiedTiles[targetHead]) {
                    targetHead += UnityEngine.Random.Range(-1, 10);
                    if (targetHead >= 100) {
                        Debug.Log("BOUNDS BROKE");
                        targetHead = UnityEngine.Random.Range(currentTail, 100);
                    }
                }
                snakeData[i].Head = targetHead;
                dirtiedTiles[currentHead] = false;
                dirtiedTiles[targetHead] = true;



                int targetTail = currentTail + UnityEngine.Random.Range(-10, 1);
                if (targetTail < 0) {
                    targetTail = UnityEngine.Random.Range(0, targetHead);
                }
                while (dirtiedTiles[targetTail]) {
                    targetTail += UnityEngine.Random.Range(-10, 1);
                    if (targetTail < 0) {
                        targetTail = UnityEngine.Random.Range(0, targetHead);
                    }
                }

                snakeData[i].Tail = targetTail;

                dirtiedTiles[currentTail] = false;
                dirtiedTiles[targetTail] = true;

                //Debug.Log(targetHead);
                //Debug.Log(targetTail);
                
            }
        }

        private void ShrinkLadders() {
            for (int i = 0; i < ladderData.Count; i++) {
                
                int currentEnd = ladderData[i].End;
                int targetEnd = ladderData[i].End - 2;
                int start = ladderData[i].Start;


                while (dirtiedTiles[targetEnd]) {
                    Debug.Log("shrinking loop");
                    targetEnd -= 1;
                    if (targetEnd / 10 <= start / 10) {
                        targetEnd = UnityEngine.Random.Range(start, start + 10);
                    }
                }

                dirtiedTiles[currentEnd] = false;
                dirtiedTiles[targetEnd] = true;
                ladderData[i].End = targetEnd;
                if (targetEnd / 10 <= start / 10) {
                    ladderData.Remove(ladderData[i]);
                    Debug.Log("Ladder Destroyed!!");
                }
            }
        }

        // Code related to player 
        public int GetPlayerPosition(int playerIndex) {
            return players[playerIndex].GetPosition();
        }

        public Player[] GetPlayers() {
            return players;
        }

        public void SetPlayerPosition(int playerIndex, int position) {
            players[playerIndex].SetPosition(position);
        }

        public void AddAbility(int playerIndex, string abilityName, int charges) {
            players[playerIndex].AddAbility(abilityName, charges);
        }

        public Ability GetAbility(int playerIndex, string abilityName) {
            return players[playerIndex].GetAbility(abilityName);
        }

        public bool[] GetAbilityStates(string abilityName) {
            bool[] states = new bool[players.Length];
            for (int i = 0; i < players.Length; i++) {
                Ability ability = players[i].GetAbility(abilityName);
                if (ability != null) {
                    if (ability.Charges > 0 && turn == i) {
                        states[i] = true;
                    } else {
                        states[i] = false;
                    }
                }
            }
            return states;
        }

        public bool GetCanUseAbility(int playerIndex, string abilityName) {
            Ability ability = players[playerIndex].GetAbility(abilityName);
            if (ability == null) {
                return false; 
            }

            if(ability.Charges > 0) {
                return true;
            }
            return false;
            
        }

        public void UseAbility(int playerIndex, string abilityName) {
            players[playerIndex].UseAbility(abilityName);
        }

        // Code related to turn
        public void IncrementTurn() {
            turn = (turn + 1) % maxPlayers;
            if(turn == 0) {
                MoveSnakes();
                ShrinkLadders();

                //PrintDirtiedTiles();
            }
            OnTurnChange?.Invoke(turn);
        }

        private void PrintDirtiedTiles() {
            for (int i = 0; i < dirtiedTiles.Length; i++) {
                if (dirtiedTiles[i]) {
                    Debug.Log(i);
                }
            }
        }

        public int GetTurn() {
            return turn;
        }
    }
}