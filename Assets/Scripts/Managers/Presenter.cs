using System.Collections;
using UnityEngine;

namespace SnakesAndLadders {

    public class Presenter : MonoBehaviour
    {
        [SerializeField] private View view;
        private Model model;
        [SerializeField] private SnakesSO[] snakeData;
        [SerializeField] private LaddersSO[] ladderData;
        
        [HideInInspector] private GameConfigSO gameConfigData;

        //private void Start() {
        //    Setup(gameConfigData);
        //}
        public void Setup(GameConfigSO gameConfigData) {
            this.gameConfigData = gameConfigData;

            model = new Model(gameConfigData.numberOfBots + gameConfigData.numberOfHumans, snakeData, ladderData);
            view.Setup();

            
            model.OnTurnChange += Model_OnTurnChange;

            view.OnPlayerTurnClicked += ViewManager_OnPlayerTurnClicked;
            view.OnPlayerSet += View_OnPlayerSet;
            view.OnSnakeLadderInteractionComplete += View_OnSnakeLadderInteractionComplete;
            view.OnPlayerAbilityClicked += View_OnPlayerAbilityClicked;
            view.OnAbilityAnimationComplete += View_OnAbilityAnimationComplete;

            view.SetSnakePositions(model.GetAllSnakePositions());
            view.SetLadderPositions(model.GetAllLadderPositions());

            for (int playerIndex = 0; playerIndex < model.GetPlayers().Length; playerIndex++) {
                view.SetPlayerDirect(playerIndex, 0);
            }
        }

        private void View_OnAbilityAnimationComplete() {
            
        }

        private void View_OnPlayerAbilityClicked(int playerIndex) {
            model.UseAbility(playerIndex, "blast");
            // code for blast ability logic
            Player[] players = model.GetPlayers();
            view.BlastAbilityAnimation();
            for (int i = 0; i < players.Length; i++) {
                if(i != playerIndex) {
                    // other players. Checking if they are in the blast line
                    //Debug.Log(players[i].GetPosition() / 10 == players[playerIndex].GetPosition() / 10);
                    if (players[i].GetPosition() / 10 == players[playerIndex].GetPosition() / 10) {
                        model.SetPlayerPosition(i, 0);
                        view.SetPlayerDirect(i, 0);
                    }
                }
            }
            view.SetAbilityButton(playerIndex, false);
            model.IncrementTurn();
            StartCoroutine(GenerateDelayBetweenPlayerAndBot());
        }

        private void View_OnSnakeLadderInteractionComplete() {
            model.IncrementTurn();
            StartCoroutine(GenerateDelayBetweenPlayerAndBot());
        }

        private IEnumerator GenerateDelayBetweenPlayerAndBot() {
            WaitForSeconds wait = new WaitForSeconds(0.5f);
            yield return wait;
            int turn = model.GetTurn();
            if (gameConfigData.numberOfBots > 0 && turn != 0) {
                if (model.GetCanUseAbility(turn, "blast")) {
                    if (model.GetPlayerPosition(0) / 10 == model.GetPlayerPosition(turn) / 10) {
                        // Player on same row as bot
                        View_OnPlayerAbilityClicked(turn);
                    } else {
                        ViewManager_OnPlayerTurnClicked(turn);
                    }
                } else {
                        ViewManager_OnPlayerTurnClicked(turn);
                }
            }
        }

        private void View_OnPlayerSet() {
            // Logic for snakes & ladder testing.
            int activePlayer = model.GetTurn();
            int currentPosition = model.GetPlayerPosition(activePlayer);
            int newPosition = currentPosition;

            View.DisplayText displayTextImage = View.DisplayText.SnakeBit;

            if (model.HasSnake(currentPosition)) {
                newPosition = model.GetNewTargetTileAfterSnakeBite(currentPosition);
                displayTextImage = View.DisplayText.SnakeBit;
                // extra code for ability handling to be added
            } else if (model.HasLadder(currentPosition)) {
                newPosition = model.GetNewTargetTileAfterLadderClimb(currentPosition);
                displayTextImage = View.DisplayText.LadderClimbed;

                // extra code for ability handling to be added
                if (model.GetAbility(activePlayer, "blast") == null) {
                    int charges = 1;
                    model.AddAbility(activePlayer, "blast", charges);
                    //view.SetAbilityButton(activePlayer, true);
                    view.UnlockAbility(activePlayer);
                }
            }
            if (currentPosition != newPosition) {
                // snake bit or ladder climbed
                view.SnakeLadderInteraction(activePlayer, displayTextImage, currentPosition, newPosition);
                model.SetPlayerPosition(activePlayer, newPosition);
            } else {
                // no snake or ladder. There's a chance of other player being there.
                CheckForOtherPlayersAndSet();
                //- causing infinite loop because, it calls setplayer, which fires an event which has this function as callback. break that chain to solve it..
                model.IncrementTurn();
                StartCoroutine(GenerateDelayBetweenPlayerAndBot());
            }
        }

        private void Model_OnTurnChange(int turn) {
            view.DisableOtherDiceButtons(turn);
            view.SetSnakePositions(model.GetAllSnakePositions());
            view.SetLadderPositions(model.GetAllLadderPositions());
            view.UpdateAbilitiesState(model.GetAbilityStates("blast"));
        }

        private void ViewManager_OnPlayerTurnClicked(int playerIndex) {
            int newRandomNumber = model.RollDie();
            view.SetDiceSprite(playerIndex, newRandomNumber-1);

            int currentPosition = model.GetPlayerPosition(playerIndex);
            int targetPosition = currentPosition + newRandomNumber;


            if(targetPosition < 99) {
                SetPlayer(playerIndex, currentPosition, targetPosition);
            }
            else if(targetPosition == 100) {
                SetPlayer(playerIndex, targetPosition, targetPosition - 1);
                view.ShowGameOverScreen();
                //GameManager.Instance.
                //Time.timeScale = 0f;
            } else {
                model.IncrementTurn();
            }
        }

        private void CheckForOtherPlayersAndSet() {
            Player[] players = model.GetPlayers();
            int currentPlayerPosition;
            int otherPlayerPosition;
            for (int i = 0; i < players.Length; i++) {
                currentPlayerPosition = players[i].GetPosition();
                for (int j = i + 1; j < players.Length; j++) {
                    otherPlayerPosition = players[j].GetPosition();
                    if (currentPlayerPosition == otherPlayerPosition) {
                        // 2 players are on same tile
                        SetPlayer(i, currentPlayerPosition, currentPlayerPosition, "topleft");
                        SetPlayer(j, otherPlayerPosition, otherPlayerPosition, "bottomright");
                    } else {
                        SetPlayer(i, currentPlayerPosition, currentPlayerPosition, "center");
                        SetPlayer(j, otherPlayerPosition, otherPlayerPosition, "center");
                    }
                }
            }
        }

        private void SetPlayer(int playerIndex, int currentPosition, int targetPosition, string anchor = "center") {
            //Debug.Log("Target Position -> " + targetPosition);
            if (currentPosition == targetPosition) {
                view.SetPlayerAnchor(playerIndex, currentPosition, anchor);
            } else { 
                view.SetPlayerOn(playerIndex, currentPosition, targetPosition);
                model.SetPlayerPosition(playerIndex, targetPosition);
            }
        }

        public GameConfigSO GetGameConfigSO() {
            return gameConfigData;
        }
    }

}